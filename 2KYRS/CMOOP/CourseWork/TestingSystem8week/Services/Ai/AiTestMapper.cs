using TestingSystem.Models;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Перетворює JSON від AI на доменний тест. Нічого не довіряємо: кожне питання проходить ті самі правила,
/// що й у ручному конструкторі; некоректні відкидаються (їх кількість повертається), тексти обрізаються.
/// </summary>
internal static class AiTestMapper
{
    public const int MaxText = 1500;

    public static (Test Test, int Skipped) ToTest(TestDto dto, int authorId, string fallbackTitle, IReadOnlyCollection<QuestionType> allowed)
    {
        var test = new Test
        {
            AuthorTeacherId = authorId,
            Title = Clean(dto.Title, 200) is { Length: > 0 } t ? t : fallbackTitle,
            Description = Clean(dto.Description, 500),
            PassScorePercent = 60,
            AllowShowExplanations = true
        };

        int skipped = 0;
        foreach (var q in dto.Questions ?? new())
        {
            var mapped = MapQuestion(q, allowed);
            if (mapped is null) skipped++;
            else test.Questions.Add(mapped);
        }
        return (test, skipped);
    }

    private static Question? MapQuestion(QuestionDto q, IReadOnlyCollection<QuestionType> allowed)
    {
        if (!Enum.TryParse<QuestionType>(q.Type?.Trim(), ignoreCase: true, out var type) || !allowed.Contains(type)) return null;
        if (type == QuestionType.FileUpload) return null;

        string text = Clean(q.Text, MaxText);
        if (text.Length == 0) return null;

        var question = new Question
        {
            Text = text,
            QuestionType = type,
            Explanation = Clean(q.Explanation, MaxText),
            MaxScore = Math.Clamp(q.MaxScore is > 0 ? q.MaxScore.Value : 1, 0.5, 10)
        };

        switch (type)
        {
            case QuestionType.SingleChoice:
            case QuestionType.MultipleChoice:
            {
                var answers = (q.Answers ?? new())
                    .Select(a => (Text: Clean(a.Text, 500), a.IsCorrect)).Where(a => a.Text.Length > 0).Take(8).ToList();
                int correct = answers.Count(a => a.IsCorrect);
                if (answers.Count < 2) return null;
                if (type == QuestionType.SingleChoice && correct != 1) return null;    // рівно одна правильна
                if (type == QuestionType.MultipleChoice && correct < 1) return null;
                question.Answers = answers.Select((a, i) => new Answer { Text = a.Text, IsCorrect = a.IsCorrect, OrderIndex = i }).ToList();
                break;
            }

            case QuestionType.TextAnswer:
            {
                question.TextEvaluationType = Enum.TryParse<TextEvaluationType>(q.TextEvaluation?.Trim(), true, out var ev)
                    ? ev : TextEvaluationType.ManualTeacherReview;
                if (question.TextEvaluationType != TextEvaluationType.ManualTeacherReview)
                {
                    var accepted = (q.AcceptedAnswers ?? new()).Select(a => Clean(a, 300)).Where(a => a.Length > 0).Distinct().Take(8).ToList();
                    if (accepted.Count == 0) question.TextEvaluationType = TextEvaluationType.ManualTeacherReview;   // без еталонів — ручна перевірка
                    else question.Answers = accepted.Select((a, i) => new Answer { Text = a, IsCorrect = true, OrderIndex = i }).ToList();
                }
                break;
            }

            case QuestionType.Matching:
            {
                var pairs = (q.Pairs ?? new())
                    .Select(p => (Left: Clean(p.Left, 300), Right: Clean(p.Right, 300)))
                    .Where(p => p.Left.Length > 0 && p.Right.Length > 0).Take(8).ToList();
                if (pairs.Count < 2) return null;
                if (pairs.Select(p => p.Right.ToLowerInvariant()).Distinct().Count() != pairs.Count) return null;   // праві частини унікальні
                question.Answers = pairs.Select((p, i) => new Answer { Text = p.Left, CorrectPairText = p.Right, OrderIndex = i }).ToList();
                break;
            }

            case QuestionType.Sequence:
            {
                var items = (q.Items ?? new()).Select(i => Clean(i, 300)).Where(i => i.Length > 0).Take(8).ToList();
                if (items.Count < 2) return null;
                question.Answers = items.Select((t, i) => new Answer { Text = t, OrderIndex = i }).ToList();
                break;
            }
        }
        return question;
    }

    public static string Clean(string? value, int max)
    {
        string s = (value ?? string.Empty).Trim();
        return s.Length <= max ? s : s[..max];
    }
}
