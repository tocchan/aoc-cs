using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Valve
   {
      public string Name = ""; 
      public int Index = 0; 
      public int Rate = 0; 
      public string[] Outputs = new string[0]; 
      public Valve[] Exits = new Valve[0];
      public int[] Distances = new int[0]; 
      public bool IsOpen = false; 
   }

   internal class Day16 : Day
   {
      private string InputFile = "2022/inputs/16.txt";

      private Dictionary<string,Valve> Valves = new(); 
      private Valve[] ValvesByIndex = new Valve[0]; 
      
      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         // I curse parnic for what he tempted me to attempt... 38 minutes wasted...
         Regex r = new(@"Valve (?<Input>[A-Z]{2}) has flow rate=(?<Rate>[0-9]+); tunnel(s*) lead(s*) to valve(s*) (?<Valves>.*)", RegexOptions.Compiled); 

         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            var match = r.Match(line); 
            string input = match.Groups["Input"].Value;
            int rate = int.Parse(match.Groups["Rate"].Value); 
            string[] outputs = match.Groups["Valves"].Value.Split(','); 

            Valve v = new(); 
            v.Name = input;
            v.Rate = rate; 
            v.Outputs = outputs; 

            Valves.Add(v.Name, v); 

         }

         ValvesByIndex = new Valve[Valves.Count]; 
         int valveIdx = 0; 
         foreach ((string key, Valve v) in Valves) {
            v.Index = valveIdx; 
            v.Exits = new Valve[v.Outputs.Length]; 
            v.Distances = new int[Valves.Count]; 

            int idx = 0; 
            foreach (string name in v.Outputs) {
               v.Exits[idx] = Valves[name.Trim()]; 
               ++idx; 
            }

            ValvesByIndex[valveIdx] = v; 
            ++valveIdx; 
         }

         ComputeDistances(); 
      }

      // super ugly, but computing "distances" between cells on the graph by just propagating distances
      // along the edges
      private void ComputeDistances(Valve v, int depth)
      {
         // depth one, my immediate 
         if (depth == 0) {
            foreach (Valve exit in v.Exits) {
               v.Distances[exit.Index] = 1;
            }
            return; 
         }

         // for every other depth, get exits 
         for (int i = 0; i < Valves.Count; ++i) {
            if (v.Distances[i] == depth) {
               Valve other = ValvesByIndex[i]; // valve at depth away
               for (int j = 0; j < Valves.Count; ++j) {
                  if (other.Distances[j] == 1) {
                     // this is depth + 1 from me potentialy
                     int newDepth = depth + 1; 
                     if ((j != v.Index) && ((newDepth < v.Distances[j]) || (v.Distances[j] == 0))) {
                        v.Distances[j] = newDepth; 
                     }
                  }
               }
            }
         }
      }

      private void ComputeDistances()
      {
         for (int depth = 0; depth < Valves.Count; ++depth) { 
            foreach ((_, Valve v) in Valves) {
               ComputeDistances(v, depth); 
            }
         }
      }

      public int ComputeBestPressure(Valve loc, Valve[] toCheck, int min, int p) 
      {
         if (loc.Rate > 0) {
            --min; // no reason to move here if we're not going to open it.
            loc.IsOpen = true; 
            p += loc.Rate * min; 
         }

         // now, try to see who else we could open
         int bestPressure = p; 
         foreach (Valve v in toCheck) {
            if (v.IsOpen) {
               continue; 
            }

            // couldn't get there in time
            int dist = loc.Distances[v.Index]; 
            if (dist >= min) {
               continue; 
            }

            bestPressure = Math.Max(bestPressure, ComputeBestPressure(v, toCheck, min - dist, p)); 
         }

         // close myself when I leave so other people can try
         loc.IsOpen = false; 
         return bestPressure; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int totalMinutes = 30; 

         // get indices of any pipes that actually have flow
         Valve start = Valves["AA"]; 
         Valve[] flows = ValvesByIndex.Where( v => v.Rate > 0 ).OrderByDescending(v => v.Rate).ToArray(); 

         // lowers my search space quite a bit
         int bestPressure = ComputeBestPressure(start, flows, totalMinutes, 0); 
         
         return bestPressure.ToString();
      }

      internal class Player
      {
         public Valve loc = new Valve(); 
         public int min = 0; 
      }

      //----------------------------------------------------------------------------------------------
      internal int ComputeBestPressureB(Valve loc, int min, Valve loc1, int min1, Valve[] toCheck, int p) 
      {
         // play the current players turn
         bool isOpen = loc.IsOpen; 
         if (!isOpen && loc.Rate > 0) {
            --min;
            loc.IsOpen = true; 
            p += loc.Rate * min; 
         }

         // whoever has more time goes next
         int bestPressure = p; 
         if (min1 > min) {
            bestPressure = ComputeBestPressureB(loc1, min1, loc, min, toCheck, p); 
         } else { 

            // now, try to see who else we could open
            foreach (Valve v in toCheck) {
               if (v.IsOpen) {
                  continue; 
               }

               // couldn't get there in time
               int dist = loc.Distances[v.Index]; 
               if (dist >= min) {
                  continue; 
               }

               int newPressure = ComputeBestPressureB(v, min - dist, loc1, min1, toCheck, p); 
               if (newPressure > bestPressure) {
                  bestPressure = newPressure; 
               }
            }
         }

         // close myself when I leave so other people can try
         loc.IsOpen = isOpen; // reset to what it was
         return bestPressure; 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int totalMinutes = 26; 

         // get indices of any pipes that actually have flow
         Valve start = Valves["AA"]; 
         Valve[] flows = ValvesByIndex.Where( v => v.Rate > 0 ).OrderByDescending(v => v.Rate).ToArray(); 

         // lowers my search space quite a bit
         int bestPressure = ComputeBestPressureB(start, totalMinutes, start, totalMinutes, flows, 0); 
         
         return bestPressure.ToString();
      }
   }
}
