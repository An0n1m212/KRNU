using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

public class InMemoryGroupRepository : IGroupRepository
{
    private readonly Dictionary<int, Group> _items = new();
    private readonly object _lock = new();
    private int _nextId;

    public IReadOnlyList<Group> GetAll() { lock (_lock) return _items.Values.OrderBy(g => g.Name).Select(g => g.Clone()).ToList(); }
    public Group? GetById(int id) { lock (_lock) return _items.TryGetValue(id, out var g) ? g.Clone() : null; }

    public Group Save(Group group)
    {
        lock (_lock)
        {
            if (group.Id == 0) group.Id = ++_nextId;
            _items[group.Id] = group.Clone();
            return group;
        }
    }

    public void Delete(int id) { lock (_lock) _items.Remove(id); }
}
