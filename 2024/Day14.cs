using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day14 : Day
   {
      private string InputFile = "2024/inputs/14.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Input
      {
         public ivec2 pos; 
         public ivec2 vel; 

         public Input(string line)
         {
            string[] pv = line.Split(' ').ToArray(); 
            pos = ivec2.Parse(pv[0].Split('=')[1]); 
            vel = ivec2.Parse(pv[1].Split('=')[1]); 
         }

         public ivec2 GetFuturePosition(ivec2 roomSize, int turns) 
         {
            ivec2 np = pos + vel * turns; 
            np = ivec2.Mod(np, roomSize); 
            
            return np; 
         }
      }

      private List<Input> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Input input = new Input(line); 
            Inputs.Add(input); 
         }
      }

      Int64 CountInArea(List<ivec2> locs, ivec2 topLeft, ivec2 size) 
      {
         Int64 count = 0; 
         ivec2 bottomRight = topLeft + size; 
         foreach (ivec2 loc in locs) {
            if ((loc >= topLeft) && (loc < bottomRight)) {
               ++count; 
            }
         }

         return count; 
      }

      Int64 GetScore(List<ivec2> locs, ivec2 roomSize) 
      {
         Int64 score = 1; 
         ivec2 halfSize = roomSize / 2; 
         for (int y = 0; y < 2; ++y) {
            for (int x = 0; x < 2; ++x) {
               ivec2 quad = new ivec2(x, y); 
               score *= CountInArea(locs, halfSize * quad + quad, halfSize);
            }
         }

         return score; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // ivec2 roomSize = new ivec2(11, 7); 
         ivec2 roomSize = new ivec2(101, 103); 
         int turns = 100; 

         List<ivec2> results = new(); 
         foreach (Input input in Inputs) {
            results.Add(input.GetFuturePosition(roomSize, turns)); 
         }

         Int64 score = GetScore(results, roomSize); 
         return score.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         ivec2 roomSize = new ivec2(101, 103); 
         IntCanvas map = new IntCanvas(); 
         map.SetSize(roomSize); 

         ivec2[] locs = new ivec2[Inputs.Count]; 
         for (int i = 0; i < Inputs.Count; ++i) {
            locs[i] = Inputs[i].pos; 
         }

         if (!Directory.Exists("temp")) {
            Directory.CreateDirectory("temp"); 
         }

         StreamWriter output = new StreamWriter("temp/day14.output.txt"); 

         // found by printing _everything_ and looking for patterns.  Noticed they clumped every 103*n + 53
         // so then just printed those and looked for an "easter egg", I guess.  Solution was found visually
         int step = 53; 
         int maxSteps = roomSize.x * roomSize.y; 
         while (step < maxSteps) {
            map.SetAll(0); 
            if ((step % 100) == 0) {
               Util.WriteLine($"Step: {step}"); 
            }
            
            output.WriteLine($"Step: {step}"); 

            for (int i = 0; i < Inputs.Count; ++i) {
               locs[i] = Inputs[i].GetFuturePosition(roomSize, step); 
               map.SetValue(locs[i], map.GetValue(locs[i]) + 1); 
            }

            output.WriteLine(map.ToString(" █")); 
            step += roomSize.y;
         }

         return ""; 
      }
   }
}
