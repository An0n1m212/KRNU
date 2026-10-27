using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class Kangaroo : Animal
    {
        public Kangaroo(string name, double age, double size, bool isSocial, bool canReproduce)
        {
            Name = name;
            Age = age;
            Size = size;
            IsSocial = isSocial;
            CanReproduce = canReproduce;

            Diet = FoodType.Grass;
        }

        public override void MakeSound() => Console.WriteLine("Hop-hop!");

        public override string ToString() => base.ToString();
    }
}
