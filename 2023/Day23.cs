using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day23 : Day
   {
      private string InputFile = "2023/inputs/23d.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Input
      {
         public Input(string line)
         {

         }
      }

      private List<Input> Inputs = new(); 
      private IntHeatMap2D Map = new IntHeatMap2D(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Map.SetFromTightBlock(lines, '#', 0);  
      }

      ivec2? GetSlopeDirection(int tile)
      {
         return tile switch {
            '^' => ivec2.UP, 
            '>' => ivec2.RIGHT, 
            '<' => ivec2.LEFT, 
            'v' => ivec2.DOWN, 
            _ => null
         }; 
      }

      bool IsValid(int value, ivec2 dir)
      {
         if (value == '#') {
            return false; 
         } else if (value == '.') {
            return true; 
         } else {
            ivec2 slopeDir = GetSlopeDirection(value)!.Value; 
            return slopeDir == dir; 
         }
      }

      int ToInt(bool value) => value ? 1 : 0; 

      int FollowPath(ivec2 pos, ivec2 dir, ivec2 end, Stack<ivec2> history)
      {
         int stepCount = 0; 
         while (true) {
            if (pos == end) {
               return stepCount; 
            }

            // we'll either return invalid, or we'll take at least one step (recusive function is one step ahead)
            ++stepCount; 

            if (history.Contains(pos)) {
               return int.MinValue; // rehit an old path
            }

            int tile = Map[pos]; 
            if (tile == '#') {
               return int.MinValue; // hit a wall?
            }
            ivec2? slope = GetSlopeDirection(tile); 
            if (slope.HasValue) {
               // can't be an intersection, we don't hav ea choice
               dir = slope.Value; 
               pos += dir; 
               continue; 
            }

            ivec2 forward = dir; 
            ivec2 left = dir.GetRotatedLeft(); 
            ivec2 right = dir.GetRotatedRight(); 

            int fTile = Map[pos + forward]; 
            int lTile = Map[pos + left]; 
            int rTile = Map[pos + right];

            bool fValid = IsValid(fTile, forward); 
            bool lValid = IsValid(lTile, left); 
            bool rValid = IsValid(rTile, right); 

            int numValid = ToInt(fValid) + ToInt(lValid) + ToInt(rValid); 
            if (numValid == 0) {
               return int.MaxValue; 
            } else if (numValid == 1) {
               if (fValid) {
                  dir = forward; 
               } else if (rValid) {
                  dir = right; 
               } else {
                  dir = left; 
               }
               pos += dir; 
            } else {
               int longest = int.MinValue;

               history.Push(pos);
               { 
                  if (fValid) {
                     longest = Math.Max(longest, FollowPath(pos + forward, forward, end, history)); 
                  }

                  if (lValid) {
                     longest = Math.Max(longest, FollowPath(pos + left, left, end, history));
                  }

                  if (rValid) {
                     longest = Math.Max(longest, FollowPath(pos + right, right, end, history));
                  }
               }
               history.Pop(); 

               return (longest >= 0) ? stepCount + longest : int.MinValue; 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public int FindLongestPath(ivec2 start, ivec2 end)
      {
         Stack<ivec2> intersections = new(); 
         return FollowPath(start, ivec2.DOWN, end, intersections); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // think just search for it?
         ivec2 size = Map.GetSize(); 
         ivec2 startPoint = new ivec2(1, 0); 
         ivec2 endPoint = size - new ivec2(2, 1); 
         int steps = FindLongestPath(startPoint, endPoint); 
         return steps.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
