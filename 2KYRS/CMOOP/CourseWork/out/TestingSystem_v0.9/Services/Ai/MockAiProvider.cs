using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Офлайн-провайдер для демонстрації та розробки: повертає ШАБЛОННІ, але структурно коректні відповіді
/// у тому ж JSON-форматі, що й реальні моделі. Дозволяє пройти весь сценарій без ключа та інтернету.
/// </summary>
public sealed class MockAiProvider : IAiProvider
{
    public AiProviderKind Kind => AiProviderKind.Mock;

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest r, CancellationToken ct)
    {
        await Task.Delay(700, ct);   // імітація мережевої затримки (і щоб було видно індикатор виконання)

        return r.Task switch
        {
            AiTask.Ping => "OK",
            AiTask.LectureTopics => Serialize(Topics(r)),
            AiTask.Lecture => Serialize(Lecture(r)),
            AiTask.Test => Serialize(Test(r)),
            AiTask.GradeAnswer => Serialize(Grade(r)),
            _ => "{}"
        };
    }

    private static string Serialize(object value) => JsonSerializer.Serialize(value);

    private static string L(AiRequest r, string ua, string ru, string en)
        => r.Language switch { "Ukrainian" => ua, "Russian" => ru, _ => en };

    private static object Topics(AiRequest r)
    {
        string t = r.Topic;
        var all = new (string, string)[]
        {
            (L(r, $"{t}: основні поняття", $"{t}: основные понятия", $"{t}: core concepts"),
             L(r, "Терміни, означення та загальна картина теми.", "Термины, определения и общая картина темы.", "Terms, definitions and the big picture.")),
            (L(r, $"{t}: практичні приклади", $"{t}: практические примеры", $"{t}: practical examples"),
             L(r, "Розбір типових задач на прикладах.", "Разбор типовых задач на примерах.", "Walkthrough of typical problems.")),
            (L(r, $"{t}: типові помилки", $"{t}: типичные ошибки", $"{t}: common mistakes"),
             L(r, "На що звертати увагу, щоб не помилитися.", "На что обращать внимание, чтобы не ошибиться.", "What to watch out for.")),
            (L(r, $"{t}: поглиблені можливості", $"{t}: углублённые возможности", $"{t}: advanced topics"),
             L(r, "Складніші прийоми для підготовлених студентів.", "Более сложные приёмы для подготовленных студентов.", "Harder techniques for prepared students.")),
            (L(r, $"{t}: підсумки та практика", $"{t}: итоги и практика", $"{t}: summary and practice"),
             L(r, "Повторення, самоперевірка, завдання.", "Повторение, самопроверка, задания.", "Review, self-check, exercises."))
        };
        return new { topics = Enumerable.Range(0, Math.Max(1, r.Count)).Select(i => all[i % all.Length])
            .Select((x, i) => new { title = r.Count > all.Length && i >= all.Length ? $"{x.Item1} ({i / all.Length + 1})" : x.Item1, description = x.Item2 }) };
    }

    private static object Lecture(AiRequest r)
    {
        string t = r.Topic;
        string content = L(r,
            $"# {t}\n\n## Вступ\nЦя лекція — **демонстраційна** (режим Mock). Підключіть реальний AI-провайдер у налаштуваннях, щоб отримати повноцінний текст.\n\n## Основні поняття\n- Поняття 1: означення та приклад\n- Поняття 2: означення та приклад\n- Поняття 3: означення та приклад\n\n## Приклад\n```\nпсевдокод прикладу для теми «{t}»\n```\n\n## Підсумки\n- Головна думка 1\n- Головна думка 2\n\n## Питання для самоперевірки\n1. Що таке головне поняття теми?\n2. Наведіть приклад.\n3. Які типові помилки?",
            $"# {t}\n\n## Введение\nЭта лекция — **демонстрационная** (режим Mock). Подключите реальный AI-провайдер в настройках, чтобы получить полноценный текст.\n\n## Основные понятия\n- Понятие 1: определение и пример\n- Понятие 2: определение и пример\n- Понятие 3: определение и пример\n\n## Пример\n```\nпсевдокод примера для темы «{t}»\n```\n\n## Итоги\n- Главная мысль 1\n- Главная мысль 2\n\n## Вопросы для самопроверки\n1. Что такое главное понятие темы?\n2. Приведите пример.\n3. Каковы типичные ошибки?",
            $"# {t}\n\n## Introduction\nThis lecture is a **demo** (Mock mode). Connect a real AI provider in the settings to get a full text.\n\n## Core concepts\n- Concept 1: definition and example\n- Concept 2: definition and example\n- Concept 3: definition and example\n\n## Example\n```\npseudo-code example for \"{t}\"\n```\n\n## Summary\n- Key idea 1\n- Key idea 2\n\n## Self-check questions\n1. What is the main concept of the topic?\n2. Give an example.\n3. What are common mistakes?");

        return new
        {
            title = t,
            summary = L(r, $"Демонстраційна лекція за темою «{t}».", $"Демонстрационная лекция по теме «{t}».", $"A demo lecture on \"{t}\"."),
            content
        };
    }

    private static object Test(AiRequest r)
    {
        string t = r.Topic;
        var questions = new List<object>
        {
            new { type = "SingleChoice", maxScore = 1, text = L(r, $"Що найточніше описує тему «{t}»?", $"Что точнее всего описывает тему «{t}»?", $"What best describes the topic \"{t}\"?"),
                  explanation = L(r, "Демо-питання (Mock).", "Демо-вопрос (Mock).", "Demo question (Mock)."),
                  answers = new object[] {
                      new { text = L(r, $"Основне поняття теми «{t}»", $"Основное понятие темы «{t}»", $"The core concept of \"{t}\""), isCorrect = true },
                      new { text = L(r, "Випадкова назва", "Случайное название", "A random name"), isCorrect = false },
                      new { text = L(r, "Не стосується навчання", "Не относится к обучению", "Unrelated to studying"), isCorrect = false } } },

            new { type = "MultipleChoice", maxScore = 2, text = L(r, $"Оберіть усі твердження, що стосуються теми «{t}».", $"Выберите все утверждения, относящиеся к теме «{t}».", $"Select all statements related to \"{t}\"."),
                  explanation = L(r, "Демо-питання (Mock).", "Демо-вопрос (Mock).", "Demo question (Mock)."),
                  answers = new object[] {
                      new { text = L(r, "Має означення", "Имеет определение", "Has a definition"), isCorrect = true },
                      new { text = L(r, "Має практичні приклади", "Имеет практические примеры", "Has practical examples"), isCorrect = true },
                      new { text = L(r, "Не потребує вивчення", "Не требует изучения", "Needs no study"), isCorrect = false },
                      new { text = L(r, "Неможливо застосувати", "Невозможно применить", "Cannot be applied"), isCorrect = false } } },

            new { type = "TextAnswer", maxScore = 1, textEvaluation = "AutoContains",
                  text = L(r, "Назвіть тему цієї лекції одним–двома словами.", "Назовите тему этой лекции одним-двумя словами.", "Name the topic of this lecture in a word or two."),
                  explanation = L(r, "Підходить будь-яка відповідь, що містить назву теми.", "Подходит любой ответ, содержащий название темы.", "Any answer containing the topic name is accepted."),
                  acceptedAnswers = new[] { t.ToLowerInvariant() } },

            new { type = "Matching", maxScore = 3, text = L(r, "Зіставте елементи.", "Сопоставьте элементы.", "Match the items."),
                  explanation = L(r, "Демо-питання (Mock).", "Демо-вопрос (Mock).", "Demo question (Mock)."),
                  pairs = new object[] {
                      new { left = L(r, "Означення", "Определение", "Definition"), right = L(r, "Що це таке", "Что это такое", "What it is") },
                      new { left = L(r, "Приклад", "Пример", "Example"), right = L(r, "Як це виглядає", "Как это выглядит", "How it looks") },
                      new { left = L(r, "Помилка", "Ошибка", "Mistake"), right = L(r, "Чого уникати", "Чего избегать", "What to avoid") } } },

            new { type = "Sequence", maxScore = 2, text = L(r, "Розташуйте кроки вивчення теми у правильному порядку.", "Расположите шаги изучения темы в правильном порядке.", "Put the study steps in the right order."),
                  explanation = L(r, "Спочатку теорія, потім практика.", "Сначала теория, потом практика.", "Theory first, then practice."),
                  items = new[] { L(r, "Прочитати лекцію", "Прочитать лекцию", "Read the lecture"), L(r, "Розібрати приклади", "Разобрать примеры", "Study the examples"), L(r, "Пройти тест", "Пройти тест", "Take the test") } },

            new { type = "TextAnswer", maxScore = 3, textEvaluation = "ManualTeacherReview",
                  text = L(r, $"Своїми словами поясніть, чому тема «{t}» важлива.", $"Своими словами объясните, почему тема «{t}» важна.", $"Explain in your own words why \"{t}\" matters."),
                  explanation = L(r, "Орієнтир: логічне пояснення з прикладом.", "Ориентир: логичное объяснение с примером.", "Reference: a logical explanation with an example."),
                  acceptedAnswers = Array.Empty<string>() }
        };

        return new
        {
            title = L(r, $"Тест: {t}", $"Тест: {t}", $"Test: {t}"),
            description = L(r, "Демонстраційний тест (режим Mock).", "Демонстрационный тест (режим Mock).", "A demo test (Mock mode)."),
            questions = questions.Take(Math.Clamp(r.Count, 1, questions.Count))
        };
    }

    private static object Grade(AiRequest r)
    {
        int length = r.Topic.Trim().Length;   // для оцінювання Topic містить текст відповіді студента
        double ratio = length < 15 ? 0.2 : length < 80 ? 0.6 : 0.85;
        return new
        {
            ratio,
            comment = L(r, "Демо-оцінка (режим Mock): орієнтується лише на обсяг відповіді. Перевірте вручну.",
                           "Демо-оценка (режим Mock): ориентируется только на объём ответа. Проверьте вручную.",
                           "Demo grade (Mock mode): based only on answer length. Please review manually.")
        };
    }
}
