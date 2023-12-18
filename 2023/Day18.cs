using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day18 : Day
   {
      private string InputFile = "2023/inputs/18.txt";

      class Input
      {
         public ivec2 Direction; 
         public int Value; 
         public int Hex; 

         public Input(string line)
         {
            string[] inputs = line.Split(' '); 
            Direction = inputs[0][0] switch {
               'D' => ivec2.DOWN, 
               'U' => ivec2.UP, 
               'L' => ivec2.LEFT, 
               'R' => ivec2.RIGHT, 
               _ => ivec2.ZERO
            }; 

            Value = int.Parse(inputs[1]); 

            string hex = inputs[2].Substring(2, 6); 
            Hex = int.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
         }

         public Input(int hex)
         {
            Value = (0x00fffff0 & hex) >> 4; 
            int dir = hex & 0xf; 
            Direction = dir switch {
               0 => ivec2.RIGHT, 
               1 => ivec2.DOWN, 
               2 => ivec2.LEFT, 
               _ => ivec2.UP
            }; 
            Hex = hex; 
         }
      }
      

      List<Input> Inputs = new List<Input>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         foreach (string line in lines) {
            Input input = new Input(line); 
            Inputs.Add(input); 
         }
      }

      //----------------------------------------------------------------------------------------------
      private int FindNextHit(ivec2 start, ivec2 end, List<ivec2> poly)
      {
         start.x++; // we will ignore hits at the start

         int hitWall = -1; 
         int leftMost = int.MaxValue; 

         for (int i = 1; i < poly.Count; ++i) {
            ivec2 p0 = poly[i -  1]; 
            ivec2 p1 = poly[i]; 
            int dx = (p1.x - p0.x); 

            // runs horizontal - doesn't factor into this problem
            if (dx != 0) {
               continue; 
            }

            if ((start.x > p0.x) || (end.x < p0.x)) { // doesn't cross
               continue; 
            }

            // swap, so it is always min to max
            int minY = Math.Min(p0.y, p1.y); 
            int maxY = Math.Max(p0.y, p1.y); 

            // doesn't cross
            if ((start.y < minY) || (start.y > maxY)) {
               continue; 
            }

            // okay, it hits
            if (p0.x < leftMost) {
               leftMost = p0.x; 
               hitWall = i; 
            }
         }


         return hitWall; 
      }

      //----------------------------------------------------------------------------------------------
      private int FollowEdges(ivec2 pos, List<ivec2> poly)
      {
         for (int i = 1; i < poly.Count; ++i) {
            if (poly[i].y != pos.y) {
               continue; 
            }

            int x0 = poly[i].x; 
            int x1 = poly[i - 1].x; 
            int sx = Math.Min(x0, x1); 
            int ex = Math.Max(x0, x1); 

            if ((ex > sx) && (sx == pos.x)) {
               return ex - 1; // move along to JUST before the end
            }
         }

         return pos.x; // no movement
      }

      //----------------------------------------------------------------------------------------------
      private Int64 CountInterior(ivec2 start, ivec2 end, List<ivec2> poly)
      {
         Int64 count = 0; 

         bool wasInside = false; 
         
         while (true) {
            int wall = FindNextHit(start, end, poly);
            if (wall != -1) { 
               // I know the poly is clockwise, so if we're crossing a line
               // that is going up, we're inside, otherwise outside
               int dir = poly[wall].y - poly[wall - 1].y; 
               bool inside = dir < 0;
               if (!inside && wasInside) {
                  count += (poly[wall].x - start.x - 1); // exclude walls
               }

               wasInside = inside;
               start.x = poly[wall].x; // move line to start on this wall

               if (inside) {
                  // if we're along any horizontal edges, follow it
                  start.x = FollowEdges(start, poly); 
               }
            } else {
               break; 
            }
         } 

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public Int64 CountInterior(List<ivec2> poly)
      {
         Int64 interior = 0; 

         ivec2 min = poly[0]; 
         ivec2 max = poly[0]; 
         foreach (ivec2 point in poly) {
            min = ivec2.Min(min, point); 
            max = ivec2.Max(max, point); 
         }

         for (int y = min.y + 1; y < max.y; ++y) {
            ivec2 start = new ivec2(min.x - 1, y); 
            ivec2 end = new ivec2(max.x + 1, y); 

            Int64 lineCount = CountInterior(start, end, poly);
            interior += lineCount; 

            // Util.WriteLine($"Line {y} has {lineCount}.  Total: {interior}"); 
         }

         return interior; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntCanvas dirt = new IntCanvas(1);

         ivec2 pos = ivec2.ZERO;
         dirt.SetValue(pos, 0);
         foreach (Input input in Inputs) {
            for (int i = 0; i < input.Value; ++i) {
               pos += input.Direction;
               dirt.SetValue(pos, 0);
            }
         }

         //cheating, there are no touching edges, and it is a clockwise path, so (1,1) is interior
         dirt.FloodFill(ivec2.ONE, 0);

         // Util.WriteLine(dirt.ToString(".#"));

         int space = dirt.Count(0);
         return space.ToString();
      }

      public void JoinEdges(List<ivec2> poly)
      {
         // each poly starts at a corner
         for (int i = 1; i < poly.Count - 1; ++i) {
            if ( ((poly[i].x == poly[i - 1].x) && (poly[i].x == poly[i + 1].x))
               || ((poly[i].y == poly[i - 1].y) && (poly[i].y == poly[i + 1].y)) ) {

               ivec2 t0 = ivec2.Sign(poly[i + 1] - poly[i]); 
               ivec2 t1 = ivec2.Sign(poly[i] - poly[i - 1]); 
               if (t1.Dot(t0) < 0) {
                  Util.WriteLine("bad clean?"); 
               }

               poly.RemoveAt(i); 
               --i; 
            }
         }
      }

      (int, ivec2) Raycast(List<ivec2> poly, int startCorner, ivec2 dir)
      {
         ivec2 start = poly[startCorner];
         start += dir;

         int bestDistance = int.MaxValue; 
         int bestEdge = 0; 
         ivec2 bestHit = start; 

         for (int i = 1; i < poly.Count; ++i) {
            ivec2 p1 = poly[i]; 
            ivec2 p0 = poly[i - 1]; 
            ivec2 d = ivec2.Sign(p1 - p0); 

            if (((p1 - start).Dot(dir) < 0) && ((p0 - start).Dot(dir) < 0)) {
               continue;
            }

            int distAlong = (start - p0).Dot(d); 
            if ((distAlong < 0) || (distAlong > (p1 - p0).GetManhattanDistance())) {
               continue; 
            }

            ivec2 nearest = d * (start - p0).Dot(d) + p0; 
            ivec2 toNearest = nearest - start; 
            int distanceToNearest = dir.Dot(toNearest); 

            if ((nearest == start) || (distanceToNearest > 0)) { 
               if (distanceToNearest < bestDistance) {
                  bestEdge = i; 
                  bestHit = nearest; 
                  bestDistance = distanceToNearest; 
               }
            }
         }

         return (bestEdge, bestHit);  // should not hit
      }

      (List<ivec2>, List<ivec2>?, Int64) ClipPolygon(List<ivec2> poly)
      {
         // look for a left turn
         for (int i = 1; i < poly.Count - 1; ++i) {
            ivec2 d0 = ivec2.Sign(poly[i] - poly[i - 1]); 
            ivec2 d1 = poly[i + 1] - poly[i - 1]; 
            
            ivec2 leftTurn = d0.GetRotatedLeft(); 
            int leftValue = leftTurn.Dot(d1); 
            if (leftValue > 0) {
               // turned left!  Okay, hit scan from the corner until we hit another wall
               (int idx, ivec2 hit) = Raycast(poly, i, d0);

               List<ivec2> first = new List<ivec2>();
               List<ivec2> clipped = new List<ivec2>();
               Int64 shared = (hit - poly[i]).GetManhattanDistance() + 1;

               if (idx < i) {
                  first.Add(hit); 
                  for (int j = idx; j <= i; ++j) {
                     first.Add(poly[j]); 
                  }
                  first.Add(hit); 

                  for (int j = 0; j < idx; ++j) {
                     clipped.Add(poly[j]); 
                  }
                  clipped.Add(hit); 
                  for (int j = i; j < poly.Count; ++j) {
                     clipped.Add(poly[j]); 
                  }
               } else {
                  clipped.Add(poly[i]); 
                  for (int j = 0; j < poly.Count; ++j) {
                     if ((j <= i) || (j >= idx)) {
                        first.Add(poly[j]); 
                     } else {
                        clipped.Add(poly[j]); 
                        if (j == idx - 1) {
                           first.Add(hit); 
                        }
                     }
                  }
                  clipped.Add(hit);
                  clipped.Add(poly[i]); 
               }

               return (first, clipped, shared); 
            }
         }

         return (poly, null, 0); 
      }

      void Validate(List<ivec2> poly)
      {
         for (int i = 1; i < poly.Count; ++i) {
            if ((poly[i].x != poly[i - 1].x) && (poly[i].y != poly[i - 1].y)) {
               Util.WriteLine("BAD"); 
            }
         }
      }

      public Int64 GetSquareVolume(List<ivec2> poly)
      {
         // Validate(poly); 

         ivec2 min = poly[0]; 
         ivec2 max = poly[0]; 
         foreach (ivec2 p in poly) {
            min = ivec2.Min(p, min); 
            max = ivec2.Max(p, max); 
         }

         Int64 width = Math.Abs(max.x - min.x) + 1;
         Int64 height = Math.Abs(max.y - min.y) + 1; 
         return width * height; 
      }

      public Int64 GetVolume(List<ivec2> polygon)
      {
         // next, see if there's an ear we can clip
         (List<ivec2> newPoly, List<ivec2>? clip, Int64 shared) = ClipPolygon(polygon); 
         if (clip != null) {
            JoinEdges(newPoly); 
            JoinEdges(clip); 

            return GetVolume(newPoly) + GetVolume(clip) - shared; 
         } else {
            // count should be 5, assuming a square
            return GetSquareVolume(newPoly);
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         List<Input> instructions = new List<Input>(); 
         foreach (Input input in Inputs) {
            Input newInput = new Input(input.Hex); 
            instructions.Add(newInput); 
         }

         ivec2 pos = ivec2.ZERO; 
         List<ivec2> polygon = new List<ivec2>(); 

         foreach (Input i in instructions) {

            polygon.Add(pos); 
            pos += i.Direction * i.Value; 
         }
         polygon.Add(pos); // pos should be zero

         /*
         foreach (ivec2 p in polygon) {
            double x = (double)p.x; 
            double y = (double)p.y;

            Util.WriteLine($"{x * 0.0001}, {y * 0.0001}"); 
         }
         */

         return GetVolume(polygon).ToString(); 
      }
   }
}
