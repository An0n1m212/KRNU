using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class CrocodileManager : AnimalManager<Crocodile>
    {
        public override void CreateFromConsole()
        {
            Console.WriteLine("\n=== Створення нового Крокодил ===");

            string name = AskName();
            double age = AskAge();
            double size = AskSize();
            bool isSocial = AskIsSocial();
            bool canRepro = AskCanRepro();
            double food = AskFood();
            FoodType diet = AskFoodType();

            Crocodile newCrocodile = new Crocodile(name, age, size, isSocial, canRepro)
            {
                Diet = diet,
                Food = food
            };

            _animals.Add(newCrocodile);
            Console.WriteLine($"[Система] Крокодил {name} доданий.");
        }
    }
}
