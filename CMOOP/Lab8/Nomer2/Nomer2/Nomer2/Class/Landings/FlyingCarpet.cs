using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Landings
{
    public class FlyingCarpet : Device
    {
        public override void GetInfo() => Console.WriteLine($"Килим-літак: {Name}, Магічний рівень: High");
    }
}
