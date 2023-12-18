using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
  

   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   internal class Day15 : Day
   {
      private string InputFile = "2022/inputs/15.txt";
      
      List<ivec2> Sensors = new(); 
      List<ivec2> Closest = new(); 
      

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            string[] parts = line.Split('='); 
            int x0 = int.Parse(parts[1].Substring(0, parts[1].IndexOf(','))); 
            int y0 = int.Parse(parts[2].Substring(0, parts[2].IndexOf(':'))); 
            int x1 = int.Parse(parts[3].Substring(0, parts[3].IndexOf(','))); 
            int y1 = int.Parse(parts[4]); 

            Sensors.Add( new ivec2(x0, y0) ); 
            Closest.Add( new ivec2(x1, y1) ); 
         }
      }

      public DisjointIntRange ComputeSet(int row)
      {
         DisjointIntRange ranges = new DisjointIntRange(); 
         for (int i = 0; i < Sensors.Count; ++i) {
            ivec2 sensor = Sensors[i]; 
            ivec2 beacon = Closest[i]; 

            int distance = (beacon - sensor).GetManhattanDistance(); 
            int ydist = Math.Abs(row - sensor.y); 
            int xwidth = distance - ydist; 

            if (xwidth >= 0) { 
               IntRange range = new IntRange(sensor.x - xwidth, sensor.x + xwidth); 
               ranges.AddRange(range); 
            }
         }

         return ranges; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // go through each beacon, figure how many of that beacon lie on my row.  Subtract a count of all unique beacons on that row
         int row = 974977; 
         DisjointIntRange ranges = ComputeSet(row); 
         
         HashSet<ivec2> rowBeacons = new(); 
         for (int i = 0; i < Sensors.Count; ++i) {
            ivec2 beacon = Closest[i]; 
            if (beacon.y == row) {
               rowBeacons.Add(beacon); 
            }
         }

         // count all ranges, but subtract any overlaps
         Int64 count = ranges.GetCount(); 
         count -= rowBeacons.Count; 

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         for (int y = 0; y < 4000000; ++y) {
            DisjointIntRange ranges = ComputeSet(y); 
            ranges.SetLowerBound(0); 
            if (ranges.GetRangeCount() > 1) {
               Int64 x = ranges.GetRange(0)!.Max + 1; 
               Int64 freq = x; 
               freq = freq * 4000000 + (Int64)y; 
               return freq.ToString(); 
            }
         }
         return ""; 
      }
   }
}
