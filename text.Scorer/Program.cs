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
        public static Boolean _v = false;
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Program thisClass = new Program();
            /*This could be much better
            A menu??
            WHOA*/
            var menu = true;
            var doScoring = true;
            bool error = false;
            drawMenu();
            while (menu)
            {
                string choice = Console.ReadLine().ToLower();
                switch(choice)
                {
                    case ("1"):
                        menu = false;
                        break;
                    case ("s"):
                        menu = false;
                        break;
                    case ("2"):
                        menu = false;
                        doScoring = false;
                        break;
                    case ("m"):
                        menu = false;
                        doScoring = false;
                        break;
                    case ("3"):
                        _v = OptionsMenu();
                        drawMenu();
                        break;
                    case("o"):
                        _v = OptionsMenu();
                        drawMenu();
                        break;
                    default:
                        break;
                }
            }
            Console.Clear();
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
                Console.BackgroundColor = ConsoleColor.Black;
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
                                error = true;
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
                        error = true;
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
                Console.BackgroundColor = ConsoleColor.Black;
                //If not scoring tell the user merge has completed
                Console.WriteLine("Merged, writing");
            }
            //Set path
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            //Write organized by point value then alphabetically
            string str = "";
            var i = 0;
            foreach (KeyValuePair<string, brain> br in thisClass.brain.OrderByDescending(o => o.Value.points).ThenBy(s => s.Key).ToList())
            {
                //Write new scores for root nodes
                string txt = String.Format(@"{0} {1}", br.Key, br.Value.points) + Environment.NewLine;
                str += txt;
                //Then do the same for nexts
                var ni = 0;
                if (_v)
                {
                    i++;
                    Console.WriteLine("Word {0} of {1} '{2}' is being processed", i, thisClass.brain.Count, br.Key);
                }
                foreach (KeyValuePair<string, float> next in br.Value.nexts)
                {
                    txt = "    " + String.Format(@"#{0} {1}", next.Key, next.Value) + Environment.NewLine;
                    str += txt;
                    if (_v)
                    {
                        ni++;
                        Console.WriteLine("    Branch {0} of {1} '{2}' in tree '{3}' is being processed", ni, br.Value.nexts.Count, next.Key,br.Key);
                    }
                }
            }
            //Delete and rewrite brain
            File.WriteAllText(p, "");
            File.AppendAllText(p, str);
            if (error || _v)
                Console.Read();
            Main();
        }
        void digestBrain()
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                if (_v)
                    Console.WriteLine("Digesting brain");
                Console.ForegroundColor = ConsoleColor.White;            
            }
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
                if (wordSplit[0].Contains("#"))
                {
                    //Add nexts
                    wordSplit[0] = wordSplit[0].Replace("#", "");
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
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (_v)
                    Console.WriteLine("Digesting spoken");
                Console.ForegroundColor = ConsoleColor.White;
            }
            //If doing scoring digest spoken words
            var pre = "";
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            var inStr = File.ReadAllText(p);
            var strB = new StringBuilder();
            foreach (char c in inStr) { if (c == '.' || c == ',' || c == '(' || c == ')') { strB.Append(" " + c + " "); } else { strB.Append(c); } }
            inStr = strB.ToString().ToLower();
            var strArr = inStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //Digesting by creating a mirror of the brain dictionary
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
        static bool OptionsMenu()
        {
            var a = false;
            Console.Clear();
            Console.WriteLine("Text.Scorer");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("1. S");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ilent");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("2. V");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("erbiose");
            var chk = Console.ReadLine().ToLower();
            if (chk == "2" || chk == "v")
                a = true;
            return a;
        }
        static void drawMenu()
        {
            Console.Clear();
            Console.WriteLine("Text.Scorer");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("1. S");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("core");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("2. M");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("erge");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("3. O");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ptions");
        }
    }
}
