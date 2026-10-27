using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer3
{
    public class LambdaTester
    {
        public static void RunTests()
        {
            int[] numbers = { 1, 7, 14, -5, 22, 0, 49, 13 };

            Func<int[], int> countSeven = arr => {
                int count = 0;
                foreach (var n in arr) if (n % 7 == 0 && n != 0) count++;
                return count;
            };

            Func<int[], int> countPositive = arr => {
                int count = 0;
                foreach (var n in arr) if (n > 0) count++;
                return count;
            };

            Predicate<DateTime> isProgrammerDay = date => date.DayOfYear == 256;

            Func<string, string[], bool> containsWords = (text, words) => {
                foreach (var word in words)
                    if (text.Contains(word, StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            };

            Console.WriteLine($"Кратних 7: {countSeven(numbers)}");
            Console.WriteLine($"Позитивних: {countPositive(numbers)}");
            Console.WriteLine($"Сьогодні день програміста? {isProgrammerDay(DateTime.Now)}");

            string message = "Чиста архітектура — це запорука успіху";
            string[] search = { "архітектура", "SOLID" };
            Console.WriteLine($"Текст містить слова: {containsWords(message, search)}");
        }
    }

}
