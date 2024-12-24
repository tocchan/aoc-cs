using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day24 : Day
   {
      private string InputFile = "2024/inputs/24.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      
      class Register
      {
         public string Name = ""; 
         public int State = -1; 
      }

      class Circuit 
      {
         public Register LH; 
         public Register RH; 
         public Register Store; 

         public Action Logic; 

         public Circuit(Register lh, Register rh, string op, Register store) 
         {
            LH = lh; 
            RH = rh; 
            Store = store; 

            if (op == "XOR") {
               Logic = () => Store.State = LH.State! ^ RH.State; 
            } else if (op == "OR") {
               Logic = () => Store.State = LH.State | RH.State; 
            } else { // AND
               Logic = () => Store.State = LH.State & RH.State; 
            } 
         }

         public int GetValue(Machine machine) 
         {
            if (Store.State < 0) {

               if (LH.State < 0) {
                  machine.Compute(LH); 
               } 
               
               if (RH.State < 0) {
                  machine.Compute(RH); 
               }

               Logic(); 
            }
            
            return Store.State; 
         }
      }

      class Machine
      {
         public Machine()
         {
         }

         public Register AddRegister(string name, int initialState) 
         {
            Register reg = FindOrAddRegister(name); 
            reg.State = initialState; 
            return reg; 
         }

         Register FindOrAddRegister(string name) 
         {
            Register? register; 
            if (!Registers.TryGetValue(name, out register)) {
               register = new Register(); 
               register.Name = name; 

               Registers.Add(name, register); 
            }

            return register; 
         }

         public Circuit AddCircuit(string eq, string output) 
         {
            string[] eqParts = eq.Split(' ').ToArray(); 

            Register lh = FindOrAddRegister(eqParts[0]); 
            Register rh = FindOrAddRegister(eqParts[2]); 
            Register store = FindOrAddRegister(output); 

            Circuit circuit = new Circuit(lh, rh, eqParts[1], store); 

            if (Circuits.ContainsKey(store.Name)) {
               Util.WriteLine($"Added double entry for '{store.Name}'"); 
            }
            Circuits.Add(store.Name, circuit); 

            return circuit; 
         }

         public Int64 Compute(string name) 
         {
            Register reg = FindOrAddRegister(name);  
            return Compute(reg); 
         }

         public Int64 Compute(Register reg) 
         {
            if (reg.State >= 0) {
               return reg.State; 
            }

            Circuit? circuit; 
            if (Circuits.TryGetValue(reg.Name, out circuit)) {
               return circuit.GetValue(this); 
            } else {
               Util.WriteLine($"Something went terribly wrong - had no source circuit or initial value for '{reg.Name}'"); 
               return -1; 
            }
         }

         public int GetRegisterCount(char leading)
         {
            int count = 0; 
            foreach ((string name, Register reg) in Registers) {
               if (name[0] == leading) {
                  ++count; 
               }
            }

            return count; 
         }

         public Int64 GetRegisterValue(char leading) 
         {
            int outputs = GetRegisterCount(leading); 

            Int64 result = 0; 
            for (int i = 0; i < outputs; ++i) {
               string name = leading + i.ToString("D2"); 
               result = result | (Compute(name) << i); 
            }

            return result; 
         }

         public void SetRegisterValue(char leading, Int64 val) 
         {
            int outputs = GetRegisterCount(leading); 

            Int64 result = 0; 
            for (int i = 0; i < outputs; ++i) {
               string name = leading + i.ToString("D2"); 
               Register reg = FindOrAddRegister(name); 
               reg.State = (val & (1L << i)) == 0 ? 0 : 1; 
            }
         }

         public void Reset()
         {
            foreach ((string name, Circuit circuit) in Circuits) {
               circuit.Store.State = -1; 
            }
         }

         public List<string> GetPossibleRegisters()
         {
            List<string> result = new(); 
            foreach ((string name, Circuit circuit) in Circuits) {
               if (name[0] != 'z') {
                  result.Add(name);
               }
            }

            return result; 
         }

         public List<string> GetFeedingRegisters(string reg) 
         {
            Circuit c = Circuits[reg]; 
            Stack<Circuit> toCheck = new(); 
            HashSet<string> results = new(); 

            toCheck.Push(c); 
            while (toCheck.Count > 0) {
               Circuit circuit = toCheck.Pop(); 

               // not going to add registers that aren't computed (input registers)
               // as they're not possible  candidates
               Circuit? next; 
               if (Circuits.TryGetValue(circuit.LH.Name, out next)) {
                  results.Add(circuit.LH.Name); 
                  toCheck.Push(next); 
               }

               if (Circuits.TryGetValue(circuit.RH.Name, out next)) {
                  results.Add(circuit.RH.Name); 
                  toCheck.Push(next); 
               }
            }

            return results.ToList(); 
         }

         public bool Swap(string regA, string regB)
         {
            if (regA == regB) {
               return false; 
            }

            // make sure this swap wouldn't result in an infinite loop
            List<string> feeders = GetFeedingRegisters(regA); 
            if (feeders.Contains(regB)) {
               return false;
            }

            feeders = GetFeedingRegisters(regB); 
            if (feeders.Contains(regA)) {
               return false; 
            }

            Circuit c0 = Circuits[regA]; 
            Circuit c1 = Circuits[regB]; 

            c0.Store = FindOrAddRegister(regB); 
            c1.Store = FindOrAddRegister(regA); 

            Circuits.Remove(regA); 
            Circuits.Remove(regB); 
            Circuits.Add(regA, c1); 
            Circuits.Add(regB, c0); 
            return true; 
         }


         Dictionary<string, Register> Registers = new(); 
         Dictionary<string, Circuit> Circuits = new(); 
      }

      Machine TheMachine = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         
         int idx = 0; 
         while (!string.IsNullOrEmpty(lines[idx])) {
            (string name, string valStr) = lines[idx].Split(':', StringSplitOptions.TrimEntries).ToArray(); 
            TheMachine.AddRegister(name, int.Parse(valStr)); 
            ++idx; 
         }

         ++idx; 
         while (idx < lines.Count) {
            (string eq, string output) = lines[idx].Split("->", StringSplitOptions.TrimEntries).ToArray(); 
            TheMachine.AddCircuit(eq, output); 
            ++idx; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 result = TheMachine.GetRegisterValue('z'); 
         return result.ToString(); 
      }

      public void Subtract(List<string> lh, List<string> rh) {
         foreach (string v in rh) {
            lh.Remove(v); 
         }
      }

      bool TryFix(string lh, string rh, Int64 testVal) 
      {
         // now, try to swap until this succeeds?
         if (!TheMachine.Swap(lh, rh)) {
            return false; 
         }

         TheMachine.Reset(); 
         Int64 newVal = TheMachine.GetRegisterValue('z');
         if (newVal == testVal) {
            return true;
         } else {
            TheMachine.Swap(rh, lh); 
         }

         return false; 
      }

      int GetMismatchedIndex(Int64 lh, Int64 rh) 
      {
         Int64 diff = lh ^ rh; 
         if (diff == 0) {
            Util.WriteLine("bad inputs, no mismatch"); 
            return -1; 
         }
         for (int i = 0; i < 64; ++i) {
            if ((diff & (1L << i)) != 0L) {
               return i; 
            }
         }

         return -1; 
      }

      List<(string, string)> Mix(List<string> lh, List<string> rh) 
      {
         List<(string, string)> result = new(); 
         foreach (string lx in lh) {
            foreach (string rx in rh) {
               result.Add((lx, rx)); 
            }
         }

         return result; 
      }

      bool TryFixMachine(int startIdx, List<string> candidates, List<string> swaps)
      {
         int regCount = TheMachine.GetRegisterCount('z'); 

         // get first error
         List<(Int64, Int64)> toCheck = new(); 
         toCheck.Add((Int64.MaxValue, Int64.MaxValue)); 
         toCheck.Add((0, Int64.MaxValue)); 
         toCheck.Add((Int64.MaxValue, 0)); 
         toCheck.Add((0xAAAAAAAAAAAAAAL, 0x55555555555555L)); 

         for (int i = startIdx; i < regCount; ++i) {
            Int64 mask = (1L << i) - 1; 

            foreach ((Int64 lhv, Int64 rhv) in toCheck) {
               TheMachine.Reset(); 

               Int64 lh = lhv & mask; 
               Int64 rh = rhv & mask; 
               TheMachine.SetRegisterValue('x', lh); 
               TheMachine.SetRegisterValue('y', rh); 
               Int64 zVal = TheMachine.GetRegisterValue('z'); 
               Int64 expected = lh + rh; 

               if (expected != zVal) {
                  int tryIdx = GetMismatchedIndex(expected, zVal); 
                  // do 
                  {
                     string regName = 'z' + (tryIdx).ToString("D2"); 

                     List<string> possibleRegisters = TheMachine.GetFeedingRegisters(regName); 
                     possibleRegisters = possibleRegisters.Intersect(candidates).ToList();

                     List<(string, string)> combos = Mix(possibleRegisters, candidates); 
                     foreach ((string lhs, string rhs) in combos) {
                        if (TryFix(lhs, rhs, expected)) {
                           List<string> newCandidates = new List<string>(candidates); 

                           Util.WriteLine($"Attempting swap at {tryIdx} of {lhs} <-> {rhs}"); 
                           if (TryFixMachine(i + 1, newCandidates, swaps)) {
                              swaps.Add(lhs); 
                              swaps.Add(rhs); 
                              return true; 
                           } else {
                              TheMachine.Swap(lhs, rhs); // undo the fix
                           }
                        }
                     }
                     ++tryIdx; 
                  }
                  // while ((tryIdx < regCount) && (tryIdx <= i + 1)); 

                  

               } else if (i >= 2) {
                  // string regName = 'z' + (i - 2).ToString("D2"); 
                  // List<string> safe = TheMachine.GetFeedingRegisters(regName); 
                  // Subtract(candidates, safe); 
               }
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 lh = TheMachine.GetRegisterValue('x'); 
         Int64 rh = TheMachine.GetRegisterValue('y'); 
         Int64 result = TheMachine.GetRegisterValue('z'); 
         Int64 correctResult = lh + rh; 

         int regCount = TheMachine.GetRegisterCount('z'); 
         Int64 mismatch = result ^ correctResult; 

         List<string> swaps = new(); 
         List<string> candidates = TheMachine.GetPossibleRegisters(); 
         TryFixMachine(1, candidates, swaps); 

         swaps.Sort(); 
         Util.WriteLine(string.Join(',', swaps)); 

         Int64 testVal = (1L << (regCount - 1)) - 1; 
         lh = testVal; 
         rh = testVal; 

         TheMachine.SetRegisterValue('x', lh); 
         TheMachine.SetRegisterValue('y', rh); 
         TheMachine.Reset(); 
         result = TheMachine.GetRegisterValue('z');

         int wrongIdx = GetMismatchedIndex(lh + rh, result); 
         Util.WriteLine($"Mismatch at index: {wrongIdx}"); 

         Util.WriteLine(Convert.ToString(result, 2)); 
         Util.WriteLine(Convert.ToString(lh + rh, 2)); 
         return ""; 
      }
   }
}
