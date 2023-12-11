using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day11 : Day
   {
      private string InputFile = "2023/inputs/11.txt";
      

      IntHeatMap2D Map = new IntHeatMap2D(); 
      List<ivec2> Stars = new List<ivec2>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         int width = lines[0].Length; 
         int height = lines.Count; 

         Map = new IntHeatMap2D(new ivec2(width, height), 1, 0);

         bool[] emptyColumns = new bool[width];
         bool[] emptyRows = new bool[height]; 

         emptyColumns.SetAll(true); 
         emptyRows.SetAll(true); 

         for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
               if (lines[y][x] == '#') {
                  Stars.Add(new ivec2(x, y)); 
                  
                  emptyColumns[x] = false; 
                  emptyRows[y] = false; 
               }
            }
         }

         for (int y = 0; y < height; ++y) {
            if (emptyRows[y]) { 
               Map.SetRow(y, 2); 
            }
         }

         for (int x = 0; x < width; ++x) {
            if (emptyColumns[x]) {
               Map.SetColumn(x, 2); 
            }
         }
      }

      Int64 GetDistance(ivec2 s, ivec2 f)
      {
         ivec2 dir = ivec2.Sign(f - s); 

         Int64 distance = 0; 

         if (dir.y != 0) {
            for (int y = s.y + dir.y; y != f.y; y += dir.y) {
               distance += Map[s.x, y]; 
            }
            ++distance; 
         }

         if (dir.x != 0) { 
            for (int x = s.x + dir.x; x != f.x; x += dir.x) {
               distance += Map[x, f.y]; 
            }
            ++distance; 
         }

         return distance; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 total = 0;
         for (int i = 0; i < Stars.Count; ++i) {
            for (int j = i + 1; j < Stars.Count; ++j) {
               Int64 dist = GetDistance(Stars[i], Stars[j]);
               total += dist;
            }
         }

         return total.ToString();
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Map.ReplaceAll(2, 1000000);
         return RunA(); 
         
      }
   }
}
