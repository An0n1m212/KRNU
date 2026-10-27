using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2.Task
{
    public class Phone
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }

        public override string ToString() => $"{Manufacturer} {Name} - ${Price} ({ReleaseDate:yyyy-MM-dd})";
    }

    class Task2
    {
        public static void Run()
        {
            List<Phone> phones = new List<Phone>
            {
                new Phone { Name = "IPhone 13", Manufacturer = "Apple", Price = 800, ReleaseDate = new DateTime(2021, 9, 24) },
                new Phone { Name = "IPhone 10", Manufacturer = "Apple", Price = 450, ReleaseDate = new DateTime(2017, 11, 3) },
                new Phone { Name = "Galaxy S22", Manufacturer = "Samsung", Price = 750, ReleaseDate = new DateTime(2022, 2, 25) },
                new Phone { Name = "Redmi Note 10", Manufacturer = "Xiaomi", Price = 200, ReleaseDate = new DateTime(2021, 3, 16) },
                new Phone { Name = "Xperia 5", Manufacturer = "Sony", Price = 600, ReleaseDate = new DateTime(2019, 10, 5) },
                new Phone { Name = "Old Phone", Manufacturer = "Nokia", Price = 50, ReleaseDate = new DateTime(2010, 1, 1) }
            };

            // Порахувати кількість телефонів
            int totalCount = phones.Count;

            // Порахувати кількість телефонів із ціною більше 100
            int countOver100 = phones.Count(p => p.Price > 100);

            // Порахувати кількість телефонів із ціною в діапазоні від 400 до 700
            int countRange = phones.Count(p => p.Price >= 400 && p.Price <= 700);

            // Порахувати кількість телефонів конкретного виробника (наприклад, Apple)
            int appleCount = phones.Count(p => p.Manufacturer == "Apple");

            // Знайти телефон із мінімальною ціною
            var cheapestPhone = phones.MinBy(p => p.Price);

            // Знайти телефон із максимальною ціною
            var mostExpensivePhone = phones.MaxBy(p => p.Price);

            // Відобразити інформацію про найстаріший телефон
            var oldestPhone = phones.MinBy(p => p.ReleaseDate);

            // Відобразити інформацію про найсвіжіший телефон
            var newestPhone = phones.MaxBy(p => p.ReleaseDate);

            // Знайти середню ціну телефону
            decimal averagePrice = phones.Average(p => p.Price);

            // Відобразити п’ять найдорожчих телефонів
            var top5Expensive = phones.OrderByDescending(p => p.Price).Take(5);

            // Відобразити п’ять найдешевших телефонів
            var top5Cheapest = phones.OrderBy(p => p.Price).Take(5);

            // Відобразити три найстаріші телефони
            var top3Oldest = phones.OrderBy(p => p.ReleaseDate).Take(3);

            // Відобразити три найновіші телефони
            var top3Newest = phones.OrderByDescending(p => p.ReleaseDate).Take(3);

            // Статистика щодо кількості телефонів кожного виробника (наприклад: Sony – 3, Samsung – 4)
            var statsByManufacturer = phones.GroupBy(p => p.Manufacturer)
                                            .Select(g => $"{g.Key} – {g.Count()}");

            // Статистика щодо кількості моделей телефонів
            var statsByModel = phones.GroupBy(p => p.Name)
                                     .Select(g => $"{g.Key} – {g.Count()}");

            // Статистика телефонів за роками
            var statsByYear = phones.GroupBy(p => p.ReleaseDate.Year)
                                    .Select(g => $"{g.Key} – {g.Count()}");
        }
    }
}
