﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text.Output
{
    class word
    {
        public string wordStr = "";
        public float perc = 0;
        public int floor = 0;
        public word(string[] wordInfo)
        {
            wordStr = wordInfo[0];
            float.TryParse(wordInfo[1], out perc);
        }
        public void floorSet(int fl)
        {
            floor = fl;
        }
    }
}
