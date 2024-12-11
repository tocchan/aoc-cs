using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day11 : Day
   {
      private string InputFile = "2024/inputs/11.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private List<Int64> Sequence = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Sequence = Util.ReadFileToString(InputFile).Split(' ').Select(Int64.Parse).ToList(); 
      }

      Dictionary<string, Int64> Cache = new(); 

      public Int64 Count(Int64 v, int stepsRemaining) 
      {
         if (stepsRemaining == 0) {
            return 1; 
         }

         string vstr = v.ToString(); 
         string key = vstr + "-" + stepsRemaining.ToString(); 
         if (Cache.ContainsKey(key)) {
            return Cache[key]; 
         }

         Int64 result = 0; 
         if (v == 0) {
            result = Count(1, stepsRemaining - 1); 
         } else if ((vstr.Length % 2) == 0) {
            int cut = vstr.Length / 2; 
            Int64 lh = Int64.Parse(vstr.Substring(0, cut));
            Int64 rh = Int64.Parse(vstr.Substring(cut)); 
            result = Count(lh, stepsRemaining - 1)
               + Count(rh, stepsRemaining - 1); 
         } else {
            result = Count(v * 2024, stepsRemaining - 1);
         }

         Cache.Add(key, result); 
         return result; 
      }

      public Int64 Count(int steps) 
      {
         Int64 count = 0; 
         foreach (Int64 v in Sequence) {
            count += Count(v, steps); 
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         return Count(25).ToString();
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return Count(75).ToString(); 
      }
   }
}
