using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day22 : Day
   {
      private string InputFile = "2022/inputs/22.txt";

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      public class Tile
      {
         public int Index = 0; 
         public ivec2 Location = ivec2.ZERO; 
         public IntHeatMap2D Map = new IntHeatMap2D(); 

         public Tile?[] wrap = new Tile?[4]; 
         public Tile?[] cube = new Tile?[4]; 
         public mat22[] trans = new mat22[4]; 

         public Tile GetWrap(eDirection dir) 
         {
            Tile? next = wrap[(int)dir]; 
            if (next != null) {
               return next; 
            }

            int opIdx = (int) dir.Negate(); 

            Tile iter = this; 
            while (iter.wrap[opIdx] != null) {
               iter = iter.wrap[opIdx]!; 
            }

            return iter; 
         }
      }

      string[] GetSubSection(string[] input, int x, int y, int w, int h) 
      {
         if ((x >= input[y].Length) || (input[y][x] == ' ')) {
            return new string[0]; 
         } 

         string[] ret = new string[h]; 
         for (int iy = 0; iy < h; ++iy) {
            ret[iy] = input[iy + y].Substring(x, w);
         }

         return ret; 
      }

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      public class Cursor 
      {
         public Cursor( Tile start )
         {
            CurTile = start; 
         }

         public Tile CurTile; 
         public ivec2 Position = ivec2.ZERO; 
         public ivec2 Direction = ivec2.RIGHT; 

         //----------------------------------------------------------------------------------------------
         public void MoveA( ivec2 ins ) 
         {
            for (int i = 0; i < ins.x; ++i) {
               MoveOneA(); 
            }

            if (ins.y == 0) {
               Direction.RotateLeft(); 
            } else if (ins.y == 1) {
               Direction.RotateRight(); 
            }
         }

         //----------------------------------------------------------------------------------------------
          public void MoveB( ivec2 ins ) 
         {
            for (int i = 0; i < ins.x; ++i) {
               MoveOneB(); 
            }

            if (ins.y == 0) {
               Direction.RotateLeft(); 
            } else if (ins.y == 1) {
               Direction.RotateRight(); 
            }
         }

         //----------------------------------------------------------------------------------------------
         public void MoveOneA() 
         {
            Tile newTile = CurTile; 
            ivec2 newPos = Position + Direction; 

            if (!CurTile.Map.ContainsPoint(newPos)) {
               if (Direction == ivec2.RIGHT) {
                  newTile = CurTile.GetWrap(eDirection.Right); 
                  newPos.x = 0; 
               } else if (Direction == ivec2.LEFT) {
                  newTile = CurTile.GetWrap(eDirection.Left); 
                  newPos.x = CurTile.Map.GetWidth() - 1; 
               } else if (Direction == ivec2.UP) {
                  newTile = CurTile.GetWrap(eDirection.Up); 
                  newPos.y = CurTile.Map.GetHeight() - 1; 
               } else if (Direction == ivec2.DOWN) {
                  newTile = CurTile.GetWrap(eDirection.Down); 
                  newPos.y = 0; 
               }
            }

            if (newTile.Map.Get(newPos) == 0) {
               CurTile = newTile; 
               Position = newPos; 
            }
         }

         //----------------------------------------------------------------------------------------------
         public void MoveOneB() 
         {
            Tile newTile = CurTile; 
            ivec2 newPos = Position + Direction; 
            ivec2 newDirection = Direction; 

            if (!CurTile.Map.ContainsPoint(newPos)) {
               eDirection nd = Direction.ToDirection(); 
               newTile = CurTile.cube[(int)nd]!; 
               mat22 trans = CurTile.trans[(int)nd]; 
               mat22 inv = trans.GetTranspose(); 

               ivec2 r = trans.i; 

               // okay, depending on how this translates, can think of the 
               // origin of the tile as being in one of four corners of the new tile
               // (cube has no flips)
               ivec2 size = CurTile.Map.GetSize(); 
               ivec2 origin = size * Direction; 
               if (r == ivec2.DOWN) {
                  origin.x += size.x - 1; 
               } else if (r == ivec2.LEFT) {
                  origin += size - ivec2.ONE; 
               } else if (r == ivec2.UP) {
                  origin.y += size.y - 1; 
               }

               // get offset from origin
               ivec2 offset = newPos - origin; 

               newPos = offset * inv; // put this offset into the faces local space
               newDirection = Direction * inv; 
            }

            if (newTile.Map.Get(newPos) == 0) {
               CurTile = newTile; 
               Position = newPos; 
               Direction = newDirection; 
            }
         }

         //----------------------------------------------------------------------------------------------
         public ivec2 GetRealLocation()
         {
            ivec2 tileLoc = CurTile.Location * CurTile.Map.GetSize(); 
            return tileLoc + Position; 
         }

         //----------------------------------------------------------------------------------------------
         public int GetFacingValue() 
         {
            if (Direction == ivec2.RIGHT) {
               return 0; 
            } else if (Direction == ivec2.DOWN) {
               return 1;
            } else if (Direction == ivec2.LEFT) {
               return 2; 
            } else {
               return 3; 
            }
         }

         //----------------------------------------------------------------------------------------------
         public override string ToString()
         {
            string facing = ">v<^"; 
            return $"Tile:{CurTile.Index}  Local:{Position.ToString().PadRight(7)}  World:{GetRealLocation().ToString().PadRight(7)}  Facing: {facing[GetFacingValue()]}"; 
         }
      }

      //----------------------------------------------------------------------------------------------
      Tile? Start = null;
      List<Tile> AllTiles = new(); 
      List<ivec2> Instructions = new(); 

      //----------------------------------------------------------------------------------------------
      Tile? FindTileAt(ivec2 loc) 
      {
         return AllTiles.Find( t => t.Location == loc ); 
      }

      //----------------------------------------------------------------------------------------------
      (Tile?, mat22) FindConnection( Tile source, eDirection passThrough, ivec2 toNeighbor )
      {
         Tile? n = source.cube[(int)passThrough]; 
         if (n == null) {
            return (null, mat22.Identity); 
         }

         // what is "up" in my neighbors point of view
         mat22 trans = source.trans[(int)passThrough]; 
         ivec2 dirToDst = toNeighbor * trans.GetTranspose(); 
         eDirection toShared = dirToDst.ToDirection(); 

         Tile? shared = n.cube[(int)toShared]; 
         if (shared == null) {
            return (null, mat22.Identity); 
         }

         trans = trans * n.trans[(int)toShared]; 
         return (shared, trans); 
      }

      //----------------------------------------------------------------------------------------------
      // return if a new connection was formed
      public bool ConnectCubePass()
      {
         foreach (Tile tile in AllTiles) {
            for (int id = 0; id < 4; ++id) {
               eDirection dir = (eDirection)id; 
               Tile? n = tile.cube[(int)dir]; 

               if (n == null) {
                  // if I'm missing this direction, so I have a left or right connetion I can travel to to find it?
                  eDirection[] try_dirs = { dir.RotateLeft(), dir.RotateRight() }; 
                  mat22[] final_trans = { mat22.RotateRight, mat22.RotateLeft }; 

                  for (int it = 0; it < 2; ++it) {
                     eDirection try_dir = try_dirs[it]; 
                     (n, mat22 t) = FindConnection( tile, try_dir, ivec2.DIRECTIONS[(int)dir] ); 
                     if (n != null) {
                        if ((tile.Index == 0) && (n.Index == 1)) {
                           Util.WriteLine("boo"); 
                        }
                        tile.cube[(int)dir] = n; 
                        tile.trans[(int)dir] = t * final_trans[it]; 
                        return true; 
                     }
                  }
               }
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      public void ConnectCube()
      {
         // do immediate, non transform connections
         foreach (Tile tile in AllTiles) {
            for (int i = 0; i < 4; ++i) {
               ivec2 dir = ivec2.DIRECTIONS[i]; 
               Tile? n = FindTileAt(tile.Location + dir); 

               tile.trans[i] = mat22.Identity; // direct connection
               if (n != null) {
                  tile.cube[i] = n; 
               }
            }
         }

         while (ConnectCubePass()) {}
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string[] lines = Util.ReadFileToLines(InputFile).ToArray();

         int maxLen = lines.Take(lines.Length - 2).Select( s => s.Length ).Max(); 

         int colCount = ((maxLen % 4) == 0) ? 4 : 3; 
         int sideLen = maxLen / colCount; 
         int rowCount = (lines.Length - 2) / sideLen; 

         int tileIdx = 0; 
         for (int iy = 0; iy < rowCount; ++iy) {
            for (int ix = 0; ix < colCount; ++ix) {
               string[] sub = GetSubSection(lines, ix * sideLen, iy * sideLen, sideLen, sideLen); 
               if ((sub.Length == 0) || (sub[0][0] == ' ')) {
                  continue; 
               }

               Tile tile = new Tile(); 
               tile.Index = tileIdx; 
               tile.Location = new ivec2(ix, iy); 
               tile.Map.SetFromTightBlock(sub, ".#"); 
               AllTiles.Add(tile); 

               if (Start == null) {
                  Start = tile; 
               }

               ++tileIdx; 
            }
         }

         // get wrap connections
         foreach (Tile tile in AllTiles) {
            for (int i = 0; i < 4; ++i) {
               ivec2 dir = ivec2.DIRECTIONS[i]; 
               Tile? n = FindTileAt(tile.Location + dir); 
               tile.wrap[i] = n; 
            }
         }

         // okay, finish up cube connections
         ConnectCube(); 

         // get instructions
         string dirs = lines.Last(); 
         while (dirs.Length > 0) {
            if (!dirs.Contains('R') && !dirs.Contains('L')) {
               Instructions.Add( new ivec2( int.Parse(dirs), -1 ) ); 
               dirs = ""; 
            } else {
               char dir = dirs.First( c => c == 'L' || c == 'R' ); 
               (string scount, dirs) = dirs.Split(dir, 2); 
               int count = int.Parse(scount); 
               ivec2 ins = new ivec2( count,  dir == 'L' ? 0 : 1 ); 
               Instructions.Add(ins); 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Cursor c = new Cursor(Start!); 

         // Util.WriteLine(c.ToString()); 
         foreach (ivec2 ins in Instructions) {
            c.MoveA(ins); 
            // Util.WriteLine(c.ToString()); 
         }

         ivec2 pos = c.GetRealLocation() + ivec2.ONE; 
         int facing = c.GetFacingValue(); 

         int result = 1000 * pos.y 
            + 4 * pos.x 
            + facing; 

         return result.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      private string InsToString( ivec2 ins ) 
      {
         string s = ins.x.ToString(); 
         if (ins.y == 0) {
            s += 'L'; 
         } else if (ins.y == 1) {
            s += 'R';
         }
         return s.PadLeft(4); 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Cursor c = new Cursor(Start!); 

         // Util.WriteLine("     Start -> " + c.ToString()); 
         int insIdx = 1; 
         foreach (ivec2 ins in Instructions) {
            c.MoveB(ins); 
            // Util.WriteLine($"{insIdx.ToString().PadLeft(4)}. {InsToString(ins)} -> {c}"); 
            ++insIdx; 
         }

         ivec2 pos = c.GetRealLocation() + ivec2.ONE; 
         int facing = c.GetFacingValue(); 

         int result = 1000 * pos.y 
            + 4 * pos.x 
            + facing; 

         return result.ToString(); 
      }
   }
}
