using Lab4.Task1;

namespace Lab4.Task2
{
    public class DictionaryExporter
    {
        private readonly DictionaryModel _dictionary;

        public DictionaryExporter(DictionaryModel dictionary)
        {
            _dictionary = dictionary;
        }

        public void ExportWord()
        {
            Console.WriteLine("\n--- ЕКСПОРТ СЛОВА У ФАЙЛ ---");
            string word = ConsoleUi.ReadLine("Введіть слово для експорту");

            WordEntry? entry = _dictionary.FindWord(word);
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            string fileName = $"Export_{entry.Word}.txt";
            string content = $"Словник: {_dictionary.Title}\n" +
                             $"Слово: {entry.Word}\n" +
                             $"Варіанти перекладу:\n" +
                             string.Join("\n", entry.Translations.Select((t, i) => $"  {i + 1}. {t}")) + "\n" +
                             $"Дата експорту: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";

            File.WriteAllText(fileName, content);
            Console.WriteLine($"[OK] Слово збережено у файл: {Path.GetFullPath(fileName)}");
        }
    }
}
