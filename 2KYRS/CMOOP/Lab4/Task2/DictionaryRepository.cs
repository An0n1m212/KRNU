using Lab4.Task1;
using System.Text.Json;

namespace Lab4.Task2
{
    public class DictionaryRepository
    {
        private const string StorageDirectory = "Dictionaries";
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        public DictionaryRepository()
        {
            Directory.CreateDirectory(StorageDirectory);
        }

        public List<DictionaryModel> LoadAll()
        {
            var dictionaries = new List<DictionaryModel>();

            try
            {
                foreach (string file in Directory.GetFiles(StorageDirectory, "*.json"))
                {
                    string json = File.ReadAllText(file);
                    DictionaryModel? dictionary = JsonSerializer.Deserialize<DictionaryModel>(json);

                    if (dictionary != null)
                    {
                        dictionaries.Add(dictionary);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка завантаження]: {ex.Message}");
            }

            return dictionaries;
        }

        public void Save(DictionaryModel dictionary)
        {
            try
            {
                string safeName = string.Join("_", dictionary.Title.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(StorageDirectory, $"{safeName}.json");

                string json = JsonSerializer.Serialize(dictionary, SerializerOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка збереження]: {ex.Message}");
            }
        }
    }
}
