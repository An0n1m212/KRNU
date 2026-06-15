using System.Text;

namespace Number4
{
    class Program
    {
        static int score = 0;
        static int bestScore = 0;
        static int n = 4;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            bool exitProgram = false;
            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("2048");
                Console.Write("Оберіть розмір поля (наприклад, 4): ");
                if (!int.TryParse(Console.ReadLine(), out n) || n < 2) n = 4;

                RunGame();

                Console.Clear();
                Console.WriteLine($"Гра завершена! Ваш результат: {score}");
                Console.WriteLine($"Найкращий результат сесії: {bestScore}");
                Console.WriteLine("\nБажаєте зіграти ще раз? (Y - так, будь-яка інша клавіша - вихід)");

                ConsoleKeyInfo retry = Console.ReadKey(true);
                if (retry.Key != ConsoleKey.Y) exitProgram = true;
            }
        }

        static void RunGame()
        {
            int[,] arr = new int[n, n];
            score = 0;
            AddRandomTile(arr);
            AddRandomTile(arr);

            while (true)
            {
                Console.Clear();
                DrawBoard(arr);

                if (IsVictory(arr)) Console.WriteLine("ВИ ДОСЯГЛИ 2048");
                if (IsGameOver(arr))
                {
                    Console.WriteLine("\nГРА ЗАКІНЧЕНА: Немає доступних ходів!");
                    Console.ReadKey();
                    break;
                }

                Console.WriteLine("\nУправління: Стрілки (рух), Esc (вихід), N (нова гра)");
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape) break;
                if (key.Key == ConsoleKey.N) break;

                int moveScore = 0;
                bool moved = false;
                int[,] backup = (int[,])arr.Clone();

                switch (key.Key) {
                    case ConsoleKey.LeftArrow: moveScore = MoveLeft(arr); break;
                    case ConsoleKey.RightArrow: moveScore = MoveRight(arr); break; 
                    case ConsoleKey.UpArrow: moveScore = MoveUp(arr); break;
                    case ConsoleKey.DownArrow: moveScore = MoveDown(arr); break;
                    default: continue;
                } 

                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (backup[i, j] != arr[i, j]) moved = true;

                if (moved)
                {
                    score += moveScore;
                    if (score > bestScore) bestScore = score;
                    AddRandomTile(arr);
                }
            }
        }

        static void DrawBoard(int[,] arr)
        {
            Console.WriteLine($"Score: {score}  |  Best Score: {bestScore}");
            string line = new string('-', n * 7 + 1);
            Console.WriteLine(line);

            for (int i = 0; i < n; i++)
            {
                Console.Write("|");
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] == 0) Console.Write("{0, 6}|", " ");
                    else
                    {
                        Console.Write("{0, 6}", arr[i, j]);
                        Console.ResetColor();
                        Console.Write("|");
                    }
                }
                Console.WriteLine("\n" + line);
            }
        }

        static void AddRandomTile(int[,] arr)
        {
            int emptyCount = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (arr[i, j] == 0) emptyCount++;

            if (emptyCount > 0)
            {
                int target = rnd.Next(emptyCount);
                int current = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (arr[i, j] == 0)
                        {
                            if (current == target)
                            {
                                arr[i, j] = (rnd.Next(100) < 90) ? 2 : 4;
                                return;
                            }
                            current++;
                        }
            }
        }

        static int ProcessLine(int[] line)
        {
            int lineScore = 0;
            int[] temp = new int[n];
            int pos = 0;
            for (int i = 0; i < n; i++) if (line[i] != 0) temp[pos++] = line[i];

            for (int i = 0; i < n - 1; i++)
            {
                if (temp[i] != 0 && temp[i] == temp[i + 1])
                {
                    temp[i] *= 2;
                    lineScore += temp[i];
                    for (int j = i + 1; j < n - 1; j++) temp[j] = temp[j + 1];
                    temp[n - 1] = 0;
                }
            }
            for (int i = 0; i < n; i++) line[i] = temp[i];
            return lineScore;
        }

        static int MoveLeft(int[,] arr)
        {
            int s = 0;
            for (int i = 0; i < n; i++)
            {
                int[] row = new int[n];
                for (int j = 0; j < n; j++) row[j] = arr[i, j];
                s += ProcessLine(row);
                for (int j = 0; j < n; j++) arr[i, j] = row[j];
            }
            return s;
        }

        static int MoveRight(int[,] arr)
        {
            int s = 0;
            for (int i = 0; i < n; i++)
            {
                int[] row = new int[n];
                for (int j = 0; j < n; j++) row[j] = arr[i, n - 1 - j];
                s += ProcessLine(row);
                for (int j = 0; j < n; j++) arr[i, n - 1 - j] = row[j];
            }
            return s;
        }

        static int MoveUp(int[,] arr)
        {
            int s = 0;
            for (int j = 0; j < n; j++)
            {
                int[] col = new int[n];
                for (int i = 0; i < n; i++) col[i] = arr[i, j];
                s += ProcessLine(col);
                for (int i = 0; i < n; i++) arr[i, j] = col[i];
            }
            return s;
        }

        static int MoveDown(int[,] arr)
        {
            int s = 0;
            for (int j = 0; j < n; j++)
            {
                int[] col = new int[n];
                for (int i = 0; i < n; i++) col[i] = arr[n - 1 - i, j];
                s += ProcessLine(col);
                for (int i = 0; i < n; i++) arr[n - 1 - i, j] = col[i];
            }
            return s;
        }

        static bool IsVictory(int[,] arr)
        {
            foreach (int x in arr) if (x == 2048) return true;
            return false;
        }

        static bool IsGameOver(int[,] arr)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] == 0) return false;
                    if (i < n - 1 && arr[i, j] == arr[i + 1, j]) return false;
                    if (j < n - 1 && arr[i, j] == arr[i, j + 1]) return false;
                }
            return true;
        }
    }
}