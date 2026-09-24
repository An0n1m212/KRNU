using Nomer2;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Тестування Валізи (Завдання 2) ===");

        Suitcase myBag = new Suitcase
        {
            Color = "Space Gray",
            Producer = "Samsonite",
            MaxVolume = 50.0
        };

        myBag.ItemAdded += (sender, e) =>
        {
            Console.WriteLine($"[ПОДІЯ]: Предмет '{e.AddedItem.Name}' (об'єм {e.AddedItem.Volume}) успішно додано!");
        };
        myBag.ItemRemoved += (sender, e) =>
        {
            Console.WriteLine($"[ПОДІЯ]: Предмет '{e.RemovedItem.Name}' (об'єм {e.RemovedItem.Volume}) успішно вийнято!");
        };

        var notebook = new Item { Name = "Ноутбук", Volume = 15.5 };

        try
        {
            
            myBag.AddItem(notebook);
            myBag.AddItem(new Item { Name = "Одяг", Volume = 30.0 });
            myBag.RemoveItem(notebook);
            myBag.AddItem(new Item { Name = "Гантелі", Volume = 10.0 });

        }
        catch (InvalidOperationException ex)
        {
            
            Console.WriteLine($"[КРИТИЧНА ПОМИЛКА]: {ex.Message}");

        }

        Console.WriteLine("\nНатисніть клавішу для наступного тесту...");
        Console.ReadKey();
    }
}