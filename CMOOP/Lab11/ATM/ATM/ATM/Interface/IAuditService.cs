using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM.Interface
{
    public interface IAuditService
    {
        void LogTransaction(string userName, string operationType, decimal amount, string statusMessage);
    }
}
