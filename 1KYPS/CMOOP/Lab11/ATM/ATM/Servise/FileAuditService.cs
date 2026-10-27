using ATM.ATM.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Servise
{
    public class FileAuditService : IAuditService
    {
        private readonly string _filePath;

        public FileAuditService(string filePath = "atm_audit.txt")
        {
            _filePath = filePath;
        }

        public void LogTransaction(string userName, string operationType, decimal amount, string statusMessage)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, true, System.Text.Encoding.UTF8))
            {
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Користувач: {userName} | " +
                                 $"Операція: {operationType} | Сума: {amount} грн. | Статус: {statusMessage}";
                writer.WriteLine(logLine);
            }
        }
    }
}
