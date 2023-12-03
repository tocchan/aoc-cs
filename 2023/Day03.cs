using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   using GearMap = Dictionary<ivec2, List<int>>; 

   internal class Day03 : Day
   {
      private string InputFile = "2023/inputs/03.txt";
      private List<string> Lines = new List<string>(); 
      private GearMap Gears = new GearMap(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Lines = Util.ReadFileToLines(InputFile);
      }

      private int? IsPart(int y, int x, int len)
      {
         string numString = Lines[y].Substring(x, len); 

         int starty = Math.Max(0, y - 1); 
         int startx = Math.Max(0, x - 1); 
         int endy = Math.Min(Lines.Count - 1, y + 1); 
         int endx = Math.Min(Lines[0].Length - 1, x + len); 

         bool found = false; 
         for (int iy = starty; iy <= endy; ++iy) {
            for (int ix = startx; ix <= endx; ++ix) {
               char c = Lines[iy][ix]; 
               if ((c != '.') && !Util.IsDigit(c)) {
                  found = true; 
                  if (c == '*') {
                     ivec2 pos = new ivec2(ix, iy);
                     
                     if (!Gears.ContainsKey(pos)) {
                        Gears[pos] = new List<int>(); 
                     }
                     Gears[pos].Add(int.Parse(numString)); 
                  }
               } 
            }
         }

         return found ? int.Parse(numString) : null; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int width = Lines[0].Length;
         int height = Lines.Count; 

         int startChar = -1; 

         int total = 0; 
         for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
               char c = Lines[y][x];
               if (Util.IsDigit(c)) {
                  if (startChar < 0) {
                     startChar = x; 
                  }
               } else if (startChar >= 0) {
                  int? part = IsPart(y, startChar, x - startChar);
                  if (part.HasValue) {
                     total += part.Value; 
                  }
                  startChar = -1; 
               }
            }

            if (startChar >= 0) {
               int? part = IsPart(y, startChar, width - startChar);
               if (part.HasValue) {
                  total += part.Value; 
               }
               startChar = -1; 
            }
         }

         return total.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int total = 0; 
         foreach ((_, var list) in Gears) {
            if (list.Count == 2) {
               // Util.WriteLine($"gear: {list[0]} * {list[1]}");
               int ratio = list[0] * list[1]; 
               total += ratio;
            }
         }

         return total.ToString(); 
      }
   }
}
