using System.IO;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Демо-данные. Работает ТОЛЬКО через интерфейсы, поэтому подходит и для первичного наполнения
/// пустой SQLite/PostgreSQL базы (вызов можно оставить, но защитить проверкой "если база пуста").
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories)
    {
        // ---------- группы и категории ----------
        var kn = groups.Save(new Group { Name = "КН-25-1" });
        groups.Save(new Group { Name = "ИБ-24-2" });
        var catProg = categories.Save(new Category { Name = "Программирование" });
        var catDb = categories.Save(new Category { Name = "Базы данных" });
        categories.Save(new Category { Name = "Общие знания" });

        // ---------- пользователи (пароль у всех: 1234) ----------
        users.Create(NewUser("admin",    "Администратор системы", "admin@uni.edu",   null,  Role.Admin,   300));
        var teacher = users.Create(NewUser("teacher",  "Иванов И.И.",     "ivanov@uni.edu",  null,  Role.Teacher, 200));
        var s1      = users.Create(NewUser("student1", "Петров Петр",     "petrov@uni.edu",  kn.Id, Role.Student, 60));
        var s2      = users.Create(NewUser("student2", "Сидоров Алексей", "sidorov@uni.edu", kn.Id, Role.Student, 45));

    }

    // ----------------------- вспомогательные методы -----------------------

    private static User NewUser(string login, string fullName, string email, int? groupId, Role role, int daysAgo) => new()
    {
        Username = login, FullName = fullName, Email = email, GroupId = groupId, Role = role,
        PasswordHash = PasswordHasher.Hash("1234"), RegistrationDate = DateTime.Now.AddDays(-daysAgo)
    };

}
