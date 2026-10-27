using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer1
{
    public interface IBirthCacl
    {
        DayOfWeek DayOfWeek(BirthDate birthDate);
        DayOfWeek DayOfWeekInYear(BirthDate birthDate, int targetYear);
        int DaysToNextBrith(BirthDate birthDate);

    }
}
