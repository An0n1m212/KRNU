using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Lab1
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n================ ГОЛОВНЕ МЕНЮ ================");
                Console.WriteLine("1. Завдання 1 (Управління списком)");
                Console.WriteLine("2. Завдання 2 (Аналіз слів у файлах)");
                Console.WriteLine("3. Завдання 3 (Черга друку з пріоритетами)");
                Console.WriteLine("0. Вихід");
                Console.Write("Виберіть завдання: ");

                switch (Console.ReadLine())
                {
                    case "1": Task1.Run(); break;
                    case "2": Task2.Run(); break;
                    case "3": Task3.Run(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір. Спробуйте ще раз."); break;
                }
            }
        }
    }
}