using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class Input
    {
        public static string ReadStringSafe(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                Console.WriteLine("Помилка: ім'я не може бути порожнім! Спробуйте ще раз.");
            }
        }
        public static int ReadIntSafe(string message, int min, int max)
        {
            int result;
            while (true)
            {
                Console.Write($"{message} ({min}-{max}): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.WriteLine($"! Помилка: введіть ціле число від {min} до {max}.");
            }
        }
        public static bool ReadBoolSafe(string message)
        {
            while (true)
            {
                Console.Write($"{message} (1 - Так, 0 - Ні): ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": return true;
                    case "0": return false;
                }
                Console.WriteLine("! Помилка: введіть 1 для підтвердження або 0 для відмови.");
            }
        }

        public static double ReadDoubleSafe(string message, int min, int max)
        {
            double result;
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (double.TryParse(input, out result) && result >=min && result <=max)
                {
                    return result;
                }
                Console.WriteLine("! Помилка: введіть коректне число (наприклад, 5 або 10,5).");
            }
        }
    }
}
