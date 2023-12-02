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
      // sets all elements in a list to a value
      public static void SetAll<T>(this List<T> list, T val)
      {
         for (int i = 0; i < list.Count; ++i) {
            list[i] = val; 
         }
      }

      //----------------------------------------------------------------------------------------------
      // sets all elements in a list to a value
      public static void SetAll<T>(this T[] list, T val)
      {
         for (int i = 0; i < list.Length; ++i) {
            list[i] = val; 
         }
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

      //----------------------------------------------------------------------------------------------
      public static eDirection Negate( this eDirection d ) => d switch 
      {
         eDirection.Right => eDirection.Left, 
         eDirection.Left => eDirection.Right, 
         eDirection.Up => eDirection.Down, 
         eDirection.Down => eDirection.Up,
         _ => eDirection.None,
      }; 

      //----------------------------------------------------------------------------------------------
      public static eDirection RotateLeft( this eDirection d ) => d switch 
      {
         eDirection.Right => eDirection.Up, 
         eDirection.Left => eDirection.Down, 
         eDirection.Up => eDirection.Left, 
         eDirection.Down => eDirection.Right,
         _ => eDirection.None,
      }; 

      //----------------------------------------------------------------------------------------------
      public static eDirection RotateRight( this eDirection d ) => d switch 
      {
         eDirection.Right => eDirection.Down, 
         eDirection.Left => eDirection.Up, 
         eDirection.Up => eDirection.Right, 
         eDirection.Down => eDirection.Left,
         _ => eDirection.None,
      }; 

      //----------------------------------------------------------------------------------------------
      public static ivec2 ToVector( this eDirection d )
      {
         return d == eDirection.None ? ivec2.ZERO : ivec2.DIRECTIONS[(int)d]; 
      }

      //----------------------------------------------------------------------------------------------
      public static eDirection ToDirection( this ivec2 v )
      {
         for (int i = 0; i < 4; ++i) {
            if (ivec2.DIRECTIONS[i] == v) {
               return (eDirection)i; 
            }
         }
         return eDirection.None;
      }
   }
}
