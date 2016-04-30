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
            var doScoring = true;
            if (Console.ReadLine().ToLower() == "merge")
                doScoring = false;
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
                else
                {
                    if (!brain.ContainsKey(wordSplit[0]))
                        brain.Add(wordSplit[0], new brain(float.Parse(wordSplit[1])));
                    if (brain.ContainsKey(wordSplit[0]))
                    {
                        float tempP;
                        float.TryParse(wordSplit[1], out tempP);
                        brain[wordSplit[0]].points = (brain[wordSplit[0]].points + tempP) / 2;                        
                    }
                    pre = wordSplit[0];
                }
            }
            if (doScoring)
            {
                pre = "";
                Dictionary<string, spoken> spokenWords = new Dictionary<string, spoken>();
                p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
                inStr = File.ReadAllText(p);
                strArr = inStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, brain> spoken = new Dictionary<string, brain>();
                for (int i = 0; i < strArr.Count(); i++)
                {
                    if (!spokenWords.ContainsKey(pre) && pre != "")
                    {
                        spokenWords.Add(pre, new spoken());
                    }
                    else if (pre != "")
                    {
                        spokenWords[pre].addNext(strArr[i]);
                    }
                    pre = strArr[i];
                }
                var go = true;
                Boolean reaction = true;
                while (go)
                {
                    Console.WriteLine("Was the output readable? y/n");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        reaction = true;
                        go = false;
                    }
                    else if (Console.ReadLine().ToLower() == "n")
                    {
                        reaction = false;
                        go = false;
                    }
                }
                float score = 0;
                while (!go)
                {
                    Console.WriteLine("On a scale of 1-10, how readable/unreadable was the output?");
                    var a = Console.ReadLine();
                    float.TryParse(a, out score);
                    if (score == 0 || Math.Abs(score) > 10)
                        Console.WriteLine("Input was not valid");
                    else
                    {
                        score = Math.Abs(score);
                        go = true;
                    }
                }
                Console.Clear();
                if (reaction == false)
                    score = -1 * score;
                foreach (KeyValuePair<string, spoken> sp in spokenWords)
                {
                    pre = sp.Key;
                    if (brain.ContainsKey(pre))
                    {
                        foreach (KeyValuePair<string, int> ne in sp.Value.nextSpoken)
                        {
                            var next = ne.Key;
                            if (brain[pre].nexts.ContainsKey(next))
                            {
                                if (brain[pre].nexts[next] + score * ne.Value > 0)
                                    brain[pre].nexts[next] += score * ne.Value;
                                else
                                    brain[pre].nexts[next] = .00001F;
                            }
                            else
                            {
                                Console.WriteLine("Word {0} is not in the tree for {1}, adding it!", next, pre);
                                if (score * ne.Value > 0)
                                    brain[pre].nexts.Add(next, score * ne.Value);
                                else
                                    Console.WriteLine("Sike that would give {0} a negative score", next);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Word {0} is not in the brain, not adding it", pre);
                        Console.Read();
                    }
                }
                Console.WriteLine("Scores updated, writing");
            }
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            File.WriteAllText(p,"");
            foreach (KeyValuePair<string, brain> br in brain.OrderByDescending(o => o.Value.points).ThenBy(s => s.Key).ToList())
            {
                //Calculate percentages of word
                string txt = String.Format(@"{0} {1}", br.Key, br.Value.points) + Environment.NewLine;
                File.AppendAllText(p, txt);
                //Then do the same for nexts
                foreach (KeyValuePair<string, float> next in br.Value.nexts)
                {
                    txt = "    " + String.Format(@"-{0} {1}", next.Key, next.Value + Environment.NewLine);
                    File.AppendAllText(p, txt);
                }
            }
        }
    }
}
