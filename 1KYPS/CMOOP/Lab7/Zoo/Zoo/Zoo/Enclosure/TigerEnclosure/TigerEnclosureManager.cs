using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public class TigerEnclosureManager : EnclosureManager<Tiger>
    {
        public void RelocateTiger(int from, int to, int idx)
        {
            var source = GetEncl(from);
            var target = GetEncl(to);

            if (idx < 0 || idx >= source.Inhabitants.Count) return;

            Tiger tiger = source.Inhabitants[idx];

            if (target.Inhabitants.Count > 0 && target.Inhabitants[0].IsSocial != tiger.IsSocial)
            {
                Console.WriteLine("! Конфлікт: не можна селити одинака з соціальними тиграми.");
                return;
            }

            if (to > 2 && target.FenceHeight < 3.0)
            {
                Console.WriteLine("! Небезпека: паркан у новому вольєрі занадто низький для тигра!");
                return;
            }

            InternalMove(from, to, idx);
            Console.WriteLine($"[Успіх] {tiger.Name} переїхав у {target.Title}");
        }
    }
}


