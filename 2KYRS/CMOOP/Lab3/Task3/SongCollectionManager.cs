using Lab3.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Lab3.Task3
{
    public class SongCollectionManager
    {
        private List<Song> songs = new List<Song>();

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public List<Song> GetAllSongs() => songs;

        public void AddSong(Song song) => songs.Add(song);

        public bool RemoveSong(string title)
        {
            var song = songs.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (song != null)
            {
                songs.Remove(song);
                return true;
            }
            return false;
        }

        public Song GetByTitle(string title)
        {
            return songs.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public List<Song> SearchByPerformer(string performer)
        {
            return songs.Where(s => s.Performers.Any(p => p.Equals(performer, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public List<Song> SearchByTitle(string title)
        {
            return songs.Where(s => s.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Song> GeneralSearch(string query)
        {
            return songs.Where(s =>
                s.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Composer.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Year.ToString().Contains(query)
            ).ToList();
        }

        public void DisplayAll()
        {
            if (!songs.Any())
            {
                Console.WriteLine("Колекція порожня.");
                return;
            }
            Console.WriteLine("\n--- ВСІ ПІСНІ В КОЛЕКЦІЇ ---");
            foreach (var song in songs)
            {
                Console.WriteLine(song);
            }
        }

        /// Асинхронне збереження. Якщо колекція велика — серіалізуємо напряму в файл через
        /// JsonSerializer.SerializeAsync (потоковий запис без проміжного рядка в пам'яті).
        public async Task SaveToFileAsync(string filePath)
        {
            try
            {
                await using var stream = FileUi.OpenWrite(filePath);

                await JsonSerializer.SerializeAsync(stream, songs, JsonOptions);
                Console.WriteLine($"Колекцію успішно збережено у файл: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження: {ex.Message}");
            }
        }

        /// Асинхронне завантаження. Великий JSON-файл (>256 КБ) читаємо потоково через
        /// JsonSerializer.DeserializeAsyncEnumerable — колекція прийде чанк за чанком.
        /// Малий файл — звичайне DeserializeAsync з пам'яті.
        public async Task LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не знайдено!");
                return;
            }

            try
            {
                bool isLarge = FileUi.IsLargeFile(filePath);
                var loaded = new List<Song>();

                if (isLarge)
                {
                    await using var stream = FileUi.OpenRead(filePath);

                    await foreach (Song? song in JsonSerializer.DeserializeAsyncEnumerable<Song>(
                        stream, JsonOptions, cancellationToken: default))
                    {
                        if (song != null)
                            loaded.Add(song);
                    }
                }
                else
                {
                    await using var stream = FileUi.OpenRead(filePath);

                    loaded = await JsonSerializer.DeserializeAsync<List<Song>>(stream, JsonOptions)
                             ?? new List<Song>();
                }

                songs = loaded;
                Console.WriteLine($"Колекцію успішно завантажено з файлу ({songs.Count} пісень).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження файлу: {ex.Message}");
            }
        }
    }
}
