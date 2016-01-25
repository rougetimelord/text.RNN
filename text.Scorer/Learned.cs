using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Scorer
{
    class Learned
    {
        public float points = 0;
        public Dictionary<string, float> nexts = new Dictionary<string, float>();
        public Learned(string s)
        {
            points = float.Parse(s);
        }
        public void addNext(string[] a)
        {
            nexts.Add(a[0], float.Parse(a[1]));
        }
    }
}
