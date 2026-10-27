using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2.Task
{
    public class Company
    {
        public string Name { get; set; }
        public DateTime FoundationDate { get; set; }
        public string BusinessProfile { get; set; }
        public string DirectorFullName { get; set; }
        public int EmployeeCount { get; set; }
        public string Address { get; set; }

        public override string ToString() =>
            $"Фірма: {Name} | Профіль: {BusinessProfile} | Директор: {DirectorFullName} | Працівники: {EmployeeCount} | Адреса: {Address} | Дата заснування: {FoundationDate:yyyy-MM-dd}";
    }

    class Task1
    {
        public static void Run()
        {
            List<Company> companies = new List<Company>
            {
                new Company { Name = "Food", FoundationDate = DateTime.Now.AddYears(-3), BusinessProfile = "Food Industry", DirectorFullName = "John White", EmployeeCount = 150, Address = "London" },
                new Company { Name = "IT Global White", FoundationDate = DateTime.Now.AddDays(-100), BusinessProfile = "IT", DirectorFullName = "Alex Black", EmployeeCount = 250, Address = "London" },
                new Company { Name = "Marketing Pro", FoundationDate = DateTime.Now.AddYears(-1), BusinessProfile = "Marketing", DirectorFullName = "Sarah Connor", EmployeeCount = 80, Address = "New York" },
                new Company { Name = "Tech Solutions", FoundationDate = DateTime.Now.AddYears(-5), BusinessProfile = "IT", DirectorFullName = "Michael White", EmployeeCount = 350, Address = "Kyiv" },
                new Company { Name = "Smart Marketing", FoundationDate = DateTime.Now.AddDays(-200), BusinessProfile = "Marketing", DirectorFullName = "David Smith", EmployeeCount = 120, Address = "London" }
            };

            // 1. Отримати інформацію про всі фірми
            var allCompanies = companies;

            // 2. Отримати фірми, які мають назву Food
            var foodCompanies = companies.Where(c => c.Name.Equals("Food", StringComparison.OrdinalIgnoreCase));

            // 3. Отримати фірми, що працюють у галузі маркетингу
            var marketingCompanies = companies.Where(c => c.BusinessProfile.Equals("Marketing", StringComparison.OrdinalIgnoreCase));

            // 4. Отримати фірми, що працюють у галузі маркетингу або IT
            var marketingOrIt = companies.Where(c => c.BusinessProfile.Equals("Marketing", StringComparison.OrdinalIgnoreCase) ||
                                                     c.BusinessProfile.Equals("IT", StringComparison.OrdinalIgnoreCase));

            // 5. Отримати фірми з кількістю співробітників більше 100
            var countMore100 = companies.Where(c => c.EmployeeCount > 100);

            // 6. Отримати фірми з кількістю співробітників у діапазоні від 100 до 300
            var countRange = companies.Where(c => c.EmployeeCount >= 100 && c.EmployeeCount <= 300);

            // 7. Отримати фірми, що знаходяться у Лондоні
            var londonCompanies = companies.Where(c => c.Address.Equals("London", StringComparison.OrdinalIgnoreCase));

            // 8. Отримати фірми, які мають прізвище директора White
            var directorWhite = companies.Where(c => c.DirectorFullName.EndsWith("White", StringComparison.OrdinalIgnoreCase));

            // 9. Отримати фірми, які засновані понад два роки тому
            var olderThan2Years = companies.Where(c => c.FoundationDate < DateTime.Now.AddYears(-2));

            // 10. Отримати фірми, з дня заснування яких минуло більше 150 днів
            var olderThan150Days = companies.Where(c => (DateTime.Now - c.FoundationDate).TotalDays > 150);

            // 11. Отримати фірми, у яких прізвище директора Black та назва фірми містить слово White
            var blackDirectorWhiteName = companies.Where(c => c.DirectorFullName.EndsWith("Black", StringComparison.OrdinalIgnoreCase) &&
                                                              c.Name.Contains("White", StringComparison.OrdinalIgnoreCase));
        }
    }
}
