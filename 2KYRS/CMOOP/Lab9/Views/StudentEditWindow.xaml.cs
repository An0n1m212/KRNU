using System.Windows;
using Lab9.ViewModels;

namespace Lab9.Views
{
    /// <summary>
    /// Діалогове вікно редагування студента. Логіка валідації — у ViewModel,
    /// тут лише получает результат у DialogResult.
    /// </summary>
    public partial class StudentEditWindow : Window
    {
        public StudentEditWindow()
        {
            InitializeComponent();
        }

        private StudentEditViewModel ViewModel => (StudentEditViewModel)DataContext;

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Validate().Count == 0)
            {
                DialogResult = true;
                return;
            }

            MessageBox.Show(ViewModel.BuildErrorMessage(), "Помилка валідації",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
