using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer3.Interface
{
    public interface ICipher
    {
        string Encode(string text);
        string Decode(string text);
    }
}
