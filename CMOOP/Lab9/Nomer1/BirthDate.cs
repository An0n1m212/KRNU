using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nomer1
{
    public struct BirthDate
    {
        public int Day { get; }
        public int Month { get; }
        public int Year { get; }
        public BirthDate(int day, int month, int year)
        {
            DateTime temp = new DateTime(year, month, day);
            if (temp > DateTime.Now)
            {
                throw new ArgumentException("");
            }
            Day = day;
            Month = month;
            Year = year;
        }
        public DateTime ToDateTime() => new DateTime(Year, Month, Day);
    }
}
