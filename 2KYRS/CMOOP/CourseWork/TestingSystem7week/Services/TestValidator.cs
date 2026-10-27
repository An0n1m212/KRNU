using TestingSystem.Models;

namespace TestingSystem.Services;

/// <summary>
/// Тиждень 3: валідація конструктора (ТЗ п.3.3.4).
/// Ті самі правила у фіналі перевіряють `Views/TestEditorWindow` і `Services/Ai/AiTestMapper`
/// (биті AI-питання відкидаються з лічильником): текст питання, макс. бал &gt; 0,
/// Single — ≥2 варіанти і рівно 1 правильний; Multiple — ≥2 і ≥1 правильний;
/// Text — ≥1 допустима відповідь; Matching — ≥2 пари й унікальність правих частин;
/// Sequence — ≥2 елементи; FileUpload — відповіді не потрібні.
/// </summary>
public static class TestValidator
{
    public static bool TryValidate(Test test, out string error)
    {
        if (string.IsNullOrWhiteSpace(test.Title)) { error = "Назва тесту порожня."; return false; }
        if (test.PassScorePercent is < 0 or > 100) { error = "Прохідний бал має бути 0–100%."; return false; }
        if (test.TimeLimitMinutes < 0) { error = "Ліміт часу не може бути від'ємним."; return false; }
        if (test.MaxAttempts < 0) { error = "Число спроб не може бути від'ємним."; return false; }
        foreach (var q in test.Questions)
        {
            if (!TryValidateQuestion(q, out error)) return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool TryValidateQuestion(Question q, out string error)
    {
        if (string.IsNullOrWhiteSpace(q.Text)) { error = "Текст питання порожній."; return false; }
        if (q.MaxScore <= 0) { error = $"Питання «{Trim(q.Text)}»: макс. бал має бути > 0."; return false; }
        switch (q.QuestionType)
        {
            case QuestionType.SingleChoice:
                if (q.Answers.Count < 2) { error = $"«{Trim(q.Text)}»: потрібно ≥2 варіанти."; return false; }
                if (q.Answers.Count(a => a.IsCorrect) != 1) { error = $"«{Trim(q.Text)}»: має бути рівно 1 правильний варіант."; return false; }
                break;
            case QuestionType.MultipleChoice:
                if (q.Answers.Count < 2) { error = $"«{Trim(q.Text)}»: потрібно ≥2 варіанти."; return false; }
                if (!q.Answers.Any(a => a.IsCorrect)) { error = $"«{Trim(q.Text)}»: потрібен хоч 1 правильний варіант."; return false; }
                break;
            case QuestionType.TextAnswer:
                if (!q.Answers.Any(a => a.IsCorrect && !string.IsNullOrWhiteSpace(a.Text))) { error = $"«{Trim(q.Text)}»: потрібна хоч 1 допустима відповідь."; return false; }
                break;
            case QuestionType.Matching:
                if (q.Answers.Count < 2) { error = $"«{Trim(q.Text)}»: потрібно ≥2 пари."; return false; }
                if (q.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text) || string.IsNullOrWhiteSpace(a.CorrectPairText))) { error = $"«{Trim(q.Text)}»: у кожної пари мають бути ліва і права частини."; return false; }
                if (q.Answers.Select(a => a.CorrectPairText.Trim().ToLowerInvariant()).Distinct().Count() != q.Answers.Count) { error = $"«{Trim(q.Text)}»: праві частини пар мають бути унікальними."; return false; }
                break;
            case QuestionType.Sequence:
                if (q.Answers.Count < 2) { error = $"«{Trim(q.Text)}»: потрібно ≥2 елементи."; return false; }
                if (q.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text))) { error = $"«{Trim(q.Text)}»: усі елементи мають текст."; return false; }
                break;
            case QuestionType.FileUpload:
                break;
        }
        if (q.Answers.Any(a => a.Text.Length > 500)) { error = $"«{Trim(q.Text)}»: текст варіанта задовгий."; return false; }
        error = string.Empty;
        return true;
    }

    private static string Trim(string s) => s.Length <= 40 ? s : s[..40] + "…";
}
