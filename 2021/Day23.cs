using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day23 : Day
    {
        IntHeatMap2D InitialStage = new IntHeatMap2D(); 
        int[] Costs = { 1, 10, 100, 1000 }; 


        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/23.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            InitialStage.Init( 13, 5, int.MaxValue, int.MaxValue ); 
            for (int x = 1; x <= 11; ++x)
            {
                InitialStage.Set( x, 1, 0 ); 
            }
            for (int y = 2; y <= 3; ++y)
            {
                for (int x = 3; x <= 9; x +=2) 
                {
                    InitialStage.Set( x, y, lines[y][x] - '@' ); 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        ivec2[] FindPositions( IntHeatMap2D map, int unit )
        {
            List<ivec2> points = new List<ivec2>(); 

            foreach ((ivec2 pos, int val) in map)
            {
                if ((pos.y > 0) && (val == unit))
                {
                    points.Add(pos); 
                }
            }

            return points.ToArray(); 
        }

        private int GetDistance(ivec2 p0, ivec2 p1)
        {
            if (p0.x == p1.x)
            { 
                return Math.Abs(p0.y - p1.y); 
            }
            else
            {
                return Math.Abs(p0.x - p1.x)
                    + p0.y + p1.y - 2; // how long does it take to travel out and into the hallway again?
            }
        }

        (int,int)[] Permutations = new (int,int)[0]; 
        int PermPossibilities = 0; 

        private void AddPermuations( int[] set )
        {
            // first guy can be whatever he wants, secdond guy gets what's left
        }

        //----------------------------------------------------------------------------------------------
        (int,int)[] GetPermuations( int possibilities )
        {
            if (PermPossibilities == possibilities) 
            {
                return Permutations;
            }

            PermPossibilities = possibilities;
            Permutations = Util.PermutePairs( possibilities ); 
            return Permutations; 
        }

        //----------------------------------------------------------------------------------------------
        private int PredictCosts( IntHeatMap2D map, int unit )
        {
            int costPerMove = Costs[unit - 1]; 

            // where I'm moving from
            ivec2[] src = FindPositions( map, unit ); 
            ivec2[] dst = new ivec2[src.Length]; 

            int x = 1 + (unit * 2);
            for (int y = 0; y < src.Length; ++y)
            {
                dst[y].x = x; 
                dst[y].y = y + 2; 
            }

            int minCost = int.MaxValue;
            (int,int)[] permutations = GetPermuations( src.Length ); 

            for (int i = 0; i < permutations.Length; i += src.Length)
            {
                int cost = 0; 
                for (int j = 0; j < src.Length; ++j) 
                {
                    cost += GetDistance( src[permutations[i + j].Item1], dst[permutations[i + j].Item2] ); 
                } 
                minCost = Math.Min( minCost, cost ); 
            }

            return Costs[unit - 1] * minCost; 
        }

        //----------------------------------------------------------------------------------------------
        private int PredictCosts( IntHeatMap2D map )
        {
            int cost = 0; 
            for (int i = 1; i <= 4; ++i)
            {
                cost += PredictCosts( map, i ); 
            }

            return cost; 
        }

        private bool CanMove( IntHeatMap2D state, ivec2 src, ivec2 dst )
        {
            while (src != dst) 
            {
                if (src.y == 1)
                {
                    if (src.x == dst.x)
                    {
                        ++src.y;
                    }
                    else
                    {
                        src.x += Math.Sign(dst.x - src.x); 
                    }
                }
                else
                {
                    if (src.x == dst.x)
                    {
                        src.y += Math.Sign(dst.y - src.y); 
                    }
                    else
                    {
                        --src.y; 
                    }
                }

                if (state.Get(src) != 0)
                {
                    return false; 
                }
            }

            return true; 
        }

        private ivec2[] GetPotentialMoves( IntHeatMap2D state, ivec2 pos, int unit )
        {
            List<ivec2> moves = new List<ivec2>(); 

            int preferredX = 1 + (unit * 2); 

            // already home, don't move
            if (preferredX == pos.x)
            {
                // check everything below me that it is the same as me; 
                bool isHome = true; 
                ivec2 preferred = new ivec2(preferredX, state.GetHeight() - 2); 
                while (preferred.y > pos.y)
                {
                    int occupied = state.Get(preferred); 
                    if (occupied != unit)
                    {
                        isHome = false; 
                        break; 
                    }
                    --preferred.y; 
                }

                if (isHome)
                {
                    return moves.ToArray(); 
                }
            }

            // Early out, I'm blocked in; 
            if (pos.y > 1)
            {
                if (state.Get(pos.x, pos.y - 1) != 0)
                {
                    return moves.ToArray(); 
                }
            }

            // in the top row, so can only return home (or I'm in a non-preferred hallway, and I can move directly to where I want to be)
            if ((pos.y == 1) || (pos.x != preferredX))
            {
                ivec2 preferred = new ivec2(preferredX, state.GetHeight() - 2); 

                while (preferred.y > 1)
                {
                    int occupied = state.Get(preferred); 
                    if ((occupied == 0) && CanMove(state, pos, preferred) )
                    {
                        moves.Add(preferred); 
                        break;
                    }
                    else if (occupied != unit)
                    {
                        break; 
                    }
                    --preferred.y;
                }
            }

            // moving out of my hallway (if I have a move that took me to where I _wanted_ to go though
            // no reason to do this; 
            if ((moves.Count == 0) && (pos.y > 1))
            {
                // I have a ton of places I could move
                int[] potentialMoves = { 1, 2, 4, 6, 8, 10, 11 }; 
                ivec2 preferred; 
                preferred.y = 1; 
                foreach (int x in potentialMoves)
                {
                    preferred.x = x; 
                    if (CanMove(state, pos, preferred))
                    {
                        moves.Add( preferred ); 
                    }
                }
            }

            return moves.ToArray(); 
        }

        //----------------------------------------------------------------------------------------------
        private List<IntHeatMap2D> GetPotentialMoves(IntHeatMap2D state)
        {
            List<IntHeatMap2D> nextStates = new List<IntHeatMap2D>(); 
            int stateCost = state.Get(0, 0); 

            ivec2 pos; 
            int height = state.GetHeight() - 1; 
            for (pos.y = 1; pos.y <= height; ++pos.y)
            {
                for (pos.x = 1; pos.x <= 11; ++pos.x)
                {
                    int val = state.Get(pos); 

                    // not a unit
                    if ((val < 1) || (val > 4))
                    {
                        continue; 
                    }

                    // if it is a unit, get its preferred destination
                    ivec2[] moves = GetPotentialMoves( state, pos, val ); 

                    foreach (ivec2 move in moves) 
                    {
                        IntHeatMap2D newState = new IntHeatMap2D(state); 

                        // move the unit
                        newState.Set(pos, 0); 
                        newState.Set(move, val); 
                        
                        int moveCost = Costs[val - 1] * GetDistance(move, pos); 
                        newState.Set(0, 0, stateCost + moveCost); 

                        int predictedCost = PredictCosts(newState) + newState.Get(0, 0); 
                        newState.Set(12, 0, predictedCost); 

                        // string unitName = ((char)('@' + val)).ToString(); 
                        // Util.WriteLine( $"Move: {unitName} from ({pos}) to ({move})..."); 

                        nextStates.Add(newState); 
                    }
                }
            }

            return nextStates; 
        }

        //----------------------------------------------------------------------------------------------
        HashSet<IntHeatMap2D> SeenStates = new HashSet<IntHeatMap2D>(); 
        bool HaveSeenState( IntHeatMap2D state )
        {
            IntHeatMap2D sub = state.GetSubRegion( 1, 1, 11, state.GetHeight() - 2 ); 
            int hash = sub.GetHashCode(); 
            
            if (SeenStates.Contains(sub))
            {
                 return true; 
            }

            SeenStates.Add(sub); 
            return false; 
        }

        private void PrintBoard( int turn, IntHeatMap2D board, bool recurse = true )
        {
            if ((board.HackParent != null) && recurse)
            {
                PrintBoard( turn, board.HackParent ); 
            }

            int currCost = board.Get(0, 0); 
            int potCost = board.Get(12, 0); 
            int predCost = PredictCosts( board ); 
            string val = $"Turn: {turn}\nCr: {currCost},  Pt: {potCost},  Pt - Cr: {potCost - currCost} | {predCost}\n"; 
           

            int y = 0; 
            foreach ((ivec2 pos, int v) in board)
            {
                char c = ' ';
                if ((pos.y == 0) || (v > 4)) 
                { 
                    c = '█';
                }
                else if (v > 0)
                {
                    c = (char)('@' + v); 
                }

                if (pos.y != y)
                {
                    val += '\n'; 
                    y = pos.y; 
                }
                val += c; 
            }
            val += '\n'; 

            Util.WriteLine(val); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            IntHeatMap2D initial = new IntHeatMap2D(InitialStage); 

            // lets store the cost to solved in the top-left corner since it isn't used for anything; 
            int initialCost = PredictCosts(initial); 
            initial.Set(0, 0, 0); // move cost so far
            initial.Set(12, 0, initialCost); // lowest potential state

            PriorityQueue<IntHeatMap2D,int> moves = new PriorityQueue<IntHeatMap2D, int>(); 
            moves.Enqueue(initial, initialCost); 

            SeenStates.Clear(); 
            int turn = 0; 
            while (moves.Count > 0)
            {
                IntHeatMap2D move = moves.Dequeue(); 
                int currentCost = move.Get(0, 0); 
                int predCost = move.Get(12, 0); 

                if ((currentCost == 0) 
                    || (currentCost == predCost))
                {
                    // PrintBoard(turn, move); 
                }
                ++turn; 

                if (currentCost == predCost)
                {
                    return currentCost.ToString(); 
                }

                // these should add the values; 
                List<IntHeatMap2D> potentialMoves = GetPotentialMoves(move); 
                foreach (IntHeatMap2D potentialMove in potentialMoves)
                {
                    // Cost didn't go up, meaning we're in a "finished" state
                    int newCost = potentialMove.Get(0, 0); 
                    if (!HaveSeenState(potentialMove))
                    {
                        int predictedCost = potentialMove.Get(12, 0); 

                        potentialMove.HackParent = move; 
                        moves.Enqueue(potentialMove, predictedCost); 
                    }
                }
            }

            return "DNF";             
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            // setup board for part two
            IntHeatMap2D initial = new IntHeatMap2D( new ivec2(13, 7), int.MaxValue, int.MaxValue ); 
            initial.SetAll( initial.GetBoundsValue() ); 

            initial.Copy( InitialStage, 0, 0, 13, 3, 0, 0 ); 
            initial.Copy( InitialStage, 0, 3, 13, 2, 0, 5 ); 
            
            // add new rows
            /* Actual Set */
            initial.Set( 3, 3, 4 ); 
            initial.Set( 3, 4, 4 ); 

            initial.Set( 5, 3, 3 ); 
            initial.Set( 5, 4, 2 ); 

            initial.Set( 7, 3, 2 ); 
            initial.Set( 7, 4, 1 ); 

            initial.Set( 9, 3, 1 ); 
            initial.Set( 9, 4, 3 ); 
            /**/

            /* Debug Set *
            initial.Set( 3, 3, 1 ); 
            initial.Set( 3, 4, 1 ); 

            initial.Set( 5, 3, 4 ); 
            initial.Set( 5, 4, 2 ); 

            initial.Set( 7, 3, 3 ); 
            initial.Set( 7, 4, 3 ); 

            initial.Set( 9, 3, 1 ); 
            initial.Set( 9, 4, 4 ); 
            /**/


            // lets store the cost to solved in the top-left corner since it isn't used for anything; 
            int initialCost = PredictCosts(initial); 
            initial.Set(0, 0, 0); // move cost so far
            initial.Set(12, 0, initialCost); // lowest potential state

            PriorityQueue<IntHeatMap2D,int> moves = new PriorityQueue<IntHeatMap2D, int>(); 
            moves.Enqueue(initial, initialCost); 

            SeenStates.Clear(); 
            int turn = 0; 
            while (moves.Count > 0)
            {
                IntHeatMap2D move = moves.Dequeue(); 
                int currentCost = move.Get(0, 0); 
                int predCost = move.Get(12, 0); 

                if ((currentCost == 0) 
                    || (currentCost == predCost))
                {
                    PrintBoard(turn, move, false); 
                }
                ++turn; 

                if (currentCost == predCost)
                {
                    return currentCost.ToString(); 
                }

                // these should add the values; 
                List<IntHeatMap2D> potentialMoves = GetPotentialMoves(move); 
                foreach (IntHeatMap2D potentialMove in potentialMoves)
                {
                    // Cost didn't go up, meaning we're in a "finished" state
                    int newCost = potentialMove.Get(0, 0); 
                    if (!HaveSeenState(potentialMove))
                    {
                        int predictedCost = potentialMove.Get(12, 0); 
                        potentialMove.HackParent = move; 
                        moves.Enqueue(potentialMove, predictedCost); 
                    }
                }
            }

            return "DNF";             
        }
    }
}
