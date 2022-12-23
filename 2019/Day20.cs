using AoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
   internal class Day20 : Day
   {
      private string InputFile = "2019/inputs/20.txt";
      
      class Tile
      {
         public int index = 0; 
         public ivec2 pos = ivec2.ZERO; 
         public ivec2[] exits = new ivec2[0];
         public int[] exitIndices = new int[0]; 
         public int[] depthChanges = new int[0]; 
      }

      class Portal
      { 
         public string name = ""; 
         public ivec2 pos0 = ivec2.ZERO; 
         public ivec2 pos1 = ivec2.ZERO; 
      }


      Tile[] Tiles = new Tile[0];
      Dictionary<string,Portal> Portals = new(); 

      //----------------------------------------------------------------------------------------------
      (ivec2, int) GetPortalExit(ivec2 entrance)
      {
         foreach (Portal p in Portals.Values) {
            if (p.pos0 == entrance) {
               return (p.pos1, -1); 
            } else if (p.pos1 == entrance) {
               return (p.pos0, 1); 
            }
         }

         return (ivec2.ZERO, 0); 
      }

      //----------------------------------------------------------------------------------------------
      // Find Portals
      bool IsAlpha(char c)
      {
         return c >= 'A' && c <= 'Z'; 
      }

      //----------------------------------------------------------------------------------------------
      void AddPortal( char a, char b, ivec2 p, bool isInner )
      {
         string s = $"{a}{b}"; 
         if (Portals.ContainsKey(s)) {
            Portal port = Portals[s]; 
            if (isInner) { 
               port.pos1 = p; 
            } else {
               port.pos0 = p; 
            }
         } else {
            Portal port = new Portal(); 
            port.name = s; 
            if (isInner) { 
               port.pos1 = p; 
            } else {
               port.pos0 = p; 
            }
            Portals[s] = port; 
         }
      }

      //----------------------------------------------------------------------------------------------
      (ivec2, bool) GetHPos( ivec2 p, string[] lines )
      {
         // space to the left?  portal is to the right, otherwise let
         if ((p.x == 0) || (lines[p.y][p.x - 1] == ' ')) {
            return (p + ivec2.RIGHT * 2, p.x != 0); 
         } else {
            return (p + ivec2.LEFT, (p.x + 2) < lines[0].Length); 
         }
      }
        
      //----------------------------------------------------------------------------------------------
      // bool is if this is an inner portal
      (ivec2, bool) GetVPos( ivec2 p, string[] lines )
      {
         if ((p.y == 0) || (lines[p.y - 1][p.x] == ' ')) {
            return (p + ivec2.DOWN * 2, p.y != 0); 
         } else {
            return (p + ivec2.UP, (p.y + 2) < lines.Length); 
         }
      }

      //----------------------------------------------------------------------------------------------
      void FindPortals(string[] lines)
      {
         for (int y = 0; y < lines.Length - 1; ++y) {
            string line = lines[y]; 
            for (int x = 0; x < line.Length - 1; ++x) {
               char c = line[x]; 
               if (IsAlpha(c)) {
                  // now, is the name going down right?
                  char nc = line[x + 1]; 
                  if (IsAlpha(nc)) {
                     // horizontal name
                     (ivec2 pPos, bool isInner) = GetHPos( new ivec2(x, y), lines );
                     AddPortal( c, nc, pPos, isInner ); 
                  }

                  // vertical name?
                  nc = lines[y + 1][x]; 
                  if (IsAlpha(nc)) {
                     (ivec2 pPos, bool isInner) = GetVPos( new ivec2(x, y), lines );
                     AddPortal( c, nc, pPos, isInner ); 
                  }
               }
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      Tile? CreateTile( ivec2 p, string[] lines)
      {
         if (lines[p.y][p.x] != '.') {
            return null; 
         }

         Tile t = new Tile(); 
         t.pos = p; 

         List<ivec2> exits = new(); 
         List<int> depthChanges = new(); 
         foreach (ivec2 dir in ivec2.DIRECTIONS) {
            ivec2 np = p + dir; 
            char c = lines[np.y][np.x]; 
            if (c == '.') {
               exits.Add(np); 
               depthChanges.Add(0); 
            } else if (c != '#') {
               (ivec2 portalExit, int depthChange) = GetPortalExit(p); 
               if (portalExit != ivec2.ZERO) { 
                  exits.Add(portalExit); 
                  depthChanges.Add(depthChange); 
               }
            }
         }

         t.exits = exits.ToArray(); 
         t.depthChanges = depthChanges.ToArray();
         return t; 
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string[] lines = Util.ReadFileToLines(InputFile).ToArray();

         FindPortals( lines ); 

         ivec2 pos = ivec2.ZERO; 
         ivec2 size = new ivec2(lines[0].Length, lines.Length); 

         List<Tile> tiles = new(); 
         int tileIdx = 0;  
         for (pos.y = 0; pos.y < size.y; ++pos.y) {
            for (pos.x = 0; pos.x < size.x; ++pos.x) {
               Tile? t = CreateTile(pos, lines); 
               if (t != null) {
                  t.index = tileIdx; 
                  ++tileIdx; 
                  tiles.Add(t); 
               }
            }
         }

         Tiles = tiles.ToArray(); 

         foreach (Tile t in Tiles) {
            t.exitIndices = new int[t.exits.Length]; 

            int i = 0; 
            foreach (ivec2 ep in t.exits) {
               Tile et = Tiles.First(t => t.pos == ep);
               t.exitIndices[i] = et.index;
               ++i; 
            }
         }
      }

      public class PathingState
      {
         public int depth; 
         public BitArray visited;
         public int[] lowestCosts; 

         public PathingState( int d, int len )
         {
            depth = d; 
            visited = new BitArray(len); 
            lowestCosts = new int[len];
            lowestCosts.SetAll( int.MaxValue );
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Portal start = Portals["AA"]; 
         Portal stop = Portals["ZZ"]; 
         Tile startTile = Tiles.First( t => t.pos == start.pos0 ); 
         Tile endTile = Tiles.First( t => t.pos == stop.pos0 ); 

         // okay, djikstra time...
         PriorityQueue<(Tile, int),int> toCheck = new PriorityQueue<(Tile,int),int>(); 

         List<PathingState> states = new(); 
         states.Add(new PathingState( 0, Tiles.Length));

         states[0].lowestCosts[startTile.index] = 0; 
         toCheck.Enqueue((startTile, 0), 0); 

         while (toCheck.Count > 0) {
            (Tile t, int depth) = toCheck.Dequeue();
            if ((t == endTile) && (depth == 0))  {
               break; 
            }

            int tIdx = t.index;

            // already visited this one
            PathingState state = states[depth]; 
            if (state.visited[tIdx]) {
               continue; 
            }
            state.visited[tIdx] = true; 

            // check exits
            foreach (int exit in t.exitIndices) {
               Tile et = Tiles[exit]; 
               int cost = state.lowestCosts[tIdx] + 1; 
               if (cost < state.lowestCosts[exit]) {
                  state.lowestCosts[exit] = cost; 
                  toCheck.Enqueue((et, depth), cost); 
               }
            }
         }

         int result = states[0].lowestCosts[endTile.index]; 
         return result.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public PathingState? GetState( List<PathingState> states, int depth )
      {
         if (depth < 0) {
            return null; 
         } else if (depth >= states.Count) {
            states.Add( new PathingState( depth, states[0].lowestCosts.Length ) ); 
         }

         return states[depth]; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Portal start = Portals["AA"]; 
         Portal stop = Portals["ZZ"]; 
         Tile startTile = Tiles.First( t => t.pos == start.pos0 ); 
         Tile endTile = Tiles.First( t => t.pos == stop.pos0 ); 

         // okay, djikstra time...
         PriorityQueue<(Tile, int),int> toCheck = new PriorityQueue<(Tile,int),int>(); 

         List<PathingState> states = new(); 
         states.Add(new PathingState( 0, Tiles.Length));

         states[0].lowestCosts[startTile.index] = 0; 
         toCheck.Enqueue((startTile, 0), 0); 

         while (toCheck.Count > 0) {
            (Tile t, int depth) = toCheck.Dequeue();
            if ((t == endTile) && (depth == 0))  {
               break; 
            }

            int tIdx = t.index;

            // already visited this one
            PathingState state = states[depth]; 
            if (state.visited[tIdx]) {
               continue; 
            }
            state.visited[tIdx] = true; 

            // check exits
            for (int ix = 0; ix < t.exits.Length; ++ix) {
               int ex = t.exitIndices[ix]; 
               int dd = t.depthChanges[ix]; 
               int cost = state.lowestCosts[tIdx] + 1; 
               int ndepth = depth + dd; 

               PathingState? nstate = GetState( states, ndepth ); 
               if (nstate == null) {
                  continue; 
               }

               if (cost < nstate.lowestCosts[ex]) {
                  Tile et = Tiles[ex]; 
                  nstate.lowestCosts[ex] = cost; 

                  if (depth != ndepth) {
                     Portal p = Portals.Values.First( p => (p.pos0 == t.pos) || (p.pos1 == t.pos) ); 
                     // Util.WriteLine( $"Take portal {p.name} at step {cost} to depth {ndepth}" ); 
                  }

                  toCheck.Enqueue((et, depth + dd), cost); 
               }
            }
         }

         int result = states[0].lowestCosts[endTile.index]; 
         return result.ToString(); 
      }
   }
}
