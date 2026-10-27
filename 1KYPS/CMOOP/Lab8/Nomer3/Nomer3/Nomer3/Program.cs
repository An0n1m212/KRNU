using Nomer3.Class.ACipher;
using Nomer3.Class.BCipher;
using Nomer3.Manager;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

bool isRunning = true;

do 
{
    Console.Clear();
    Manager.ShowMenu();
    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Введіть текст (через пробіл): ");
            Manager.SetData(Console.ReadLine());
            Manager.Wait();
            break;
        case "2":
            Manager.LoadFromFile();
            Manager.Wait();
            break;
        case "3":
            Manager.ProcessAndSave<ACipher>();
            Manager.Wait();
            break;
        case "4":
            Manager.ProcessAndSave<BCipher>();
            Manager.Wait();
            break;
        case "0":
            isRunning = false;
            break;
        default:
            Console.WriteLine("Невірний вибір, спробуйте ще раз.");
            Console.Clear();   
            break;
    }
}while (isRunning);
        


