using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day25 : Day
   {
      private string InputFile = "2023/inputs/25.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Graph
      {
         public void AddNode(string id) 
         {
            if (!NodeLookup.ContainsKey(id)) {
               Node n = new Node(id); 
               Nodes.Add(n); 
               NodeLookup.Add(id, n);
            }
         }

         public void AddEdge(string a, string b)
         {
            Node aNode = NodeLookup[a]; 
            Node bNode = NodeLookup[b];
            aNode.Edges.Add(bNode); 
            bNode.Edges.Add(aNode); 
         }

         public void RemoveEdge(Node a, Node b)
         {
            a.Edges.Remove(b); 
            b.Edges.Remove(a); 
         }

         public void FloodFrom(Node start)
         {
            // clear it
            foreach (Node n in Nodes) {
               n.Distance = int.MaxValue; 
               n.PrevNode = null; 
            }

            Node cur = start; 
            cur.Distance = 0; 

            PriorityQueue<Node, int> toCheck = new();
            toCheck.Enqueue(cur, 0);  

            while (toCheck.Count > 0) {
               cur = toCheck.Dequeue(); 
               int newDist = cur.Distance + 1; 

               foreach (Node e in cur.Edges) {
                  // someone should have gotten here before me. 
                  if (e.Distance == int.MaxValue) {
                     e.Distance = newDist; 
                     e.PrevNode = cur; 
                     toCheck.Enqueue(e, newDist); 
                  }
               }
            }
         }

         public List<string>? FindLongestPath(Node start)
         {
            FloodFrom(start); 

            Node farthest = start; 
            int dist = start.Distance; 
            foreach (Node n in Nodes) {
               if (n.Distance > dist) {
                  farthest = n; 
                  dist = n.Distance; 
               }
            }

            if (dist == int.MaxValue) {
               // couldn't fill!?  We're split
               return null; 
            }

            Node? cur = farthest;
            List<string> path = new();
            while (cur != null) {
               path.Add(cur.Id); 
               cur = cur.PrevNode; 
            }

            return path; 
         }

         int CountIsland(Node start)
         {
            List<Node> island = new(); 
            island.Add(start); 

            Stack<Node> toCheck = new(); 
            toCheck.Push(start); 

            while (toCheck.Count > 0) {
               Node next = toCheck.Pop(); 

               foreach (Node e in next.Edges) {
                  if (!island.Contains(e)) {
                     island.Add(e); 
                     toCheck.Push(e); 
                  }
               }
            }

            return island.Count; 
         }

         public (int, int) GetIslands()
         {
            FloodFrom(Nodes[0]); 
            int countA = CountIsland(Nodes[0]); 

            int countB = 0; 
            foreach (Node n in Nodes) {
               if (n.Distance == int.MaxValue) {
                  countB = CountIsland(n); 
                  break; 
               }
            }

            return (countA, countB); 
         }

         public List<Node> Nodes = new(); 
         public Dictionary<string, Node> NodeLookup = new();
      }

      class Node
      {
         public Node(string id)
         {
            Id = id; 
         }

         public string Id = ""; 
         public List<Node> Edges = new(); 
         public int Distance = int.MaxValue; 
         public Node? PrevNode = null; 
      }

      private Graph System = new Graph(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            (string id, string otherIds) = line.Split(":", StringSplitOptions.TrimEntries) ; 
            System.AddNode(id); 

            foreach (string otherId in otherIds.Split(' ')) {
               System.AddNode(otherId); 
            }
         }


         foreach (string line in lines) {
            (string id, string connList) = line.Split(":", StringSplitOptions.TrimEntries);
            string[] edges = connList.Split(' '); 

            foreach (string edge in edges) {
               System.AddEdge(id, edge); 
            }
         }
      }

      (int, int) RemoveWire(List<string> toRemove, int count)
      {
         if (count == 0) {
            return System.GetIslands(); 
         }

         foreach (string nodeId in toRemove) {
            Node n = System.NodeLookup[nodeId]; 
            
            /*
            if (count == 1) { 
               Util.WriteLine($"{" " * countProcessing [{n.Id}] (e:{n.Edges.Count})"); 
            }
            */

            Node[] edgeCache = n.Edges.ToArray(); 
            foreach (Node e in edgeCache) {
               System.RemoveEdge(n, e); 
               (int a, int b) = RemoveWire(toRemove, count - 1); 
               System.AddEdge(n.Id, e.Id); 

               if (b != 0) {
                  return (a, b); 
               }
            }
         }

         return (0, 0); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // made this a lot harder than I needed to, I only need three wires to check, and there's not _that many_, so... let's brute for it. 
         List<(string,int)> NodeRanks = new(); 

         Dictionary<string, int> Frequency = new(); 


         foreach (Node n in System.Nodes) {
            List<string>? path = System.FindLongestPath(n); 
            NodeRanks.Add( (n.Id, path.Count) ); 
            // Util.WriteLine($"{n.Id} had longest path {path!.Count}");

            foreach (String s in path) {
               int oldCount = Frequency.GetValueOrDefault(s, 0);
               Frequency[s] = oldCount + 1; 
            }
         }

         /*
         NodeRanks.Sort( (a, b) => a.Item2 - b.Item2 ); 
         foreach (var item in NodeRanks) {
            Util.WriteLine($"({item.Item1} has longest path {item.Item2}"); 
         }
         */

         // assume the most likely edges are connected to the nodes most frequently hit
         var sorted = Frequency.OrderBy(item => -item.Value).ToArray(); 
         /*
         foreach (var item in sorted) {
            Util.WriteLine($"{item.Key} showed up {item.Value} times."); 
         }
         */

         List<string> toRemove = sorted.Where(item => item.Value > 100).Select(item => item.Key).ToList(); 
         (int a, int b) = RemoveWire(toRemove, 3); 

         // (int a, int b) = System.GetIslands(); 
         return (a * b).ToString(); 

         /*
         foreach (Node n in System.Nodes) {
            Util.WriteLine($"{n.Id}: {n.Edges.Count}"); 
         }
         */
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
