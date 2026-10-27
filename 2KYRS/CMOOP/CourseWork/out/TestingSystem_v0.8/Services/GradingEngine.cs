using System.IO;
using TestingSystem.Models;

namespace TestingSystem.Services;

/// <summary>
/// Движок оценки. Автоматически проверяет все типы, кроме «ручных» (FileUpload и TextAnswer с
/// ManualTeacherReview): они получают IsGraded=false, а работа — статус PendingTeacherReview.
/// Не зависит ни от UI, ни от хранилища — его легко покрыть unit-тестами.
/// </summary>
public class GradingEngine
{
    private const double Eps = 1e-9;

    /// <summary>
    /// Нормализация текстового ответа: .Trim() убирает пробелы по краям, нижний регистр делает сравнение
    /// нечувствительным к регистру. Используем ToLowerInvariant — это .ToLower() без зависимости от
    /// региональных настроек ПК (иначе, например, в турецкой культуре "I" превратится в "ı").
    /// </summary>
    public static string Normalize(string? input) => (input ?? string.Empty).Trim().ToLowerInvariant();

    // ============================== оценка одного ответа ==============================

    public void GradeDetail(Question q, StudentAnswerDetail d)
    {
        // снимок вопроса (нужен для разбора ошибок и аналитики)
        d.QuestionId = q.Id;
        d.QuestionText = q.Text;
        d.QuestionType = q.QuestionType;
        d.MaxScore = q.MaxScore;
        d.Explanation = q.Explanation;
        d.CorrectSummary = DescribeCorrect(q);
        d.GivenSummary = DescribeGiven(q, d);

        switch (q.QuestionType)
        {
            case QuestionType.SingleChoice:
                Set(d, GradeSingle(q, d));
                break;
            case QuestionType.MultipleChoice:
                Set(d, GradeMultiple(q, d));
                break;
            case QuestionType.Matching:
                Set(d, GradeMatching(q, d));
                break;
            case QuestionType.Sequence:
                Set(d, GradeSequence(q, d));
                break;
            case QuestionType.TextAnswer when q.TextEvaluationType == TextEvaluationType.ManualTeacherReview:
                MarkManual(d, hasContent: !string.IsNullOrWhiteSpace(d.GivenTextAnswer));
                break;
            case QuestionType.TextAnswer:
                Set(d, GradeText(q, d));
                break;
            case QuestionType.FileUpload:
                MarkManual(d, hasContent: !string.IsNullOrWhiteSpace(d.UploadedFilePath));
                break;
        }

        void Set(StudentAnswerDetail x, double score) { x.ScoreAwarded = Math.Round(score, 2); x.IsGraded = true; }
    }

    /// <summary>Пустой ответ на «ручной» вопрос проверять не нужно (0 баллов); непустой уходит учителю.</summary>
    private static void MarkManual(StudentAnswerDetail d, bool hasContent)
    {
        d.ScoreAwarded = 0;
        d.IsGraded = !hasContent;
    }

    // SingleChoice (RadioButton): точное совпадение — выбран ровно один и он правильный
    private static double GradeSingle(Question q, StudentAnswerDetail d)
    {
        if (d.SelectedAnswerIds.Count != 1) return 0;
        var chosen = q.Answers.FirstOrDefault(a => a.Id == d.SelectedAnswerIds[0]);
        return chosen is { IsCorrect: true } ? q.MaxScore : 0;
    }

    // MultipleChoice (CheckBox): (верно выбранные − неверно выбранные) / всего верных, не ниже 0.
    // Все верные и ни одного лишнего = полный балл, часть верных = частичный балл.
    private static double GradeMultiple(Question q, StudentAnswerDetail d)
    {
        var correct = q.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();
        if (correct.Count == 0) return 0;

        var selected = d.SelectedAnswerIds.Distinct().ToList();
        int right = selected.Count(correct.Contains);
        int wrong = selected.Count - right;
        return q.MaxScore * Math.Max(0, (right - wrong) / (double)correct.Count);
    }

    // TextAnswer (AutoExactMatch / AutoContains): сравнение нормализованных строк
    private static double GradeText(Question q, StudentAnswerDetail d)
    {
        string given = Normalize(d.GivenTextAnswer);
        if (given.Length == 0) return 0;

        var accepted = q.Answers.Where(a => a.IsCorrect).Select(a => Normalize(a.Text)).Where(s => s.Length > 0).ToList();

        bool ok = q.TextEvaluationType == TextEvaluationType.AutoContains
            ? accepted.Any(a => given.Contains(a))     // ответ СОДЕРЖИТ допустимую подстроку
            : accepted.Contains(given);                // ответ СОВПАДАЕТ с одним из допустимых
        return ok ? q.MaxScore : 0;
    }

    // Matching: по баллу за каждую верную пару
    private static double GradeMatching(Question q, StudentAnswerDetail d)
    {
        var left = q.Answers.OrderBy(a => a.OrderIndex).ToList();
        if (left.Count == 0) return 0;

        int right = 0;
        for (int i = 0; i < left.Count && i < d.SelectedAnswerIds.Count; i++)
            if (d.SelectedAnswerIds[i] == left[i].Id) right++;   // выбрана правая часть именно этого варианта
        return q.MaxScore * right / left.Count;
    }

    // Sequence: полный балл только за полностью верный порядок
    private static double GradeSequence(Question q, StudentAnswerDetail d)
    {
        var expected = q.Answers.OrderBy(a => a.OrderIndex).Select(a => a.Id).ToList();
        return expected.Count > 0 && expected.SequenceEqual(d.SelectedAnswerIds) ? q.MaxScore : 0;
    }

    // ============================== результат попытки ==============================

    /// <summary>Строит результат: оценивает ответы на все вопросы теста (неотвеченные = 0 баллов).</summary>
    public Result BuildResult(TestSession session, Test test, IReadOnlyDictionary<int, StudentAnswerDetail> given, bool timedOut)
    {
        var result = new Result
        {
            SessionId = session.Id, StudentId = session.StudentId, TestId = test.Id, Date = DateTime.Now
        };

        foreach (var q in test.Questions)
        {
            var d = given.TryGetValue(q.Id, out var g) ? g.Clone() : new StudentAnswerDetail { QuestionId = q.Id };
            GradeDetail(q, d);
            result.Details.Add(d);
        }

        Recalculate(result, test);

        bool pending = result.Details.Any(x => !x.IsGraded);
        result.Status = pending ? ResultStatus.PendingTeacherReview
                      : timedOut ? ResultStatus.Expired
                      : ResultStatus.Completed;
        result.IsPassed = !pending && result.Percentage >= test.PassScorePercent;
        return result;
    }

    /// <summary>Учитель выставляет балл за «ручной» ответ (балл ограничивается диапазоном 0..MaxScore).</summary>
    public void ApplyManualScore(StudentAnswerDetail d, double score, string? note)
    {
        d.ScoreAwarded = Math.Round(Math.Clamp(score, 0, d.MaxScore), 2);
        d.TeacherNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        d.IsGraded = true;
    }

    /// <summary>Завершение проверки: PendingTeacherReview -> Completed, пересчёт итогов и признака «сдал».</summary>
    public void FinalizeReview(Result result, Test test, int teacherId)
    {
        Recalculate(result, test);
        result.Status = ResultStatus.Completed;
        result.IsPassed = result.Percentage >= test.PassScorePercent;
        result.ReviewedByTeacherId = teacherId;
    }

    private static void Recalculate(Result r, Test test)
    {
        r.TotalScore = Math.Round(r.Details.Sum(x => x.ScoreAwarded), 2);
        r.MaxScore = Math.Round(r.Details.Sum(x => x.MaxScore), 2);
        r.Percentage = r.MaxScore > Eps ? Math.Round(100 * r.TotalScore / r.MaxScore, 1) : 0;
    }

    // ============================== текстовые описания ==============================

    private static IEnumerable<Answer> Ordered(Question q) => q.Answers.OrderBy(a => a.OrderIndex);

    private static string DescribeCorrect(Question q) => q.QuestionType switch
    {
        QuestionType.SingleChoice or QuestionType.MultipleChoice
            => string.Join("; ", q.Answers.Where(a => a.IsCorrect).Select(a => a.Text)),
        QuestionType.TextAnswer when q.TextEvaluationType == TextEvaluationType.ManualTeacherReview => string.Empty,
        QuestionType.TextAnswer => string.Join(" / ", q.Answers.Where(a => a.IsCorrect).Select(a => a.Text)),
        QuestionType.Matching => string.Join("; ", Ordered(q).Select(a => $"{a.Text} → {a.CorrectPairText}")),
        QuestionType.Sequence => string.Join(" → ", Ordered(q).Select(a => a.Text)),
        _ => string.Empty
    };

    private static string DescribeGiven(Question q, StudentAnswerDetail d)
    {
        string TextOf(int id) => q.Answers.FirstOrDefault(a => a.Id == id)?.Text ?? "?";

        return q.QuestionType switch
        {
            QuestionType.SingleChoice or QuestionType.MultipleChoice
                => string.Join("; ", d.SelectedAnswerIds.Select(TextOf)),
            QuestionType.TextAnswer => d.GivenTextAnswer,
            QuestionType.Matching => string.Join("; ", Ordered(q).Select((a, i) =>
                $"{a.Text} → {(i < d.SelectedAnswerIds.Count ? q.Answers.FirstOrDefault(x => x.Id == d.SelectedAnswerIds[i])?.CorrectPairText : null) ?? "—"}")),
            QuestionType.Sequence => string.Join(" → ", d.SelectedAnswerIds.Select(TextOf)),
            QuestionType.FileUpload => string.IsNullOrWhiteSpace(d.UploadedFilePath) ? string.Empty : Path.GetFileName(d.UploadedFilePath),
            _ => string.Empty
        };
    }
}

/// <summary>Есть ли в ответе содержимое (используется для статуса «отвечено» в навигации).</summary>
public static class AnswerState
{
    public static bool HasAnswer(StudentAnswerDetail? d)
        => d != null && (d.SelectedAnswerIds.Any(id => id > 0)
                         || !string.IsNullOrWhiteSpace(d.GivenTextAnswer)
                         || !string.IsNullOrWhiteSpace(d.UploadedFilePath));
}
