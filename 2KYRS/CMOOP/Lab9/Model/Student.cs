using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lab9.Model
{
    /// <summary>
    /// Модель студента (чотири дані з файлу Students.xml: прізвище, ім'я, вік, стать).
    /// Реалізує INotifyPropertyChanged, щоб View оновлювалась через прив'язку даних.
    /// </summary>
    public class Student : INotifyPropertyChanged
    {
        private string _lastName;
        private string _firstName;
        private int _age;
        private string _gender;

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); OnPropertyChanged(nameof(AgeDisplay)); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Прізвище та ім'я як єдине ціле (вимога до дизайну).
        /// </summary>
        public string FullName
        {
            get
            {
                string last = _lastName?.Trim() ?? string.Empty;
                string first = _firstName?.Trim() ?? string.Empty;
                return (last + " " + first).Trim();
            }
        }

        /// <summary>
        /// Вік з постфіксом "рік / роки / років" залежно від значення (вимога до дизайну).
        /// </summary>
        public string AgeDisplay => $"{Age} {AgeSuffix(Age)}";

        /// <summary>
        /// Правильна форма слова "рік" для віку: 1 рік, 2 роки, 5 років,
        /// а для 11-14 — тільки "років".
        /// </summary>
        public static string AgeSuffix(int age)
        {
            int lastDigit = age % 10;
            int lastTwoDigits = age % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14) return "років";
            if (lastDigit == 1) return "рік";
            if (lastDigit >= 2 && lastDigit <= 4) return "роки";
            return "років";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
