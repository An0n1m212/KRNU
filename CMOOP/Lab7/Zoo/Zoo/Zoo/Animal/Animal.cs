using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public enum FoodType { Meat, Grass, Fish, Universal }

    public abstract class Animal
    {
        public string Name { get; set; }
        public double Age { get; set; }
        public double Size { get; set; }
        public bool CanReproduce { get; set; }
        public bool IsSocial { get; set; }
        public double Food { get; set; }
        public FoodType Diet { get; set; }
        public abstract void MakeSound();

        public override string ToString()
        {
            string socialStatus = IsSocial ? "Соціальна" : "Одинак";
            string reproStatus = CanReproduce ? "Може розмножуватись" : "Не розмножується";

            return $"[{this.GetType().Name}] Ім'я: {Name}, Вік: {Age}, Розмір: {Size}, " +
                   $"Статус: {socialStatus}, Репродуктивність: {reproStatus}, Раціон: {Diet}, Споживає {Food}кг/день";
        }
    }
}
