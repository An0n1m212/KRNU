using System.Text;

namespace Nomer2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            int n;
            int INF = 1000;
            Console.WriteLine("Впишіть розмірність матриці:");
            do{
                if (!int.TryParse(Console.ReadLine(), out n) || n <= 0) { Console.WriteLine("Помилка вводу. Число не є додатнім цілим типом"); }
            } while (n<=0);
                int[,] arr = new int[n, n];
            Random rnd = new Random();

            Console.WriteLine("Початкова матриця суміжності графа:");

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) arr[i, j] = INF;
                    else arr[i, j] = rnd.Next(-10, 51);

                    if (arr[i, j] == INF) Console.Write("{0, 5}", " M ");
                    else Console.Write("{0, 5}", arr[i, j]);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < n; i++)
            {
                int minRow = INF;
                for (int j = 0; j < n; j++)
                    if (arr[i, j] < minRow) minRow = arr[i, j];

                if (minRow != INF && minRow > 0)
                    for (int j = 0; j < n; j++)
                        if (arr[i, j] != INF) arr[i, j] -= minRow;
            }

            for (int j = 0; j < n; j++)
            {
                int minCol = INF;
                for (int i = 0; i < n; i++)
                    if (arr[i, j] < minCol) minCol = arr[i, j];

                if (minCol != INF && minCol > 0)
                    for (int i = 0; i < n; i++)
                        if (arr[i, j] != INF) arr[i, j] -= minCol;
            }

            int maxWeight = -1;
            int[,] weights = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] == 0)
                    {
                        int minR = INF, minC = INF;
                        for (int k = 0; k < n; k++)
                            if (k != j && arr[i, k] < minR) minR = arr[i, k];

                        for (int k = 0; k < n; k++)
                            if (k != i && arr[k, j] < minC) minC = arr[k, j];

                        weights[i, j] = minR + minC;
                        if (weights[i, j] > maxWeight) maxWeight = weights[i, j];
                    }
                    else weights[i, j] = -1;
                }
            }

            Console.WriteLine("\n Приведена матриця:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] >= INF) Console.Write("{0, 5}", "M");
                    else Console.Write("{0, 5}", arr[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\n Найбільша вага нуля: {maxWeight}");
            Console.WriteLine(" Індекси нулів з найбільшою вагою:");

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (weights[i, j] == maxWeight && maxWeight != -1)
                    {
                        Console.WriteLine($" Рядок: {i}, Стовпець: {j} (вага {weights[i, j]})");
                    }
                }
            }

            Console.WriteLine("\n Робота програми завершена.");
            Console.ReadKey();
        }
    }
}
