using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

/// <summary>Тесты в памяти. Отдаёт и принимает копии; ключи присваивает сама, как БД с автоинкрементом.</summary>
public class InMemoryTestRepository : ITestRepository
{
    private readonly Dictionary<int, Test> _tests = new();
    private readonly object _lock = new();
    private int _testId, _questionId, _answerId;

    public IReadOnlyList<Test> GetAll() { lock (_lock) return _tests.Values.OrderBy(t => t.Id).Select(t => t.DeepClone()).ToList(); }

    public Test? GetById(int id) { lock (_lock) return _tests.TryGetValue(id, out var t) ? t.DeepClone() : null; }

    public Test Save(Test test)
    {
        lock (_lock)
        {
            if (test.Id == 0) test.Id = ++_testId;
            foreach (var q in test.Questions)
            {
                if (q.Id == 0) q.Id = ++_questionId;
                q.TestId = test.Id;
                foreach (var a in q.Answers)
                {
                    if (a.Id == 0) a.Id = ++_answerId;
                    a.QuestionId = q.Id;
                }
            }
            _tests[test.Id] = test.DeepClone();
            return test;
        }
    }

    public void Delete(int id) { lock (_lock) _tests.Remove(id); }
}
