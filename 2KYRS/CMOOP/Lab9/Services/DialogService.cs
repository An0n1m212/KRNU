using System.Linq;
using System.Windows;
using Lab9.Model;
using Lab9.ViewModels;
using Lab9.Views;

namespace Lab9.Services
{
    /// <summary>
    /// Сервіс відображення діалогового вікна редактування студента.
    /// ViewModel не залежить від UI: вона лише викликає цей сервіс
    /// і отримує результат (true — збережено, false — скасовано).
    /// </summary>
    public static class DialogService
    {
        public static bool ShowStudentEditor(Student student, bool isNew)
        {
            var dialog = new StudentEditWindow
            {
                DataContext = new StudentEditViewModel(student, isNew),
                Owner = Application.Current?.MainWindow
            };

            return dialog.ShowDialog() == true;
        }
    }
}
