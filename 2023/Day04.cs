using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day04 : Day
   {
      private string InputFile = "2023/inputs/04.txt";
    
      class Game
      {
         public void Init(string line)
         {
            string numbers = line.Split(':')[1];
            (string winning, string mine) = numbers.Split('|'); 
            var winningNumbers = winning.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(); 
            var myNumbers = mine.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(); 

            foreach (int n in winningNumbers) {
               WinningNumbers.Add(n); 
            }
            foreach (int n in myNumbers) {
               MyNumbers.Add(n);
            }
         }

         public int CountMyWinners()
         {
            return MyNumbers.Intersect(WinningNumbers).Count(); 
         }

         public HashSet<int> WinningNumbers = new HashSet<int>();
         public List<int> MyNumbers = new List<int>(); 
      }

      List<Game> Games = new List<Game>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            Game game = new Game(); 
            game.Init(line); 
            Games.Add(game); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int total = 0; 
         foreach (Game g in Games) {
            int winners = g.CountMyWinners();
            if (winners > 0) {
               int points = (1 << (winners - 1)); 
               total += points ;
            }
         }
         return total.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int[] copies = new int[Games.Count];
         copies.SetAll(1); 

         for (int i = 0; i < Games.Count; ++i) {  
            Game g = Games[i]; 
            int winners = g.CountMyWinners();
            int endIndex = Math.Min(Games.Count, i + 1 + winners); 
            for (int j = i + 1; j < endIndex; ++j) {
               copies[j] += copies[i]; 
            }
         }

         return copies.Sum().ToString();
      }
   }
}
