using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day03 : Day
   {
      private string InputFile = "2024/inputs/03.txt";

      private string Input = ""; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Input = Util.ReadFileToString(InputFile); 
      }

      int Eval(string mulStr) 
      {
            string exp = mulStr.Substring(4);  
            exp = exp.Remove(exp.Length - 1); 
            (int a, int b) = exp.Split(',').Select(int.Parse).ToArray(); 
            int prod = a * b; 
            return prod;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         string pattern = @"(mul\(\d+,\d+\))"; 

         int sum = 0; 
         foreach (Match match in Regex.Matches(Input, pattern))
         {
            sum += Eval(match.ToString()); 
         }

         return sum.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         string pattern = @"(mul\(\d+,\d+\))|do\(\)|don\'t\(\)"; 

         int sum = 0; 
         bool summing = true; 
         foreach (Match match in Regex.Matches(Input, pattern))
         {
            string exp = match.ToString(); 
            if (exp == "do()") 
            {
               summing = true; 
            }
            else if (exp == "don't()") 
            {
               summing = false; 
            }
            else if (summing) 
            {
               sum += Eval(exp); 
            }
         }

         return sum.ToString(); 
      }
   }
}

