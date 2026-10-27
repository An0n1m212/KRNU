using Lab3.Common;

namespace Lab3.Task3
{
    public static class Task3
    {
        private static SongCollectionManager manager = new SongCollectionManager();
        private const string DefaultFilePath = "songs_db.json";

        public static async Task Run()
        {
            if (File.Exists(DefaultFilePath))
            {
                await manager.LoadFromFileAsync(DefaultFilePath);
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("ЗАВДАННЯ 3: Колекція пісень");
                Console.WriteLine($"Всього пісень у базі: {manager.GetAllSongs().Count}");
                Console.WriteLine("1. Додати пісню");
                Console.WriteLine("2. Видалити пісню");
                Console.WriteLine("3. Редагувати інформацію про пісню");
                Console.WriteLine("4. Пошук пісні за характеристиками");
                Console.WriteLine("5. Вивести пісні вказаного виконавця");
                Console.WriteLine("6. Показати всі пісні");
                Console.WriteLine("7. Зберегти колекцію у файл (вибір директорії)");
                Console.WriteLine("8. Завантажити колекцію з файлу (вибір директорії)");
                Console.WriteLine("0. Повернутися до головного меню");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1":
                        AddNewSong();
                        break;
                    case "2":
                        RemoveExistingSong();
                        break;
                    case "3":
                        EditExistingSong();
                        break;
                    case "4":
                        SearchSongs();
                        break;
                    case "5":
                        ShowByPerformer();
                        break;
                    case "6":
                        manager.DisplayAll();
                        break;
                    case "7":
                        await SaveCollectionToFile();
                        break;
                    case "8":
                        await LoadCollectionFromFile();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        private static void AddNewSong()
        {
            Console.WriteLine("--- Додавання нової пісні ---");
            Song song = new Song();

            Console.Write("Введіть назву пісні: ");
            song.Title = Console.ReadLine();

            Console.Write("Введіть П.І.Б. автора слів: ");
            song.Author = Console.ReadLine();

            Console.Write("Введіть композитора: ");
            song.Composer = Console.ReadLine();

            Console.Write("Введіть рік написання: ");
            int.TryParse(Console.ReadLine(), out int year);
            song.Year = year;

            Console.Write("Введіть текст пісні: ");
            song.Lyrics = Console.ReadLine();

            Console.Write("Введіть виконавців через кому: ");
            string perfInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(perfInput))
            {
                song.Performers = perfInput.Split(',')
                                           .Select(p => p.Trim())
                                           .Where(p => !string.IsNullOrEmpty(p))
                                           .ToList();
            }

            manager.AddSong(song);
            Console.WriteLine("\nПісню успішно додано!");
        }

        private static void RemoveExistingSong()
        {
            Console.WriteLine("--- Видалення пісні ---");
            Console.Write("Введіть назву пісні для видалення: ");
            string title = Console.ReadLine();

            if (manager.RemoveSong(title))
            {
                Console.WriteLine("Пісню успішно видалено.");
            }
            else
            {
                Console.WriteLine("Пісню з такою назвою не знайдено.");
            }
        }

        private static void EditExistingSong()
        {
            Console.WriteLine("--- Редагування пісні ---");
            Console.Write("Введіть назву пісні для редагування: ");
            string title = Console.ReadLine();

            Song song = manager.GetByTitle(title);
            if (song == null)
            {
                Console.WriteLine("Пісню з такою назвою не знайдено.");
                return;
            }

            Console.Write($"Нова назва (натисніть Enter, щоб залишити '{song.Title}'): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) song.Title = input;

            Console.Write($"Новий автор (натисніть Enter, щоб залишити '{song.Author}'): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) song.Author = input;

            Console.Write($"Новий композитор (натисніть Enter, щоб залишити '{song.Composer}'): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) song.Composer = input;

            Console.Write($"Новий рік (натисніть Enter, щоб залишити '{song.Year}'): ");
            input = Console.ReadLine();
            if (int.TryParse(input, out int newYear)) song.Year = newYear;

            Console.Write($"Новий текст (натисніть Enter, щоб залишити поточний): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) song.Lyrics = input;

            Console.Write($"Нові виконавці через кому (натисніть Enter, щоб залишити поточних): ");
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                song.Performers = input.Split(',').Select(p => p.Trim()).ToList();
            }

            Console.WriteLine("\nІнформацію про пісню оновлено.");
        }

        private static void SearchSongs()
        {
            Console.WriteLine("--- Пошук пісні ---");
            Console.Write("Введіть пошуковий запит (назва, автор, композитор або рік): ");
            string query = Console.ReadLine();

            var results = manager.GeneralSearch(query);
            if (results.Any())
            {
                Console.WriteLine($"\nЗнайдено {results.Count} пісень:\n");
                results.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("Пісень за вашим запитом не знайдено.");
            }
        }

        private static void ShowByPerformer()
        {
            Console.WriteLine("--- Пошук за виконавцем ---");
            Console.Write("Введіть ім'я/назву виконавця: ");
            string performer = Console.ReadLine();

            var results = manager.SearchByPerformer(performer);
            if (results.Any())
            {
                Console.WriteLine($"\nПісні, які виконує \"{performer}\":\n");
                results.ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine($"Пісень у виконанні \"{performer}\" не знайдено.");
            }
        }

        /// Збереження: користувач обирає директорію, потім файл (або вводить нове ім'я).
        private static async Task SaveCollectionToFile()
        {
            Console.WriteLine("--- Збереження колекції у файл ---");

            string directory = FileUi.PromptDirectory();
            string? filePath = FileUi.PickFileInDirectory(directory, "*.json");

            if (filePath == null)
            {
                // Файл не обрано — пропонуємо створити новий у цій директорії.
                Console.Write($"Введіть ім'я нового файлу (Enter — {DefaultFilePath}): ");
                string name = Console.ReadLine()?.Trim('"') ?? "";
                if (string.IsNullOrWhiteSpace(name)) name = DefaultFilePath;
                filePath = Path.Combine(directory, name);
            }

            await manager.SaveToFileAsync(filePath);
        }

        ///Завантаження: користувач обирає директорію та JSON-файл у ній.
        private static async Task LoadCollectionFromFile()
        {
            Console.WriteLine("--- Завантаження колекції з файлу ---");

            string directory = FileUi.PromptDirectory();
            string? filePath = FileUi.PickFileInDirectory(directory, "*.json");

            if (filePath == null)
            {
                Console.WriteLine("Завантаження скасовано.");
                return;
            }

            await manager.LoadFromFileAsync(filePath);
        }
    }
}
