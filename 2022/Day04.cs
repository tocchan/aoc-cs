using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   

   internal class Day04 : Day
   {
      private string InputFile = "2022/inputs/04.txt";
      
      private List<IntRange> Ranges = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            Ranges.AddRange(line.Split(',').Select(IntRange.Parse)); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int count = 0; 
         for (int i = 0; i < Ranges.Count; i += 2) {
            IntRange r0 = Ranges[i + 0]; 
            IntRange r1 = Ranges[i + 1]; 
            if (r0.Contains(r1) || r1.Contains(r0)) {
               ++count;
            }
         }

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int count = 0; 
         for (int i = 0; i < Ranges.Count; i += 2) {
            IntRange r0 = Ranges[i + 0]; 
            IntRange r1 = Ranges[i + 1]; 
            if (r0.Intersects(r1)) { 
               ++count;
            }
         }

         return count.ToString(); 
      }
   }
}
