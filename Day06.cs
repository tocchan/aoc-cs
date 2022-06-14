using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Orbit
    {
        public string ID = "";
        public int Depth = 0; 
        public Orbit? Parent = null; 

        public Orbit(string id)
        {
            ID = id; 
            Parent = null; 
            Depth = -1; // will calcualte later
        }

        public int GetDepth()
        {
            if (Depth < 0) {
                if (null == Parent) {
                    Depth = 0;
                } else {
                    Depth = 1 + Parent.GetDepth(); 
                }
            }

            return Depth; 
        }

        public List<Orbit> GetAncestors()
        {
            List<Orbit> ancestors = new List<Orbit>(); 
            Orbit? iter = Parent; 

            while (iter != null) {
                ancestors.Add(iter); 
                iter = iter.Parent; 
            }

            ancestors.Reverse(); 
            return ancestors; 
        }
    }

    internal class Day06 : Day
    {
        private string InputFile = "inputs/06.txt"; 

        //----------------------------------------------------------------------------------------------
        // values
        Dictionary<string, Orbit> Orbits = new Dictionary<string, Orbit>(); 

        private Orbit FindOrCreate( string id )
        {
            Orbit? orbit = null; 
            if (!Orbits.TryGetValue( id, out orbit )) {
                orbit = new Orbit(id); 
                Orbits.Add( id, orbit ); 
            } 

            return orbit; 
        }

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            foreach( string line in lines ) {
                (string parentID, string childID) = line.Split(')'); 

                Orbit parent = FindOrCreate( parentID ); 
                Orbit child = FindOrCreate( childID ); 
                child.Parent = parent; 
            }

            foreach ( (string id, Orbit orbit) in Orbits ) {
                orbit.GetDepth(); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int checksum = 0; 
            foreach ( (string id, Orbit orbit) in Orbits ) {
                checksum += orbit.GetDepth(); 
            }

            return checksum.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            Orbit you = Orbits["YOU"]; 
            Orbit san = Orbits["SAN"]; 

            // find a common ancestor. 
            List<Orbit> yourAncestory = you.GetAncestors(); 
            List<Orbit> sanAncestory = san.GetAncestors(); 

            Orbit common = yourAncestory[0]; 
            int num = Math.Min( yourAncestory.Count, sanAncestory.Count ); 
            for (int i = 0; i < num; ++i) {
                if (yourAncestory[i] != sanAncestory[i]) {
                    break; 
                }
                common = yourAncestory[i]; 
            }

            int yourDepth = you.GetDepth() - common.GetDepth() - 1; 
            int sanDepth = san.GetDepth() - common.GetDepth() - 1;
            int totalDist = yourDepth + sanDepth; 

            return totalDist.ToString(); 
        }
    }
}
