using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day12 : Day
   {
      private string InputFile = "2024/inputs/12.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      IntHeatMap2D Garden = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string data = Util.ReadFileToString(InputFile); 
         Garden.InitFromString(data); 
      }

      //----------------------------------------------------------------------------------------------
      int CountEdges(List<ivec2> region) 
      {
         // this could be better - I'm double checking most edges, but good enough for AoC work.
         int edgeCount = 0; 
         foreach (ivec2 p in region) {
            foreach (ivec2 dir in ivec2.DIRECTIONS) {
               if (!region.Contains(p + dir)) {
                  ++edgeCount; 
               }
            }
         }

         return edgeCount; 
      }

      void RemoveConnectingSide(List<ivec2> sides, ivec2 start, ivec2 dir)
      {
         // again inefficient, I'm double checking, but removing first makes it so I at least
         // don't infinite loop
         if (!sides.Remove(start)) {
            return; 
         }

         RemoveConnectingSide(sides, start + dir, dir); 
         RemoveConnectingSide(sides, start - dir, dir); 
      } 

      //----------------------------------------------------------------------------------------------
      int CountSides(List<ivec2> region) 
      {
         List<ivec2> hSides = new(); 
         List<ivec2> vSides = new(); 
         // this could be better - I'm double checking most edges, but good enough for AoC work.
         foreach (ivec2 p in region) {
            ivec2 ip = p + new ivec2(1, 1); // get rid of 0's for the side trick
            foreach (ivec2 dir in ivec2.DIRECTIONS) {
               if (!region.Contains(p + dir)) {
                  if (dir.y == -1) {
                     hSides.Add(ip * new ivec2(1, -1)); // see note below
                  } else if (dir.y == 1) {
                     hSides.Add(ip); 
                  } else if (dir.x == -1) {
                     vSides.Add(ip * new ivec2(-1, 1));  // use negative to differentiate between left & right walls
                  } else {
                     vSides.Add(ip); 
                  }
               }
            }
         }

         int hSideCount = 0; 
         while (hSides.Count > 0) {
            ++hSideCount; 
            RemoveConnectingSide(hSides, hSides[0], ivec2.RIGHT); 
         }

         int vSideCount = 0; 
         while (vSides.Count > 0) {
            ++vSideCount; 
            RemoveConnectingSide(vSides, vSides[0], ivec2.DOWN); 
         }

         return hSideCount + vSideCount; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int cost = 0; 
         IntHeatMap2D seen = new IntHeatMap2D(Garden.GetSize(), 0); 
         foreach ((ivec2 p, int v) in Garden) {
            if (seen.Get(p) == 1) {
               continue; 
            }

            List<ivec2> region = Garden.GetMagicWandRegion(p, v); 
            int area = region.Count; 
            int edges = CountEdges(region); 
            cost += area * edges; 

            // mark plot as seen
            foreach (ivec2 r in region) {
               seen.Set(r, 1); 
            }
         }

         return cost.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int cost = 0; 
         IntHeatMap2D seen = new IntHeatMap2D(Garden.GetSize(), 0); 
         foreach ((ivec2 p, int v) in Garden) {
            if (seen.Get(p) == 1) {
               continue; 
            }

            List<ivec2> region = Garden.GetMagicWandRegion(p, v); 
            int area = region.Count; 
            int sides = CountSides(region); 
            cost += area * sides; 

            // Util.WriteLine($"{area} * {sides} = {area * sides}"); 

            // mark plot as seen
            foreach (ivec2 r in region) {
               seen.Set(r, 1); 
            }
         }

         return cost.ToString(); 
      }
   }
}
