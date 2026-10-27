using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

public class InMemoryResultRepository : IResultRepository
{
    private readonly Dictionary<int, Result> _results = new();
    private readonly Dictionary<int, TestSession> _sessions = new();
    private readonly object _lock = new();
    private int _nextResult, _nextDetail, _nextSession;

    public Result SaveResult(Result result)
    {
        lock (_lock)
        {
            if (result.Id == 0) result.Id = ++_nextResult;
            foreach (var d in result.Details)
            {
                if (d.Id == 0) d.Id = ++_nextDetail;
                d.ResultId = result.Id;
            }
            _results[result.Id] = result.DeepClone();
            return result;
        }
    }

    public Result? GetById(int id) { lock (_lock) return _results.TryGetValue(id, out var r) ? r.DeepClone() : null; }

    public IReadOnlyList<Result> GetAll() => Query(_ => true);
    public IReadOnlyList<Result> GetByStudent(int studentId) => Query(r => r.StudentId == studentId);
    public IReadOnlyList<Result> GetByTest(int testId) => Query(r => r.TestId == testId);
    public IReadOnlyList<Result> GetPendingReview() => Query(r => r.Status == ResultStatus.PendingTeacherReview);

    private IReadOnlyList<Result> Query(Func<Result, bool> filter)
    {
        lock (_lock) return _results.Values.Where(filter).OrderByDescending(r => r.Date).Select(r => r.DeepClone()).ToList();
    }

    public void DeleteResult(int id) { lock (_lock) _results.Remove(id); }

    public TestSession SaveSession(TestSession session)
    {
        lock (_lock)
        {
            if (session.Id == 0) session.Id = ++_nextSession;
            _sessions[session.Id] = session.Clone();
            return session;
        }
    }

    public TestSession? GetSession(int id) { lock (_lock) return _sessions.TryGetValue(id, out var s) ? s.Clone() : null; }

    public IReadOnlyList<TestSession> GetSessions(int studentId, int testId)
    {
        lock (_lock) return _sessions.Values.Where(s => s.StudentId == studentId && s.TestId == testId)
            .OrderBy(s => s.StartTime).Select(s => s.Clone()).ToList();
    }
}
