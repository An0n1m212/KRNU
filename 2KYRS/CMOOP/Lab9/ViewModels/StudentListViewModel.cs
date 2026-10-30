using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Lab9.Data;
using Lab9.Model;
using Lab9.Services;

namespace Lab9.ViewModels
{
    /// <summary>
    /// ViewModel головного вікна: надає списку студентів дані та команди
    /// (додати / редагувати / видалити). Логіка перебуває тут, а не в коді View.
    /// </summary>
    public class StudentListViewModel
    {
        private readonly StudentRepository _repository;

        public ObservableCollection<Student> Students { get; private set; }

        /// <summary>
        /// Синхронізується з виділенням ListBox з боку View (викликається з обробника SelectionChanged).
        /// </summary>
        public List<Student> SelectedStudents { get; private set; } = new List<Student>();

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public StudentListViewModel()
        {
            _repository = new StudentRepository();
            Students = _repository.Load();

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit, _ => CanEdit);
            DeleteCommand = new RelayCommand(Delete, _ => CanDelete);
        }

        /// <summary>Редагувати можна лише один виділений запис.</summary>
        public bool CanEdit => SelectedStudents.Count == 1;

        /// <summary>Видалення доступне, якщо виділено хоча б один запис (при порожньому списку вимкнено).</summary>
        public bool CanDelete => SelectedStudents.Count > 0;

        /// <summary>
        /// Оновлює виділені елементи (викликається View при зміні виділення).
        /// </summary>
        public void SyncSelection(IList selection)
        {
            SelectedStudents = selection.Cast<Student>().ToList();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private void Add(object parameter)
        {
            var student = new Student { Age = 16, Gender = "Чоловіча" };

            if (DialogService.ShowStudentEditor(student, isNew: true))
            {
                Students.Add(student);
                _repository.Save(Students);
            }
        }

        private void Edit(object parameter)
        {
            var student = SelectedStudents.FirstOrDefault();
            if (student == null)
            {
                return;
            }

            // Редагуємо копію, щоб при "Скасувати" зміни не залишались у списку
            var copy = new Student
            {
                LastName = student.LastName,
                FirstName = student.FirstName,
                Age = student.Age,
                Gender = student.Gender
            };

            if (DialogService.ShowStudentEditor(copy, isNew: false))
            {
                student.LastName = copy.LastName;
                student.FirstName = copy.FirstName;
                student.Age = copy.Age;
                student.Gender = copy.Gender;
                _repository.Save(Students);
            }
        }

        private void Delete(object parameter)
        {
            int count = SelectedStudents.Count;
            string message = count == 1
                ? $"Видалити студента {SelectedStudents[0].FullName}?"
                : $"Видалити студентів: {count}?";

            if (MessageBox.Show(message, "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var student in SelectedStudents.ToList())
                {
                    Students.Remove(student);
                }
                SelectedStudents.Clear();
                _repository.Save(Students);
            }
        }
    }
}
