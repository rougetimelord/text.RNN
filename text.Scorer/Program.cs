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
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\Output.txt";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, word> spokenWords = new Dictionary<string, word>();
            Random r = new Random();
            string pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                if(pre != "" && !spokenWords.ContainsKey(pre))
                    spokenWords.Add(pre, new word(strArr[i].Split()[0]));
                else
                    spokenWords[pre].
                pre = strArr[i].Split()[0];
            }
            pre = "";
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            inStr = File.ReadAllText(p);
            strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, Learned> learnedWords = new Dictionary<string,Learned>();
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(wordSplit[0].Contains("-"))
                {
                    learnedWords[pre].addNext(strArr);
                }
                else if (!learnedWords.ContainsKey(wordSplit[0]))
                {
                    learnedWords.Add(wordSplit[0], new Learned(wordSplit[1]));
                    pre = wordSplit[0];
                }
                else
                {
                    int tempP;
                    int.TryParse(wordSplit[1], out tempP);
                    learnedWords[wordSplit[0]].points = (learnedWords[wordSplit[0]].points + tempP) / 2;
                }
            }
            Console.WriteLine("Give a score to output -10 - 10");
            int sint = 0;
            while (sint == 0)
            {
                string sc = Console.ReadLine();
                int.TryParse(sc, out sint);            
            }
        }
    }
}
