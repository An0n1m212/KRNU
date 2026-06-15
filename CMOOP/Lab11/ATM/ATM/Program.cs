using ATM.ATM;
using ATM.ATM.Interface;
using ATM.Bank;
using ATM.Servise;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;


        AtmCassette[] initialCash = new AtmCassette[]
        {
                new AtmCassette(1000, 10), 
                new AtmCassette(500, 20),  
                new AtmCassette(200, 50),  
                new AtmCassette(100, 100) 
        }; 

        IAtmIncasator incasator = new BankIncasationService();
        IAuditService auditService = new FileAuditService();

        Bank myBank = new Bank(incasator, 3, initialCash);

        Console.WriteLine("=== БАНКІВСЬКА СИСТЕМА ЗАПУЩЕНА ===");
        Console.WriteLine($"Кількість банкоматів у мережі: {myBank.AtmCount}");
        Console.WriteLine($"Загальний стартовий баланс мережі: {myBank.CalculateTotalNetworkBalance()} грн.");

        IAtm activeAtm = myBank.GetAtmByIndex(0);
        AtmController atmTerminal = new AtmController(activeAtm, auditService);

        string user = "Дмитро Марченко";
        Console.WriteLine($"\n--- Початок сесії для користувача: {user} ---");

        atmTerminal.CheckBalance();

        atmTerminal.Withdraw(user, 3700);

        atmTerminal.Deposit(user, 500, 4);

        atmTerminal.Withdraw(user, 150);

        atmTerminal.CheckBalance();

        Console.WriteLine($"\n=================================================");
        Console.WriteLine($"Поточний загальний баланс усієї мережі: {myBank.CalculateTotalNetworkBalance()} грн.");
        Console.WriteLine("Дані аудиту успішно збережені у файлі 'atm_audit.txt'.");
    }
}