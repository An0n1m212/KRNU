using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer1
{
    public class FunctionalModule
    {
        public static Action DisplayTime = () => Console.WriteLine($"Поточний час: {DateTime.Now:T}");
        public static Action DisplayDate = () => Console.WriteLine($"Поточна дата: {DateTime.Now:d}");
        public static Action DisplayDayOfWeek = () => Console.WriteLine($"День тижня: {DateTime.Now.DayOfWeek}");

        public static Predicate<int> IsPrime = (n) => {
            if (n <= 1) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
                if (n % i == 0) return false;
            return true;
        };

        public static Predicate<int> IsFibonacci = (n) => {
            Func<int, bool> isSquare = (x) => {
                int s = (int)Math.Sqrt(x);
                return s * s == x;
            };
            return isSquare(5 * n * n + 4) || isSquare(5 * n * n - 4);
        };

        public static Func<double, double, double> GetTriangleArea = (baseLen, height) => 0.5 * baseLen * height;
        public static Func<double, double, double> GetRectangleArea = (width, height) => width * height;
    }
}
