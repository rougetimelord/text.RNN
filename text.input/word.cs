using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.split
{
    class Word
    {
        public int count = 1;
        public Dictionary<string, float> nexts = new Dictionary<string, float>();
        public Word()
        {
        }
        public void nextWord(string next)
        {
            if (nexts.ContainsKey(next))
                nexts[next]++;
            else
                nexts.Add(next, 1);
        }
    }
}
