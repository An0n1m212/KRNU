namespace Lab4.Task2
{
    internal static class ConsoleUi
    {
        private const string Separator = "------------------------------------------";

        public static void ShowHeader(string title)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine($"  {title}");
            Console.WriteLine("==========================================");
        }

        public static void ShowFooter()
        {
            Console.WriteLine(Separator);
        }

        public static string ReadLine(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public static int? ReadIndex(string message, int min, int max)
        {
            Console.Write($"{message}: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= min && index <= max)
            {
                return index;
            }

            return null;
        }

        public static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }
    }
}
