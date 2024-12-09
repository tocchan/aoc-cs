using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day06 : Day
   {
      private string InputFile = "2024/inputs/06.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private IntHeatMap2D Map = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string str = Util.ReadFileToString(InputFile); 
         Map.InitFromString(str); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntHeatMap2D travel = new IntHeatMap2D(Map); 

         ivec2 loc = travel.FindLocation((ivec2 pos, int v) => v == '^')!.Value; 
         ivec2 dir = ivec2.UP; 

         int visited = 1; 
         travel.Set(loc, 2); 

         ivec2 next = loc + dir; 
         while (travel.ContainsPoint(next)) 
         {
            int nextVal = travel.Get(next); 
            if (nextVal == '#') {
               dir.RotateRight(); 
            } else {
               if (nextVal == '.') {
                  travel.Set(next, 2); 
                  ++visited; 
               }
               
               loc = next; 
            }

            next = loc + dir; 
         }


         return visited.ToString(); 
      }

      int ToUniqueId(ivec2 dir) 
      {
         return dir.x * 7 + dir.y * 11; // honestly 1 and 2 would have been fine, but whatever
      }

      bool WouldLoop(IntHeatMap2D map, ivec2 loc, ivec2 dir) 
      {
         int idToFind = ToUniqueId(dir); 
         while (map.ContainsPoint(loc)) {
            if (map.Get(loc) == idToFind) {
               return true; 
            }

            loc += dir; 
         }

         return false; 
      }

      bool WouldLoopSlow(ivec2 loc, ivec2 dir) 
      {
         ivec2 stop = loc + dir; 
         if (!Map.ContainsPoint(stop)) {
            return false; 
         }

         IntHeatMap2D copy = new IntHeatMap2D(Map); 
         copy.Set(stop, '#'); 

         ivec2 next = loc + dir; 
         while (copy.ContainsPoint(next)) {
            int val = copy.Get(next);  
            if (val == '#') {
               int dirBitFlag = 1 << dir.GetDirectionBit(); 
               int locVal = copy.Get(loc); 
               if (locVal == '.') {
                  locVal = dirBitFlag; 
               }  else {
                  if ((locVal & dirBitFlag) != 0) {
                     return true; 
                  }

                  locVal |= dirBitFlag; 
               }
               copy.Set(loc, locVal); 
               dir = dir.GetRotatedRight(); 
            } else {
               loc = next; 
            }

            next = loc + dir; 
         }

         return false; 
      }

      bool WouldLoopVerySlow(ivec2 loc, ivec2 dir, ivec2 stopPos) 
      {
         if (!Map.ContainsPoint(stopPos)) {
            return false; 
         }

         IntHeatMap2D copy = new IntHeatMap2D(Map); 
         copy.Set(stopPos, '#'); 

         int stopVal = Map.GetArea(); 

         ivec2 next = loc + dir; 
         int counter = 0; 
         while ((counter < stopVal) && copy.ContainsPoint(next)) {
            ++counter; 
            int val = copy.Get(next);  
            if (val == '#') {
               dir = dir.GetRotatedRight(); 
            } else {
               loc = next; 
            }

            next = loc + dir; 
         }

         return counter == stopVal; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntHeatMap2D travel = new IntHeatMap2D(Map); 

         ivec2 loc = travel.FindLocation((ivec2 pos, int v) => v == '^')!.Value; 
         ivec2 dir = ivec2.UP; 
         ivec2 startLoc = loc; 
         
         travel.Set(loc, ToUniqueId(dir)); // starting location can't be used for a loop

         int answers = 0; 
         for (int y = 0; y < Map.GetHeight(); ++y) {
            for (int x = 0; x < Map.GetWidth(); ++x) {
               ivec2 block = new ivec2(x, y); 
               if (block == startLoc) {
                  continue; 
               }
               if (WouldLoopVerySlow(startLoc, ivec2.UP, block)) {
                  ++answers;
               }
            }
         }

         return answers.ToString(); 
      }
   }
}
