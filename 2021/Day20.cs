using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day20 : Day
    {

        private string Algorithm = ""; 
        private IntHeatMap2D Original = new IntHeatMap2D();
        

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/20.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            Algorithm = lines[0]; 

            int height = lines.Count - 2; 
            int width = lines[2].Length; 
            Original.Resize( width, height ); 

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Original.Set( x, y, (lines[y + 2][x] == '#') ? 1 : 0 ); 
                }
            }
            Original.SetBoundsValue( 0 ); 
        }

        //----------------------------------------------------------------------------------------------
        private int ComputeNextValue( IntHeatMap2D map, ivec2 pos )
        {
            int idx = 0; 
            int power = 8; 

            ivec2 p;
            for (p.y = pos.y - 1; p.y <= pos.y + 1; ++p.y)
            {
                for (p.x = pos.x - 1; p.x <= pos.x + 1; ++p.x)
                {
                    int val = map.Get( p ); 
                    idx |= val << power; 
                    --power; 
                }
            }

            return (Algorithm[idx] == '#') ? 1 : 0; 
        }

        //----------------------------------------------------------------------------------------------
        private void DebugDraw( IntHeatMap2D map )
        {
            /*
            for (int y = 0; y < map.GetHeight(); ++y)
            {
                string line = ""; 
                for (int x = 0; x < map.GetWidth(); ++x)
                {
                    line += map.Get( x, y ) == 1 ? '#' : '.'; 
                }
                Util.WriteLine( line ); 
            }

            Util.WriteLine(""); 
            */
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int steps = 2; 

            IntHeatMap2D current = new IntHeatMap2D(Original); 
            DebugDraw( current ); 

            for (int step = 0; step < steps; ++step)
            {
                int newBorderValue = (Algorithm[0] == '.') ? current.GetBoundsValue() : (1 - current.GetBoundsValue()); 
                IntHeatMap2D next = new IntHeatMap2D( current.GetSize() + new ivec2(2, 2), newBorderValue ); 
                next.Copy( current, 1, 1 ); 

                ivec2 pos; 
                for (pos.y = 0; pos.y < next.GetHeight(); ++pos.y)
                {
                    for (pos.x = 0; pos.x < next.GetWidth(); ++pos.x)
                    {
                        int val = ComputeNextValue( current, pos - ivec2.ONE ); 
                        next.Set( pos, val ); 
                    }
                }

                current = next; 
                DebugDraw( current ); 

                // next.TrimBorder()
            }

            return current.Count( 1 ).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int steps = 50; 

            IntHeatMap2D current = new IntHeatMap2D(Original); 
            DebugDraw( current ); 

            for (int step = 0; step < steps; ++step)
            {
                int newBorderValue = (Algorithm[0] == '.') ? current.GetBoundsValue() : (1 - current.GetBoundsValue()); 
                IntHeatMap2D next = new IntHeatMap2D( current.GetSize() + new ivec2(2, 2), newBorderValue ); 
                next.Copy( current, 1, 1 ); 

                ivec2 pos; 
                for (pos.y = 0; pos.y < next.GetHeight(); ++pos.y)
                {
                    for (pos.x = 0; pos.x < next.GetWidth(); ++pos.x)
                    {
                        int val = ComputeNextValue( current, pos - ivec2.ONE ); 
                        next.Set( pos, val ); 
                    }
                }

                current = next; 
                DebugDraw( current ); 

                // next.TrimBorder()
            }

            return current.Count( 1 ).ToString(); 
        }
    }
}
