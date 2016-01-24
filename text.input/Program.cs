using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace text.split
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Operations
            var inStr = "";
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\Input.txt";
            if (File.Exists(p))
                inStr = File.ReadAllText(p);
            else
                inStr = Console.ReadLine();
            inStr.Replace(Environment.NewLine, " ");
            var strB = new StringBuilder();
            foreach (char c in inStr){if (!char.IsPunctuation(c) && (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))) { strB.Append(c);}}
            List <string> strList= new List<string>();
            Dictionary<string, Word> words = new Dictionary<string, Word>();
            strList = strB.ToString().ToLower().Split().Where(c => c!="").ToList();
            int currPerc, oldPerc = -1, i = 0;
            foreach(string str in strList)
            {
                if (words.ContainsKey(str))
                    words[str].count++;
                else
                    words.Add(str, new Word());
                currPerc = (int)Math.Round(((float)i / (float)strList.Count)*100, 1);
                if (currPerc > oldPerc)
                {
                    Console.Clear();
                    Console.Write(currPerc+"%");
                    oldPerc = currPerc;
                }
                i++;
            }
            #endregion
            #region Write data
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\Output.txt";
            if(File.Exists(p))
                File.Delete(p);
            Console.Clear();
            currPerc = 0; oldPerc = -1; i = 0;
            foreach (KeyValuePair<string, Word> wo in words.OrderByDescending(o => o.Value.count).ThenBy(s => s.Key).ToList())
            {
                float percent = (float)Math.Round(((float)wo.Value.count / (float)strList.Count) * 100,4);
                string txt = String.Format(@"{0} {1}", wo.Key, percent) + Environment.NewLine;
                File.AppendAllText(p, txt);
                currPerc = (int)Math.Round(((float)i / (float)words.Count) * 100, 1);
                if (currPerc > oldPerc)
                {
                    Console.Clear();
                    Console.Write(currPerc + "%");
                    oldPerc = currPerc;
                }
                i++;
            }
            #endregion
        }
    }
}
