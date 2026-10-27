using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public interface IUserRepository
{
    User? GetById(int id);
    User? GetByUsername(string username);
    IReadOnlyList<User> GetAll();
    IReadOnlyList<User> GetStudents();

    /// <summary>Присваивает Id; бросает InvalidOperationException, если логин занят.</summary>
    User Create(User user);

    bool UpdateProfile(int userId, string fullName, string email, string? avatarPath);

    /// <summary>Compare-and-swap: хэш меняется только если сохранённый хэш равен oldPasswordHash.</summary>
    bool ChangePassword(int userId, string oldPasswordHash, string newPasswordHash);

    void Delete(int id);
}
