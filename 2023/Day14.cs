using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AoC2023
{
   internal class Day14 : Day
   {
      private string InputFile = "2023/inputs/14.txt";
      
      IntHeatMap2D Map = new IntHeatMap2D(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Map.SetFromTightBlock(lines.ToArray(), ".#O", -1); 
      }

      public void Tilt(IntHeatMap2D map, ivec2 dir)
      {
         bool changed = true;
         while (changed) {
            changed = false;
            map.CellStep((ivec2 pos, int value) => {
               int curTile = map[pos];
               int southTile = map[pos - dir];

               if ((curTile == 0) && (southTile == 2)) {
                  changed = true;
                  return 2;
               } else if ((curTile == 2) && map[pos + dir] == 0) {
                  changed = true;
                  return 0;
               } else {
                  return curTile;
               }
            });
         }
      }

      int Score(IntHeatMap2D map)
      {
         int height = map.GetHeight();
         int points = 0;
         foreach ((ivec2 pos, int val) in map) {
            if (val == 2) {
               points += (height - pos.y);
            }
         }

         return points; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntHeatMap2D copy = new IntHeatMap2D(Map); 
         
         Tilt(copy, ivec2.UP);
         
         return Score(copy).ToString(); 
      }

      bool IsRepeated(List<int> sequence)
      {
         // must be event for a repeat
         if ((sequence.Count % 2) == 1) {
            return false;
         }

         int size = sequence.Count / 2; 
         for (int i = 0; i < size; ++i) {
            if (sequence[i] != sequence[size + i]) {
               return false; 
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntHeatMap2D copy = new IntHeatMap2D(Map);

         Dictionary<int, int> cachedScores = new Dictionary<int, int>();
         int timeSinceNew = 0; 

         int seqStart = -1; 
         List<int> sequence = new List<int>(); 

         for (int i = 0; i < 1000000000; ++i) {
            Tilt(copy, ivec2.UP); 
            Tilt(copy, ivec2.LEFT); 
            Tilt(copy, ivec2.DOWN); 
            Tilt(copy, ivec2.RIGHT);

            int score = Score(copy); 
            if (cachedScores.ContainsKey(score)) {
               timeSinceNew++;  
            } else {
               timeSinceNew = 0;   
            }

            // >, not >= due to adding to cache afterwards
            if (timeSinceNew > cachedScores.Count) {
               if (seqStart < 0) {
                  seqStart = i + 1; // +1 since the step has been run
                  timeSinceNew = 0; 

                  cachedScores.Clear();
               } else if (IsRepeated(sequence)) {
                  // for safety, we'll wait for a full repeat before exiting
                  break; 
               }
            }

            cachedScores[score] = i;
            if (seqStart >= 0) {
               sequence.Add(score); 
            }
         }

         int remaining = 1000000000 - seqStart; 
         int idx = remaining % sequence.Count(); 
         return sequence[idx].ToString(); 
      }
   }
}
