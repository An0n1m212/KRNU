using Lab3.Task1;
using Lab3.Task2;
using Lab3.Task3;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

while (true)
{
    Console.Clear();
    Console.WriteLine("   Лабораторна робота № 3");
    Console.WriteLine("1. Завдання 1: Статистика файлу");
    Console.WriteLine("2. Завдання 2: Додаток «Цензор»");
    Console.WriteLine("3. Завдання 3: Колекція пісень");
    Console.WriteLine("0. Вихід");
    Console.Write("Оберіть номер завдання: ");

    string choice = Console.ReadLine();
    Console.Clear();

    switch (choice)
    {
        case "1":
            await Task1.Run();
            break;
        case "2":
            await Task2.Run();
            break;
        case "3":
            await Task3.Run();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Невірний вибір. Натисніть Enter...");
            break;
    }

    Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися до меню...");
    Console.ReadKey();
}
