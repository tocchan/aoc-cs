using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AoC2023
{
   internal class Day10 : Day
   {
      private string InputFile = "2023/inputs/10.txt";
      
      private IntHeatMap2D Map = new IntHeatMap2D(); 
      private IntHeatMap2D FillMap = new IntHeatMap2D(); 
      private ivec2 StartPos = new ivec2(0); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int width = lines[0].Length;
         int height = lines.Count; 

         Map = new IntHeatMap2D(new ivec2(width, height), '.', '.'); 
         for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
               char c = lines[y][x]; 
               if (c == 'S') {
                  StartPos = new ivec2(x, y);
               }
               Map[x, y] = c; 
            }
         }

         FillMap = new IntHeatMap2D(Map.GetSize(), 0, 0); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // just looking at the input, know S is a '|', so just set that for now for simplicity
         Map[StartPos] = '|'; 
         
         ivec2 pos = StartPos; 
         ivec2 dir = ivec2.UP;  // again, now S is a straight, so start going up or down
         int length = 0; 
         do {
            FillMap[pos] = 1; 

            ++length; 
            pos = pos + dir; 

            int next = Map[pos]; 
            switch (next) {
               case '-':
                  break; 
               case '|':
                  break; // keep going
               case '7':
                  dir = dir == ivec2.UP ? ivec2.LEFT : ivec2.DOWN; 
                  break; 
               case 'J':
                  dir = dir == ivec2.DOWN ? ivec2.LEFT : ivec2.UP;
                  break; 
               case 'L':
                  dir = dir == ivec2.LEFT ? ivec2.UP : ivec2.RIGHT; 
                  break;
               case 'F':
                  dir = dir == ivec2.UP ? ivec2.RIGHT : ivec2.DOWN; 
                  break; 
               default:
                  break; // leave it - shouldn't hit here though
            }

         } while (pos != StartPos); 

         return (length / 2).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int height = FillMap.GetHeight(); 
         int width = FillMap.GetWidth();

         // L---JF-JLJIIIIFJLJJ7

         int fillCount = 0; 
         bool isInFill = false; 
         for (int y = 0; y < height; ++y) {
            isInFill = false; 
            int startCorner = 0; 
            for (int x = 0; x < width; ++x) {
               if (FillMap[x, y] == 1) {
                  // cross a boundry, either cross a '|', or two corners
                  int wall = Map[x, y]; 
                  if (wall == '|') {
                     isInFill = !isInFill; 
                  } else if (wall == '-') { 
                     // ignore, along an edge
                  } else { // a corner
                     // two corners
                     if (startCorner == 0) {
                        startCorner = wall;
                     } else {
                        bool swap = false; 
                        switch (startCorner) {
                           case 'F': swap = wall == 'J'; break; 
                           case 'L': swap = wall == '7'; break; 
                           default: break; 
                        }
                        startCorner = 0; 
                        
                        if (swap) {
                           isInFill = !isInFill; 
                        }
                     }
                  }
               } else if (isInFill) {
                  FillMap[x, y] = 2; 
                  ++fillCount; 
               }
            }
         }

         // Util.WriteLine(FillMap.ToString()); 

         return fillCount.ToString();  
      }
   }
}
