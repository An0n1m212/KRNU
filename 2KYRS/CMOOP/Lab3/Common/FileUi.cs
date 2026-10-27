namespace Lab3.Common
{
    /// <summary>
    /// Спільні допоміжні методи для всіх завдань:
    /// вибір файлу у директорії та асинхронне потокове читання великих файлів.
    /// </summary>
    public static class FileUi
    {
       public const long LargeFileThresholdBytes = 256 * 1024;

        public static bool IsLargeFile(string filePath) => new FileInfo(filePath).Length > LargeFileThresholdBytes;

        public static string FormatSize(long bytes)
        {
            const long kb = 1024;
            const long mb = 1024 * 1024;

            if (bytes < kb) return $"{bytes} Б";
            if (bytes < mb) return $"{(double)bytes / kb:F1} КБ";
            return $"{(double)bytes / mb:F1} МБ";
        }

        public static string FormatFileSize(string filePath) => FormatSize(new FileInfo(filePath).Length);

        public static string PromptDirectory(string? defaultDirectory = null)
        {
            string fallback = defaultDirectory ?? Directory.GetCurrentDirectory();

            while (true)
            {
                Console.Write($"Введіть шлях до директорії (Enter — поточна \"{fallback}\"): ");
                string input = Console.ReadLine()?.Trim('"') ?? "";

                string dir = string.IsNullOrWhiteSpace(input) ? fallback : input;

                if (Directory.Exists(dir))
                    return Path.GetFullPath(dir);

                Console.WriteLine("Такої директорії не існує. Спробуйте ще раз.\n");
            }
        }


        /// Показує список файлів директорії та дає обрати потрібний номером.
        /// "0" — ввести шлях вручну, Enter — скасувати (повертає null).
        /// <param name="directory">Директорія, у якій обираємо файл.</param>
        /// <param name="searchPattern">Шаблон, наприклад "*.json" (порожньо або "*" — усі файли).</param>
        public static string? PickFileInDirectory(string directory, string? searchPattern = null)
        {
            string pattern = string.IsNullOrWhiteSpace(searchPattern) ? "*" : searchPattern;
            string fullPath = Path.GetFullPath(directory);

            while (true)
            {
                var files = Directory.GetFiles(fullPath, pattern)
                                     .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                     .ToArray();

                Console.WriteLine($"\nФайли у директорії \"{fullPath}\" (шаблон: {pattern}):");
                if (files.Length == 0)
                    Console.WriteLine("   (файлів за цим шаблоном не знайдено)");

                const int maxShown = 50;
                int shown = Math.Min(files.Length, maxShown);

                for (int i = 0; i < shown; i++)
                {
                    var info = new FileInfo(files[i]);
                    Console.WriteLine($"  {i + 1,2}. {info.Name}  [{FormatSize(info.Length)}]");
                }
                if (files.Length > shown)
                    Console.WriteLine($"   ... ще {files.Length - shown} файлів (номерація продовжується)");

                Console.WriteLine("   0. Ввести шлях до файлу вручну");
                Console.WriteLine("  Enter — скасувати");

                Console.Write("Оберіть файл: ");
                string choice = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(choice))
                    return null;

                if (choice == "0")
                {
                    Console.Write("Введіть повний шлях до файлу: ");
                    string manual = Console.ReadLine()?.Trim('"') ?? "";

                    if (File.Exists(manual))
                        return Path.GetFullPath(manual);

                    Console.WriteLine("Такого файлу не існує. Спробуйте ще раз.\n");
                    continue;
                }

                if (int.TryParse(choice, out int index) && index >= 1 && index <= files.Length)
                    return files[index - 1];

                Console.WriteLine("Невірний номер. Спробуйте ще раз.\n");
            }
        }

        /// Асинхронний потік (async stream): читає файл рядок за рядком.
        /// Використовується для великих файлів, щоб не тримати весь текст у пам'яті.
        /// Потік відкривається лише тоді, коли починається реальне перебирання рядків.
        public static async IAsyncEnumerable<string> ReadLinesAsync(
            string filePath,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(
                filePath,
                System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 64 * 1024);

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                yield return line;
            }
        }

        /// Читає весь файл як текст (малі файли).
        public static Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default) =>
            File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8, cancellationToken);

        /// Читає весь файл у масив рядків (малі файли).
        public static Task<string[]> ReadAllLinesAsync(string filePath, CancellationToken cancellationToken = default) =>
            File.ReadAllLinesAsync(filePath, System.Text.Encoding.UTF8, cancellationToken);

        /// Записує текст у файл цілком (малі файли).
        public static Task WriteAllTextAsync(string filePath, string text, CancellationToken cancellationToken = default) =>
            File.WriteAllTextAsync(filePath, text, System.Text.Encoding.UTF8, cancellationToken);

        /// Записує послідовність рядків у файл потоково (великі файли).
        /// Потік запису відкривається і закривається тут, виклик лише надає рядки.
        public static async Task WriteLinesAsync(
            string filePath,
            IAsyncEnumerable<string> lines,
            CancellationToken cancellationToken = default)
        {
            await using var writer = new StreamWriter(OpenWrite(filePath), System.Text.Encoding.UTF8);

            await foreach (string line in lines.WithCancellation(cancellationToken))
            {
                await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
            }

            await writer.FlushAsync(cancellationToken);
        }

        /// Відкриває файл на читання (для потокової десеріалізації тощо).
        public static FileStream OpenRead(string filePath) =>
            new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 64 * 1024, useAsync: true);

        /// Відкриває файл на запис (для потокової серіалізації тощо).
        public static FileStream OpenWrite(string filePath) =>
            new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 64 * 1024, useAsync: true);
    }
}
