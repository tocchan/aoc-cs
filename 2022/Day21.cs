using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day21 : Day
   {
      private string InputFile = "2022/inputs/21.txt";
      
      Dictionary<string, Monkey> Monkies = new(); 

      internal class Monkey 
      {
         public Int64 EvalValue()
         {
            if (Op == null) {
               return Value!.Value; 
            } else {
               Int64 lh = Friends[LName].EvalValue(); 
               Int64 rh = Friends[RName].EvalValue(); 
               Value = Op!(lh, rh); 
               return Value.Value; 
            }
         }

         public Int64 GetValue()
         {
            if (Value != null) {
               return Value.Value;
            } else {
               return EvalValue(); 
            }
         }

         public string Name = ""; 
         public Int64? Value = null; 
         public string LName = ""; 
         public string RName = ""; 
         public Func<Int64, Int64, Int64>? Op = null; 
         public Func<Int64?, Int64?, Int64, Int64>? RevOp = null; 

         public char OpChar = ' '; 

         public Dictionary<string, Monkey> Friends = new(); 
      }


      private Int64 SolveAdd(Int64? lh, Int64? rh, Int64 ans) 
      {
         if (lh == null) {
            return ans - rh!.Value; 
         } else {
            return ans - lh!.Value; 
         }
      }

      private Int64 SolveSub(Int64? lh, Int64? rh, Int64 ans) 
      {
         if (lh == null) {
            return ans + rh!.Value; 
         } else {
            return lh!.Value - ans; 
         }
      }

      private Int64 SolveMul(Int64? lh, Int64? rh, Int64 ans) 
      {
         if (lh == null) {
            return ans / rh!.Value; 
         } else {
            return ans / lh!.Value; 
         }
      }
      private Int64 SolveDiv(Int64? lh, Int64? rh, Int64 ans) 
      {
         if (lh == null) {
            return ans * rh!.Value; 
         } else {
            return lh!.Value / ans; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            (string name, string val, _) = line.Split(": ");

            Monkey m = new Monkey(); 
            m.Name = name; 
            m.Friends = Monkies; 

            string[] ops = val.Split(' '); 
            if (ops.Length == 1) {
               m.Value = Int64.Parse(val); 
            } else {
               m.LName = ops[0].Trim(); 
               m.RName = ops[2].Trim(); 
               char op = ops[1].Trim()[0]; 

               m.OpChar = op; 
               m.Op = op switch {
                  '+' => (Int64 a, Int64 b) => a + b, 
                  '-' => (Int64 a, Int64 b) => a - b,
                  '*' => (Int64 a, Int64 b) => a * b, 
                  '/' => (Int64 a, Int64 b) => a / b,
                  _ => null
               }; 

               m.RevOp = op switch {
                  '+' => SolveAdd, 
                  '-' => SolveSub, 
                  '*' => SolveMul, 
                  '/' => SolveDiv, 
                  _ => null
               };
            }

            Monkies.Add(m.Name, m); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Monkey root = Monkies["root"]; 
         Int64 val = root.GetValue(); 
         return val.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      void ClearCache()
      {
         foreach (var pair in Monkies) {
            if (pair.Value.Op != null) {
               pair.Value.Value = null; 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      Int64 HumValue = 1; 
      
      Int64 GetHumanValue(Monkey root, Monkey me, Int64 wantedValue) 
      {
         // Util.WriteLine($"\nEval {root.Name} to equal {wantedValue}"); 
         if (root == me) {
            return wantedValue; 
         }

         Monkey lh = Monkies[root.LName]; 
         Monkey rh = Monkies[root.RName]; 

         // just picking numbers at random
         me.Value = 2213;  
            
         root.EvalValue(); 
         Int64 lh0 = lh.GetValue(); 
         Int64 rh0 = rh.GetValue(); 

         me.Value = -7759; 
         root.EvalValue(); 
         Int64 lh1 = lh.GetValue(); 
         Int64 rh1 = rh.GetValue(); 

         if (lh0 == lh1) {
            Int64 nres = root.RevOp!(lh0, null, wantedValue); 
            // Util.WriteLine($"- {lh1} {root.OpChar} {nres} == {wantedValue}");
            return GetHumanValue(rh, me, nres); 

         } else {
            Int64 nres = root.RevOp!(null, rh0, wantedValue); 
            // Util.WriteLine($"- {nres} {root.OpChar} {rh0} == {wantedValue}");
            return GetHumanValue(lh, me, nres); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Monkey root = Monkies["root"]; 
         root.Op = (a, b) => (a == b) ? 1 : 0; 
         root.RevOp = (a, b, c) => (a == null) ? b!.Value : a!.Value; 
         root.OpChar = '='; 

         Monkey me = Monkies["humn"]; 
         me.Value = HumValue; 
         me.Op = null; 

         return GetHumanValue(root, me, 1).ToString(); 
      }
   }
}
