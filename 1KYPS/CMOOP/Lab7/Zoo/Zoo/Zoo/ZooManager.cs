namespace Zoo
{
    public class ZooManager
    {
        public TigerManager Tiger { get; set; } = new TigerManager();
        public TigerEnclosureManager TigerEncl { get; set; } = new TigerEnclosureManager();

        public KangarooManager Kangaroo { get; set; } = new KangarooManager();
        public KangarooEnclosureManager KangarooEncl { get; set; } = new KangarooEnclosureManager();

        public CrocodileManager Crocodile { get; set; } = new CrocodileManager();
        public CrocodileEnclosureManager CrocodileEncl { get; set; } = new CrocodileEnclosureManager();

        private Dictionary<int, ISpeciesModule> _speciesMenu;

        public Dictionary<string, ISpeciesModule> GetSections() => new Dictionary<string, ISpeciesModule>
            {
                { "SECTION:TIGERS", _speciesMenu[1] },
                { "SECTION:KANGAROOS", _speciesMenu[2] },
                { "SECTION:CROCODILES", _speciesMenu[3] }
            };

        public ZooManager()
        {
            TigerEncl.CreateDefaultWaitEnclosures();
            KangarooEncl.CreateDefaultWaitEnclosures();
            CrocodileEncl.CreateDefaultWaitEnclosures();

            _speciesMenu = new Dictionary<int, ISpeciesModule>
            {
                { 1, new SpeciesModule<Tiger>(Tiger, TigerEncl) },
                { 2, new SpeciesModule<Kangaroo>(Kangaroo, KangarooEncl) },
                { 3, new SpeciesModule<Crocodile>(Crocodile, CrocodileEncl) }
            };
        }
        public void AddAnimal()
        {
            int type = SelectSpecies("Кого додаємо?");
            _speciesMenu[type].CreateInteractive();
        }
        public void RemoveAnimal()
        {
            int type = SelectSpecies("Кого хочете видалити?");
            _speciesMenu[type].DeleteAnimal();
        }

        public void ModifyAnimal()
        {
            int type = SelectSpecies("Кого хочете редагувати?");
            _speciesMenu[type].EditAnimal();
        }

        public void Relocate()
        {
            int type = SelectSpecies("Кого розселяємо?");
            _speciesMenu[type].RelocateProcess();
        }

        public void CreateEncl()
        {
            int type = SelectSpecies("Для кого створюємо вольєр?");
            _speciesMenu[type].CreateEnclosure();
        }

        public void ShowAllStatus()
        {
            foreach (var module in _speciesMenu.Values)
                module.ShowStatus();
        }

        public void ShowStructure()
        {
            foreach (var module in _speciesMenu.Values)
                module.ShowStructure();
        }

        public void CalculateFood()
        {
            int days = Input.ReadIntSafe("На скільки днів розрахувати запас? ", 1, 365);
            double total = 0;

            Console.WriteLine($"\n--- ПРОГНОЗ КОРМУ НА {days} ДНІ(В) ---");
            foreach (var entry in _speciesMenu)
            {
                double speciesFood = entry.Value.GetFoodForPeriod(days);
                total += speciesFood;
                Console.WriteLine($"Вид #{entry.Key}: {speciesFood:F2} кг");
            }
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"ЗАГАЛОМ: {total:F2} кг\n");
        }

        private int SelectSpecies(string message)
        {
            Console.WriteLine($"\n{message}");
            Console.WriteLine("1 - Тигр, 2 - Кенгуру, 3 - Крокодил");
            return Input.ReadIntSafe("Ваш вибір: ", 1, _speciesMenu.Count);
        }

        public Dictionary<int, ISpeciesModule> GetModules() => _speciesMenu;
    }
}


//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Timers;


//namespace Zoo
//{
//    public class ZooManager
//    {
//        private Dictionary<int, ISpeciesModule> _speciesMenu;
//        public TigerManager Tiger { get; set; } = new TigerManager();
//        public TigerEnclosureManager TigerEncl { get; set; } = new TigerEnclosureManager();

//        public KangarooManager Kangaroo { get; set; } = new KangarooManager();
//        public KangarooEnclosureManager KangarooEncl { get; set; } = new KangarooEnclosureManager();

//        public CrocodileManager Crocodile { get; set; } = new CrocodileManager();
//        public CrocodileEnclosureManager CrocodileEncl { get; set; } = new CrocodileEnclosureManager();

//        public ZooManager()
//        {
//            InitializeDefaultZoo();

//            _speciesMenu = new Dictionary<int, ISpeciesModule>
//        {
//            { 1, new SpeciesModule<Tiger>(Tiger, TigerEncl) },
//            { 2, new SpeciesModule<Kangaroo>(Kangaroo, KangarooEncl) },
//            { 3, new SpeciesModule<Crocodile>(Crocodile, CrocodileEncl) }
//        };
//        }

//        private void InitializeDefaultZoo()
//        {
//            TigerEncl.CreateDefaultWaitEnclosures();
//            KangarooEncl.CreateDefaultWaitEnclosures();
//            CrocodileEncl.CreateDefaultWaitEnclosures();
//        }

//        public void AddAnimalWorkflow()
//        {
//            Console.WriteLine("\nОберіть вид: 1-Тигр, 2-Кенгуру, 3-Крокодил");
//            int type = Input.ReadIntSafe("Вибір: ", 1, 3);

//            if (type == 1)
//            {
//                Tiger.CreateFromConsole();
//                TigerEncl.GetEncl(0).Inhabitants.Add(Tiger.GetAll().Last());
//            }
//            else if (type == 2)
//            {
//                Kangaroo.CreateFromConsole();
//                KangarooEncl.GetEncl(0).Inhabitants.Add(Kangaroo.GetAll().Last());
//            }
//            else
//            {
//                Crocodile.CreateFromConsole();
//                CrocodileEncl.GetEncl(0).Inhabitants.Add(Crocodile.GetAll().Last());
//            }
//            Console.WriteLine("[Система] Тварину додано у вольєр очікування.");
//        }

//        public void RelocateAnimalWorkflow()
//        {
//            Console.WriteLine("\nКого розселяємо? 1-Тигр, 2-Кенгуру, 3-Крокодил");
//            int type = Input.ReadIntSafe("Вибір: ", 1, 3);

//            if (type == 1) RelocateProcess(TigerEncl);
//            else if (type == 2) RelocateProcess(KangarooEncl);
//            else RelocateProcess(CrocodileEncl);
//        }

//        private void RelocateProcess<T>(EnclosureManager<T> manager) where T : Animal
//        {
//            var waitEncl = manager.GetEncl(0);
//            if (waitEncl.Inhabitants.Count == 0)
//            {
//                Console.WriteLine("У вольєрі очікування порожньо!");
//                return;
//            }

//            Console.WriteLine("\n--- Тварини у вольєрі очікування ---");
//            for (int i = 0; i < waitEncl.Inhabitants.Count; i++)
//                Console.WriteLine($"[{i}] {waitEncl.Inhabitants[i].Name}");

//            int animalIdx = Input.ReadIntSafe("Виберіть номер тварини: ", 0, waitEncl.Inhabitants.Count - 1);

//            manager.ShowStatus();
//            int toIdx = Input.ReadIntSafe("У який вольєр переселити (0 - залишити в очікуванні): ", 0, manager.EnclosuresCount - 1);

//            if (toIdx == 0) return;

//            if (manager is TigerEnclosureManager tem) tem.RelocateTiger(0, toIdx, animalIdx);
//            else if (manager is CrocodileEnclosureManager cem) cem.RelocateCrocodile(0, toIdx, animalIdx);
//            else if (manager is KangarooEnclosureManager kem) kem.RelocateKangaroo(0, toIdx, animalIdx);
//        }
//        public void CreateCustomEnclosureWorkflow()
//        {
//            Console.WriteLine("\nДля кого вольєр? 1-Тигр, 2-Кенгуру, 3-Крокодил");
//            int type = Input.ReadIntSafe("Вибір: ", 1, 3);

//            if (type == 1) TigerEncl.CreateCustomEnclosure();
//            else if (type == 2) KangarooEncl.CreateCustomEnclosure();
//            else CrocodileEncl.CreateCustomEnclosure();
//        }
//        public void ShowZooStatus()
//        {
//            TigerEncl.ShowStatus();
//            KangarooEncl.ShowStatus();
//            CrocodileEncl.ShowStatus();
//        }

//        public void ShowZooStructure()
//        {
//            Console.WriteLine("\n========== ТЕХНІЧНІ ПАРАМЕТРИ ==========");
//            PrintSec(TigerEncl);
//            PrintSec(KangarooEncl);
//            PrintSec(CrocodileEncl);
//        }

//        private void PrintSec<T>(EnclosureManager<T> manager) where T : Animal
//        {
//            Console.WriteLine($"\nСекція {typeof(T).Name}:");
//            for (int i = 0; i < manager.EnclosuresCount; i++)
//                Console.WriteLine($"[{i}] {manager.GetEncl(i)}");
//        }

//        public void CalculateFoodWorkflow()
//        {
//            Console.Clear();
//            Console.WriteLine("========================================");
//            Console.WriteLine("    ПРОГНОЗ СПОЖИВАННЯ КОРМУ           ");
//            Console.WriteLine("========================================");

//            int days = Input.ReadIntSafe("На скільки днів розрахувати запас? ", 1, 365);

//            double meatPerDay = 0;
//            double grassPerDay = 0;
//            double fishPerDay = 0;

//            foreach (var t in Tiger.GetAll()) meatPerDay += t.Food;
//            foreach (var k in Kangaroo.GetAll()) grassPerDay += k.Food;
//            foreach (var c in Crocodile.GetAll()) fishPerDay += c.Food;

//            double totalMeat = meatPerDay * days;
//            double totalGrass = grassPerDay * days;
//            double totalFish = fishPerDay * days;

//            Console.WriteLine($"\n--- ПЛАН НА {days} ДНІ(В) ---");
//            Console.WriteLine($"[М'ясо]   Потрібно: {totalMeat:F2} кг (в день: {meatPerDay:F1})");
//            Console.WriteLine($"[Трава]   Потрібно: {totalGrass:F2} кг (в день: {grassPerDay:F1})");
//            Console.WriteLine($"[Риба]    Потрібно: {totalFish:F2} кг (в день: {fishPerDay:F1})");

//            Console.WriteLine("----------------------------------------");
//            double totalAll = totalMeat + totalGrass + totalFish;
//            Console.WriteLine($"ЗАГАЛЬНА ВАГА ЗАПАСІВ: {totalAll:F2} кг");
//            Console.WriteLine("========================================\n");

//            Console.WriteLine("Натисніть будь-яку клавішу...");
//            Console.ReadKey();
//        }
//        public void SaveData()
//        {
//            Console.WriteLine($"\n[Система] Дані успішно збережено у файл {0}");
//        }

//        public void LoadData()
//        {
//            Console.WriteLine($"\n[Система] Дані успішно підвантаженні у файл {0}...");
//        }

//    }
//}