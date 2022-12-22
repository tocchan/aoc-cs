using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   public struct mat22
   {
      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      public ivec2 c0;
      public ivec2 c1;

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------

      //----------------------------------------------------------------------------------------------
      // Identity
      public mat22()
      {
         c0 = new ivec2(1, 0); 
         c1 = new ivec2(0, 1); 
      }

      //----------------------------------------------------------------------------------------------
      public mat22( ivec2 i, ivec2 j )
      {
         c0.x = i.x; 
         c0.y = j.x; 
         c1.x = i.y; 
         c1.y = j.y; 
      }


      //----------------------------------------------------------------------------------------------
      public ivec2 i {
         get => new ivec2(c0.x, c1.x);
         set {
            c0.x = value.x; 
            c1.x = value.y; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public ivec2 j {
         get => new ivec2(c0.y, c1.y);
         set {
            c0.y = value.x; 
            c1.y = value.y; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public void Transpose()
      {
         int t = c0.y; 
         c0.y = c1.x; 
         c1.x = t; 
      }

      //----------------------------------------------------------------------------------------------
      public mat22 GetTranspose()
      {
         mat22 ret = this; 
         ret.Transpose();
         return ret; 
      }

      //----------------------------------------------------------------------------------------------
      public static ivec2 operator *(ivec2 v, mat22 t) 
      {
         return new ivec2( t.c0.Dot(v), t.c1.Dot(v) ); 
      }

      //----------------------------------------------------------------------------------------------
      public static mat22 operator *(mat22 lh, mat22 rh) 
      {
         ivec2 ni = lh.i * rh; 
         ivec2 nj = lh.j * rh; 
         return new mat22( ni, nj ); 
      }

      public static bool operator ==(mat22 a, mat22 b) => (a.c0 == b.c0) && (a.c1 == b.c1); 
      public static bool operator !=(mat22 a, mat22 b) => (a.c0 != b.c0) || (a.c1 != b.c1); 


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      public static readonly mat22 Zero = new mat22( ivec2.ZERO, ivec2.ZERO ); 
      public static readonly mat22 Identity = new mat22();  
      public static readonly mat22 RotateRight = new mat22( ivec2.DOWN, ivec2.LEFT );
      public static readonly mat22 RotateLeft = new mat22( ivec2.UP, ivec2.RIGHT );

      //----------------------------------------------------------------------------------------------
      // Object overrides
      //----------------------------------------------------------------------------------------------

      //----------------------------------------------------------------------------------------------
      public override string ToString()
      {
         if (this == Identity) {
            return "I"; 
         } else if (this == mat22.Zero) {
            return "0";
         } else {
            return $"{c0.x},{c1.x}\n{c0.y},{c1.y}";
         }
      }

      //----------------------------------------------------------------------------------------------
      public override bool Equals([NotNullWhen(true)] object? obj)
      {
         if ((obj == null) || !obj.GetType().Equals(GetType())) {
            return false;
         }

         mat22 other = (mat22)obj;
         return this == other; 
      }

      //----------------------------------------------------------------------------------------------
      public override int GetHashCode()
      {
         return HashCode.Combine( c0.GetHashCode(), c1.GetHashCode() ); 
      }
   }
}
