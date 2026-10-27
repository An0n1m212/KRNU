using Lab4;
using Lab4.Task1.Lab4App;
using Lab4.Task2;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var task1Service = new Task1Service();
var task2Service = new Task2Service();

while (true)
{
    Console.Clear();
    Console.WriteLine("==================================================");
    Console.WriteLine("   ЛАБОРАТОРНА РОБОТА № 4: СЕРІАЛІЗАЦІЯ В C#");
    Console.WriteLine("==================================================");
    Console.WriteLine("1. Завдання 1 (Варіант 22: Клас «ЗАПИС»)");
    Console.WriteLine("2. Завдання 2 (Додаткове: Програма «Словники»)");
    Console.WriteLine("0. Вихід з програми");
    Console.WriteLine("--------------------------------------------------");
    Console.Write("Виберіть завдання: ");

    string? choice = Console.ReadLine()?.Trim();
    switch (choice)
    {
        case "1":
            task1Service.RunMenu();
            break;
        case "2":
            task2Service.RunMenu();
            break;
        case "0":
            Console.WriteLine("\nЗавершення роботи програми. До побачення!");
            return;
        default:
            Console.WriteLine("Некоректний вибір. Натисніть будь-яку клавішу...");
            Console.ReadKey();
            break;
    }
}