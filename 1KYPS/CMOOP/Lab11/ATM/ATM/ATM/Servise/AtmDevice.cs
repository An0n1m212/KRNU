using ATM.ATM.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM.Servise
{
    public class AtmDevice : IAtm
    {
        public int Id { get; }

        private readonly AtmCassette[] _cassettes;

        public AtmDevice(int id)
        {
            Id = id;
            _cassettes = new AtmCassette[]
            {
                new AtmCassette(1000, 0),
                new AtmCassette(500, 0),
                new AtmCassette(200, 0),
                new AtmCassette(100, 0)
            };
        }

        public decimal CalculateTotalBalance()
        {
            decimal total = 0;
            foreach (AtmCassette cassette in _cassettes)
            {
                total += (decimal)cassette.Denomination * cassette.Count;
            }
            return total;
        }

        public AtmCassette[] GetCassettes()
        {
            AtmCassette[] copy = new AtmCassette[_cassettes.Length];
            for (int i = 0; i < _cassettes.Length; i++)
            {
                copy[i] = new AtmCassette(_cassettes[i].Denomination, _cassettes[i].Count);
            }
            return copy;
        }

        public void AddBanknotes(int denomination, int count)
        {
            bool found = false;
            foreach (AtmCassette cassette in _cassettes)
            {
                if (cassette.Denomination == denomination)
                {
                    cassette.Add(count);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new ArgumentException($"Номінал {denomination} грн не підтримується цим банкоматом.");
            }
        }

        public bool WithdrawBanknotes(int amount, AtmCassette[] dispensedCassettes)
        {
            if (amount <= 0 || amount % 100 != 0) return false;

            int remainingAmount = amount;

            int[] tempCountsToTake = new int[_cassettes.Length];

            for (int i = 0; i < _cassettes.Length; i++)
            {
                int needed = remainingAmount / _cassettes[i].Denomination;
                if (needed > 0)
                {
                    int take = Math.Min(needed, _cassettes[i].Count);
                    tempCountsToTake[i] = take;
                    remainingAmount -= take * _cassettes[i].Denomination;
                }
            }

            if (remainingAmount == 0)
            {
                for (int i = 0; i < _cassettes.Length; i++)
                {
                    if (tempCountsToTake[i] > 0)
                    {
                        _cassettes[i].Remove(tempCountsToTake[i]);
                        // Наповнюємо масив звіту видачі
                        dispensedCassettes[i] = new AtmCassette(_cassettes[i].Denomination, tempCountsToTake[i]);
                    }
                    else
                    {
                        dispensedCassettes[i] = new AtmCassette(_cassettes[i].Denomination, 0);
                    }
                }
                return true;
            }

            return false;
        }
    }
}
