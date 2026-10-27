using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly Dictionary<int, Category> _items = new();
    private readonly object _lock = new();
    private int _nextId;

    public IReadOnlyList<Category> GetAll() { lock (_lock) return _items.Values.OrderBy(c => c.Name).Select(c => c.Clone()).ToList(); }
    public Category? GetById(int id) { lock (_lock) return _items.TryGetValue(id, out var c) ? c.Clone() : null; }

    public Category Save(Category category)
    {
        lock (_lock)
        {
            if (category.Id == 0) category.Id = ++_nextId;
            _items[category.Id] = category.Clone();
            return category;
        }
    }

    public void Delete(int id) { lock (_lock) _items.Remove(id); }
}
