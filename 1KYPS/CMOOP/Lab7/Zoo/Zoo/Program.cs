namespace Zoo
{
    class Program
    {
        private static string _dataFilePath = "zoo_data.txt";
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            ZooManager zoo = new ZooManager();
            bool isRunning = true;
            DataLoad.FromFile(_dataFilePath, zoo);

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    СИСТЕМА УПРАВЛІННЯ ЗООПАРКОМ    ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Додати тварину (через базу у WaitList)");
                Console.WriteLine("2. Редагувати тварину");
                Console.WriteLine("3. Видалити тварину");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("4. Розселити тварину з WaitList у вольєр");
                Console.WriteLine("5. Створити додатковий вольєр");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("6. МОНІТОРИНГ: Показати стан усіх вольєрів");
                Console.WriteLine("7. МОНІТОРИНГ: Структуру всіх вольєрів");
                Console.WriteLine("8. МОНІТОРИНГ: Переглянути кількість споживаємого корму");
                Console.WriteLine("0. Вийти з програми(Зберегти данні за сессію)");
                Console.WriteLine("========================================");

                int choice = Input.ReadIntSafe("Ваш вибір: ", 0, 8);

                switch (choice)
                {
                    case 1: zoo.AddAnimal(); break;
                    case 2: zoo.ModifyAnimal(); break;
                    case 3: zoo.RemoveAnimal(); break;
                    case 4: zoo.Relocate(); break;
                    case 5: zoo.CreateEncl(); break;
                    case 6: zoo.ShowAllStatus(); break;
                    case 7: zoo.ShowStructure(); break;
                    case 8: zoo.CalculateFood(); break;
                    case 0:
                        isRunning = false;
                        Console.WriteLine("Програма завершена. Гарного дня!");
                        DataSave.ToFile(_dataFilePath, zoo);
                        break;
                }

                if (choice != 6 && choice != 0)
                {
                    Console.WriteLine("\nОперація виконана. Натисніть клавішу...");
                    Console.ReadKey();
                }
            }
        }
    }
}