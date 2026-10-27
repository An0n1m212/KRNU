using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Nomer3.Interface;

namespace Nomer3.Manager
{
    public static class Manager
    {
        private static string[] _sourceData = new string[0];
        private const string InputFile = "input.txt";
        private const string OutputFile = "results.txt";

        public static bool HasData => _sourceData.Length > 0;

        public static void SetData(string input)
        {
            _sourceData = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"Успішно занесено.");


        }

        public static void LoadFromFile()
        {
            if (File.Exists(InputFile))
            {
                _sourceData = File.ReadAllLines(InputFile);
                Console.WriteLine($"Успішно завантажено {_sourceData.Length} рядків.");

            }
            else
            {
                Console.WriteLine("Помилка: Файл input.txt не знайдено.");
            }
        }

        public static void ProcessAndSave<T>() where T : class, ICipher, IComparable<T>
        {
            if (!HasData)
            {
                Console.WriteLine("Дані для обробки відсутні!");
                return;
            }

            // Створення об'єктів через активатор
            T[] items = _sourceData
                .Select(s => (T)Activator.CreateInstance(typeof(T), s)!)
                .ToArray();

            // Фільтрація дублікатів та сортування за зашифрованим значенням
            var result = items
                              .Where(x => x != null)
                              .GroupBy(x => ((dynamic)x).Data)
                              .Select(g => g.First())
                              .ToList();
            result.Sort();

            SaveResultsToFile<T>(result);
        }

        private static void SaveResultsToFile<T>(System.Collections.Generic.List<T> result) where T : ICipher
        {
            using (StreamWriter sw = new StreamWriter(OutputFile, true))
            {
                sw.WriteLine($"\n--- {typeof(T).Name} | {DateTime.Now:dd.MM.yyyy HH:mm} ---");

                Console.WriteLine($"\nРезультати {typeof(T).Name}:");
                foreach (var item in result)
                {
                    string encoded = ((dynamic)item).Data;
                    string decoded = item.Decode(encoded);
                    string outputLine = $"Заш.: {encoded} | Розш.: {decoded}";

                    Console.WriteLine(outputLine);
                    sw.WriteLine(outputLine);
                }
            }
            Console.WriteLine($"\nДані додано у файл: {OutputFile}");
        }

        public static void Wait()
        {
            Console.Write(" Натисніть клавішу для продовження.");
            Console.ReadKey();
        }

        public static void ShowMenu()
        {
            Console.WriteLine("============================");
            Console.WriteLine("      МЕНЮ ШИФРУВАННЯ       ");
            Console.WriteLine("============================");
            Console.WriteLine("1. Введення з клавіатури");
            Console.WriteLine("2. Завантажити з input.txt");
            Console.WriteLine("3. Обробити ACipher (Зсув)");
            Console.WriteLine("4. Обробити BCipher (Дзеркало)");
            Console.WriteLine("0. Вихід");
            Console.Write("\nВаш вибір: ");
        }
    }
}

