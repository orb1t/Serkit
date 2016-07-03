using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    class Word : List<Bit>
    {
        private int _WordSize;

        public int WordSize
        {
            get { return _WordSize; }
            set { _WordSize = value; }
        }

        public Word(int size)
        {
            WordSize = size;
        }

        public static Word operator +(Word destination, Word source)
        {
            return (Word)destination.Concat(source);
        }

        public new string ToString()
        {
            string s = "";
            for (int i = 0; i < Count; i++)
            {
                s = this.ElementAt(i).Value + s;
            }
            return s;
        }
    }
}
