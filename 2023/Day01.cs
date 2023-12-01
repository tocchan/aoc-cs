using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day01 : Day
   {
      private string InputFile = "2023/inputs/01.txt";
      private List<string> Lines = new List<string>();  

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Lines = Util.ReadFileToLines(InputFile);
      }

      private bool IsDigit(char c)
      {
         return (c >= '0') && (c <= '9'); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int total = 0; 
         foreach (var line in Lines) {
            char first = line.First(Util.IsDigit); 
            char last = line.Last(Util.IsDigit); 

            string number = first.ToString() + last.ToString(); 
            int num = int.Parse(number); 
            total += num; 
         }
         return total.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         string[] things = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};

         int total = 0;
         foreach (var line in Lines) {
            int minIndex = line.Length;
            int maxIndex = -1;
            int minValue = 0;
            int maxValue = 0;
            for (int i = 0; i < things.Length; ++i) {
               string thing = things[i];
               int newMin = line.IndexOf(thing);
               int newMax = line.LastIndexOf(thing);

               if ((newMin > -1) && (newMin < minIndex)) {
                  minIndex = newMin;
                  minValue = i % 10;
               }

               if (newMax > maxIndex) {
                  maxIndex = newMax;
                  maxValue = i % 10;
               }
            }
            string valueString = minValue.ToString() + maxValue.ToString();
            int value = int.Parse(valueString);
            total += value; 
         }

         return total.ToString(); 
      }
   }
}

