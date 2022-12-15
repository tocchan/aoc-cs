using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   // Defines a set of ints, defined by a list of disjoint int ranges
   // so save on space/time.
   internal class IntSet
   {
      //----------------------------------------------------------------------------------------------
      public void Add(IntRange range)
      {
         IntRange? firstIntersection = null; 
         int intCount = 0; 
         IntRange finalRange = new IntRange(range); 

         int idx = 0; 
         for (idx = 0; idx < DisjointRanges.Count; ++idx) {
            IntRange r = DisjointRanges[idx]; 
            if (range.Intersects(r)) {
               if (firstIntersection == null) { 
                  firstIntersection = r; 
               }
               finalRange = finalRange.GetUnion(r); 
               ++intCount; 
            } else if (range.Max < r.Min) {
               break; // moved past, idx is not my insert location
            }
         }

         // either remove excess ranges that were joined...
         if (firstIntersection != null) {
            firstIntersection.Set(finalRange); 
            if (intCount > 1) {
               int count = intCount - 1; 
               DisjointRanges.RemoveRange(idx - count, count); // remove all intersections that were joined
            }
         } else { // ...or insert the new range in sorted order
            DisjointRanges.Insert(idx, finalRange); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public int GetCount()
      {
         int count = 0; 
         foreach (IntRange r in DisjointRanges) {
            count += r.Count; 
         }
         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public void SetLowerBound(int v)
      {
         while (DisjointRanges.Count > 0) {
            IntRange r = DisjointRanges[0]; 
            if (r.Max < v) {
               DisjointRanges.RemoveAt(0); 
            } else if (r.Contains(v)) {
               r.Min = v;
               return; 
            } else if (r.Min >= v) {
               return; // done; 
            }
         }
      }

      public int GetRangeCount() => DisjointRanges.Count; 
      public IntRange GetRange(int idx) => DisjointRanges[idx]; 

      // Disjoint list of ranges - sorted by mins
      List<IntRange> DisjointRanges = new(); 
   }
}
