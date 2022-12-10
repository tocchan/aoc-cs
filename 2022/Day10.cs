using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Context
   { 
      public int X = 1; 
      public int Cycle = 0; 
   }

   internal class Op
   {
      public int Delay = 1; 
      public Action<Context>? Work;
   }

   internal class Day10 : Day
   {
      private string InputFile = "2022/inputs/10.txt";
      
      List<Op> Ops = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            string[] ops = line.Split(' '); 
            if (ops[0] == "addx") {
               Op op = new Op(); 
               op.Delay = 2; 
               int v = int.Parse(ops[1]); 
               op.Work = (Context c) => c.X += v;

               Ops.Add(op); 
            } else {
               Op op = new Op(); 
               op.Delay = 1; 
               op.Work = (Context c) => { }; 

               Ops.Add(op); 
            }

         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Context c = new(); 

         int target = 20; 
         int value = 0; 
         foreach (Op op in Ops) {
            int newCycle = c.Cycle + op.Delay; 
            
            if ((target > c.Cycle) && (target <= newCycle)) {
               value += target * c.X; 
               target += 40; 
            }

            c.Cycle = newCycle; 

            op.Work!(c); 
         }

         return value.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntCanvas canvas = new IntCanvas(); 
         ivec2 size = new ivec2(40, 6); 
         canvas.SetValue(ivec2.ZERO, 0); 
         canvas.SetValue(size - ivec2.ONE, 0); 

         Context ctx = new(); 
         ctx.Cycle = 0; 

         foreach (Op op in Ops) {
            int newCycle = ctx.Cycle + op.Delay; 

            // c.X (one less or one greater is the sprite)
            // cycle is the X
            int minX = ctx.X - 1; 
            int maxX = ctx.X + 1; 

            for (int c = ctx.Cycle; c < newCycle; ++c) {
               int x = c % 40;
               int y = c / 40; 

               if ((x >= minX) && (x <= maxX)) {
                  canvas.SetValue( new ivec2(x, y), 1 ); 
               } else {
                  canvas.SetValue( new ivec2(x, y), 0 ); 
               }
            }
            ctx.Cycle = newCycle; 

            op.Work!(ctx); 
         }

         return "\n" + canvas.ToString(" █"); 
      }
   }
}
