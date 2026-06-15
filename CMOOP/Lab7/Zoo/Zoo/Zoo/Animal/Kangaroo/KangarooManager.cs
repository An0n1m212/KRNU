using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class KangarooManager : AnimalManager<Kangaroo>
    {
        public override void CreateFromConsole()
        {
            Console.WriteLine("\n=== Створення нового Кенгуру ===");

            string name = AskName();
            double age = AskAge();
            double size = AskSize();
            bool isSocial = AskIsSocial();
            bool canRepro = AskCanRepro();
            double food = AskFood();
            FoodType diet = AskFoodType();

            Kangaroo newKangaroo = new Kangaroo(name, age, size, isSocial, canRepro)
            {
                Diet = diet,
                Food = food
            };

            _animals.Add(newKangaroo);
            Console.WriteLine($"[Система] Кенгуру {name} доданий.");
        }
    }
}
