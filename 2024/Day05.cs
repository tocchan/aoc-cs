using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day05 : Day
   {
      private string InputFile = "2024/inputs/05.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private List<ivec2> Ordering = new(); 
      private List<List<int>> Issues = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int idx = 0; 
         while (!String.IsNullOrEmpty(lines[idx]))
         {
            (int lh, int rh) = lines[idx].Split('|').Select(int.Parse).ToArray(); 
            Ordering.Add(new ivec2(lh, rh)); 
            ++idx; 
         }
         ++idx; 

         while (idx < lines.Count) 
         {
            List<int> issue = lines[idx].Split(',').Select(int.Parse).ToList(); 
            Issues.Add(issue); 
            ++idx; 
         }
      }

      private int CompareOrder(int lh, int rh) 
      {
         foreach (ivec2 o in Ordering) 
         {
            if ((o.x == lh) && (o.y == rh)) {
               return 1; 
            }

            if ((o.x == rh) && (o.y == lh)) {
               return -1; 
            }
         }
         
         // not found it is correct
         return 0; 
      }

      private bool IsInCorrectPlace(List<int> issue, int idx) 
      {
         for (int i = 0; i < idx; ++i) 
         {
            if (CompareOrder(issue[i], issue[idx]) < 0) {
               return false; 
            }
         }

         for (int i = idx + 1; i < issue.Count; ++i) 
         {
            if (CompareOrder(issue[idx], issue[i]) < 0) 
            {
               return false; 
            }
         }

         return true; 
      }

      private bool IsInCorrectOrder(List<int> issue) 
      {
         for (int i = 0; i < issue.Count; ++i) 
         {
            if (!IsInCorrectPlace(issue, i)) 
            {
               return false; 
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int sum = 0; 
         foreach (var issue in Issues) 
         {
            if (IsInCorrectOrder(issue)) 
            {
               sum += issue[issue.Count / 2]; 
            }
         }
         
         return sum.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int sum = 0; 
         foreach (var issue in Issues) 
         {
            if (!IsInCorrectOrder(issue)) 
            {
               issue.Sort((int lh, int rh) => CompareOrder(lh, rh)); 
               sum += issue[issue.Count / 2]; 
            }

         }
         
         return sum.ToString(); 
      }
   }
}
