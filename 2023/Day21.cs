using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day21 : Day
   {
      private string InputFile = "2023/inputs/21.txt";

      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      IntHeatMap2D Map = new IntHeatMap2D(); 
      ivec2 Start = ivec2.ZERO; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Map.SetFromTightBlock(lines.ToArray(), ".#S", -1); 

         Start = (ivec2)(Map.FindLocation((ivec2 p, int value) => value == 2))!; 
         Map.Set(Start, 0); 
      }

      class Cache
      {
         public IntHeatMap2D FillMap = new IntHeatMap2D(); 
         public Int64 EvenCount = 0; 
         public Int64 OddCount = 0; 
         public Int64 MaxCount = 0; 
         public int Parity = 0; 

         public Cache(IntHeatMap2D map, ivec2[] starts, int[] startCosts, int stepsRemaining)
         {
            int maxSteps = startCosts.Max();
            int minSteps = startCosts.Min();
            Parity = stepsRemaining % 2; 

            int[] unused;
            int[] costs = new int[starts.Length]; 
            for (int i = 0; i < startCosts.Length; ++i) {
               costs[i] = startCosts[i] - minSteps;
            }

            FillMap = map.DijkstraFlood(starts, costs,
               (ivec2 dst, ivec2 src) => map[dst] == 0 ? 1 : -1, 
               out unused);

            EvenCount = GetCount(0, int.MaxValue - 1); 
            OddCount = GetCount(1, int.MaxValue - 1); 

            foreach ((ivec2 pos, int v) in FillMap) {
               if (v != int.MaxValue) {
                  MaxCount = Math.Max(v, MaxCount); 
               }
            }
         }

         public Int64 GetCount(int parity) => (parity == 0) ? EvenCount : OddCount; 

         public Int64 GetCount(int parity, int stepCount)
         {
            int p = Math.Abs(Parity - parity); 
            return FillMap.Count((int v) => {
               return (v <= stepCount)
                  && ((v % 2) == p);
            });
         }
      }

      Int64 CountQuadrant(ivec2 c0, ivec2 c1, ivec2 cornerStart, int[] edgeCosts, int stepsRemaining)
      {
         int minEdgeCost = edgeCosts.Min();

         Int64 cardinalCount = 0; 
         Int64 cornerCount = 0; 

         List<ivec2> edgePoints = new(); 
         ivec2 d = ivec2.Sign(c1 - c0); 
         for (ivec2 p = c0; p != c1; p += d) {
            edgePoints.Add(p); 
         }
         edgePoints.Add(c1); 

         int cornerOffset = (cornerStart == c0) ? edgeCosts[0] : edgeCosts.Last(); 
         cornerOffset = cornerOffset - minEdgeCost + 1;  

         // todo, passing in costs was because sample input had different corner costs
         // real input though doesn't matter
         int width = Map.GetWidth();

         int counter = stepsRemaining - minEdgeCost - 1;
         Cache edge = new Cache(Map, edgePoints.ToArray(), edgeCosts.ToArray(), 1);
         Cache corner = new Cache(Map, new ivec2[] { cornerStart }, new int[] { 0 }, 0); // parity of neighbor tile is reversed. 

         int parity = 0; 
         Int64 tiles = 0;
         while (counter >= 0) {
            ++tiles;

            // bug should be here, but the sum of all these counts doesn't
            // even add up to the error... :confused:  
            if (counter >= edge.MaxCount) {
               cardinalCount += edge.GetCount(parity); // bug isn't here, multiple would cause it to be way off
            
            } else {
               Int64 edgePlots = edge.GetCount(parity, counter);
               cardinalCount += edgePlots; 
               // Util.WriteLine($"edge: {edgePlots}"); 
            }

            // don't think the bug is here
            // if anything was even off by 1, I'd be wrong in the millions
            int cornerSteps = counter - cornerOffset; 
            int cornerParity = 1 - parity;  
            if (cornerSteps >= corner.MaxCount) {
               cornerCount += corner.GetCount(cornerParity) * (tiles); // count diagonal (startin above me, counting back)
            } else if (cornerSteps >= 0) { 
               Int64 cornerPlots = corner.GetCount(cornerParity, cornerSteps); 
               // Util.WriteLine($"corner: {cornerPlots} * {tiles} : {15004 - cornerPlots}"); 
               cornerCount += cornerPlots * tiles; 
            }

            counter -= width;
            parity = 1 - parity; 
         }

         // Util.WriteLine($"c:{cardinalCount}, d:{cornerCount}"); 
         return cardinalCount + cornerCount; 
      }

      public void Draw(IntHeatMap2D map, int steps)
      {
         Console.Clear(); 
         IntCanvas canvas = new IntCanvas(); 
         canvas.SetSize(map.GetSize(), new ivec2(0, 0)); 

         int count = 0; 
         foreach ((ivec2 p, int v) in map) {
            if (v == int.MaxValue) {
               canvas.SetValue(p, 2); 
            } else if (((v % 2) == 0) && (v <= steps)) {
               canvas.SetValue(p, 1); 
               ++count; 
            } else {
               canvas.SetValue(p, 0); 
            }
         }

         // Util.WriteLine(canvas.ToString(".O#")); 
         // Util.WriteLine($"Count:{count}"); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int stepCount = 64;
         Cache test = new Cache(Map, new ivec2[] {Start}, new int[] {0}, 0); 
         return test.GetCount(0, stepCount).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         // edges are clear, so that gives me an easy "shortest path" 
         // between tile and tile (at leaset a good assumption at our distances
         // so I can assume corners?
         int stepCount = 26501365; 
         int width = Map.GetWidth(); 
         int height = Map.GetHeight(); 

         Cache center = new Cache(Map, new ivec2[] {Start}, new int[] {0}, 0); 
         // Util.WriteLine(center.FillMap.ToString()); 

         // get steps to corners
         ivec2 tl = ivec2.ZERO; 
         ivec2 tr = new ivec2(width - 1, 0); 
         ivec2 bl = new ivec2(0, height - 1); 
         ivec2 br = new ivec2(width - 1, height - 1); 

         int parity = stepCount % 2; 
         Int64 plots = (stepCount >= center.MaxCount) 
            ? center.GetCount(parity)
            : center.GetCount(parity, stepCount); 

         int[] rightEdge = center.FillMap.GetColumn(width - 1);
         int[] topEdge = center.FillMap.GetRow(0);
         int[] leftEdge = center.FillMap.GetColumn(0);
         int[] bottomEdge = center.FillMap.GetRow(width - 1);

         // okay, now count the four quadrants, let's start to the right
         plots += CountQuadrant(tl, bl, bl, leftEdge, stepCount); // right
         plots += CountQuadrant(bl, br, br, topEdge, stepCount); // up
         plots += CountQuadrant(tr, br, tr, leftEdge, stepCount); // left
         plots += CountQuadrant(tl, tr, tl, bottomEdge, stepCount); // down

         // needed to look up the answer to debug...
         // Int64 answer = 622926941971282; 
         // Int64 diff = answer - plots; 

         return plots.ToString(); 
      }
   }
}
