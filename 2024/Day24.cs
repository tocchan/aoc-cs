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

            results.Add(reg); 
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

      bool TryFix(string lh, string rh, Int64 testVal, int tryIdx) 
      {
         // now, try to swap until this succeeds?
         if (!TheMachine.Swap(lh, rh)) {
            return false; 
         }

         TheMachine.Reset(); 
         Int64 newVal = TheMachine.GetRegisterValue('z');
         if (((newVal ^ testVal) & (1L << tryIdx)) == 0) {
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

      List<string> GetPotentialProblemChildren(Int64 diff) 
      {
         HashSet<string> result = new(); 

         for (int i = 0; i < 46; ++i) {
            if ((diff & (1L << i)) != 0) {
               string regName = 'z' + i.ToString("D2"); 
               List<string> feeders = TheMachine.GetFeedingRegisters(regName); 
               foreach (string f in feeders) {
                  result.Add(f); 
               }
            }
         }

         return result.ToList(); 
      }

      bool TryFixMachine(List<string> candidates, List<string> swaps, Int64 lh, Int64 rh, int depth = 0)
      {
         int regCount = TheMachine.GetRegisterCount('z'); 

         // get first error
         TheMachine.SetRegisterValue('x', lh); 
         TheMachine.SetRegisterValue('y', rh); 
         Int64 zVal = TheMachine.GetRegisterValue('z'); 
         Int64 expected = lh + rh; 

         if (expected != zVal) {
            if (depth >= 4) { 
               return false; // know answer will be at most 4 pairs
            }

            Int64 diff = expected ^ zVal; 
            List<string> swapTargets = GetPotentialProblemChildren(diff).Intersect(candidates).ToList(); 

            int tryIdx = GetMismatchedIndex(expected, zVal); 
            // do 
            {
               string regName = 'z' + (tryIdx).ToString("D2"); 

               List<string> possibleRegisters = TheMachine.GetFeedingRegisters(regName); 
               possibleRegisters = possibleRegisters.Intersect(swapTargets).ToList();

               List<(string, string)> combos = Mix(possibleRegisters, swapTargets); 
               foreach ((string lhs, string rhs) in combos) {
                  if (TryFix(lhs, rhs, expected, tryIdx)) {

                     swapTargets.Remove(lhs); 
                     swapTargets.Remove(rhs); 

                     Util.WriteLine($"Attempting swap at {tryIdx} of {lhs} <-> {rhs}"); 
                     if (TryFixMachine(swapTargets, swaps, lh, rh, depth + 1)) {
                        swaps.Add(lhs); 
                        swaps.Add(rhs); 
                        return true; 
                     } else {
                        TheMachine.Swap(lhs, rhs); // undo the fix
                        swapTargets.Add(lhs); 
                        swapTargets.Add(rhs); 
                     }
                  }
               }
               ++tryIdx; 
            }

            return false; 
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 lh = TheMachine.GetRegisterValue('x'); 
         Int64 rh = TheMachine.GetRegisterValue('y'); 

         int regCount = TheMachine.GetRegisterCount('z'); 

         Int64 testVal = (1L << (regCount - 1)) - 1; 
         lh = testVal; 
         rh = testVal; 

         TheMachine.SetRegisterValue('x', lh); 
         TheMachine.SetRegisterValue('y', rh); 

         List<string> swaps = new(); 
         List<string> candidates = TheMachine.GetPossibleRegisters(); 
         TryFixMachine(candidates, swaps, lh, rh); 

         swaps.Sort(); 
         string ans = string.Join(',', swaps); 
        
         TheMachine.Reset(); 
         Int64 result = TheMachine.GetRegisterValue('z');

         Util.WriteLine(Convert.ToString(result, 2)); 
         Util.WriteLine(Convert.ToString(lh + rh, 2)); 
         return ans; 
      }
   }
}
