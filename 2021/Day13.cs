using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day13 : Day
    {
        private string InputFile = "2021/inputs/13.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        char[,] ParsePage( ref List<string> lines )
        {
            List<ivec2> points = new List<ivec2>(); 

            for (int i = 0; i < lines.Count; ++i)
            {
                string line = lines[i]; 
                if (string.IsNullOrEmpty(line))
                {
                    lines.RemoveRange( 0, i + 1 ); 
                    break; 
                }

                points.Add( ivec2.Parse(line) ); 
            }

            // fill in the page; 
            ivec2 max = ivec2.Max( points ) + ivec2.ONE; 
            char[,] map = new char[max.x, max.y]; 
            for (int x = 0; x < max.x; ++x)
            {
                for (int y = 0; y < max.y; ++y)
                {
                    map[x, y] = ' '; 
                }
            }

            foreach (ivec2 point in points)
            {
                map[point.x, point.y] = '#'; 
            }

            return map; 
        }

        //----------------------------------------------------------------------------------------------
        List<ivec2> ParseFolds( ref List<string> lines )
        {
            List<ivec2> folds = new List<ivec2>(); 

            foreach (string line in lines)
            {

                (string dir, string coord) = line.Split('=', 2); 
                int val = int.Parse(coord); 
                if (dir.Last() == 'y')
                {
                    folds.Add( new ivec2(0, val) ); 
                }
                else
                {
                    folds.Add( new ivec2(val, 0) ); 
                }
            }

            return folds; 
        }

        //----------------------------------------------------------------------------------------------
        char[,] GetFoldedPage( char[,] page, ivec2 fold )
        { 
            ivec2 pageSize = new ivec2( page.GetLength(0), page.GetLength(1) ); 
            ivec2 newSize; 
            if (fold.x == 0) 
            { 
                if (fold.y >= pageSize.y)
                {
                    return page; 
                }

                newSize = new ivec2( pageSize.x, fold.y ); 
            }
            else
            {
                if (fold.x >= pageSize.x) 
                { 
                    return page;     
                }
                newSize = new ivec2( fold.x, pageSize.y ); 
            }

            // copy existing part of the page to the new page; 
            char[,] newPage = new char[newSize.x, newSize.y]; 
            for (int x = 0; x < newSize.x; ++x)
            {
                for (int y = 0; y < newSize.y; ++y)
                {
                    newPage[x, y] = page[x, y]; 
                }
            }

            // copy the folded over part; 
            ivec2 readMin = fold + ivec2.Sign(fold); 
            for (int x = pageSize.x - 1; x >= readMin.x; --x)
            {
                for (int y = pageSize.y - 1; y >= readMin.y; --y)
                {
                    // flip the coordinate across the y; 
                    ivec2 readCoord = new ivec2( x, y );  
                    ivec2 writeCoord; 
                    if (fold.x == 0)
                    {
                        int offset = y - fold.y;
                        writeCoord = new ivec2( x, fold.y - offset ); 
                    }
                    else
                    {
                        int offset = x - fold.x;
                        writeCoord = new ivec2( fold.x - offset, y ); 
                    }

                    if (page[readCoord.x, readCoord.y] == '#')
                    {
                        newPage[writeCoord.x, writeCoord.y] = '#'; 
                    }
                }
            }

            return newPage; 
        }

        //----------------------------------------------------------------------------------------------
        string PrintPage( char[,] page )
        {
            string c = ""; 
            for (int y = 0; y < page.GetLength(1); ++y)
            {
                for (int x = 0; x < page.GetLength(0); ++x)
                {
                    c += page[x, y]; 
                }
                c += '\n'; 
            }
            return c; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            char[,] paper = ParsePage( ref lineInput ); 
            List<ivec2> folds = ParseFolds( ref lineInput ); 

            paper = GetFoldedPage( paper, folds[0] ); 

            int count = 0; 
            foreach (char c in paper)
            {
                if (c == '#')
                {
                    ++count; 
                }
            }

            // Console.WriteLine( PrintPage( paper ) ); 
            return count.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            char[,] paper = ParsePage( ref lineInput ); 
            List<ivec2> folds = ParseFolds( ref lineInput ); 

            foreach( ivec2 fold in folds )
            {
                paper = GetFoldedPage( paper, fold ); 
            }
         
            return "\n" + PrintPage( paper ); 
        }
    }
}

