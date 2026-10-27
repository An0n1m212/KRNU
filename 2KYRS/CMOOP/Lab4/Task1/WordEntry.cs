using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task1
{
    public class WordEntry
    {
        public string Word { get; set; } = string.Empty;
        public List<string> Translations { get; set; } = new List<string>();

        public WordEntry() { }

        public WordEntry(string word, IEnumerable<string> translations)
        {
            Word = word.Trim();
            Translations = translations.Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
        }
    }
}
