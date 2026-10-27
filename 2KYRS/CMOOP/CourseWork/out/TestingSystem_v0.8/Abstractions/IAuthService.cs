using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>Коды результатов операций: UI подбирает локализованный текст по ключу "Err_" + код.</summary>
public enum AuthResult
{
    Success,
    UsernameRequired, UsernameTaken,
    FullNameRequired, InvalidEmail, GroupRequired,
    PasswordTooShort, PasswordMismatch, WrongOldPassword, SamePassword,
    UserNotFound, Conflict
}

public interface IAuthService
{
    User? Login(string username, string password);
    void Logout();
    User? GetCurrentUser();
    event EventHandler? CurrentUserChanged;

    AuthResult RegisterStudent(string username, string password, string fullName, string email, int? groupId);
    AuthResult UpdateProfile(string fullName, string email, string? avatarPath);
    AuthResult ChangePassword(string oldPassword, string newPassword, string confirmPassword);

    /// <summary>
    /// Повторная проверка пароля ТЕКУЩЕГО пользователя (разблокировка после бездействия).
    /// Не меняет сессию и не поднимает CurrentUserChanged.
    /// </summary>
    bool Reauthenticate(string password);

    /// <summary>Проверка прав по роли текущего пользователя.</summary>
    bool IsInRole(params Role[] roles);

    /// <summary>Админ управляет любыми тестами, учитель — только своими.</summary>
    bool CanManage(Test test);

    /// <summary>Те саме для лекцій: адмін — будь-які, викладач — власні.</summary>
    bool CanManage(Lecture lecture);
}
