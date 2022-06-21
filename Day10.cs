using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day10 : Day
 {
        private string InputFile = "inputs/10d.txt"; 
        private List<vec2> Asteroids = new List<vec2>();

        private vec2 StationPosition = vec2.ZERO; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            for (int y = 0; y < lines.Count; ++y) {
                for (int x = 0; x < lines[y].Length; ++x) {
                    char c = lines[y][x]; 
                    if (c == '#') {
                        vec2 pos = new vec2( (float)x, (float)y ); 
                        Asteroids.Add(pos); 
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        bool HasLineOfSight(int startIdx, int endIdx)
        {
            line2 line = new line2(Asteroids[startIdx], Asteroids[endIdx]); 
            for (int i = 0; i < Asteroids.Count; ++i) {
                if ((i == startIdx) || (i == endIdx)) {
                    continue; 
                }

                if (line.IsTouching(Asteroids[i], 0.001f)) {
                    return false; 
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        private int GetVisibleAsteroids(int idx)
        {
            HashSet<int> uniques = new HashSet<int>(); 
            vec2 asteroid = Asteroids[idx]; 

            for (int i = 0; i < Asteroids.Count; ++i) {
                if (i == idx) {
                    continue; 
                }

                ivec2 disp = vec2.RoundToInt(Asteroids[i] - asteroid); 
                int gdc = Util.GCD( disp.x, disp.y ); 
                disp  = disp / gdc; 
                int key = 10000 * disp.x + disp.y; 
                uniques.Add(key); 
            }

            return uniques.Count;
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int maxVisible = 0; 
            int bestIdx = 0; 

            // Util.WriteLine(Program.ToString()); 
            for (int i = 0; i < Asteroids.Count; ++i) {
             //   int i = 8; 
                int visible = GetVisibleAsteroids(i); 
                if (visible > maxVisible) {
                    bestIdx = i; 
                    maxVisible = visible; 
                }
            }

            // setup for the next part - remove this asteroid and create a station there
            StationPosition = Asteroids[bestIdx]; 
            Asteroids.RemoveAt(bestIdx); 

            return maxVisible.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private vec2 GetNearest( List<vec2> points, vec2 point )
        {
            return points.MinBy(p => (p - point).GetLengthSquared()); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            // goal here is to sort all asteroids by angle around the object, put into buckets slope. 
            SortedDictionary<double,List<vec2>> toHit = new SortedDictionary<double, List<vec2>>(); 
            for (int i = 0; i < Asteroids.Count; ++i) {
                vec2 pos = Asteroids[i]; 
                ivec2 disp = vec2.RoundToInt(pos - StationPosition); 

                // get an angle, put it in 0 to 360 degrees rotating clockwise around "up"
                double angle = Math.Atan2( disp.x, -disp.y ); 
                if (angle < 0.0) {
                    angle += 2.0 * Math.PI; 
                }

                if (!toHit.ContainsKey(angle)) {
                    toHit.Add( angle, new List<vec2>() ); 
                }

                toHit[angle].Add(Asteroids[i]); 
            }

            // go through, and start destroying asteroids
            int destroyed = 0; 
            foreach ((double angle, List<vec2> asteroids) in toHit) {
                if (asteroids.Count == 0) {
                    continue; 
                }

                vec2 pos = GetNearest(asteroids, StationPosition); 
                asteroids.Remove(pos); 

                // Util.WriteLine(angle.ToString() + ", " + pos.ToString() ); 
                ++destroyed; 
                if (destroyed == 200) {
                    ivec2 ipos = vec2.RoundToInt(pos - new vec2(0.5f)); 
                    int result = 100 * ipos.x + ipos.y;
                    return result.ToString(); 
                }
            }

            return "error"; 
        }
    }
}
