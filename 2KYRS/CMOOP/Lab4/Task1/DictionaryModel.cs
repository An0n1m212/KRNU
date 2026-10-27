using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task1
{
    public class DictionaryModel
    {
        public string Title { get; set; } = string.Empty;
        public List<WordEntry> Words { get; set; } = new List<WordEntry>();

        public DictionaryModel() { }

        public DictionaryModel(string title)
        {
            Title = title.Trim();
        }
    }
}
