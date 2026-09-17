using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class PrintLog
    {
        public string UserName { get; set; }
        public string DocumentName { get; set; }
        public DateTime PrintedAt { get; set; }

        public override string ToString()
        {
            return $"[{PrintedAt:yyyy-MM-dd HH:mm:ss}] Користувач: {UserName} | Документ: {DocumentName}";
        }
    }
}
