using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day05 : Day
   {
      private string InputFile = "2022/inputs/05.txt";
      
      private List<Stack<char>> Stacks = new(); 
      private List<ivec3> Instructions = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Stacks = new(); 
         Instructions = new(); 
         List<string> lines = Util.ReadFileToLines(InputFile);

         for (int j = 0; j < 9; ++j) {
            List<char> stack = new(); 
            for (int i = 0; i < 8; ++i) {
               char c = lines[i][4 * j + 1]; 
               if (c != ' ') {
                  stack.Add(c); 
               }
            }

            stack.Reverse(); 
            Stack<char> val = new(); 
            for (int i = 0; i < stack.Count; ++i) {
               val.Push(stack[i]); 
            }

            Stacks.Add(val); 
         }

         for (int i = 10; i < lines.Count; ++i) {
            string[] words = lines[i].Split(' '); 
            ivec3 val = new ivec3(int.Parse(words[1]), int.Parse(words[3]) - 1, int.Parse(words[5]) - 1); 
            Instructions.Add(val); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         foreach (ivec3 i in Instructions) {
            for (int c = 0; c < i.x; ++c) { 
               char v = Stacks[i.y].Pop();
               Stacks[i.z].Push(v); 
            }
         }

         string result = ""; 
         for (int i = 0; i < Stacks.Count; ++i) {
            result += Stacks[i].Peek(); 
         }
         
         return result; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         ParseInput(); // start fresh; 

         Stack<char> hold = new(); 
         foreach (ivec3 i in Instructions) {
            
            for (int c = 0; c < i.x; ++c) { 
               hold.Push(Stacks[i.y].Pop());
            }

            while (hold.Count > 0) { 
               Stacks[i.z].Push(hold.Pop()); 
            }
         }

         string result = ""; 
         for (int i = 0; i < Stacks.Count; ++i) {
            result += Stacks[i].Peek(); 
         }
         
         return result; 
      }
   }
}
