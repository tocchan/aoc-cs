using AoC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day09 : Day
   {
      private string InputFile = "2022/inputs/09.txt";
      
      List<ivec2> Instructions = new(); 
      ivec2 Head = ivec2.ZERO; 
      ivec2 Tail = ivec2.ZERO; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            string[] parts = line.Split(' ');
            int count = int.Parse(parts[1]); 
            ivec2 dir = parts[0][0] switch {
               'R' => ivec2.RIGHT, 
               'L' => ivec2.LEFT, 
               'U' => ivec2.UP, 
               'D' => ivec2.DOWN, 
               _ => ivec2.ZERO
            }; 

            for (int i = 0; i < count; ++i) { 
               Instructions.Add(dir); 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      private ivec2 Follow(ivec2 head, ivec2 tail)
      {
         ivec2 diff = head - tail; 
         ivec2 adiff = ivec2.Abs(diff); 
         if (adiff.MaxElement() > 1) {
            if (adiff.x > adiff.y) {
               return new ivec2(head.x - Math.Sign(diff.x), head.y);
            } else if (adiff.y > adiff.x) { 
               return new ivec2(head.x, head.y - Math.Sign(diff.y));
            } else {
               return head - ivec2.Sign(diff); 
            }
         }

         return tail; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         HashSet<ivec2> visited = new(); 
         visited.Add(ivec2.ZERO); 

         foreach (ivec2 dir in Instructions) {
            Head += dir;
            Tail = Follow(Head, Tail); 
            visited.Add(Tail); 
         }

         return visited.Count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         ivec2[] rope = new ivec2[10]; 

         HashSet<ivec2> visited = new();
         visited.Add(rope[9]); 

         foreach (ivec2 dir in Instructions) {
            rope[0] += dir; 
            for (int i = 1; i < 10; ++i) {
               rope[i] = Follow(rope[i - 1], rope[i]); 
            }

            visited.Add(rope[9]); 
         }

         return visited.Count.ToString(); 
      }
   }
}
