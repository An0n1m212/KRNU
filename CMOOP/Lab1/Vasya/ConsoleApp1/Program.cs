using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const double mon = 1.5;
            const int shtaf = 20;
            double dohid = 0;
            double coun = 0;
            int zap = 0;
            int a;
            int zassth;
            Console.OutputEncoding = Encoding.UTF8;
            do
            {
                Console.WriteLine("Зробіть вибір");
                Console.WriteLine("Вийти з циклу - 0");
                Console.WriteLine("Порахувати скільки рядків Вася повинен написати - 1");
                Console.WriteLine("Порахувати скільки разів Вася може запізниться - 2");
                Console.WriteLine("Визначити скільки грошей заплатять Васі - 3");
                a = int.Parse(Console.ReadLine());
                switch (a)
                {
                    case 0: break;
                    case 1:
                        {
                            Console.WriteLine("Впшиіть бажаний дохід");
                            double.TryParse(Console.ReadLine(), out dohid);
                            Console.WriteLine("Впшиіть кількість запізнень");
                            int.TryParse(Console.ReadLine(), out zap);
                            if (zap % 3 == 0)
                            {
                                zassth = 1;
                                zap /= 3;
                            }
                            else
                            {
                                zassth = 0;
                            }
                            coun = (dohid + zap * shtaf * zassth) / mon;
                            Console.WriteLine($"Кількіть рядків коду {coun:F0}");
                            continue;
                        }
                    case 2:
                        {
                            Console.WriteLine("Впшиіть кількіть рядків коду");
                            double.TryParse(Console.ReadLine(), out coun);
                            Console.WriteLine("Впшиіть бажаний дохід");
                            double.TryParse(Console.ReadLine(), out dohid);
                            zap = (int)Math.Abs((dohid - coun * mon) / shtaf * 3);
                            Console.WriteLine($"Кількіть можливих пропусків {zap:F0}");
                            continue;
                        }
                    case 3:
                        {
                            Console.WriteLine("Впшиіть кількіть рядків коду");
                            double.TryParse(Console.ReadLine(), out coun);
                            Console.WriteLine("Впшиіть кількість запізнень");
                            int.TryParse(Console.ReadLine(), out zap);
                            if (zap % 3 == 0)
                            {
                                zassth = 1;
                            }
                            else
                            {
                                zassth = 0;
                            }
                            dohid = coun * mon - (zap * shtaf * zassth) / 3;
                            Console.WriteLine($"Дохід Васі {dohid:F2}");
                            continue;
                        }
                }
            }
            while (a != 0);
        }
    }
}
