using ATM.ATM.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM
{
    public class AtmController : IAtmUserOperations
    {
        private readonly IAtm _atm;
        private readonly IAuditService _auditService;

        public AtmController(IAtm atm, IAuditService auditService)
        {
            _atm = atm ?? throw new ArgumentNullException(nameof(atm));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        }

        public void CheckBalance()
        {
            Console.WriteLine($"\n[АТМ №{_atm.Id}] Баланс пристрою: {_atm.CalculateTotalBalance()} грн.");
            Console.WriteLine("Доступні купюри в касетах:");
            foreach (AtmCassette cassette in _atm.GetCassettes())
            {
                Console.WriteLine($"  > {cassette.Denomination} грн: {cassette.Count} шт.");
            }
        }

        public void Deposit(string userName, int denomination, int count)
        {
            try
            {
                _atm.AddBanknotes(denomination, count);
                decimal totalAdded = (decimal)denomination * count;
                Console.WriteLine($"[Успіх] Внесено купюри: {denomination} грн. x {count} шт. (+{totalAdded} грн.)");

                _auditService.LogTransaction(userName, "Поповнення", totalAdded, "Успішно");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка поповнення]: {ex.Message}");
                _auditService.LogTransaction(userName, "Поповнення", (decimal)denomination * count, $"Помилка: {ex.Message}");
            }
        }

        public void Withdraw(string userName, int amount)
        {
            AtmCassette[] dispensed = new AtmCassette[4];

            if (_atm.WithdrawBanknotes(amount, dispensed))
            {
                Console.WriteLine($"\n[Успішне зняття] Видано {amount} грн. Наступними купюрами:");
                foreach (AtmCassette cassette in dispensed)
                {
                    if (cassette != null && cassette.Count > 0)
                    {
                        Console.WriteLine($"  * {cassette.Denomination} грн. x {cassette.Count} шт.");
                    }
                }
                _auditService.LogTransaction(userName, "Зняття готівки", amount, "Успішно");
            }
            else
            {
                Console.WriteLine($"\n[Відмова] Неможливо видати {amount} грн (некратна сума або відсутні купюри).");
                _auditService.LogTransaction(userName, "Зняття готівки", amount, "Відмовлено: недостатньо відповідних купюр");
            }
        }
    }
}
