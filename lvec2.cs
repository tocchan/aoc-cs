using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{   public struct lvec2
   {
      public Int64 x = 0;
      public Int64 y = 0;

      public static readonly lvec2 ZERO = new lvec2(0, 0);
      public static readonly lvec2 ONE = new lvec2(1, 1);

      public static readonly lvec2 LEFT = new lvec2(-1, 0);
      public static readonly lvec2 RIGHT = new lvec2(1, 0);
      public static readonly lvec2 UP = new lvec2(0, -1);
      public static readonly lvec2 DOWN = new lvec2(0, 1);
      public static readonly lvec2[] DIRECTIONS = { RIGHT, LEFT, UP, DOWN }; 
      public static readonly lvec2[] CARDINAL = {
         RIGHT, 
         RIGHT + UP, 
         UP, 
         UP + LEFT, 
         LEFT, 
         DOWN + LEFT, 
         DOWN, 
         DOWN + RIGHT
      };


      public lvec2(Int64 v)
      {
         x = v;
         y = v;
      }

      public lvec2(Int64 xv, Int64 yv)
      {
         x = xv;
         y = yv;
      }

      public bool IsZero() => ((x == 0) && (y == 0)); 
      public Int64 Sum() => x + y;
      public Int64 Product() => x * y;
      public Int64 MaxElement() => Math.Max(x, y); 

      public lvec2 GetRotatedLeft() => new lvec2(y, -x);
      public void RotateLeft() => this = GetRotatedLeft();

      public lvec2 GetRotatedRight() => new lvec2(-y, x);
      public void RotateRight() => this = GetRotatedRight();

      public Int64 Dot(lvec2 v) => x * v.x + y * v.y;
      public Int64 GetLengthSquared() => x * x + y * y;
      public float GetLength() => MathF.Sqrt((float)GetLengthSquared());

      public lvec2 GetReduced()
      {
         Int64 gcd = (Int64) Util.GCD(x, y); 
         if (gcd > 1) {
            return new lvec2(x / gcd, y / gcd); 
         } else {
            return this; 
         }
      }

      public bool IsColinear(lvec2 other) 
      {
         Int64 det = x * other.y - other.x * y; 
         return det == 0; 
      }
      
      public Int64 GetManhattanDistance() => Abs(this).Sum();

      public Int64 GetDirectionBit() 
      {
         for (Int64 i = 0; i < CARDINAL.Length; ++i) {
            if (CARDINAL[i] == this) {
               return i; 
            }
         }

         return CARDINAL.Length; // anything non cardinal we'll just use the last bit for
      }

      public lvec2 GetBestDirectionTo(lvec2 p)
      {
         lvec2 diff = p - this;  
         if (diff.IsZero()) { 
            return lvec2.ZERO; 
         } else {
            lvec2 dir = diff / lvec2.Abs(diff).MaxElement(); 
            return lvec2.Sign(dir); 
         }
      }

      // get a point in the 8 cells around me closest to p
      public lvec2 GetNearestNeighbor(lvec2 p)
      {
         lvec2 dir = GetBestDirectionTo(p); 
         return this + dir; 
      }

      public Int64 this[Int64 i]
      {
         get { return (i == 0) ? x : y; }
         set { if (i == 0) { x = value; } else { y = value; } }
      }

      //----------------------------------------------------------------------------------------------
      // operators
      //----------------------------------------------------------------------------------------------

      public static lvec2 operator +(lvec2 v) => v;
      public static lvec2 operator -(lvec2 v) => new lvec2(-v.x, -v.y);
      public static lvec2 operator +(lvec2 a, lvec2 b) => new lvec2(a.x + b.x, a.y + b.y);
      public static lvec2 operator -(lvec2 a, lvec2 b) => new lvec2(a.x - b.x, a.y - b.y);
      public static lvec2 operator -(lvec2 a, Int64 b) => new lvec2(a.x - b, a.y - b);
      public static lvec2 operator *(lvec2 a, lvec2 b) => new lvec2(a.x * b.x, a.y * b.y);
      public static lvec2 operator *(Int64 a, lvec2 v) => new lvec2(a * v.x, a * v.y);
      public static lvec2 operator *(lvec2 v, Int64 a) => new lvec2(a * v.x, a * v.y);
      public static lvec2 operator /(lvec2 v, Int64 a) => new lvec2(v.x / a, v.y / a);
      public static lvec2 operator %(lvec2 a, lvec2 b) => new lvec2(a.x % b.x, a.y % b.y);
      public static lvec2 operator %(lvec2 v, Int64 a) => new lvec2(v.x % a, v.y % a);
      public static bool operator ==(lvec2 a, lvec2 b) => (a.x == b.x) && (a.y == b.y);
      public static bool operator !=(lvec2 a, lvec2 b) => (a.x != b.x) || (a.y != b.y);
      public static bool operator <(lvec2 a, lvec2 b) => (a.x < b.x) && (a.y < b.y);
      public static bool operator <=(lvec2 a, lvec2 b) => (a.x <= b.x) && (a.y <= b.y);
      public static bool operator >(lvec2 a, lvec2 b) => (a.x > b.x) && (a.y > b.y);
      public static bool operator >=(lvec2 a, lvec2 b) => (a.x >= b.x) && (a.y >= b.y);

      public static lvec2 Sign(lvec2 v) => new lvec2(Math.Sign(v.x), Math.Sign(v.y));
      public static lvec2 Min(lvec2 a, lvec2 b) => new lvec2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
      public static lvec2 Max(lvec2 a, lvec2 b) => new lvec2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
      public static lvec2 Clamp(lvec2 v, lvec2 lh, lvec2 rh) { return Min(rh, Max(lh, v)); }
      public static lvec2 Abs(lvec2 v) => new lvec2(Math.Abs(v.x), Math.Abs(v.y));
      public static lvec2 Max(IEnumerable<lvec2> list)
      {
         lvec2 ret = list.First();
         foreach (lvec2 v in list) {
            ret = lvec2.Max(ret, v);
         }

         return ret;
      }

      public static lvec2 Mod(lvec2 val, lvec2 den) => new lvec2( (Int64) Util.Mod(val.x, den.x), (Int64) Util.Mod(val.y, den.y) ); 
      public static lvec2 Mod(lvec2 val, Int64 den) => new lvec2( (Int64) Util.Mod(val.x, den), (Int64) Util.Mod(val.y, den) ); 

      public static lvec2 FloorToBoundary(lvec2 val, Int64 boundary)
      {
         lvec2 offset = lvec2.Mod(val, boundary);
         return val - offset;
      }

      public static lvec2 CeilToBoundary(lvec2 val, Int64 boundary)
      {
         lvec2 ret = lvec2.ZERO; 
         ret.x = (Int64) Util.CeilToBoundary(val.x, boundary); 
         ret.y = (Int64) Util.CeilToBoundary(val.y, boundary); 
         return ret; 
      }

      public static Int64 Dot(lvec2 a, lvec2 b) => a.x * b.x + a.y * b.y;

      public static lvec2 Parse(string s)
      {
         string[] parts = s.Split(',', 2);
         Int64 x = Int64.Parse(parts[0]);
         Int64 y = Int64.Parse(parts[1]);
         return new lvec2(x, y);
      }

      public override bool Equals([NotNullWhen(true)] object? obj)
      {
         if ((obj == null) || !obj.GetType().Equals(GetType())) {
            return false;
         }

         lvec2 other = (lvec2)obj;
         return (x == other.x) && (y == other.y);
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(x.GetHashCode(), y.GetHashCode());
      }

      public override string ToString()
      {
         return $"{x},{y}";
      }

      public static IEnumerable<lvec2> EnumArea( lvec2 size, lvec2 origin )
      {
         lvec2 r = origin; 
         for (Int64 y = 0; y < size.y; ++y) {
            r.x = origin.x; 
            for (Int64 x = 0; x < size.x; ++x) {
               yield return r; 
               ++r.x; 
            }
            ++r.y;
         }
      }

      public static IEnumerable<lvec2> EnumArea( lvec2 size )
      {
         return EnumArea( size, lvec2.ZERO ); 
      }

   }
}
