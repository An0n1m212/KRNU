using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM
{
    public class AtmCassette
    {
        public int Denomination { get; }
        public int Count { get; private set; }

        public AtmCassette(int denomination, int initialCount)
        {
            if (denomination <= 0)
            {
                throw new ArgumentException("Номінал має бути більшим за нуль.");
            }
            if (initialCount < 0)
            {
                throw new ArgumentException("Кількість купюр не може бути від'ємною.");
            }

            Denomination = denomination;
            Count = initialCount;
        }

        public void Add(int count)
        {
            if (count < 0) throw new ArgumentException("Не можна додати від'ємну кількість купюр.");
            Count += count;
        }

        public void Remove(int count)
        {
            if (count < 0) throw new ArgumentException("Не можна вилучити від'ємну кількість купюр.");
            if (Count < count) throw new InvalidOperationException("Недостатньо купюр у касеті.");
            Count -= count;
        }
    }
}
