using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Interface
{
    public interface IEngine
    {
        string EngineType { get; set; }
        int Power { get; set; }
    }
}
