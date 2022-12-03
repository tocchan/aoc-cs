using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day03 : Day
   {
      private string InputFile = "2022/inputs/03.txt";
      private List<string> Lines = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Lines = Util.ReadFileToLines(InputFile);
      }

      //----------------------------------------------------------------------------------------------
      public char GetCommon(string a, string b)
      {
         return a.Intersect(b).First();
      }

      //----------------------------------------------------------------------------------------------
      public int GetPriority( char c )
      {
         if (c >= 'a') {
            return c - 'a' + 1; 
         }

         return c - 'A' + 27; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int priority = 0; 
         foreach (string line in Lines) {
            int halfLen = line.Length / 2; 
            string first = line.Substring(0, halfLen); 
            string second = line.Substring(halfLen); 
            char common = GetCommon(first, second); 
            int pri = GetPriority(common); 
            priority += pri; 
         }

         return priority.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public char GetCommon(string a, string b, string c)
      {
         return a.Intersect(b).Intersect(c).First(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int priority = 0; 
         for (int i = 0; i < Lines.Count; i += 3) {
            string first = Lines[i + 0]; 
            string second = Lines[i + 1];
            string third = Lines[i + 2]; 
            char common = GetCommon(first, second, third); 
            int pri = GetPriority(common); 
            priority += pri; 
         }

         return priority.ToString(); 
      }
   }
}
