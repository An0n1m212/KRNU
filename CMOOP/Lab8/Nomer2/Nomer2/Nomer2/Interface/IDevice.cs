using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Interface
{
    public interface IDevice
    {
        string Name { get; set; }
        void GetInfo();
    }
}
