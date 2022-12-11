using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Monkey
   {
      public List<Int64> Worry = new(); 
      public Func<Int64,Int64>? Op; 
      public Int64 Divisor = 1; 
      public int MonkeyOnTrue = 0; 
      public int MonkeyOnFalse = 0;  

      public Int64 InspectionCount = 0; 
   }


   internal class Day11 : Day
   {
      private string InputFile = "2022/inputs/11.txt";
      
      List<Monkey> Monkeys = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Monkeys.Clear(); 
         List<string> lines = Util.ReadFileToLines(InputFile);
         
         int idx = 0; 
         while (idx < lines.Count) {
            Monkey monk = new Monkey(); 
            monk.Worry = lines[idx + 1].Split(':')[1].Split(',').Select(Int64.Parse).ToList();

            string[] ops = lines[idx + 2].Split('=')[1].Trim().Split(' '); 
            string opType = ops[1]; 

            if (ops[2] == "old") { 
               if (opType == "+") { 
                  monk.Op = (Int64 v) => v + v; 
               } else { 
                  monk.Op = (Int64 v) => v * v; 
               }
            } else {
               Int64 opValue = Int64.Parse(ops[2]); 
               if (opType == "+") { 
                  monk.Op = (Int64 v) => v + opValue; 
               } else { 
                  monk.Op = (Int64 v) => v * opValue; 
               }
            }

            monk.Divisor = Int64.Parse(lines[idx + 3].Split(' ').Last());

            monk.MonkeyOnTrue = int.Parse(lines[idx + 4].Split(' ').Last()); 
            monk.MonkeyOnFalse = int.Parse(lines[idx + 5].Split(' ').Last()); 
            
            Monkeys.Add(monk); 
            
            idx += 7; 
         }
      }

      void ProcessTurn(Monkey monk)
      {
         while (monk.Worry.Count() > 0) {
            Int64 worry = monk.Worry[0]; 

            monk.Worry.RemoveAt(0); 
            monk.InspectionCount++; 

            worry = monk.Op!(worry); 
            worry = worry / 3; 
            if ((worry % monk.Divisor) == 0) {
               Monkeys[monk.MonkeyOnTrue].Worry.Add(worry); 
            } else {
               Monkeys[monk.MonkeyOnFalse].Worry.Add(worry); 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         for (int i = 0; i < 20; ++i) {
            foreach (Monkey monk in Monkeys) {
               ProcessTurn(monk); 
            }
         }

         Monkey[] sorted = Monkeys.OrderByDescending( (Monkey m) => m.InspectionCount ).ToArray(); 
         Int64 result = sorted[0].InspectionCount * sorted[1].InspectionCount; 

         return result.ToString(); 
      }

      private Int64 LCW = 1; 

      void ProcessTurnB(Monkey monk)
      {
         while (monk.Worry.Count() > 0) {
            Int64 worry = monk.Worry[0]; 

            monk.Worry.RemoveAt(0); 
            monk.InspectionCount++; 

            worry = monk.Op!(worry) % LCW; 

            if ((worry % monk.Divisor) == 0) {
               Monkeys[monk.MonkeyOnTrue].Worry.Add(worry); 
            } else {
               Monkeys[monk.MonkeyOnFalse].Worry.Add(worry); 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         ParseInput(); // reset inputs

         foreach (Monkey m in Monkeys) {
            LCW = Util.LCM(LCW, m.Divisor); 
         }

         for (int i = 0; i < 10000; ++i) {
            foreach (Monkey monk in Monkeys) {
               ProcessTurnB(monk); 
            }
         }

         Monkey[] sorted = Monkeys.OrderByDescending( (Monkey m) => m.InspectionCount ).ToArray(); 
         Int64 result = sorted[0].InspectionCount * sorted[1].InspectionCount; 

         return result.ToString(); 
      }
   }
}
