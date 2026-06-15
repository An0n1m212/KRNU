using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM.Interface
{
    public interface IAtmUserOperations
    {
        void CheckBalance();
        void Deposit(string userName, int denomination, int count);
        void Withdraw(string userName, int amount);
    }
}
