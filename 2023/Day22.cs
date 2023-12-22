using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day22 : Day
   {
      private string InputFile = "2023/inputs/22.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Input
      {
         public iaabb3 Brick = new iaabb3(); 
         public List<Input> Supports = new(); 
         public List<Input> SupportedBy = new();

         public Input(string line)
         {
            (string p0, string p1) = line.Split('~'); 
            ivec3 a = ivec3.Parse(p0); 
            ivec3 b = ivec3.Parse(p1); 
            Brick = iaabb3.ThatContains(a, b); 
         }

         public Input(Input toCopy)
         {
            // don't care about supports in this case
            Brick = toCopy.Brick; 
         }

         public bool CanDisintergrate()
         {
            foreach (Input child in Supports) {
               if (child.SupportedBy.Count == 1) {
                  return false; 
               }
            }

            return true; 
         }
      }

      private List<Input> Inputs = new(); 
      private ivec2 GroundSize = ivec2.ZERO; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Input input = new Input(line); 
            Inputs.Add(input); 
         }

         Inputs.Sort((Input a, Input b) => a.Brick.GetBottom() - b.Brick.GetBottom()); 

         // min ends up being 0
         // ivec2 min = new ivec2(int.MaxValue); 
         ivec2 max = new ivec2(int.MinValue); 
         foreach (Input input in Inputs) {
            // min = ivec2.Min(min, input.Brick.MinInclusive.xy); 
            max = ivec2.Max(max, input.Brick.MaxInclusive.xy); 
         }

         GroundSize = max; 
      }

      //----------------------------------------------------------------------------------------------
      int SettleStack(List<Input> inputs)
      {
         IntHeatMap2D HeightMap = new IntHeatMap2D(GroundSize + ivec2.ONE, 0);

         // okay, make them all fall
         int dropped = 0; 
         foreach (Input input in inputs) {
            iaabb2 xy = input.Brick.GetXYSlice();
            int ground = HeightMap.GetMaxIn(xy);

            int bottom = input.Brick.GetBottom();
            int dropDist = bottom - ground - 1;
            if (dropDist > 0) { 
               input.Brick.Move(0, 0, -dropDist);
               ++dropped;
            }

            int height = input.Brick.GetHeight();
            int top = ground + height;
            HeightMap.SetArea(xy, top);
         }

         return dropped; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         SettleStack(Inputs); 

         // figure out what can be destroyed, so figure out what is supporting what
         for (int i = 0; i < Inputs.Count; ++i) {
            Input bottom = Inputs[i]; 
            int heightNeighborWouldBe = bottom.Brick.GetTop() + 1; 
            iaabb2 botXY = bottom.Brick.GetXYSlice(); 

            for (int j = 0; j < Inputs.Count; ++j) {
               if (i == j) {
                  continue; 
               }

               Input top = Inputs[j]; 
               int botOfTop = top.Brick.GetBottom(); 
               if (botOfTop != heightNeighborWouldBe) {
                  continue; 
               }

               iaabb2 topXY = top.Brick.GetXYSlice(); 
               if (botXY.Intersects(topXY)) {
                  bottom.Supports.Add(top); 
                  top.SupportedBy.Add(bottom); 
               }
            }
         }

         int count = 0; 
         foreach (Input input in Inputs) {
            if (input.CanDisintergrate()) {
               ++count;
            }
         }

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      List<Input> CopyStackWithBlockRemoved(int idx)
      {
         List<Input> copy = new(); 
         for (int i = 0; i < Inputs.Count; ++i) {
            if (i == idx) {
               continue; 
            }

            Input input = new Input(Inputs[i]); 
            copy.Add(input); 
         }

         return copy; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         // there is probalby a smart solution here, but I think I can just brute force it. 
         // inputs are currently tight from part A, so copy that except for ONE of them, and resettle, count number moved. 
         int count = 0; 
         for (int i = 0; i < Inputs.Count; ++i) {
            List<Input> stack = CopyStackWithBlockRemoved(i); 
            count += SettleStack(stack); 
         }

         return count.ToString(); 
      }
   }
}
