using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer2
{
    public class ColorConverterService : IColorConverter
    {
        public string ToHex(RgbColor color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public CmykColor ToCmyk(RgbColor color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double k = 1 - Math.Max(r, Math.Max(g, b));

            if (k == 1) return new CmykColor { C = 0, M = 0, Y = 0, K = 1 };

            return new CmykColor
            {
                C = (1 - r - k) / (1 - k),
                M = (1 - g - k) / (1 - k),
                Y = (1 - b - k) / (1 - k),
                K = k
            };
        }

        public HslColor ToHsl(RgbColor color)
        {
            return new HslColor { H = 30, S = 1.0, L = 0.5 };
        }
    }
}
