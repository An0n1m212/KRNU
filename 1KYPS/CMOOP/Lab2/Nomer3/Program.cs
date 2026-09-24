using System.Text;

namespace Nomer3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            const int q = 12;
            int a;
            int min, max;
            Random random = new Random();
            do
            {
                Console.WriteLine("\tГра перевір свої знання");
                Console.WriteLine("\tОбери рівень знань:");
                Console.WriteLine($"Легкий ({1 * q} питання): - 1 \nНормальний ({2 * q} питання): - 2 \nХард ({3 * q} питання): - 3 \nВихід: - 0 ");
                a = int.Parse(Console.ReadLine());
                switch (a)
                {
                    case 1:
                           {
                                min = 0;
                                max = 9;
                                int countres = 0;
                                for (int i = 0; i < q * a; i++)
                                {
                                int a1 = random.Next(min, max);
                                int a2 = random.Next(min, max);
                                int res = a1 * a2;
                                    Console.WriteLine($"{i + 1}) {a1} * {a2} = ");
                                    int.TryParse(Console.ReadLine(), out int val);
                                    if (res == val)
                                    {
                                        Console.WriteLine("Правильна відповідь!");
                                        countres++;
                                    }
                                }
                            Console.WriteLine($"Кількість правильних відповідей {countres}!");
                            break;
                        }

                    case 2:
                            {
                                min = 0;
                                max = 99;
                                int countres = 0;
                                for (int i = 0; i < a * q; i++)
                                {
                                int a1 = random.Next(min, max); 
                                int a2 = random.Next(min, max); 
                                int res = a1 * a2;
                                Console.WriteLine($"{i+1}) {a1} * {a2} = ");
                                int.TryParse(Console.ReadLine(), out int val);
                                if (res == val)
                                    {
                                        Console.WriteLine("Правильна відповідь!");
                                        countres++;
                                    }
                                }
                            Console.WriteLine($"Кількість правильних відповідей {countres}!");
                            break;
                            }

                    case 3:
                                {
                                    min = 0;
                                    max = 999;
                                    int countres = 0;
                                    for (int i = 0; i < a * q; ++i)
                                    {
                                int a1 = random.Next(min, max);
                                int a2 = random.Next(min, max);
                                int res = a1 * a2;
                                     Console.WriteLine($"{i + 1}) {a1} * {a2} = ");
                                        int.TryParse(Console.ReadLine(), out int val);
                                        if (res == val)
                                        {
                                            Console.WriteLine("Правильна відповідь!");
                                            countres++;
                                        }
                                    }
                                    Console.WriteLine($"Кількість правильних відповідей {countres}!");
                                    break;
                    }
                    case 0:
                        {
                            Console.WriteLine("Вихід");
                            break;
                        }
                }




            } while (a != 0);
        }
    }
}
