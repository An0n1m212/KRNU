using System.Text;

namespace Nomer
{
    internal class Program
    {

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            int[][] arr;
            Console.WriteLine("Введіть вибір заповнення масиву");
            Console.WriteLine("1 - Через генерацію");
            Console.WriteLine("2 - Через консоль");
            Console.WriteLine("3 - Через текст документ");

            int.TryParse(Console.ReadLine(), out int choise);
            switch (choise)
            {
                case 1: arr = CreateRandArray(); break;
                case 2: arr = CreateInputArray(); break;
                case 3: arr = CreateInputTXTArray(); break;
                default: arr = CreateRandArray(); break;
            }
            Task1(arr);

            Task2();

            Task3(arr);
        }

        static int[][] CreateRandArray()
        {
            Random rnd = new Random();
            Console.Write("Введіть розмірність n: ");
            int.TryParse(Console.ReadLine(), out int n);
            int[][] arr = new int[n][];

            for (int i = 0; i < n; i++)
            {
                arr[i] = new int[n];
                for (int j = 0; j < n; j++)
                    arr[i][j] = rnd.Next(1, 20);
            }
            return arr;
        }
        static int[][] CreateInputArray()
        {
            Console.Write("Введіть розмірність n: ");
            int.TryParse(Console.ReadLine(), out int n);
            int[][] arr = new int[n][];
            for (int i = 0; i < n; i++)
            {
                arr[i] = new int[n];
                Console.WriteLine($"Введіть {n} елементів для рядка {i}:");
                for (int j = 0; j < n; j++)
                    int.TryParse(Console.ReadLine(), out arr[i][j]);
            }
            return arr;
        }
        static int[][] CreateInputTXTArray()
        {
            using (StreamReader streamReader = new StreamReader("input.txt"))
            {
                Console.Write("Введіть розмірність n: ");
                int.TryParse(Console.ReadLine(), out int n);
                int[][] arr = new int[n][];

                for (int i = 0; i < n; i++)
                {
                    arr[i] = new int[n];
                    for (int j = 0; j < n; j++)
                        int.TryParse(streamReader.ReadLine(), out arr[i][j]);

                }
                return arr;
            }
        }
        static void Task1(int[][] m)
        {
            Console.WriteLine("\nЗавдання 1");
            long sum = 0;
            long product = 1;
            PrintArr(m);
            for (int i = 0; i < m.Length; i++)
            {
                int maxVal = m[i][0];
                int maxIdx = 0;

                for (int j = 0; j < m[i].Length; j++)
                {
                    sum += m[i][j];
                    product *= m[i][j];

                    if (m[i][j] > maxVal)
                    {
                        maxVal = m[i][j];
                        maxIdx = j;
                    }
                }

                int start = maxIdx + 1;
                Array.Sort(m[i], start, m[i].Length-start);
            }

            Console.WriteLine($"Різниця (Сума - Добуток): {sum - product}");
            Console.WriteLine("Матриця після сортування елементів після максимуму:");
            PrintArr(m);
            Console.WriteLine("Натисніть будь яку клавішу для продоження");
            Console.ReadLine();
        }

        static void Task2()
        {
            Random rnd = new Random();
            Console.WriteLine("\nЗавдання 2: Перестановка максимуму");
            Console.Write("Введіть розмірність n: ");
            int.TryParse(Console.ReadLine(), out int n);
            int[][] array = new int[n][];
            for (int i = 0; i < n; i++)
            {
                Console.Write("Введіть розмірність m: ");
                int.TryParse(Console.ReadLine(), out int m);
                array[i] = new int[m];
                for (int j = 0; j < n; j++)
                    array[i][j] = rnd.Next(-10, 20);

            }
            PrintArr(array);
            int maxVal = int.MinValue;
            int maxRow = 0, maxCol = 0;

            for (int i = 0; i < array.Length; i++)
                for (int j = 0; j < array[i].Length; j++)
                    if (array[i][j] > maxVal) { maxVal = array[i][j]; maxRow = i; maxCol = j; }

            Console.WriteLine($"Максимум {maxVal} знайдено в [{maxRow}][{maxCol}]");
            if (maxRow != 0)
            {
                int[] tempRow = array[0];
                array[0] = array[maxRow];
                array[maxRow] = tempRow;
            }
            if (maxCol != 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Length > maxCol)
                    {
                        int temp = array[i][0];
                        array[i][0] = array[i][maxCol];
                        array[i][maxCol] = temp;
                    }
                }
            }
            PrintArr(array);
            Console.WriteLine("Натисніть будь яку клавішу для продоження");
            Console.ReadLine();
        }

        static void Task3(int[][] arr)
        {
            Console.WriteLine("\n--- Завдання 3: Магічний квадрат ---");
            if (IsMagicSquare(arr))
                Console.WriteLine("Ваш масив є магічним квадратом");
            else
                Console.WriteLine("Ваш масив НЕ є магічним квадратом (або він не квадратний)");
            Console.WriteLine("Натисніть будь яку клавішу для продоження");
            Console.ReadLine();
        }

        static bool IsMagicSquare(int[][] arr)
        {
            int n = arr.Length;
            foreach (var row in arr) if (row.Length != n) return false;

            int targetSum = arr[0].Sum();

            for (int i = 1; i < n; i++)
                if (arr[i].Sum() != targetSum) return false;

            for (int j = 0; j < n; j++)
            {
                int colSum = 0;
                for (int i = 0; i < n; i++) colSum += arr[i][j];
                if (colSum != targetSum) return false;
            }

            int diag1 = 0, diag2 = 0;
            for (int i = 0; i < n; i++)
            {
                diag1 += arr[i][i];
                diag2 += arr[i][n - 1 - i];
            }

            return diag1 == targetSum && diag2 == targetSum;
        }

        static void PrintArr(int[][] m)
        {
            foreach (var row in m)
            {
                foreach (var item in row) Console.Write($"{item,4}");
                Console.WriteLine();
            }
        }
    }
}
