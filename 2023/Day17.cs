using AoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day17 : Day
   {
      private string InputFile = "2023/inputs/17d.txt";

      struct lookup_key
      {
         public ivec2 pos; 
         public ivec2 dir; 
         public int step;

         public override bool Equals([NotNullWhen(true)] object? obj)
         {
            if ((obj == null) || !obj.GetType().Equals(GetType())) {
               return false;
            }

            lookup_key other = (lookup_key)obj;
            return pos == other.pos
               && dir == other.dir
               && step == other.step; 
         }

         public override int GetHashCode()
         {
            return HashCode.Combine(pos, dir, step); 
         }
      }

      IntHeatMap2D Map = new IntHeatMap2D(); 
      IntHeatMap2D Visited = new IntHeatMap2D(); 
      Dictionary<lookup_key,int> Lookup = new Dictionary<lookup_key, int>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Map.SetFromTightBlock(lines, int.MaxValue, '0'); 
      }

      //----------------------------------------------------------------------------------------------
      public int FindBestPath(ivec2 pos, ivec2 dir, int stepsRemaining, ivec2 end)
      {
         if (!Map.ContainsPoint(pos)) {
            return int.MaxValue; 
         }

         if (Visited[pos] == 1) {
            return int.MaxValue; 
         }

         int heat = Map[pos]; 
         if (pos == end) {
            return heat; 
         }

         lookup_key key; 
         key.pos = pos; 
         key.dir = dir; 
         key.step = stepsRemaining; 
         if (Lookup.ContainsKey(key)) {
            return Lookup[key]; 
         }

         Visited[pos] = 1; 

         int bestNext = int.MaxValue; 
         if (stepsRemaining > 0) {
            bestNext = Math.Min(bestNext, FindBestPath(pos + dir, dir, stepsRemaining - 1, end)); 
         } 

         ivec2 left = dir.GetRotatedLeft(); 
         ivec2 right = dir.GetRotatedRight(); 
         int leftBest = FindBestPath(pos + left, left, 2, end); 
         int bestRight = FindBestPath(pos + right, right, 2, end); 

         bestNext = Math.Min(bestNext, Math.Min(leftBest, bestRight));
         Visited[pos] = 0;  // other person can try this; 

         if (bestNext == int.MaxValue) {
            Lookup[key] = int.MaxValue; 
            return int.MaxValue; 
         } else {
            Lookup[key] = heat + bestNext;
            return heat + bestNext; 
         }
      }

      // this is A*, but with a slight twist.  So doing it here
      public int FindBestPath(ivec2 start, ivec2 end)
      {
         Visited.Init(Map.GetSize(), 0, int.MaxValue); 

         int option0 = FindBestPath(start, ivec2.RIGHT, 2, end); 
         int option1 = FindBestPath(start, ivec2.DOWN, 2, end); 

         return Math.Min(option0, option1) - Map[start]; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int heatLost = FindBestPath(ivec2.ZERO, Map.GetSize() - ivec2.ONE); 
         return heatLost.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
