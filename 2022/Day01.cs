﻿using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day01 : Day
   {
      private string InputFile = "2022/inputs/01.txt";
      private List<List<int>> Elves = new List<List<int>>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (List<string> group in lines.SplitAllWhen(x => (x.Length == 0))) {
            List<int> elf = group.Select(int.Parse).ToList(); 
            Elves.Add(elf); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int maxIdx = 0; 
         int maxCalories = Elves[0].Sum(); 

         for (int i = 1; i < Elves.Count; ++i) {
            int sum = Elves[i].Sum(); 
            if (sum > maxCalories) {
               maxCalories = sum; 
               maxIdx = i; 
            }
         }

         return maxCalories.ToString(); 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         PriorityQueue<int,int> maxValues = new PriorityQueue<int,int>(); 

         for (int i = 0; i < Elves.Count; ++i) {
            int cal = Elves[i].Sum(); 
            maxValues.Enqueue(cal, -cal); 
         }

         int sum = 0; 
         for (int i = 0; i < 3; ++i) {
            sum += maxValues.Dequeue(); 
         }

         return sum.ToString(); 
      }
   }
}
