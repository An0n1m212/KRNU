using System;
using System.Linq;
using System.Text;

namespace Nomer
{
    class Program
    {
        static Student[] students = new Student[0];

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.Unicode;

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ГОЛОВНЕ МЕНЮ ===");
                Console.WriteLine("1. Додати студента");
                Console.WriteLine("2. Знайти за прізвищем (+ повна інфо)");
                Console.WriteLine("3. Додати іспит студенту");
                Console.WriteLine("4. Вивести список всіх (коротко)");
                Console.WriteLine("5. 8 LAB");
                Console.WriteLine("0. Вихід");
                Console.Write("\nВаш вибір: ");

                switch (Console.ReadLine())
                {
                    case "1": Method.AddStudent(); Method.Wait(); break;
                    case "2": Method.SearchStudent(); Method.Wait(); break;
                    case "3": Method.AddExamToStudent(); Method.Wait(); break;
                    case "4": Method.ShowAllStudents(); Method.Wait(); break;
                    case "5": Method.ExecuteTask(); Method.Wait();break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Помилка: оберіть число від 0 до 4."); Method.Wait(); break;
                }
            }
        }

    }
}