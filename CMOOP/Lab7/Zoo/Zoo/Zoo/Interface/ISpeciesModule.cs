using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo
{
    public interface ISpeciesModule
    {
        void CreateInteractive();
        void EditAnimal(); 
        void DeleteAnimal();
        void RelocateProcess();

        void CreateEnclosure();

        void ShowStatus();
        void ShowStructure();

        double GetFoodForPeriod(int days);

        void LoadAnimal(string name, int age, double size, bool isSocial, double food);

        System.Collections.Generic.List<Animal> GetAllAnimals();
    }
}