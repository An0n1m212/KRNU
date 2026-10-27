using ATM.ATM;
using ATM.ATM.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Bank
{
    public class Bank
    {
        private readonly IAtm[] _atms;
        public int AtmCount => _atms.Length;

        public Bank(IAtmIncasator incasator, int atmCount, AtmCassette[] initialCash)
        {
            _atms = incasator.InitializeNetwork(atmCount, initialCash);
        }

        public IAtm GetAtmByIndex(int index)
        {
            if (index < 0 || index >= _atms.Length) throw new IndexOutOfRangeException("Банкомат не знайдено.");
            return _atms[index];
        }

        public decimal CalculateTotalNetworkBalance()
        {
            decimal totalBalance = 0;
            foreach (IAtm atm in _atms)
            {
                if (atm != null)
                {
                    totalBalance += atm.CalculateTotalBalance();
                }
            }
            return totalBalance;
        }
    }
}
