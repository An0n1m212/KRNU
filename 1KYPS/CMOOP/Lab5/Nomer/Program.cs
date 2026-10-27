using System.Text;


namespace Nomer
{
    internal class Program
    {
        private const int Key = 3;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            while (true)
            {
                Console.WriteLine(" МЕНЮ ");
                Console.WriteLine("1. Зашифрувати слово та зберегти у файл");
                Console.WriteLine("2. Грати (Зчитати та розшифрувати з файлу)");
                Console.WriteLine("3. Грати у двох");
                Console.WriteLine("0. Вихід");

                int.TryParse(Console.ReadLine(), out int choice);
                switch (choice) {
                    case 1: EncodeToFile("words.txt"); break;
                    case 2: StartGameff("words.txt"); break;
                    case 3: StartGameiw(); break;
                    default: Console.WriteLine("Невірний вибір!"); break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static string UnicodeTransform(string text, int offset)
        {
            StringBuilder result = new StringBuilder(text.Length);
            foreach (char c in text.ToLower())
            {
                int code = (int)c;
                result.Append((char)(code + offset));
            }
            return result.ToString();
        }

        static void EncodeToFile(string path)
        {
            Console.Write("Введіть слово для зашифровки: ");
            string word = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(word)) return;

            string encrypted = UnicodeTransform(word, Key);

            File.AppendAllLines(path, new[] { encrypted }, Encoding.UTF8);
            Console.WriteLine($"Слово '{word}' зашифровано як '{encrypted}' і збережено.");
        }

        static void EncodeRand(string path)
        {
            string[] ar = { "річка", "вода" };
            string word = ar[new Random().Next(0, ar.Length)];
            if (string.IsNullOrWhiteSpace(word)) return;

            string encrypted = UnicodeTransform(word, Key);

            File.AppendAllLines(path, new[] { encrypted }, Encoding.UTF8);
            Console.WriteLine($"Слово '{word}' зашифровано як '{encrypted}' і збережено.");
        }


        static void StartGameff(string path)
        {
            if (!File.Exists(path))
            {
                EncodeRand("words.txt");
            }

            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0) return;

            string encryptedWord = lines[new Random().Next(lines.Length)];

            string secretWord = UnicodeTransform(encryptedWord, -Key);

            PlayHangman(secretWord);
        }

        static void StartGameiw()
        {
            Console.WriteLine("\nЗагадайте слово супротивнику");
            string secretWord = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(secretWord))
            {
                Console.WriteLine("Ви не ввели слово!");
                return;
            }
            Console.Clear();
            PlayHangman(secretWord);
        }

        static void PlayHangman(string word)
        {
            int lives = 6;
            char[] display = new char[word.Length];
            for (int i = 0; i < word.Length; i++)
                display[i] = (i == 0 || i == word.Length - 1) ? word[i] : '_';

            while (lives > 0 && new string(display) != word)
            {
                Console.WriteLine($"\nСлово: {new string(display)} | Життів: {lives}");
                Console.Write("Ваша буква (укр): ");

                string input = Console.ReadLine()?.ToLower();
                if (string.IsNullOrEmpty(input)) continue;
                char guess = input[0];

                bool hit = false;
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == guess && display[i] == '_')
                    {
                        display[i] = word[i];
                        hit = true;
                    }
                }

                if (!hit) lives--;
            }

            if (new string(display) == word)
                Console.WriteLine($"\nПЕРЕМОГА! Слово: {word}");
            else
                Console.WriteLine($"\nПРОГРАШ. Слово було: {word}");
        }
    }
}