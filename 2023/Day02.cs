using AoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day02 : Day
   {
      private string InputFile = "2023/inputs/02.txt";
      private List<string> Games = new List<string>();  

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Games = Util.ReadFileToLines(InputFile);
      }

      //----------------------------------------------------------------------------------------------
      private Dictionary<string, int>[] ParseTurns(string turns)
      {
         List<Dictionary<string, int>> result = new List<Dictionary<string, int>>(); 

         foreach (string turn in turns.Split(';')) {
            Dictionary<string, int> dict = new Dictionary<string, int>(); 
            foreach (string marbles in turn.Split(',')) {
               (string countStr, string key) = marbles.Trim().Split(' ');
               dict[key] = int.Parse(countStr); 
            }

            result.Add(dict); 
         }

         return result.ToArray(); 
      }

      //----------------------------------------------------------------------------------------------
      private bool IsPossible(Dictionary<string, int> turn, Dictionary<string, int> possible)
      {
         foreach ((var color, var count) in turn) {
            if (!possible.ContainsKey(color)) {
               return false; 
            }

            if (count > possible[color]) {
               return false; 
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      private bool IsGamePossible(string game, Dictionary<string, int> possible)
      {
         Dictionary<string, int>[] pulls = ParseTurns(game);
         foreach (var turn in pulls) {
            if (!IsPossible(turn, possible)) {
               return false; 
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Dictionary<string,int> possible = new Dictionary<string, int>();
         possible["red"] = 12; 
         possible["green"] = 13; 
         possible["blue"] = 14; 

         int total = 0; 
         foreach (var game in Games)
         {
            (string gameNum, string turns) = game.Split(':'); 
            int gameNumber = int.Parse(gameNum.Remove(0, 5)); 

            if (IsGamePossible(turns, possible)) {
               total += gameNumber; 
            }
         }
         return total.ToString();  
      }

      //----------------------------------------------------------------------------------------------
      private int GetMinPower(string turnString)
      {
         Dictionary<string, int> minTurns = new Dictionary<string, int>(); 
         var turns = ParseTurns(turnString);
         
         foreach (var turn in turns) {
            foreach ((var color, var count) in turn) {
               minTurns[color] = Math.Max(minTurns.GetValueOrDefault(color, 0), count); 
            }
         }

         if (minTurns.Count == 0) {
            return 0; 
         } 

         int product = 1; 
         foreach ((_, var count) in minTurns) {
            product *= count; 
         }

         return product; 

      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int total = 0; 
         foreach (var game in Games) {
            string turns = game.Split(':')[1];
            
            int power = GetMinPower(turns);
            total += power; 
         }
         return total.ToString();
      }
   }
}

