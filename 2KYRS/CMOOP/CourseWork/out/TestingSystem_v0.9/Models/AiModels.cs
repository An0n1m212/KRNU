namespace TestingSystem.Models;

/// <summary>Запропонована AI тема лекції.</summary>
public sealed record LectureTopicSuggestion(string Title, string Description);

/// <summary>Лекція, створена AI (ще НЕ збережена — викладач перевіряє її в редакторі).</summary>
public sealed record GeneratedLecture(string Title, string Summary, string Content);

/// <summary>Тест, створений AI. SkippedQuestions — скільки питань відхилено перевіркою (некоректна структура).</summary>
public sealed record GeneratedTestResult(Test Test, int SkippedQuestions);

/// <summary>Порада AI щодо оцінки відкритої відповіді. Остаточне рішення завжди за викладачем.</summary>
public sealed record AiGradeSuggestion(double Score, string Comment);
