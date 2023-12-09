using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day09 : Day
   {
      private string InputFile = "2023/inputs/09.txt";
      
      List<List<Int64>> Sequences = new List<List<Int64>>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (var line in lines) {
            Sequences.Add(line.Split(' ').Select(Int64.Parse).ToList());  
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 sum = 0; 
         foreach (var sequence in Sequences) {
            sum += Util.GetNextIn(sequence); 
         }
         return sum.ToString();  
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 sum = 0;
         foreach (var sequence in Sequences) {
            sum += Util.GetPreviousIn(sequence);
         }
         return sum.ToString();
      }
   }
}
