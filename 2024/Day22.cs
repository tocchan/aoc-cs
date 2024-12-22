using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day22 : Day
   {
      private string InputFile = "2024/inputs/22.txt";


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

      public Dictionary<int, int> GetPriceLookup(Int64 code, int iterations)
      {
         Dictionary<int, int> lookup = new(); 

         Int64 key = 0; 
         Int64 prev = code % 10; 
         for (int i = 0; i < iterations; ++i) {
            code = ComputeNext(code); 
            Int64 next = code % 10; 
            Int64 diff = next - prev; 
            prev = next; 

            key = ((key & 0x00ffffff) << 8) | (diff + 10); // make sure it is positive
            if (i >= 3) {
               int actualKey = (int) key; 
               if (!lookup.ContainsKey(actualKey)) {
                  lookup.Add(actualKey, (int) next); 
               }
            }
         }

         return lookup; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         List<Dictionary<int, int>> lookups = new(); 
         foreach (Int64 code in Inputs) {
            lookups.Add( GetPriceLookup(code, 2000) ); 
         }

         HashSet<int> uniqueKeys = new(); 
         foreach (Dictionary<int, int> lookup in lookups) {
            foreach (int key in lookup.Keys) {
               uniqueKeys.Add(key); 
            }
         }

         // -2, 1, -1, 3
         // int testKey = (8 << 24) | (11 << 16) | (9 << 8) | 13; 

         // -1 -1 0 2
         // int testKey = (9 << 24) | (9 << 16) | (10 << 8) | 12; 
         // Util.WriteLine(lookups[3][testKey].ToString()); 

         int bestPrice = 0; 
         foreach (int key in uniqueKeys) {
            int price = 0; 
            foreach (Dictionary<int, int> lookup in lookups) {
               int cost = 0; 
               if (lookup.TryGetValue(key, out cost)) {
                  price += cost; 
               }
            }

            bestPrice = Math.Max(price, bestPrice); 
         }

         return bestPrice.ToString(); 
      }
   }
}
