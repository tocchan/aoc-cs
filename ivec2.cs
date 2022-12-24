using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{

   public enum eDirection
   {
      Right = 0, 
      Left, 
      Up, 
      Down, 
      None,
   }

   public enum eCardinal
   {
      East, 
      NorthEast, 
      North, 
      NorthWest, 
      West, 
      SouthWest, 
      South, 
      SouthEast
   };

   public struct ivec2
   {
      public int x = 0;
      public int y = 0;

      public static readonly ivec2 ZERO = new ivec2(0, 0);
      public static readonly ivec2 ONE = new ivec2(1, 1);

      public static readonly ivec2 LEFT = new ivec2(-1, 0);
      public static readonly ivec2 RIGHT = new ivec2(1, 0);
      public static readonly ivec2 UP = new ivec2(0, -1);
      public static readonly ivec2 DOWN = new ivec2(0, 1);
      public static readonly ivec2[] DIRECTIONS = { RIGHT, LEFT, UP, DOWN }; 
      public static readonly ivec2[] CARDINAL = {
         RIGHT, 
         RIGHT + UP, 
         UP, 
         UP + LEFT, 
         LEFT, 
         DOWN + LEFT, 
         DOWN, 
         DOWN + RIGHT
      };


      public ivec2(int v)
      {
         x = v;
         y = v;
      }

      public ivec2(int xv, int yv)
      {
         x = xv;
         y = yv;
      }

      public bool IsZero() => ((x == 0) && (y == 0)); 
      public int Sum() => x + y;
      public int Product() => x * y;
      public int MaxElement() => Math.Max(x, y); 

      public ivec2 GetRotatedLeft() => new ivec2(y, -x);
      public void RotateLeft() => this = GetRotatedLeft();

      public ivec2 GetRotatedRight() => new ivec2(-y, x);
      public void RotateRight() => this = GetRotatedRight();

      public int Dot(ivec2 v) => x * v.x + y * v.y;
      public int GetLengthSquared() => x * x + y * y;
      public float GetLength() => MathF.Sqrt((float)GetLengthSquared());
      
      public int GetManhattanDistance() => Abs(this).Sum();

      public ivec2 GetBestDirectionTo(ivec2 p)
      {
         ivec2 diff = p - this;  
         if (diff.IsZero()) { 
            return ivec2.ZERO; 
         } else {
            ivec2 dir = diff / ivec2.Abs(diff).MaxElement(); 
            return ivec2.Sign(dir); 
         }
      }

      // get a point in the 8 cells around me closest to p
      public ivec2 GetNearestNeighbor(ivec2 p)
      {
         ivec2 dir = GetBestDirectionTo(p); 
         return this + dir; 
      }

      public int this[int i]
      {
         get { return (i == 0) ? x : y; }
         set { if (i == 0) { x = value; } else { y = value; } }
      }

      //----------------------------------------------------------------------------------------------
      // operators
      //----------------------------------------------------------------------------------------------

      public static ivec2 operator +(ivec2 v) => v;
      public static ivec2 operator -(ivec2 v) => new ivec2(-v.x, -v.y);
      public static ivec2 operator +(ivec2 a, ivec2 b) => new ivec2(a.x + b.x, a.y + b.y);
      public static ivec2 operator -(ivec2 a, ivec2 b) => new ivec2(a.x - b.x, a.y - b.y);
      public static ivec2 operator -(ivec2 a, int b) => new ivec2(a.x - b, a.y - b);
      public static ivec2 operator *(ivec2 a, ivec2 b) => new ivec2(a.x * b.x, a.y * b.y);
      public static ivec2 operator *(int a, ivec2 v) => new ivec2(a * v.x, a * v.y);
      public static ivec2 operator *(ivec2 v, int a) => new ivec2(a * v.x, a * v.y);
      public static ivec2 operator /(ivec2 v, int a) => new ivec2(v.x / a, v.y / a);
      public static ivec2 operator %(ivec2 a, ivec2 b) => new ivec2(a.x % b.x, a.y % b.y);
      public static ivec2 operator %(ivec2 v, int a) => new ivec2(v.x % a, v.y % a);
      public static bool operator ==(ivec2 a, ivec2 b) => (a.x == b.x) && (a.y == b.y);
      public static bool operator !=(ivec2 a, ivec2 b) => (a.x != b.x) || (a.y != b.y);
      public static bool operator <(ivec2 a, ivec2 b) => (a.x < b.x) && (a.y < b.y);
      public static bool operator <=(ivec2 a, ivec2 b) => (a.x <= b.x) && (a.y <= b.y);
      public static bool operator >(ivec2 a, ivec2 b) => (a.x > b.x) && (a.y > b.y);
      public static bool operator >=(ivec2 a, ivec2 b) => (a.x >= b.x) && (a.y >= b.y);

      public static ivec2 Sign(ivec2 v) => new ivec2(Math.Sign(v.x), Math.Sign(v.y));
      public static ivec2 Min(ivec2 a, ivec2 b) => new ivec2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
      public static ivec2 Max(ivec2 a, ivec2 b) => new ivec2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
      public static ivec2 Clamp(ivec2 v, ivec2 lh, ivec2 rh) { return Min(rh, Max(lh, v)); }
      public static ivec2 Abs(ivec2 v) => new ivec2(Math.Abs(v.x), Math.Abs(v.y));
      public static ivec2 Max(IEnumerable<ivec2> list)
      {
         ivec2 ret = list.First();
         foreach (ivec2 v in list) {
            ret = ivec2.Max(ret, v);
         }

         return ret;
      }

      public static ivec2 Mod(ivec2 val, ivec2 den) => new ivec2( Util.Mod(val.x, den.x), Util.Mod(val.y, den.y) ); 
      public static ivec2 Mod(ivec2 val, int den) => new ivec2( Util.Mod(val.x, den), Util.Mod(val.y, den) ); 

      public static ivec2 FloorToBoundary(ivec2 val, int boundary)
      {
         ivec2 offset = ivec2.Mod(val, boundary);
         return val - offset;
      }

      public static ivec2 CeilToBoundary(ivec2 val, int boundary)
      {
         ivec2 ret = ivec2.ZERO; 
         ret.x = (int) Util.CeilToBoundary(val.x, boundary); 
         ret.y = (int) Util.CeilToBoundary(val.y, boundary); 
         return ret; 
      }

      public static ivec2 CeilToPow2(ivec2 val)
      {
         return new ivec2(
            Util.CeilToPow2(val.x), 
            Util.CeilToPow2(val.y)
         ); 
      }

      public static int Dot(ivec2 a, ivec2 b) => a.x * b.x + a.y * b.y;

      public static ivec2 Parse(string s)
      {
         string[] parts = s.Split(',', 2);
         int x = int.Parse(parts[0]);
         int y = int.Parse(parts[1]);
         return new ivec2(x, y);
      }

      public override bool Equals([NotNullWhen(true)] object? obj)
      {
         if ((obj == null) || !obj.GetType().Equals(GetType())) {
            return false;
         }

         ivec2 other = (ivec2)obj;
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

      public static IEnumerable<ivec2> EnumArea( ivec2 size, ivec2 origin )
      {
         ivec2 r = origin; 
         for (int y = 0; y < size.y; ++y) {
            r.x = origin.x; 
            for (int x = 0; x < size.x; ++x) {
               yield return r; 
               ++r.x; 
            }
            ++r.y;
         }
      }

      public static IEnumerable<ivec2> EnumArea( ivec2 size )
      {
         return EnumArea( size, ivec2.ZERO ); 
      }

   }
}
