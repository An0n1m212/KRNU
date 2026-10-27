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
    public static void Seed(IUserRepository users, IGroupRepository groups, ICategoryRepository categories, ITestRepository tests, IResultRepository results, GradingEngine grading)
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

        // ---------- тест 1: 10 минут, пин-код 1234, ВСЕ 6 типов вопросов ----------
        var full = tests.Save(new Test
        {
            Title = "Комплексный тест: паттерны и C#",
            Description = "Демонстрирует все типы вопросов. Пин-код доступа: 1234",
            AuthorTeacherId = teacher.Id, CategoryId = catProg.Id,
            TimeLimitMinutes = 10, PassScorePercent = 60, MaxAttempts = 3,
            IsRandomizeQuestions = false, IsRandomizeAnswers = true,
            AccessCode = "1234", AssignedGroupIds = { kn.Id }, AllowShowExplanations = true,
            Questions =
            {
                Single(catProg.Id, "Какое ключевое слово объявляет константу времени компиляции в C#?", 1,
                    "const вычисляется на этапе компиляции, readonly — при создании объекта.",
                    ("const", true), ("readonly", false), ("static", false)),

                Multiple(catProg.Id, "Какие из перечисленных типов являются ссылочными?", 2,
                    "string и object — ссылочные типы; int и double — значимые.",
                    ("string", true), ("int", false), ("object", true), ("double", false)),

                TextQ(catProg.Id, "Как называется метод — точка входа консольной программы C#? (одно слово)", 1,
                    TextEvaluationType.AutoExactMatch, "Метод называется Main.", "main"),

                TextQ(catProg.Id, "Назовите любой паттерн проектирования из книги «банды четырёх» (GoF).", 1,
                    TextEvaluationType.AutoContains, "Подходят Singleton, Factory, Observer, Strategy, Adapter и др.",
                    "singleton", "factory", "observer", "strategy", "adapter", "decorator"),

                TextQ(catProg.Id, "Своими словами объясните, зачем нужен паттерн «Репозиторий».", 3,
                    TextEvaluationType.ManualTeacherReview, "Репозиторий изолирует логику доступа к данным от бизнес-логики и UI."),

                Matching(catProg.Id, "Сопоставьте термин и определение.", 3, "Это три базовых понятия архитектуры.",
                    ("Repository", "Абстракция доступа к данным"),
                    ("Dependency Injection", "Передача зависимостей извне"),
                    ("Strategy", "Взаимозаменяемые алгоритмы")),

                Sequence(catDb.Id, "Расставьте шаги работы с базой данных в правильном порядке.", 2,
                    "Соединение открывают, создают команду, выполняют её и закрывают соединение.",
                    "Открыть соединение", "Создать команду", "Выполнить запрос", "Закрыть соединение"),

                FileQ(catProg.Id, "Загрузите файл с решением лабораторной работы (до 10 МБ).", 2,
                    "Допустимы документы, архивы и исходники.")
            }
        });

        // ---------- тест 2: короткий, без кода и времени, для всех групп ----------
        var db = tests.Save(new Test
        {
            Title = "Основы баз данных",
            Description = "Три простых вопроса без ограничения по времени",
            AuthorTeacherId = teacher.Id, CategoryId = catDb.Id,
            TimeLimitMinutes = 0, PassScorePercent = 60, MaxAttempts = 0,
            IsRandomizeAnswers = false, AllowShowExplanations = true,
            Questions =
            {
                Single(catDb.Id, "Какая команда SQL читает данные из таблицы?", 1, "SELECT выбирает строки.",
                    ("SELECT", true), ("INSERT", false), ("DELETE", false)),
                Single(catDb.Id, "Что означает аббревиатура PK?", 1, "Primary Key — первичный ключ.",
                    ("Primary Key", true), ("Public Key", false), ("Private Key", false)),
                Single(catDb.Id, "Какой оператор объединяет таблицы по условию?", 1, "JOIN связывает строки таблиц.",
                    ("JOIN", true), ("GROUP", false), ("ORDER", false))
            }
        });

        // ---------- результаты ----------
        // 1) Работа ученика, ожидающая проверки (есть ручные вопросы) — для демонстрации экрана проверки
        string? demoFile = CreateDemoUpload();
        var q = full.Questions;
        Submit(results, grading, s1, full, DateTime.Now.AddHours(-3),
            Pick(q[0], 0), PickMany(q[1], 0, 2), Typed(q[2], "  MAIN "), Typed(q[3], "Я знаю паттерн Singleton"),
            Typed(q[4], "Репозиторий скрывает детали хранения данных: UI и бизнес-логика работают с интерфейсом, а реализацию (память, SQLite) можно заменить."),
            Matched(q[5], 0, 1, 1),            // третья пара выбрана неверно -> частичный балл 2 из 3
            Ordered(q[6], 0, 1, 2, 3), UploadedFile(q[7], demoFile));

        // 2) Проверенные попытки по тесту 2: динамика успеваемости и сертификаты
        Submit(results, grading, s1, db, DateTime.Now.AddDays(-3), Pick(db.Questions[0], 0), Pick(db.Questions[1], 1), Pick(db.Questions[2], 2)); // 33%
        Submit(results, grading, s1, db, DateTime.Now.AddDays(-1), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 2)); // 67%
        Submit(results, grading, s2, db, DateTime.Now.AddDays(-2), Pick(db.Questions[0], 0), Pick(db.Questions[1], 0), Pick(db.Questions[2], 0)); // 100%

    }

    // ----------------------- вспомогательные методы -----------------------

    private static User NewUser(string login, string fullName, string email, int? groupId, Role role, int daysAgo) => new()
    {
        Username = login, FullName = fullName, Email = email, GroupId = groupId, Role = role,
        PasswordHash = PasswordHasher.Hash("1234"), RegistrationDate = DateTime.Now.AddDays(-daysAgo)
    };


    // --- конструкторы вопросов ---
    private static Question Single(int cat, string text, double max, string explanation, params (string Text, bool Correct)[] answers) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.SingleChoice, MaxScore = max, Explanation = explanation,
        Answers = answers.Select((a, i) => new Answer { Text = a.Text, IsCorrect = a.Correct, OrderIndex = i }).ToList()
    };

    private static Question Multiple(int cat, string text, double max, string explanation, params (string Text, bool Correct)[] answers) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.MultipleChoice, MaxScore = max, Explanation = explanation,
        Answers = answers.Select((a, i) => new Answer { Text = a.Text, IsCorrect = a.Correct, OrderIndex = i }).ToList()
    };

    private static Question TextQ(int cat, string text, double max, TextEvaluationType eval, string explanation, params string[] accepted) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.TextAnswer, TextEvaluationType = eval, MaxScore = max, Explanation = explanation,
        Answers = accepted.Select((a, i) => new Answer { Text = a, IsCorrect = true, OrderIndex = i }).ToList()
    };

    private static Question Matching(int cat, string text, double max, string explanation, params (string Left, string Right)[] pairs) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.Matching, MaxScore = max, Explanation = explanation,
        Answers = pairs.Select((p, i) => new Answer { Text = p.Left, CorrectPairText = p.Right, OrderIndex = i }).ToList()
    };

    private static Question Sequence(int cat, string text, double max, string explanation, params string[] itemsInCorrectOrder) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.Sequence, MaxScore = max, Explanation = explanation,
        Answers = itemsInCorrectOrder.Select((t, i) => new Answer { Text = t, OrderIndex = i }).ToList()
    };

    private static Question FileQ(int cat, string text, double max, string explanation) => new()
    {
        CategoryId = cat, Text = text, QuestionType = QuestionType.FileUpload, MaxScore = max, Explanation = explanation
    };

    private static void Submit(IResultRepository results, GradingEngine grading, User student, Test test, DateTime when,
                               params StudentAnswerDetail[] given)
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

    private static string? CreateDemoUpload()
    {
        try
        {
            string dir = Path.Combine(AppFiles.Root, "uploads");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, "demo_lab_solution.txt");
            File.WriteAllText(path, "Демо-файл ученика: решение лабораторной работы.\nЗдесь мог бы быть ваш код.");
            return path;
        }
        catch { return null; }
    }

    // --- ответы ученика ---
    private static StudentAnswerDetail Pick(Question q, int index) => new() { QuestionId = q.Id, SelectedAnswerIds = { q.Answers[index].Id } };
    private static StudentAnswerDetail PickMany(Question q, params int[] indexes) => new() { QuestionId = q.Id, SelectedAnswerIds = indexes.Select(i => q.Answers[i].Id).ToList() };
    private static StudentAnswerDetail Typed(Question q, string text) => new() { QuestionId = q.Id, GivenTextAnswer = text };
    private static StudentAnswerDetail UploadedFile(Question q, string? path) => new() { QuestionId = q.Id, UploadedFilePath = path };

    /// <summary>Sequence: индексы вариантов в порядке, расставленном учеником.</summary>
    private static StudentAnswerDetail Ordered(Question q, params int[] indexes)
        => new() { QuestionId = q.Id, SelectedAnswerIds = indexes.Select(i => q.Answers[i].Id).ToList() };

    /// <summary>Matching: для каждой левой строки — индекс варианта, чья правая часть выбрана.</summary>
    private static StudentAnswerDetail Matched(Question q, params int[] rightOwnerIndexes)
        => new() { QuestionId = q.Id, SelectedAnswerIds = rightOwnerIndexes.Select(i => q.Answers[i].Id).ToList() };

}
