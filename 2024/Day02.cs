using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day02 : Day
   {
      private string InputFile = "2024/inputs/02.txt";

      private List<List<Int64>> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) 
         {
            Inputs.Add(line.Split(' ').Select(Int64.Parse).ToList()); 
         }
      }

      public bool IsSafe(List<Int64> input) 
      {
         if (input.Count <= 1) 
         {
            return true; 
         }

         Int64 prevDiff = 0; 
         for (int i = 1; i < input.Count; ++i) 
         {
            Int64 diff = input[i] - input[i - 1]; 

            // sign change?
            if ((prevDiff * diff) < 0) 
            {
               return false; 
            }

            Int64 adiff = Math.Abs(diff); 
            if ((adiff < 1) || (adiff > 3))
            {
               return false; 
            }

            prevDiff = diff; 
         }

         return true; 
      }

      bool IsSafeWithOneRemoved(List<Int64> input) 
      {
         for (int i = 0; i < input.Count; ++i) 
         {
            Int64 oldVal = input[i]; 
            input.RemoveAt(i); 
            bool isSafe = IsSafe(input); 

            input.Insert(i, oldVal); 
            if (isSafe)
            {
               return true; 
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int safeCount = 0; 
         foreach (var input in Inputs) 
         {
            safeCount += IsSafe(input) ? 1 : 0; 
         }

         return safeCount.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int safeCount = 0; 
         foreach (var input in Inputs) 
         {
            if (IsSafe(input) || IsSafeWithOneRemoved(input)) 
            {
               ++safeCount; 
            }
         }

         return safeCount.ToString(); 
      }
   }
}

