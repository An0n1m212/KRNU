namespace TestingSystem.Models;

/// <summary>Плоская строка для экспорта результатов (CSV/Excel/PDF).</summary>
public record ResultExportRow(
    string Student, string Group, string Test, double Score, double MaxScore,
    double Percent, bool Passed, ResultStatus Status, DateTime Date, int Violations);

/// <summary>Сложность вопроса: доля попыток, где ответ не получил полный балл.</summary>
public record QuestionDifficulty(string TestTitle, string QuestionText, int Attempts, double WrongRatePercent, bool IsHard);

public record CertificateData(string StudentName, string TestTitle, double Percent, DateTime Date, string Number);

public record ReportData(string Title, string FilterDescription, IReadOnlyList<ResultExportRow> Rows, IReadOnlyList<QuestionDifficulty> HardQuestions);
