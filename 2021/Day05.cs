using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day05 : Day
    {
        private string InputFile = "2021/inputs/05.txt"; 
        // private string DebugInputFile = "2021/inputs/05d.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private struct Line
        {
            public ivec2 Start; 
            public ivec2 End; 

            public static Line Parse( string s )
            {
                string[] parts = s.Split("->"); 
                Line l = new Line(); 
                l.Start = ivec2.Parse( parts[0] ); 
                l.End = ivec2.Parse( parts[1] ); 

                return l; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            List<Line> lines = lineInput.Select(Line.Parse).ToList(); 

            // create the board
            ivec2 size = new ivec2(); 
            foreach (Line line in lines)
            {
                ivec2 max = ivec2.Max(line.Start, line.End); 
                size = ivec2.Max(max, size); 
            }; 

            size += new ivec2(1, 1); // 9 is a valid point, so be one larger; 
            int[,] boards = new int[size.x, size.y]; 

            foreach (Line line in lines)
            {
                ivec2 dir = ivec2.Sign(line.End - line.Start); 

                // skip diagonals for now; 
                if ((dir.x * dir.y) != 0)
                {
                    // if it is a single point, still do it; 
                    if (dir == ivec2.ZERO) 
                    {
                        boards[line.Start.x, line.Start.y]++; 
                    }

                    continue; 
                }

                // add the points; 
                for (ivec2 iter = line.Start; iter != line.End; iter += dir)
                {
                    boards[iter.x, iter.y]++; 
                }
                boards[line.End.x, line.End.y]++; 
            }

            // count number of ones greater than one
            int count = 0; 
            for (int x = 0; x < size.x; ++x)
            {
                for (int y = 0; y < size.y; ++y)
                {
                    if (boards[x, y] >= 2)
                    {
                        ++count;
                    }
                }
            }

            return count.ToString(); 
        }

        


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            List<Line> lines = lineInput.Select(Line.Parse).ToList(); 

            // create the board
            ivec2 size = new ivec2(); 
            foreach (Line line in lines)
            {
                ivec2 max = ivec2.Max(line.Start, line.End); 
                size = ivec2.Max(max, size); 
            }; 

            size += new ivec2(1, 1); // 9 is a valid point, so be one larger; 
            int[,] boards = new int[size.x, size.y]; 

            foreach (Line line in lines)
            {
                ivec2 dir = ivec2.Sign(line.End - line.Start); 

                // add the points; 
                for (ivec2 iter = line.Start; iter != line.End; iter += dir)
                {
                    boards[iter.x, iter.y]++; 
                }
                boards[line.End.x, line.End.y]++; 
            }

            // count number of ones greater than one
            int count = 0; 
            for (int x = 0; x < size.x; ++x)
            {
                for (int y = 0; y < size.y; ++y)
                {
                    if (boards[x, y] >= 2)
                    {
                        ++count;
                    }
                }
            }

            return count.ToString(); 
        }
    }
}
