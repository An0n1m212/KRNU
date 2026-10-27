using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Persistence;

/// <summary>
/// Тиждень 7: сід додає лекції (опубліковані + чернетка). Фінальний сідер відрізняється
/// лише демо-результатом «на перевірці» з файлом (див. фінальний `Persistence/DemoDataSeeder.cs`).
/// </summary>
public static class DemoDataSeeder
{
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories,
        ITestRepository tests, IResultRepository results, ILectureRepository lectures, GradingEngine grading)
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

        Test db = tests.GetAll().FirstOrDefault(t => t.Title == "Основи баз даних")
            ?? tests.Save(new Test
            {
                Title = "Основи баз даних", Description = "Три прості питання", AuthorTeacherId = teacher.Id, CategoryId = catDb.Id,
                TimeLimitMinutes = 0, PassScorePercent = 60, MaxAttempts = 0, AllowShowExplanations = true,
                Questions =
                {
                    Q(catDb.Id, "Яка команда SQL читає дані?", QuestionType.SingleChoice, 1, "SELECT.",
                        ("SELECT", true), ("INSERT", false), ("DELETE", false)),
                    Q(catDb.Id, "Що означає PK?", QuestionType.SingleChoice, 1, "Primary Key.",
                        ("Primary Key", true), ("Public Key", false), ("Private Key", false)),
                    Q(catDb.Id, "Який оператор об'єднує таблиці?", QuestionType.SingleChoice, 1, "JOIN.",
                        ("JOIN", true), ("GROUP", false), ("ORDER", false)),
                }
            });

        Test full = tests.GetAll().FirstOrDefault(t => t.Title == "Комплексний тест: паттерни і C#")
            ?? tests.Save(new Test
            {
                Title = "Комплексний тест: паттерни і C#", Description = "Усі 6 типів. Пін: 1234",
                AuthorTeacherId = teacher.Id, CategoryId = catProg.Id,
                TimeLimitMinutes = 10, PassScorePercent = 60, MaxAttempts = 3,
                IsRandomizeQuestions = false, IsRandomizeAnswers = true,
                AccessCode = "1234", AssignedGroupIds = { kn.Id }, AllowShowExplanations = true,
                Questions =
                {
                    Q(catProg.Id, "const чи readonly?", QuestionType.SingleChoice, 1, "const.",
                        ("const", true), ("readonly", false), ("static", false)),
                    Q(catProg.Id, "Назва методу-точки входу?", QuestionType.TextAnswer, 1, "Main.", "main"),
                }
            });

        if (results.GetAll().Count == 0)
        {
            Submit(results, grading, s1, db, DateTime.Now.AddDays(-3), Pick(db.Questions[0], 1), Pick(db.Questions[1], 1), Pick(db.Questions[2], 2));
            Submit(results, grading, s1, db, DateTime.Now.AddDays(-1), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 2));
            Submit(results, grading, s2, db, DateTime.Now.AddDays(-2), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 0));
        }

        if (lectures.GetAll().Count == 0)
        {
            lectures.Save(new Lecture
            {
                Title = "Паттерн «Репозиторій»", Summary = "Доступ до даних за інтерфейсом.",
                Content = "# Паттерн «Репозиторій»\n\n## Навіщо\nРепозиторій **ховає спосіб зберігання** за інтерфейсом.\n\n## Ідеї\n- Інтерфейс: отримати, зберегти, видалити.\n- Реалізація: пам'ять, SQLite, PostgreSQL.\n\n## Приклад\n```\ninterface ITestRepository\n{\n    Test? GetById(int id);\n}\n```\n",
                AuthorTeacherId = teacher.Id, CategoryId = catProg.Id,
                AssignedGroupIds = { kn.Id }, IsPublished = true, RelatedTestId = full.Id,
                CreatedAt = DateTime.Now.AddDays(-10), UpdatedAt = DateTime.Now.AddDays(-2)
            });
            lectures.Save(new Lecture
            {
                Title = "Основи SQL: SELECT і JOIN", Summary = "Вибірка та об'єднання.",
                Content = "# Основи SQL\n\n## SELECT\n```\nSELECT Name FROM Students;\n```\n\n## JOIN\n```\nSELECT s.Name FROM Students s\nJOIN Groups g ON g.Id = s.GroupId;\n```\n",
                AuthorTeacherId = teacher.Id, CategoryId = catDb.Id,
                IsPublished = true, RelatedTestId = db.Id,
                CreatedAt = DateTime.Now.AddDays(-6), UpdatedAt = DateTime.Now.AddDays(-1)
            });
            lectures.Save(new Lecture
            {
                Title = "Чернетка: Dependency Injection", Summary = "Ще не опубліковано.",
                Content = "# Dependency Injection\n\nЗаготовка лекції.",
                AuthorTeacherId = teacher.Id, CategoryId = catProg.Id, IsPublished = false,
                CreatedAt = DateTime.Now.AddHours(-5), UpdatedAt = DateTime.Now.AddHours(-5)
            });
        }
    }

    private static Question Q(int cat, string text, QuestionType type, double max, string expl, params object[] answers)
    {
        var q = new Question { CategoryId = cat, Text = text, QuestionType = type, MaxScore = max, Explanation = expl };
        if (type == QuestionType.TextAnswer)
        {
            int i = 0;
            foreach (var a in answers) q.Answers.Add(new Answer { Text = a.ToString()!, IsCorrect = true, OrderIndex = i++ });
        }
        else
        {
            int i = 0;
            foreach (var (Text, Correct) in answers.Cast<(string, bool)>())
                q.Answers.Add(new Answer { Text = Text, IsCorrect = Correct, OrderIndex = i++ });
        }
        return q;
    }

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
