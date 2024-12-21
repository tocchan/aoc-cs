using AoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day21 : Day
   {
      private string InputFile = "2024/inputs/21d.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
  

      private List<string> Codes = new(); 
      private Dictionary<char, ivec2> Numpad = new(); 
      private Dictionary<char, ivec2> Keypad = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Codes = Util.ReadFileToLines(InputFile);

         Numpad['7'] = new ivec2(0, 0); 
         Numpad['8'] = new ivec2(1, 0); 
         Numpad['9'] = new ivec2(2, 0); 
         Numpad['4'] = new ivec2(0, 1); 
         Numpad['5'] = new ivec2(1, 1); 
         Numpad['6'] = new ivec2(2, 1); 
         Numpad['1'] = new ivec2(0, 2); 
         Numpad['2'] = new ivec2(1, 2); 
         Numpad['3'] = new ivec2(2, 2); 
         Numpad['0'] = new ivec2(1, 3); 
         Numpad['A'] = new ivec2(2, 3); 

         Keypad['^'] = new ivec2(1, 0); 
         Keypad['A'] = new ivec2(2, 0); 
         Keypad['<'] = new ivec2(0, 1); 
         Keypad['v'] = new ivec2(1, 1); 
         Keypad['>'] = new ivec2(2, 1); 
      }

      Int64 GetShortest(char start, char end, Dictionary<char, ivec2> pad, int levels) 
      {
         ivec2 startLoc = pad[start]; 
         ivec2 endLoc = pad[end]; 

         if ((startLoc == endLoc) || (levels == 0)) {
            return 1; // just need to press A
         }

         // two real options, going left, or going right
         ivec2 disp = endLoc - startLoc; 
         ivec2 hCorner = startLoc + new ivec2(disp.x, 0); 
         ivec2 vCorner = startLoc + new ivec2(0, disp.y); 

         string hFirst = ""; 
         string vFirst = ""; 
         if (pad.ContainsValue(hCorner)) {
            if (disp.x > 0) {
               hFirst += Util.GetRepeatedChar('>', disp.x); 
            } else if (disp.x < 0) {
               hFirst += Util.GetRepeatedChar('<', -disp.x); 
            }

            if (disp.y > 0) {
               hFirst += Util.GetRepeatedChar('v', disp.y); 
            } else if (disp.y < 0) {
               hFirst += Util.GetRepeatedChar('^', -disp.y); 
            }

            hFirst += 'A'; 
         }

         if (pad.ContainsValue(vCorner)) {
            if (disp.y > 0) {
               vFirst += Util.GetRepeatedChar('v', disp.y); 
            } else if (disp.y < 0) {
               vFirst += Util.GetRepeatedChar('^', -disp.y); 
            }

            if (disp.x > 0) {
               vFirst += Util.GetRepeatedChar('>', disp.x); 
            } else if (disp.x < 0) {
               vFirst += Util.GetRepeatedChar('<', -disp.x); 
            }

            vFirst += 'A'; 
         }


         return Math.Min(GetShortest(hFirst, Keypad, levels - 1), GetShortest(vFirst, Keypad, levels - 1)); 
      }

      Dictionary<int, Dictionary<int, Int64>> Cache = new(); 

      Int64 GetShortest(string seqA, Dictionary<char, ivec2> keypad, int levels) 
      {
         if (string.IsNullOrEmpty(seqA)) {
            return int.MaxValue;
         }

         if (levels ==  0) {
            return seqA.Length; 
         }

         // cache off our shortcuts
         int cacheKey = ((keypad == Numpad) ? 8675309 : 0) + levels; 
         Dictionary<int, Int64>? shortest = null; 

         if (Cache.ContainsKey(cacheKey)) {
            shortest = Cache[cacheKey]; 
         } else {
            shortest = new Dictionary<int, Int64>(); 

               
            HashSet<char> uniques = new(); 
            foreach ((char key, ivec2  v) in keypad) {
               uniques.Add(key); 
            }

            char[] values = uniques.ToArray(); 

            for (int i = 0; i < values.Length; ++i) {
               char ci = values[i]; 
               for (int j = 0; j < values.Length; ++j) {
                  char cj = values[j]; 
                  int key = (ci << 8) + cj; 

                  shortest[key] = GetShortest(ci, cj, keypad, levels); 
               }
            }

            Cache[cacheKey] = shortest; 
         }

         // run through
         char prev = 'A'; 
         Int64 dist = 0; 
         for (int i = 0; i < seqA.Length; ++i) {
            char next = seqA[i]; 
            int key = (prev << 8) + next;
            prev = next; 
            dist += shortest[key];
         }

         return dist; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 ans = 0; 
         foreach (string code in Codes) {
            Int64 shortest = GetShortest(code, Numpad, 3); 
            Int64 codeVal = Int64.Parse(code.Substring(0, code.Length - 1)); 
            ans += shortest * codeVal; 
         }

         return ans.ToString(); 

         // v <vA 3
         // < <A 5
         // < A 6
         // A >>^A 10
         // > vA 12
         // > A 13
         // ^ <^A 16
         // A<A>AvA<^AA>A<vAAA>^A  >A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A

         /*
         string test = "v<<A>>^A<A>AvA<^AA>A<vAAA>^A"; 
         for (int i = 1; i < test.Length; ++i) {
            string token = test.Substring(0, i); 
            int testLen = GetShortest(token, Keypad, 1); 
            Util.WriteLine($"{token} -> {testLen}"); 
         }
         */

         // int len2 = GetShortest("v<<A>>^A<A>AvA<^AA>A<vAAA>^A", Keypad, 1);
         // int len2 = GetShortest(">^", Keypad, 1); 
         // return len.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 ans = 0; 
         foreach (string code in Codes) {
            Int64 shortest = GetShortest(code, Numpad, 26); 
            Int64 codeVal = Int64.Parse(code.Substring(0, code.Length - 1)); 

            Util.WriteLine($"{code} * {shortest} = {shortest * codeVal}"); 
            ans += shortest * codeVal; 
         }

         Util.WriteLine("---"); 
         for (int i = 3; i <= 26; ++i) {
            Int64 shortest = GetShortest(Codes[0], Numpad, i); 
            Int64 codeVal = Int64.Parse(Codes[0].Substring(0, Codes[0].Length - 1)); 
            Util.WriteLine($"{codeVal} * {shortest} = {shortest * codeVal}"); 
         }


         //       220,684,114,439,922
         // 9,223,372,036,854,775,807

         return ans.ToString(); 
      }
   }
}
