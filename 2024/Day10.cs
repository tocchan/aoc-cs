using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day10 : Day
   {
      private string InputFile = "2024/inputs/10.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private IntHeatMap2D Map = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> inputs = Util.ReadFileToLines(InputFile); 
         Map.SetFromTightBlock(inputs); 
      }

      public int RatePath(HashSet<ivec2> ends, ivec2 pos, int val) 
      {
         if (!Map.ContainsPoint(pos) || (Map.Get(pos) != val)) {
            return 0; 
         }

         if (val == 9) {
            ends.Add(pos); 
            return 1; 
         }

         int score = 0; 
         foreach (ivec2 dir in ivec2.DIRECTIONS) {
            score += RatePath(ends, pos + dir, val + 1); 
         }

         return score; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int score = 0; 
         foreach ((ivec2 pos, int v) in Map)
         {
            if (v == 0) {
               HashSet<ivec2> ends = new(); 
               RatePath(ends, pos, v); 
               score += ends.Count; 
            }
         }
         return score.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int score = 0; 
         foreach ((ivec2 pos, int v) in Map)
         {
            if (v == 0) {
               HashSet<ivec2> ends = new(); 
               score += RatePath(ends, pos, v); 
            }
         }
         return score.ToString(); 
      }
   }
}
