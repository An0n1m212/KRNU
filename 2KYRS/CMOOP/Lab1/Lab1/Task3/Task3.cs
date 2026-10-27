using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public static class Task3
    {
        private static PriorityQueue<PrintRequest, (Priority Priority, DateTime CreatedAt)> printQueue =
            new PriorityQueue<PrintRequest, (Priority Priority, DateTime CreatedAt)>(new PriorityComparer());

        private static List<PrintLog> printStatistics = new List<PrintLog>();

        public static void Run()
        {
            while (true)
            {
                Console.WriteLine("\n--- ЗАВДАННЯ 3: Черга друку принтера (PriorityQueue) ---");
                Console.WriteLine("1. Надіслати документ на друк");
                Console.WriteLine("2. Обробити наступний документ у черзі");
                Console.WriteLine("3. Обробити всі документи");
                Console.WriteLine("4. Переглянути поточний стан черги");
                Console.WriteLine("5. Вивести статистику друку");
                Console.WriteLine("6. Зберегти статистику у файл");
                Console.WriteLine("0. Повернутися до головного меню");
                Console.Write("Виберіть дію: ");

                switch (Console.ReadLine())
                {
                    case "1": AddToQueue(); break;
                    case "2": ProcessNext(); break;
                    case "3": ProcessAll(); break;
                    case "4": ShowQueue(); break;
                    case "5": DisplayStats(); break;
                    case "6": SaveStatsToFile(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір!"); break;
                }
            }
        }

        private static void AddToQueue()
        {
            Console.Write("Введіть ім'я користувача: ");
            string user = Console.ReadLine();
            Console.Write("Введіть назву документа: ");
            string doc = Console.ReadLine();

            Console.WriteLine("Оберіть пріоритет (1 - Low, 2 - Normal, 3 - High): ");
            int.TryParse(Console.ReadLine(), out int p);
            Priority priority = Enum.IsDefined(typeof(Priority), p) ? (Priority)p : Priority.Normal;

            PrintRequest request = new PrintRequest
            {
                UserName = user,
                DocumentName = doc,
                UserPriority = priority
            };

            printQueue.Enqueue(request, (request.UserPriority, request.CreatedAt));

            Console.WriteLine("Запит успішно додано до черги.");
        }

        private static void ProcessNext()
        {
            if (printQueue.Count == 0)
            {
                Console.WriteLine("Черга друку порожня.");
                return;
            }

            PrintRequest request = printQueue.Dequeue();

            PrintLog log = new PrintLog
            {
                UserName = request.UserName,
                DocumentName = request.DocumentName,
                PrintedAt = DateTime.Now
            };

            printStatistics.Add(log);
            Console.WriteLine($"Надруковано: {request.DocumentName} (Користувач: {request.UserName}, Пріоритет: {request.UserPriority})");
        }

        private static void ProcessAll()
        {
            if (printQueue.Count == 0)
            {
                Console.WriteLine("Черга друку порожня.");
                return;
            }

            while (printQueue.Count > 0)
            {
                ProcessNext();
            }
        }

        private static void ShowQueue()
        {
            Console.WriteLine("\nПоточний стан черги друку:");
            if (printQueue.Count == 0)
            {
                Console.WriteLine("Черга порожня.");
                return;
            }

            var unorderedItems = printQueue.UnorderedItems;
            List<PrintRequest> tempQueue = new List<PrintRequest>();

            foreach (var item in unorderedItems)
            {
                tempQueue.Add(item.Element);
            }

            PriorityComparer comparer = new PriorityComparer();

            tempQueue.Sort((x, y) => comparer.Compare(
                (x.UserPriority, x.CreatedAt),
                (y.UserPriority, y.CreatedAt)
            ));

            foreach (PrintRequest req in tempQueue)
            {
                Console.WriteLine($"[{req.UserPriority}] {req.UserName} - {req.DocumentName} ({req.CreatedAt:HH:mm:ss})");
            }
        }

        private static void DisplayStats()
        {
            Console.WriteLine("\nСтатистика друку:");
            if (printStatistics.Count == 0)
            {
                Console.WriteLine("Історія друку порожня.");
                return;
            }

            foreach (PrintLog log in printStatistics)
            {
                Console.WriteLine(log);
            }
        }

        private static void SaveStatsToFile()
        {
            if (printStatistics.Count == 0)
            {
                Console.WriteLine("Немає даних для збереження.");
                return;
            }

            string filename = "print_statistics.txt";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (PrintLog log in printStatistics)
                {
                    sw.WriteLine(log.ToString());
                }
            }
            Console.WriteLine($"Статистику збережено у файл '{filename}'.");
        }
    }

}
