using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day15 : Day
    {
        private string InputFile = "2021/inputs/15.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            IntHeatMap2D heatmap = new IntHeatMap2D(); 
            heatmap.SetFromTightBlock( lines ); 

            List<ivec2> path = heatmap.FindPathDijkstra( ivec2.ZERO, heatmap.GetSize() - ivec2.ONE ); 
            int total = heatmap.SumValuesAlong( path ) - heatmap.Get(ivec2.ZERO); 

            return total.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            IntHeatMap2D stamp = new IntHeatMap2D(); 
            stamp.SetFromTightBlock( lines ); 

            IntHeatMap2D heatmap = new IntHeatMap2D(); 
            heatmap.Resize( stamp.GetSize() * 5 ); 

            // Construct the new map
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    int threat = x + y; 
                    IntHeatMap2D newStamp = new IntHeatMap2D(); 
                    newStamp.Resize( stamp.GetSize() ); 
                    foreach ((ivec2 pos, int height) in stamp)
                    {
                        int newHeight = height + threat; 
                        while (newHeight > 9)
                        {
                            newHeight -= 9; 
                        }

                        newStamp.Set(pos, newHeight); 
                    }

                    heatmap.Copy( newStamp, x * stamp.GetWidth(), y * stamp.GetHeight() ); 
                }
            }


            List<ivec2> path = heatmap.FindPathDijkstra( ivec2.ZERO, heatmap.GetSize() - ivec2.ONE ); 
            int total = heatmap.SumValuesAlong( path ) - heatmap.Get(ivec2.ZERO); 

            return total.ToString(); 
        }
    }
}
