using AoC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AoC2024
{
   internal class Day17 : Day
   {
      private string InputFile = "2024/inputs/17.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Program
      {
         public Program()
         {

         }

         public Program(List<string> lines)
         {
            RegisterA = int.Parse(lines[0].Split(':')[1]); 
            RegisterB = int.Parse(lines[1].Split(':')[1]); 
            RegisterC = int.Parse(lines[2].Split(':')[1]); 

            OpCodes = lines[4].Split(':')[1].Split(',').Select(int.Parse).ToList(); 
         }

         public Program(Program other) 
         {
            RegisterA = other.RegisterA; 
            RegisterB = other.RegisterB; 
            RegisterC = other.RegisterC; 

            OpCodes = new List<int>(other.OpCodes); 
         }

         public Int64 RegisterA = new(); 
         public Int64 RegisterB = new(); 
         public Int64 RegisterC = new(); 

         public List<int> OpCodes = new(); 

         int OpIndex = 0; 

         public List<int> Output = new(); 

         public void Reset(Int64 regA) 
         {
            RegisterA = regA; 
            RegisterB = 0; 
            RegisterC = 0; 
            OpIndex = 0; 
            Output.Clear(); 
         }

         int GetCombo(int idx) 
         {
            if (idx <= 3) {
               return idx; 
            }

            if (idx == 4) {
               return (int)(RegisterA & 0x7fffffff); 
            } else if (idx == 5) {
               return (int)(RegisterB & 0x7fffffff); 
            } else if (idx == 6) {
               return (int)(RegisterC & 0x7fffffff); 
            } else {
               Util.WriteLine($"ERROR: Reserver combo: {idx}"); 
               return 0; 
            }
         }

         public Int64 DV(int opCode) 
         {
            int combo = GetCombo(opCode); 
            if (combo >= 64) {
               return 0; 
            } else {
               return RegisterA >> combo; 
            }
         }

         public bool ExecuteNext()
         {
            if (OpIndex >= OpCodes.Count) {
               return false; 
            }

            int instCode = OpCodes[OpIndex]; 
            int opCode = OpCodes[OpIndex + 1]; 
            OpIndex += 2; 

            switch (instCode) {
               case 0: // A = A / pow(2, combo)
                  RegisterA = DV(opCode); 
                  break; 

               case 1: // bXOR (B = B ^ op)
                  RegisterB = RegisterB ^ opCode; 
                  break; 

               case 2: // B = combo & 0x3
                  RegisterB = GetCombo(opCode) & 0x7; 
                  break; 

               case 3: // jnz 
                  if (RegisterA == 0) {
                     // do nothing
                  } else {
                     OpIndex = opCode; 
                  }
                  break; 

               case 4: // B = (B ^ C) & 8
                  RegisterB = RegisterB ^ RegisterC; 
                  break;

               case 5: {// out
                  int outVal = GetCombo(opCode) & 0x7; 
                  Output.Add(outVal); 
               } break; 

               case 6: // B = A / pow(2, combo)
                  RegisterB = DV(opCode); 
                  break; 

               case 7: // C = A / pow(2 combo)
                  RegisterC = DV(opCode);
                  break; 
            }

            return true; 
         }

         public void Execute()
         {
            while (ExecuteNext());               
         }

         public bool ExecuteWhileMatching(List<int> seq)
         {
            int outputSize = 0; 
            while (ExecuteNext()) {
               if (Output.Count > outputSize) {
                  if ((Output.Count > seq.Count)
                     || (Output[outputSize] != seq[outputSize])) {

                     return false; 
                  } else {
                     ++outputSize; 
                  }
               }
            }

            return Output.Count == seq.Count; 
         }

         public bool ExecuteWhileMatching() 
         {
            return ExecuteWhileMatching(OpCodes); 
         }

         public override string ToString()
         {
            string output = string.Join(',', Output); 
            return $"A[{RegisterA}] B[{RegisterB}] C[{RegisterC}]\nOutput: {output}";
         }
      }

      private Program InitialProgram = new Program(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         InitialProgram = new Program(lines); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Program process = new Program(InitialProgram); 
         process.Execute(); 

         return string.Join(',', process.Output); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Program process = new Program(InitialProgram); 

         Int64 ans = 0; 
         List<int> match = new(); 

         do {
            int insertIdx = process.OpCodes.Count - match.Count - 1; 
            match.Insert(0, process.OpCodes[insertIdx]); 

            ans *= 8; 
            do {
               process.Reset(ans); 
               ++ans; 
            } while (!process.ExecuteWhileMatching(match));
            --ans; 
         } while (match.Count < process.OpCodes.Count); 

         // process.Reset(ans); 
         // process.Execute(); 
         // Util.WriteLine(process.ToString()); 

         return (ans).ToString(); 
      }
   }
}
