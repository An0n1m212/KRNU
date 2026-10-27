using Lab4.Task1;

namespace Lab4.Task2
{
    internal static class DictionaryModelExtensions
    {
        public static WordEntry? FindWord(this DictionaryModel dictionary, string word)
        {
            return dictionary.Words.FirstOrDefault(
                entry => entry.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
        }
    }
}
