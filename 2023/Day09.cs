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

      public Int64 GetNextIn(List<Int64> sequence)
      {
         if (sequence.Count == 0) {
            return 0; 
         }

         List<Int64> diff = new List<Int64>(); 
         bool isAllZero = true; 
         for (int i = 0; i < sequence.Count - 1; ++i) {
            diff.Add(sequence[i + 1] - sequence[i]); 
            isAllZero = isAllZero && diff[i] == 0; 
         }

         if (isAllZero) {
            return sequence.Last(); 
         } else {
            return GetNextIn(diff) + sequence.Last(); 
         }
      }

      public Int64 GetPreviousIn(List<Int64> sequence)
      {
         if (sequence.Count == 0) {
            return 0;
         }

         List<Int64> diff = new List<Int64>();
         bool isAllZero = true;
         for (int i = 0; i < sequence.Count - 1; ++i) {
            diff.Add(sequence[i + 1] - sequence[i]);
            isAllZero = isAllZero && diff[i] == 0;
         }

         if (isAllZero) {
            return sequence.First();
         } else {
            return sequence.First() - GetPreviousIn(diff);
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 sum = 0; 
         foreach (var sequence in Sequences) {
            sum += GetNextIn(sequence); 
         }
         return sum.ToString();  
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 sum = 0;
         foreach (var sequence in Sequences) {
            sum += GetPreviousIn(sequence);
         }
         return sum.ToString();
      }
   }
}
