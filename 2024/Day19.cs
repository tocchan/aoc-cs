using AoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day19 : Day
   {
      private string InputFile = "2024/inputs/19.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      public List<string> Tokens = new(); 
      public List<string> Inputs = new();   

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Tokens = lines[0].Split(',', StringSplitOptions.TrimEntries).ToList(); 

         for (int i = 2; i < lines.Count; ++i) {
            Inputs.Add(lines[i]); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public bool CanConstruct(string onsen, int offset = 0) 
      {
         if (offset >= onsen.Length) {
            return true; 
         }

         foreach (string token in Tokens) {
            if (onsen.MatchesAt(token, offset)) {
               if (CanConstruct(onsen,  offset + token.Length)) {
                  return true; 
               }
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      public Dictionary<string, Int64> Cache = new(); 

      public Int64 CountPatterns(string onsen) 
      {
         if (string.IsNullOrEmpty(onsen)) {
            return 1; 
         }

         if (Cache.ContainsKey(onsen)) {
            return Cache[onsen]; 
         }


         Int64 sum = 0; 
         foreach (string token in Tokens) {
            if (onsen.StartsWith(token)) {
               sum += CountPatterns(onsen.Substring(token.Length)); 
            }
         }

         Cache[onsen] = sum; 
         return sum; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int count = 0; 
         foreach (string input in Inputs) {
            if (CanConstruct(input)) {
               ++count; 
            }
         }

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 count = 0; 
         foreach (string input in Inputs) {
            count += CountPatterns(input); 
         }

         return count.ToString(); 
      }
   }
}
