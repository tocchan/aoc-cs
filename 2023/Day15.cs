using AoC;
using AoC2022;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   class Box
   {
      class Lens
      { 
         public string label; 
         public int size; 
      }

      LinkedList<Lens> Lenses = new LinkedList<Lens>(); 

      LinkedListNode<Lens>? Find(string label)
      {
         for (var iter = Lenses.First; iter != null; iter = iter.Next) {
            if (iter.Value.label == label) {
               return iter; 
            }
         }

         return null; 
      }

      public void AddLens(string label, int size)
      {
         LinkedListNode<Lens>? iter = Find(label);
         if (iter != null) {
            iter.Value.size = size; 
         } else {
            Lens lens = new Lens(); 
            lens.label = label ;
            lens.size = size; 
            Lenses.AddLast(lens); 
         }
         
      }

      public void RemoveLens(string label)
      {
         LinkedListNode<Lens>? iter = Find(label); 
         if (iter != null) {
            Lenses.Remove(iter); 
         }
      }

      public int Score(int boxNumber)
      {
         int score = 0; 

         int lensIndex = 0; 
         foreach (Lens lens in Lenses) {
            ++lensIndex; 
            score += boxNumber * lensIndex * lens.size; 
         }

         return score; 
      }
   }

   internal class Day15 : Day
   {
      private string InputFile = "2023/inputs/15.txt";

      List<string> Tokens = new List<string>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Tokens = lines[0].Split(',').ToList();  
      }

      int Step(int value, int c)
      {
         value += c; 
         value *= 17; 
         value = value % 256; 
         return value; 
      }

      int Hash(string token)
      {
         int value = 0; 
         foreach (char c in token) {
            value = Step(value, c); 
         }
         return value; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         /*
         Determine the ASCII code for the current character of the string.
         Increase the current value by the ASCII code you just determined.
         Set the current value to itself multiplied by 17.
         Set the current value to the remainder of dividing itself by 256.
         */

         int value = 0; 
         foreach (string token in Tokens) { 
            value += Hash(token); 
         }

         return value.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Box[] boxes = new Box[256]; 
         for (int i = 0; i < 256; ++i) {
            boxes[i] = new Box(); 
         }

         foreach (string token in Tokens) {
            if (token.EndsWith('-')) {
               string label = token.Substring(0, token.Length - 1); 
               int hash = Hash(label);
               
               boxes[hash].RemoveLens(label); 
            } else {
               (string label, string lensStr) = token.Split('='); 
               int lens = int.Parse(lensStr); 
               int hash = Hash(label); 
               
               boxes[hash].AddLens(label, lens); 
            }
         }

         int score = 0;
         for (int i = 0; i < 256; ++i) {
            score += boxes[i].Score(i + 1); 
         }

         return score.ToString(); 
      }
   }
}
