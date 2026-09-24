using System.Text;

namespace lab7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("Введіть день: ");
            int.TryParse(Console.ReadLine(), out int d);
            Console.Write("Введіть місяць: ");
            int.TryParse(Console.ReadLine(), out int m);
            Console.Write("Введіть рік: ");
            int.TryParse(Console.ReadLine(), out int y);
            DateTime date = new DateTime(y, m, d);

            string season = "";
            if (m == 12 || m == 1 || m == 2) season = "Winter";
            else if (m >= 3 && m <= 5) season = "Spring";
            else if (m >= 6 && m <= 8) season = "Summer";
            else if (m >= 9 && m <= 11) season = "Autumn";

            Console.WriteLine($"{season} {date.DayOfWeek}");
        }
    }
}
