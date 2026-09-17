using Nomer2.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class.Landings
{
    public class HotAirBalloon : Device, IPart
    {
        public string Material { get; set; }
        public override void GetInfo() => Console.WriteLine($"Повітряна куля: {Name}, Матеріал купола: {Material}");
    }
}
