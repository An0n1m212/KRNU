using System;
using System.Linq;
using System.Text;

namespace Nomer
{
    class Program
    {
        static Student[] students = new Student[0];

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.Unicode;

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ГОЛОВНЕ МЕНЮ ===");
                Console.WriteLine("1. Додати студента");
                Console.WriteLine("2. Знайти за прізвищем (+ повна інфо)");
                Console.WriteLine("3. Додати іспит студенту");
                Console.WriteLine("4. Вивести список всіх (коротко)");
                Console.WriteLine("0. Вихід");
                Console.Write("\nВаш вибір: ");

                switch (Console.ReadLine())
                {
                    case "1": AddStudent(); Wait(); break;
                    case "2": SearchStudent(); Wait(); break;
                    case "3": AddExamToStudent(); Wait(); break;
                    case "4": ShowAllStudents(); Wait(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Помилка: оберіть число від 0 до 4."); Wait(); break;
                }
            }
        }

        static void AddStudent()
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

            int group = ReadInt("Номер групи: (тільки додатьнє ціле число)");

            Console.WriteLine("Оберіть форму: 1-Specialist, 2-Bachelor, 3-SecondEducation");
            int choice = ReadInt("Ваш вибір: ") - 1;

            Education edu = Enum.IsDefined(typeof(Education), choice)
                            ? (Education)choice
                            : Education.Bachelor;

            Student s = new Student(new Person(fn, ln, bday), edu, group);
            try
            {
                s.GroupNumber = -25;
            }catch(ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }

            Array.Resize(ref students, students.Length + 1);
            students[students.Length - 1] = s;

            Console.WriteLine("Студент доданий успішно!");
        }

        static void SearchStudent()
        {
            string ln = ReadString("\nВведіть прізвище для пошуку: ");
            bool found = false;

            foreach (var s in students)
            {
                if (s.StudentData.LastName.Equals(ln, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n" + s.ToString());
                    found = true;
                }
            }

            if (!found) Console.WriteLine("Студентів з таким прізвищем не знайдено.");
        }

        static void AddExamToStudent()
        {
            string searchName = ReadString("\nВведіть прізвище студента: ");

            Student target = null; 
            foreach (var s in students)
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

        static void ShowAllStudents()
        {
            Console.WriteLine("\n--- Список всіх студентів ---");
            if (students.Length == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            foreach (var s in students) Console.WriteLine(s.ToShortString());
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

        static void Wait()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ReadKey();
        }
    }
}