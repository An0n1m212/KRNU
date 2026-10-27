using Lab4.Task1;

namespace Lab4.Task2
{
    public class Task2Service
    {
        private readonly DictionaryRepository _repository;
        private readonly List<DictionaryModel> _dictionaries;

        public Task2Service()
        {
            _repository = new DictionaryRepository();
            _dictionaries = _repository.LoadAll();
        }

        public void RunMenu()
        {
            while (true)
            {
                ConsoleUi.ShowHeader("ЗАВДАННЯ 2 (Додаткове: Програма «Словники»)");
                Console.WriteLine("1. Створити новий словник");
                Console.WriteLine("2. Вибрати словник для роботи");
                Console.WriteLine("0. <- Повернутися у Головне меню");
                ConsoleUi.ShowFooter();

                switch (ConsoleUi.ReadLine("Виберіть підзавдання"))
                {
                    case "1":
                        CreateDictionary();
                        ConsoleUi.Pause();
                        break;
                    case "2":
                        SelectDictionary();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некоректний вибір.");
                        ConsoleUi.Pause();
                        break;
                }
            }
        }

        private void CreateDictionary()
        {
            Console.WriteLine("\n--- СТВОРЕННЯ НОВОГО СЛОВНИКА ---");
            string title = ConsoleUi.ReadLine("Введіть тип/назву словника (наприклад, 'Англо-український')");

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Помилка: Назва словника не може бути порожньою.");
                return;
            }

            if (_dictionaries.Any(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Помилка: Словник з такою назвою вже існує.");
                return;
            }

            var dictionary = new DictionaryModel(title);
            _dictionaries.Add(dictionary);
            _repository.Save(dictionary);
            Console.WriteLine($"[OK] Словник '{title}' успішно створено та збережено у файл.");
        }

        private void SelectDictionary()
        {
            if (_dictionaries.Count == 0)
            {
                Console.WriteLine("\nСловники поки не створені.");
                ConsoleUi.Pause();
                return;
            }

            while (true)
            {
                ConsoleUi.ShowHeader("ВИБІР СЛОВНИКА ДЛЯ РОБОТИ");

                for (int i = 0; i < _dictionaries.Count; i++)
                {
                    DictionaryModel dictionary = _dictionaries[i];
                    Console.WriteLine($"{i + 1}. {dictionary.Title} (Слів: {dictionary.Words.Count})");
                }

                Console.WriteLine("0. <- Назад у меню Завдання 2");
                ConsoleUi.ShowFooter();

                int? index = ConsoleUi.ReadIndex("Виберіть номер словника", 0, _dictionaries.Count);
                if (index == null)
                {
                    Console.WriteLine("Невірний вибір.");
                    ConsoleUi.Pause();
                    continue;
                }

                if (index == 0)
                {
                    return;
                }

                new DictionaryWorkMenu(_dictionaries[index.Value - 1], _repository).Run();
            }
        }
    }
}
