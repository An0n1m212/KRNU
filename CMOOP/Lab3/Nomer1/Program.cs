namespace Nomer1
{
    using System;
    using System.Text;

    internal class Program
    {
        const int N = 20;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
                int[] arr = new int[N];
                int choice;
                int i;
            do
            {
                Console.WriteLine("Обрати тип заповнення масиву:");
                Console.WriteLine("1 - Ввести з клавіатури");
                Console.WriteLine("2 - Сгенерувати");
                Console.WriteLine("3 - Прочитати з файлу(input.txt)");
                Console.WriteLine("0 - Вихід з программи");

                if (!int.TryParse(Console.ReadLine(), out choice)) choice = 0;

                switch (choice)
                {
                    case 1:
                        Console.WriteLine($"Input {N} elements of array:");
                        for (i = 0; i < N; i++)
                        {
                            if (!int.TryParse(Console.ReadLine(), out arr[i])) { Console.WriteLine("Помилка введене число не є цілим"); }
                        }
                        return;

                    case 2:
                        Random rnd = new Random();
                        for (i = 0; i < N; i++)
                        {
                            arr[i] = rnd.Next(-50, 51);
                        }
                        return;

                    case 3:
                        {
                            string strValue;
                            using (StreamReader sRead = new StreamReader("dat.txt"))
                            {
                                using (StreamWriter sWrite = new StreamWriter("res.txt"))
                                {
                                    for (i = 0; i < N; i++)
                                    {
                                        string line = sRead.ReadLine();
                                        if (line != null)
                                        { arr[i] = Convert.ToInt32(line); }
                                    }
                                }
                            }
                            return;
                        }
                    case 0:
                        {
                            Console.WriteLine($"Вихід з программи, натисніть будь яку кнопку для продовження");
                            Console.ReadLine();
                            break;
                        }
                    default:
                        Console.WriteLine("Error!");
                        return;
                }
           

                Console.Write("\nInput array: ");
                foreach (var item in arr) Console.Write(item + " ");

                int first = -1;
                for (i = 0; i < N; i++)
                {
                    if (arr[i] % 2 != 0)
                    {
                        first = arr[i];
                        break;
                    }
                }

                if (first == -1)
                {
                    Console.WriteLine("\nНе вдалося знайти %2 != 0 в масиві!");
                    return;
                }

                int last = -1;
                for (i = N - 1; i >= 0; i--) 
                {
                    if (arr[i] % 2 != 0)
                    {
                        last = arr[i];
                        break;
                    }
                }

                int dif = first - last;
                Console.WriteLine($"\nРізниця між першим та останнім парним єлементомnumbers: {dif}");

                for (i = 2; i < N; i += 3)
                {
                    arr[i] += dif;
                }

                Console.Write("Модифікований масив: ");
                foreach (var item in arr) Console.Write(item + " ");
                Console.WriteLine();
            } while (choice == 0);
        }
    }
}
   
