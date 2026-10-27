using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services;

public record StudentStats(int TestsCompleted, double AveragePercent, int Certificates);
public record TeacherStats(int TestsCreated, int WorksReviewed, int PendingReview, double AveragePercent);
public record GroupStat(string GroupName, int Students, int Attempts, double AveragePercent, double PassRatePercent);
public record ProgressPoint(DateTime Date, double Percent);

/// <summary>Расчёт статистики и аналитики. Работает только через интерфейсы репозиториев.</summary>
public class StatisticsService
{
    public const double HardThresholdPercent = 50;

    private readonly ITestRepository _tests;
    private readonly IResultRepository _results;
    private readonly IUserRepository _users;
    private readonly IGroupRepository _groups;

    public StatisticsService(ITestRepository tests, IResultRepository results, IUserRepository users, IGroupRepository groups)
    {
        _tests = tests;
        _results = results;
        _users = users;
        _groups = groups;
    }

    private static bool IsFinal(Result r) => r.Status != ResultStatus.PendingTeacherReview && r.Status != ResultStatus.InProgress;

    public StudentStats GetStudentStats(int studentId)
    {
        var done = _results.GetByStudent(studentId).Where(IsFinal).ToList();
        return new StudentStats(done.Count, done.Count == 0 ? 0 : done.Average(r => r.Percentage), done.Count(r => r.IsPassed));
    }

    /// <summary>Динамика успеваемости ученика (по возрастанию даты).</summary>
    public IReadOnlyList<ProgressPoint> GetStudentProgress(int studentId)
        => _results.GetByStudent(studentId).Where(IsFinal).OrderBy(r => r.Date)
            .Select(r => new ProgressPoint(r.Date, r.Percentage)).ToList();

    private IEnumerable<Test> TestsOf(User teacher)
        => _tests.GetAll().Where(t => teacher.Role == Role.Admin || t.AuthorTeacherId == teacher.Id);

    public TeacherStats GetTeacherStats(User teacher)
    {
        var tests = TestsOf(teacher).ToList();
        var results = tests.SelectMany(t => _results.GetByTest(t.Id)).ToList();
        var final = results.Where(IsFinal).ToList();

        return new TeacherStats(
            tests.Count,
            results.Count(r => r.ReviewedByTeacherId == teacher.Id),
            results.Count(r => r.Status == ResultStatus.PendingTeacherReview),
            final.Count == 0 ? 0 : final.Average(r => r.Percentage));
    }

    /// <summary>Статистика по группам учеников, проходивших тесты данного учителя.</summary>
    public IReadOnlyList<GroupStat> GetGroupStats(User teacher)
    {
        var results = TestsOf(teacher).SelectMany(t => _results.GetByTest(t.Id)).Where(IsFinal).ToList();
        var students = _users.GetStudents().ToDictionary(u => u.Id);
        var groups = _groups.GetAll().ToDictionary(g => g.Id, g => g.Name);

        return results
            .Where(r => students.ContainsKey(r.StudentId))
            .GroupBy(r => students[r.StudentId].GroupId)
            .Select(g => new GroupStat(
                g.Key is int id && groups.TryGetValue(id, out var name) ? name : "—",
                g.Select(r => r.StudentId).Distinct().Count(),
                g.Count(),
                g.Average(r => r.Percentage),
                100.0 * g.Count(r => r.IsPassed) / g.Count()))
            .OrderBy(s => s.GroupName).ToList();
    }

    /// <summary>
    /// Сложность вопросов: какая доля оценённых ответов НЕ получила полный балл.
    /// Вопросы, где ошибаются чаще порога (50%), помечаются как «сложные» и подсвечиваются в UI.
    /// </summary>
    public IReadOnlyList<QuestionDifficulty> GetQuestionDifficulty(IEnumerable<Result> results)
    {
        var titles = _tests.GetAll().ToDictionary(t => t.Id, t => t.Title);

        return results
            .SelectMany(r => r.Details.Where(d => d.IsGraded).Select(d => (r.TestId, Detail: d)))
            .GroupBy(x => (x.TestId, x.Detail.QuestionId))
            .Select(g =>
            {
                int attempts = g.Count();
                int notFull = g.Count(x => x.Detail.ScoreAwarded < x.Detail.MaxScore - 1e-9);
                double rate = 100.0 * notFull / attempts;
                return new QuestionDifficulty(
                    titles.TryGetValue(g.Key.TestId, out var t) ? t : "—",
                    g.First().Detail.QuestionText, attempts, Math.Round(rate, 1), rate >= HardThresholdPercent);
            })
            .OrderByDescending(q => q.WrongRatePercent).ThenByDescending(q => q.Attempts).ToList();
    }
}
