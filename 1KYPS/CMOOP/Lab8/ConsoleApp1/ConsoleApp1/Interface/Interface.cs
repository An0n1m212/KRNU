using System;
using System.Collections.Generic;
using System.Text;

namespace Nomer
{
        public interface IContainer<TElement> : IEnumerable<TElement>
        {
            int Count { get; }
            TElement this[int index] { get; set; }
            void Add(TElement element);
            void Delete(TElement element);
        }

        public interface IFileContainer<TElement> : IContainer<TElement>
        {
            void Save(string fileName);
            void Load(string fileName);
            bool IsDataSaved { get; }
        }
    }

