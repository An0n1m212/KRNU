using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task1
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    namespace Lab4App
    {
        public class Task1Service
        {
            private Record[] _records;

            public Task1Service()
            {
                // Ініціалізація початкового масиву об'єктів
                _records = new Record[]
                {
                new Record("Іваненко Іван Іванович",     "044-111-2233", "044-555-6677", "+380671234567", new DateTime(1995, 5, 15)),
                new Record("Петренко Петро Петрович",    "044-222-3344", "044-666-7788", "+380509876543", new DateTime(1998, 8, 22)),
                new Record("Сидоренко Анна Сергіївна",   "044-333-4455", "044-777-8899", "+380677778899", new DateTime(2001, 12, 3)),
                new Record("Коваленко Олексій Ігорович", "044-444-5566", "044-888-9900", "+380631112233", new DateTime(1992, 3, 30)),
                new Record("Мельник Ольга Миколаївна",  "044-555-6677", "044-999-0011", "+380505554433", new DateTime(1997, 10, 10))
                };
            }

            public void RunMenu()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("==================================================");
                    Console.WriteLine("  ЗАВДАННЯ 1 (Варіант 22: Клас «ЗАПИС»)");
                    Console.WriteLine("==================================================");
                    Console.WriteLine("1. Переглянути початковий масив об'єктів");
                    Console.WriteLine("2. XML серіалізація та десеріалізація (XmlSerializer)");
                    Console.WriteLine("3. JSON серіалізація та десеріалізація (JsonSerializer)");
                    Console.WriteLine("4. Результат запиту: Групування за мобільними операторами)");
                    Console.WriteLine("5. Виконати запит: Точковий парсинг великого JSON через JsonElement");
                    Console.WriteLine("0. <- Повернутися у Головне меню");
                    Console.WriteLine("--------------------------------------------------");
                    Console.Write("Виберіть підзавдання: ");

                    string? choice = Console.ReadLine()?.Trim();
                    switch (choice)
                    {
                        case "1":
                            ShowRecords(_records, "ПОЧАТКОВИЙ МАСИВ ЗАПИСІВ");
                            Pause();
                            break;
                        case "2":
                            ExecuteXmlSerialization();
                            Pause();
                            break;
                        case "3":
                            ExecuteJsonSerialization();
                            Pause();
                            break;
                        case "4":
                            ExecuteOperatorQuery();
                            Pause();
                            break;
                        case "5":
                            ExecuteLargeJsonParsing();
                            Pause();
                            break;
                        case "0":
                            return; // Повернення до головного меню
                        default:
                            Console.WriteLine("Некоректний вибір. Спробуйте ще раз.");
                            Pause();
                            break;
                    }
                }
            }

            private void ShowRecords(Record[] data, string title)
            {
                Console.WriteLine($"\n=== {title} ===");
                foreach (var r in data)
                {
                    Console.WriteLine(r);
                    Console.WriteLine(new string('-', 40));
                }
            }

            private void ExecuteXmlSerialization()
            {
                Console.WriteLine("\n--- 3. XML серіалізація (XmlSerializer) ---");
                string path = "records.xml";
                XmlSerializer serializer = new XmlSerializer(typeof(Record[]));

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(fs, _records);
                    Console.WriteLine($"[OK] Об'єкти збережено у XML-файл '{path}'");
                }

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Record[]? restored = serializer.Deserialize(fs) as Record[];
                    Console.WriteLine($"[OK] Об'єкти прочитано з XML-файлу. Прочитано елементів: {restored?.Length ?? 0}");
                }
            }

            private void ExecuteJsonSerialization()
            {
                Console.WriteLine("\n--- 4. JSON серіалізація (JsonSerializer) ---");
                string path = "records.json";
                var options = new JsonSerializerOptions { WriteIndented = true };

                string json = JsonSerializer.Serialize(_records, options);
                File.WriteAllText(path, json);
                Console.WriteLine($"[OK] Об'єкти збережено у JSON-файл '{path}'");

                string readJson = File.ReadAllText(path);
                Record[]? restored = JsonSerializer.Deserialize<Record[]>(readJson);
                Console.WriteLine($"[OK] Об'єкти прочитано з JSON-файлу. Прочитано елементів: {restored?.Length ?? 0}");
            }

            private void ExecuteOperatorQuery()
            {
                Console.WriteLine("\n--- 5. Результат запиту: Групування за мобільними операторами ---");

                var groups = _records
                    .GroupBy(r => r.GetMobileOperatorCode())
                    .Where(g => g.Count() > 1);

                bool found = false;
                foreach (var group in groups)
                {
                    found = true;
                    Console.WriteLine($"\n>>> Мобільний оператор (Код {group.Key}) — Абонентів: {group.Count()} <<<");
                    foreach (var person in group)
                    {
                        Console.WriteLine(person);
                        Console.WriteLine();
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Не знайдено кілька осіб із однаковим оператором.");
                }
            }
            private void ExecuteLargeJsonParsing()
            {
                Console.WriteLine("\n--- 6. Точковий парсинг великого JSON через JsonDocument ---");

                
                string simulatedApiResponse = @"
            {
                ""status"": ""success"",
                ""timestamp"": ""2026-10-24T14:30:00Z"",
                ""server_info"": {
                    ""region"": ""eu-central-1"",
                    ""load_average"": 0.12,
                    ""active_connections"": 4520
                },
                ""payload"": {
                    ""total_count"": 5,
                    ""page"": 1,
                    ""records_data"": [
                        {
                            ""FullName"": ""Иваненко Иван Иванович"",
                            ""HomePhone"": ""044-111-2233"",
                            ""WorkPhone"": ""044-555-6677"",
                            ""MobilePhone"": ""+380671234567"",
                            ""BirthDate"": ""1995-05-15T00:00:00""
                        },
                        {
                            ""FullName"": ""Петренко Петр Петрович"",
                            ""HomePhone"": ""044-222-3344"",
                            ""WorkPhone"": ""044-666-7788"",
                            ""MobilePhone"": ""+380509876543"",
                            ""BirthDate"": ""1998-08-22T00:00:00""
                        }
                    ]
                }
            }";

                Console.WriteLine("Считуєм входящий JSON и шукаем тільки массив 'records_data'...\n");

                // Парс поток/строк через JsonDocument без полной десериал ответа
                using (JsonDocument doc = JsonDocument.Parse(simulatedApiResponse))
                {
                    JsonElement root = doc.RootElement;

                    // Фаст доступ к нужному узлу по пути: payload -> records_data
                    if (root.TryGetProperty("payload", out JsonElement payloadElement) &&
                        payloadElement.TryGetProperty("records_data", out JsonElement recordsElement))
                    {
                        // Десериал онли найденный массив напрямую в наш класс Record[]
                        Record[]? extractedRecords = recordsElement.Deserialize<Record[]>();

                        if (extractedRecords != null)
                        {
                            Console.WriteLine($"[УСПЕХ] Извлечено записей из глубокого JSON: {extractedRecords.Length}\n");
                            foreach (var record in extractedRecords)
                            {
                                Console.WriteLine(record);
                                Console.WriteLine(new string('-', 30));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("[Помилка] Не вдалось знайти вузол 'records_data' в отриманном JSON.");
                    }
                }
            }
    private void Pause()
            {
                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }
    }
}
