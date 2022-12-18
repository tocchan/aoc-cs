using AoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day18 : Day
   {
      private string InputFile = "2022/inputs/18.txt";

      ivec3[] Blocks = new ivec3[0]; 
      HashSet<ivec3> Map = new(); 


      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Blocks = lines.Select(ivec3.Parse).ToArray(); 
         for (int i = 0; i < Blocks.Length; ++i) {
            // taking advantage of my input set.  Min was (0, 0, 0), so pushing it to (1, 1, 1) to avoid having to offset later. 
            Blocks[i] = Blocks[i] + ivec3.ONE;
            Map.Add(Blocks[i]); 
         }  
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int count = 0; 
         foreach (ivec3 b in Blocks) {
            foreach (ivec3 d in ivec3.DIRECTIONS) {
               if (!Map.Contains(b + d)) {
                  ++count; 
               }
            }
         }

         return count.ToString(); 
      }

      private bool Contains(ivec3 p, ivec3 s)
      {
         return (p >= ivec3.ZERO) && (p < s); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         ivec3 minBlock = Blocks[0]; 
         ivec3 maxBlock = Blocks[0]; 
         foreach (ivec3 b in Blocks) {
            minBlock = ivec3.Min(b, minBlock); 
            maxBlock = ivec3.Max(b, maxBlock); 
         }

         maxBlock += ivec3.ONE; // we know this will be air... so... this is where we're start; 
         ivec3 visitSize = maxBlock + ivec3.ONE; 
         // min block was at zero, that makes this easier.

         BitArray visited = new BitArray(visitSize.Product()); 
         int slice = visitSize.y * visitSize.x; 
         int width = visitSize.x; 
         ivec3 toIdx = new ivec3(1, width, slice); 

         // flood fill the exterior
         Stack<ivec3> toCheck = new();
         toCheck.Push(maxBlock); 
         visited[ivec3.Dot(maxBlock, toIdx)] = true; 

         while (toCheck.Count > 0) {
            ivec3 pos = toCheck.Pop(); 
            int idx = ivec3.Dot(pos, toIdx); 

            foreach (ivec3 d in ivec3.DIRECTIONS) {
               ivec3 newPos = pos + d; 
               int newIdx = ivec3.Dot(newPos, toIdx); 
               if (!Contains(newPos, visitSize) || visited[newIdx] || Map.Contains(newPos)) {
                  continue; 
               }

               visited[newIdx] = true; 
               toCheck.Push(newPos); 
            }
         }

         // count exterior walls
         int count = 0; 
         foreach (ivec3 b in Blocks) {
            foreach (ivec3 d in ivec3.DIRECTIONS) {
               ivec3 p = b + d; 
               int idx = ivec3.Dot(p, toIdx); 
               if (!Map.Contains(p) && (!Contains(p, visitSize) || visited[idx])) {
                  ++count; 
               }
            }
         }

         return count.ToString(); 
      }
   }
}
