using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Тиждень 4: демо-тест покриває ВСІ 6 типів (ТЗ п.3.3.2), пін-код 1234, ліміт 10 хв, 3 спроби.
/// У тижні 5 сюди додадуться результати + GradingEngine.
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories, ITestRepository tests)
    {
        var kn = groups.GetAll().FirstOrDefault(g => g.Name == "КН-25-1") ?? groups.Save(new Group { Name = "КН-25-1" });
        if (groups.GetAll().All(g => g.Name != "ІБ-24-2")) groups.Save(new Group { Name = "ІБ-24-2" });

        if (users.GetByUsername("admin") is null)
            users.Create(NewUser("admin", "Адміністратор системи", "admin@uni.edu", null, Role.Admin));
        var teacher = users.GetByUsername("teacher") ?? users.Create(NewUser("teacher", "Іванов І.І.", "ivanov@uni.edu", null, Role.Teacher));
        if (users.GetByUsername("student1") is null)
            users.Create(NewUser("student1", "Петров Петро", "petrov@uni.edu", kn.Id, Role.Student));
        if (users.GetByUsername("student2") is null)
            users.Create(NewUser("student2", "Сидоров Олексій", "sidorov@uni.edu", kn.Id, Role.Student));

        var catProg = categories.GetAll().FirstOrDefault(c => c.Name == "Програмування") ?? categories.Save(new Category { Name = "Програмування" });
        var catDb = categories.GetAll().FirstOrDefault(c => c.Name == "Бази даних") ?? categories.Save(new Category { Name = "Бази даних" });

        if (tests.GetAll().Count == 0)
        {
            tests.Save(new Test
            {
                Title = "Комплексний тест: паттерни і C#",
                Description = "Усі 6 типів. Пін-код: 1234",
                AuthorTeacherId = teacher.Id, CategoryId = catProg.Id,
                TimeLimitMinutes = 10, PassScorePercent = 60, MaxAttempts = 3,
                IsRandomizeQuestions = false, IsRandomizeAnswers = true,
                AccessCode = "1234", AssignedGroupIds = { kn.Id }, AllowShowExplanations = true,
                Questions =
                {
                    new Question { CategoryId = catProg.Id, Text = "Яке ключове слово оголошує константу часу компіляції в C#?",
                        QuestionType = QuestionType.SingleChoice, MaxScore = 1, Explanation = "const — час компіляції.",
                        Answers = { new Answer { Text = "const", IsCorrect = true, OrderIndex = 0 }, new Answer { Text = "readonly", OrderIndex = 1 }, new Answer { Text = "static", OrderIndex = 2 } } },
                    new Question { CategoryId = catProg.Id, Text = "Які типи є посилальними?",
                        QuestionType = QuestionType.MultipleChoice, MaxScore = 2, Explanation = "string і object.",
                        Answers = { new Answer { Text = "string", IsCorrect = true, OrderIndex = 0 }, new Answer { Text = "int", OrderIndex = 1 }, new Answer { Text = "object", IsCorrect = true, OrderIndex = 2 } } },
                    new Question { CategoryId = catProg.Id, Text = "Назва методу — точки входу консольної програми (одне слово)?",
                        QuestionType = QuestionType.TextAnswer, TextEvaluationType = TextEvaluationType.AutoExactMatch, MaxScore = 1, Explanation = "Main.",
                        Answers = { new Answer { Text = "main", IsCorrect = true, OrderIndex = 0 } } },
                    new Question { CategoryId = catProg.Id, Text = "Зіставте термін і визначення.",
                        QuestionType = QuestionType.Matching, MaxScore = 3, Explanation = "Базові поняття архітектури.",
                        Answers = {
                            new Answer { Text = "Repository", CorrectPairText = "Абстракція доступу до даних", OrderIndex = 0 },
                            new Answer { Text = "Dependency Injection", CorrectPairText = "Передача залежностей ззовні", OrderIndex = 1 },
                            new Answer { Text = "Strategy", CorrectPairText = "Взаємозамінні алгоритми", OrderIndex = 2 } } },
                    new Question { CategoryId = catDb.Id, Text = "Розставте кроки роботи з БД у правильному порядку.",
                        QuestionType = QuestionType.Sequence, MaxScore = 2, Explanation = "Відкрити → команда → запит → закрити.",
                        Answers = {
                            new Answer { Text = "Відкрити з'єднання", OrderIndex = 0 }, new Answer { Text = "Створити команду", OrderIndex = 1 },
                            new Answer { Text = "Виконати запит", OrderIndex = 2 }, new Answer { Text = "Закрити з'єднання", OrderIndex = 3 } } },
                    new Question { CategoryId = catProg.Id, Text = "Завантажте файл з розв'язком лабораторної (до 10 МБ).",
                        QuestionType = QuestionType.FileUpload, MaxScore = 2, Explanation = "Документи, архіви, код." },
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
