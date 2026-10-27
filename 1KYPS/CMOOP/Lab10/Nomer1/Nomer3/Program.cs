using Nomer3;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Тестування Лямбда-виразів (Завдання 3) ===");

        LambdaTester.RunTests();

        DateTime progDay2026 = new DateTime(2026, 9, 13);

        Predicate<DateTime> checkProgDay = d => d.DayOfYear == 256;

        Console.WriteLine($"\nПеревірка 13.09.2026: {(checkProgDay(progDay2026) ? "Це День Програміста!" : "Звичайна дата")}");

        Console.WriteLine("\nВсі тести завершено. Система стабільна.");
        Console.ReadKey();
    }
}