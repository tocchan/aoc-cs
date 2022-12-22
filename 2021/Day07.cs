using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day07 : Day
    {
        private string InputFile = "2021/inputs/07.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        int CalcDistance( int point, List<int> positions )
        {
            int sum = 0; 
            foreach (int pos in positions)
            {
                sum += Math.Abs( pos - point ); 
            }
            return sum; 
        }

        //----------------------------------------------------------------------------------------------
        int CalcDistance2( int point, List<int> positions )
        {
            int sum = 0; 
            foreach (int pos in positions)
            {
                int val = Math.Abs( pos - point ); 
                int cost = ((val) * (val + 1)) / 2; 
                sum += cost; 
            }
            return sum; 
        }

        //----------------------------------------------------------------------------------------------
        int GetGeoMidpoint( List<int> positions, Func<int, List<int>, int> funcDist )
        {
            int min = positions.Min(); 
            int max = positions.Max(); 

            int minPoint = max; 
            int minDistance = funcDist( max, positions ); 

            for (int i = min; i < max; ++i)
            {
                int dist = funcDist( i, positions ); 
                if (dist < minDistance)
                {
                    minDistance = dist; 
                    minPoint = i; 
                }
            }

            return minPoint; 
        }

     
        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            List<int> initialPositions = lineInput[0].Split(',').Select(int.Parse).ToList(); 

            int midpoint = GetGeoMidpoint( initialPositions, CalcDistance ); 
            int distance = CalcDistance( midpoint, initialPositions ); 

            return distance.ToString(); 
        }


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            List<int> initialPositions = lineInput[0].Split(',').Select(int.Parse).ToList(); 

            int midpoint = GetGeoMidpoint( initialPositions, CalcDistance2 ); 
            int distance = CalcDistance2( midpoint, initialPositions ); 

            return distance.ToString(); 
        }
    }
}
