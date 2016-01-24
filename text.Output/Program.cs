using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace text.Output
{
    class Program
    {
        static void Main(string[] args)
        {
            if(!Directory.Exists(@"\Users\" + Environment.UserName + @"\Documents\text.RNN"))
                Directory.CreateDirectory(@"\Users\" + Environment.UserName + @"\Documents\text.RNN");
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\Output.txt";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<word, string> words = new Dictionary<word, string>();
            Random r = new Random();
            string output = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(null);
                words.Add(new word(wordSplit[1]), wordSplit[0]);
            }
            int currentMax = 0, currentVal = 0;
            foreach (KeyValuePair<word,string> wo in words)
            {
                wo.Key.floorSet(currentVal);
                currentMax = currentVal + (int)(wo.Key.perc * 10000);
                currentVal = currentMax;
            }
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            if (File.Exists(p))
                File.Delete(p);
            float oldPerc = 0, currPerc = 0;
            Console.Write("0.0%");
            for (int i = 0; i <= 10000; i++)
            {
                currPerc = (float)Math.Round(((float)i/(float)10000)*100, 1);
                if(currPerc>oldPerc)
                {
                    Console.Clear();
                    Console.Write(currPerc + "%");
                    oldPerc = currPerc;
                }
                int rRes = r.Next(1, currentMax + 1);
                foreach (KeyValuePair<word, string> wo in words)
                {
                    if (wo.Key.floor <= rRes && rRes <= (wo.Key.floor + (int)(wo.Key.perc * 10000)))
                    {
                        output = wo.Value + " ";
                        File.AppendAllText(p, output);
                    }
                }
            }
        }
    }
}
