using Lab3.Common;
using System.Text.RegularExpressions;

namespace Lab3.Task2
{
    public static class Task2
    {
        public static async Task Run()
        {
            Console.WriteLine("--- ЗАВДАННЯ 2: Додаток «Цензор» ---\n");

            Console.WriteLine("Оберіть директорію, у якій лежать файли для цензурування.");
            string directory = FileUi.PromptDirectory();

            Console.WriteLine("\nКрок 1: оберіть файл з текстом.");
            string? textFilePath = FileUi.PickFileInDirectory(directory);
            if (textFilePath == null)
            {
                Console.WriteLine("Файл з текстом не обрано. Повернення до меню.");
                return;
            }

            Console.WriteLine("\nКрок 2: оберіть файл зі стоп-словами (кожне слово — окремий рядок).");
            string? wordsFilePath = FileUi.PickFileInDirectory(directory);
            if (wordsFilePath == null)
            {
                Console.WriteLine("Файл зі словами не обрано. Повернення до меню.");
                return;
            }

            try
            {
                bool textIsLarge = FileUi.IsLargeFile(textFilePath);
                bool wordsIsLarge = FileUi.IsLargeFile(wordsFilePath);

                Console.WriteLine($"\nФайл з текстом: {textFilePath}");
                Console.WriteLine($"   Розмір: {FileUi.FormatFileSize(textFilePath)}" +
                    (textIsLarge ? " → великий: читаємо асинхронним потоком построчно" : ""));
                Console.WriteLine($"Файл зі словами: {wordsFilePath}");
                Console.WriteLine($"   Розмір: {FileUi.FormatFileSize(wordsFilePath)}" +
                    (wordsIsLarge ? " → великий: читаємо асинхронним потоком построчно" : ""));

                // Стоп-слова: якщо файл великий — асинхронний потік рядків, інакше читаємо цілком.
                HashSet<string> stopWords = wordsIsLarge
                    ? await ReadStopWordsByStreamAsync(wordsFilePath)
                    : (await FileUi.ReadAllLinesAsync(wordsFilePath))
                        .Select(w => w.Trim())
                        .Where(w => w.Length > 0)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (stopWords.Count == 0)
                {
                    Console.WriteLine("Файл зі словами порожній або не містить валідних слів.");
                    return;
                }

                Console.WriteLine($"\nЗавантажено стоп-слів: {stopWords.Count}");

                string resultFilePath = Path.Combine(
                    Path.GetDirectoryName(textFilePath) ?? ".",
                    "censor_result.txt");

                List<string> preview;

                if (textIsLarge)
                {
                    // Великий файл: потік рядок за рядком, цензура на льоту, результат одразу у файл.
                    preview = await CensorByStreamAsync(textFilePath, resultFilePath, stopWords);
                    Console.WriteLine("Обробка: рядок за рядком через IAsyncEnumerable (текст не тримається в пам'яті цілком).");
                }
                else
                {
                    // Малий файл: читаємо цілком, цензуруємо, пишемо результат асинхронно.
                    string original = await FileUi.ReadAllTextAsync(textFilePath);
                    string censored = CensorText(original, stopWords);
                    await FileUi.WriteAllTextAsync(resultFilePath, censored);
                    preview = censored
                        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Take(20)
                        .ToList();
                    Console.WriteLine("Обробка: файл прочитано цілком (ReadAllTextAsync) і оброблено разом.");
                }

                Console.WriteLine("\nОригінальний текст (перші 20 рядків):");
                await PrintPreviewAsync(textFilePath);

                Console.WriteLine("\nЗацензурований текст (перші 20 рядків):");
                foreach (string line in preview)
                {
                    string cut = line.Length > 100 ? line[..100] + "…" : line;
                    Console.WriteLine($"   {cut}");
                }

                Console.WriteLine($"\n[Результат збережено у файл: {Path.GetFullPath(resultFilePath)}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час цензурування: {ex.Message}");
            }
        }

        /// <summary>Прев'ю оригінального файлу: перші 20 рядків (для великих теж безпечне).</summary>
        private static async Task PrintPreviewAsync(string filePath)
        {
            int count = 0;
            await foreach (string line in FileUi.ReadLinesAsync(filePath))
            {
                string cut = line.Length > 100 ? line[..100] + "…" : line;
                Console.WriteLine($"   {cut}");
                count++;
                if (count >= 20) break;
            }
        }

        /// <summary>Читає стоп-слова з великого файла потоково (IAsyncEnumerable).</summary>
        private static async Task<HashSet<string>> ReadStopWordsByStreamAsync(string wordsFilePath)
        {
            var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            await foreach (string line in FileUi.ReadLinesAsync(wordsFilePath))
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0)
                    words.Add(trimmed);
            }

            return words;
        }

        /// <summary>
        /// Великий файл: читаємо рядок за рядком асинхронним потоком, цензуруємо кожен рядок
        /// і одразу віддаємо його у FileUi.WriteLinesAsync, який сам відкриває та закриває
        /// потік запису. Весь текст у пам'яті не тримається — повертаємо лише прев'ю.
        /// </summary>
        private static async Task<List<string>> CensorByStreamAsync(
            string textFilePath, string resultFilePath, HashSet<string> stopWords)
        {
            var preview = new List<string>();

            await FileUi.WriteLinesAsync(resultFilePath, CensoredLinesAsync(textFilePath, stopWords, preview));

            return preview;
        }

        /// <summary>Потік зацензурованих рядків; попутно збирає прев'ю перших 20 рядків.</summary>
        private static async IAsyncEnumerable<string> CensoredLinesAsync(
            string textFilePath, HashSet<string> stopWords, List<string> preview)
        {
            await foreach (string line in FileUi.ReadLinesAsync(textFilePath))
            {
                string censored = CensorText(line, stopWords);

                if (preview.Count < 20)
                    preview.Add(censored);

                yield return censored;
            }
        }

        /// <summary>Застосовує заміну стоп-слів на зірочки (реєстронезалежно).</summary>
        private static string CensorText(string text, HashSet<string> stopWords)
        {
            foreach (string word in stopWords)
            {
                text = Regex.Replace(
                    text,
                    Regex.Escape(word),
                    new string('*', word.Length),
                    RegexOptions.IgnoreCase);
            }
            return text;
        }
    }
}
