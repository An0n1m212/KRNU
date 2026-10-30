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
using System.Windows.Shapes;

namespace Lab8
{
    /// <summary>
    /// Логіка взаємодії для CandidatesListPage.xaml
    /// </summary>
    public partial class CandidatesListPage : Page
    {
        public CandidatesListPage()
        {
            InitializeComponent();
            UpdateList();
        }

        private void UpdateList()
        {
            // Захист від виклику до повної ініціалізації XAML
            if (LstCandidates == null) return;

            IEnumerable<Candidate> filtered = DataStorage.Candidates;

            // Фільтр за освітою (значення мають збігатися з ComboBox на сторінці анкети)
            if (CmbFilterEducation?.SelectedItem is ComboBoxItem selectedEdu && selectedEdu.Content?.ToString() != "Всі")
            {
                filtered = filtered.Where(c => c.Education == selectedEdu.Content.ToString());
            }

            // Фільтр за стажем
            if (int.TryParse(TxtFilterExp?.Text, out int minExp))
            {
                filtered = filtered.Where(c => c.ExperienceYears >= minExp);
            }

            // Фільтр за володінням ПК
            if (ChkFilterComputer?.IsChecked == true)
            {
                filtered = filtered.Where(c => c.KnowsComputer);
            }

            // Фільтр за наявністю рекомендацій
            if (ChkFilterRec?.IsChecked == true)
            {
                filtered = filtered.Where(c => c.HasRecommendations);
            }

            LstCandidates.ItemsSource = filtered.ToList();
        }

        private void ApplyFilter(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbFilterEducation.SelectedIndex = 0;
            TxtFilterExp.Text = string.Empty;
            ChkFilterComputer.IsChecked = false;
            ChkFilterRec.IsChecked = false;
            UpdateList();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Candidate candidate)
            {
                NavigationService?.Navigate(new CandidateEditPage(candidate));
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Candidate candidate)
            {
                if (MessageBox.Show($"Видалити анкету: {candidate.FullName}?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DataStorage.Candidates.Remove(candidate);
                    UpdateList();
                }
            }
        }
    }
}
