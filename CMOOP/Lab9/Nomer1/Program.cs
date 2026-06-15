using System;
using System.Linq.Expressions;

namespace Nomer1
{
    public static class Program
    {
        static void Main()
        {
            try {
                BirthDate myBirh = new BirthDate(7, 11, 2007);
                IBirthCacl calc = new BirthDateService();

                DayOfWeek bornOn = calc.DayOfWeek(myBirh);
                int daysLeft = calc.DaysToNextBrith(myBirh);

                Console.WriteLine($"Народився у {bornOn}");
                int yer = int.Parse(Console.ReadLine());
                DayOfWeek inYer = calc.DayOfWeekInYear(myBirh, yer);
                Console.WriteLine($"У {yer} році дунь народження буде у: {inYer}");
                Console.WriteLine($"Днів до настуаного дня народження {daysLeft}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }       
    }
}