using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace text.Scorer
{
    class Program
    {
        Dictionary<string, brain> brain = new Dictionary<string, brain>();
        Dictionary<string, spoken> spokenWords = new Dictionary<string, spoken>();
        static void Main(string[] args)
        {
            Program thisClass = new Program();
            /*This could be much better
            A menu??*/
            Console.WriteLine(@"Write anything but ""Merge"" to do score updates");
            var doScoring = true;
            if (Console.ReadLine().ToLower() == "merge")
                doScoring = false;
            //Read brain
            Thread brainThread = new Thread(thisClass.digestBrain);
            brainThread.Start();
            Thread spokenThread = new Thread(thisClass.digestSpoken);
            if (doScoring)
            {
                spokenThread.Start();
                while(spokenThread.IsAlive || brainThread.IsAlive)
                {
                    Thread.Sleep(10);
                }
                var go = true;
                Boolean reaction = true;
                while (go)
                {
                    //Check if readable
                    Console.WriteLine("Was the output readable? y/n");
                    var resp = Console.ReadLine().ToLower();
                    if (resp == "y")
                    {
                        reaction = true;
                        go = false;
                    }
                    else if (resp == "n")
                    {
                        reaction = false;
                        go = false;
                    }
                }
                float score = 0;
                while (!go)
                {
                    //Check score
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
                //If bad subtract points
                if (reaction == false)
                    score = -1 * score;
                foreach (KeyValuePair<string, spoken> sp in thisClass.spokenWords)
                {
                    //Change scores
                    var pre = sp.Key;
                    if (thisClass.brain.ContainsKey(pre))
                    {
                        foreach (KeyValuePair<string, int> ne in sp.Value.nextSpoken)
                        {
                            var next = ne.Key;
                            if (thisClass.brain[pre].nexts.ContainsKey(next))
                            {
                                //If new score is more than 0 change it
                                if (thisClass.brain[pre].nexts[next] + score * ne.Value > 0)
                                    thisClass.brain[pre].nexts[next] += score * ne.Value;
                                //Else throw it away
                                else
                                    thisClass.brain[pre].nexts.Remove(next);
                            }
                            else
                            {
                                //If errors happen warn the user and try to add it
                                Console.WriteLine("Word {0} is not in the tree for {1}, adding it!", next, pre);
                                if (score * ne.Value > 0)
                                    thisClass.brain[pre].nexts.Add(next, score * ne.Value);
                                //If score would be >= 0 don't bother
                                else
                                    Console.WriteLine("Sike that would give {0} a negative score", next);
                            }
                        }
                    }
                    else
                    {
                        //If root word isn't in brain just ignore
                        Console.WriteLine("Word {0} is not in the brain, not adding it", pre);
                        Console.Read();
                    }
                }
                Console.WriteLine("Scores updated, writing");
            }
            else
            {
                while (brainThread.IsAlive)
                    Thread.Sleep(10);
                //If not scoring tell the user merge has completed
                Console.WriteLine("Merged, writing");
            }
            //Delete and rewrite brain
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            File.WriteAllText(p,"");
            //Write by point value then alphabetically
            foreach (KeyValuePair<string, brain> br in thisClass.brain.OrderByDescending(o => o.Value.points).ThenBy(s => s.Key).ToList())
            {
                //Write new scores for root nodes
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
        void digestBrain()
        {
            //Set path for brain
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                /*Digests brain and merges entries
                Could probably be done much better*/
                var wordSplit = strArr[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (wordSplit[0].Contains("-"))
                {
                    //Add nexts
                    wordSplit[0] = wordSplit[0].Replace("-", "");
                    brain[pre].addNext(wordSplit);
                }
                else
                {
                    //Else add words
                    if (!brain.ContainsKey(wordSplit[0]))
                        brain.Add(wordSplit[0], new brain(float.Parse(wordSplit[1])));
                    if (brain.ContainsKey(wordSplit[0]))
                    {
                        //Else merge entries
                        float tempP;
                        float.TryParse(wordSplit[1], out tempP);
                        brain[wordSplit[0]].points = (brain[wordSplit[0]].points + tempP) / 2;
                    }
                    //Increment word
                    pre = wordSplit[0];
                }
            }
        }
        void digestSpoken()
        {
            //If doing scoring digest spoken words
            var pre = "";
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //Digesting by creating a mirror of the brain deictionary
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
        } 
    }
}
