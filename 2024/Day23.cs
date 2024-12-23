using AoC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day23 : Day
   {
      private string InputFile = "2024/inputs/23.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Node
      {
         public Node(string name)
         {
            Name = name; 
         }

         public string Name = ""; 
         public List<Node> Neighbors = new(); 
      }

         //----------------------------------------------------------------------------------------------
         //----------------------------------------------------------------------------------------------
      class Graph
      {
         //----------------------------------------------------------------------------------------------
         Node FindOrCreateNode(string name) 
         {
            if (Lookup.ContainsKey(name)) {
               return Lookup[name]; 
            }

            Node node = new Node(name); 
            Lookup.Add(name, node); 
            Nodes.Add(node); 
            
            return node; 
         }

         //----------------------------------------------------------------------------------------------
         public void AddEdge(string lh, string rh) 
         {
            Node lhn = FindOrCreateNode(lh); 
            Node rhn = FindOrCreateNode(rh); 

            lhn.Neighbors.Add(rhn); 
            rhn.Neighbors.Add(lhn); 
         }

         public void Prune()
         {
            foreach (Node n in Nodes) {
               if (n.Neighbors.Count == 1) {
                  // remove myself from my neighbor
                  Node fn = n.Neighbors.First(); 
                  fn.Neighbors.Remove(n); 

               }
            }
         }

         int GetIndex(Node n) 
         {
            return Nodes.IndexOf(n);  
         }

         public void Sort()
         {
            Nodes.Sort((Node lh, Node rh) => string.Compare(lh.Name, rh.Name)); 
            foreach (Node n in Nodes) {
               n.Neighbors.Sort((Node lh, Node rh) => string.Compare(lh.Name, rh.Name)); 
            }
         }

         public List<List<Node>> FindCompleteSubgraphs(int size, bool haltAfterFirst) 
         {
            List<List<Node>> output = new(); 
            if (size == 0) {
               return output; 
            } else if (size == 1) {
               // todo, if I ever make this generic, but subgraphs of size 1 would just 
               // be all the original nodes.  But not relevant to this problem, so skipping the case
               return output; 
            }

            BitArray visited = new BitArray(Nodes.Count); 
            Stack<Node> stack = new(); 

            for (int i = 0; i < Nodes.Count; ++i) {
               Node root = Nodes[i]; 
               visited[i] = true; 

               if (root.Neighbors.Count < (size - 1)) {
                  continue; // no chance
               }

               stack.Push(root); 
               BitArray childrenVisited = new BitArray(visited); 
               foreach (Node neighbor in root.Neighbors) {
                  GetCommonNeighbors(output, neighbor, stack, root.Neighbors.ToHashSet(), childrenVisited, size - 1);
                  if (haltAfterFirst && output.Count > 0) {
                     return output; 
                  }
               }
               stack.Pop(); 
            }

            return output;
         }

         //----------------------------------------------------------------------------------------------
         void GetCommonNeighbors(List<List<Node>> output, Node node, Stack<Node> commonSet, HashSet<Node> workingSet, BitArray visited, int size)
         {
            int idx = GetIndex(node); 
            if (visited[idx]) {
               return; 
            }
            visited[idx] = true; 

            if (size == 1) {
               List<Node> newOutput = commonSet.Reverse().ToList(); // reverse for debug purposes, so it shows up in the order I constructed it
               newOutput.Add(node); 
               output.Add(newOutput); 
               return; 
            }

            Node parent = commonSet.Peek(); 

            HashSet<Node> commonNeighbors = node.Neighbors.Intersect(workingSet).ToHashSet(); 
            if (commonNeighbors.Count < (size - 1)) {
               return; 
            }

            commonSet.Push(node); 
            BitArray layerVisited = new BitArray(visited); 
            foreach (Node neighbor in commonNeighbors) {
               GetCommonNeighbors(output, neighbor, commonSet, commonNeighbors, layerVisited, size - 1); 
            }

            commonSet.Pop(); 
         }


         //----------------------------------------------------------------------------------------------
         public List<Node> Nodes = new(); 
         public Dictionary<string, Node> Lookup = new(); 
      }

      //----------------------------------------------------------------------------------------------
      private Graph OriginalGraph = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            (string lh, string rh) = line.Split('-'); 
            OriginalGraph.AddEdge(lh, rh); 
         }

         OriginalGraph.Sort(); 
      }

      bool Validate(List<Node> cluster) 
      {
         if ((string.Compare(cluster[0].Name, cluster[1].Name) < 0) 
            && (string.Compare(cluster[1].Name, cluster[2].Name) < 0)) {
            
            // properly sorted, so, make sure everyone has each other as a neighbor
            for (int i = 0; i < cluster.Count; ++i) {
               for (int j = 0; j < cluster.Count; ++j) {
                  if (i == j) {
                     continue; 
                  }

                  if (!cluster[i].Neighbors.Contains(cluster[j])) {
                     return false; 
                  }
               }
            }
         }

         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         // find all clusters of o3
         List<List<Node>> clusters = OriginalGraph.FindCompleteSubgraphs(3, false); 
         
         int count = 0; 
         foreach (List<Node> cluster in clusters) {

            /*
            if (!Validate(cluster)) {
               Util.WriteLine("bad"); 
            }
            */

            bool hasT = false; 
            foreach (Node n in cluster) {
               if (n.Name.StartsWith('t')) {
                  hasT = true; 
                  break; 
               }
            }

            if (hasT) {
               ++count; 
            }
         }

         return count.ToString(); 
      }

      string ClusterToString(List<Node> n) 
      {
         List<string> names = new();
         foreach (Node i in n) {
            names.Add(i.Name); 
         }

         return string.Join(',', names); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int maxPossible = 0; 
         foreach (Node n in OriginalGraph.Nodes) {
            maxPossible = Math.Max(maxPossible, n.Neighbors.Count + 1); 
         }

         /*
         foreach (Node n in OriginalGraph.Nodes) {
            int k = n.Neighbors.Count + 1; 
            if (k == maxPossible) {
               Util.WriteLine(n.Name); 
            }
            maxPossible = Math.Max(maxPossible, n.Neighbors.Count + 1); 
         }
         */

         // Util.WriteLine($"Max Possible: {maxPossible}"); 
         // List<List<Node>> check = OriginalGraph.FindCompleteSubgraphs(maxPossible, false); 

         string pw = ""; 
         for (int i = maxPossible; i >= 3; --i) {
            List<List<Node>> clusters = OriginalGraph.FindCompleteSubgraphs(i, false); 
            
            if (clusters.Count != 0) {
               pw = ClusterToString(clusters[0]); 
               break; 
            }
         }
         return pw; 
      }
   }
}
