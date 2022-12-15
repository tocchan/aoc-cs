using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   // Defines an inclusive range between min and max
   internal class IntRange
   {
      public int Min; 
      public int Max; 

      //----------------------------------------------------------------------------------------------
      public IntRange()
      {
         Min = 1; 
         Max = -1; 
      }

      //----------------------------------------------------------------------------------------------
      public IntRange( IntRange copy )
      {
         Min = copy.Min; 
         Max = copy.Max; 
      }

      //----------------------------------------------------------------------------------------------
      public IntRange( int min, int max )
      {
         Min = min; 
         Max = max; 
      }

      public int Count => IsValid() ? (Max - Min) + 1 : 0; 


      //----------------------------------------------------------------------------------------------
      // Contracts from a string that looks like
      // > "1-5"
      // would give an IntRange(Min = 1, Max = 5)
      static public IntRange Parse( string range )
      {
         (int min, int max) = range.Split('-').Select(int.Parse).ToArray(); 
         return new IntRange( min, max ); 
      }

      //----------------------------------------------------------------------------------------------
      public void Set( IntRange other )
      {
         Min = other.Min; 
         Max = other.Max; 
      }

      //----------------------------------------------------------------------------------------------
      public bool IsValid()
      {
         return Max >= Min; 
      }

      public bool Contains( int v ) => ((v >= Min) && (v <= Max));

      //----------------------------------------------------------------------------------------------
      public bool Contains( IntRange other )
      {
         return (other.Min >= Min) && (other.Max <= Max); 
      }

      //----------------------------------------------------------------------------------------------
      public bool Intersects( IntRange other )
      {
         return Math.Min(Max, other.Max) >= Math.Max(Min, other.Min); 
      }

      //----------------------------------------------------------------------------------------------
      public IntRange GetIntersection( IntRange other )
      {
         return new IntRange( Math.Max(Min, other.Min), Math.Min(Max, other.Max) ); 
      }

      //----------------------------------------------------------------------------------------------
      public IntRange GetUnion( IntRange other )
      {
         return new IntRange( Math.Min(Min, other.Min), Math.Max(Max, other.Max) ); 
      }

      public override string ToString()
      {
         return $"[{Min}~{Max}], Count = {Count}"; 
      }
   }
}
