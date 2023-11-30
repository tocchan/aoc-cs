using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day01 : Day
   {
      private string InputFile = "2023/inputs/01.txt";
      private List<List<int>> Elves = new List<List<int>>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (List<string> group in lines.SplitAllWhen(x => (x.Length == 0))) {
            List<int> elf = group.Select(int.Parse).ToList(); 
            Elves.Add(elf); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         return "test"; 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return "test2";
      }
   }
}
