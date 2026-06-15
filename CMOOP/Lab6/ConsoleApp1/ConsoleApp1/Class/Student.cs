using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{
    public class Student
    {
        private Person studentData;
        private Education degree;
        private int groupNumber;
        private Exam[] exams;

        public Student(Person person, Education edu, int group)
        {
            studentData = person;
            degree = edu;
            groupNumber = group;
            exams = new Exam[0];
        }

        public Student() : this(new Person(), Education.Bachelor, 101) { }

        // Властивості
        public Person StudentData { get => studentData; set => studentData = value; }
        public Education Degree { get => degree; set => degree = value; }

        public int GroupNumber
        {
            get => groupNumber;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Номер групи має бути додатним");
                groupNumber = value;
            }
        }

        public Exam[] Exams { get => exams; set => exams = value; }

        // Середній бал (тільки get)
        public double AverageGrade
        {
            get
            {
                if (exams == null || exams.Length == 0) return 0;
                return exams.Average(e => e.Grade);
            }
        }

        // Індексатор
        public bool this[Education indexEdu]
        {
            get { return degree == indexEdu; }
        }

        // Додавання іспитів
        public void AddExams(params Exam[] newExams)
        {
            if (newExams == null) return;
            int oldLen = exams.Length;
            Array.Resize(ref exams, oldLen + newExams.Length);
            Array.Copy(newExams, 0, exams, oldLen, newExams.Length);
        }

        public override string ToString()
        {
            string examsList = exams.Length > 0 ? string.Join("\n  ", (object[])exams) : "немає іспитів";
            return $"{studentData}\nФорма: {degree}, Група: {groupNumber}\nІспити:\n  {examsList}";
        }
        public string ToShortString()
        {
            return $"{studentData.ToShortString()}, Форма: {degree}, Група: {groupNumber}, Сер. бал: {AverageGrade:F2}";
        }
    }
}
