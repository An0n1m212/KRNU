using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class Crocodile : Animal
    {
        public Crocodile(string name, double age, double size, bool isSocial, bool canReproduce)
        {
            Name = name;
            Age = age;
            Size = size;
            IsSocial = isSocial;
            CanReproduce = canReproduce;

            Diet = FoodType.Fish;
        }

        public override void MakeSound() => Console.WriteLine("Snap!");

        public override string ToString() => base.ToString();
    }
}
