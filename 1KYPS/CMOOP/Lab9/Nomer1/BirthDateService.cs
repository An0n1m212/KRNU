using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer1
{
    public class BirthDateService : IBirthCacl
    {
        public DayOfWeek DayOfWeek(BirthDate birthDate) 
        {
            return birthDate.ToDateTime().DayOfWeek;
        }

        public DayOfWeek DayOfWeekInYear(BirthDate birthDate, int targetYear)
        {
            DateTime targetDate = new DateTime(targetYear, birthDate.Month, birthDate.Day);
            return targetDate.DayOfWeek;
        }

        public int DaysToNextBrith(BirthDate birthDate) 
        { 
            DateTime today = DateTime.Today;
            DateTime nextBirth = new DateTime(today.Year, birthDate.Month, birthDate.Day);
            if (nextBirth < today) 
            {
                nextBirth = nextBirth.AddDays(1);
            }
            TimeSpan difference = nextBirth - today;
            return difference.Days;
        }
    }
}
