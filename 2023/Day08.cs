using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
   internal class Day08 : Day
   {
      private string InputFile = "2023/inputs/08.txt";

      class Node
      {
         public Node(string line)
         {
            (Id, string rest) = line.Split('='); 
            Id = Id.Trim();

            rest = rest.Trim();
            rest = rest.Substring(1, rest.Length - 2); 
            (Left, Right) = rest.Split(','); 

            Left = Left.Trim();
            Right = Right.Trim(); 
         }

         public string Id; 
         public string Left; 
         public string Right; 
      }; 
      
      string Directions = ""; 
      Dictionary<string, Node> Network = new Dictionary<string, Node>(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         Directions = lines[0]; 
         for (int i = 2; i < lines.Count; ++i) {
            Node n = new Node(lines[i]);
            Network[n.Id] = n; 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         string loc = "AAA"; 
         return Solve(loc).ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public Int64 Solve(string loc)
      {
         int step = 0; 
         while (!loc.EndsWith('Z')) {
            int dirIndex = step % Directions.Length;
            ++step;

            char dir = Directions[dirIndex];
            Node n = Network[loc];
            loc = (dir == 'L') ? n.Left : n.Right;
         }

         return (Int64)step;
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         List<string> startingNodes = new List<string>(); 

         foreach ((string key, _) in Network) {
            if (key.EndsWith('A')) {
               startingNodes.Add(key); 
            }
         }

         List<Int64> steps = new List<Int64>(startingNodes.Count); 
         foreach (string key in startingNodes) {
            steps.Add(Solve(key)); 
         }

         Int64 total = Util.LCM(steps); 
         return total.ToString(); 
      }
   }
}
