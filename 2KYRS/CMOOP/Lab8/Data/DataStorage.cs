using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab8
{
    public static class DataStorage
    {
        public static ObservableCollection<Candidate> Candidates { get; set; } = new ObservableCollection<Candidate>();
    }
}
