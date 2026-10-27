using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Persistence;

/// <summary>Пользователи в памяти. Как и БД, отдаёт КОПИИ записей.</summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();   // «таблица Users»
    private readonly object _lock = new();
    private int _nextId;

    public User? GetById(int id) { lock (_lock) return _users.TryGetValue(id, out var u) ? u.Clone() : null; }

    public User? GetByUsername(string username)
    {
        lock (_lock)
            return _users.Values.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase))?.Clone();
    }

    public IReadOnlyList<User> GetAll() { lock (_lock) return _users.Values.OrderBy(u => u.Id).Select(u => u.Clone()).ToList(); }

    public IReadOnlyList<User> GetStudents()
    {
        lock (_lock) return _users.Values.Where(u => u.Role == Role.Student).OrderBy(u => u.FullName).Select(u => u.Clone()).ToList();
    }

    public User Create(User user)
    {
        lock (_lock)
        {
            if (_users.Values.Any(u => string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Username '{user.Username}' is already taken.");
            user.Id = ++_nextId;
            _users[user.Id] = user.Clone();
            return user;
        }
    }

    /// <summary>
    /// СЕЙЧАС (InMemory): правим сохранённую запись в словаре. В SQLite/EF Core тот же вызов станет:
    ///   UPDATE Users SET FullName=@fullName, Email=@email, AvatarPath=@avatar WHERE Id=@id
    /// (EF Core: загрузить сущность, поменять поля, SaveChanges()). Сервисы и UI не меняются.
    /// </summary>
    public bool UpdateProfile(int userId, string fullName, string email, string? avatarPath)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(userId, out var stored)) return false;
            stored.FullName = fullName;
            stored.Email = email;
            stored.AvatarPath = avatarPath;
            return true;
        }
    }

    /// <summary>
    /// Compare-and-swap. SQL-аналог:
    ///   UPDATE Users SET PasswordHash=@new WHERE Id=@id AND PasswordHash=@old   -- 1 строка = успех
    /// Проверку СТАРОГО ПАРОЛЯ выполняет AuthService (хэши с солью напрямую не сравнить).
    /// </summary>
    public bool ChangePassword(int userId, string oldPasswordHash, string newPasswordHash)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(userId, out var stored) || stored.PasswordHash != oldPasswordHash) return false;
            stored.PasswordHash = newPasswordHash;
            return true;
        }
    }

    public void Delete(int id) { lock (_lock) _users.Remove(id); }
}
