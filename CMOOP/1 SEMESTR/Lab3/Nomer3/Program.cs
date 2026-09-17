using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Random rnd = new Random();

        int n = 4;
        int[,] arr = new int[n, n];

        arr[0, 0] = 2; arr[0, 1] = 2;
        arr[1, 0] = 4; arr[1, 2] = 4;
        arr[2, 2] = 2; arr[2, 3] = 2;
        arr[3, 0] = 8; arr[3, 1] = 8;

        Console.WriteLine("Початковий масив:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) Console.Write("{0, 5}", arr[i, j]);
            Console.WriteLine();
        }

        int emptyCount = 0;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (arr[i, j] == 0) emptyCount++;

        if (emptyCount > 0)
        {
            int target = rnd.Next(emptyCount);
            int current = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (arr[i, j] == 0)
                    {
                        if (current == target)
                        {
                            arr[i, j] = (rnd.Next(100) < 90) ? 2 : 4;
                            i = n; break;
                        }
                        current++;
                    }
                }
            }
        }


        int score = 0;

        // б) ВЛІВО
        //for (int i = 0; i < n; i++)
        //{
        //    int[] temp = new int[n];
        //    int pos = 0;
        //    for (int j = 0; j < n; j++) if (arr[i, j] != 0) temp[pos++] = arr[i, j];
        //    for (int j = 0; j < n - 1; j++)
        //    {
        //        if (temp[j] != 0 && temp[j] == temp[j + 1])
        //        {
        //            temp[j] *= 2; score += temp[j];
        //            for (int k = j + 1; k < n - 1; k++) temp[k] = temp[k + 1];
        //            temp[n - 1] = 0;
        //        }
        //    }
        //    for (int j = 0; j < n; j++) arr[i, j] = temp[j];
        //}

        // в) ВПРАВО

        for (int i = 0; i < n; i++)
        {
            int[] temp = new int[n];
            int pos = n - 1;
            for (int j = n - 1; j >= 0; j--) if (arr[i, j] != 0) temp[pos--] = arr[i, j];
            for (int j = n - 1; j > 0; j--)
            {
                if (temp[j] != 0 && temp[j] == temp[j - 1])
                {
                    temp[j] *= 2; score += temp[j];
                    for (int k = j - 1; k > 0; k--) temp[k] = temp[k - 1];
                    temp[0] = 0;
                }
            }
            for (int j = 0; j < n; j++) arr[i, j] = temp[j];
        }


        // с) ВГОРУ

        //for (int j = 0; j < n; j++)
        //{
        //    int[] temp = new int[n];
        //    int pos = 0;
        //    for (int i = 0; i < n; i++) if (arr[i, j] != 0) temp[pos++] = arr[i, j];
        //    for (int i = 0; i < n - 1; i++)
        //    {
        //        if (temp[i] != 0 && temp[i] == temp[i + 1])
        //        {
        //            temp[i] *= 2; score += temp[i];
        //            for (int k = i + 1; k < n - 1; k++) temp[k] = temp[k + 1];
        //            temp[n - 1] = 0;
        //        }
        //    }
        //    for (int i = 0; i < n; i++) arr[i, j] = temp[i];
        //}


        // д) ВНИЗ

        //for (int j = 0; j < n; j++)
        //{
        //    int[] temp = new int[n];
        //    int pos = n - 1;
        //    for (int i = n - 1; i >= 0; i--) if (arr[i, j] != 0) temp[pos--] = arr[i, j];
        //    for (int i = n - 1; i > 0; i--)
        //    {
        //        if (temp[i] != 0 && temp[i] == temp[i - 1])
        //        {
        //            temp[i] *= 2; score += temp[i];
        //            for (int k = i - 1; k > 0; k--) temp[k] = temp[k - 1];
        //            temp[0] = 0;
        //        }
        //    }
        //    for (int i = 0; i < n; i++) arr[i, j] = temp[i];
        //}


        Console.WriteLine("\nМасив після зсуву (приклад Вліво):");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++) Console.Write("{0, 5}", arr[i, j]);
            Console.WriteLine();
        }
        Console.WriteLine($"\nСума балів від об'єднання: {score}");

        Console.ReadKey();
    }
}