using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class PrintRequest
    {
        public string UserName { get; set; } = "NoName";
        public string DocumentName { get; set; } = "Empty";
        public Priority UserPriority { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
