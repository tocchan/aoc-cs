using AoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day20 : Day
   {
      private string InputFile = "2024/inputs/20.txt";

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      IntHeatMap2D Map = new(); 
      ivec2 StartLocation; 
      ivec2 EndLocation; 

      class PathState
      {
         public ivec2 Location = new ivec2(); 
         public int CheatTime = 0; 
         public int Cost = 0; 
         public PathState? Prev = null;

         public override int GetHashCode()
         {
            return HashCode.Combine(Location, CheatTime); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string mapString = Util.ReadFileToString(InputFile); 
         Map.InitFromString(mapString); 

         StartLocation = Map.FindLocation('S')!.Value; 
         EndLocation = Map.FindLocation('E')!.Value; 

         Map.Set(StartLocation, '.'); 
         Map.Set(EndLocation, '.'); 
      }

      //----------------------------------------------------------------------------------------------

      int CostFunction(ivec2 dst, ivec2 src) 
      {
         return Map.Get(dst) == '#' ? -1 : 1; 
      }

      List<ivec2> FindCheatLocations(IntHeatMap2D map, ivec2 start, int dist) 
      {
         List<ivec2> results = new(); 

         bool isEven = (dist % 2) == 0; 
         for (int y = -dist; y <= dist; ++y) {
            for (int x = -dist; x <= dist; ++x) {
               ivec2 disp = new ivec2(x, y); 
               int manDist = disp.GetManhattanDistance(); 
               if ((manDist > dist) || (manDist == 0)) {
                  continue; 
               }

               results.Add(start + disp); 
            }
         }

         return results; 
      }

      int CountShortcuts(IntHeatMap2D fromStart, IntHeatMap2D fromEnd, int cheatDist, int minSavings, int maxCost) 
      {
         int count = 0; 
         foreach ((ivec2 v, int val) in fromStart) {
            if ((val < 0) || (val == int.MaxValue)) {
               continue; 
            }

            if (val >= maxCost) {
               continue; 
            }

            List<ivec2> dstLocs = FindCheatLocations(fromStart, v, cheatDist); 
            foreach (ivec2 dst in dstLocs) {
               if (!fromEnd.ContainsPoint(dst)) {
                  continue; 
               }

               int valToGetToCheat = val + (dst - v).GetManhattanDistance(); 
               int dstCost = fromEnd.Get(dst); 
               if (dstCost >= maxCost) {
                  continue; 
               }

               int totalCost = valToGetToCheat + dstCost; 
               int savings = maxCost - totalCost; 

               if (savings >= minSavings) {
                  ++count; 
               }
            }
         }

         return count;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntHeatMap2D fromEnd = Map.DijkstraFlood(EndLocation, CostFunction); 
         IntHeatMap2D fromStart = Map.DijkstraFlood(StartLocation, CostFunction); 
         int turns = fromEnd.Get(StartLocation); 

         int numShortcuts = CountShortcuts(fromStart, fromEnd, 2, 100, turns); 

         // Util.WriteLine(flood.ToString()); 

         return numShortcuts.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntHeatMap2D fromEnd = Map.DijkstraFlood(EndLocation, CostFunction); 
         IntHeatMap2D fromStart = Map.DijkstraFlood(StartLocation, CostFunction); 
         int turns = fromEnd.Get(StartLocation); 

         int numShortcuts = CountShortcuts(fromStart, fromEnd, 20, 100, turns); 

         // Util.WriteLine(flood.ToString()); 

         return numShortcuts.ToString(); 
      }
   }
}
