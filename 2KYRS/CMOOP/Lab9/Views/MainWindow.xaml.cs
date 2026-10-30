using System.Windows;
using System.Windows.Controls;
using Lab9.ViewModels;

namespace Lab9.Views
{
    /// <summary>
    /// Головне вікно застосунку. Код біхайнд використовується лише для тонкої
    /// синхронізації виділення списку з ViewModel (Multi-select не біндиться напряму).
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StudentsList.ItemsSource = ((StudentListViewModel)DataContext).Students;
        }

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((StudentListViewModel)DataContext).SyncSelection(StudentsList.SelectedItems);
        }
    }
}
