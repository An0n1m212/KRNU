using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Lab9.Model;

namespace Lab9.Data
{
    /// <summary>
    /// Робота з джерелом даних (файл Students.xml): завантаження списку студентів
    /// та збереження змін назад у файл.
    /// </summary>
    public class StudentRepository
    {
        private readonly string _filePath;

        public StudentRepository()
        {
            // Студенти.xml копіюється в папку виконуваного файлу під час збірки
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Students.xml");
        }

        /// <summary>
        /// Завантажує студентів із XML-файлу. Якщо файл не знайдено — повертає порожню колекцію.
        /// </summary>
        public ObservableCollection<Student> Load()
        {
            var students = new ObservableCollection<Student>();

            if (!File.Exists(_filePath))
            {
                return students;
            }

            var doc = XDocument.Load(_filePath);

            foreach (var element in doc.Root?.Elements("Student") ?? Enumerable.Empty<XElement>())
            {
                var student = new Student
                {
                    LastName = (string)element.Element("LastName"),
                    FirstName = (string)element.Element("FirstName"),
                    Age = (int?)element.Element("Age") ?? 0,
                    Gender = (string)element.Element("Gender")
                };
                students.Add(student);
            }

            return students;
        }

        /// <summary>
        /// Зберігає поточний список студентів у XML-файл.
        /// </summary>
        public void Save(ObservableCollection<Student> students)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Students",
                    students.Select(s =>
                        new XElement("Student",
                            new XElement("LastName", s.LastName),
                            new XElement("FirstName", s.FirstName),
                            new XElement("Age", s.Age),
                            new XElement("Gender", s.Gender)))));

            doc.Save(_filePath);
        }
    }
}
