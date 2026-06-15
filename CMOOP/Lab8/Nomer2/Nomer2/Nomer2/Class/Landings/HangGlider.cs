using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Landings
{
    public class HangGlider : Device
    {
        public override void GetInfo() => Console.WriteLine($"Дельтаплан: {Name}, Тип: Спортивний");
    }
}
