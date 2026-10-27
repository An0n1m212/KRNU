using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3.Task3
{
    public class Song
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Composer { get; set; }
        public int Year { get; set; }
        public string Lyrics { get; set; }
        public List<string> Performers { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"Назва: \"{Title}\" | Автор слів: {Author} | Композитор: {Composer} | Рік: {Year}\n" +
                   $"Виконавці: {string.Join(", ", Performers)}\nТекст:\n{Lyrics}\n" +
                   new string('-', 50);
        }
    }
}
