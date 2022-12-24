using AoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day24 : Day
   {
      private string InputFile = "2022/inputs/24.txt";


      IntHeatMap2D[] BlizzStates = new IntHeatMap2D[0]; 
      ivec2 StartPos = new ivec2(1, 0); 
      ivec2 EndPos = ivec2.ZERO; 
      ivec2 Size = ivec2.ONE; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string[] lines = Util.ReadFileToLines(InputFile).ToArray(); 

         Size = new ivec2(lines[0].Length, lines.Length ); 
         ivec2 valleySize = Size - 2; 
         EndPos = Size - new ivec2(2, 1); 

         // create all the blizzards
         List<ivec4> blizz = new(); 
         foreach (ivec2 p in ivec2.EnumArea(Size)) {
            char c = lines[p.y][p.x]; 

            if (c == '>') {
               blizz.Add( new ivec4(p - 1, ivec2.RIGHT) ); 
            } else if (c == 'v') {
               blizz.Add( new ivec4(p - 1, ivec2.DOWN) ); 
            } else if (c == '<') {
               blizz.Add( new ivec4(p - 1, ivec2.LEFT) ); 
            } else if (c == '^') {
               blizz.Add( new ivec4(p - 1, ivec2.UP) ); 
            }
         }

         // generate all blizzard states
         int numUniqueStates = (int) Util.LCM(valleySize.x, valleySize.y); 
         BlizzStates = new IntHeatMap2D[numUniqueStates]; 

         for (int i = 0; i < numUniqueStates; ++i) {
            IntHeatMap2D state = new IntHeatMap2D(); 
            state.Init(valleySize, 0); 
            for (int bidx = 0; bidx < blizz.Count; ++bidx) { 
               ivec2 pos = blizz[bidx].xy; 
               ivec2 vel = blizz[bidx].zw; 
               int cnt = state.Get(pos);
               state.Set( pos, cnt + 1 ); 

               // move it for next turn
               pos = ivec2.Mod((pos + vel), valleySize);
               blizz[bidx] = new ivec4( pos, vel ); 
            }
            BlizzStates[i] = state; 
         }
      }

      public int GetStateIndex( ivec3 p )
      {
         return p.z * Size.Product() 
            + p.y * Size.x 
            + p.x; 
      }

      //----------------------------------------------------------------------------------------------
      bool CanVisit(ivec2 pos, IntHeatMap2D blizzState, ivec2 start, ivec2 end)
      {
         // these are always legal to move into
         if ((pos == end) || (pos == start)) {
            return true; 
         }

         pos = pos - 1; 
         return blizzState.Get(pos) == 0; 
      }

      //----------------------------------------------------------------------------------------------
      int Path( ivec2 start, ivec2 end, int startMin )
      {
         PriorityQueue<ivec3,int> nextMove = new(); 
         int numStates = BlizzStates.Length * Size.Product(); 

         ivec3 startMove = new ivec3( start, startMin % BlizzStates.Length ); 

         BitArray visited = new BitArray(numStates); 
         int[] lowestCost = new int[numStates];
         ivec3[] movesTaken = new ivec3[numStates]; 

         lowestCost.SetAll(int.MaxValue); 
         lowestCost[GetStateIndex(startMove)] = 0; 
         nextMove.Enqueue( startMove, 0 ); 
         
         List<ivec2> dirs = new(); 
         dirs.AddRange( ivec2.DIRECTIONS ); 
         dirs.Add( ivec2.ZERO ); // not moving is a move in this one

         ivec3 finalMove = ivec3.ZERO; 
         while (nextMove.Count > 0) {
            ivec3 move = nextMove.Dequeue(); 
            int moveIdx = GetStateIndex(move); 

            // reached our goal
            if (move.xy == end) {
               finalMove = move; 
               break; 
            }

            // don't revisit if we've already processed
            if (visited[moveIdx]) {
               continue; 
            }
            visited[moveIdx] = true; 

            // determine next move
            int curCost = lowestCost[moveIdx]; 
            int blizzIdx = (move.z + 1) % BlizzStates.Length; 
            IntHeatMap2D blizzMap = BlizzStates[blizzIdx]; 

            ivec2 pos = move.xy; 
            int ncost = curCost + 1; 
            foreach (ivec2 d in dirs) {
               ivec2 npos = pos + d; 
               if (!CanVisit(npos, blizzMap, start, end)) {
                  continue; 
               }

               ivec3 nmove = new ivec3(npos, blizzIdx); 
               int nidx = GetStateIndex(nmove); 
               if (ncost < lowestCost[nidx]) {
                  lowestCost[nidx] = ncost;

                  // for debug
                  movesTaken[nidx] = move; 

                  nextMove.Enqueue( nmove, ncost ); 
               }
            }
         }

         int finalIdx = GetStateIndex(finalMove); 
         int totalMoves = lowestCost[finalIdx]; 
         return totalMoves; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // think of this as a 3d pathing problem.  "staying still" is still moving forward in time, and if I'm 
         // ending up in the same tile during the same cycle, that is just being revisited
         int res = Path( StartPos, EndPos, 0 ); 
         return res.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int there = Path( StartPos, EndPos, 0 ); 
         int andBack = Path( EndPos, StartPos, there ); 
         int again = Path( StartPos, EndPos, there + andBack ); 
         
         int aTaleByBilboBaggins = there + andBack + again; 
         return aTaleByBilboBaggins.ToString(); 
      }
   }
}
