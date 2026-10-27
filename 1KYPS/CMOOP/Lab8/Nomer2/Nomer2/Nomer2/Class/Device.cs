using Nomer2.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer2.Class
{
    public abstract class Device : IDevice, ICloneable, IComparable//<Device>
    {
        public string Name { get; set; }
        public bool IsElectronic { get; set; }
        public bool HasEngine { get; set; }

        public abstract void GetInfo();
        public object Clone() => this.MemberwiseClone();
        public int CompareTo(Device other) => string.Compare(this.Name, other.Name);

        public int CompareTo(object? obj)
        {
            return Name.CompareTo(obj);
        }
    }
}
