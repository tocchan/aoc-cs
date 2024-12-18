using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day18 : Day
   {
      bool IsDebug = false; 
      private string InputFile = "2024/inputs/18.txt";
      private string DebugInputFile = "2024/inputs/18d.txt"; 


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      ivec2 DebugSize = new ivec2(7); 
      ivec2 RealSize = new ivec2(71);       

      ivec2 SizeToUse = new ivec2(0); 
      List<ivec2> ByteLocations = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string inputFile = IsDebug ? DebugInputFile : InputFile; 
         SizeToUse = IsDebug ? DebugSize : RealSize; 

         List<string> lines = Util.ReadFileToLines(inputFile);
         ByteLocations = lines.Select(ivec2.Parse).ToList(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int turnsToRun = IsDebug ? 12 : 1024; 

         IntHeatMap2D map = new IntHeatMap2D(SizeToUse); 
         for (int i = 0; i < turnsToRun; ++i) {
            map.Set(ByteLocations[i], -1); 
         }

         ivec2 startLocation = new ivec2(0); 
         ivec2 endLocation = SizeToUse - new ivec2(1); 

         IntHeatMap2D pathMap = map.DijkstraFlood(startLocation, (ivec2 to, ivec2 from) => {
            return (map.Get(to) == 0) ? 1 : -1; 
         }); 

         /*
         Util.WriteLine(map.ToString()); 
         Util.WriteLine("---"); 
         Util.WriteLine(pathMap.ToString()); 
         */

         List<ivec2> path = pathMap.GetSlopePath(endLocation); 
         return (path.Count - 1).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int ans = 0; 
         for (int turnsToRun = 0; turnsToRun < ByteLocations.Count; ++turnsToRun) {
            IntHeatMap2D map = new IntHeatMap2D(SizeToUse); 
            for (int i = 0; i < turnsToRun; ++i) {
               map.Set(ByteLocations[i], -1); 
            }

            ivec2 startLocation = new ivec2(0); 
            ivec2 endLocation = SizeToUse - new ivec2(1); 

            IntHeatMap2D pathMap = map.DijkstraFlood(startLocation, (ivec2 to, ivec2 from) => {
               return (map.Get(to) == 0) ? 1 : -1; 
            }); 

            /*
            Util.WriteLine(map.ToString()); 
            Util.WriteLine("---"); 
            Util.WriteLine(pathMap.ToString()); 
            */

            int endCost = pathMap.Get(endLocation); 
            if (endCost == int.MaxValue) {
               ans = turnsToRun; 
               break; 
            }
         }

         return ByteLocations[ans - 1].ToString(); 
      }
   }
}
