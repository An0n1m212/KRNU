using Lab4.Task1;

namespace Lab4.Task2
{
    public class DictionaryWorkMenu
    {
        private readonly DictionaryModel _dictionary;
        private readonly DictionaryEditor _editor;
        private readonly DictionaryExporter _exporter;

        public DictionaryWorkMenu(DictionaryModel dictionary, DictionaryRepository repository)
        {
            _dictionary = dictionary;
            _editor = new DictionaryEditor(dictionary, repository);
            _exporter = new DictionaryExporter(dictionary);
        }

        public void Run()
        {
            while (true)
            {
                ConsoleUi.ShowHeader($"РОБОТА ЗІ СЛОВНИКОМ: [{_dictionary.Title}]");
                Console.WriteLine("1. Шукати переклад слова");
                Console.WriteLine("2. Додати слово та переклад(и)");
                Console.WriteLine("3. Замінити слово або переклад");
                Console.WriteLine("4. Видалити слово або переклад");
                Console.WriteLine("5. Показати всі слова у словнику");
                Console.WriteLine("6. Експортувати слово з перекладами у файл");
                Console.WriteLine("0. <- Назад до вибору словника");
                ConsoleUi.ShowFooter();

                switch (ConsoleUi.ReadLine("Виберіть дію"))
                {
                    case "1":
                        _editor.SearchTranslation();
                        ConsoleUi.Pause();
                        break;
                    case "2":
                        _editor.AddWord();
                        ConsoleUi.Pause();
                        break;
                    case "3":
                        _editor.RunEditMenu();
                        break;
                    case "4":
                        _editor.RunDeleteMenu();
                        break;
                    case "5":
                        _editor.ShowAllWords();
                        ConsoleUi.Pause();
                        break;
                    case "6":
                        _exporter.ExportWord();
                        ConsoleUi.Pause();
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
    }
}
