﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
   static class Util
   {
      //----------------------------------------------------------------------------------------------
      public static List<string> ReadFileToLines(string filename)
      {
         string[] lines;
         try {
            lines = File.ReadAllLines(filename);
            List<string> ret = new List<string>(lines);

            // remove erroneous empty lines at the end
            while ((ret.Count > 0) && string.IsNullOrEmpty(ret.Last())) {
               ret.RemoveAt(ret.Count - 1);
            }

            return ret;
         } catch (Exception e) {
            Console.WriteLine("File read failed: " + e.ToString());
            return new List<string>();
         }
      }

      //----------------------------------------------------------------------------------------------
      public static string ReadFileToString(string filename)
      {
         try {
            return File.ReadAllText(filename);

         } catch (Exception e) {
            Console.WriteLine("File read failed: " + e.ToString());
            return String.Empty;
         }
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 BinaryStringToInt(string s)
      {
         return Convert.ToInt64(s, 2);
      }

      //----------------------------------------------------------------------------------------------
      public static string ApplyMarkup(string str)
      {
         (string, string)[] list = {
                ("-",        "\u001b[0m"),
                ("black",    "\u001b[30m"),
                ("red",      "\u001b[31m"),
                ("green",    "\u001b[32m"),
                ("yellow",   "\u001b[33m"),
                ("blue",     "\u001b[34m"),
                ("magenta",  "\u001b[35m"),
                ("cyan",     "\u001b[36m"),
                ("white",    "\u001b[37m"),
                ("+black",   "\u001b[30;1m"),
                ("+red",     "\u001b[31;1m"),
                ("+green",   "\u001b[32;1m"),
                ("+yellow",  "\u001b[33;1m"),
                ("+blue",    "\u001b[34;1m"),
                ("+magenta", "\u001b[35;1m"),
                ("+cyan",    "\u001b[36;1m"),
                ("+white",   "\u001b[37;1m")
            };

         // forgive me...
         while (str.Contains("[ ")) {
            str = str.Replace("[ ", "[");
         }

         // todo: bug that I'm not escaping the '<' character, but not a case I need so ignoring it; 
         // todo: currently unhandled types will be left in, would be nice to cleanse and warn about them
         foreach ((string find, string replace) in list) {
            string search = $"[{find}]";
            str = str.Replace(search, replace);
         }

         // always cleanup at the end of a line; 
         return $"{str}{list[0].Item2}";
      }

      //----------------------------------------------------------------------------------------------
      public static void WriteLine(string line)
      {
         Console.WriteLine(ApplyMarkup(line));
      }

      public static bool IsTraceEnabled = false; 
      public static void TraceLine(string line)
      {
         if (IsTraceEnabled) {
            WriteLine(line); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public static void WriteArray<T>(IEnumerable<T> array)
      {
         WriteLine(array.ToString() + " {");
         foreach (T item in array) {
            string itemStr = (item != null) ? item.ToString()! : "<null>";
            WriteLine("   " + itemStr + ",");
         }
         WriteLine("}");
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 Mod( Int64 lh, Int64 rh )
      {
         Int64 r = lh % rh; 
         if (r < 0) {
            return r + rh; 
         } else {
            return r; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public static (float, float) QuadraticF(float a, float b, float c)
      {
         float inner = b * b - 4 * a * c;
         if (inner < 0) {
            return (float.NaN, float.NaN);
         }

         inner = MathF.Sqrt(inner);
         float ansA = (-b - inner) / (2 * a);
         float ansB = (-b + inner) / (2 * a);

         return (Math.Min(ansA, ansB), Math.Max(ansA, ansB));
      }

      //----------------------------------------------------------------------------------------------
      public static (double, double) Quadratic(double a, double b, double c)
      {
         double inner = b * b - 4 * a * c;
         if (inner < 0) {
            return (double.NaN, double.NaN);
         }

         inner = Math.Sqrt(inner);
         double ansA = (-b - inner) / (2 * a);
         double ansB = (-b + inner) / (2 * a);

         return (Math.Min(ansA, ansB), Math.Max(ansA, ansB));
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 CeilToBoundary(Int64 val, Int64 boundary)
      {
         Int64 rem = val % boundary;
         if (rem == 0) {
            return val;
         } else {
            return val + (boundary - rem);
         }
      }

      //----------------------------------------------------------------------------------------------
      public static int CeilToPow2(int val)
      {
         if (val < 2) {
            return 1; 
         } else { 
            --val; 
            val |= (val >> 1); 
            val |= (val >> 2); 
            val |= (val >> 4); 
            val |= (val >> 8); 
            val |= (val >> 16); 
            return val + 1; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public static int HexToByte(char c)
      {
         return (c <= '9') ? (c - '0') : (c - 'A' + 10);
      }

      //----------------------------------------------------------------------------------------------
      public static List<int> GetIndexList(int count)
      {
         List<int> list = new List<int>(count);
         for (int i = 0; i < count; ++i) {
            list.Add(i);
         }

         return list;
      }

      //----------------------------------------------------------------------------------------------
      public static T[] GetSubsetByRemovingAt<T>(this T[] array, int idx, int count = 1)
      {
         count = Math.Clamp(count, 0, array.Length - idx);
         int newCount = Math.Max(array.Length - count, 0);
         T[] newArray = new T[newCount];

         for (int i = 0; i < idx; ++i) {
            newArray[i] = array[i];
         }

         for (int i = idx; i < newCount; ++i) {
            newArray[i] = array[i + count];
         }

         return newArray;
      }

      //----------------------------------------------------------------------------------------------
      private static (int, int)[] PermutePairs(int v0, int[] set)
      {
         if (set.Length == 1) {
            return new (int, int)[] { (v0, set[0]) };
         }

         List<(int, int)> sets = new List<(int, int)>();
         for (int i = 0; i < set.Length; ++i) {
            int v1 = set[i];
            int[] subset = set.GetSubsetByRemovingAt(i);
            (int, int)[] subsetPairs = PermutePairs(v0 + 1, subset);

            for (int j = 0; j < subsetPairs.Length; j += subset.Length) {
               sets.Add((v0, v1));
               for (int k = 0; k < subset.Length; ++k) {
                  sets.Add(subsetPairs[j + k]);
               }
            }
         }

         return sets.ToArray();
      }

      //----------------------------------------------------------------------------------------------
      // Given 0 to N inputs and outputs, will 
      // generate all possible ways to hook them up.  
      // Returned array will be size X * N, where X is the number of possible hookups
      public static (int, int)[] PermutePairs(int setSize)
      {
         int[] initialSet = Enumerable.Range(0, setSize).ToArray();
         return PermutePairs(0, initialSet);
      }

      //----------------------------------------------------------------------------------------------
      // Returns all permuations of the set (in order)
      // todo: contains a lot of sub allocations, could probably use an optimization pass if my 
      public static IEnumerable<T[]> GetPermutations<T>(List<T> set)
      {
         if (set.Count <= 1) {
            yield return set.ToArray();
         } else {
            List<T> setCopy = new List<T>();
            setCopy.AddRange(set);

            for (int i = 0; i < set.Count; ++i) {
               T item = set[i];
               T[] perm = new T[set.Count];
               perm[0] = item;

               setCopy.RemoveAt(i);
               foreach (T[] subperm in GetPermutations<T>(setCopy)) {
                  for (int j = 0; j < subperm.Length; ++j) {
                     perm[j + 1] = subperm[j];
                  }
                  yield return perm;
               }
               setCopy.Insert(i, item);
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public static IEnumerable<int[]> GetPermutations(int n)
      {
         List<int> indices = GetIndexList(n);
         return GetPermutations<int>(indices);
      }

      //----------------------------------------------------------------------------------------------
      public static IEnumerable<int[]> GetPermutations(int minInclusive, int maxInclusive)
      {
         int count = maxInclusive - minInclusive + 1;
         List<int> indices = GetIndexList(count);
         for (int i = 0; i < indices.Count; ++i) {
            indices[i] += minInclusive;
         }

         return GetPermutations<int>(indices);
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 GCD(Int64 num, Int64 den)
      {
         num = Math.Abs(num);
         den = Math.Abs(den);

         while ((num * den) != 0) {
            Int64 r = num % den;
            num = den;
            den = r;
         }

         return Math.Max(num, den);
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 LCM(Int64 a, Int64 b)
      {
         Int64 gcd = GCD(a, b); 
         return (a / gcd) * b; 
      }

      //----------------------------------------------------------------------------------------------
      public static Int64 LCM(IEnumerable<Int64> numbers)
      {
         Int64 lcm = 1; 
         foreach (Int64 num in numbers) {
            lcm = Util.LCM(num, lcm); 
         }

         return lcm; 
      }

      //----------------------------------------------------------------------------------------------
      static private Int64 GetNextInPlace(List<Int64> sequence)
      {
         if (sequence.Count == 0) {
            return 0; 
         } else if (sequence.Count == 1) {
            return sequence[0]; 
         }

         bool isAllZero = false;
         int diffCount = sequence.Count;
         while (!isAllZero) {
            isAllZero = true;
            --diffCount; // this will leave the last element unaffected

            for (int i = 0; i < diffCount; ++i) {
               sequence[i] = sequence[i + 1] - sequence[i];
               isAllZero = isAllZero && (sequence[i] == 0);
            }
         }

         Int64 sum = 0;
         while (diffCount < sequence.Count) {
            sum += sequence[diffCount];
            ++diffCount;
         }

         return sum;
      }

      //----------------------------------------------------------------------------------------------
      static public Int64 GetNextIn(IEnumerable<Int64> sequence)
      {
         List<Int64> copy = new List<Int64>(); 
         copy.AddRange(sequence); 
         return GetNextInPlace(copy); 
      }

      //----------------------------------------------------------------------------------------------
      static public Int64 GetPreviousIn(List<Int64> sequence)
      {
         List<Int64> copy = new List<Int64>();
         copy.AddRange(sequence);
         copy.Reverse(); 
         return GetNextInPlace(copy);
      }

      //----------------------------------------------------------------------------------------------
      public static bool IsDigit(char c) => (c >= '0') && (c <= '9'); 

      public static bool IsLetter(char c) => ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')); 

      public static bool IsAlphaNumeric(char c) => IsDigit(c) || IsLetter(c); 

      //----------------------------------------------------------------------------------------------
      public static string GetRepeatedChar(char c, int count) 
      {
         if (count == 0) {
            return ""; 
         }

         char[] str = new char[count]; 
         for (int i = 0; i < count; ++i) {
            str[i] = c; 
         }

         return new string(str); 
      }
   }
}
