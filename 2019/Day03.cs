using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day03 : Day
    {
        private string InputFile = "2019/inputs/03.txt"; 

        //----------------------------------------------------------------------------------------------
        // values
        List<ivec2> PathDirA = new List<ivec2>(); 
        List<iaabb2> PathA = new List<iaabb2>(); 

        List<ivec2> PathDirB = new List<ivec2>(); 
        List<iaabb2> PathB = new List<iaabb2>(); 


        //----------------------------------------------------------------------------------------------
        private void ParsePath( List<ivec2> path, string line )
        {
            path.AddRange( line.Split(',').Select<string, ivec2>( d => {
                char dir = d[0]; 
                int dist = int.Parse(d.Substring(1)); 
                return dir switch {
                    'R' => new ivec2(dist, 0),
                    'U' => new ivec2(0, dist), 
                    'L' => new ivec2(-dist, 0),
                    'D' => new ivec2(0, -dist),
                    _ => ivec2.ZERO
                };
            } ) ); 
        }

        //----------------------------------------------------------------------------------------------
        private void GeneratePath( List<iaabb2> path, List<ivec2> dirs )
        {
            ivec2 cursor = ivec2.ZERO; 
            foreach (ivec2 dir in dirs) {
                ivec2 next = cursor + dir; 
                path.Add( iaabb2.ThatContains(cursor, next) ); 
                cursor = next; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            ParsePath( PathDirA, lines[0] ); 
            GeneratePath( PathA, PathDirA ); 

            ParsePath( PathDirB, lines[1] ); 
            GeneratePath( PathB, PathDirB ); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int minDist = int.MaxValue; 
            foreach (iaabb2 edgeA in PathA) {
                foreach (iaabb2 edgeB in PathB) {
                    iaabb2 overlap = edgeA.GetOverlap(edgeB); 
                    if (overlap.IsValid()) {
                        // making an assumption that later edges will not cross at zero
                        // either.  Not technically correct, but doubt the problem set will do it
                        int dist = overlap.MinInclusive.GetManhattanDistance(); 
                        if ((dist > 0) && (dist < minDist)) {
                            minDist = dist; 
                        }
                    }
                }
            }

            return minDist.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int minSteps = int.MaxValue; 

            ivec2 cursorA = ivec2.ZERO; 
            int lengthA = 0; 

            ivec2 cursorB = ivec2.ZERO;
            int lengthB = 0; 

            for (int a = 0; a < PathA.Count; ++a) {
                if (lengthA >= minSteps) {
                    break; 
                }

                iaabb2 edgeA = PathA[a]; 
                cursorB = ivec2.ZERO; 
                lengthB = 0; 
                
                for (int b = 0; b < PathB.Count; ++b) {
                    iaabb2 edgeB = PathB[b]; 
                    iaabb2 overlap = edgeA.GetOverlap(edgeB); 
                    if (overlap.IsValid()) {
                        ivec2 intersection = overlap.MinInclusive; 
                        int stepsA = lengthA + (intersection - cursorA).GetManhattanDistance(); 
                        int stepsB = lengthB + (intersection - cursorB).GetManhattanDistance(); 
                        int steps = stepsA + stepsB; 

                        if ((steps > 0) && (steps < minSteps)) {
                            minSteps = steps; 
                        }
                    }

                    lengthB += (int) edgeB.GetArea() - 1; 
                    cursorB += PathDirB[b]; 
                }

                lengthA += (int) edgeA.GetArea() - 1; 
                cursorA += PathDirA[a]; 
            }

            return minSteps.ToString(); 
        }
    }
}
