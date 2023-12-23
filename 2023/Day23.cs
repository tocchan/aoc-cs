using AoC;
using AoC2021;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day23 : Day
   {
      private string InputFile = "2023/inputs/23.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------

      

      internal class Graph
      {
      //----------------------------------------------------------------------------------------------
         public class Node
         {
            public int Id = 0; 
            public ivec2 Location = ivec2.ZERO; 
            public Edge?[] Edges = new Edge?[4]; 

            public Edge? FindEdge(eDirection dir)
            {
               int dirIdx = (int) dir; 
               return Edges[dirIdx]; 
            }

            public void AddOrUpdateEdge(eDirection dir, Node n, int cost)
            {
               Edge? e = FindEdge(dir); 
               if (e != null) {
                  e.Cost = Math.Min(e.Cost, cost); 
               } else {
                  int edgeIdx = (int)dir; 
                  Edges[edgeIdx] = new Edge(n, cost); 
               }
            }

            public override string ToString()
            {
               StringBuilder sb = new(); 
               sb.Append($"{Id} ({Location}): "); 

               bool isFirst = true; 
               string dirs = "EWNS"; 

               for (int i = 0; i < Edges.Length; ++i) {
                  Edge? e = Edges[i]; 
                  if (e != null) {
                     if (!isFirst) {
                        sb.Append(", "); 
                     }
                     isFirst = false; 

                     sb.Append($"({dirs[i]}):[{e.ToString()}]"); 
                  }
               }

               return sb.ToString(); 
            }
         }

      //----------------------------------------------------------------------------------------------
         public class Edge
         {
            public Node Destination; 
            public int Cost; 

            public Edge(Node n, int cost)
            {
               Destination = n; 
               Cost = cost; 
            }

            public override string ToString()
            {
               return $"{Destination.Id} ({Cost})"; 
            }

         }

      //----------------------------------------------------------------------------------------------
         public Node AddNode(ivec2 loc)
         {
            int id = Nodes.Count; 
            Node n = new Node(); 
            n.Id = id; 
            n.Location = loc; 

            Nodes.Add(n); 
            return n; 
         }

      //----------------------------------------------------------------------------------------------
         public Node? FindNode(ivec2 loc)
         {
            foreach (Node n in Nodes) {
               if (n.Location == loc) {
                  return n; 
               }
            }

            return null; 
         }

         //----------------------------------------------------------------------------------------------
         public void AddEdge(Node src, eDirection srcDir, Node dst, eDirection dstDir, int cost)
         {
            Node srcNode = Nodes[src.Id]; 
            Node dstNode = Nodes[dst.Id]; 

            srcNode.AddOrUpdateEdge(srcDir, dstNode, cost); 
            dstNode.AddOrUpdateEdge(dstDir, srcNode, cost); 
         }

         //----------------------------------------------------------------------------------------------
         public void Debug()
         {
            foreach (Node n in Nodes) {
               Util.WriteLine(n.ToString()); 
            }
         }

         //----------------------------------------------------------------------------------------------
         public int FindLongestPath(ivec2 startPoint, ivec2 endPoint)
         {
            Node start = FindNode(startPoint)!; 
            Node end = FindNode(endPoint)!;

            Stack<Node> visited = new(); 
            return FindLongestPath(start, end, visited); 
         }

         //----------------------------------------------------------------------------------------------
         int FindLongestPath(Node start, Node end, Stack<Node> visited)
         { 
            Node cur = start; 
            visited.Push(cur); 
            int longest = 0; 
            foreach (Edge? e in cur.Edges) {
               if (e == null) {
                  continue; 
               }

               if (e.Destination == end) {
                  longest = Math.Max(longest, e.Cost); 
               }

               if (visited.Contains(e.Destination)) {
                  continue; 
               }

               int pathLength = FindLongestPath(e.Destination, end, visited); 
               if (pathLength > 0) {
                  longest = Math.Max(longest, pathLength + e.Cost); 
               }
            }
            visited.Pop(); 

            return longest; 
         }

         List<Node> Nodes = new();
      }

      private IntHeatMap2D Map = new IntHeatMap2D(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Map.SetFromTightBlock(lines, '#', 0);  
      }

      ivec2? GetSlopeDirection(int tile)
      {
         return tile switch {
            '^' => ivec2.UP, 
            '>' => ivec2.RIGHT, 
            '<' => ivec2.LEFT, 
            'v' => ivec2.DOWN, 
            _ => null
         }; 
      }

      bool IsValid(int value, ivec2 dir, bool useSlopes)
      {
         if (value == '#') {
            return false; 
         } else if ((value == '.') || !useSlopes) {
            return true; 
         } else {
            ivec2 slopeDir = GetSlopeDirection(value)!.Value; 
            return slopeDir == dir; 
         }
      }

      int ToInt(bool value) => value ? 1 : 0; 

      int FollowPath(ivec2 pos, ivec2 dir, ivec2 end, Stack<ivec2> history, bool useSlopes)
      {
         int stepCount = 0; 
         while (true) {
            if (pos == end) {
               return stepCount; 
            }

            // we'll either return invalid, or we'll take at least one step (recusive function is one step ahead)
            ++stepCount; 

            if (history.Contains(pos)) {
               return int.MinValue; // rehit an old path
            }

            int tile = Map[pos]; 
            if (tile == '#') {
               return int.MinValue; // hit a wall?
            }

            if (useSlopes) { 
               ivec2? slope = GetSlopeDirection(tile); 
               if (slope.HasValue) {
                  // can't be an intersection, we don't hav ea choice
                  dir = slope.Value; 
                  pos += dir; 
                  continue; 
               }
            }

            ivec2 forward = dir; 
            ivec2 left = dir.GetRotatedLeft(); 
            ivec2 right = dir.GetRotatedRight(); 

            int fTile = Map[pos + forward]; 
            int lTile = Map[pos + left]; 
            int rTile = Map[pos + right];

            bool fValid = IsValid(fTile, forward, useSlopes); 
            bool lValid = IsValid(lTile, left, useSlopes); 
            bool rValid = IsValid(rTile, right, useSlopes); 

            int numValid = ToInt(fValid) + ToInt(lValid) + ToInt(rValid); 
            if (numValid == 0) {
               return int.MaxValue; 
            } else if (numValid == 1) {
               if (fValid) {
                  dir = forward; 
               } else if (rValid) {
                  dir = right; 
               } else {
                  dir = left; 
               }
               pos += dir; 
            } else {
               int longest = int.MinValue;

               history.Push(pos);
               { 
                  if (fValid) {
                     longest = Math.Max(longest, FollowPath(pos + forward, forward, end, history, useSlopes)); 
                  }

                  if (lValid) {
                     longest = Math.Max(longest, FollowPath(pos + left, left, end, history, useSlopes));
                  }

                  if (rValid) {
                     longest = Math.Max(longest, FollowPath(pos + right, right, end, history, useSlopes));
                  }
               }
               history.Pop(); 

               return (longest >= 0) ? stepCount + longest : int.MinValue; 
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public int FindLongestPath(ivec2 start, ivec2 end, bool useSlopes)
      {
         Stack<ivec2> intersections = new(); 
         return FollowPath(start, ivec2.DOWN, end, intersections, useSlopes); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // think just search for it?
         ivec2 size = Map.GetSize(); 
         ivec2 startPoint = new ivec2(1, 0); 
         ivec2 endPoint = size - new ivec2(2, 1); 
         int steps = FindLongestPath(startPoint, endPoint, true); 
         return steps.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      eDirection DirectionFromVector(ivec2 dir)
      {
         for (int i = 0; i < ivec2.DIRECTIONS.Length; ++i) { 
            if (dir == ivec2.DIRECTIONS[i]) {
               return (eDirection) i; 
            }
         }

         return eDirection.None; 
      }

      //----------------------------------------------------------------------------------------------
      void FollowGraphPath(Graph g, Graph.Node src, eDirection srcDir, ivec2 end)
      {
         ivec2 dir = ivec2.DIRECTIONS[(int)srcDir];
         ivec2 pos = src.Location + dir; 
         int stepCount = 0; 

         ivec2[] toTry = new ivec2[3];
         bool[] valid = new bool[3];

         while (true) {
            // we'll either return invalid, or we'll take at least one step (recusive function is one step ahead)
            ++stepCount;

            if (pos == end) {
               Graph.Node dst = g.AddNode(end); 
               g.AddEdge(src, srcDir, dst, DirectionFromVector(dir).Negate(), stepCount); 
               return; 
            }

            int tile = Map[pos];
            if (tile == '#') {
               return; 
            }

            toTry[0] = dir;
            toTry[1] = dir.GetRotatedLeft();
            toTry[2] = dir.GetRotatedRight();

            for (int i = 0; i < 3; ++i) {
               ivec2 newDir = toTry[i]; 
               ivec2 newPos = pos + newDir; 
               int newTile = Map[newPos];
               valid[i] = IsValid(newTile, newDir, false); 
            }

            int numValid = valid.Count(v => v); 
            if (numValid == 0) {
               return; 
            } else if (numValid == 1) {
               // follow the valid one
               for (int i = 0; i < 3; ++i) {
                  if (valid[i]) {
                     dir = toTry[i]; 
                     pos += dir; 
                     break; // for
                  }
               }
            } else {
               Graph.Node? newNode = g.FindNode(pos);
               eDirection curDir = DirectionFromVector(dir);
               eDirection inDir = curDir.Negate();

               if (newNode != null) {
                  // will update the edge, but don't split.  The first person who hit this junction should already have created threads
                  g.AddEdge(src, srcDir, newNode, inDir, stepCount); 
                  return; 
               }

               newNode = g.AddNode(pos); 
               g.AddEdge(src, srcDir, newNode, inDir, stepCount); 

               for (int i = 0; i < 3; ++i) {
                  if (!valid[i]) {
                     continue; 
                  }

                  eDirection outDir = DirectionFromVector(toTry[i]); 
                  FollowGraphPath(g, newNode, outDir, end);                   
               }

               return; 
            }
         }
      }

      Graph ConstructGraph()
      {
         ivec2 size = Map.GetSize();
         ivec2 startPoint = new ivec2(1, 0);
         ivec2 endPoint = size - new ivec2(2, 1);

         Graph g = new Graph(); 
         Graph.Node start = g.AddNode(startPoint);

         FollowGraphPath(g, start, eDirection.Down, endPoint); 
         return g; 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Graph graph = ConstructGraph(); 
         // graph.Debug(); // just reducing the problem to a graph was a big enough perf boost it now finishes in 7s

         ivec2 size = Map.GetSize();
         ivec2 startPoint = new ivec2(1, 0);
         ivec2 endPoint = size - new ivec2(2, 1);
         int length = graph.FindLongestPath(startPoint, endPoint); 

         return length.ToString(); 
      }
   }
}
