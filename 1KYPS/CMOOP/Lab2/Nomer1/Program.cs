namespace Nomer1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double dx, xmax, xmin;
            using (StreamReader streamReader = new StreamReader("input.txt"))
            {
                double.TryParse(streamReader.ReadLine(), out xmin);
                double.TryParse(streamReader.ReadLine(), out xmax);
                double.TryParse(streamReader.ReadLine(), out dx);
            }
            using (StreamWriter p = new StreamWriter("rez.txt"))
            {
                double a;
                double x = xmin;
                p.WriteLine("РЕЗУЛЬТАТИ РОЗРАХУНКУ");
                Console.WriteLine("РЕЗУЛЬТАТИ РОЗРАХУНКУ");
                for (a = 0.5; a <= 2; a += 0.5)
                {
                    p.WriteLine(" a = " + a);
                    for (x = xmin; x < xmax; x += dx)
                    {
                        double y = (double)((Math.Sqrt(x) + a * a * Math.Exp(-x * x)) / (a + x));
                        p.WriteLine(" x =  " + x + '\t' + " y = " + y);
                        Console.WriteLine(" x =  " + x + '\t' + " y = " + y);
                    }

                }
            }
            }
        }
    } 
