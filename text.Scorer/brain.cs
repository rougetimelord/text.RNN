using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Scorer
{
    class brain
    {
        public float points;
        public Dictionary<string, float> nexts = new Dictionary<string, float>();
        public brain(float a)
        {
            //Put in points
            points = a;
        }
        public void addNext(string[] b)
        {
            if (!nexts.ContainsKey(b[0]))
                nexts.Add(b[0], float.Parse(b[1]));
            else
                nexts[b[0]] = (nexts[b[0]] + float.Parse(b[1])) / 2;
        }
    }
}
