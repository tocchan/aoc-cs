using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day23 : Day
   {
      private string InputFile = "2022/inputs/23.txt";
      
      // each tile is going to stored as follows
      // 0000'00MM`DDCC`BBAA
      // Tile is 0 if no elve is there;
      // 
      // A,B,C,D -> order to consider moves in
      // M -> Move I chose this turn

      IntCanvas Elves = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         int defValue = 0x00000132; // initial move order (north, south, west, east)
         Dictionary<char,int> map = new();
         map['.'] = 0;
         map['#'] = defValue; 

         Elves.InitFromStringArray(lines.ToArray(), map); 
      }

      public int DetermineMove( IntCanvas can, ivec2 pos, int curValue )
      {
         // empty tiles don't move; 
         if (curValue == 0) {
            return 0; 
         }

         int[] values = new int[8]; 
         int wantsToMove = 0; 
         for (int i = 0; i < ivec2.CARDINAL.Length; ++i) { 
            ivec2 d = ivec2.CARDINAL[i];
            values[i] = can.GetValue(pos + d); 
            wantsToMove = wantsToMove | values[i]; 
         }

         eDirection desiredMove = eDirection.None; 
         if (wantsToMove != 0) {
            // okay, determine move order
            int toMove = curValue & 0xffff; 
            for (int i = 0; (i < 4) && (desiredMove == eDirection.None); ++i) {
               int move = toMove & 0xf; 
               toMove = toMove >> 4; 

               switch (move) {
                  case 0: 
                     if ((values[(int)eCardinal.NorthEast] == 0)
                        && (values[(int)eCardinal.East] == 0)
                        && (values[(int)eCardinal.SouthEast] == 0)) {
                        desiredMove = eDirection.Right; 
                     }
                     break; 

                  case 1: 
                     if ((values[(int)eCardinal.NorthWest] == 0)
                        && (values[(int)eCardinal.West] == 0)
                        && (values[(int)eCardinal.SouthWest] == 0)) {
                        desiredMove = eDirection.Left; 
                     }
                     break; 

                  case 2: 
                     if ((values[(int)eCardinal.NorthWest] == 0)
                        && (values[(int)eCardinal.North] == 0)
                        && (values[(int)eCardinal.NorthEast] == 0)) {
                        desiredMove = eDirection.Up; 
                     }
                     break; 

                  case 3: 
                     if ((values[(int)eCardinal.SouthWest] == 0)
                        && (values[(int)eCardinal.South] == 0)
                        && (values[(int)eCardinal.SouthEast] == 0)) {
                        desiredMove = eDirection.Down; 
                     }
                     break; 

                  default:
                     break; 
               }
            } // for
         } // wants to move

         // if we have a desired move, put that in there, otherwise, 0
         int moveVal = (int)desiredMove + 1; 
         int nextTurn = ((curValue & 0xffff) >> 4) | ((curValue & 0xf) << 12); 

         return moveVal << 16 | nextTurn; 
      }

      //----------------------------------------------------------------------------------------------
      public bool Move( IntCanvas elves )
      {
         bool moved = false; 

         ivec2 min = elves.GetMinSetPosition() - ivec2.ONE; 
         ivec2 max = elves.GetMaxSetPosition() + ivec2.ONE; 

         ivec2 p = ivec2.ZERO; 
         for (p.y = min.y; p.y <= max.y; ++p.y) {
            for (p.x = min.x; p.x <= max.x; ++p.x) {
               int v = elves.GetValue( p ); 
               // only empty tiles will be moved into
               if (v != 0) {
                  continue; 
               }

               // okay, get my four corners
               int count = 0; 
               ivec2 elfDir = ivec2.ZERO; 

               for (int i = 0; i < 4; ++i) {
                  eDirection d = (eDirection)i; 
                  ivec2 dv = d.ToVector(); 
                  int nv = elves.GetValue( p + dv ); 
                  if (nv == 0) {
                     continue; 
                  }

                  int mv = (nv >> 16);
                  if (mv > 0) { 
                     eDirection move = (eDirection)(mv - 1); 
                     if (move == d.Negate()) {
                        ++count; 
                        elfDir = dv; 
                     }
                  }
               }

               // move the elf
               if (count == 1) {
                  int ev = elves.GetValue( p + elfDir ) & 0xffff;  
                  elves.SetValue( p + elfDir, 0 ); 
                  elves.SetValue( p, ev ); 
                  moved = true; 
               }
            }
         }

         return moved; 
      }

      //----------------------------------------------------------------------------------------------
      int CountGround( IntCanvas c )
      {
         ivec2 min = c.GetMaxSetPosition(); 
         ivec2 max = c.GetMinSetPosition(); 

         foreach ((ivec2 p, int v) in c) {
            if (v != 0) {
               min = ivec2.Min(p, min); 
               max = ivec2.Max(p, max); 
            }
         }

         int count = 0; 
         for (int y = min.y; y <= max.y; ++y) {
            for (int x = min.x; x <= max.x; ++x) {
               if (c.GetValue(x, y) == 0) {
                  ++count; 
               }
            }
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntCanvas elves = new IntCanvas(Elves); 
         
         Util.IsTraceEnabled = false; 
         Util.TraceLine( $"\nStart...\n{elves.ToString('.', '#')}" ); 

         int rounds = 10; 
         for (int i = 0; i < rounds; ++i) { 
            elves.Automata( DetermineMove ); 
            if (!Move( elves )) {
               break; 
            }

            Util.TraceLine( $"\nTurn {i + 1};\n{elves.ToString('.', '#')}" ); 
         }

         Util.TraceLine( $"\nFinished!;\n{elves.ToString('.', '#')}" ); 


         int ground = CountGround(elves); 
         return ground.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntCanvas elves = new IntCanvas(Elves); 

         int rounds = 0; 
         while (true) { 
            ++rounds; 
            elves.Automata( DetermineMove ); 
            if (!Move( elves )) {
               break; 
            }
         }
         
         return rounds.ToString(); 
      }
   }
}
