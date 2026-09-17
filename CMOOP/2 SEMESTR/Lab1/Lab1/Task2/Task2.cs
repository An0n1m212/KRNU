using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab1
{
    public static class Task2
    {
        private const string IndexFileName = "firstFile.txt";

        public static void Run()
        {
            EnsureSampleFilesExist();

            while (true)
            {
                Console.WriteLine("\n--- ЗАВДАННЯ 2: Аналіз текстових файлів ---");
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

        private static void AnalyzeFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Файл '{fileName}' не існує на диску.");
                return;
            }

            string text = File.ReadAllText(fileName).ToLower();
            MatchCollection matches = Regex.Matches(text, @"\b[\w\u0400-\u04FF']+\b");

            Dictionary<string, int> wordCounts = new Dictionary<string, int>();
            foreach (Match match in matches)
            {
                string word = match.Value;
                if (wordCounts.ContainsKey(word))
                {
                    wordCounts[word]++;
                }
                else
                {
                    wordCounts[word] = 1;
                }
            }

            List<KeyValuePair<string, int>> statsList = new List<KeyValuePair<string, int>>(wordCounts);

            for (int i = 0; i < statsList.Count - 1; i++)
            {
                for (int j = 0; j < statsList.Count - i - 1; j++)
                {
                    if (statsList[j].Value < statsList[j + 1].Value)
                    {
                        KeyValuePair<string, int> temp = statsList[j];
                        statsList[j] = statsList[j + 1];
                        statsList[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine($"\nСтатистика слів для файлу '{fileName}':");
            foreach (KeyValuePair<string, int> pair in statsList)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }

            Console.Write("\nЗберегти результат у файл? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                string outputFileName = $"stat_{Path.GetFileNameWithoutExtension(fileName)}.txt";
                using (StreamWriter sw = new StreamWriter(outputFileName))
                {
                    sw.WriteLine($"Статистика слів для файлу {fileName}:");
                    foreach (KeyValuePair<string, int> pair in statsList)
                    {
                        sw.WriteLine($"{pair.Key}: {pair.Value}");
                    }
                }
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
}
