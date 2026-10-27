using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer2
{
    public interface IColorConverter
    {
        string ToHex(RgbColor color);
        CmykColor ToCmyk(RgbColor color);
        HslColor ToHsl(RgbColor color);
    }
}
