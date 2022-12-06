using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day06 : Day
   {
      private string InputFile = "2022/inputs/06.txt";
      
      private string Input = "";

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Input = Util.ReadFileToLines(InputFile)[0]; 
      }

      //----------------------------------------------------------------------------------------------
      private bool IsUnique(string s, int startIdx, int len)
      {
         bool[] chars = new bool[26]; 
         for (int i = 0; i < len; ++i) {
            int idx = s[startIdx + i] - 'a'; 
            if (chars[idx]) {
               return false; 
            }
            chars[idx] = true; 
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         for (int i = 0; i < Input.Length - 4; ++i) {
            if (IsUnique(Input, i, 4)) {
               return (i + 4).ToString(); 
            }
         }

         return ""; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         for (int i = 0; i < Input.Length - 14; ++i) {
            if (IsUnique(Input, i, 14)) {
               return (i + 14).ToString(); 
            }
         }

         return ""; 
      }
   }
}
