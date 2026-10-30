using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lab9.Model;

namespace Lab9.ViewModels
{
    /// <summary>
    /// ViewModel вікна анкети студента: робоча копія даних + валідація цілісності.
    /// </summary>
    public class StudentEditViewModel
    {
        /// <summary>Робоча копія даних, до якої прив'язується View.</summary>
        public Student Student { get; }

        /// <summary>Вік у вигляді рядка для TextBox (щоб не ламати введення).</summary>
        public string AgeText { get; set; }

        public StudentEditViewModel(Student student, bool isNew)
        {
            Student = student;
            IsNew = isNew;
            AgeText = student.Age.ToString();
        }

        public bool IsNew { get; }

        /// <summary>Шапка вікна залежно від режиму.</summary>
        public string Title => IsNew ? "Додавання студента" : "Редагування студента";

        /// <summary>
        /// Перевіряє вимоги до цілісності даних. Повертає список конкретних
        /// порушень (порожній список — все добре).
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Student.LastName))
            {
                errors.Add("Прізвище обов'язкове для заповнення");
            }

            if (string.IsNullOrWhiteSpace(Student.FirstName))
            {
                errors.Add("Ім'я обов'язкове для заповнення");
            }

            if (string.IsNullOrWhiteSpace(Student.Gender))
            {
                errors.Add("Стать обов'язково вказати");
            }

            if (!int.TryParse(AgeText?.Trim(), out int age))
            {
                errors.Add("Вік має бути цілим числом");
            }
            else if (age < 16 || age > 100)
            {
                errors.Add("Вік має бути в межах від 16 до 100");
            }
            else
            {
                Student.Age = age;
            }

            return errors;
        }

        /// <summary>
        /// Формує повідомлення користувачу про всі невідповідності.
        /// </summary>
        public string BuildErrorMessage()
        {
            var sb = new StringBuilder("Порушено вимоги до заповнення полів:");
            foreach (var error in Validate())
            {
                sb.AppendLine();
                sb.Append("• ").Append(error);
            }
            return sb.ToString();
        }

        /// <summary>Чи проходять дані валідацію.</summary>
        public bool IsValid => Validate().Count == 0;
    }
}
