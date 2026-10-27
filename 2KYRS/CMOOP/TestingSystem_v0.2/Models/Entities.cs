namespace TestingSystem.Models;

// Сущности — простые POCO без привязки к UI и конкретной БД. Связи выражены Id-полями (как внешние ключи)
// и вложенными коллекциями (Test.Questions, Question.Answers, Result.Details) — для EF Core это
// стандартные навигационные свойства.

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Group Clone() => (Group)MemberwiseClone();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Category Clone() => (Category)MemberwiseClone();
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    /// <summary>PBKDF2 + соль. Пароль в открытом виде не хранится.</summary>
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? GroupId { get; set; }
    public Role Role { get; set; }
    public string? AvatarPath { get; set; }
    public DateTime RegistrationDate { get; set; }

    public User Clone() => (User)MemberwiseClone();
}

public class Test
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorTeacherId { get; set; }
    public int? CategoryId { get; set; }
    /// <summary>0 — без ограничения времени.</summary>
    public int TimeLimitMinutes { get; set; }
    public double PassScorePercent { get; set; } = 60;
    /// <summary>0 — без ограничения числа попыток.</summary>
    public int MaxAttempts { get; set; }
    public bool IsRandomizeQuestions { get; set; }
    public bool IsRandomizeAnswers { get; set; }
    /// <summary>Пин-код доступа; пусто — тест открыт.</summary>
    public string? AccessCode { get; set; }
    /// <summary>Группы, которым назначен тест; пусто — доступен всем ученикам.</summary>
    public List<int> AssignedGroupIds { get; set; } = new();
    /// <summary>Расширение: разрешено ли ученику видеть правильные ответы и пояснения после сдачи.</summary>
    public bool AllowShowExplanations { get; set; } = true;
    public List<Question> Questions { get; set; } = new();

    public Test DeepClone()
    {
        var c = (Test)MemberwiseClone();
        c.AssignedGroupIds = new List<int>(AssignedGroupIds);
        c.Questions = Questions.Select(q => q.DeepClone()).ToList();
        return c;
    }
}

public class Question
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int? CategoryId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public TextEvaluationType TextEvaluationType { get; set; } = TextEvaluationType.AutoExactMatch;
    public double MaxScore { get; set; } = 1;
    public string Explanation { get; set; } = string.Empty;
    public string? MediaPath { get; set; }
    public List<Answer> Answers { get; set; } = new();

    public Question DeepClone()
    {
        var c = (Question)MemberwiseClone();
        c.Answers = Answers.Select(a => a.Clone()).ToList();
        return c;
    }
}

/// <summary>
/// Универсальный вариант ответа; смысл полей зависит от типа вопроса:
///  Single/Multiple — Text + IsCorrect;
///  TextAnswer     — каждая запись с IsCorrect=true — допустимый ответ;
///  Matching       — Text (левый элемент) + CorrectPairText (правый), OrderIndex — порядок слева;
///  Sequence       — Text + OrderIndex (правильная позиция).
/// </summary>
public class Answer
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CorrectPairText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsCorrect { get; set; }

    public Answer Clone() => (Answer)MemberwiseClone();
}

public class TestSession
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public ResultStatus Status { get; set; } = ResultStatus.InProgress;
    /// <summary>Сколько раз ученик терял фокус окна / сворачивал его во время теста.</summary>
    public int ViolationsCount { get; set; }

    public TestSession Clone() => (TestSession)MemberwiseClone();
}

public class Result
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public double TotalScore { get; set; }
    public double MaxScore { get; set; }
    public double Percentage { get; set; }
    public bool IsPassed { get; set; }
    public ResultStatus Status { get; set; }
    public DateTime Date { get; set; }
    public string? TeacherComment { get; set; }
    /// <summary>Расширение: кто проверил работу вручную (для статистики учителя).</summary>
    public int? ReviewedByTeacherId { get; set; }
    public List<StudentAnswerDetail> Details { get; set; } = new();

    public Result DeepClone()
    {
        var c = (Result)MemberwiseClone();
        c.Details = Details.Select(d => d.Clone()).ToList();
        return c;
    }
}

/// <summary>
/// Ответ ученика на один вопрос. SelectedAnswerIds — упорядоченный список:
///  Single/Multiple — выбранные варианты;
///  Sequence        — Id в порядке, расставленном учеником;
///  Matching        — для каждого левого элемента (по OrderIndex) Id варианта, чья правая часть выбрана (0 — не выбрано).
/// Поля-снимки (QuestionText ... Explanation) фиксируют вопрос на момент сдачи: разбор ошибок работает,
/// даже если учитель позже изменит или удалит тест (в SQL — денормализованные колонки).
/// </summary>
public class StudentAnswerDetail
{
    public int Id { get; set; }
    public int ResultId { get; set; }
    public int QuestionId { get; set; }
    public string GivenTextAnswer { get; set; } = string.Empty;
    public List<int> SelectedAnswerIds { get; set; } = new();
    public string? UploadedFilePath { get; set; }
    public double ScoreAwarded { get; set; }
    /// <summary>false — требуется ручная проверка учителем.</summary>
    public bool IsGraded { get; set; }
    public string? TeacherNote { get; set; }

    // --- снимок вопроса ---
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public double MaxScore { get; set; }
    public string GivenSummary { get; set; } = string.Empty;
    public string CorrectSummary { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public StudentAnswerDetail Clone()
    {
        var c = (StudentAnswerDetail)MemberwiseClone();
        c.SelectedAnswerIds = new List<int>(SelectedAnswerIds);
        return c;
    }
}
