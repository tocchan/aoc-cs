using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day07 : Day
   {
      private string InputFile = "2023/inputs/07.txt";
      
      internal class Hand
      {
         public string Cards = ""; 
         public string CompareCards = ""; 
         public int Wager = 0; 
         public int Rank = 0; 

         public Hand(string line)
         {
            (Cards, string wagerStr) = line.Split(' '); 
            Wager = int.Parse(wagerStr); 

            // (T, J, Q, K, A)
            // (A, B, C, D, E)
            CompareCards = Cards.Replace('A', 'E')
               .Replace('T', 'A')
               .Replace('J', 'B')
               .Replace('Q', 'C')
               .Replace('K', 'D');
         }

         public void ApplyJokers()
         {
            Rank = 0; // force a recompute 

            CompareCards = CompareCards.Replace('B', '1'); // Worst for tie breaks

            // break into buckets
            Dictionary<char, int> counts = new Dictionary<char, int>();
            foreach (char c in CompareCards) {
               int newCount = counts.GetValueOrDefault(c, 0) + 1;
               counts[c] = newCount;
            }

            // always best to just apply joker to the best bucket
            char bestChar = ' '; 
            int bestCount = 0; 

            foreach ((char c, int v) in counts) {
               // don't count the jokers in this (in the case of 5 jokers, nothing will be replaced, 5 of a kind)
               if (c == '1') {
                  continue; 
               }

               if (v > bestCount) {
                  bestCount = v; 
                  bestChar = c; 
               } else if (v == bestCount) {
                  if (c > bestChar) {
                     bestChar = c;
                  }
               }
            }

            if (bestCount > 0) {
               Cards = CompareCards.Replace('1', bestChar); // destructive, but fast
            }
         }

         public int GetRank()
         {
            if (Rank == 0) {
               Dictionary<char, int> counts = new Dictionary<char, int>(); 
               foreach (char c in Cards) {
                  int newCount = counts.GetValueOrDefault(c, 0) + 1;
                  counts[c] = newCount; 
               }

               if (counts.Count == 1) {
                  Rank = 10; 
               } else if (counts.Count == 2) {
                  if (counts.ContainsValue(4)) {
                     Rank = 9; 
                  } else {
                     Rank = 8; // full house
                  }
               } else if (counts.ContainsValue(3)) {
                  Rank = 7; // three of a kind
               } else if (counts.Count == 3) {
                  Rank = 6; // two parks 
               } else if (counts.Count == 4) {
                  Rank = 5; // one pair
               } else {
                  Rank = 1; // high card
               }
            }

            return Rank; 
         }



         public static int Compare(Hand lh, Hand rh)
         {
            int lhRank = lh.GetRank(); 
            int rhRank = rh.GetRank(); 

            if (lhRank == rhRank) {
               return -string.Compare(lh.CompareCards, rh.CompareCards);
            } else {
               return rhRank - lhRank; 
            }
         }
      }

      private List<Hand> Hands = new List<Hand>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (var line in lines) {
            Hand input = new Hand(line);
            Hands.Add(input); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Hands.Sort(Hand.Compare);
         
         int total = 0; 
         int rank = Hands.Count; 
         foreach (Hand hand in Hands) {
            // Util.WriteLine($"{rank}: {hand.Cards} - {hand.Wager}"); 
            total += rank * hand.Wager; 
            --rank; 
         }

         return total.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         foreach (Hand hand in Hands) {
            hand.ApplyJokers(); 
         }

         Hands.Sort(Hand.Compare);

         int total = 0;
         int rank = Hands.Count;
         foreach (Hand hand in Hands) {
            // Util.WriteLine($"{rank}: {hand.Cards} - {hand.CompareCards} - {hand.Wager}"); 
            total += rank * hand.Wager;
            --rank;
         }

         return total.ToString(); 
      }
   }
}
