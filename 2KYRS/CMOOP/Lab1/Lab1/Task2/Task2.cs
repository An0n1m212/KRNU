public static class Task2
{
    private const string IndexFileName = "firstFile.txt";

    public static void Run()
    {
        EnsureSampleFilesExist();

        while (true)
        {
            Console.WriteLine("\n--- ЗАВДАННЯ 2: Аналіз текстових файлів (з LINQ) ---");
            if (!File.Exists(IndexFileName))
            {
                Console.WriteLine($"Файл-індекс {IndexFileName} не знайдено.");
                return;
            }

            string[] availableFiles = File.ReadAllLines(IndexFileName);
            Console.WriteLine("Доступні файли для аналізу:");
            for (int i = 0; i < availableFiles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {availableFiles[i]}");
            }
            Console.WriteLine("0. Повернутися до головного меню");

            Console.Write("Оберіть номер файлу для аналізу: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0) return;

            if (choice > 0 && choice <= availableFiles.Length)
            {
                AnalyzeFile(availableFiles[choice - 1].Trim());
            }
            else
            {
                Console.WriteLine("Невірний номер.");
            }
        }
    }

    private static void AnalyzeFile(string fileName) // Познайомитися з Dictionary
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Файл '{fileName}' не існує на диску.");
            return;
        }

        string text = File.ReadAllText(fileName).ToLower();

        char[] separators = new char[]
        {
            ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '-', '—',
            '(', ')', '[', ']', '{', '}', '"', '«', '»', '/', '\\'
        };

        var wordStats = text
                            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w.Trim('\'', '`'))
                            .Where(w => !string.IsNullOrWhiteSpace(w))
                            .GroupBy(w => w)
                            .Select(g => new { Word = g.Key, Count = g.Count() })
                            .OrderByDescending(x => x.Count)
                            .ThenBy(x => x.Word);

        Console.WriteLine($"\nСтатистика слів для файлу '{fileName}':");
        foreach (var item in wordStats)
        {
            Console.WriteLine($"{item.Word}: {item.Count}");
        }

        Console.Write("\nЗберегти результат у файл? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            string outputFileName = $"stat_{Path.GetFileNameWithoutExtension(fileName)}.txt";

            File.WriteAllLines(
                outputFileName,
                Enumerable.Repeat($"Статистика слів для файлу {fileName}:", 1)
                          .Concat(wordStats.Select(x => $"{x.Word}: {x.Count}"))
            );

            Console.WriteLine($"Результати збережено у файл '{outputFileName}'.");
        }
    }

    private static void EnsureSampleFilesExist()
    {
        if (!File.Exists(IndexFileName))
        {
            File.WriteAllLines(IndexFileName, new string[] { "sample1.txt", "sample2.txt" });
            File.WriteAllText("sample1.txt", "Привіт світ. Привіт програмування на C#. Світ великий.");
            File.WriteAllText("sample2.txt", "Колекції List, Queue, PriorityQueue та Dictionary в мові C#.");
        }
    }
}