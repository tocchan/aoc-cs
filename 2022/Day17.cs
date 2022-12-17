using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{


   internal class Day17 : Day
   {
      private string InputFile = "2022/inputs/17.txt";
      
      List<ivec2> Push = new(); 

      List<IntMap> Shapes = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (char c in lines[0]) {
            Push.Add(c == '>' ? ivec2.RIGHT : ivec2.LEFT); 
         }

         // Add shapes
         Shapes.Add( new IntMap(
            "####"
         )); 
         Shapes.Add( new IntMap(
            " # \n" +
            "###\n" +
            " # "
         )); 
         Shapes.Add( new IntMap(
            "  #\n" +
            "  #\n" +
            "###"
         )); 
         Shapes.Add( new IntMap(
            "#\n" +
            "#\n" +
            "#\n" +
            "#"
         )); 
         Shapes.Add( new IntMap(
            "##\n" + 
            "##"
         )); 
      }

      private bool Collides( ivec2 pos, IntMap rock, IntCanvas map, int width )
      {
         // hit a wall
         if ((pos.x < 0) || ((pos.x + rock.Width) > width)) {
            return true; 
         }

         // hit the floor
         if ((pos.y + rock.Height) > 1) {
            return true; 
         }

         return map.CollidesWith(pos, rock); 
      }

      int? CheckForTetris(IntCanvas c, int y, int height)
      {
         for (int i = 0; i < height; ++i) {
            bool hasTetris = true; 
            int ly = y + i; 
            for (int x = 0; x < 7; ++x) {
               if (c.GetValue(x, ly) == 0) {
                  hasTetris = false; 
                  break; 
               }
            }

            if (hasTetris) {
               return ly; 
            }
         }

         return null; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntCanvas canvas = new(); 
         canvas.SetSize( new ivec2(1, 7) ); 
         int maxHeight = 0; 
         int width = 7; 

         int numRocks = 5050; 
         int rockIdx = 0; 
         int dirIdx = 0; 

         for (int ri = 0; ri < numRocks; ++ri) {
            IntMap rock = Shapes[rockIdx]; 
            ivec2 pos = new ivec2(2, maxHeight - rock.Height - 2); 

            while (true) { // got to remember I'm moving positive to go down
               ivec2 dir = Push[dirIdx]; 
               ++dirIdx; 
               if (dirIdx >= Push.Count) {
                  dirIdx = 0; 
               }

               if (!Collides(pos + dir, rock, canvas, width)) {
                  pos = pos + dir; 
               }

               pos.y += 1; 
               if (Collides(pos, rock, canvas, width)) {
                  pos.y -= 1; 
                  break; // hit a rock or hit bottom, stop
               }
            }

            // we hit, draw this intno the canvas
            canvas.DrawIntMap(pos, rock); 
            maxHeight = Math.Min(pos.y - 1, maxHeight);  

            int? lineIdx = CheckForTetris(canvas, pos.y, rock.Height); 
            if (lineIdx.HasValue) { 
               Util.WriteLine($"Tetris: Line {1 - lineIdx.Value}, rockIdx: {rockIdx},  windIdx: {dirIdx}, rockCount={ri + 1}, linevalue={-maxHeight}");
            }

            ++rockIdx; 
            if (rockIdx >= Shapes.Count) {
               rockIdx = 0; 
            }
         }

         // Util.WriteLine(canvas.ToString(" #") + "\n-------\n"); 

         return (-maxHeight).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
