using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{
    public class Method
    {
        static StudentContainer container = new StudentContainer();

        public static void AddStudent()
        {
            Console.WriteLine("\n--- Реєстрація нового студента ---");

            string fn = ReadString("Ім'я: ");
            string ln = ReadString("Прізвище: ");

            int year = ReadInt("Рік народження (напр. 2005): ");
            int month = ReadInt("Місяць (1-12): ");
            int day = ReadInt("День (1-31): ");

            DateTime bday;
            try { bday = new DateTime(year, month, day); }
            catch
            {
                Console.WriteLine("Некоректна дата. Встановлено 01.01.2000");
                bday = new DateTime(2000, 1, 1);
            }

            int group = ReadInt("Номер групи: ");

            Console.WriteLine("Оберіть форму: 1-Specialist, 2-Bachelor, 3-SecondEducation");
            int choice = ReadInt("Ваш вибір: ") - 1;

            Education edu = Enum.IsDefined(typeof(Education), choice)
                            ? (Education)choice
                            : Education.Bachelor;

            // Створюємо студента та додаємо його в контейнер через метод Add()
            Student s = new Student(new Person(fn, ln, bday), edu, group);
            container.Add(s);

            Console.WriteLine("Студент доданий успішно!");
        }
        public static void SearchStudent()
        {
            string ln = ReadString("\nВведіть прізвище для пошуку: ");
            bool found = false;

            // Завдяки реалізації IEnumerable ми можемо використовувати foreach безпосередньо для контейнера
            foreach (var s in container)
            {
                if (s.StudentData.LastName.Equals(ln, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + s.ToString());
                    found = true;
                }
            }

            if (!found) Console.WriteLine("Студентів з таким прізвищем не знайдено.");
        }

        public static void AddExamToStudent()
        {
            string searchName = ReadString("\nВведіть прізвище студента: ");

            Student target = null;
            foreach (var s in container)
            {
                if (s.StudentData.LastName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                {
                    target = s;
                    break;
                }
            }

            if (target != null)
            {
                string subject = ReadString("Назва предмету: ");
                int grade = ReadInt("Оцінка (2-5): ");

                target.AddExams(new Exam(subject, grade, DateTime.Now));
                Console.WriteLine("Іспит успішно додано!");
            }
            else Console.WriteLine("Студента не знайдено.");
        }

        public static void ShowAllStudents()
        {
            Console.WriteLine("\n--- Список всіх студентів ---");
            if (container.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            foreach (var s in container) Console.WriteLine(s.ToShortString());
        }

        // --- Нові методи для роботи з завданням ---

        public static void SortS()
        {
            Console.WriteLine("\nСортування списку за прізвищем...");
            container.Sort();
            ShowAllStudents();
        }

        public static void SaveS()
        {
            container.Save("all_students.txt");
            Console.WriteLine("\nДані головного контейнера збережено у файл all_students.txt");
        }

        public static void ExecuteTask()
        {
            container.Add(new Student(new Person("Олексій", "Зінченко", new DateTime(2004, 5, 10)), Education.Bachelor, 201));
            container.Add(new Student(new Person("Анна", "Абрамова", new DateTime(2005, 1, 15)), Education.Specialist, 102));
            container.Add(new Student(new Person("Борис", "Бондар", new DateTime(2003, 11, 20)), Education.Bachelor, 201));

            SortS();
            SaveS();

            StudentContainer secondContainer = new StudentContainer();
            for (int i = 0; i < Math.Min(2, container.Count); i++)
            {
                secondContainer.Add(container[i]);
            }

            Console.WriteLine("\n--- Новий контейнер (перші 2 елементи) ---");
            foreach (var s in secondContainer) Console.WriteLine(s.ToShortString());

            secondContainer.Save("top_students.txt");
            Console.WriteLine("Дані нового контейнера збережено в top_students.txt");
        }

        static int ReadInt(string message)
        {
            int result;
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (int.TryParse(input, out result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("Помилка: Введіть ціле число більше за 0.");
            }
        }

        static string ReadString(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                Console.WriteLine("Помилка: Це поле обов'язкове! Введіть текст.");
            }
        }

        public static void Wait()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ReadKey();
        }
    }
}
