using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day16 : Day
   {
      private string InputFile = "2023/inputs/16.txt";
      

      IntHeatMap2D Map = new IntHeatMap2D(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Map.SetFromTightBlock(lines, -1, 0); 
      }

      void LaunchBeam(IntHeatMap2D lightMap, ivec2 origin, ivec2 dir)
      {
         ivec2 pos = origin; 
         while (lightMap.ContainsPoint(pos)) {
            int oldValue = lightMap[pos]; 
            int newValue = oldValue; 
            if (dir == ivec2.RIGHT) {
               newValue |= 1; 
            } else if (dir == ivec2.DOWN) {
               newValue |= 2; 
            } else if (dir == ivec2.LEFT) {
               newValue |= 4; 
            } else if (dir == ivec2.UP) {
               newValue |= 8; 
            }
            if (oldValue == newValue) {
               return; 
            }

            lightMap.Set(pos, newValue); 
            
            int tile = Map[pos];
            switch (tile) {
               case '.': {
                     // nothing
                  }
                  break;

               case '|': {
                     if (dir.y == 0) {
                        LaunchBeam(lightMap, pos + ivec2.UP, ivec2.UP);
                        LaunchBeam(lightMap, pos + ivec2.DOWN, ivec2.DOWN);
                        return;
                     }
                  }
                  break;

               case '-': {
                     if (dir.x == 0) {
                        LaunchBeam(lightMap, pos + ivec2.RIGHT, ivec2.RIGHT);
                        LaunchBeam(lightMap, pos + ivec2.LEFT, ivec2.LEFT);
                        return;
                     }
                  }
                  break;

               case '\\': {
                     if (dir.x == 0) {
                        dir.RotateLeft(); 
                     } else {
                        dir.RotateRight(); 
                     }
                  }
                  break;

               case '/': {
                     if (dir.x == 0) {
                        dir.RotateRight(); 
                     } else {
                        dir.RotateLeft(); 
                     }
                  }
                  break;

               default: break;
            }

            pos += dir; 
         }
      }

      int Solve(ivec2 origin, ivec2 dir)
      {
         IntHeatMap2D lightMap = new IntHeatMap2D(Map.GetSize());
         LaunchBeam(lightMap, origin, dir);

         int offCount = lightMap.Count(0);
         int onCount = lightMap.GetArea() - offCount;
         return onCount;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         return Solve(ivec2.ZERO, ivec2.RIGHT).ToString();  
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int width = Map.GetWidth(); 
         int height = Map.GetHeight(); 

         int maxCount = 0; 
         for (int x = 0; x < width; ++x) {
            maxCount = Math.Max(maxCount, Solve(new ivec2(x, 0), ivec2.DOWN));
            maxCount = Math.Max(maxCount, Solve(new ivec2(x, height - 1), ivec2.UP));
         }

         for (int y = 0; y < height; ++y) {
            maxCount = Math.Max(maxCount, Solve(new ivec2(0, y), ivec2.RIGHT));
            maxCount = Math.Max(maxCount, Solve(new ivec2(width - 1, y), ivec2.LEFT));
         }

         return maxCount.ToString(); 
      }
   }
}
