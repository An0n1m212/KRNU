using Nomer2.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Landings
{
    public class Helicopter : Device, IEngine
    {
        public string EngineType { get; set; }
        public int Power { get; set; }
        public override void GetInfo() => Console.WriteLine($"Вертоліт: {Name}, Гвинтів: 2, Потужність: {Power}");
    }
}
