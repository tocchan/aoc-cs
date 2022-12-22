using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day17 : Day
    {
        private string InputFile = "2021/inputs/17.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private (ivec2, ivec2) GetTargetArea( string line )
        {
            string usefulBit = line.Substring( line.IndexOf(':') + 2 ); 
            (string xBit, string yBit) = usefulBit.Split(','); 

            int[] xValues = xBit.Split('=', 2)[1].Split("..").Select(int.Parse).ToArray();
            int[] yValues = yBit.Split('=', 2)[1].Split("..").Select(int.Parse).ToArray();

            ivec2 p0 = new ivec2(xValues[0], yValues[0]); 
            ivec2 p1 = new ivec2(xValues[1], yValues[1]); 

            return (ivec2.Min(p0, p1), ivec2.Max(p0, p1)); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            (ivec2 min, ivec2 max) = GetTargetArea( lines[0] ); 

            // want a velocity that just hits the bottom, so....
            int maxVelY = Math.Abs(min.y) - 1; 
            int maxHeight = (maxVelY) * (maxVelY + 1) / 2; 
            
            return maxHeight.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private bool HitsTarget( ivec2 v, ivec2 min, ivec2 max )
        {
            ivec2 p = ivec2.ZERO; 
            while ((p.y >= min.y) && (p.x <= max.x))
            {
                if ((p >= min) && (p <= max))
                {
                    return true; 
                }

                p += v; 
                v.x = (v.x > 0) ? (v.x - 1) : 0; 
                v.y--; 
            }

            return false; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            (ivec2 min, ivec2 max) = GetTargetArea( lines[0] ); 

            // every possible y velocity that potentially hits it.
            int maxVelY = Math.Abs(min.y) - 1; 
            int minVelY = min.y; 

            // every possible x velocity that would hit it. 
            (float x0, float x1) = Util.Quadratic( 1, 1, -2.0f * min.x ); 
            int minVelX = (int) Math.Ceiling( Math.Max( x0, x1 ) ); 
            int maxVelX = max.x; 
            
            ivec2 minV = new ivec2(minVelX, minVelY); 
            ivec2 maxV = new ivec2(maxVelX, maxVelY); 

            int hits = 0; 
            ivec2 v; 
            for (v.y = minV.y; v.y <= maxV.y; ++v.y)
            {
                for (v.x = minV.x; v.x <= maxV.x; ++v.x)
                {
                    if (HitsTarget(v, min, max))
                    {
                        ++hits; 
                    }
                }
            }

            return hits.ToString(); 
        }
    }
}
