using AoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day17 : Day
   {
      private string InputFile = "2023/inputs/17.txt";

      private int MaxStep = 2;

      struct LookupKey
      {
         public ivec2 pos; 
         public ivec2 dir; 
         public int step;

         public override bool Equals([NotNullWhen(true)] object? obj)
         {
            if ((obj == null) || !obj.GetType().Equals(GetType())) {
               return false;
            }

            LookupKey other = (LookupKey)obj;
            return pos == other.pos
               && dir == other.dir
               && step == other.step; 
         }

         public override int GetHashCode()
         {
            return HashCode.Combine(pos, dir, step); 
         }
      }

      class Move
      {
         public Move? prev_move;
         public LookupKey key; 
         public int cost; 

         public Move Copy()
         {
            Move copy = new Move(); 
            copy.prev_move = this; 
            copy.key = key; 
            copy.cost = cost; 
            return copy; 

         }
      }

      IntHeatMap2D Map = new IntHeatMap2D(); 
      Dictionary<LookupKey,int> Lookup = new Dictionary<LookupKey, int>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Map.SetFromTightBlock(lines, int.MaxValue, '0'); 
      }

      //----------------------------------------------------------------------------------------------
      void TryAdd(PriorityQueue<Move, int> moves, Move move, ivec2 end)
      {
         move.key.pos += move.key.dir; 
         if (!Map.ContainsPoint(move.key.pos)) {
            return; 
         }

         ivec2 distToEnd = move.key.pos - end; 
         int hcost = distToEnd.GetManhattanDistance(); 

         move.cost += Map[move.key.pos]; 
         moves.Enqueue(move, move.cost + hcost); 
      }

      //----------------------------------------------------------------------------------------------
      Move FindBestPath(ivec2 pos, ivec2 dir, int stepsRemaining, ivec2 end)
      {
         Move firstMove = new Move(); 
         firstMove.prev_move = null; 
         firstMove.key.pos = pos; 
         firstMove.key.dir = dir; 
         firstMove.key.step = stepsRemaining;
         firstMove.cost = 0; 

         PriorityQueue<Move, int> moves = new PriorityQueue<Move, int>(); 
         moves.Enqueue(firstMove, 0); 

         while (moves.Count > 0) {
            Move move = moves.Dequeue(); 
            if (Lookup.ContainsKey(move.key)) { 
               if (Lookup[move.key] > move.cost) {
                  Util.WriteLine("ERROR");  // if we got here, this move would have been cheaper
               }
               continue; // we've been here with a better move, ignore it. 
            }
            Lookup[move.key] = move.cost; 

            if (move.key.pos == end) {
               return move; 
            }

            if (move.key.step > 0) {
               Move forward = move.Copy(); ; 
               forward.key.step--; 
               TryAdd(moves, forward, end); 
            }

            Move right = move.Copy(); 
            right.key.dir = right.key.dir.GetRotatedRight();
            right.key.step = MaxStep; 
            TryAdd(moves, right, end); 

            Move left = move.Copy(); 
            left.key.dir = left.key.dir.GetRotatedLeft(); 
            left.key.step = MaxStep; 
            TryAdd(moves, left, end); 
         }
            
         return new Move(); 
      }

      // this is A*, but with a slight twist.  So doing it here
      public int FindBestPath(ivec2 start, ivec2 end)
      {
         Move move = FindBestPath(start, ivec2.RIGHT, MaxStep + 1, end); 
         
         string pal = "0123456789^>v<"; 
         IntCanvas canvas = new IntCanvas(); 
         canvas.SetSize(Map.GetSize()); 

         foreach ((ivec2 pos, int val) in Map) {
            canvas.SetValue(pos, val); 
         }

         Move? iter = move; 
         while (iter != null) {
            if (iter.key.dir == ivec2.UP) {
               canvas.SetValue(iter.key.pos, 10);
            } else if (iter.key.dir == ivec2.RIGHT) {
               canvas.SetValue(iter.key.pos, 11);
            } else if (iter.key.dir == ivec2.DOWN) {
               canvas.SetValue(iter.key.pos, 12);
            } else if (iter.key.dir == ivec2.LEFT) {
               canvas.SetValue(iter.key.pos, 13);
            } 
            // Util.WriteLine($"{iter.key.pos}: {Map[iter.key.pos]} -> {iter.cost}"); 
            iter = iter.prev_move; 
         }

         Util.WriteLine(canvas.ToString(pal)); 

         return move.cost;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int heatLost = FindBestPath(ivec2.ZERO, Map.GetSize() - ivec2.ONE); 
         return heatLost.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
