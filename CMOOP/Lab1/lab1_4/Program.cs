using System.Text;

namespace lab1_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int counts = 0;
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Перевірю свої можливості");            
            Console.WriteLine("Професор ліг спати о 8 годині, а встав о 9 годині. Кількість годин сну професору?");
            int.TryParse(Console.ReadLine(), out int a);
            if (a == 1) counts++; 
            Console.WriteLine("На двох руках десять пальців. Скільки пальців на 10?");
            int.TryParse(Console.ReadLine(), out int a);
            if (a == 50) counts++;
            Console.WriteLine("Скільки цифр у дюжині?");
            int.TryParse(Console.ReadLine(), out int a);
            if (a == 2) counts++;
            Console.WriteLine("Скільки потрібно зробити розпилів, щоб розпиляти колоду на 12 частин?");
            int.TryParse(Console.ReadLine(), out int d);
            if (c == 2) counts++;
            Console.WriteLine("Лікар зробив три уколи в інтервалі 30 хвилин. Скільки часу він витратив?");
            int.TryParse(Console.ReadLine(), out int f);
            if (c == 2) counts++;
            Console.WriteLine("Скільки цифр 9 в інтервалі 1100?");
            int.TryParse(Console.ReadLine(), out int e);
            Console.WriteLine("Пастух мав 30 овець. Усі, окрім однієї, розбіглися. Скільки овець лишилося?");
            int.TryParse(Console.ReadLine(), out int i);
                        if (d == 11) counts++;
            if (f == 30) counts++;
            if (e == 1) counts++;
            if (i == 1) counts++;
            switch (counts)
            {
                case 3: Console.WriteLine("Здібності нижче середнього"); break;
                case 4: Console.WriteLine("Здібності середні"); break;
                case 5: Console.WriteLine("Нормальний"); break;
                case 6: Console.WriteLine("Ерудит"); break;
                case 7: Console.WriteLine("Геній"); break;
                default: Console.WriteLine("Вам треба відпочити!"); break;


            }
        }
    }
}
