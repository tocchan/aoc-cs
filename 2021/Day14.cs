using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day14 : Day
    {
        private string InputFile = "2021/inputs/14.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private bool ApplyRules(ref string seq, Dictionary<string, char> rules)
        {
            StringBuilder sb = new StringBuilder(); 

            bool changed = false; 
            for (int i = 0; i < seq.Length - 1; ++i)
            {
                string substr = seq.Substring(i, 2); 
                sb.Append(seq[i]); // first is always added

                char insert;
                if (rules.TryGetValue(substr, out insert))
                {
                    sb.Append(insert); 
                    changed = true; 
                }
            }
            sb.Append(seq.Last()); 

            seq = sb.ToString(); 

            return changed; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            string sequence = lineInput[0]; 

            Dictionary<string, char> rules = new Dictionary<string, char>(); 
            for (int i = 2; i < lineInput.Count; ++i)
            {
                (string key, string value) = lineInput[i].Split("->", 2, StringSplitOptions.TrimEntries); 
                rules.Add( key, value[0] ); 
            }

            // Console.WriteLine( sequence ); 

            int maxDays = 10; 
            for (int i = 0; i < maxDays; ++i) 
            { 
                ApplyRules( ref sequence, rules ); 
                // Console.WriteLine(sequence); 
            }
            
            int leastValueCount = int.MaxValue; 
            int maxValueCount = -1; 
            for (char c = 'A'; c <= 'Z'; ++c)
            {
                int value = sequence.Sum( v => v == c ? 1 : 0 ); 
                if (value > 0)
                {
                    leastValueCount = Math.Min( value, leastValueCount ); 
                    maxValueCount = Math.Max( value, maxValueCount ); 
                }
            }

            return (maxValueCount - leastValueCount).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private void AddToken( Dictionary<string, Int64> pairs, string tok, Int64 count = 1 )
        {
            Int64 val; 
            if (pairs.TryGetValue(tok, out val))
            {
                pairs[tok] = val + count; 
            }
            else
            {
                pairs.Add( tok, count ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        private void RemoveToken( Dictionary<string, Int64> pairs, string tok, Int64 count )
        {
            Int64 val; 
            if (pairs.TryGetValue(tok, out val))
            {
                pairs[tok] = val - count; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private void ApplyRules2( ref Dictionary<string, Int64> pairs, Dictionary<string, char> rules )
        {
            char[] a = new char[2]; 
            char[] b = new char[2]; 

            Dictionary<string, Int64> nextGen = new Dictionary<string, Int64>(); 
            foreach ( (string key, Int64 count) in pairs ) 
            { 
                char insert; 
                if (rules.TryGetValue( key, out insert ))
                {
                    // Add the combination of new pairs
                    a[0] = key[0]; 
                    a[1] = insert; 
                    b[0] = insert; 
                    b[1] = key[1]; 
                    AddToken( nextGen, new string(a), count ); 
                    AddToken( nextGen, new string(b), count ); 
                }
                else
                {
                    // just add in old pair
                    AddToken( nextGen, key, count ); 
                }
            }

            pairs = nextGen; 
        }

        private Int64[] CountChars( Dictionary<string, Int64> pairs )
        {
            // everything counts twice
            Int64[] counts = new Int64[26]; 

            foreach ((string key, Int64 count) in pairs)
            {
                char a = key[0]; 
                char b = key[1]; 

                // I have non alpha markers on the start and end to prevent them counting twice; 
                int idxA = a - 'A'; 
                int idxB = b - 'A'; 
                if ((idxA >= 0) && (idxA < 26))
                {
                    counts[idxA] += count; 
                }

                if ((idxB >= 0) && (idxB < 26))
                {
                    counts[idxB] += count; 
                }
            }

            // everything counts twice
            for (int i = 0; i < 26; ++i)
            {
                counts[i] = counts[i] / 2; 
            }

            return counts; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            string sequence = lineInput[0]; 
            sequence = "_" + sequence + "_"; // apply a start/end token to help denote the "start" and "end" pairs (every thing will add twice)

            Dictionary<string, char> rules = new Dictionary<string, char>(); 
            for (int i = 2; i < lineInput.Count; ++i)
            {
                (string key, string value) = lineInput[i].Split("->", 2, StringSplitOptions.TrimEntries); 
                rules.Add( key, value[0] ); 
            }

            // setup the initial pair counts; 
            Dictionary<string, Int64> pairs = new Dictionary<string, Int64>(); 
            for (int i = 0; i < sequence.Length - 1; ++i)
            {
                string token = sequence.Substring(i, 2); 
                AddToken( pairs, token ); 
            }

            int numDays = 40; 
            for (int i = 0; i < numDays; ++i)
            {
                ApplyRules2( ref pairs, rules ); 
            }
            
            Int64[] charCounts = CountChars( pairs ); 
            Int64 maxValue = charCounts.Where( v => (v > 0) ).Max(); 
            Int64 minValue = charCounts.Where( v => (v > 0) ).Min(); 

            return (maxValue - minValue).ToString(); 
        }
    }
}

