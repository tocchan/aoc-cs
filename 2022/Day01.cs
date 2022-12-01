using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day01 : Day
   {
      private string InputFile = "2022/inputs/01.txt";
      private List<int> Values = new List<int>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Values = lines.Select(int.Parse).ToList();
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int fuelSum = 0;
         for (int i = 0; i < Values.Count; ++i)
         {
            fuelSum += Values[i];
         }
         return fuelSum.ToString();
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int fuelSum = 0;
         for (int i = 0; i < Values.Count; ++i)
         {
            fuelSum += Values[i];
         }
         return fuelSum.ToString();
      }
   }
}
