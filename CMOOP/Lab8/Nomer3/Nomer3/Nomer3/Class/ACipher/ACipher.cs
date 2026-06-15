using Nomer3.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer3.Class.ACipher
{
    public class ACipher : ICipher, IComparable<ACipher>
    {
        public string Data { get; private set; }
        private const int Shift = 1;

        public ACipher(string text)
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
                    buffer[i] = (char)((((buffer[i] + Shift) - offset) % 26) + offset);
                }
            }
            return new string(buffer);
        }

        public string Decode(string text)
        {
            char[] buffer = text.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                if (char.IsLetter(buffer[i]))
                {
                    char offset = char.IsUpper(buffer[i]) ? 'A' : 'a';
                    buffer[i] = (char)((((buffer[i] - Shift) - offset + 26) % 26) + offset);
                }
            }
            return new string(buffer);
        }

        public int CompareTo(ACipher? other) => string.Compare(this.Data, other?.Data);

        public override string ToString() => $"ACipher(Encoded: {Data})";
    }
}
