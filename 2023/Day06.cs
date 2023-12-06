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

      public int DetermineWinners(int ms, int distanceToBeat)
      {
         int count = 0; 
         for (int i = 1; i < ms; ++i) {
            int distance = (ms - i) * i; 
            if (distance > distanceToBeat) {
               ++count; 
            }
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int total = 1; 
         for (int i = 0; i < Times.Length; ++i) {
            total *= DetermineWinners(Times[i], Distances[i]); 
         }
         return total.ToString(); 
      }

      public Int64 DetermineWinnersSmart(Int64 time, Int64 distance)
      {
         // determine first winner
         Int64 startTime = 1; 
         Int64 endTime = time; 

         while (startTime < time) {
            Int64 dist = (time - startTime) * startTime; 
            if (dist > distance) {
               break; 
            }
            ++startTime; 
         }

         while (endTime > 1) {
            Int64 dist = (time - endTime) * endTime;
            if (dist > distance) {
               break; 
            }
            --endTime; 
         }

         // determine last winner
         return (endTime - startTime) + 1; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         string timeString = ""; 
         string distanceString = ""; 

         for (int i = 0; i < Times.Length; ++i) {
            timeString += Times[i].ToString(); 
            distanceString += Distances[i].ToString(); 
         }

         Int64 time = Int64.Parse(timeString); 
         Int64 distance = Int64.Parse(distanceString); 
         
         return DetermineWinnersSmart(time, distance).ToString(); 
      }
   }
}
