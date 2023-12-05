using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   class Range
   {
      public Int64 Min; 
      public Int64 Max; 

      public Range(Int64 min, Int64 max)
      {
         Min = min; 
         Max = max; 
      }

      public bool IsIntersecting(Range other)
      {
         return Math.Max(Min, other.Min) <= Math.Min(Max, other.Max); 
      }

      public Range Intersect(Range other)
      {
         return new Range(Math.Max(Min, other.Min), Math.Min(Max, other.Max)); 
      }

      public Range Union(Range other)
      {
         return new Range(Math.Min(Min, other.Min), Math.Max(Max, other.Max)); 
      }

      public bool IsValid()
      {
         return Max >= Min; 
      }
   }

   class DisjointRange
   {
      public void RemoveRange(Range r)
      {
         for (int i = Ranges.Count - 1; i >= 0; --i) {
            Range other = Ranges[i]; 

            if (r.IsIntersecting(other)) {
               Range lh = new Range(other.Min, r.Min - 1); 
               Range rh = new Range(r.Max + 1, other.Max); 

               Ranges.RemoveAt(i); 
               if (rh.IsValid()) {
                  Ranges.Insert(i, rh); 
               }
               if (lh.IsValid()) {
                  Ranges.Insert(i, lh); 
               }
            }
         }
      }

      public void AddRange(Range r)
      {
         int insertIndex = -1; 

         for (int i = 0; i < Ranges.Count; ++i) {
            Range other = Ranges[i]; 
            if (r.IsIntersecting(other)) {
               r = r.Union(other); 
               if (insertIndex < 0) {
                  insertIndex = i; 
               }

               Ranges.RemoveAt(i); 
               --i; 
            }
         }

         if (insertIndex >= 0) {
            Ranges.Insert(insertIndex, r); 
         } else {
            Ranges.Add(new Range(r.Min, r.Max)); 
         } 
      }

      public List<Range> Ranges = new List<Range>();   
   }

   class RangeMap
   {
      public RangeMap(string line)
      {
         Int64[] ranges = line.Split(' ').Select(Int64.Parse).ToArray();
         DstStart = ranges[0];
         SrcStart = ranges[1];
         Count = ranges[2];
      }

      public bool Contains(Int64 input)
      {
         Int64 delta = input - SrcStart; 
         return (delta >= 0) && (delta < Count); 
      }

      public Int64 Map(Int64 input)
      {
         return DstStart + (input - SrcStart); 
      }

      public void MapRanges(ref DisjointRange src, ref DisjointRange dst)
      {
         Range r = new Range(SrcStart, SrcStart + Count - 1);
         
         bool tryAgain = true; 
         while (tryAgain) { 
            tryAgain = false ;
            foreach (Range srcRange in src.Ranges) {
               Range intersection = srcRange.Intersect(r); 
               if (intersection.IsValid()) {
                  src.RemoveRange(intersection); 
               
                  intersection.Min = intersection.Min - SrcStart + DstStart; 
                  intersection.Max = intersection.Max - SrcStart + DstStart; 
                  dst.AddRange(intersection); 
                  tryAgain = true; 
                  break; 
               }
            }
         }
      }

      Int64 DstStart;
      Int64 SrcStart;
      Int64 Count;
   }

   class Mapping
   {
      public void AddRange(string line)
      {
         Ranges.Add(new RangeMap(line)); 
      }

      public Int64 Map(Int64 input)
      {
         foreach (RangeMap rm in Ranges) {
            if (rm.Contains(input)) {
               return rm.Map(input); 
            }
         }

         return input; 
      }

      public void MapRanges(ref DisjointRange src, ref DisjointRange dst)
      {
         foreach (RangeMap rm in Ranges) {
            rm.MapRanges(ref src, ref dst); 
         }
      }

      List<RangeMap> Ranges = new List<RangeMap>(); 
   }


   internal class Day05 : Day
   {

      private string InputFile = "2023/inputs/05.txt";

      Int64[] Seeds = new Int64[0]; 
      List<Mapping> Mappings = new List<Mapping>();

      //----------------------------------------------------------------------------------------------
      int FindIndex(string[] lines, string line)
      {
         for (int i = 0; i < lines.Length; ++i) {
            if (lines[i] == line) {
               return i; 
            }
         }

         return -1; 
      }

      int FindEndIndex(string[] lines, int startIdx)
      {
         for (int i = startIdx; i < lines.Length; ++i) {
            if (string.IsNullOrEmpty(lines[i])) {
               return i; 
            }
         }

         return lines.Length; 
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string[] lines = Util.ReadFileToLines(InputFile).ToArray(); 

         Seeds = lines[0].Split(":")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();

         int startLine = 3;
         int line = startLine + 1;

         string[] titles = {
            "seed-to-soil map:",
            "soil-to-fertilizer map:",
            "fertilizer-to-water map:",
            "water-to-light map:",
            "light-to-temperature map:",
            "temperature-to-humidity map:",
            "humidity-to-location map:"
         };

         foreach (string title in titles) {
            int startIndex = FindIndex(lines, title) + 1;
            int endIndex = FindEndIndex(lines, startIndex);

            Mapping mapping = new Mapping(); 
            for (int i = startIndex; i < endIndex; ++i) {
               mapping.AddRange(lines[i]); 
            }

            Mappings.Add(mapping); 
         }
      }

      //----------------------------------------------------------------------------------------------
      Int64 MapSeed(Int64 seed)
      {
         Int64 result = seed; 
         foreach (Mapping map in Mappings) {
            result = map.Map(result); 
         }

         return result; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Int64 min = Int64.MaxValue; 
         foreach (Int64 seed in Seeds) {
            min = Math.Min(min, MapSeed(seed)); 
         }

         return min.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         DisjointRange src = new DisjointRange(); 
         for (int i = 0; i < Seeds.Count(); i += 2) {
            Int64 start = Seeds[i]; 
            Int64 count = Seeds[i + 1]; 
            src.AddRange(new Range(start, start + count - 1)); 
         }

         foreach (Mapping mapping in Mappings) {
            DisjointRange dst = new DisjointRange(); 
            mapping.MapRanges(ref src, ref dst); 

            foreach (Range r in src.Ranges) {
               dst.AddRange(r); 
            }

            src = dst; 
         }

         Int64 min = Int64.MaxValue; 
         foreach (Range r in src.Ranges) {
            min = Math.Min(r.Min, min); 
         }

         return min.ToString(); 
      }
   }
}
