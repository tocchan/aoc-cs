using AoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day16 : Day
   {
      private string InputFile = "2024/inputs/16.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      IntHeatMap2D Map = new(); 
      ivec2 StartLocation; 
      ivec2 EndLocation; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string mapString = Util.ReadFileToString(InputFile); 
         Map.InitFromString(mapString); 

         StartLocation = Map.FindLocation('S')!.Value; 
         Map.Set(StartLocation, '.'); 

         EndLocation = Map.FindLocation('E')!.Value; 
         Map.Set(EndLocation, '.'); 
      }

      //----------------------------------------------------------------------------------------------
      // Hello A* my old friend
      struct MapLoc 
      {
         public ivec2 loc; 
         public ivec2 dir;

         public override int GetHashCode()
         {
            return HashCode.Combine(loc, dir); 
         }

         public MapLoc(ivec2 l, ivec2 d) 
         {
            loc = l; 
            dir = d; 
         }
      }

      class PathToken 
      {
         public MapLoc loc; 
         public int cost; 
         public List<PathToken> prev = new();  // any possible paths that could have gotten here

         public PathToken(MapLoc l, int c) 
         {
            loc = l; 
            cost = c; 
         }

         public PathToken()
         {
            loc = new MapLoc(new ivec2(-1), ivec2.ZERO); 
            cost = -1; 
         }

         public bool IsValid() 
         {
            return cost >= 0; 
         }

         public PathToken Forward()
         {
            ivec2 l = loc.loc + loc.dir; 
            MapLoc nloc = new MapLoc(l, loc.dir); 
            return new PathToken(nloc, cost + 1); 
         }

         public PathToken Right()
         {
            ivec2 l = loc.loc; 
            ivec2 d = loc.dir.GetRotatedRight(); 
            MapLoc nloc = new MapLoc(l, d); 
            return new PathToken(nloc, cost + 1000); 
         }

         public PathToken Left()
         {
            ivec2 l = loc.loc; 
            ivec2 d = loc.dir.GetRotatedLeft(); 
            MapLoc nloc = new MapLoc(l, d); 
            return new PathToken(nloc, cost + 1000); 
         }

      }

      PathToken[] GetPossibleMoves(PathToken token) 
      {
         PathToken[] moves = new PathToken[3]; 
         moves[0] = token.Forward(); 
         moves[1] = token.Right(); 
         moves[2] = token.Left(); 

         foreach (PathToken move in moves) {
            move.prev.Add(token); 
         }

         return moves; 
      }

      PathToken Path() 
      {
         PriorityQueue<PathToken, int> moves = new(); 
         Dictionary<MapLoc, PathToken> visited = new(); 

         MapLoc start = new MapLoc(StartLocation, ivec2.RIGHT); 

         PathToken startToken = new PathToken(start, 0); 
         moves.Enqueue(new PathToken(start, 0), 0); 
         visited.Add(startToken.loc, startToken); 

         while (moves.Count > 0) {
            PathToken token = moves.Dequeue(); 
         
            if (token.loc.loc == EndLocation) 
            {
               return token; 
            }

            PathToken[] nextMoves = GetPossibleMoves(token); 
            foreach (PathToken nextMove in nextMoves) 
            {
               ivec2 pos = nextMove.loc.loc; 
               if (Map.Get(pos) != '.') {
                  continue; 
               }

               // could probably get more aggressive by factoring in min turns to reach it, but whatever
               int h = (EndLocation - pos).GetManhattanDistance();

               bool enqueue = true; // add if we haven't visited this tile before or if we're a better option
               PathToken? existing; 
               if (visited.TryGetValue(nextMove.loc, out existing)) 
               {
                  enqueue = nextMove.cost < existing.cost; 

                  // if we have something already here, no need to readd it, but 
                  // add our previous to it's possibilities
                  if (existing.cost == nextMove.cost) 
                  {
                     existing.prev.Add(token); // add aa different way to get here, but do not need to enqueue; 
                  }
               }

               if (enqueue)
               {
                  moves.Enqueue(nextMove, nextMove.cost + h); 
                  visited.Remove(nextMove.loc); 
                  visited.Add(nextMove.loc, nextMove); 
               }
            }
         }

         return new PathToken(); 
      }




      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         PathToken path = Path(); 

         /*
         PathToken iter = path; 
         while (iter.prev.Count > 0) 
         {
            Map.Set(iter.loc.loc, iter.loc.dir.ToDirChar()); 
            iter = iter.prev[0]; 
         }
         Util.WriteLine(Map.Draw()); 
         */

         return path.cost.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         PathToken path = Path(); 

         PathToken iter = path; 

         HashSet<ivec2> uniques = new(); 
         Queue<PathToken> tokens = new(); 
         tokens.Enqueue(iter); 

         while (tokens.Count > 0) 
         {
            PathToken token = tokens.Dequeue(); 
            uniques.Add(token.loc.loc); 
            foreach (PathToken prev in token.prev) {
               tokens.Enqueue(prev); 
            }
         }

         return uniques.Count.ToString(); 
      }
   }
}
