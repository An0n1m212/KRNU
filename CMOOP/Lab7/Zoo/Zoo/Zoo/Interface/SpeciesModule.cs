using System;
using System.Collections.Generic;
using System.Text;
using Zoo;

public class SpeciesModule<T> : ISpeciesModule where T : Animal
{
    private readonly AnimalManager<T> _am;
    private readonly EnclosureManager<T> _em;

    public SpeciesModule(AnimalManager<T> am, EnclosureManager<T> em)
    {
        _am = am;
        _em = em;
    }

    public void CreateInteractive()
    {
        _am.CreateFromConsole();

        var last = _am.GetAll().LastOrDefault();
        if (last != null)
        {
            _em.GetEncl(0).Inhabitants.Add(last);
        }
    }

    public void EditAnimal()
    {
        ShowCurrentSpeciesList(); 
        int index = Input.ReadIntSafe("Виберіть номер для редагування: ", 0, _am.GetAllFromDB().Count - 1);
        _am.Edit(index);
    }

    public void DeleteAnimal()
    {
        ShowCurrentSpeciesList();
        int index = Input.ReadIntSafe("Виберіть номер для видалення: ", 0, _am.GetAllFromDB().Count - 1);
        _am.Delete(index);
    }

    private void ShowCurrentSpeciesList()
    {
        var list = _am.GetAllFromDB();
        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"[{i}] {list[i].Name} (Вік: {list[i].Age})");
        }
    }


    public void RelocateProcess()
    {
        var waitEncl = _em.GetEncl(0);
        if (waitEncl.Inhabitants.Count == 0)
        {
            Console.WriteLine("Вольєр очікування порожній.");
            return;
        }

        for (int i = 0; i < waitEncl.Inhabitants.Count; i++)
            Console.WriteLine($"[{i}] {waitEncl.Inhabitants[i].Name}");

        int animalIdx = Input.ReadIntSafe("Виберіть номер тварини: ", 0, waitEncl.Inhabitants.Count - 1);

        _em.ShowStatus();
        int toIdx = Input.ReadIntSafe("Куди переселити (не 0): ", 1, _em.EnclosuresCount - 1);

        var animal = waitEncl.Inhabitants[animalIdx];
        _em.GetEncl(toIdx).Inhabitants.Add(animal);
        waitEncl.Inhabitants.RemoveAt(animalIdx);
    }

    public void CreateEnclosure() => _em.CreateCustomEnclosure();

    public void ShowStatus() => _em.ShowStatus();

    public void ShowStructure()
    {
        Console.WriteLine($"\nСекція {typeof(T).Name}:");
        for (int i = 0; i < _em.EnclosuresCount; i++)
            Console.WriteLine($"  [{i}] {_em.GetEncl(i)}");
    }

    public double GetFoodForPeriod(int days)
    {
        return _am.GetAll().Sum(a => a.Food) * days;
    }

    public List<Animal> GetAllAnimals()
    {
        return _am.GetAll().Cast<Animal>().ToList();
    }

    public void LoadAnimal(string name, int age, double size, bool isSocial, double food)
    {
        T animal = (T)Activator.CreateInstance(typeof(T), name, age, size, isSocial, true);
        animal.Food = food;

        _am.GetAllFromDB().Add(animal);
        _em.GetEncl(0).Inhabitants.Add(animal);
    }
}