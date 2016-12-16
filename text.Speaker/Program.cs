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
        static void Main(string[] args)
        {
            //Find or create output file/directory then set post
            if(!Directory.Exists(@"\Users\" + Environment.UserName + @"\Documents\text.RNN"))
                Directory.CreateDirectory(@"\Users\" + Environment.UserName + @"\Documents\text.RNN");
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            //Read brain and split it
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, word> words = new Dictionary<string, word>();
            //Setup random function
            Random r = new Random();
            //Setup output variables
            string output = "", pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                //Split words from their scores
                var wordSplit = strArr[i].Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (wordSplit[0].Contains("#"))
                {
                    //If it's a word inside of a tree add it to the tree
                    words[pre].setNext(wordSplit);
                }
                else if(!words.ContainsKey(wordSplit[0]))
                {
                    //If it isn't in the dictionary add it
                    words.Add(wordSplit[0], new word(wordSplit[1]));
                    pre = wordSplit[0];
                }
                else
                {
                    //Otherwise average the points of the double entries
                    int tempP;
                    int.TryParse(wordSplit[1], out tempP);
                    words[wordSplit[0]].points = (words[wordSplit[0]].points + tempP) / 2;
                }
            }
            //Set floors for all of the words
            int currentVal = 0;
            foreach (KeyValuePair<string, word> wo in words)
            {
                wo.Value.floorSet(currentVal);
                currentVal += (int)(wo.Value.points * 10000);
                //Set floors for all of the next words in the tree
                wo.Value.nextFloorSet();
            }
            //Start picking words
            int rResult = r.Next(currentVal + 1);
            pre = "";
            foreach (KeyValuePair<string, word> wo in words)
            {
                //Pick first word super lazily
                if (rResult >= wo.Value.floor && rResult < wo.Value.floor + (wo.Value.points * 10000))
                {
                    output += wo.Key.First().ToString().ToUpper() + wo.Key.Substring(1) + " ";
                    pre = wo.Key;
                }
            }
            //Set path for outputting
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            if (File.Exists(p))
                File.Delete(p);
            //Start picking words
            bool capitalize = false, per = false;
            int open = 0;
            while(output.Length <= 140)
            {
                if (words[pre].nexts.Count > 0)
                {
                    //If word has more than 0 next words (most words, but not all) 
                    //Pick a number then find which word is next
                    rResult = r.Next(words[pre].nextsCurrentVal + 1);
                    foreach (KeyValuePair<string, next> nxt in words[pre].nexts)
                    {
                        if (nxt.Value.floorVar <= rResult && rResult < nxt.Value.floorVar + nxt.Value.points * 10000)
                        {
                            //Handle capitalization and parenthesis values
                            if (nxt.Key == ")" && open > 0)
                                open--;
                            if (nxt.Key == "(")
                                open++;
                            if (per)
                            {
                                capitalize = true;
                                per = false;
                            }
                            //Remove proceeding space
                            if (nxt.Key == "." || nxt.Key == "," || nxt.Key == ")" || nxt.Key == "'" || nxt.Key == "-")
                                output = output.Remove(output.Length - 1);
                            if (nxt.Key == ".")
                            {
                                per = true;
                                if (open > 0)
                                    open--;
                            }
                            //Add to output and move word in
                            output += Format(nxt.Key, capitalize, open);
                            pre = nxt.Key;
                            capitalize = false;
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
                            //Handle capitalization and parenthesis values
                            if (wo.Key == ")" && open > 0)
                                open--;
                            if (wo.Key == "(")
                                open++;
                            if (per)
                            {
                                capitalize = true;
                                per = false;
                            }
                            //Remove proceeding space
                            if (wo.Key == "." || wo.Key == "," || wo.Key == ")" || wo.Key == "'" || wo.Key == "-")
                                output = output.Remove(output.Length - 1);
                            if (wo.Key == ".")
                            {
                                per = true;
                                if (open > 0)
                                    open--;
                            }
                            //Add to output and move word in
                            output += Format(wo.Key, capitalize, open);
                            pre = wo.Key;
                            capitalize = false;
                            break;
                        }
                    }
                }
            }
            //Write text and tell user we're done
            File.AppendAllText(p, output);
            Console.Clear();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
        static string Format(string word, bool capitalize, int open)
        {
            //Set up values
            string output = "";
            //Filter out symbols based on if they need a space before or after
            if (word == "(")
            {
                return word;
            }
            else if (word == "." || word == "," || word == ")" || word == "'" || word == "-")
            {
                //Handle punctuation stuff
                if (word == ".")
                {
                    if (open > 0)
                    {
                        return ".) ";
                    }
                }
                else if (word == "'" || word == "-")
                {
                    return word;
                }
                else if (word == ")" && open <= 0)
                {
                    return "";
                }
            }
            if (capitalize == true)
            {
                //Capitalize first letter and write word then finish
                output += word.First().ToString().ToUpper() + word.Substring(1) + " ";
                return output;
            }
            //Return stuff
            return word + " ";
        }
    }
}
