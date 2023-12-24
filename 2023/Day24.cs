using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day24 : Day
   {
      private string InputFile = "2023/inputs/24.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Hail
      {
         public Hail(string line)
         {
            (string ps, string vs) = line.Split('@'); 

            var pos = ps.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray(); 
            var vel = vs.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray(); 

            x = pos[0]; 
            y = pos[1]; 
            z = pos[2]; 

            vx = vel[0]; 
            vy = vel[1]; 
            vz = vel[2]; 
         }

         //----------------------------------------------------------------------------------------------
         public double? GetIntersectTime2D(Hail other)
         {
            double dx = x - other.x;
            double dy = y - other.y; 
            double dvx = vx - other.vx; 
            double dvy = vy - other.vy; 

            double? tx = null; 
            double? ty = null; 

            // watch for parellel 
            if (dvx != 0) {
               tx = -dx / dvx; 
            } else if (dx == 0) {
               tx = double.MaxValue; 
            }

            if (dvy != 0) {
               ty = -dy / dvy; 
            } else if (dy == 0) {
               ty = double.MaxValue; 
            }

            if (tx.HasValue && ty.HasValue) {
               if ((tx.Value == double.MaxValue) && (ty.Value == double.MaxValue)) {
                  return 0.0; // started on each other
               } else if (ty.Value == double.MaxValue) {
                  return tx.Value; 
               } else if (tx.Value == double.MaxValue) {
                  return ty.Value; 
               } else if (tx.Value == ty.Value) {
                  return tx.Value; 
               }
            } 

            return null; 
         }

         public double? GetTrajectoryIntersection(Hail other)
         {
            vec2 p0 = new vec2(x, y); 
            vec2 v0 = new vec2(vx,vy); 
            vec2 n0 = v0.GetPerpendicular(); 

            vec2 p1 = new vec2(other.x, other.y); 
            vec2 v1 = new vec2(other.vx, other.vy); 
            vec2 dp = p1 - p0; 

            double dn = dp.Dot(n0); 
            double vn = v1.Dot(n0); 
            
            if (vn.IsNear(0.0f)) {
               if (dn.IsNear(0.0f)) {
                  double t = dp.Dot(v0) / v0.Dot(v0);
                  return t >= 0 ? t : null; 
               } else {
                  return null; 
               }
            } else {
               double t = -dn / vn; 
               if (t >= 0) {
                  vec2 i = dp + v1 * t; 
                  t = i.Dot(v0) / v0.Dot(v0); 
                  return t >= 0 ? t : null; 
               } else {
                  return null; 
               }
            }
         }

         public (double x, double y, double z) GetPositionAtTime(double t)
         {
            return (x + vx * t, y + vy * t, z + vz * t); 
         }

         
         public double x, y, z; 
         public double vx, vy, vz; 
      }

      private List<Hail> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Hail input = new Hail(line); 
            Inputs.Add(input); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // double minArea = 7; 
         //double maxArea = 27;
         double minArea = 200000000000000; 
         double maxArea = 400000000000000; 

         Int64 hits = 0; 
         for (int i = 0; i < Inputs.Count; ++i) {
            Hail hail = Inputs[i]; 
            for (int j = i + 1; j < Inputs.Count; ++j) {
               Hail other = Inputs[j]; 
               double? hitTime = hail.GetTrajectoryIntersection(other); 
               if ((hitTime.HasValue) && (hitTime.Value >= 0.0)) {
                  var pos = hail.GetPositionAtTime(hitTime.Value); 

                  if ((pos.x >= minArea) && (pos.x <= maxArea)
                      && (pos.y >= minArea) && (pos.y <= maxArea)) {
                     // Util.WriteLine($"{i},{j} intersect at {pos.x}, {pos.y}");
                     ++hits;
                  }
               } else {
                  // Util.WriteLine($"{i},{j} do not intersect."); 
               }
            }
         }

         return hits.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         // I think I can solve this along a single axis.  I mean, what are the odds there's be multiople X velocities but only a single y?
         // man, ven if that is the case, it limits my pool a ton...

         // no, single axis will have a _lot_ of solutions, the secondary axis helps limit it.  Third even more so... and each
         // will need to get solved to get the solution


         return ""; 
      }
   }
}
