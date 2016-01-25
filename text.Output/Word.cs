using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Output
{
    class word
    {
        public float perc = 0;
        public int floor = 0;
        public Dictionary<string, next> nexts = new Dictionary<string, next>();
        public int nextsCurrentVal = 0;
        public word(string wordInfo)
        {
            float.TryParse(wordInfo, out perc);
        }
        public void floorSet(int fl)
        {
            floor = fl;
        }
        public void setNext(string[] a)
        {
            string next = a[0].Replace("-", "");
            float nextPerc;
            float.TryParse(a[1], out nextPerc);
            if(!nexts.ContainsKey(next))
                nexts.Add(next, new next(perc));
            else
            {
                nexts[next].perc = (perc + nexts[next].perc) / 2;
            }
        }
        public void nextFloorSet()
        {
            foreach (KeyValuePair<string, next> next in nexts)
            {
                next.Value.floorSetNext(nextsCurrentVal);
                nextsCurrentVal += (int)(next.Value.perc * 10000);
            }
        }
    }
}
