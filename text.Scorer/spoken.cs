using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Scorer
{
    class spoken
    {
        Dictionary<string, int> nextSpoken = new Dictionary<string, int>();
        int count = 0;
        public spoken()
        {
            count = 1;
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
