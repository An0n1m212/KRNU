using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class Enclosure<T> where T : Animal
    {
        public string Title { get; set; }
        public double AreaSize { get; set; }
        public double FenceHeight { get; set; }
        public double WaterTemperature { get; set; }

        public List<T> Inhabitants { get; set; } = new List<T>();

        public override string ToString()
        {
            string water = WaterTemperature > 0 ? $", Темп. води: {WaterTemperature}°C" : "";
            return $"Вольєр '{Title}' [{AreaSize} м², Паркан: {FenceHeight} м{water}] - Тварин: {Inhabitants.Count}";
        }
    }
}
