using System;

namespace lab1_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? s;
            StreamWriter f = new StreamWriter("out.txt");
            StreamReader f1 = new StreamReader("in.txt");
            f.WriteLine(new string('+', 23));
            Console.WriteLine(new string('+', 23));
            f.WriteLine("+   Таблиця значень   +");
            Console.WriteLine("+   Таблиця значень   +");
            f.WriteLine(new string('+',23));
            Console.WriteLine(new string('+', 23));

            while ((s = f1.ReadLine()) != null)
            {
                if (double.TryParse(s, out double x))
                {
                    double y = Math.Sqrt(Math.E) * Math.Abs(Math.Abs(Math.Pow(x, 2) - 1) - 2);
                    f.WriteLine($"+ X= {x,5:F2} + Y= {y,5:F2} +");
                    Console.WriteLine($"+ X= {x,5:F2} + Y= {y,5:F2} +");
                }
            }
            f.WriteLine(new string('+', 23));
            f.WriteLine($" Таблицю сформував {s} \n");
            Console.WriteLine(new string('+', 23));
            Console.WriteLine($" Таблицю сформував {s} \n");
            f.Close();
            f1.Close();
        }
    }
}


          