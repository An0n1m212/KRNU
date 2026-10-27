using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Тиждень 5: сід додає результати через <see cref="GradingEngine"/> (динаміка студента).
/// Лекції з'являться у тижні 7 (див. фінальний `Persistence/DemoDataSeeder.cs`).
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories,
        ITestRepository tests, IResultRepository results, GradingEngine grading)
    {
        var kn = groups.GetAll().FirstOrDefault(g => g.Name == "КН-25-1") ?? groups.Save(new Group { Name = "КН-25-1" });
        if (groups.GetAll().All(g => g.Name != "ІБ-24-2")) groups.Save(new Group { Name = "ІБ-24-2" });

        if (users.GetByUsername("admin") is null)
            users.Create(NewUser("admin", "Адміністратор системи", "admin@uni.edu", null, Role.Admin));
        var teacher = users.GetByUsername("teacher") ?? users.Create(NewUser("teacher", "Іванов І.І.", "ivanov@uni.edu", null, Role.Teacher));
        var s1 = users.GetByUsername("student1") ?? users.Create(NewUser("student1", "Петров Петро", "petrov@uni.edu", kn.Id, Role.Student));
        var s2 = users.GetByUsername("student2") ?? users.Create(NewUser("student2", "Сидоров Олексій", "sidorov@uni.edu", kn.Id, Role.Student));

        var catProg = categories.GetAll().FirstOrDefault(c => c.Name == "Програмування") ?? categories.Save(new Category { Name = "Програмування" });
        var catDb = categories.GetAll().FirstOrDefault(c => c.Name == "Бази даних") ?? categories.Save(new Category { Name = "Бази даних" });

        Test db;
        var existing = tests.GetAll().FirstOrDefault(t => t.Title == "Основи баз даних");
        if (existing is null)
        {
            db = tests.Save(new Test
            {
                Title = "Основи баз даних",
                Description = "Три прості питання без ліміту часу",
                AuthorTeacherId = teacher.Id, CategoryId = catDb.Id,
                TimeLimitMinutes = 0, PassScorePercent = 60, MaxAttempts = 0,
                AllowShowExplanations = true,
                Questions =
                {
                    Q(catDb.Id, "Яка команда SQL читає дані з таблиці?", QuestionType.SingleChoice, 1, "SELECT вибирає рядки.",
                        ("SELECT", true), ("INSERT", false), ("DELETE", false)),
                    Q(catDb.Id, "Що означає абревіатура PK?", QuestionType.SingleChoice, 1, "Primary Key — первинний ключ.",
                        ("Primary Key", true), ("Public Key", false), ("Private Key", false)),
                    Q(catDb.Id, "Який оператор об'єднує таблиці за умовою?", QuestionType.SingleChoice, 1, "JOIN зв'язує рядки.",
                        ("JOIN", true), ("GROUP", false), ("ORDER", false)),
                }
            });
        }
        else db = existing;

        if (tests.GetAll().All(t => t.Title != "Комплексний тест: паттерни і C#"))
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
                    Q(catProg.Id, "Яке ключове слово оголошує константу часу компіляції в C#?", QuestionType.SingleChoice, 1, "const — час компіляції.",
                        ("const", true), ("readonly", false), ("static", false)),
                    new Question { CategoryId = catProg.Id, Text = "Зіставте термін і визначення.", QuestionType = QuestionType.Matching, MaxScore = 3,
                        Explanation = "Базові поняття.", Answers = {
                            new Answer { Text = "Repository", CorrectPairText = "Абстракція доступу до даних", OrderIndex = 0 },
                            new Answer { Text = "DI", CorrectPairText = "Передача залежностей ззовні", OrderIndex = 1 } } },
                    new Question { CategoryId = catDb.Id, Text = "Розставте кроки у правильному порядку.", QuestionType = QuestionType.Sequence, MaxScore = 2,
                        Explanation = "Відкрити → закрити.", Answers = {
                            new Answer { Text = "Відкрити з'єднання", OrderIndex = 0 }, new Answer { Text = "Закрити з'єднання", OrderIndex = 1 } } },
                }
            });
        }

        // Демо-динаміка: 33% → 67% → 100% (сертифікати й аналітика — тиждень 6).
        if (results.GetAll().Count == 0)
        {
            Submit(results, grading, s1, db, DateTime.Now.AddDays(-3), Pick(db.Questions[0], 1), Pick(db.Questions[1], 1), Pick(db.Questions[2], 2));
            Submit(results, grading, s1, db, DateTime.Now.AddDays(-1), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 2));
            Submit(results, grading, s2, db, DateTime.Now.AddDays(-2), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 0));
        }
    }

    private static Question Q(int cat, string text, QuestionType type, double max, string expl, params (string Text, bool Correct)[] answers) => new()
    {
        CategoryId = cat, Text = text, QuestionType = type, MaxScore = max, Explanation = expl,
        Answers = answers.Select((a, i) => new Answer { Text = a.Text, IsCorrect = a.Correct, OrderIndex = i }).ToList()
    };

    private static User NewUser(string login, string fullName, string email, int? groupId, Role role) => new()
    {
        Username = login, FullName = fullName, Email = email, GroupId = groupId, Role = role,
        PasswordHash = PasswordHasher.Hash("1234"), RegistrationDate = DateTime.Now
    };

    private static void Submit(IResultRepository results, GradingEngine grading, User student, Test test, DateTime when, params StudentAnswerDetail[] given)
    {
        var session = results.SaveSession(new TestSession
        {
            StudentId = student.Id, TestId = test.Id, StartTime = when.AddMinutes(-7), EndTime = when,
            CurrentQuestionIndex = test.Questions.Count - 1, Status = ResultStatus.Completed
        });
        var result = grading.BuildResult(session, test, given.ToDictionary(d => d.QuestionId), timedOut: false);
        result.Date = when;
        session.Status = result.Status;
        results.SaveSession(session);
        results.SaveResult(result);
    }

    private static StudentAnswerDetail Pick(Question q, int index) => new() { QuestionId = q.Id, SelectedAnswerIds = { q.Answers[index].Id } };
}
