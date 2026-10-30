using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab8
{
    public class Candidate : INotifyPropertyChanged
    {
        private string _fullName;
        private DateTime _birthDate = DateTime.Now.AddYears(-20);
        private string _education;
        private string _languageSkills;
        private bool _knowsComputer;
        private int _experienceYears;
        private bool _hasRecommendations;

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set { _birthDate = value; OnPropertyChanged(); }
        }

        public string Education
        {
            get => _education;
            set { _education = value; OnPropertyChanged(); }
        }

        public string LanguageSkills
        {
            get => _languageSkills;
            set { _languageSkills = value; OnPropertyChanged(); }
        }

        public bool KnowsComputer
        {
            get => _knowsComputer;
            set { _knowsComputer = value; OnPropertyChanged(); }
        }

        public int ExperienceYears
        {
            get => _experienceYears;
            set { _experienceYears = value; OnPropertyChanged(); }
        }

        public bool HasRecommendations
        {
            get => _hasRecommendations;
            set { _hasRecommendations = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Створює копію анкети (для безпечного редагування без зміни оригіналу в колекції).
        /// </summary>
        public Candidate Clone()
        {
            return new Candidate
            {
                FullName = FullName,
                BirthDate = BirthDate,
                Education = Education,
                LanguageSkills = LanguageSkills,
                KnowsComputer = KnowsComputer,
                ExperienceYears = ExperienceYears,
                HasRecommendations = HasRecommendations
            };
        }

        /// <summary>
        /// Копіює значення з іншої анкети (для збереження відредагованої копії в оригінал).
        /// </summary>
        public void CopyFrom(Candidate other)
        {
            FullName = other.FullName;
            BirthDate = other.BirthDate;
            Education = other.Education;
            LanguageSkills = other.LanguageSkills;
            KnowsComputer = other.KnowsComputer;
            ExperienceYears = other.ExperienceYears;
            HasRecommendations = other.HasRecommendations;
        }
    }
}
