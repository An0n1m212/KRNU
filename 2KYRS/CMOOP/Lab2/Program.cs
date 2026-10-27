using Lab2.Task;    
int choise = int.TryParse(Console.ReadLine(), out int result) ? result : 0;

switch (choise)
{
    case 1:
        Task1.Run();
        break;
    case 2:
        Task2.Run();
        break;
    case 3:
        Task3.Run();
        break;
}