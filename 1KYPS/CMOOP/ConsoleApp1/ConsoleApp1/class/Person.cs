using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{
    public class Person
    {
        private string firstName;
        private string lastName;
        private DateTime birthDate;

        // Конструктор з параметрами
        public Person(string firstName, string lastName, DateTime birthDate)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.birthDate = birthDate;
        }

        // Конструктор за замовчуванням
        public Person() : this("Ivan", "Ivanov", new DateTime(2000, 1, 1)) { }

        // Властивості
        public string FirstName
        {
            get => firstName;
            set => firstName = value;
        }
        public string LastName
        {
            get => lastName;
            set => lastName = value;
        }
        public DateTime BirthDate
        {
            get => birthDate;
            set => birthDate = value;
        }

        // Властивість для зміни року народження
        public int BirthYear
        {
            get => birthDate.Year;
            set => birthDate = new DateTime(value, birthDate.Month, birthDate.Day);
        }

        // Метод для розрахунку повних років
        public int GetAge()
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now < birthDate.AddYears(age)) age--;
            return age;
        }

        public override string ToString()
        {
            return $"{firstName} {lastName}, Дата народження: {birthDate.ToShortDateString()}";
        }

        public string ToShortString()
        {
            return $"{firstName} {lastName}";
        }
    }

}
