using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day19 : Day
   {
      private string InputFile = "2023/inputs/19.txt";
      
      enum eCompareOp
      {
         Always, 
         Greater, 
         Less
      }

      static internal int PartToIndex(char c) => c switch {
         'x' => 0,
         'm' => 1,
         'a' => 2,
         's' => 3, 
         _ => 4
      };

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Condition
      {
         public int Part; 
         public int Value; 
         public eCompareOp CompareOp; 
         public string WorkflowId; 

         public Condition(string str)
         {
            if (str.Contains(':')) {
               string[] parts = str.Split(':'); 

               Part = PartToIndex(parts[0][0]); 
               CompareOp = parts[0][1] == '>'
                  ? eCompareOp.Greater
                  : eCompareOp.Less; 
               Value = int.Parse(parts[0].Substring(2)); 
               WorkflowId = parts[1]; 

            } else {
               Part = 0; 
               Value = 0; 
               CompareOp = eCompareOp.Always; 
               WorkflowId = str; 
            }
         }

         public bool Passes(Part p)
         {
            if (CompareOp == eCompareOp.Always) {
               return true; 
            } else if (CompareOp == eCompareOp.Greater) {
               return p.Values[Part] > Value; 
            } else {
               return p.Values[Part] < Value; 
            }
         }

         public (IntRange[], IntRange[]?) Split(IntRange[] ranges)
         {
            if (CompareOp == eCompareOp.Always) {
               return (ranges, null); 
            }

            IntRange[] lh = new IntRange[4];
            IntRange[] rh = new IntRange[4]; 
            for (int i = 0; i < 4; ++i) {
               lh[i] = new IntRange(ranges[i]);
               rh[i] = new IntRange(ranges[i]);
            }

            if (CompareOp == eCompareOp.Greater) {
               lh[Part].Min = Value + 1;
               rh[Part].Max = Value; 
            } else {
               lh[Part].Max = Value - 1;
               rh[Part].Min = Value; 
            }

            return (lh, rh); 
         }
      }

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Workflow
      {
         public string Id; 
         public List<Condition> Conditions; 

         public Workflow(string line)
         {
            (Id, string conditions) = line.Split('{'); 
            conditions = conditions.Substring(0, conditions.Length - 1);
            string[] cond = conditions.Split(','); 

            Conditions = new(); 
            foreach (string c in cond) {
               Condition condition = new Condition(c); 
               Conditions.Add(condition); 
            }
         }

         public string GetNext(Part part)
         {
            foreach (Condition c in Conditions) {
               if (c.Passes(part)) {
                  return c.WorkflowId; 
               }
            }

            return Conditions.Last().WorkflowId; 
         }
      }

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Part
      {
         public int[] Values = new int[4]; 

         public Part(string line)
         {
            line = line.Substring(1, line.Length - 2);
            string[] parts = line.Split(',');

            for (int i = 0; i < 4; ++i) {
               Values[i] = int.Parse(parts[i].Split('=')[1]);
            }
         }

         public int GetRating()
         {
            return Values.Sum(); 
         }
      }

      Dictionary<string, Workflow> Workflows = new(); 
      List<Part> Parts = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int i = 0; 
         for (i = 0; i < lines.Count; ++i) {
            if (string.IsNullOrEmpty(lines[i])) {
               break; 
            } else {
               Workflow workflow = new Workflow(lines[i]); 
               Workflows.Add(workflow.Id, workflow); 
            }
         }

         for (i = i + 1; i < lines.Count; ++i) {
            string line = lines[i]; 
            Part part = new Part(line); 
            Parts.Add(part); 
         }
      }

      //----------------------------------------------------------------------------------------------
      bool IsPartAccepted(Part part)
      {
         Workflow? cur = Workflows["in"]; 
         while (cur != null) {
            string id = cur.GetNext(part); 
            if (id == "A") {
               return true; 
            } else if (id == "R") {
               return false; 
            } else {
               cur = Workflows[id]; 
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int rating = 0; 
         foreach (Part part in Parts) {
            if (IsPartAccepted(part)) {
               rating += part.GetRating(); 
            }
         }
         return rating.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      Int64 CountRanges(IntRange[] ranges)
      {
         Int64 count = 0; 
         foreach (IntRange r in ranges) {
            if (r.IsValid()) {
               if (count == 0) {
                  count = 1; 
               }
               count *= r.Max - r.Min + 1; 
            }
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      Int64 CountAccepted(IntRange[] initialRanges)
      {
         Int64 accepted = 0; 
         Workflow start = Workflows["in"]; 

         Stack<(Workflow, IntRange[])> todo = new(); 
         todo.Push((start, initialRanges)); 

         while (todo.Count > 0) {
            (Workflow workflow, IntRange[] ranges) = todo.Pop(); 

            foreach (Condition c in workflow.Conditions) {
               (IntRange[] lh, IntRange[]? rh) = c.Split(ranges); 

               if (c.WorkflowId == "A") {
                  accepted += CountRanges(lh); 
               } else if (c.WorkflowId == "R") {
                  // nothing, done with them
               } else {
                  todo.Push((Workflows[c.WorkflowId], lh)); 
               }

               // if there is remainer, it goes onto the next set
               if (rh != null) {
                  ranges = rh; 
               }
            }
         }
         
         return accepted; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntRange[] ranges = new IntRange[4]; 
         for (int i = 0; i < 4; ++i) {
            ranges[i] = new IntRange(1, 4000); 
         }

         Int64 accepted = CountAccepted(ranges); 
         return accepted.ToString(); 
      }
   }
}
