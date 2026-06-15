using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Zoo
{
    public class CrocodileEnclosureManager : EnclosureManager<Crocodile>
    {
        public void RelocateCrocodile(int from, int to, int idx)
        {
            var source = GetEncl(from);
            var target = GetEncl(to);

            if (idx < 0 || idx >= source.Inhabitants.Count) return;

            Crocodile crocodile = source.Inhabitants[idx];

            if (target.Inhabitants.Count > 0 && target.Inhabitants[0].IsSocial != crocodile.IsSocial)
            {
                Console.WriteLine("! Конфлікт: не можна селити одинака з соціальними крокодилами.");
                return;
            }

            if (target.WaterTemperature > 0 && target.WaterTemperature < 22)
            {
                Console.WriteLine($"[Помилка] Вода у вольєрі '{target.Title}' занадто холодна ({target.WaterTemperature}°C)!");
                return;
            }

            InternalMove(from, to, idx);
            Console.WriteLine($"[Успіх] {crocodile.Name} переїхав у {target.Title}");
        }
    }
}