using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day22 : Day
   {
      private string InputFile = "2024/inputs/22d.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      private List<Int64> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Inputs = Util.ReadFileToLines(InputFile).Select(Int64.Parse).ToList(); 
      }

      Int64 ClampToInt(Int64 v)
      {
         return Math.Clamp(v, int.MinValue, int.MaxValue); 
      }

      Int64 Mix(Int64 lh, Int64 rh)
      {
         return lh ^ rh; 
      }

      Int64 Prune(Int64 v) 
      {
         return v % 16777216; 
      }

      Int64 ComputeNext(Int64 code) 
      {
         Int64 newCode = Mix(code * 64, code); 
         newCode = Prune(newCode); 
         newCode = Mix(newCode / 32, newCode); 
         newCode = Prune(newCode); 
         newCode = Mix(newCode * 2048, newCode); 
         return Prune(newCode); 
      }

      Int64 ComputeNext(Int64 code, Int64 count) 
      {
         for (int i = 0; i < count; ++i) {
            code = ComputeNext(code); 
         }

         return code; 
      }

      

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 sum = 0; 
         foreach (Int64 code in Inputs) {
            sum += ComputeNext(code, 2000); 
         }
         
         return sum.ToString(); 
      }

//----------------------------------------------------------------------------------------------
      string GetSequence(Int64 code, Int64 count, out string priceSeq) 
      {
         List<char> costs = new(); 
         Int64 prev = code % 10; 

         priceSeq = ""; 
         priceSeq += (char)('A' + prev); 

         for (int i = 0; i <= count; ++i) {

            code = ComputeNext(code); 
            Int64 next = code % 10; 
            Int64 diff = next - prev; 
            costs.Add((char)('A' + diff)); 

            prev = next; 
            priceSeq += (char)('A' + prev); 
         }

         return new string(costs.ToArray()); 
      }

      //----------------------------------------------------------------------------------------------
      public Int64 GetPrice(string price, string seq, string subseq) 
      {
         int loc = seq.IndexOf(subseq); 
         if (loc >= 0) {
            return price[loc + 4] - 'A'; 
         }

         return 0; 
      }

      public Int64 GetTotalPrice(List<string> prices, List<string> sequences, string seq) 
      {
         Int64 price = 0; 
         for (int i = 0; i < sequences.Count; ++i) {
            price += GetPrice(prices[i], sequences[i], seq); 
         }

         return price; 
      }

      //----------------------------------------------------------------------------------------------
      public void GetAllSubsequences(HashSet<string> subseq, string seq)
      {
         for (int i = 1; i < seq.Length - 4; ++i) {
            subseq.Add(seq.Substring(i, 4)); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         List<string> sequences = new(); 
         List<string> prices = new(); 
         HashSet<string> subsequenceSet = new(); 

         foreach (Int64 code in Inputs) {
            string price; 
            sequences.Add(GetSequence(code, 2000, out price)); 
            prices.Add(price); 
         }

         /*
         foreach (string seq in sequences) {
            GetAllSubsequences(subsequenceSet, seq); 
         }
         */
         GetAllSubsequences(subsequenceSet, sequences[0]); 

         // best seq was @CAA for the real input?

         // ">?@A" // ascii values before A for testing
         string p;
         string s = GetSequence(123, 10, out p); 
         Int64 tp = GetPrice(p, s, "@@AC"); 
         Util.WriteLine($"{s} - {p} - {tp}"); 

         string testSeq = "?B@D"; // -2 1 -1 3
         for (int testIdx = 0; testIdx < prices.Count; ++testIdx) {
            Int64 testVal = GetPrice(prices[testIdx], sequences[testIdx], testSeq); 
            Int64 testPrice = GetTotalPrice(prices, sequences, testSeq); 
            Util.WriteLine($"{testPrice} - {testVal}"); 
         }

         Int64 bestPrice = 0; 
         string bestSeq = ""; 

         int working = 0; 
         foreach (string subseq in subsequenceSet) {
            Int64 newPrice = GetTotalPrice(prices, sequences, subseq); 
            if (newPrice > bestPrice) {
               bestPrice = newPrice; 
               bestSeq = subseq; 
            }

            ++working; 
            if ((working % 1000) == 0) {
               Util.WriteLine($"{working} / {subsequenceSet.Count}"); 
            }
         }

         Util.WriteLine($"BestSeq: {bestSeq}"); 

         return bestPrice.ToString(); 
      }
   }
}
