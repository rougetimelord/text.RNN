using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace text.Speaker
{
    class Program
    {
        static Dictionary<string, word> words = new Dictionary<string, word>();
        static void Main(string[] args)
        {
            //Find or create output file/directory then set post
            if(!Directory.Exists(@"\Users\" + Environment.UserName + @"\Documents\text.RNN"))
                Directory.CreateDirectory(@"\Users\" + Environment.UserName + @"\Documents\text.RNN");
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            //Read brain and split it
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //Setup random function
            Random r = new Random();
            string output = "", pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (wordSplit[0].Contains("#"))
                {
                    words[pre].setNext(wordSplit);
                }
                else if(!words.ContainsKey(wordSplit[0]))
                {
                    words.Add(wordSplit[0], new word(wordSplit[1]));
                    pre = wordSplit[0];
                }
                else
                {
                    int tempP;
                    int.TryParse(wordSplit[1], out tempP);
                    words[wordSplit[0]].points = (words[wordSplit[0]].points + tempP) / 2;
                }
            }
            int currentVal = 0;
            foreach (KeyValuePair<string, word> wo in words)
            {
                wo.Value.floorSet(currentVal);
                currentVal += (int)(wo.Value.points * 10000);
                wo.Value.nextFloorSet();
            }
            int rResult = r.Next(currentVal + 1);
            pre = "";
            foreach (KeyValuePair<string, word> wo in words)
            {
                if (rResult >= wo.Value.floor && rResult < wo.Value.floor + (wo.Value.points * 10000))
                {
                    output += wo.Key.First().ToString().ToUpper() + wo.Key.Substring(1) + " ";
                    pre = wo.Key;
                }
            }
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            if (File.Exists(p))
                File.Delete(p);
            bool capitalize = false;
            int open = 0;
            while(output.Length <= 140)
            {
                if (words[pre].nexts.Count > 0)
                {
                    rResult = r.Next(words[pre].nextsCurrentVal + 1);
                    foreach (KeyValuePair<string, next> nxt in words[pre].nexts)
                    {
                        if (nxt.Value.floorVar <= rResult && rResult < nxt.Value.floorVar + nxt.Value.points * 10000)
                        {
                            if (nxt.Key == ")" && open > 0)
                                open--;
                            if (nxt.Key == "(")
                                open++;
                            if (nxt.Key == ".")
                            {
                                capitalize = true;
                                if (open > 0)
                                    open--;
                            }
                            output += Format(nxt.Key, capitalize, open);
                            pre = nxt.Key;
                            break;
                        }
                    }
                }
                else 
                {
                    rResult = r.Next(currentVal + 1);
                    foreach (KeyValuePair<string, word> wo in words)
                    {
                        if (rResult >= wo.Value.floor && rResult < wo.Value.floor + (wo.Value.points * 10000))
                        {
                            if (wo.Key == ")" && open > 0)
                                open--;
                            if (wo.Key == "(")
                                open++;
                            if (wo.Key == ".")
                            {
                                capitalize = true;
                                if (open > 0)
                                    open--;
                            }
                            output += Format(wo.Key, capitalize, open);
                            pre = wo.Key;
                            break;
                        }
                    }
                }
            }
            File.AppendAllText(p, output);
            Console.Clear();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
        static string Format(string word, bool capitalize, int open)
        {
            bool done = false;
            string output = "";
            if (capitalize == true)
            {
                output += word.First().ToString().ToUpper() + word.Substring(1) + " ";
                capitalize = false;
                done = true;
            }
            if (word == "." || word == "," || word == ")" || word == "'" || word == "-")
            {
                output = output.Remove(output.Length - 1);
                if (word == ".")
                {
                    if (open > 0)
                    {
                        output += ".) ";
                        done = true;
                    }
                }
                if (word == "'" || word == "-")
                {
                    output += word;
                    done = true;
                }
                if (word == ")" && open <= 0)
                {
                    done = true;
                }
            }
            if (!done && word != "(")
                output += word + " ";
            if (!done && word == "(")
            {
                output += word;
            }
            return output;
        }
    }
}
