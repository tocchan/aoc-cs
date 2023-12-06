using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day06 : Day
   {
      private string InputFile = "2023/inputs/06.txt";

      int[] Times = new int[0]; 
      int[] Distances = new int[0]; 


      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Times = lines[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
         Distances = lines[1].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
      }


      //----------------------------------------------------------------------------------------------
      public Int64 DetermineWinners(Int64 time, Int64 distance)
      {
         (double t0, double t1) = Util.Quadratic(-1.0, time, -distance);
         t0 = Math.Ceiling(t0);
         t1 = Math.Floor(t1);

         return (Int64)(t1 - t0) + 1;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 total = 1; 
         for (int i = 0; i < Times.Length; ++i) {
            total *= DetermineWinners(Times[i], Distances[i]); 
         }
         return total.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 time = Int64.Parse(string.Concat(Times.Select(i => i.ToString())));
         Int64 distance = Int64.Parse(string.Concat(Distances.Select(i => i.ToString())));
         return DetermineWinners(time, distance).ToString(); 
      }
   }
}
