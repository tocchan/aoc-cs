using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Node
    {
        internal enum NodeType
        {
            START, 
            END, 
            MINOR, 
            MAJOR, 
        };

        public NodeType Type; 
        public string ID; 
        public int VisitCount; 
        public List<Node> Edges; 

        public Node( string id, NodeType type )
        {
            ID = id; 
            Type = type; 
            VisitCount = 0; 
            Edges = new List<Node>();
        }

        public void AddEdge( Node other )
        {
            Edges.Add( other ); 
        }

        public bool IsStart() => Type == NodeType.START; 
        public bool IsEnd() => Type == NodeType.END; 
        public bool IsMinor() => Type != NodeType.MAJOR; // start and end are considered minor
        public bool IsMajor() => Type == NodeType.MAJOR; 
        public bool IsVisited
        {
            get { return VisitCount > 0; }
        }
    }
    internal class CaveMap
    {
        public CaveMap( List<string> input )
        {
            NodeLookup = new Dictionary<string, Node>(); 
            AllNodes = new List<Node>(); 

            foreach (string line in input)
            {
                (string a, string b) = line.Split('-', 2); 

                Node nodeA = FindOrCreateNode( a ); 
                Node nodeB = FindOrCreateNode( b ); 
                nodeA.AddEdge( nodeB ); 
                nodeB.AddEdge( nodeA ); 
            }

            if (Start == null)
            {
                // shouldn't hit, just shutting up a warning; 
                Start = new Node( "start", Node.NodeType.START ); 
            }
        }

        private Node.NodeType GetTypeFromName( string name )
        {
            if (name == "start")
            {
                return Node.NodeType.START; 
            }
            else if (name == "end")
            {
                return Node.NodeType.END; 
            }
            else if (name[0] <= 'Z')
            {
                return Node.NodeType.MAJOR; 
            }
            else
            {
                return Node.NodeType.MINOR; 
            }
        }

        private Node FindOrCreateNode( string name )
        {
            Node? node = null; 
            if (NodeLookup.TryGetValue(name, out node) && (node != null))
            {
                return node; 
            }

            Node.NodeType type = GetTypeFromName( name ); 
            node = new Node( name, type ); 

            AllNodes.Add(node); 
            NodeLookup.Add(name, node); 
            if (type == Node.NodeType.START)
            {
                Start = node; 
            }

            return node; 
        }

        private int Visit( Node node )
        {
            if (node.IsEnd())
            {
                return 1; 
            }

            if (node.IsMinor() && node.IsVisited) 
            {
                return 0;     
            }

            int paths = 0; 
            ++node.VisitCount; 
            foreach (Node edge in node.Edges) 
            { 
                paths += Visit(edge); 
            }
            --node.VisitCount;

            return paths; 
        }

        public int Explore()
        {
            return Visit( Start ); 
        }

        // hasVisitedMinor is really "has visiited a minor twice"
        private int Visit2( Node node, bool hasVisitedMinor )
        {
            if (node.IsEnd())
            {
                return 1;
            }

            if (node.IsMinor()) 
            {
                if (node.IsVisited && (hasVisitedMinor || node.IsStart())) 
                {
                    return 0;     
                }
            }

            int paths = 0; 
            ++node.VisitCount; 
            foreach (Node edge in node.Edges) 
            { 
                paths += Visit2(edge, hasVisitedMinor || (node.IsMinor() && (node.VisitCount > 1))); 
            }
            --node.VisitCount;

            return paths; 
        }

        public int Explore2()
        {
            return Visit2( Start, false ); 
        }


        public Node Start; 
        public List<Node> AllNodes; 
        public Dictionary<string, Node> NodeLookup; 
    }

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day12 : Day
    {
        private string InputFile = "2021/inputs/12.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            CaveMap map = new CaveMap( lineInput ); 

            int numPaths = map.Explore(); 
            return numPaths.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            CaveMap map = new CaveMap( lineInput ); 
            int numPaths = map.Explore2(); 

            return numPaths.ToString(); 
        }
    }
}

