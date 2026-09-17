using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM.Interface
{
    public interface IAtm
    {
        int Id { get; }
        decimal CalculateTotalBalance();
        AtmCassette[] GetCassettes();
        void AddBanknotes(int denomination, int count);
        bool WithdrawBanknotes(int amount, AtmCassette[] dispensedCassettes);
    }
}
