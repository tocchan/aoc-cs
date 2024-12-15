using AoC;
using AoC2023;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day15 : Day
   {
      private string InputFile = "2024/inputs/15.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      IntHeatMap2D Map = new(); 
      List<ivec2> Moves = new(); 
      ivec2 StartLocation; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         List<string> mapLines = new(); 
         int idx = 0; 
         while (!String.IsNullOrEmpty(lines[idx])) {
            mapLines.Add(lines[idx]); 
            ++idx; 
         }

         Map.SetFromTightBlock(mapLines.ToArray(), ""); 
         StartLocation = Map.FindLocation('@')!.Value;
         Map.Set(StartLocation, '.'); 

         ++idx; 
         for (; idx < lines.Count; ++idx) {
            foreach (char c in lines[idx]) {
               ivec2 dir = c switch 
               {
                  '^' => ivec2.UP, 
                  '>' => ivec2.RIGHT, 
                  '<' => ivec2.LEFT, 
                  'v' => ivec2.DOWN,
                  _ => ivec2.ZERO
               };
               Moves.Add(dir); 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      ivec2 DoMove(IntHeatMap2D map, ivec2 pos, ivec2 dir) 
      {
         ivec2 look = pos + dir; 
         while (map.Get(look) == 'O') {
            look += dir; 
         }

         if (map.Get(look) == '#') {
            // do nothing, we hit a wall first
            return pos; 
         }

         // open tile is the only other option ('.')
         // move all blocks into that open space
         while (map.Get(look - dir) == 'O') 
         {
            map.Set(look, 'O'); 
            map.Set(look - dir, '.');  
            look -= dir; 
         }

         return look; 
      }

      //----------------------------------------------------------------------------------------------
      int GetScore(IntHeatMap2D map) 
      {
         int score = 0; 
         foreach ((ivec2 p, int v) in map) {
            if ((v == 'O') || (v == '[')) {
               score += 100 * p.y + p.x; 
            }
         }

         return score; 
      }

      void DebugDraw(IntHeatMap2D map, ivec2 loc, ivec2 dir) 
      {
         char v = (dir == ivec2.RIGHT) ? '>'
            : (dir == ivec2.LEFT) ? '<'
            : (dir == ivec2.DOWN) ? 'v'
            : (dir == ivec2.UP) ? '^' : '.'; 

         map.Set(loc, v); 
         Util.WriteLine(map.Draw() + "\n---------"); 
         map.Set(loc, '.'); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntHeatMap2D newMap = new IntHeatMap2D(Map); 
         ivec2 loc = StartLocation; 
         
         foreach (ivec2 move in Moves) {
            loc = DoMove(newMap, loc, move); 
         }

         return GetScore(newMap).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      IntHeatMap2D GetBigMap()
      {
         IntHeatMap2D map = new IntHeatMap2D(Map.GetSize() * new ivec2(2, 1)); 

         for (int y = 0; y < Map.GetHeight(); ++y) {
            for (int x = 0; x < Map.GetWidth(); ++x) {
               int v = Map.Get(x, y); 
               int lh = 2 * x; 
               int rh = lh + 1; 
               if (v == 'O') {
                  map.Set(lh, y, '['); 
                  map.Set(rh, y, ']'); 
               } else {
                  map.Set(lh, y, v); 
                  map.Set(rh, y, v); 
               }
            }
         }

         return map; 
      }

      bool CanPush(IntHeatMap2D map, ivec2 loc, ivec2 dir) 
      {
         int v = map.Get(loc); 
         if (v == '.') {
            return true;
         } else if (v == '#') {
            return false; 
         }

         return false; 
      }

      void GetAllTouching(List<ivec2> touching, IntHeatMap2D map, ivec2 loc, ivec2 dir) 
      {
         int v = map.Get(loc); 
         if ((v == '#') || (v == '.')) {
            return; 
         }

         ivec2 bloc = loc; 
         if (v == ']') {
            bloc = loc + ivec2.LEFT; 
         } 

         if (touching.AddUnique(bloc)) 
         {
            ivec2 offset = dir; 
            GetAllTouching(touching, map, bloc + offset, dir); 
            GetAllTouching(touching, map, bloc + offset + ivec2.RIGHT, dir); 
         }
      }

      bool CanMove(IntHeatMap2D map, List<ivec2> blocks, ivec2 dir) 
      {
         // only care about walls - anything tht we would have been pushed into
         // should be in touching
         foreach (ivec2 bloc in blocks) {
            ivec2 nbloc = bloc + dir; 
            if ((map.Get(nbloc) == '#') || (map.Get(nbloc + ivec2.RIGHT) == '#')) {
               return false; 
            }
         }

         // if everyone can move, we're good
         return true; 
      }

      //----------------------------------------------------------------------------------------------
      ivec2 DoBigMove(IntHeatMap2D map, ivec2 loc, ivec2 dir) 
      {
         ivec2 nloc = loc + dir; 
         int v = map.Get(nloc); 
         if (v == '.') {
            return nloc; 
         } else if (v == '#') {
            return loc; 
         }

         List<ivec2> touching = new(); 

         GetAllTouching(touching, map, nloc, dir); 
         if (CanMove(map, touching, dir)) {
            // clear out old blocks
            foreach (ivec2 bloc in touching) {
               map.Set(bloc, '.'); 
               map.Set(bloc + ivec2.RIGHT, '.'); 
            }
            
            // put them in their new position
            foreach (ivec2 bloc in touching) 
            {
               ivec2 nbloc = bloc + dir; 
               map.Set(nbloc, '['); 
               map.Set(nbloc + ivec2.RIGHT, ']'); 
            }

            return nloc; 
         } 

         // couldn't move
         return loc; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntHeatMap2D map = GetBigMap(); 
         ivec2 loc = StartLocation * new ivec2(2, 1); 
         
         foreach (ivec2 move in Moves) {
            loc = DoBigMove(map, loc, move); 
         }

         return GetScore(map).ToString(); 
      }
   }
}
