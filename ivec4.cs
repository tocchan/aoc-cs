using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   public struct ivec4
   {
      public int x = 0;
      public int y = 0;
      public int z = 0;
      public int w = 0; 

      public static readonly ivec4 ZERO = new ivec4(0, 0, 0, 0);
      public static readonly ivec4 ONE = new ivec4(1, 1, 1, 1);

      public ivec4(int v)
      {
         x = v;
         y = v;
         z = v;
         w = 0; 
      }

      public ivec4(int xv, int yv, int zv, int wv)
      {
         x = xv;
         y = yv;
         z = zv;
         w = wv; 
      }

      public int this[int i]
      {
         get => i switch {
            0 => x,
            1 => y,
            2 => z,
            3 => w,
            _ => 0
         };

         set {
            switch (i) {
               case 0: x = value; break;
               case 1: y = value; break;
               case 2: z = value; break;
               case 3: w = value; break; 
               default: break;
            }
         }
      }

      public void Add( ivec4 other )
      {
         x += other.x; 
         y += other.y; 
         z += other.z; 
         w += other.y; 
      }

      public int Max() 
      {
         return Math.Max( Math.Max(x, y), Math.Max(z, w) ); 
      }

      public int GetMaxIndex()
      {
         int max = Max(); 
         for (int i = 1; i < 4; ++i) {
            if (this[i] == max) {
               return i; 
            }
         }
         return 0; 
      }

      public static ivec4 operator +(ivec4 v) => v;
      public static ivec4 operator -(ivec4 v) => new ivec4(-v.x, -v.y, -v.z, -v.w);
      public static ivec4 operator +(ivec4 a, ivec4 b) => new ivec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
      public static ivec4 operator -(ivec4 a, ivec4 b) => new ivec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
      public static ivec4 operator *(ivec4 a, ivec4 b) => new ivec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
      public static ivec4 operator *(int a, ivec4 v) => new ivec4(a * v.x, a * v.y, a * v.z, a * v.w);
      public static ivec4 operator *(ivec4 v, int a) => new ivec4(a * v.x, a * v.y, a * v.z, a * v.w);
      public static ivec4 operator /(ivec4 a, ivec4 b) => new ivec4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
      public static ivec4 operator /(ivec4 v, int a) => new ivec4(v.x / a, v.y / a, v.z / a, v.w / a);
      public static bool operator ==(ivec4 a, ivec4 b) => (a.x == b.x) && (a.y == b.y) && (a.z == b.z) && (a.w == b.w);
      public static bool operator !=(ivec4 a, ivec4 b) => (a.x != b.x) || (a.y != b.y) || (a.z != b.z) || (a.w != b.w);
      public static bool operator <(ivec4 a, ivec4 b) => (a.x < b.x) && (a.y < b.y) && (a.z < b.z) && (a.w < b.w);
      public static bool operator <=(ivec4 a, ivec4 b) => (a.x <= b.x) && (a.y <= b.y) && (a.z <= b.z) && (a.w <= b.w);
      public static bool operator >(ivec4 a, ivec4 b) => (a.x > b.x) && (a.y > b.y) && (a.z > b.z) && (a.w > b.w);
      public static bool operator >=(ivec4 a, ivec4 b) => (a.x >= b.x) && (a.y >= b.y) && (a.z >= b.z) && (a.w >= b.w);

      public static ivec4 Min(ivec4 a, ivec4 b) => new ivec4(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z), Math.Min(a.w, b.w));
      public static ivec4 Max(ivec4 a, ivec4 b) => new ivec4(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z), Math.Max(a.w, b.w));
      public static ivec4 Abs(ivec4 v) => new ivec4(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z), Math.Abs(v.w));

      public static ivec4 Sign(ivec4 v) => new ivec4( Math.Sign(v.x), Math.Sign(v.y), Math.Sign(v.z), Math.Sign(v.w) ); 

      public static ivec4 Parse(string s)
      {
         s = s.Trim();
         if (string.IsNullOrEmpty(s)) {
            return ivec4.ZERO;
         }

         // assume "#, #, #, #" format
         int[] parts = s.Split(',', 4).Select(int.Parse).ToArray(); 
         return new ivec4(parts[0], parts[1], parts[2], parts[3]); 
      }

      public override bool Equals([NotNullWhen(true)] object? obj)
      {
         if ((obj == null) || !obj.GetType().Equals(GetType())) {
            return false;
         }

         ivec4 other = (ivec4)obj;
         return this == other; 
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(x.GetHashCode(), HashCode.Combine(y.GetHashCode(), z.GetHashCode(), w.GetHashCode()));
      }

      public override string ToString()
      {
         return $"{x},{y},{z},{w}";
      }
   }
}
