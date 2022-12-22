using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day11 : Day
    {
        private string InputFile = "2021/inputs/11.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private int Flash( ivec2 coord, IntHeatMap2D grid )
        {
            int numFlashes = 0; 
            ivec2 min = ivec2.Max( coord - new ivec2(1, 1), ivec2.ZERO ); 
            ivec2 max = ivec2.Min( coord + new ivec2(1, 1), new ivec2(9, 9) ); 
            
            foreach( (ivec2 p, int v) in grid.GetRegionEnumerator( coord - ivec2.ONE, coord + ivec2.ONE ) )
            {
                if (v < 10) 
                { 
                    ++grid[p]; 
                    if (grid[p] >= 10) 
                    {
                        ++numFlashes; 
                        numFlashes += Flash( p, grid ); 
                    }
                }
            }

            return numFlashes;
        }

        //----------------------------------------------------------------------------------------------
        private int RunDay( IntHeatMap2D grid )
        {
            int numFlashes = 0; 
            foreach ((ivec2 p, int v) in grid) 
            {
                if (grid[p] < 10) 
                {
                    ++grid[p]; 
                    if (grid[p] >= 10) 
                    { 
                        ++numFlashes; 
                        numFlashes += Flash( p, grid ); 
                    }
                }
            }

            grid.CellStep( (p, v) => v >= 10 ? 0 : v ); 
            return numFlashes; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            IntHeatMap2D grid = new IntHeatMap2D( lineInput ); 


            int numDays = 100; 
            int numFlashes = 0; 
            for (int i = 0; i < numDays; ++i)
            {
                numFlashes += RunDay( grid ); 
            }

            return numFlashes.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            IntHeatMap2D grid = new IntHeatMap2D( lineInput ); 

            int numDays = 0; 
            int flashCount = 0; 
            while (flashCount < 100)
            {
                ++numDays; 
                flashCount = RunDay( grid ); 
            }

            return numDays.ToString(); 
        }
    }
}

