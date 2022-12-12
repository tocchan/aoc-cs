using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day12 : Day
   {
      private string InputFile = "2022/inputs/12.txt";
      

      ivec2 Start = new(); 
      ivec2 End = new(); 
      IntHeatMap2D Map = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Map.SetFromTightBlock(lines, int.MaxValue, 'a'); 

         ivec2? start = Map.FindLocation( (ivec2 pos, int val) => val == 'S' - 'a' ); 
         ivec2? end = Map.FindLocation( (ivec2 pos, int val) => val == 'E' - 'a' ); 

         Start = start!.Value;
         End = end!.Value; 

         Map.Set( Start, 0 ); 
         Map.Set( End, 'z' - 'a' ); 
      }

      private int PathCost(ivec2 s, ivec2 e)
      {
         int sv = Map.Get(s); 
         int ev = Map.Get(e); 
         int diff = ev - sv; 
         if (sv == Map.GetBoundsValue() || ev == Map.GetBoundsValue() || (diff > 1)) {
            return Map.GetBoundsValue(); 
         } else {
            return 1; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntHeatMap2D fill = Map.DijkstraFlood(End, PathCost); 
         return fill.Get(Start).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntHeatMap2D fill = Map.DijkstraFlood(End, PathCost); 

         int lowest = int.MaxValue; 
         ivec2 start = ivec2.ZERO; 
         foreach ((ivec2 p, int v) in Map) { 
            if (v == 0) {
               if (fill.Get(p) < lowest) {
                  start = p; 
                  lowest = fill.Get(p); 
               }
            }
         }

         return lowest.ToString(); 
      }
   }
}
