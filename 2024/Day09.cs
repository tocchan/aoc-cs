using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day09 : Day
   {
      private string InputFile = "2024/inputs/09.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      List<Int64> InitialDisk = new(); 
      private Int64 HighestId = 0; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string line = Util.ReadFileToLines(InputFile)[0]; 

         Int64 id = 0; 
         bool isFile = true; 
         foreach (char c in line) {
            Int64 count = c - '0'; 
            if (isFile) {
               for (Int64 i = 0; i < count; ++i) {
                  InitialDisk.Add(id); 
               }
               HighestId = id; 
               ++id; 
            } else {
               for (Int64 i = 0; i < count; ++i) {
                  InitialDisk.Add(-1); 
               }
            }

            isFile = !isFile; 
         }
      }

      private Int64 Checksum(List<Int64> disk) 
      {
         Int64 val = 0; 
         for (int i = 0; i < disk.Count; ++i) {
            Int64 v = disk[i]; 
            if (v >= 0) {
               val += v * i; 
            }
         }

         return val; 
      }

      int FindInsert(List<Int64> disk, int startIdx) 
      {
         for (int i = startIdx; i < disk.Count; ++i) {
            if (disk[i] < 0) {
               return i; 
            }
         }

         return disk.Count; 
      }

      int FindPull(List<Int64> disk, int idx) 
      {
         while (idx >= 0) {
            if (disk[idx] >= 0) {
               return idx; 
            }
            --idx; 
         }

         return 0; 
      }

      private void Pack(List<Int64> disk) 
      {
         int insertIdx = FindInsert(disk, 0); 
         int pullIdx = FindPull(disk, disk.Count - 1); 

         while (pullIdx > insertIdx) {
            disk[insertIdx] = disk[pullIdx]; 
            disk[pullIdx] = -1; 

            insertIdx = FindInsert(disk, insertIdx + 1); 
            pullIdx = FindPull(disk, pullIdx - 1); 
         }
      }

      ivec2 FindFile(List<Int64> disk, Int64 id) 
      {
         int idx = disk.FindIndex((Int64 v) => v == id); 
         int count = 0; 
         while ((idx + count < disk.Count) && (disk[idx + count] == id)) {
            ++count; 
         }

         return new ivec2(idx, count); 
      }

      int FindSpace(List<Int64> disk, int space) 
      {
         for (int i = 0; i < disk.Count; ++i) {
            if (disk[i] == -1) {
               int count = 0; 
               while ((i + count < disk.Count) && (disk[i + count] == -1)) {
                  ++count; 
               }

               if (count >= space) {
                  return i; 
               }
            }
         }

         return disk.Count; 
      }

      //----------------------------------------------------------------------------------------------
      private void PackFull(List<Int64> disk) 
      {
         Int64 idToFind = HighestId; 
         
         while (idToFind >= 0) {
            ivec2 pullSpan = FindFile(disk, idToFind); 
            int insertIdx = FindSpace(disk, pullSpan.y); 
            if (insertIdx < pullSpan.x) 
            {
               for (int i = 0; i < pullSpan.y; ++i) {
                  disk[insertIdx + i] = disk[pullSpan.x + i]; 
                  disk[pullSpan.x + i] = -1; 
               }
            }
            --idToFind; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         List<Int64> disk = new List<Int64>(InitialDisk); 
         Pack(disk); 

         return Checksum(disk).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         List<Int64> disk = new List<Int64>(InitialDisk); 
         PackFull(disk); 
         return Checksum(disk).ToString(); 
      }
   }
}
