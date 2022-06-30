using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    using Inventory = Dictionary<string,(Int64,Int64)>; 

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal struct ResourceInput
    {
        public ResourceNode Resource;
        public Int64 Count = 1; 

        //----------------------------------------------------------------------------------------------
        public ResourceInput( ResourceNode res, Int64 cnt )
        {
            Resource = res; 
            Count = cnt; 
        }
    }

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class ResourceNode
    { 
        public string ID = "";
        public Int64 OutputSize = 0; // how many are created at once
        public List<ResourceInput> Inputs = new List<ResourceInput>();

        public ResourceNode(string id)
        { 
            ID = id; 
        }


        //----------------------------------------------------------------------------------------------
        public void ParseInputs( string inputLine )
        {
            string[] inputs = inputLine.Split(','); 
            foreach (string input in inputs) {
                (string amountStr, string id) = input.Trim().Split(' '); 
                int amount = int.Parse(amountStr); 
               
                ResourceNode srcNode = FindOrCreate(id);
                Inputs.Add( new ResourceInput( srcNode, amount ) ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public bool HasInputs()
        {
            return Inputs.Count > 0; 
        }

        static public Dictionary<string, ResourceNode> AllNodes = new Dictionary<string, ResourceNode>(); 

        //----------------------------------------------------------------------------------------------
        static public ResourceNode FindOrCreate(string id)
        {
            id = id.Trim(); 

            ResourceNode? node; 
            if (AllNodes.TryGetValue(id, out node) && (node != null)) {
                return node; 
            }

            node = new ResourceNode(id); 
            AllNodes.Add(id, node); 
            return node; 
        }
    }

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day14 : Day
 {
        private string InputFile = "inputs/14.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            foreach (string line in lines) {
                (string src, string dst) = line.Split("=>"); 

                (string amtStr, string dstId) = dst.Trim().Split(' '); 
                
                ResourceNode dstNode = ResourceNode.FindOrCreate( dstId ); 
                Debug.Assert( dstNode.OutputSize == 0 ); // has been seen before
                dstNode.OutputSize = int.Parse(amtStr); 
                dstNode.ParseInputs(src); 
            }
        }


        //----------------------------------------------------------------------------------------------
        private void AddRequirement( Inventory inv, Int64 amount, string id )
        {
            if (inv.ContainsKey(id)) {
                (Int64 needed, Int64 produced) = inv[id]; 
                needed += amount; 

                inv[id] = (needed, produced); 
            } else {
                inv.Add(id, (amount, 0)); 
            }
        }

        //----------------------------------------------------------------------------------------------
        private void Produce( Inventory inv, Int64 amount, string id )
        {
            ResourceNode res = ResourceNode.FindOrCreate(id); 
            Int64 bunches = amount; 
            if (res.HasInputs()) {
                amount = Util.CeilToBoundary(amount, res.OutputSize); 
                bunches = amount / res.OutputSize; 
            }

            (Int64 needed, Int64 produced) = inv[id]; 
            produced += amount; 
            inv[id] = (needed, produced); 

            foreach (ResourceInput input in res.Inputs) {
                AddRequirement( inv, bunches * input.Count, input.Resource.ID ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        // Find if we need anything, and if so, produce it.  Returns true if this ended up producing something
        // False if we have everything we needed
        private bool Reduce( Inventory inv )
        {
            foreach (var iter in inv) {
                Int64 needed = iter.Value.Item1 - iter.Value.Item2; 
                if (needed > 0) {
                    // need more, so produce more
                    Produce( inv, needed, iter.Key ); 
                    return true; 
                }
            }

            return false; 
        }

        Int64 OreForFuel = 0; 

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Inventory inventory = new Inventory(); 
            AddRequirement( inventory, 1, "FUEL" ); 

            while (Reduce( inventory )) { }; 

            (Int64 needed, Int64 available) = inventory["ORE"]; 
            OreForFuel = available; 

            return OreForFuel.ToString(); 
        }

       
        
        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            Int64 maxOre = 1000000000000; 

            Int64 minGuess = maxOre / OreForFuel; 
            Int64 maxGuess = minGuess * 2; 

            // binary search for the correct amount needed 
            Int64 guess = (minGuess + maxGuess) / 2; 
            while (minGuess < maxGuess) {
                Inventory inventory = new Inventory(); 
                AddRequirement( inventory, guess, "FUEL" ); 

                while (Reduce( inventory )) { }; 

                (Int64 needed, Int64 available) = inventory["ORE"]; 

                if (available > maxOre) {
                    maxGuess = guess - 1; 
                } else {
                    minGuess = guess; 
                }

                guess = (minGuess + maxGuess + 1) / 2; 
            }

            return guess.ToString(); 
        }
    }
}
