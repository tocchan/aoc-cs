using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day21 : Day
    {

        private int[] StartingPositions = new int[2]; 

        private int FixedDieValue = 1; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/21.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            StartingPositions[0] = int.Parse( lines[0].Split(':')[1] ); 
            StartingPositions[1] = int.Parse( lines[1].Split(':')[1] ); 
        }

        //----------------------------------------------------------------------------------------------
        private int RollFixedDie()
        {
            int retValue = FixedDieValue; 
            ++FixedDieValue; 
            if (FixedDieValue > 100)
            {
                FixedDieValue -= 100; 
            }

            return retValue; 
        }

        private int RollFixedDie( int count )
        {
            int result = 0; 
            for (int i = 0; i < count; ++i)
            {
                result += RollFixedDie(); 
            }

            return result; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int[] place = new int[2]; 
            int[] scores = new int[2]; 
            int winningScore = 1000; 

            Int64 turnNumber = 0; 
            place[0] = StartingPositions[0];
            place[1] = StartingPositions[1]; 

            while ((scores[0] < winningScore) && (scores[1] < winningScore))
            {
                int playerTurn = (int)(turnNumber % 2L); 
                int roll = RollFixedDie(3); 

                place[playerTurn] += roll; 
                while (place[playerTurn] > 10)
                {
                    place[playerTurn] -= 10; 
                }
                scores[playerTurn] += place[playerTurn]; 

                ++turnNumber; 
            }

            Int64 player2Turns = turnNumber / 2; 
            Int64 player1Turns = turnNumber - player2Turns; 
            
            Int64 answer; 
            if (scores[0] >= 1000)
            {
                answer = scores[1] * turnNumber * 3L; 
            }
            else
            {
                answer = scores[0] * turnNumber * 3L; 
            }

            return answer.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            QuantumGame game = new QuantumGame(); 
            game.Init( StartingPositions[0], StartingPositions[1] ); 

            int turnNumber = 0; 
            while (!game.IsDone())
            {
                game.RunTurn( turnNumber ); 
                ++turnNumber; 
            }

            return game.Wins.Max().ToString(); 
        }
    }

    public struct BoardState 
    {
        public ivec2 Positions; 
        public ivec2 Scores;

        public void Change( int player, int roll )
        {
            Positions[player] += roll; 
            if (Positions[player] > 10)
            {
                Positions[player] -= 10; 
            }

            Scores[player] += Positions[player];
        }

        public BoardState GetNextState(int player, int roll)
        {
            BoardState nextState = new BoardState(); 
            nextState.Positions = Positions; 
            nextState.Scores = Scores; 
            nextState.Change( player, roll ); 
            return nextState; 
        }

        public bool IsComplete()
        {
            return GetWinner() >= 0; 
        }

        public int GetWinner()
        {
            if (Scores.x >= 21)
            {
                return 0; 
            }
            else if (Scores.y >= 21)
            {
                return 1; 
            }
            else
            {
                return -1; 
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Positions.GetHashCode(), Scores.GetHashCode() ); 
        }
    }

    public class QuantumGame
    {
        private Dictionary<BoardState,Int64> CurrentState = new Dictionary<BoardState, Int64>();
        private int[] DiceDistribution = { 1, 3, 6, 7, 6, 3, 1 }; 
        public Int64[] Wins = { 0, 0 }; 

        public void Init( int start0, int start1 )
        {
            BoardState state; 
            state.Positions = new ivec2( start0, start1 ); 
            state.Scores = ivec2.ZERO; 

            CurrentState[state] = 1L; 
        }

        public void RunTurn( int turnNumber )
        { 
            int player = turnNumber % 2; 
            Dictionary<BoardState,Int64> nextState = new Dictionary<BoardState, Int64>();

            foreach ((BoardState state, Int64 count) in CurrentState) 
            {
                for (int i = 0; i < DiceDistribution.Length; ++i)
                {
                    int roll = 3 + i; 
                    int prob = DiceDistribution[i]; 

                    BoardState newState = state.GetNextState( player, roll ); 
                    Int64 newCount = count * prob; 
                    int winner = newState.GetWinner(); 
                    if (winner >= 0)
                    {
                        Wins[winner] += newCount; 
                    }
                    else
                    {
                        Int64 oldVal;
                        if (nextState.TryGetValue( newState, out oldVal ))
                        {
                            nextState[newState] = oldVal + newCount; 
                        }
                        else
                        {
                            nextState[newState] = newCount; 

                        }
                    }
                }
            }

            CurrentState = nextState; 
        }

        public bool IsDone()
        {
            return CurrentState.Count() == 0; 
        }
    }
}
