using System.Diagnostics;
using System.Text;

namespace Nomer4
{
    class Program
    {
       static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Random random = new Random();
            int score = 0;
            int ques = 10;

            Console.WriteLine("Тестувальна гра Brain Training!");
            Console.WriteLine($"Ваше завдання: розв'язати {ques} прикладів якомога швидше.");
            Console.WriteLine("Натисніть Enter, щоб почати...");
            Console.ReadLine();
            for (int i = 1; i <= ques; i++)
            {
                int num1 = random.Next(1, 21);
                int num2 = random.Next(1, 21);
                int opType = random.Next(0, 3); 
                int cA = 0;
                string opSym = "";

                switch (opType)
                {
                    case 0:
                        cA = num1 + num2;
                        opSym = "+";
                        break;
                    case 1:
                        cA = num1 - num2;
                        opSym = "-";
                        break;
                    case 2:
                        cA = num1 * num2;
                        opSym = "*";
                        break;
                }

                Console.Write($"Приклад {i}: {num1} {opSym} {num2} = ");

                if (int.TryParse(Console.ReadLine(), out int uA) && uA == cA)
                {
                    Console.WriteLine("Правильно!");
                    score++;
                }
                else
                {
                    Console.WriteLine($"Помилка! Правильна відповідь: {cA}");
                }
                Console.ResetColor();
            }

            Console.WriteLine("\n\tРезультати тренування");
            Console.WriteLine($"Правильних відповідей: {score} з {ques}");
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
        }
    }
