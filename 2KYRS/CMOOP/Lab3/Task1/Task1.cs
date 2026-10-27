using System.Text.RegularExpressions;

namespace Lab3.Task1
{
    using Lab3.Common;

    public static class Task1
    {
        // Речення = будь-який відрізок тексту, закінчений на '.', '!' або '?' (або останній відрізок без знака).
        // Регулярка шукає послідовності символів, які НЕ є кінцівками речення — це і є речення.
        private static readonly Regex SentenceRegex = new Regex(@"[^.!?]+", RegexOptions.Compiled);

        private static readonly char[] Vowels = new char[] {
            'а', 'е', 'є', 'и', 'і', 'ї', 'о', 'у', 'ю', 'я',
            'A', 'E', 'I', 'O', 'U', 'Y', 'a', 'e', 'i', 'o', 'u', 'y'
        };

        public static async Task Run()
        {
            Console.WriteLine("ЗАВДАННЯ 1: Аналіз статистики файлу\n");

            string directory = FileUi.PromptDirectory();
            string? filePath = FileUi.PickFileInDirectory(directory);

            if (filePath == null)
            {
                Console.WriteLine("Файл не обрано. Повернення до меню.");
                return;
            }

            bool isLarge = FileUi.IsLargeFile(filePath);
            Console.WriteLine($"\nОбрано файл: {filePath}");
            Console.WriteLine($"Розмір: {FileUi.FormatFileSize(filePath)}" +
                (isLarge ? "  → великий файл: асинхронний потік (IAsyncEnumerable), читаємо рядок за рядком" : ""));

            try
            {
                var (stats, preview) = isLarge
                    ? await AnalyzeByStreamAsync(filePath)
                    : await AnalyzeAllAtOnceAsync(filePath);

                Console.WriteLine("\nПрочитані перші рядки файлу:");                
                foreach (string line in preview)
                {
                    string cut = line.Length > 100 ? line[..100] + "…" : line;
                    Console.WriteLine($"   {cut}");
                }

                Console.WriteLine("\n--- Результати аналізу ---");
                Console.WriteLine($"Кількість речень: {stats.Sentences}");
                Console.WriteLine($"Кількість великих літер: {stats.UpperLetters}");
                Console.WriteLine($"Кількість маленьких літер: {stats.LowerLetters}");
                Console.WriteLine($"Кількість голосних літер: {stats.VowelsCount}");
                Console.WriteLine($"Кількість приголосних літер: {stats.ConsonantsCount}");
                Console.WriteLine($"Кількість цифр: {stats.DigitsCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні файлу: {ex.Message}");
            }
        }

        /// Малий файл: зчитуємо цілком асинхронно (ReadAllTextAsync) і рахуємо в пам'яті.
        private static async Task<(TextStats Stats, List<string> Preview)> AnalyzeAllAtOnceAsync(string filePath)
        {
            string text = await FileUi.ReadAllTextAsync(filePath);

            var stats = new TextStats();
            var preview = new List<string>();

            foreach (string line in text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                stats.AddLine(line);

                if (preview.Count < 10)
                    preview.Add(line);
            }

            return (stats, preview);
        }

        /// Великий файл: асинхронний потік рядків (IAsyncEnumerable) — файл не зберігається в пам'яті цілком,
        /// статистика накопичується по мірі читання.
        private static async Task<(TextStats Stats, List<string> Preview)> AnalyzeByStreamAsync(string filePath)
        {
            var stats = new TextStats();
            var preview = new List<string>();

            await foreach (string line in FileUi.ReadLinesAsync(filePath))
            {
                stats.AddLine(line);

                if (preview.Count < 10)
                    preview.Add(line);
            }

            return (stats, preview);
        }

        private sealed class TextStats
        {
            public int Sentences;
            public int UpperLetters;
            public int LowerLetters;
            public int VowelsCount;
            public int ConsonantsCount;
            public int DigitsCount;

            public void AddLine(string line)
            {
                Sentences += SentenceRegex.Matches(line).Count;
                UpperLetters += line.Count(char.IsUpper);
                LowerLetters += line.Count(char.IsLower);
                DigitsCount += line.Count(char.IsDigit);
                VowelsCount += line.Count(c => Vowels.Contains(char.ToLower(c)));
                ConsonantsCount += line.Count(c => char.IsLetter(c) && !Vowels.Contains(char.ToLower(c)));
            }
        }
    }
}
