using Nomer2.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Auxiliary
{
    public class Registry
    {
        private List<Device> _items = new List<Device>();

        public void AddDevice(Device d) => _items.Add(d);

        public void ShowAll()
        {
            Console.WriteLine("\n--- Весь список обладнання ---");
            _items.ForEach(i => i.GetInfo());
        }

        public void ShowElectronic()
        {
            Console.WriteLine("\n--- Електронне обладнання ---");
            _items.Where(i => i.IsElectronic).ToList().ForEach(i => i.GetInfo());
        }

        public void ShowWithoutEngine()
        {
            Console.WriteLine("\n--- Устаткування без двигунів ---");
            _items.Where(i => !i.HasEngine).ToList().ForEach(i => i.GetInfo());
        }

        public void SortByName() => _items.Sort();

        public void SortByPower() => _items.Sort(( x , y) => {
            int p1 = (x is IEngine e1) ? e1.Power : 0;
            int p2 = (y is IEngine e2) ? e2.Power : 0;
            return p1.CompareTo(p2);
        });

        //public int Compare(Device x, Device y)
        //{
        //    int p1 = (x is IEngine e1) ? e1.Power : 0;
        //    int p2 = (y is IEngine e2) ? e2.Power : 0;
        //    return p1.CompareTo(p2);
        //}
    }
}
