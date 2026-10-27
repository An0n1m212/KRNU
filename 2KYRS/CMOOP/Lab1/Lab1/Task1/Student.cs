using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public double AverageScore { get; set; }

        public override string ToString()
        {
            return $"[ID: {Id}] {Name} | Група: {Group} | Середній бал: {AverageScore:F1}";
        }
    }
}
