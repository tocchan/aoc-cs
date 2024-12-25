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
               result.Add(name);
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

         public List<string> GetRootDependencies(string reg) 
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
                  toCheck.Push(next); 
               } else {
                  results.Add(circuit.LH.Name); 
               }

               if (Circuits.TryGetValue(circuit.RH.Name, out next)) {
                  toCheck.Push(next); 
               } else {
                  results.Add(circuit.RH.Name); 
               }
            }

            return results.ToList(); 
         }

         public bool Swap(string regA, string regB)
         {
            if (regA == regB) {
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

            if (CheckForLoop(regA) || CheckForLoop(regB)) {
               Swap(regA, regB); 
               return false; 
            }

            return true; 
         }
         
         bool CheckForLoop(string reg) 
         {
            Register top = FindOrAddRegister(reg); 
            Stack<Register> toCheck = new(); 

            Circuit? circuit;
            if (Circuits.TryGetValue(top.Name, out circuit)) {
               toCheck.Push(circuit.LH); 
               toCheck.Push(circuit.RH);
            }

            while (toCheck.Count > 0) {
               Register cur = toCheck.Pop();
               if (cur == top) {
                  return true; 
               }

               if (Circuits.TryGetValue(cur.Name, out circuit)) {
                  toCheck.Push(circuit.LH); 
                  toCheck.Push(circuit.RH);
               }
            }

            return false; 
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

      Int64 TryFix(string lh, string rh, Int64 testVal) 
      {
         // now, try to swap until this succeeds?
         if (!TheMachine.Swap(lh, rh)) {
            return -1; 
         }

         TheMachine.Reset(); 
         Int64 newVal = TheMachine.GetRegisterValue('z');
         if (newVal == testVal) {
            return newVal;
         } 

         return newVal;
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

      string GetAnswerString(List<string> swaps) 
      {
         return string.Join(',', swaps); 
      }

      List<string> FilterToLevel(List<string> candidates, int level)
      {
         // get rid of all options that contain a higher root value then the level we're looking for
         List<string> result = new(); 
         foreach (string c in candidates) {

            if (c.StartsWith('z')) {
               int cLevel = int.Parse(c.Substring(1)); 
               if (cLevel >= level) {
                  result.Add(c); // any z higher than me is a poptential candidate
               }
               
               continue; // can't swap with something that has been determined (lower)
            }

            List<string> roots = TheMachine.GetRootDependencies(c); 
            bool discard = false; 
            for (int i = 0; (i <= level) && !discard; ++i) {
               discard = false; 
               foreach (string root in roots) {
                  int rootLevel = int.Parse(root.Substring(1)); 
                  if (root.StartsWith('z')) {
                     discard = rootLevel < level; 
                  } else {
                     discard = rootLevel > level; 
                  }

                  if (discard) {
                     break; 
                  }
               }
            }

            if (!discard) {
               result.Add(c); 
            }
         }

         // also, save to throw out anything that a lower level is dependent on, at risk of breaking it
         if (level > 0) {
            string regName = 'z' + (level - 1).ToString("D2"); 
            List<string> protMe = TheMachine.GetFeedingRegisters(regName); 
            foreach (string p in protMe) {
               candidates.Remove(p); 
            }
         }

         return result;
      }

      bool TryFixMachine(int startIdx, List<string> prevCandidates, List<string> swaps, int depth = 0, int highestBit = -1)
      {
         int regCount = 45; 
         List<string> candidates = new List<string>(prevCandidates); 

         if (startIdx <= highestBit) {
            return false; 
         }

         for (int i = startIdx; i < regCount; ++i) {

            Int64 xval = (1L << i) - 1; 
            Int64 yval = xval & 0x7AAAAAAAAAAAAAAAL; 
            xval = xval & 0x555555555555555L;
            Int64 expected = xval + yval; 

            TheMachine.Reset(); 
            TheMachine.SetRegisterValue('x', xval); 
            TheMachine.SetRegisterValue('y', yval); 
            Int64 result = TheMachine.GetRegisterValue('z'); 

            if (result != expected) {
               if (depth >= 4) {
                  return false; // too many fixes - abort
               }

               int mismatchIdx = GetMismatchedIndex(result, expected); 
               if (mismatchIdx <= highestBit) {
                  return false; 
               }

               string regName = 'z' + mismatchIdx.ToString("D2"); 
               List<string> used = TheMachine.GetFeedingRegisters(regName); 


               // Util.WriteLine($"broke at {i}");
               // Util.WriteLine("exp: " + Convert.ToString(expected, 2).PadLeft(24)); 
               // Util.WriteLine("res: " + Convert.ToString(result, 2).PadLeft(24)); 
               List<string> filtered = FilterToLevel(candidates, mismatchIdx); 
               List<string> search = used.Intersect(filtered).ToList(); 

               if (regName == "z23") {
                  candidates.Sort(); 
                  filtered.Sort(); 
                  Util.WriteLine("what is going on"); 
               }

               Subtract(filtered, search); 

               List<(string, string)> combos = Mix(search, filtered); 

               int comboIdx = 0; 
               foreach ((string lh, string rh) in combos) {
                  ++comboIdx; 
                  candidates.Remove(lh); 
                  candidates.Remove(rh); 

                  Int64 newResult = TryFix(lh, rh, expected);

                  // confirm this fix didn't end up breaking sometihng earliar on
                  Int64 minmask = (1L << (mismatchIdx + 1)) - 1; 
                  bool wasFixed = false;
                  if (newResult >= 0) {
                     wasFixed = true; 

                     Int64 mask = (1L << (mismatchIdx + 1)) - 1; 
                     for (int ci = 0; ci <= mismatchIdx; ++ci) {
                        Int64 cval = (1L << (ci + 1)) - 1; 
                        Int64 cx = xval & cval;
                        Int64 cy = yval & cval; 
                        TheMachine.SetRegisterValue('x', cx); 
                        TheMachine.SetRegisterValue('y', cy); 
                        TheMachine.Reset(); 
                        Int64 cans = TheMachine.GetRegisterValue('z'); 

                        if ((cans & mask) != ((cx + cy) & mask)) {
                           wasFixed = false; 
                           break; 
                        }
                     }
                  }

                  if (wasFixed) {
                     if (lh == "z23") {
                        Util.WriteLine("what is going on"); 
                     }

                     if (depth < 1) {
                        Util.WriteLine($"{depth}: ({comboIdx}/{combos.Count} - attempting swap {lh} <-> {rh} at bit {mismatchIdx}"); 
                     }
                     
                     Subtract(candidates, used); 
                     if (TryFixMachine(mismatchIdx + 1, candidates, swaps, depth + 1, Math.Max(mismatchIdx, highestBit))) {
                        Util.WriteLine($"Adding Swap: ({lh},{rh})");
                        swaps.Add(lh); 
                        swaps.Add(rh); 
                        return true; 
                     } else {
                        candidates.AddRange(used); 
                     }
                  } 
                  
                  if (newResult >= 0) {
                     // try again at this level, do an additional swap
                     /*
                     if (TryFixMachine(i, candidates, swaps, depth + 1, combos)) {
                        swaps.Add(lh); 
                        swaps.Add(rh); 
                        return true; 
                     }
                     */

                     TheMachine.Swap(lh, rh); // fixup the machine
                  }

                  candidates.Add(lh); 
                  candidates.Add(rh); 
               }      

               // Util.WriteLine($"Failed at {i}"); 
               return false; // nothing we did could fix it. 

            } else if (highestBit >= 0) {
               // string regName = 'z' + highestBit.ToString("D2"); 
               // List<string> used = TheMachine.GetFeedingRegisters(regName); 
               // Subtract(candidates, used); 
            }
         }

         return CheckResult(); ; 
      }

      public bool CheckResult()
      {
         TheMachine.SetRegisterValue('x', X); 
         TheMachine.SetRegisterValue('y', Y); 
         TheMachine.Reset(); 
         Int64 res = TheMachine.GetRegisterValue('z'); 

         return res == X + Y; 
      }

      Int64 X;
      Int64 Y; 

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 lh = TheMachine.GetRegisterValue('x'); 
         Int64 rh = TheMachine.GetRegisterValue('y'); 
         X = lh; 
         Y = rh; 

         int regCount = TheMachine.GetRegisterCount('z'); 

         // Int64 testVal = (1L << (regCount - 1)) - 1; 
         // lh = testVal; 
         // rh = testVal; 

         List<string> swaps = new(); 
         List<string> candidates = TheMachine.GetPossibleRegisters(); 

         /*
         TheMachine.Swap("svm", "nbc"); 
         TheMachine.Swap("z15", "kqk"); 
         TheMachine.Swap("z23", "cgq"); 
         TheMachine.Swap("z39", "fnr"); 
         */
         
         TryFixMachine(0, candidates, swaps); 
         TryFixMachine(0, candidates, swaps); 

         swaps.Sort(); 
         string ans = string.Join(',', swaps); 
        
         TheMachine.Reset(); 
         TheMachine.SetRegisterValue('x', lh); 
         TheMachine.SetRegisterValue('y', rh); 
         Int64 result = TheMachine.GetRegisterValue('z');

         Util.WriteLine(Convert.ToString(result, 2)); 
         Util.WriteLine(Convert.ToString(lh + rh, 2)); 
         Util.WriteLine(Convert.ToString(result ^ (lh + rh), 2)); 

         return ans; 
      }
   }
}
