using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Тиждень 3: сід розширено категоріями і демо-тестом (3 питання).
/// Сигнатура росте до фінальної: у тижні 5 додадуться результати + GradingEngine,
/// у тижні 7 — лекції (див. фінальний `Persistence/DemoDataSeeder.cs`).
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories, ITestRepository tests)
    {
        var kn = groups.GetAll().FirstOrDefault(g => g.Name == "КН-25-1") ?? groups.Save(new Group { Name = "КН-25-1" });
        groups.GetAll().FirstOrDefault(g => g.Name == "ІБ-24-2");
        if (groups.GetAll().Count < 2) groups.Save(new Group { Name = "ІБ-24-2" });

        if (users.GetByUsername("admin") is null)
            users.Create(NewUser("admin", "Адміністратор системи", "admin@uni.edu", null, Role.Admin));
        var teacher = users.GetByUsername("teacher") ?? users.Create(NewUser("teacher", "Іванов І.І.", "ivanov@uni.edu", null, Role.Teacher));
        if (users.GetByUsername("student1") is null)
            users.Create(NewUser("student1", "Петров Петро", "petrov@uni.edu", kn.Id, Role.Student));
        if (users.GetByUsername("student2") is null)
            users.Create(NewUser("student2", "Сидоров Олексій", "sidorov@uni.edu", kn.Id, Role.Student));

        var catDb = categories.GetAll().FirstOrDefault(c => c.Name == "Бази даних") ?? categories.Save(new Category { Name = "Бази даних" });
        categories.GetAll().FirstOrDefault(c => c.Name == "Програмування");
        if (categories.GetAll().Count < 2) categories.Save(new Category { Name = "Програмування" });

        if (tests.GetAll().Count == 0)
        {
            tests.Save(new Test
            {
                Title = "Основи баз даних",
                Description = "Демо-тест тижня 3: один/кілька варіантів + текст",
                AuthorTeacherId = teacher.Id,
                CategoryId = catDb.Id,
                TimeLimitMinutes = 0,
                PassScorePercent = 60,
                MaxAttempts = 0,
                AllowShowExplanations = true,
                Questions =
                {
                    new Question
                    {
                        CategoryId = catDb.Id, Text = "Яка команда SQL читає дані з таблиці?",
                        QuestionType = QuestionType.SingleChoice, MaxScore = 1, Explanation = "SELECT вибирає рядки.",
                        Answers =
                        {
                            new Answer { Text = "SELECT", IsCorrect = true, OrderIndex = 0 },
                            new Answer { Text = "INSERT", IsCorrect = false, OrderIndex = 1 },
                            new Answer { Text = "DELETE", IsCorrect = false, OrderIndex = 2 },
                        }
                    },
                    new Question
                    {
                        CategoryId = catDb.Id, Text = "Які типи є посилальними?",
                        QuestionType = QuestionType.MultipleChoice, MaxScore = 2, Explanation = "string і object — посилальні.",
                        Answers =
                        {
                            new Answer { Text = "string", IsCorrect = true, OrderIndex = 0 },
                            new Answer { Text = "int", IsCorrect = false, OrderIndex = 1 },
                            new Answer { Text = "object", IsCorrect = true, OrderIndex = 2 },
                        }
                    },
                    new Question
                    {
                        CategoryId = catDb.Id, Text = "Як називається метод — точка входу консольної програми C#?",
                        QuestionType = QuestionType.TextAnswer, TextEvaluationType = TextEvaluationType.AutoExactMatch,
                        MaxScore = 1, Explanation = "Метод називається Main.",
                        Answers = { new Answer { Text = "main", IsCorrect = true, OrderIndex = 0 } }
                    },
                }
            });
        }
    }

    private static User NewUser(string login, string fullName, string email, int? groupId, Role role) => new()
    {
        Username = login, FullName = fullName, Email = email, GroupId = groupId, Role = role,
        PasswordHash = PasswordHasher.Hash("1234"), RegistrationDate = DateTime.Now
    };
}
