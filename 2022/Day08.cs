using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day08 : Day
   {
      private string InputFile = "2022/inputs/08.txt";
      
      IntHeatMap2D Map = new(); 


      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Map = new IntHeatMap2D(lines); 
      }

      //----------------------------------------------------------------------------------------------
      private bool IsVisibleInDirection( ivec2 pos, ivec2 dir )
      {
         int height = Map.Get(pos); 
         ivec2 p = pos + dir; 

         while (Map.ContainsPoint(p)) {
            if (Map.Get(p) >= height) {
               return false; 
            }

            p += dir; 
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      private bool IsVisible( int x, int y )
      {
         ivec2 pos = new ivec2(x, y); 
         foreach (ivec2 dir in ivec2.DIRECTIONS) {
            if (IsVisibleInDirection(pos, dir)) {
               return true; 
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int width = Map.GetWidth(); 
         int height = Map.GetHeight(); 

         int edgeCount = width * 2 + (height - 2) * 2; 

         int count = 0; 
         for (int y = 1; y < height - 1; ++y) {
            for (int x = 1; x < width - 1; ++x) {
               if (IsVisible(x, y)) {
                  ++count; 
               }
            }
         }

         return (count + edgeCount).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      private int GetScore(ivec2 pos, ivec2 dir)
      {
         int stopHeight = Map.Get(pos); 

         ivec2 p = pos + dir; 
         while (Map.ContainsPoint(p)) {
            int h = Map.Get(p); 
            if (h >= stopHeight) {
               break;
            }
            p += dir; 
         }

         p = ivec2.Clamp(p, ivec2.ZERO, Map.GetSize() - ivec2.ONE); 
         return (p - pos).GetManhattanDistance(); 
      }

      //----------------------------------------------------------------------------------------------
      private int GetScore(ivec2 pos)
      {
         int score = 1; 
         foreach (ivec2 dir in ivec2.DIRECTIONS) { 
            score *= GetScore(pos, dir);
         }

         return score; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int width = Map.GetWidth(); 
         int height = Map.GetHeight(); 

         ivec2 bestPos = ivec2.ZERO; 
         int bestScore = 0; 

         int test = GetScore( new ivec2(2, 3) ); 

         for (int y = 1; y < height - 1; ++y) {
            for (int x = 1; x < width - 1; ++x) {
               int score = GetScore(new ivec2(x, y)); 
               if (score > bestScore) {
                  bestScore = score; 
                  bestPos = new ivec2(x, y); 
               }
            }
         }

         return bestScore.ToString(); 
      }
   }
}
