using Nomer3.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer3.Class.BCipher
{
    public class BCipher : ICipher, IComparable<BCipher>
    {
        public string Data { get; private set; }

        public BCipher(string text)
        {
            Data = Encode(text);
        }

        public string Encode(string text)
        {
            char[] buffer = text.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                if (char.IsLetter(buffer[i]))
                {
                    char offset = char.IsUpper(buffer[i]) ? 'A' : 'a';
                    buffer[i] = (char)(offset + (25 - (buffer[i] - offset)));
                }
            }
            return new string(buffer);
        }

        public string Decode(string text) => Encode(text); // Атбаш симетричний

        public int CompareTo(BCipher? other) => string.Compare(this.Data, other?.Data);

        public override string ToString() => $"BCipher(Encoded: {Data})";
    }
}
