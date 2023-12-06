using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   // Defines a collection of ranges
   internal class DisjointIntRange
   {
      public LinkedList<IntRange> Ranges = new LinkedList<IntRange>();

      public bool Contains(int value)
      {
         if (IsEmpty()) {
            return false; 
         }

         if (value > Ranges.Last().Max) {
            return false; 
         }

         foreach (IntRange range in Ranges) {
            if (value < range.Min) {
               return false; 
            } else if (value <= range.Max) {
               return true; 
            } // eles, carry on
         }

         return false; // honestly shouldn't get here - early out up top should have caught it
      }

      public bool IsEmpty()
      {
         return Ranges.Count == 0; 
      }

      public void AddRange(IntRange range)
      {
         if (!range.IsValid()) {
            return; 
         }

         IntRange newRange = new IntRange(range);

         if (Ranges.Count == 0) {
            Ranges.AddFirst(newRange); 
         } else {
            var iter = Ranges.First; 
            while (iter != null) {
               var nextIter = iter.Next; 

               // join with things if we intersect.
               if (newRange.Intersects(iter.Value) || ((newRange.Max + 1) == iter.Value.Min)) {
                  newRange = iter.Value.GetUnion(newRange); 
                  Ranges.Remove(iter);
               } else if (newRange.Max < iter.Value.Min) {
                  Ranges.AddBefore(iter, newRange); 
                  return; // add in the new item before the next
               } // else, just try the next item.

               iter = nextIter; 
            }

            // got to the end?  Add it to the end
            Ranges.AddLast(newRange); 
         }
      }

      public void RemoveRange(IntRange range)
      {
         var iter = Ranges.First; 

         while (iter != null) {
            var iterNext = iter.Next; 
            if (range.Max < iter.Value.Min) {
               return; // nothing left to check
            } else if (range.Intersects(iter.Value)) {
               if (range.Contains(iter.Value)) {
                  Ranges.Remove(iter); // fully contained, just remove it. 
               } else {
                  IntRange lh = new IntRange(iter.Value.Min, range.Min - 1); 
                  IntRange rh = new IntRange(range.Max + 1, iter.Value.Max); 
                  if (lh.IsValid()) {
                     Ranges.AddBefore(iter, lh); 
                  }
                  if (rh.IsValid()) {
                     Ranges.AddBefore(iter, rh); 
                  }
                  Ranges.Remove(iter); 
               }
            }

            iter = iterNext; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public Int64 GetCount()
      {
         Int64 count = 0;
         foreach (IntRange r in Ranges) {
            count += r.Count;
         }

         return count;
      }

      //----------------------------------------------------------------------------------------------
      public void SetLowerBound(Int64 v)
      {
         while (Ranges.Count > 0) {
            IntRange r = Ranges.First();
            if (r.Max < v) {
               Ranges.RemoveFirst();
            } else if (r.Contains(v)) {
               r.Min = v;
               return;
            } else if (r.Min >= v) {
               return; // done; 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public int GetRangeCount() => Ranges.Count;

      //----------------------------------------------------------------------------------------------
      public IntRange? GetRange(int idx)
      {
         if ((idx < 0) || (idx >= Ranges.Count)) {
            return null; 
         }

         int iter = 0; 
         foreach (IntRange range in Ranges) {
            if (iter == idx) {
               return range; 
            }
            ++iter; 
         }

         return null; 
      }
   }
}
