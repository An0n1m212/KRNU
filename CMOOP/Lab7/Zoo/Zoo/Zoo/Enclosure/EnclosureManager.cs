using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;

namespace Zoo
{
    public abstract class EnclosureManager<T> where T : Animal
    {
        protected List<Enclosure<T>> _enclosures = new List<Enclosure<T>>();

        public int EnclosuresCount => _enclosures.Count;

        public void CreateDefaultWaitEnclosures()
        {
                _enclosures.Add(new Enclosure<T>
                {
                    Title = $"WaitEnclosure #{0}",
                    AreaSize = 30.0,
                    FenceHeight = 2.0
                });
            }
        

        public void CreateCustomEnclosure()
        {
            Console.WriteLine($"\n--- Створення вольєра для {typeof(T).Name} ---");
            string title = Input.ReadStringSafe("Назва: ");
            double size = Input.ReadDoubleSafe("Площа (м2): ", 0, 1000);
            double fence = Input.ReadDoubleSafe("Висота паркану (м): ", 0, 10);
            double temp = 0;

            if (typeof(T) == typeof(Crocodile))
                temp = Input.ReadDoubleSafe("Температура води (C): ", -100, 100);

            _enclosures.Add(new Enclosure<T> { Title = title, AreaSize = size, FenceHeight = fence, WaterTemperature = temp });
        }

        public void ShowStatus()
        {
            Console.WriteLine($"\n======= МОНІТОРИНГ: {typeof(T).Name.ToUpper()} =======");
            for (int i = 0; i < _enclosures.Count; i++)
            {
                Console.WriteLine($"[{i}] {_enclosures[i]}");
                foreach (var a in _enclosures[i].Inhabitants)
                    Console.WriteLine($"   -> {a}");
            }
        }

        protected void InternalMove(int from, int to, int animalIdx)
        {
            var animal = _enclosures[from].Inhabitants[animalIdx];
            _enclosures[from].Inhabitants.RemoveAt(animalIdx);
            _enclosures[to].Inhabitants.Add(animal);
        }



        public Enclosure<T> GetEncl(int index) => _enclosures[index];
    }
}