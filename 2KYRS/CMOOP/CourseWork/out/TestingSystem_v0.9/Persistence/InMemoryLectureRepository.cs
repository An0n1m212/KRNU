using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

/// <summary>Лекції в пам'яті (як і решта репозиторіїв, віддає копії). Заміна на SQLite/EF Core — інша реалізація інтерфейсу.</summary>
public class InMemoryLectureRepository : ILectureRepository
{
    private readonly Dictionary<int, Lecture> _items = new();
    private readonly object _lock = new();
    private int _nextId;

    public IReadOnlyList<Lecture> GetAll()
    {
        lock (_lock) return _items.Values.OrderByDescending(l => l.UpdatedAt).Select(l => l.DeepClone()).ToList();
    }

    public Lecture? GetById(int id) { lock (_lock) return _items.TryGetValue(id, out var l) ? l.DeepClone() : null; }

    public Lecture Save(Lecture lecture)
    {
        lock (_lock)
        {
            if (lecture.Id == 0) lecture.Id = ++_nextId;
            _items[lecture.Id] = lecture.DeepClone();
            return lecture;
        }
    }

    public void Delete(int id) { lock (_lock) _items.Remove(id); }
}
