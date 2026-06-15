
using System.Text;

namespace lab1_6
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Введіть число від 1 до 100");
            int.TryParse(Console.ReadLine(), out int a);
            if (!(a < 1 && a > 99))
            {
                Console.WriteLine("Помилка, число не підлягає діапазону");
                Console.ReadLine();
                return;
            }
            string s = "";
            if (a % 3 == 0)
            {
                s += "Fizz";
            }
            if (a % 5 == 0)
            {
                s += "Buzz";
            }
                if (s == "")
                {
                    s = $"{a}";
                }
                Console.WriteLine(s);

            

        }
    }
}
    
