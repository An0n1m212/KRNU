using Nomer1;
public class Program
{
    public static void Main(string[] args)
    {

            Console.WriteLine("=== Тестування Делегатів (Завдання 1) ===");

            FunctionalModule.DisplayTime();
            FunctionalModule.DisplayDate();
            FunctionalModule.DisplayDayOfWeek();

            int testNum = 13;
            bool isPrime = FunctionalModule.IsPrime(testNum);
            bool isFib = FunctionalModule.IsFibonacci(testNum);
            Console.WriteLine($"Число {testNum}: Просте = {isPrime}, Фібоначчі = {isFib}");

            double triArea = FunctionalModule.GetTriangleArea(10, 5);
            double rectArea = FunctionalModule.GetRectangleArea(10, 5);
            Console.WriteLine($"Площа трикутника: {triArea}, Прямокутника: {rectArea}");

            Console.WriteLine("\nНатисніть клавішу для наступного тесту...");
            Console.ReadKey();
        }
}