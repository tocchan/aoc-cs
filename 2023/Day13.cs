using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day13 : Day
   {
      private string InputFile = "2023/inputs/13.txt";
      
      List<IntHeatMap2D> Maps = new List<IntHeatMap2D>(); 

      void AddMap(List<string> lines, int startIdx, int endIdx)
      {
         int width = lines[startIdx].Length; 
         int height = endIdx - startIdx; 
         IntHeatMap2D map = new IntHeatMap2D(new ivec2(width, height), 0); 

         int y = 0; 
         for (int i = startIdx; i < endIdx; ++i) {
            for (int x = 0; x < width; ++x) {
               if (lines[i][x] == '#') {
                  map[x, y] = 1; 
               }
            }
            ++y; 
         }

         Maps.Add(map); 
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int startIdx = 0; 
         for (int i = 0; i < lines.Count; ++i) {
            if (string.IsNullOrEmpty(lines[i])) {
               AddMap(lines, startIdx, i); 
               startIdx = i + 1; 
            }
         }

         AddMap(lines, startIdx, lines.Count); 
      }

      int GetColumnDiffCount(IntHeatMap2D map, int col0, int col1)
      {
         int diffCount = 0; 
         for (int y = 0; y < map.GetHeight(); ++y) {
            if (map[col0, y] != map[col1, y]) {
               ++diffCount;  
            }
         }

         return diffCount; 
      }

      int GetRowDiffCount(IntHeatMap2D map, int row0, int row1)
      {
         int diffCount = 0; 
         for (int x = 0; x < map.GetWidth(); ++x) { 
            if (map[x, row0] != map[x, row1]) {
               ++diffCount; 
            }
         }

         return diffCount; 
      }

      //----------------------------------------------------------------------------------------------
      int? FindVerticalSplit(IntHeatMap2D map, int smudgeCount)
      {
         for (int x = 1; x < map.GetWidth(); ++x) {
            int checkSize = Math.Min(x, map.GetWidth() - x); 
            int diffCount = 0; 
            for (int j = 0; j < checkSize; ++j) {
               diffCount += GetColumnDiffCount(map, x - j - 1, x + j); 
            }

            if (diffCount == smudgeCount) {
               return x;
            }
         }

         return null; 
      }

      //----------------------------------------------------------------------------------------------
      int? FindHorizontalSplit(IntHeatMap2D map, int smudgeCount)
      {
         for (int y = 1; y < map.GetHeight(); ++y) {
            int checkSize = Math.Min(y, map.GetHeight() - y);
            bool isMirrored = true;
            int diffCount = 0; 
            for (int j = 0; j < checkSize; ++j) {
               diffCount += GetRowDiffCount(map, y - j - 1, y + j);
            }

            if (diffCount == smudgeCount) {
               return y;
            }
         }

         return null;
      }

      int Solve(int smudgeCount)
      {
         int count = 0;
         foreach (IntHeatMap2D map in Maps) {
            int? hSplit = FindVerticalSplit(map, smudgeCount);
            if (!hSplit.HasValue) {
               int? vSplit = FindHorizontalSplit(map, smudgeCount);
               count += 100 * vSplit!.Value;
            } else {
               count += hSplit!.Value;
            }
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         return Solve(0).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return Solve(1).ToString(); 
      }
   }
}
