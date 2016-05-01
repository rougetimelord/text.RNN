﻿using System;
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
            if(!Directory.Exists(@"\Users\" + Environment.UserName + @"\Documents\text.RNN"))
                Directory.CreateDirectory(@"\Users\" + Environment.UserName + @"\Documents\text.RNN");
            var p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\brain.rouge";
            var inStr = File.ReadAllText(p);
            var strArr = inStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, word> words = new Dictionary<string, word>();
            Random r = new Random();
            string output = "", pre = "";
            for (int i = 0; i < strArr.Count(); i++)
            {
                var wordSplit = strArr[i].Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (wordSplit[0].Contains("-"))
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
                    output = wo.Key + " ";
                    pre = wo.Key;
                }
            }
            p = @"\Users\" + Environment.UserName + @"\Documents\text.RNN\text.output.txt";
            if (File.Exists(p))
                File.Delete(p);
            File.AppendAllText(p, output);
            for(int i = 0; i <= 28; i++)
            {
                if (words[pre].nexts.Count > 0)
                {
                    rResult = r.Next(words[pre].nextsCurrentVal + 1);
                    foreach (KeyValuePair<string, next> nxt in words[pre].nexts)
                    {
                        if (nxt.Value.floorVar <= rResult && rResult < nxt.Value.floorVar + nxt.Value.points * 10000)
                        {
                            output = nxt.Key + " ";
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
                            output = wo.Key + " ";
                            pre = wo.Key;
                        }
                    }
                }
                File.AppendAllText(p, output);
            }
            Console.Clear();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}