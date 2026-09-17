using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public static class Task1
    {
        private static List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "Іван Петренко", Group = "КН-21", AverageScore = 88.5 },
            new Student { Id = 2, Name = "Марія Коваленко", Group = "КН-21", AverageScore = 95.0 },
            new Student { Id = 3, Name = "Олексій Сидоренко", Group = "КН-22", AverageScore = 74.2 }
        };

        public static void Run()
        {
            while (true)
            {
                Console.WriteLine("\n--- ЗАВДАННЯ 1: Управління списком екземплярів ---");
                Console.WriteLine("1. Показати всі елементи");
                Console.WriteLine("2. Додати новий елемент");
                Console.WriteLine("3. Редагувати елемент");
                Console.WriteLine("4. Видалити елемент");
                Console.WriteLine("5. Виконати запит (відмінники: бал >= 90)");
                Console.WriteLine("0. Повернутися до головного меню");
                Console.Write("Виберіть дію: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": DisplayAll(); break;
                    case "2": AddStudent(); break;
                    case "3": EditStudent(); break;
                    case "4": DeleteStudent(); break;
                    case "5": ExecuteQuery(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір!"); break;
                }
            }
        }

        private static void DisplayAll()
        {
            Console.WriteLine("\nПоточний список:");
            if (students.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            foreach (Student student in students)
            {
                Console.WriteLine(student);
            }
        }

        private static void AddStudent()
        {
            Console.Write("Введіть ПІБ: ");
            string name = Console.ReadLine();
            Console.Write("Введіть групу: ");
            string group = Console.ReadLine();
            Console.Write("Введіть середній бал: ");
            double.TryParse(Console.ReadLine(), out double score);

            int maxId = 0;
            foreach (Student s in students)
            {
                if (s.Id > maxId)
                {
                    maxId = s.Id;
                }
            }

            students.Add(new Student { Id = maxId + 1, Name = name, Group = group, AverageScore = score });
            Console.WriteLine("Елемент успішно додано!");
        }

        private static Student FindStudentById(int id)
        {
            foreach (Student s in students)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        private static void EditStudent()
        {
            Console.Write("Введіть ID для редагування: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Student student = FindStudentById(id);
                if (student != null)
                {
                    Console.Write($"Нове ім'я ({student.Name}): ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name)) student.Name = name;

                    Console.Write($"Нова група ({student.Group}): ");
                    string group = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(group)) student.Group = group;

                    Console.Write($"Новий бал ({student.AverageScore}): ");
                    if (double.TryParse(Console.ReadLine(), out double score)) student.AverageScore = score;

                    Console.WriteLine("Дані оновлено!");
                }
                else
                {
                    Console.WriteLine("Студента з таким ID не знайдено.");
                }
            }
        }

        private static void DeleteStudent()
        {
            Console.Write("Введіть ID для видалення: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Student student = FindStudentById(id);
                if (student != null)
                {
                    students.Remove(student);
                    Console.WriteLine("Елемент видалено!");
                }
                else
                {
                    Console.WriteLine("Елемент не знайдено.");
                }
            }
        }

        private static void ExecuteQuery()
        {
            List<Student> highAchievers = new List<Student>();
            foreach (Student s in students)
            {
                if (s.AverageScore >= 90.0)
                {
                    highAchievers.Add(s);
                }
            }

            Console.WriteLine("\nРезультат запиту (Студенти з балом >= 90):");
            if (highAchievers.Count > 0)
            {
                foreach (Student s in highAchievers)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Запиту не відповідає жоден елемент.");
            }
        }
    }
}
