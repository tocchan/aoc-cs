using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
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

      public void MapRanges(ref DisjointIntRange src, ref DisjointIntRange dst)
      {
         IntRange r = new IntRange(SrcStart, SrcStart + Count - 1);
         
         bool tryAgain = true; 
         while (tryAgain) { 
            tryAgain = false ;
            foreach (IntRange srcRange in src.Ranges) {
               IntRange intersection = srcRange.GetIntersection(r); 
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

      public void MapRanges(ref DisjointIntRange src, ref DisjointIntRange dst)
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
         DisjointIntRange src = new DisjointIntRange(); 
         for (int i = 0; i < Seeds.Count(); i += 2) {
            Int64 start = Seeds[i]; 
            Int64 count = Seeds[i + 1]; 
            src.AddRange(new IntRange(start, start + count - 1)); 
         }

         foreach (Mapping mapping in Mappings) {
            DisjointIntRange dst = new DisjointIntRange(); 
            mapping.MapRanges(ref src, ref dst); 

            foreach (IntRange r in src.Ranges) {
               dst.AddRange(r); 
            }

            src = dst; 
         }

         Int64 min = Int64.MaxValue; 
         foreach (IntRange r in src.Ranges) {
            min = Math.Min(r.Min, min); 
         }

         return min.ToString(); 
      }
   }
}
