using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{


    public class StudentContainer : IFileContainer<Student>
    {
        private List<Student> _students = new List<Student>();
        private bool _isSaved = false;

        public int Count => _students.Count;
        public bool IsDataSaved => _isSaved;

        public Student this[int index]
        {
            get
            {
                if (index < 0 || index >= _students.Count) throw new IndexOutOfRangeException();
                return _students[index];
            }
            set
            {
                if (index < 0 || index >= _students.Count) throw new IndexOutOfRangeException();
                _students[index] = value;
                _isSaved = false;
            }
        }

        public void Add(Student element) { _students.Add(element); _isSaved = false; }

        public void Delete(Student element) { _students.Remove(element); _isSaved = false; }

        public void Sort() => _students.Sort((s1, s2) => s1.StudentData.CompareTo(s2.StudentData));

        public void Save(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var s in _students) sw.WriteLine(s.ToShortString());
            }
            _isSaved = true;
        }

        public void Load(string fileName)
        {
            if (File.Exists(fileName)) Console.WriteLine("Дані завантажено з " + fileName);
        }

        public IEnumerator<Student> GetEnumerator() => _students.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}