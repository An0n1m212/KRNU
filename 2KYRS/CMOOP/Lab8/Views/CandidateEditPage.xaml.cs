using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab8
{
    /// <summary>
    /// Логіка взаємодії для CandidateEditPage.xaml
    /// </summary>
    public partial class CandidateEditPage : Page
    {
        private readonly Candidate _candidate;      // робоча копія, до якої прив'язано інтерфейс
        private readonly Candidate _original;       // оригінал у колекції (null, якщо створюємо нового)
        private readonly bool _isNew = false;

        public CandidateEditPage(Candidate candidateToEdit)
        {
            InitializeComponent();

            if (candidateToEdit == null)
            {
                // Додаємо нового кандидата
                _candidate = new Candidate();
                _isNew = true;
            }
            else
            {
                // Редагуємо копію, щоб при "Скасувати" зміни не залишались у колекції
                _original = candidateToEdit;
                _candidate = candidateToEdit.Clone();
            }

            DataContext = _candidate;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Ручна валідація перед збереженням
            if (string.IsNullOrWhiteSpace(_candidate.FullName))
            {
                MessageBox.Show("Введіть П.І.Б. кандидата!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_candidate.BirthDate > DateTime.Now)
            {
                MessageBox.Show("Дата народження не може бути в майбутньому!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int age = DateTime.Now.Year - _candidate.BirthDate.Year;
            if (age < 16)
            {
                MessageBox.Show("Кандидат має бути старше 16 років!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_candidate.Education))
            {
                MessageBox.Show("Виберіть рівень освіти!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_candidate.ExperienceYears < 0 || _candidate.ExperienceYears > 70)
            {
                MessageBox.Show("Вкажіть коректний стаж роботи (0 - 70 років)!", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isNew)
            {
                DataStorage.Candidates.Add(_candidate);
            }
            else
            {
                _original.CopyFrom(_candidate);
            }

            NavigationService?.Navigate(new CandidatesListPage());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CandidatesListPage());
        }
    }
}
