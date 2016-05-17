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
            //Do I need a constructor? Probably
        }
        public void addNext(string a)
        {
            //Add new branches
            if (!nextSpoken.ContainsKey(a))
                nextSpoken.Add(a, 1);
            //Or increase value of branch
            else
                nextSpoken[a]++;
        }
    }
}
