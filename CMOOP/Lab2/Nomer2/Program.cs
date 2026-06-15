using System.Text;

namespace Nomer2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            double x, y, s, u, t;
            int n;
            string rep;
            do
            {
                Console.WriteLine("Введіть значення x"); 
                double.TryParse(Console.ReadLine(), out x);
                Console.WriteLine('\t' + "Поточні результати" + '\n');
                n = 0;
                t = (x - 1) / (x + 1);
                s = 0;
                u = t;
                while (Math.Abs(u) >= 0.000001)
                {
                    s += u;
                    u *= ((2 * n + 1)/(2 * n + 3))* t * t;
                    n++;

                    Console.WriteLine($"Ітерація {n}: член ряду = {u:F8} Поточна сума = {s:F8}");
                    }
                y = 0.5 * Math.Log(x); ; 
                Console.WriteLine( '\t' + $"РЕЗУЛЬТАТИ:\nВведене x = {x}"); 
                Console.WriteLine($"Обчислене значення суми ряду S = {s:F8}"); 
                Console.WriteLine($"Кількість членів ряду - {n}");
                Console.WriteLine($"Значення функції 0.5*ln(x) = {y}");
                Console.WriteLine("Для повторного вводу натисніть довільну клавішу" + 
                    "Для закінчення програми натисніть Enter.");
                rep = Console.ReadLine();
            } while (rep != "");
        }
    }
}
