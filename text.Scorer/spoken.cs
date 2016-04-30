using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Scorer
{
    class spoken
    {
        public Dictionary<string, int> nextSpoken = new Dictionary<string, int>();
        public spoken()
        {
        }
        public void addNext(string a)
        {
            if (!nextSpoken.ContainsKey(a))
                nextSpoken.Add(a, 1);
            else
                nextSpoken[a]++;
        }
    }
}
