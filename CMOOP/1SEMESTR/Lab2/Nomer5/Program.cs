using System;
using System.Text;

class Nomer5
{
    static int PlScore = 0;
    static int CompScore = 0;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("\tGUESS MY NUMBER");

        bool lvl1compl = Lvlstart(1, 3, 1, 10, 0.5);

        if (lvl1compl)
        {
            Console.WriteLine("\nВітаємо! Ви пройшли перший рівень.");
            Console.Write("Бажаєте перейти на другий рівень? (так/ні): ");
            if (Console.ReadLine().ToLower() == "так")
            {
                Lvlstart(2, 2, 10, 100, 0.25);
            }
        }
        else
        {
            Console.WriteLine("\nГра завершена. Ви не пройшли перший рівень.");
        }

        Console.WriteLine("\n=====================================");
        Console.WriteLine($"ФІНАЛЬНИЙ РЕЗУЛЬТАТ:");
        Console.WriteLine($"ВАШІ ОЧКИ: {PlScore}");
        Console.WriteLine($"ОЧКИ КОМП'ЮТЕРА: {CompScore}");
        Console.WriteLine("Дякуємо за гру!");
    }

    static bool Lvlstart(int NumLvl, int totalRounds, int min, int max, double lifeFactor)
    {
        Console.WriteLine($"\nРІВЕНЬ {NumLvl} (Діапазон: {min}-{max}");
        int multiplier = (NumLvl == 1) ? 5 : 10;
        int initialLives = (int)Convert.ToInt16((max - min + 1) * lifeFactor);

        for (int round = 1; round <= totalRounds; round++)
        {
            Console.WriteLine($"\nРаунд {round} з {totalRounds}. Життів на раунд: {initialLives}");

            if (!PlayRound(min, max, initialLives, multiplier))
            {
                Console.WriteLine($"\nВи програли раунд {round}!");
                return false;
            }
        }

        Console.WriteLine($"\n>>> РІВЕНЬ {NumLvl} ЗАВЕРШЕНО УСПІШНО! <<<");
        Console.WriteLine($"Поточний рахунок - Ви: {PlScore}, Комп'ютер: {CompScore}");
        return true;
    }

    static bool PlayRound(int min, int max, int lives, int multiplier)
    {
        Random rand = new Random();
        int targetNumber = rand.Next(min, max + 1);
        int currentLives = lives;

        while (currentLives > 0)
        {
            Console.Write($"Вгадайте число ({min}-{max}) [Життів: {currentLives}]: ");
            if (!int.TryParse(Console.ReadLine(), out int guess)) continue;

            if (guess == targetNumber)
            {
                int roundPoints = currentLives * multiplier;
                PlScore += roundPoints;
                Console.WriteLine($"ВІРНО! Ви отримали {roundPoints} очок.");
                return true;
            }

            currentLives--;
            Console.WriteLine("Неправильно!");

            if (currentLives > 0)
            {
                Console.Write("Використати 1 життя на підказку? (так/ні): ");
                if (Console.ReadLine().ToLower() == "так")
                {
                    currentLives--;
                    string hint = (targetNumber > guess) ? "більше" : "менше";
                    Console.WriteLine($"ПІДКАЗКА: Загадане число {hint}, ніж {guess}.");
                }
            }
        }

        int compPoints = lives * multiplier;
        CompScore += compPoints;
        Console.WriteLine($"ПРОГРАШ! Загадане число було: {targetNumber}");
        Console.WriteLine($"Комп'ютер отримує {compPoints} очок.");
        return false;
    }
}