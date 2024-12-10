using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day08 : Day
   {
      private string InputFile = "2024/inputs/08.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------

      private IntHeatMap2D Map = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string input = Util.ReadFileToString(InputFile); 
         Map.InitFromString(input); 
      }

      //----------------------------------------------------------------------------------------------
      private void AddAntinodes(HashSet<ivec2> antinodes, int val) 
      {
         List<ivec2> locations = Map.FindLocations((ivec2 loc) => Map.Get(loc) == val).ToList(); 
         for (int i = 0; i < locations.Count; ++i) {
            for (int j = i + 1; j < locations.Count; ++j) {
               ivec2 diff = locations[j] - locations[i]; 
               ivec2 min = locations[i] - diff; 
               ivec2 max = locations[j] + diff; 
               if (Map.ContainsPoint(min)) {
                  antinodes.Add(min); 
               }
               if (Map.ContainsPoint(max)) {
                  antinodes.Add(max); 
               }
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      private void AddAllAntinodes(HashSet<ivec2> antinodes, int val) 
      {
         List<ivec2> locations = Map.FindLocations((ivec2 loc) => Map.Get(loc) == val).ToList(); 
         for (int i = 0; i < locations.Count; ++i) {
            for (int j = i + 1; j < locations.Count; ++j) {
               ivec2 diff = locations[j] - locations[i]; 
               int gcd = (int) Util.GCD(diff.x, diff.y); 
               diff /= gcd; 

               ivec2 pos = locations[i]; 

               // all all in the line
               while (Map.ContainsPoint(pos)) {
                  pos -= diff; 
               }

               pos += diff;
               while (Map.ContainsPoint(pos)) {
                  antinodes.Add(pos); 
                  pos += diff; 
               }
            }
         }
      }


      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         HashSet<ivec2> uniqueNodes = new(); 
         HashSet<int> seen = new(); 

         foreach ((ivec2 loc, int val) in Map)
         {
            if ((val == '.') || seen.Contains(val)) {
               continue; 
            }

            seen.Add(val); 
            AddAntinodes(uniqueNodes, val); 
         }

         return uniqueNodes.Count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         HashSet<ivec2> uniqueNodes = new(); 
         HashSet<int> seen = new(); 

         foreach ((ivec2 loc, int val) in Map)
         {
            if ((val == '.') || seen.Contains(val)) {
               continue; 
            }

            seen.Add(val); 
            AddAllAntinodes(uniqueNodes, val); 
         }

         return uniqueNodes.Count.ToString(); 
      }
   }
}
