using Nomer2.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Auxiliary
{
    public class PowerComparer : IComparer<Device>
    {
        public int Compare(Device x, Device y)
        {
            int p1 = (x is IEngine e1) ? e1.Power : 0;
            int p2 = (y is IEngine e2) ? e2.Power : 0;
            return p1.CompareTo(p2);
        }

    }
}
