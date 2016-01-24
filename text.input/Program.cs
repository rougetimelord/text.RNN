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
            var found = false;
            List < string > strList= new List<string>();
            List<Word> words = new List<Word>();
            strList = strB.ToString().ToLower().Split().Where(c => c!="").ToList();
            for (int i = 0; i < strList.Count; i++)
            {
                var strCheck = strList[i];
                found = false;
                foreach (Word wordInst in words)
                {
                    if (wordInst.word == strCheck && !found)
                    {
                        found = true;
                        wordInst.count++;
                        break;
                    }
                }
                if (!found || words.Count == 0)
                {
                    words.Add(new Word(strCheck));
                }
            }
            #endregion
            #region Write data
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\Output.txt";
            if(File.Exists(p))
                File.Delete(p);
            Console.Clear();
            foreach (Word wo in words.OrderByDescending(o => o.count).ThenBy(s => s.word).ToList())
            {
                float percent = (float)Math.Round(((float)wo.count / (float)strList.Count) * 100,4);
                string txt = String.Format(@"{0} {1}", wo.word, percent) + Environment.NewLine;
                File.AppendAllText(p, txt);
            }
            #endregion
        }
    }
}
