using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>Результаты и сессии тестирования (сессия — «попытка», результат — её итог).</summary>
public interface IResultRepository
{
    Result SaveResult(Result result);              // insert/update
    Result? GetById(int id);
    IReadOnlyList<Result> GetAll();
    IReadOnlyList<Result> GetByStudent(int studentId);
    IReadOnlyList<Result> GetByTest(int testId);
    IReadOnlyList<Result> GetPendingReview();
    void DeleteResult(int id);

    TestSession SaveSession(TestSession session);  // insert/update
    TestSession? GetSession(int id);
    IReadOnlyList<TestSession> GetSessions(int studentId, int testId);
}
