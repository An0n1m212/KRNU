using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2.Task
{
    public abstract class Employer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int WorkExperienceYears { get; set; }
        public bool HasHigherEducation { get; set; }
        public decimal Salary { get; set; }
        public int Age => DateTime.Now.Year - BirthDate.Year - (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    }

    public class President : Employer { }
    public class Manager : Employer { }
    public class Worker : Employer { }

    public class CompanyTASK3
    {
        public string Name { get; set; }
        public List<Employer> Employees { get; set; } = new List<Employer>();
    }

    class Task3
    {
        public static void Run()
        {
            CompanyTASK3 company = new CompanyTASK3
            {
                Name = "TechCorp",
                Employees = new List<Employer>
                {
                    new President { FirstName = "Володимир", LastName = "Петров", BirthDate = new DateTime(1980, 5, 12), WorkExperienceYears = 20, HasHigherEducation = true, Salary = 50000 },
                    new Manager { FirstName = "Анна", LastName = "Сидорова", BirthDate = new DateTime(1995, 10, 5), WorkExperienceYears = 6, HasHigherEducation = true, Salary = 25000 },
                    new Manager { FirstName = "Ігор", LastName = "Іванов", BirthDate = new DateTime(1985, 3, 20), WorkExperienceYears = 12, HasHigherEducation = true, Salary = 30000 },
                    new Worker { FirstName = "Володимир", LastName = "Кравченко", BirthDate = new DateTime(2001, 10, 15), WorkExperienceYears = 2, HasHigherEducation = true, Salary = 15000 },
                    new Worker { FirstName = "Олег", LastName = "Коваль", BirthDate = new DateTime(1992, 10, 10), WorkExperienceYears = 8, HasHigherEducation = false, Salary = 18000 },
                    new Worker { FirstName = "Володимир", LastName = "Бойко", BirthDate = new DateTime(1998, 7, 22), WorkExperienceYears = 4, HasHigherEducation = true, Salary = 16000 }
                }
            };

            // 1. Кількість робітників підприємства
            int totalWorkersCount = company.Employees.Count;

            // 2. Об’єм заробітної платні, що необхідно виплатити підприємству своїм робітникам
            decimal totalSalaryVolume = company.Employees.Sum(e => e.Salary);

            // 3. Колекція з 10 робітників, що мають найбільший стаж роботи, серед яких обрати найменшого за віком, що має вищу освіту
            var youngestTopExperiencedWithDegree = company.Employees
                .OrderByDescending(e => e.WorkExperienceYears)
                .Take(10)
                .Where(e => e.HasHigherEducation)
                .MinBy(e => e.Age);

            // 4. Обрати самого молодого та найстаршого менеджера компанії
            var managers = company.Employees.OfType<Manager>();
            var youngestManager = managers.MinBy(m => m.Age);
            var oldestManager = managers.MaxBy(m => m.Age);

            // 5. Колекція робітників компанії, що народилися у жовтні, та видача цієї інформації на екран відповідно до професійного спрямування
            var octoberBornGrouped = company.Employees
                .Where(e => e.BirthDate.Month == 10)
                .GroupBy(e => e.GetType().Name);

            foreach (var group in octoberBornGrouped)
            {
                Console.WriteLine($"Професійне спрямування: {group.Key}");
                foreach (var emp in group)
                {
                    Console.WriteLine($" - {emp.FirstName} {emp.LastName} (Дата народження: {emp.BirthDate:dd.MM.yyyy})");
                }
            }

            // 6. Вивести на екран усіх Володимирів, обрати наймолодшого і поздоровити його з премією у розмірі третини посадового окладу
            var vladimirs = company.Employees
                .Where(e => e.FirstName.Equals("Володимир", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine("Усі Володимири на підприємстві:");
            foreach (var v in vladimirs)
            {
                Console.WriteLine($" - {v.FirstName} {v.LastName}, вік: {v.Age}");
            }

            var youngestVladimir = vladimirs.MinBy(v => v.Age);
            if (youngestVladimir != null)
            {
                decimal bonus = youngestVladimir.Salary / 3m;
                Console.WriteLine($"Вітаємо співробітника {youngestVladimir.FirstName} {youngestVladimir.LastName} з премією у розмірі {bonus:F2} грн (1/3 посадового окладу)!");
            }
        }
    }
}
