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
      
      int[] Moves = new int[0]; 
      IntMap[] Shapes = new IntMap[0]; 

      internal class GameState
      {
         public IntCanvas Canvas = new(); 
         public int Width = 7; 
         public int RockIndex = 0; 
         public int MoveIndex = 0; 

         public IntMap[] Shapes = new IntMap[0]; 
         public int[] Moves = new int[0]; 

         public int MaxHeight = 0; 

         public void IncrementRockIndex()
         {
            ++RockIndex; 
            if (RockIndex >= Shapes.Length) { 
               RockIndex = 0; 
            }
         }

         public void IncrementMoveIndex()
         {
            ++MoveIndex;
            if (MoveIndex >= Moves.Length) {
               MoveIndex = 0; 
            }
         }

         private bool Collides( ivec2 pos, IntMap rock )
         {
            // hit a wall
            if ((pos.x < 0) || ((pos.x + rock.Width) > Width)) {
               return true; 
            }

            // hit the floor
            if ((pos.y + rock.Height) > 1) {
               return true; 
            }

            return Canvas.CollidesWith(pos, rock); 
         }

         public void DropNext()
         {
            IntMap rock = Shapes[RockIndex]; 
            IncrementRockIndex();

            ivec2 pos = new ivec2(2, MaxHeight - rock.Height - 2); 
            while (true) { // got to remember I'm moving positive to go down
               int dir = Moves[MoveIndex]; 
               IncrementMoveIndex(); 

               pos.x += dir; 
               if (Collides(pos, rock)) {
                  pos.x -= dir; 
               }

               pos.y += 1; 
               if (Collides(pos, rock)) {
                  pos.y -= 1; 
                  break; // hit a rock or hit bottom, stop
               }
            }

            // we hit, draw this intno the canvas
            Canvas.DrawIntMap(pos, rock); 
            MaxHeight = Math.Min(pos.y - 1, MaxHeight);  
         }

         public int Height => -MaxHeight; 
         public ivec2 MoveState => new ivec2(RockIndex, MoveIndex);
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Moves = lines[0].Select( c => (c == '>') ? 1 : -1 ).ToArray();

         Shapes = new IntMap[5];
         // Add shapes
         Shapes[0] = new IntMap(
            "####"
         );
         Shapes[1] = new IntMap(
            " # \n" +
            "###\n" +
            " # "
         ); 
         Shapes[2] = new IntMap(
            "  #\n" +
            "  #\n" +
            "###"
         ); 
         Shapes[3] = new IntMap(
            "#\n" +
            "#\n" +
            "#\n" +
            "#"
         ); 
         Shapes[4] = new IntMap(
            "##\n" + 
            "##"
         ); 
      }

      



      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         GameState state = new GameState(); 
         state.Shapes = Shapes;
         state.Moves = Moves; 

         int numRocks = 2022; 
         for (int ri = 0; ri < numRocks; ++ri) {
            state.DropNext(); 
         }

         return state.Height.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         // so, similar, but drop until I notice a pattern.  
         GameState state = new GameState(); 
         state.Shapes = Shapes;
         state.Moves = Moves; 

         Dictionary<int,(int, int, Int64)> SeenStates = new(); 

         Int64 totalRocks = 1000000000000; 
         Int64 totalHeight = 0; 

         for (int ri = 0; ri <= 10000; ++ri) {
            if (state.RockIndex == 0) {
               int moveIdx = state.MoveIndex; 
               if (SeenStates.ContainsKey(moveIdx)) { 
                  (int rock, int height, Int64 skip) = SeenStates[moveIdx]; 

                  Int64 hInterval = state.Height - height; 
                  Int64 rInterval = ri - rock; 
                  if (rInterval == skip) { // did we skip the same amount since last we saw this come up?  Probably a repeat...
                     totalHeight += state.Height; 
                     totalRocks = totalRocks - ri;

                  
                     Int64 intervals = totalRocks / rInterval; 
                     totalHeight += intervals * hInterval; 

                     totalRocks -= intervals * rInterval; 
                     break; 
                  } else {
                     SeenStates[moveIdx] = (ri, state.Height, rInterval);
                  }
               } else { 
                  SeenStates[moveIdx] = (ri, state.Height, 0);
               }
            }
            state.DropNext();
         }

         // okay, we just need to close out
         int rem0 = state.Height; 
         for (Int64 i = 0; i < totalRocks; ++i) {
            state.DropNext(); 
         }
         totalHeight += state.Height - rem0; 

         return totalHeight.ToString(); 
      }
   }
}
