using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Zoo
{
    public class KangarooEnclosureManager : EnclosureManager<Kangaroo>
    {

            public void RelocateKangaroo(int from, int to, int idx)
            {
                var source = GetEncl(from);
                var target = GetEncl(to);

                if (idx < 0 || idx >= source.Inhabitants.Count) return;

            Kangaroo kangaroo = source.Inhabitants[idx];

                if (target.Inhabitants.Count > 0 && target.Inhabitants[0].IsSocial != kangaroo.IsSocial)
                {
                    Console.WriteLine("! Конфлікт: не можна селити одинака з соціальними кенгуру.");
                    return;
                }

                if (to > 2 && target.FenceHeight < 6.0)
                {
                    Console.WriteLine("! Небезпека: паркан у новому вольєрі занадто низький для кенгуру!");
                    return;
                }

                InternalMove(from, to, idx);
                Console.WriteLine($"[Успіх] {kangaroo.Name} переїхав у {target.Title}");
            }
        }
    }

