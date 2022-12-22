using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day09 : Day
    {
        private string InputFile = "2021/inputs/09.txt"; 
     
        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private int GetHeight( int[,] heightmap, int x, int y )
        {
            int w = heightmap.GetLength(0); 
            int h = heightmap.GetLength(1); 

            if ((x < 0) || (x >= w) || (y < 0) || (y >= h))
            {
                return int.MaxValue; 
            }
            else
            {
                return heightmap[x, y]; 
            }
        }

        private bool IsLowPoint( int[,] heightmap, int x, int y )
        {
            int v = GetHeight( heightmap, x, y ); 
            
            return (v < GetHeight(heightmap, x - 1, y))
                && (v < GetHeight(heightmap, x + 1, y))
                && (v < GetHeight(heightmap, x, y - 1))
                && (v < GetHeight(heightmap, x, y + 1)); 
        }

        //----------------------------------------------------------------------------------------------
        private int[,] ParseInput( List<string> input )
        {
            int width = input[0].Length; 
            int height = input.Count; 

            int[,] heightmap = new int[width, height]; 
            int y = 0; 
            foreach (string line in input)
            {
                int x = 0; 
                foreach( char c in line )
                {
                    heightmap[x, y] = (int)(c - '0'); 
                    ++x; 
                }
                ++y; 
            }

            return heightmap; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            int[,] heightMap = ParseInput(lineInput); 

            int riskLevel = 0; 

            int w = heightMap.GetLength(0); 
            int h = heightMap.GetLength(1); 
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (IsLowPoint(heightMap, x, y))
                    {
                        int height = GetHeight(heightMap, x, y); 
                        riskLevel += 1 + height; 
                    }
                }
            }

            return riskLevel.ToString(); 
        }


        int GetBasinSize(int[,] heightmap, int x, int y)
        {
            int w = heightmap.GetLength(0); 
            int h = heightmap.GetLength(1); 
            bool[,] visited = new bool[w, h]; 

            Queue<ivec2> toVisit = new Queue<ivec2>();


            toVisit.Enqueue( new ivec2(x, y) ); 
            visited[x, y] = true; 

            int size = 0; 
            while (toVisit.Count > 0)
            {
                ivec2 point = toVisit.Dequeue(); 

                ++size; 

                ivec2[] toCheck =
                {
                    point + new ivec2(-1, 0),
                    point + new ivec2(1, 0), 
                    point + new ivec2(0, 1), 
                    point + new ivec2(0, -1)
                }; 

                foreach (ivec2 p in toCheck)
                {
                    // can't be an edge (9 can't be part of a basin)
                    int pHeight = GetHeight(heightmap, p.x, p.y); 
                    if (pHeight >= 9)
                    {
                        continue; 
                    }

                    // don't double check (doing this second as the GetHeight() will do a bounds check for us; 
                    if (visited[p.x, p.y])
                    {
                        continue; 
                    }

                    // has to be higher than where we're coming from
                    if (pHeight <= GetHeight(heightmap, point.x, point.y))
                    {
                        continue; 
                    }

                    // part of the basin
                    visited[p.x, p.y] = true; 
                    toVisit.Enqueue(p); 
                }
            }

            return size; 
        }


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            int[,] heightMap = ParseInput(lineInput); 

            int w = heightMap.GetLength(0); 
            int h = heightMap.GetLength(1); 

            List<int> largestBasins = new List<int>(); 

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (IsLowPoint(heightMap, x, y))
                    {
                        int basinSize = GetBasinSize(heightMap, x, y);
                        largestBasins.Add(basinSize); 
                        if (largestBasins.Count > 3)
                        {
                            largestBasins.Remove(largestBasins.Min()); 
                        }
                    }
                }
            }

            int answer = 1; 
            foreach (int val in largestBasins)
            {
                answer *= val; 
            }

            return answer.ToString(); 
        }
    }
}

