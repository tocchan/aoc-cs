using AoC;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day13 : Day
   {
      private string InputFile = "2024/inputs/13.txt";

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Input
      {
         public Input(List<string> lines, ref int idx)
         {
            string btnA = lines[idx]; 
            string btnB = lines[idx + 1]; 
            string prize = lines[idx + 2]; 
            idx += 4; 

            ButtonA = ParseButton(btnA); 
            ButtonB = ParseButton(btnB); 
            Prize = ParsePrize(prize); 
         }

         public lvec2 ButtonA; 
         public lvec2 ButtonB; 
         public lvec2 Prize; 

         private lvec2 ParseButton(string btn)
         {
            btn = btn.Split(':', StringSplitOptions.TrimEntries)[1]; 
            string[] btns = btn.Split(',').ToArray(); 

            lvec2 ret = new lvec2(); 
            ret.x = Int64.Parse(btns[0].Split('+')[1]); 
            ret.y = Int64.Parse(btns[1].Split('+')[1]); 
            return ret; 
         }

         private lvec2 ParsePrize(string prize) 
         {
            prize = prize.Split(':', StringSplitOptions.TrimEntries)[1]; 
            string[] vals = prize.Split(',').ToArray(); 

            lvec2 ret = new lvec2(); 
            ret.x = Int64.Parse(vals[0].Split('=')[1]); 
            ret.y = Int64.Parse(vals[1].Split('=')[1]); 
            return ret; 
         }

         public Int64 ComputeCost(Int64 extra = 0)
         {
            Int64 aCount = 0; 
            Int64 bCount = 0; 
            lvec2 prize = Prize + new lvec2(extra, extra); 

            if (ButtonA.IsColinear(ButtonB)) {
               lvec2 reduced = ButtonA.GetReduced(); 
               Int64 gcdA = ButtonA.x / reduced.x; 
               Int64 gcdB = ButtonB.x / reduced.x; 

               Int64 gcdP = prize.x / reduced.x; 
               if (reduced * gcdP != prize) {
                  return 0; // impossible
               }

               // colinear, so make sure we can get a solution that is
               // A * gcdA + B * gcdB = gcdP;
               if ((3 * gcdB) > gcdA) {
                  // slope is positive, take the smallest A value I can
                  for (Int64 x = 0; x < 100; ++x) {
                     Int64 num = gcdP - gcdA * x;
                     if ((num % gcdB) == 0) {
                        aCount = x; 
                        bCount = num / gcdB; 
                        break; 
                     }
                  }
               } else {
                  // slope is negative, take the smallest B value I can
                  for (Int64 y = 0; y < 100; ++y) {
                     Int64 num = gcdP - gcdB * y;
                     if ((num % gcdA) == 0) {
                        bCount = y; 
                        aCount = num / gcdA; 
                        break; 
                     }
                  }
               }

            } else {
               Int64 num = prize.x * ButtonA.y - ButtonA.x * prize.y; 
               Int64 den = ButtonB.x * ButtonA.y - ButtonA.x * ButtonB.y;
               if ((num % den) != 0) {
                  return 0; // no solution; 
               }
               bCount = num / den; 

               num = prize.x - ButtonB.x * bCount; 
               den = ButtonA.x; 
               if ((num % den) != 0) {
                  return 0; // no whole solution
               }               
               aCount = num / den; 
            }

            return 3 * aCount + bCount; 
         }
      }

      private List<Input> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int idx = 0; 
         while (idx < lines.Count) {
            Input input = new Input(lines, ref idx); 
            Inputs.Add(input); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 cost = 0; 
         foreach (Input input in Inputs) {
            cost += input.ComputeCost(); 
         }
         return cost.ToString();  
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 cost = 0; 
         foreach (Input input in Inputs) {
            cost += input.ComputeCost(10000000000000); 
         }
         return cost.ToString();  
      }
   }
}
