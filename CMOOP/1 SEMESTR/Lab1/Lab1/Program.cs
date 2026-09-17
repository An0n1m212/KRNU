using System.Text;

namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            double result = 0;
            Console.WriteLine("Введіть значення u");
            double.TryParse(Console.ReadLine(), out double u);
            Console.WriteLine("Введіть значення t");
            if (!double.TryParse(Console.ReadLine(), out double t)) {
                Console.WriteLine("Помилка. Число введено в невірному формті");
                Console.ReadLine();
                return;
            };
            result = Math.Pow((u / t), 4) / Math.Abs(u - t) + Math.Pow(Math.Cos(u / t), 2) * Math.Pow(u, 2) * Math.Exp(u / t);
            Console.WriteLine($"Result = {result:F3}");
        }
    }
}
