using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day07 : Day
   {
      private string InputFile = "2024/inputs/07.txt";

      struct Input
      {
         public Input(string val) 
         {
            (string test, string list) = val.Split(": "); 
            TestValue = Int64.Parse(test); 
            Params = list.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Int64.Parse).ToList(); 
         }

         public Int64 TestValue; 
         public List<Int64> Params; 

         bool CheckA(Int64 tally, int idx) 
         {
            if (idx >= Params.Count) {
               return tally == TestValue; 
            }

            return CheckA(tally + Params[idx], idx + 1)
               || CheckA(tally * Params[idx], idx + 1); 
         }

         Int64 Concat(Int64 lh, Int64 rh) 
         {
            return Int64.Parse(lh.ToString() + rh.ToString()); 
         }

         bool CheckB(Int64 tally, int idx) 
         {
            if (idx >= Params.Count) {
               return tally == TestValue; 
            }

            return CheckB(tally + Params[idx], idx + 1)
               || CheckB(tally * Params[idx], idx + 1)
               || CheckB(Concat(tally, Params[idx]), idx + 1); 
         }

         public bool IsValidA()
         {
            return CheckA(Params[0], 1); 
         }

         public bool IsValidB()
         {
            return CheckB(Params[0], 1); 
         }
      }

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private List<Input> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> inputs = Util.ReadFileToLines(InputFile); 
         foreach (string s in inputs) {
            Inputs.Add(new Input(s)); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 answer = 0; 
         foreach (Input i in Inputs) {
            if (i.IsValidA()) {
               answer += i.TestValue; 
            }
         }
         return answer.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 answer = 0; 
         foreach (Input i in Inputs) {
            if (i.IsValidB()) {
               answer += i.TestValue; 
            }
         }
         return answer.ToString(); 
      }
   }
}
