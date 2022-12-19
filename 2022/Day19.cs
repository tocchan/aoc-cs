using AoC;
using AoC2019;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day19 : Day
   {

      private string InputFile = "2022/inputs/19d.txt";

      
      internal struct Blueprint
      {
         public int Index;
         public ivec4[] Costs; 

         public Blueprint( int idx )
         {
            Index = idx; 
            Costs = new ivec4[4]; 
         }
      }

      internal List<Blueprint> Inputs = new(); 


      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Regex r = new(@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.", RegexOptions.Compiled); 
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            var match = r.Match(line);

            Blueprint v = new Blueprint(int.Parse(match.Groups[1].Value)); 

            v.Costs[0].x = int.Parse(match.Groups[2].Value); 

            v.Costs[1].x = int.Parse(match.Groups[3].Value); 

            v.Costs[2].x = int.Parse(match.Groups[4].Value); 
            v.Costs[2].y = int.Parse(match.Groups[5].Value); 

            v.Costs[3].x = int.Parse(match.Groups[6].Value); 
            v.Costs[3].z = int.Parse(match.Groups[7].Value); 

            Inputs.Add(v); 
         }
      }

      //----------------------------------------------------------------------------------------------
      private bool CanBuild( Blueprint bp, ivec4 res )
      {
         foreach (ivec4 cost in bp.Costs) {
            if (cost <= res) {
               return true; 
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      private string GetBotName( int idx ) => idx switch {
         0 => "ore", 
         1 => "clay", 
         2 => "obsidian", 
         3 => "geode", 
         _ => "errno"
      }; 
      
      //----------------------------------------------------------------------------------------------
      // Get number of days to get the required materials with our currenet bot setup
      private int GetDaysNeededSingle( Blueprint bp, int idx, ivec4 res, ivec4 bots, int needed )
      {
         if (needed == 0) {
            return 0; 
         }

         ivec4 botCost = bp.Costs[idx]; 
         ivec4 resNeeded = ivec4.Max( ivec4.ZERO, (needed * botCost - res) * ivec4.Sign(botCost) ); 
         int daysNeeded = 0; 
         for (int i = 0; i < 4; ++i) {
            int cost = resNeeded[i]; 
            if (cost == 0) {
               // nothing
            } else if (bots[i] == 0) {
               return int.MaxValue; 
            } else {
               int days = (cost + bots[i] - 1) / bots[i]; // div round up
               daysNeeded = Math.Max(days, daysNeeded); 
            }
         }

         return daysNeeded; 
      }


      //----------------------------------------------------------------------------------------------
      // Get number of days needed to produce each of the individual bots. .
      private ivec4 GetDaysNeeded( Blueprint bp, ivec4 res, ivec4 bots, ivec4 totalNeeded )
      {
         ivec4 needed = ivec4.ZERO;  
         for (int i = 0; i < 4; ++i) {
            needed[i] = GetDaysNeededSingle( bp, i, res, bots, totalNeeded[i] ); 
         }

         return needed;
      }

      //----------------------------------------------------------------------------------------------
      private void AttemptBuild( Blueprint bp, Int64 min, ref ivec4 bots, ref ivec4 res )
      {
         if (!CanBuild(bp, res)) {
            return; 
         }

         // can only build one at a time, figure out what I need the most of.  
         ivec4 totalCost = ivec4.ZERO; 
         for (int i = 0; i < 4; ++i) {
            totalCost = ivec4.Max(bp.Costs[i], totalCost); 
         }
         ivec4 totalNeeded = ivec4.Max( ivec4.ZERO, totalCost - bots ); 
         totalNeeded.w = 1000; // want as many of these as possible

         // now, determine based on what I actually have (don't ask for things that I have enough of)
         totalCost = ivec4.ZERO; 
         for (int i = 0; i < 4; ++i) {
            if (totalNeeded[i] > 0) {
               totalCost = ivec4.Max(bp.Costs[i], totalCost); 
            }
         }
         totalNeeded = ivec4.Max( ivec4.ZERO, totalCost - bots ); 
         totalNeeded.w = 1000; // want as many of these as possible
         ivec4 mask = ivec4.Sign(totalNeeded); 

         ivec4 daysNeeded = GetDaysNeeded( bp, res, bots, totalNeeded );

         int buildIdx = 3; // I always want to build geodes
         int idx = 3; 
         while (true) {
            if (res >= bp.Costs[buildIdx]) {
               break; 
            } else {
               if (idx < 0) {
                  return; // don't want to build anything
               }

               // what happens if I build this?
               ivec4 newRes = res - bp.Costs[idx]; 
               ivec4 newBots = bots; 
               newBots[idx] += 1; 

               // does building this get us there faster?
               int newDays = GetDaysNeededSingle( bp, buildIdx, newRes, newBots, totalNeeded[buildIdx] ); // how many days to get to this?

               if ((idx != buildIdx) && (newDays <= daysNeeded[buildIdx])) {
                  // this improved what we _wanted_ to build, so try to build this
                  buildIdx = idx; 
               } else {
                  // this had no change, so don't bother, pick the next heighest thing we need
                  mask[idx] = 0; 
               }
               idx--; 
            }
         }

         if (buildIdx >= 0) { 
            bots[buildIdx] += 1; 
            res -= bp.Costs[buildIdx]; 
            Util.WriteLine( $"+ Built {GetBotName(buildIdx)}..." ); 
         } 
      }

      private ivec4 Attempt2( Blueprint bp, Int64 min ) 
      {
         ivec4 bots = new ivec4(1, 0, 0, 0);  
         ivec4 res = ivec4.ZERO;  
        
         while (min > 0) {
            --min; 

            Util.WriteLine($"\nMinute {24 - min}"); 

            ivec4 origBots = bots; 
            AttemptBuild( bp, min, ref bots, ref res ); 

            res += origBots; 
            Util.WriteLine( $"- {GetBotName(0)}:{res.x},  {GetBotName(1)}:{res.y},  {GetBotName(2)}:{res.z},  {GetBotName(3)}:{res.w}" ); 
         }

         return res; 
      }      

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 minutes = 24; 
         Int64 quality = 0; 
         // foreach (Blueprint bp in Inputs) {
            // Int64 geodes = Attempt2(bp, minutes).w; 
            // quality += bp.Index * geodes; 
            Int64 geodes = Attempt2(Inputs[0], minutes).w; 
            quality += Inputs[0].Index * geodes; 
         // }

         return quality.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
