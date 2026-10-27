using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task1
{
    public class Record
    {
        public string FullName { get; set; } = string.Empty;
        public string HomePhone { get; set; } = string.Empty;
        public string WorkPhone { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // Конструктор без параметрів обов'язковий для XML та JSON серіалізації
        public Record() { }

        public Record(string fullName, string homePhone, string workPhone, string mobilePhone, DateTime birthDate)
        {
            FullName = fullName;
            HomePhone = homePhone;
            WorkPhone = workPhone;
            MobilePhone = mobilePhone;
            BirthDate = birthDate;
        }

        // Метод витягує код мобільного оператора (наприклад, 067, 050, 063)
        public string GetMobileOperatorCode()
        {
            if (string.IsNullOrWhiteSpace(MobilePhone)) return "Невідомо";

            string clean = new string(MobilePhone.Where(char.IsDigit).ToArray());
            if (clean.StartsWith("380") && clean.Length >= 6) return clean.Substring(2, 3);
            if (clean.StartsWith("0") && clean.Length >= 3) return clean.Substring(0, 3);

            return "Інший";
        }

        public override string ToString()
        {
            return $"ПІБ: {FullName}\n" +
                   $"  Дата народження: {BirthDate:dd.MM.yyyy}\n" +
                   $"  Моб. тел.:     {MobilePhone} (Код оператора: {GetMobileOperatorCode()})\n" +
                   $"  Дом. тел.:     {HomePhone}\n" +
                   $"  Роб. тел.:     {WorkPhone}";
        }
    }
}
