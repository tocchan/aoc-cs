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
      private string InputFile = "2024/inputs/21.txt";


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

      List<List<ivec2>> GetAllOptions(Dictionary<char, ivec2> pad, ivec2 startLoc, ivec2 endLoc)
      {
         List<List<ivec2>> paths = new(); 
         List<ivec2> path = new(); 
         AddOption(paths, path, pad, startLoc, endLoc); 

         return paths; 
      }

      void AddOption(List<List<ivec2>> paths, List<ivec2> curPath, Dictionary<char, ivec2> pad, ivec2 curLoc, ivec2 endLoc) 
      {
         if (curLoc == endLoc) {
            paths.Add(curPath); 
            return; 
         }

         if (!pad.ContainsValue(curLoc)) {
            return; // not a valid path
         }


         ivec2 disp = endLoc - curLoc; 
         ivec2 dirs = ivec2.Sign(disp); 

         if (dirs.x != 0) {
            List<ivec2> newPath = new List<ivec2>(curPath); // copy, as we'll be adding new options to it
            ivec2 move = new ivec2(dirs.x, 0); 
            newPath.Add(move); 
            AddOption(paths, newPath, pad, curLoc + move, endLoc); 
         }

         if (dirs.y != 0) {
            List<ivec2> newPath = new List<ivec2>(curPath); // copy, as we'll be adding new options to it
            ivec2 move = new ivec2(0, dirs.y); 
            newPath.Add(move); 
            AddOption(paths, newPath, pad, curLoc + move, endLoc); 
         }
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

         List<List<ivec2>> paths = GetAllOptions(pad, startLoc, endLoc); 

         Int64 shortest = Int64.MaxValue; 
         foreach (List<ivec2> path in paths) {
            string pathStr = ""; 
            foreach (ivec2 dir in path) {
               pathStr += dir.ToDirChar(); 
            }

            pathStr += 'A'; 
            shortest = Math.Min(shortest, GetShortest(pathStr, Keypad, levels - 1)); 
         }

         return shortest; 
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
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 ans = 0; 
         foreach (string code in Codes) {
            Int64 shortest = GetShortest(code, Numpad, 26); 
            Int64 codeVal = Int64.Parse(code.Substring(0, code.Length - 1)); 

            // Util.WriteLine($"{code} * {shortest} = {shortest * codeVal}"); 
            ans += shortest * codeVal; 
         }

         return ans.ToString(); 
      }
   }
}
