using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Тиждень 2: сід лише користувачів і груп (пароль усіх демо-акаунтів — 1234).
/// У тижнях 5–7 цей файл буде замінено повним сідером фіналу
/// (тести, результати, лекції — див. фінальний `Persistence/DemoDataSeeder.cs`).
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups)
    {
        var kn = groups.Save(new Group { Name = "КН-25-1" });
        groups.Save(new Group { Name = "ІБ-24-2" });

        if (users.GetByUsername("admin") is null)
            users.Create(NewUser("admin", "Адміністратор системи", "admin@uni.edu", null, Role.Admin));
        if (users.GetByUsername("teacher") is null)
            users.Create(NewUser("teacher", "Іванов І.І.", "ivanov@uni.edu", null, Role.Teacher));
        if (users.GetByUsername("student1") is null)
            users.Create(NewUser("student1", "Петров Петро", "petrov@uni.edu", kn.Id, Role.Student));
        if (users.GetByUsername("student2") is null)
            users.Create(NewUser("student2", "Сидоров Олексій", "sidorov@uni.edu", kn.Id, Role.Student));
    }

    private static User NewUser(string login, string fullName, string email, int? groupId, Role role) => new()
    {
        Username = login,
        FullName = fullName,
        Email = email,
        GroupId = groupId,
        Role = role,
        PasswordHash = PasswordHasher.Hash("1234"),
        RegistrationDate = DateTime.Now
    };
}
