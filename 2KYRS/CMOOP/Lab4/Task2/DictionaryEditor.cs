using Lab4.Task1;

namespace Lab4.Task2
{
    public class DictionaryEditor
    {
        private readonly DictionaryModel _dictionary;
        private readonly DictionaryRepository _repository;

        public DictionaryEditor(DictionaryModel dictionary, DictionaryRepository repository)
        {
            _dictionary = dictionary;
            _repository = repository;
        }

        public void SearchTranslation()
        {
            Console.WriteLine("\n--- ПОШУК ПЕРЕКЛАДУ ---");
            string word = ConsoleUi.ReadLine("Введіть слово для пошуку");
            if (string.IsNullOrWhiteSpace(word))
            {
                return;
            }

            WordEntry? entry = _dictionary.FindWord(word);
            if (entry == null)
            {
                Console.WriteLine($"Слово '{word}' не знайдено.");
                return;
            }

            Console.WriteLine($"\nСлово: {entry.Word}");
            Console.WriteLine("Варіанти перекладу:");
            foreach (string translation in entry.Translations)
            {
                Console.WriteLine($" - {translation}");
            }
        }

        public void AddWord()
        {
            Console.WriteLine("\n--- ДОДАВАННЯ СЛОВА ---");
            string word = ConsoleUi.ReadLine("Введіть нове слово");
            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine("Помилка: Слово не може бути порожнім.");
                return;
            }

            Console.WriteLine("Введіть варіанти перекладу через кому (наприклад: книга, том, книжка):");
            List<string> translations = ParseTranslations(Console.ReadLine());
            if (translations.Count == 0)
            {
                Console.WriteLine("Помилка: Вкажіть хоча б один переклад.");
                return;
            }

            WordEntry? existing = _dictionary.FindWord(word);
            if (existing == null)
            {
                _dictionary.Words.Add(new WordEntry(word, translations));
                Console.WriteLine($"[OK] Слово '{word}' додано.");
            }
            else
            {
                int addedCount = 0;
                foreach (string translation in translations)
                {
                    if (!existing.Translations.Contains(translation, StringComparer.OrdinalIgnoreCase))
                    {
                        existing.Translations.Add(translation);
                        addedCount++;
                    }
                }

                Console.WriteLine($"[OK] Слово вже існувало. Додано нових варіантів перекладу: {addedCount}.");
            }

            _repository.Save(_dictionary);
        }

        public void RunEditMenu()
        {
            while (true)
            {
                ConsoleUi.ShowHeader($"РЕДАГУВАННЯ У СЛОВНИКУ [{_dictionary.Title}]");
                Console.WriteLine("1. Замінити слово");
                Console.WriteLine("2. Замінити варіант перекладу");
                Console.WriteLine("0. <- Назад у меню словника");
                ConsoleUi.ShowFooter();

                switch (ConsoleUi.ReadLine("Виберіть дію"))
                {
                    case "1":
                        ReplaceWord();
                        ConsoleUi.Pause();
                        break;
                    case "2":
                        ReplaceTranslation();
                        ConsoleUi.Pause();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        ConsoleUi.Pause();
                        break;
                }
            }
        }

        public void RunDeleteMenu()
        {
            while (true)
            {
                ConsoleUi.ShowHeader($"ВИДАЛЕННЯ ЗІ СЛОВНИКА [{_dictionary.Title}]");
                Console.WriteLine("1. Видалити слово (з усіма перекладами)");
                Console.WriteLine("2. Видалити конкретний варіант перекладу");
                Console.WriteLine("0. <- Назад у меню словника");
                ConsoleUi.ShowFooter();

                switch (ConsoleUi.ReadLine("Виберіть дію"))
                {
                    case "1":
                        DeleteWord();
                        ConsoleUi.Pause();
                        break;
                    case "2":
                        DeleteTranslation();
                        ConsoleUi.Pause();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        ConsoleUi.Pause();
                        break;
                }
            }
        }

        public void ShowAllWords()
        {
            Console.WriteLine($"\n--- ВСІ СЛОВА У СЛОВНИКУ [{_dictionary.Title}] ---");
            if (_dictionary.Words.Count == 0)
            {
                Console.WriteLine("Словник порожній.");
                return;
            }

            foreach (WordEntry entry in _dictionary.Words)
            {
                Console.WriteLine($"{entry.Word} — {string.Join(", ", entry.Translations)}");
            }
        }

        private void ReplaceWord()
        {
            Console.WriteLine("\n--- ЗАМІНА СЛОВА ---");
            string oldWord = ConsoleUi.ReadLine("Введіть слово для зміни");

            WordEntry? entry = _dictionary.FindWord(oldWord);
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            string newWord = ConsoleUi.ReadLine($"Введіть нове значення замість '{entry.Word}'");
            if (string.IsNullOrWhiteSpace(newWord))
            {
                return;
            }

            entry.Word = newWord;
            _repository.Save(_dictionary);
            Console.WriteLine("[OK] Слово замінено.");
        }

        private void ReplaceTranslation()
        {
            Console.WriteLine("\n--- ЗАМІНА ПЕРЕКЛАДУ ---");
            string word = ConsoleUi.ReadLine("Введіть слово");

            WordEntry? entry = _dictionary.FindWord(word);
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            int? index = SelectTranslation(entry, "заміни");
            if (index == null)
            {
                return;
            }

            string newTranslation = ConsoleUi.ReadLine(
                $"Введіть новий переклад замість '{entry.Translations[index.Value - 1]}'");
            if (string.IsNullOrWhiteSpace(newTranslation))
            {
                return;
            }

            entry.Translations[index.Value - 1] = newTranslation;
            _repository.Save(_dictionary);
            Console.WriteLine("[OK] Переклад успішно замінено.");
        }

        private void DeleteWord()
        {
            Console.WriteLine("\n--- ВИДАЛЕННЯ СЛОВА ---");
            string word = ConsoleUi.ReadLine("Введіть слово для видалення");

            WordEntry? entry = _dictionary.FindWord(word);
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            _dictionary.Words.Remove(entry);
            _repository.Save(_dictionary);
            Console.WriteLine($"[OK] Слово '{entry.Word}' видалено.");
        }

        private void DeleteTranslation()
        {
            Console.WriteLine("\n--- ВИДАЛЕННЯ ВАРІАНТА ПЕРЕКЛАДУ ---");
            string word = ConsoleUi.ReadLine("Введіть слово");

            WordEntry? entry = _dictionary.FindWord(word);
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            // Правило з методички: не можна видалити останній переклад
            if (entry.Translations.Count <= 1)
            {
                Console.WriteLine("[ПОМИЛКА] У цього слова залишився тільки один варіант перекладу!");
                Console.WriteLine("Неможливо видалити останній переклад.");
                return;
            }

            int? index = SelectTranslation(entry, "видалення");
            if (index == null)
            {
                return;
            }

            string removed = entry.Translations[index.Value - 1];
            entry.Translations.RemoveAt(index.Value - 1);
            _repository.Save(_dictionary);
            Console.WriteLine($"[OK] Переклад '{removed}' видалено.");
        }

        private static List<string> ParseTranslations(string? input)
        {
            return (input ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(translation => translation.Trim())
                .Where(translation => !string.IsNullOrEmpty(translation))
                .ToList();
        }

        private static int? SelectTranslation(WordEntry entry, string action)
        {
            Console.WriteLine("Наявні переклади:");
            for (int i = 0; i < entry.Translations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {entry.Translations[i]}");
            }

            int? index = ConsoleUi.ReadIndex(
                $"Виберіть номер перекладу для {action}", 1, entry.Translations.Count);

            if (index == null)
            {
                Console.WriteLine("Невірний вибір.");
            }

            return index;
        }
    }
}
