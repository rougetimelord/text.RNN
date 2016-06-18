using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace text.Learner
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
            //Get text from file into string and remove newlines
            inStr.Replace(Environment.NewLine, " ");
            var strB = new StringBuilder();
            //Remove punctuation
            char[] allowedPunc = { '.', '\'', ',', '-'};
            foreach (char c in inStr){if (allowedPunc.Contains(c) || (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))) {if (c != '.' && c != ',') strB.Append(c); else strB.Append(" " +  c  + " "); }}
            //Make a list for all of the words
            List <string> strList= new List<string>();
            //Make a dictionary for the words and word class instances
            Dictionary<string, Word> words = new Dictionary<string, Word>();
            //Standardize entries
            strList = strB.ToString().ToLower().Split().Where(c => c!="").ToList();
            string pre = "";
            int currPerc, oldPerc = -1, i = 0;
            foreach(string str in strList)
            {
                //If no previous word don't add nothing, duh!
                if (pre != "")
                {
                    if (words.ContainsKey(pre))
                    {
                        //If already in dictionary add to the sighting count
                        words[pre].count++;
                    }
                    else
                    {
                        //Else add to the dictionary
                        words.Add(pre, new Word());
                    }
                    //Add the next word
                    words[pre].nextWord(str);
                }
                //Set "next" word to previous word
                pre = str;
                //Calculate percentage of completion
                currPerc = (int)Math.Round(((float)i / (float)strList.Count)*100, 1);
                if (currPerc > oldPerc)
                {
                    Console.Clear();
                    Console.Write(currPerc+"% Crunched");
                    oldPerc = currPerc;
                }
                i++;
            }
            //If last word isn't in the dictionary add it
            if(!words.ContainsKey(pre))
                words.Add(pre, new Word());
            #endregion
            #region Write data
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            if (!Directory.Exists(@"\Users\" + Environment.UserName + @"\Documents\text.RNN\"))
                Directory.CreateDirectory(@"\Users\" + Environment.UserName + @"\Documents\text.RNN\");
            Console.Clear();
            currPerc = 0; oldPerc = -1; i = 0;
            //For each entry in the words dictionary do some stuff
            foreach (KeyValuePair<string, Word> wo in words.OrderByDescending(o => o.Value.count).ThenBy(s => s.Key).ToList())
            {
                //Calculate percentages of word
                float percent = (float)Math.Round(((float)wo.Value.count / (float)strList.Count) * 100,4);
                string txt = String.Format(@"{0} {1}", wo.Key, percent) + Environment.NewLine;
                File.AppendAllText(p, txt);
                //Then do the same for nexts
                foreach(KeyValuePair<string,float> next in wo.Value.nexts)
                {
                    txt = "    " + String.Format(@"#{0} {1}", next.Key, (float)Math.Round((next.Value / wo.Value.nexts.Count) * 100, 4))+Environment.NewLine;
                    File.AppendAllText(p, txt);
                }
                //Completion percentage
                currPerc = (int)Math.Round(((float)i / (float)words.Count) * 100, 1);
                if (currPerc > oldPerc)
                {
                    Console.Clear();
                    Console.Write(currPerc + "% Written");
                    oldPerc = currPerc;
                }
                i++;
            }
            #endregion
            Environment.Exit(0);
        }
    }
}
