using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   public static class Extensions
   {
      //----------------------------------------------------------------------------------------------
      // Allow (x, y, ...) = list; 
      public static void Deconstruct<T>(this IList<T> list, out T first, out T second)
      {
         first = list.Count > 0 ? list[0] : default!;
         second = list.Count > 1 ? list[1] : default!;
      }

      //----------------------------------------------------------------------------------------------
      public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
      {
         first = list.Count > 0 ? list[0] : default!;
         second = list.Count > 1 ? list[1] : default!;
         rest = list.Skip(2).ToList();
      }

      //----------------------------------------------------------------------------------------------
      // Splits the list at first element that passes the predicate. 
      // return value is all items before the match, 
      // and the list will contain all elements after the match.  The match is removed.
      public static List<T> SplitWhen<T>(this List<T> list, Predicate<T> predicate)
      {
         int splitIdx = list.FindIndex(predicate); 
         if (splitIdx < 0) {
            List<T> ret = new List<T>(list); 
            list.Clear(); 
            return ret; 
         } else {
            List<T> ret = list.GetRange(0, splitIdx); 
            list.RemoveRange(0, splitIdx + 1); // remove the split item
            return ret; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public static List<T> SplitOn<T>(this List<T> list, T splitter)
      {
         return list.SplitWhen<T>(x => (x != null) && x.Equals(splitter)); 
      }

      //----------------------------------------------------------------------------------------------
      public static IEnumerable<List<T>> SplitAllWhen<T>(this List<T> list, Predicate<T> predicate)
      {
         List<T> copy = new List<T>(list); 
         List<T> group = copy.SplitWhen<T>(predicate); 

         // list check is because if we have two splitters in a row, it is still a valid split, but group will be nil, with the list not being
         while ((group.Count > 0) || (copy.Count > 0)) {
            yield return group; 
            group = copy.SplitWhen<T>(predicate); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public static IEnumerable<List<T>> SplitAllOn<T>(this List<T> list, T splitter)
      {
         return list.SplitAllWhen<T>(x => (x != null) && x.Equals(splitter));
      }

   }
}
