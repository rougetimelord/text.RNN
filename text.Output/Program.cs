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
            List<word> words = new List<word>();
            Random r = new Random();
            string output = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(null);
                words.Add(new word(wordSplit));
            }
            int currentMax = 0, currentVal = 0;
            foreach (word wo in words)
            {
                wo.floorSet(currentVal);
                currentMax = currentVal + (int)(wo.perc * 10000);
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
                foreach (word wo in words)
                {
                    if (wo.floor <= rRes && rRes <= (wo.floor + (int)(wo.perc * 10000)))
                    {
                        output = wo.wordStr + " ";
                        File.AppendAllText(p, output);
                    }
                }
            }
        }
    }
}
