using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day12 : Day
   {
      private string InputFile = "2023/inputs/12.txt";
      
      class Input
      {
         public Input(string line)
         {
            (Sequence, string numStrs) = line.Split(' '); 
            Numbers = numStrs.Split(',').Select(int.Parse).ToList(); 
         }

         public void SomeBullshit()
         {
            Sequence = $"{Sequence}?{Sequence}?{Sequence}?{Sequence}?{Sequence}"; 

            List<int> newNumbers = new List<int>(); 
            newNumbers.AddRange(Numbers);
            newNumbers.AddRange(Numbers);
            newNumbers.AddRange(Numbers);
            newNumbers.AddRange(Numbers);
            newNumbers.AddRange(Numbers);
            Numbers = newNumbers; 

            Cache.Clear(); 
         }

         public string Sequence = ""; 
         List<int> Numbers = new List<int>(); 

         public string GetToken(char c, int length)
         {
            string ret = ""; 
            for (int i = 0; i < length; ++i) {
               ret += c; 
            }

            return ret; 
         }

         private Dictionary<ivec2, Int64> Cache = new Dictionary<ivec2, Int64>(); 

         public Int64 GetAllSequences(string start, int length, int countIdx)
         { 
            if (Numbers.Count <= countIdx) {
               string seq = start + GetToken('.', length); 
               return CanMatch(seq) ? 1 : 0; 
            }

            ivec2 key = new ivec2(length, countIdx); 
            if (Cache.ContainsKey(key)) {
               return Cache[key]; 
            }

            if (countIdx > 0) { 
               start += '.'; 
               length -= 1; 
            }

            // calculate min length we'd need - gives me an early ou
            int minLength = 0; 
            for (int i = countIdx; i < Numbers.Count; ++i) {
               minLength += Numbers[i]; 
            }
            minLength += Numbers.Count - countIdx - 1; 
            if (minLength > length) {
               return 0; // no other possible results
            }

            // okay, my options now are to fill my remaining space any way I can
            int tokenLen = Numbers[countIdx];  
            string token = GetToken('#', tokenLen); 

            Int64 sum = 0; 
            for (int i = 0; i <= length - tokenLen; ++i) {
               string pad = GetToken('.', i); 
               string newStart = start + pad + token; 

               if (CanMatch(newStart)) { 
                  sum += GetAllSequences(newStart, length - tokenLen - i, countIdx + 1); 
               }
            }

            Cache[key] = sum; 
            return sum; 
         }

         public Int64 GetAllSequences()
         {
            int length = Sequence.Length;
            return GetAllSequences("", length, 0); 
         }

         public bool CanMatch(string seq)
         {
            if (seq.Length > Sequence.Length) {
               return false; 
            }

            for (int i = 0; i < seq.Length; ++i) {
               if (Sequence[i] != '?') {
                  if (Sequence[i] != seq[i]) {
                     return false; 
                  }
               }
            }

            return true; 
         }

         public Int64 GetPossibleSequenceCount()
         {
            return GetAllSequences(); 
         }
      }

      List<Input> Inputs = new List<Input>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            Input input = new Input(line); 
            Inputs.Add(input); 
         }   
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 count = 0; 
         foreach (Input input in Inputs) {
            count += input.GetPossibleSequenceCount(); 
         }

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 count = 0; 
         foreach (Input input in Inputs) {
            input.SomeBullshit();
            Int64 lineCount = input.GetPossibleSequenceCount(); 
            count += lineCount; 
            // Util.WriteLine($"+{lineCount} = {count}"); 
         }


         return count.ToString(); 
      }
   }
}
