using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{
    public class Exam
    {
        public string Subject { get; set; }
        public int Grade { get; set; }
        public DateTime ExamDate { get; set; }

        public Exam(string subject, int grade, DateTime date)
        {
            Subject = subject;
            Grade = grade;
            ExamDate = date;
        }

        public Exam() : this("Math", 5, DateTime.Now) { }

        public override string ToString()
        {
            return $"Предмет: {Subject}, Оцінка: {Grade}, Дата: {ExamDate.ToShortDateString()}";
        }
    }

}

       