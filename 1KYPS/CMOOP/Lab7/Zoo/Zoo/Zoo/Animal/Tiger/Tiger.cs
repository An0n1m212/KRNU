using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class Tiger : Animal
    {
        public Tiger(string name, double age, double size, bool isSocial, bool canReproduce)
        {
            Name = name;
            Age = age;
            Size = size;
            IsSocial = isSocial;
            CanReproduce = canReproduce;

            Diet = FoodType.Meat;
        }

        public override void MakeSound() => Console.WriteLine("Roar!");

        public override string ToString() => base.ToString();
    }
}
