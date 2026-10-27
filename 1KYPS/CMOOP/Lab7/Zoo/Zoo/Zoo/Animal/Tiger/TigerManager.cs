using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Zoo
{
    public class TigerManager : AnimalManager<Tiger>
    {
        public override void CreateFromConsole()
        {
            Console.WriteLine("\n=== Створення нового Тигра ===");

            string name = AskName();
            double age = AskAge();
            double size = AskSize();
            bool isSocial = AskIsSocial();
            bool canRepro = AskCanRepro();
            double food = AskFood();
            FoodType diet = AskFoodType();
            

            Tiger newTiger = new Tiger(name, age, size, isSocial, canRepro)
            {
                Diet = diet,
                Food = food
            };

            _animals.Add(newTiger);
            Console.WriteLine($"[Система] Тигр {name} доданий.");
        }
    }
}
