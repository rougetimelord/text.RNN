using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace text.Scorer
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, brain> brain = new Dictionary<string, brain>();
            string pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (wordSplit[0].Contains("-"))
                {
                    wordSplit[0] = wordSplit[0].Replace("-", "");
                    brain[pre].addNext(wordSplit);
                }
                else if (!brain.ContainsKey(wordSplit[0]))
                {
                    brain.Add(wordSplit[0], new brain(float.Parse(wordSplit[1])));
                    pre = wordSplit[0];
                }
                else
                {
                    int tempP;
                    int.TryParse(wordSplit[1], out tempP);
                    brain[wordSplit[0]].points = (brain[wordSplit[0]].points + tempP) / 2;
                }
            }
            pre = "";
            Dictionary<string, spoken> spokenWords = new Dictionary<string, spoken>();
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output";

        }
    }
}
