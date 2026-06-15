using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public abstract class AnimalManager<T> where T : Animal
    {
        protected List<T> _animals = new List<T>();
        public void AddToDatabase(T animal)
        {
            _animals.Add(animal);
        }
        protected string AskName() => Input.ReadStringSafe("Введіть ім'я: ");
        protected double AskAge() => Input.ReadDoubleSafe("Введіть вік: ", 0,100);
        protected double AskSize() => Input.ReadDoubleSafe("Введіть вагу (кг): ",1,100);
        protected bool AskIsSocial() => Input.ReadBoolSafe("Чи соціальна тварина?");
        protected bool AskCanRepro() => Input.ReadBoolSafe("Чи може розмножуватись?");
        protected double AskFood() => Input.ReadDoubleSafe("Скільки споживає корму (кг/день): ",0,100);

        protected FoodType AskFoodType()
        {
            Console.WriteLine("Оберіть раціон:");
            var foods = Enum.GetValues(typeof(FoodType));
            for (int i = 0; i < foods.Length; i++)
            {
                Console.WriteLine($"{i} - {foods.GetValue(i)}");
            }
            int index = Input.ReadIntSafe("Ваш вибір: ", 0, foods.Length - 1);
            return (FoodType)index;
        }

        public abstract void CreateFromConsole();

        public void ShowAll()
        {
            Console.WriteLine($"\n--- Список {typeof(T).Name} ---");
            if (_animals.Count == 0) Console.WriteLine("База порожня.");
            for (int i = 0; i < _animals.Count; i++) Console.WriteLine($"{i}. {_animals[i]}");
        }

        public void Delete(int index)
        {
            if (index >= 0 && index < _animals.Count) _animals.RemoveAt(index);
        }

        public void Edit(int index)
        {
            if (index >= 0 && index < _animals.Count)
            {
                T animal = _animals[index];
                Console.WriteLine($"\n--- Редагування: {animal.Name} ---");

                string name = AskName();
                double age = AskAge();
                double size = AskSize();
                bool isSocial = AskIsSocial();
                bool canRepro = AskCanRepro();
                double food = AskFood();
                FoodType diet = AskFoodType();

                Console.WriteLine("[Система] Дані оновлено.");
            }
            else
            {
                Console.WriteLine("[Помилка] Невірний індекс.");
            }
        }


        public List<T> GetAll()
        {
            return _animals;
        }

        public List<T> GetAllFromDB() => _animals;
    }
}
