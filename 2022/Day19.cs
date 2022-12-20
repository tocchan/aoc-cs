using AoC;
using AoC2019;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AoC2022
{
   internal class Day19 : Day
   {

      private string InputFile = "2022/inputs/19d.txt";

      
      internal class Blueprint
      {
         public int Index;
         public ivec4[] Costs; 
         public Dictionary<ivec4,state> Cache = new(); 

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
      private string GetBotName( int idx ) => idx switch {
         0 => "ore", 
         1 => "clay", 
         2 => "obsidian", 
         3 => "geode", 
         _ => "errno"
      }; 

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      internal class state
      {
         public int min; 
         public ivec4 res; 
         public ivec4 bots; 
         public state? prev = null; 

         public state( ivec4 b ) 
         {
            min = 0; 
            res = ivec4.ZERO; 
            bots = b; 
            prev = null; 
         }

         public state( state parent )
         {
            min = parent.min; 
            res = parent.res; 
            bots = parent.bots; 
            prev = parent; 
         }

         //----------------------------------------------------------------------------------------------
         public int GetRating() { return res.w; }

         //----------------------------------------------------------------------------------------------
         public static state Max( state lh, state rh ) 
         {

            if (lh.GetRating() > rh.GetRating()) {
               return lh; 
            } else {
               return rh;
            }
         }

         //----------------------------------------------------------------------------------------------
         public ivec4 GetTurnstoBuild(Blueprint bp)
         {
            ivec4 turns = ivec4.ZERO; 
            ivec4 maxNeeded = new ivec4(0, 0, 0, int.MaxValue); 
            for (int i = 0; i < 4; ++i) {
               ivec4 cost = bp.Costs[i]; 
               maxNeeded = ivec4.Max( maxNeeded, cost ); 

               int turn = 0; 
               cost = ivec4.Max( ivec4.ZERO, cost - res ); 
               for (int j = 0; j < 4; ++j) {
                  if (cost[j] > 0) {
                     if (bots[j] == 0) {
                        turn = int.MaxValue; 
                        break; 
                     } else {
                        turn = Math.Max( turn, (cost[j] + bots[j] - 1) / bots[j] ); 
                     }
                  }
               }

               turns[i] = turn; 
            }

            for (int i = 0; i < 4; ++i) {
               if (bots[i] >= maxNeeded[i]) {
                  turns[i] = int.MaxValue; 
               }
            }

            return turns; 
         }

         //----------------------------------------------------------------------------------------------
         public bool IsValid(Blueprint bp, int totalMin) 
         {
            // may be my memoize step - figuring out with the time remaining, can get I X number of materials to build a new 
            // geode factor... but for now... lets just throw out anything with no geodes at turn 24
            if (min >= totalMin) {
               return res.w > 0; 
            } else {
               return true; 
            }
         }

         //----------------------------------------------------------------------------------------------
         public override string ToString()
         {
            return $"Minute {min}, bots:{bots}, res:{res}"; 
         }
      }

      //----------------------------------------------------------------------------------------------
      IEnumerable<state> PermuteStates(Blueprint bp, state s, int totalMin) 
      {
         if (s.min >= totalMin) {
            yield break; 
         }

         int timeRemaining = totalMin - s.min; 
         ivec4 turnsToBuild = s.GetTurnstoBuild(bp); 

         // try to build geodes first; 
         for (int i = 3; i >= 0; --i) {
            int turns = turnsToBuild[i]; 
            if (turns >= timeRemaining) { // building this turn, so if equal, too far already
               continue; 
            }

            state ns = new state(s); 
            ns.min += (turns + 1); 
            ns.res += (turns + 1) * s.bots; // get resources up to this turn, AND this turn without the bot
            ns.bots[i] += 1; 
            ns.res -= bp.Costs[i]; 

            yield return ns; 
         }

         // do we even want to "not" build anything?  It is always best to make a move... if we make  no move, just..... add up to the end
         state fs = new state(s); 
         fs.min += timeRemaining; 
         fs.res += timeRemaining * s.bots; 
         yield return fs; 
      }

      private int GetLongest(ivec4 v) 
      {
         int longest = v[0]; 
         int bestIdx = 0; 
         for (int i = 1; i < 4; ++i) {
            if (v[i] < int.MaxValue) {
               if (v[i] >= longest) { // greateer or equal - prefer the more useful one
                  bestIdx = i; 
                  longest = v[i]; 
               }
            }
         }

         return bestIdx; 
      }


      //----------------------------------------------------------------------------------------------
      state ComputeBest(Blueprint bp, state s0, int totalMin) 
      {
         // initial best is just... complete this to the end
         Queue<state> toTry = new(); 
         int lowBound = 0; 

         state best = s0; 
         int bestScore = 0; 

         toTry.Enqueue(s0); 
         while (toTry.Count > 0) {
            state s = toTry.Dequeue(); 

            // pick best; 
            int pot = (totalMin - s.min) * s.bots.w + s.res.w; 
            if (pot > bestScore) {
               best = s; 
               bestScore = pot; 
            }

            foreach (state ns in PermuteStates(bp, s, totalMin)) {
               int minRem = totalMin - ns.min; 

               // this computes a lower bound of what this could accomplish.. what would be the upper bound?
               state subBest = Compute(bp, ns.bots, minRem); 
               int score = subBest.res.w + ns.res.w;
               lowBound = Math.Max(score, lowBound); 
               
               // upper bound.... just assume I was able to build a geode bot this turn somehow
               int potential = score + Math.Max(minRem - 1, 0); 

               // no possible way to finish - trim it out
               if (potential < lowBound) {
                  continue; 
               }

               toTry.Enqueue(ns);
            }
         }

         // advance the best if we ended up with time left. 
         if (best.min < totalMin) {
            best = new state(best); 
            best.res += best.bots * (totalMin - best.min); 
            best.min = totalMin; 
         }

         return best; 
      }


      //----------------------------------------------------------------------------------------------
      state Compute(Blueprint bp, ivec4 startBots, int totalMin) 
      {
         // one minute - just return a state with the number of resources it could mine 
         totalMin = Math.Max(0, totalMin); 
         int geodeBots = startBots.w; 
         if (totalMin <= 1) {
            state s = new state(startBots); 
            s.min = totalMin; 
            s.res = totalMin * startBots; 
            return s; 
         }

         startBots.w = 0; // just assume we're starting at 0 bots - want to see if we can even build one
         ivec4 key = new ivec4(startBots.xyz, totalMin); 
         state? cs = null; 
         if (!bp.Cache.TryGetValue(key, out cs)) {
            state s0 = new state(startBots); 
            state best = ComputeBest(bp, s0, totalMin); 
            bp.Cache.Add(key, best); 
            cs = best; 
         }

         state ns = new state(cs.bots); 
         ns.res = cs.res; 
         ns.min = cs.min; 
         ns.prev = cs.prev; 
         ns.res.w += totalMin * geodeBots; 
         
         return ns; 
      }

      //----------------------------------------------------------------------------------------------
      public void TraceTurns(state s)
      {
         if (s.prev != null) {
            TraceTurns(s.prev); 
         }

         Util.WriteLine( "- " + s.ToString() ); 
      }  


      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int minutes = 24; 
         int quality = 0; 

         // int idx = 1; 
         for (int idx = 0; idx < Inputs.Count; ++idx) 
         {
            Blueprint bp = Inputs[idx]; 
            state s = Compute(bp, new ivec4(1, 0, 0, 0), minutes); 
            Util.WriteLine($"\nInput {idx + 1};"); 
            TraceTurns(s); 
            int geodes = s.res.w; 
            quality += bp.Index * geodes; 
         }

         return quality.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int minutes = 32; 
         int result = 1; 

         // int idx = 1; 
         int count = Math.Min(1, Inputs.Count); 
         for (int idx = 0; idx < Inputs.Count; ++idx) 
         {
            Blueprint bp = Inputs[idx]; 
            int geodes = Compute(bp, new ivec4(1, 0, 0, 0), minutes).res.w; 
            result *= geodes; 
         }

         return result.ToString(); 
      }
   }
}
