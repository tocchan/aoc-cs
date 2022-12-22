using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day25 : Day
    {
        IntHeatMap2D Seafloor = new IntHeatMap2D(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/25.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            Seafloor.Init( lines[0].Length, lines.Count, 0 ); 
            foreach ((ivec2 pos, _) in Seafloor)
            {
                char c = lines[pos.y][pos.x]; 
                if (c == '>')
                {
                    Seafloor[pos] = 1; 
                }
                else if (c == 'v')
                {
                    Seafloor[pos] = 2; 
                }
            }
        }

        bool RunStep( IntHeatMap2D map )
        {
            bool moved = false; 
            map.CellStep( (p, v) => {
                if (v == 1) 
                {
                    ivec2 newPos = p + new ivec2(1, 0); 
                    if (newPos.x >= map.GetWidth())
                    {
                        newPos.x = 0; 
                    }

                    if (map[newPos] == 0)
                    {
                        moved = true; 
                        return 0; 
                    }
                    else
                    {
                        return v; 
                    }
                }
                else if (v == 0)
                {
                    ivec2 prevPos = p + new ivec2(-1, 0); 
                    if (prevPos.x < 0)
                    {
                        prevPos.x += map.GetWidth(); 
                    }

                    if (map[prevPos] == 1)
                    {
                        return 1; 
                    }
                    else
                    {
                        return 0; 
                    }
                }
                else
                {
                    return v; 
                }
            } ); 

            // downs
            map.CellStep( (p, v) => {
                if (v == 2) 
                {
                    ivec2 newPos = p + new ivec2(0, 1); 
                    if (newPos.y >= map.GetHeight())
                    {
                        newPos.y = 0; 
                    }

                    if (map[newPos] == 0)
                    {
                        moved = true; 
                        return 0; 
                    }
                    else
                    {
                        return v; 
                    }
                }
                else if (v == 0)
                {
                    ivec2 prevPos = p + new ivec2(0, -1); 
                    if (prevPos.y < 0)
                    {
                        prevPos.y += map.GetHeight(); 
                    }

                    if (map[prevPos] == 2)
                    {
                        return 2; 
                    }
                    else
                    {
                        return 0; 
                    }
                }
                else
                {
                    return v; 
                }
            } ); 

            return moved; 
        }

        void PrintBoard(int step, IntHeatMap2D board)
        {
            string c = $"Step {step}\n"; 
            int prevY = 0; 
            foreach ((ivec2 p, int v) in board)
            {
                if (prevY != p.y)
                {
                    c += '\n'; 
                    prevY = p.y; 
                }
                
                c += v switch
                {
                    0 => '.', 
                    1 => '>', 
                    2 => 'v',
                    _ => '#'
                }; 
            }

            c += '\n'; 
            Util.WriteLine(c); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            IntHeatMap2D floor = new IntHeatMap2D(Seafloor); 

            int step = 0; 
            while (RunStep(floor))
            {
                ++step; 
                // PrintBoard( step, floor ); 
            }

            return (step + 1).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            return ""; 
        }
    }
}
