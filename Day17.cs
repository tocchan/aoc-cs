using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day17 : Day
 {
        private string InputFile = "inputs/17.txt"; 
        private IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        private bool IsIntersection(IntHeatMap2D map, ivec2 pos)
        {
            const int scaffold = 35; 
            return (map.Get(pos) == scaffold)
                && (map.Get(pos + ivec2.LEFT) == scaffold)
                && (map.Get(pos + ivec2.RIGHT) == scaffold)
                && (map.Get(pos + ivec2.UP) == scaffold)
                && (map.Get(pos + ivec2.DOWN) == scaffold);
        }

        private IntHeatMap2D Map = new IntHeatMap2D(); 

        private string DrawMap(bool print)
        {
            char prevc = '\0'; 
            string mapStr = ""; 
            while (Program.HasOutput()) {
                Int64 output = Program.TryDequeueOutput(0); 
                char c = (char) output; 
                mapStr += c; 

                if ((c == '\n') && (prevc == '\n')) {
                    break; 
                }
                prevc = c; 
            }

            if (print) {
                Console.SetCursorPosition(0, 0); 
                Util.WriteLine(mapStr); 
            }

            return mapStr; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Program.RunUntilRequiresInput(); 

            string mapStr = DrawMap(false); 
            Map.InitFromString(mapStr); 

            Console.SetWindowSize(Map.GetWidth() + 1, Map.GetHeight() + 1); 

            int checksum = 0; 
            foreach (ivec2 p in Map.FindLocations( (ivec2 pos) => IsIntersection(Map, pos) )) {
                checksum += p.Product();
            }


            return checksum.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private bool DetermineNextMove(StringBuilder moves, ref ivec2 pos, ref ivec2 dir)
        {
            // first, can I move forward?
            ivec2 nextPos = pos + dir; 
            int val = Map.Get(nextPos); 
            if (val == '#') {
                // great, how much can I move forward?
                int dist = 1; 
                nextPos += dir; 
                while (Map.Get(nextPos) == '#') {
                    nextPos += dir; 
                    ++dist; 
                }

                moves.Append(dist.ToString()); 
                pos += dist * dir; 
                
                return true; 
            }

            // left or right?
            ivec2 left = dir.GetRotatedLeft(); 
            nextPos = pos + left; 
            if (Map.Get(nextPos) == '#') {
                moves.Append('L'); 
                dir = left; 
                return true; 
            }

            ivec2 right = dir.GetRotatedRight(); 
            nextPos = pos + right;
            if (Map.Get(nextPos) == '#') {
                moves.Append('R'); 
                dir = right; 
                return true; 
            }

            return false; 
        }

        //----------------------------------------------------------------------------------------------
        private string DeterminePath(ivec2 pos, ivec2 dir)
        {
            StringBuilder path = new StringBuilder(); 
            while (DetermineNextMove(path, ref pos, ref dir)) {
                path.Append(','); 
            }

            path.Remove(path.Length - 1, 1); // remove last
            return path.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private IEnumerable<string> RemoveSubpath(string path, string subpath, string set)
        {
            // return all permuations of me removing this subpath
            if (set == "C") {
                // special case, has to replace all of them
                string newPath = path.Replace(subpath, set); 
                if (IsFinished(newPath)) {
                    yield return newPath; 
                }

                yield break; 
            }

            // does not exist
            int startIdx = path.IndexOf(subpath); 
            while (startIdx >= 0) {
                string newPath = path.Remove(startIdx, subpath.Length).Insert(startIdx, set); 
                foreach (string s in RemoveSubpath(newPath, subpath, set)) {
                    yield return s; 
                }
                yield return newPath; 

                startIdx = path.IndexOf(subpath, startIdx + 1); 
            }
        }

        private bool IsValidSubpath(string path)
        {
            return !path.Contains('A') && !path.Contains('B') && !path.Contains('C');
        }

        //----------------------------------------------------------------------------------------------
        private bool IsFinished(string path)
        { 
            foreach (char c in path) {
                if ((c != 'A') 
                    && (c != 'B')
                    && (c != 'C')
                    && (c != ',')) {
                    return false; 
                }
            }

            return true; 
        }

        private int FindFirstValid(string path)
        {
            for (int i = 0; i < path.Length; ++i) {
                char c = path[i]; 
                if ((c != 'A') 
                    && (c != 'B')
                    && (c != 'C')
                    && (c != ',')) {
                    return i; 
                }
            }

            return -1; 
        }

        private int FindFirstInvalid(string path, int startIdx)
        {
            for (int i = startIdx; i < path.Length; ++i) {
                char c = path[i]; 
                if ((c == 'A')
                    || (c == 'B') 
                    || (c == 'C')) {
                    return i; 
                }
            }

            return path.Length; 
        }
        

        //----------------------------------------------------------------------------------------------
        private string BreakNext( string path, List<string> subpaths )
        {
            int start = FindFirstValid(path); 
            int end = FindFirstInvalid(path, start + 1) - 1; 
            if (path[end] == ',') {
                --end; 
            }

            int maxLen = Math.Min(20, end - start + 1); 
            for (int lenToFind = maxLen; lenToFind > 2; --lenToFind) {
                
                // ignore substrings that end on a comma
                int lastIdx = start + lenToFind - 1; 
                if (path[lastIdx] == ',') {
                    continue; 
                }

                // also if we're cutting a word in half, ignore it
                if ((lastIdx < (path.Length - 1)) && (path[lastIdx + 1] != ',')) {
                    continue; 
                }

                string subpath = path.Substring(start, lenToFind); 
                if ((subpath == "R,8,R,8") && (subpaths.Count == 0)) {
                    Util.WriteLine("hi"); 
                }

                string set = ((char)('A' + subpaths.Count)).ToString(); 
                foreach (string newPath in RemoveSubpath(path, subpath, set)) {
                    subpaths.Add(subpath); 

                    if (subpaths.Count == 3) {
                        if (IsFinished(newPath)) {
                             return newPath; 
                        }
                    } else {
                        string retPath = BreakNext(newPath, subpaths); 
                        if (!string.IsNullOrEmpty(retPath)) {
                            return retPath;
                        }
                    }

                    subpaths.RemoveAt(subpaths.Count - 1); // remove last
                }
            }

            return string.Empty; 
        }

        private (string, string[]) BreakPath( string path )
        {
            // so what path is going to do is, first, find the longest string it can
            // remove all instances of it, and then repeat on the remaining
            List<string> subpaths = new List<string>(); 
            string command = BreakNext( path, subpaths ); 

            if (command.Length != 0) {
                return (command, subpaths.ToArray()); 
            }

            return ("", new string[3]); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            // let's build my path; 
            ivec2? start = Map.FindLocation( (ivec2 pos, int val) => (val != '#') && (val != '.') ); 
            if (null == start) {
                return "failed"; 
            }

            ivec2 shipPos = start.Value; 
            ivec2 dir = Map.Get(shipPos) switch {
                'v' => ivec2.DOWN,
                '^' => ivec2.UP, 
                '>' => ivec2.RIGHT, 
                '<' => ivec2.LEFT,
                _ => ivec2.ZERO
            };

            string path = DeterminePath(shipPos, dir); 

            // now break it down to large substrings
            // lets be greedy about it.  

            (string commands, string[] subs) = BreakPath(path); 
            // (string commands, string[] subs) = BreakPath("R,8,R,8,R,4,R,4,R,8,L,6,L,2,R,4,R,4,R,8,R,8,R,8,L,6,L,2"); 
            
            Program.Reset(); 
            Program.Set(0, 2); 
            Program.SetTextInput(commands, true); 
            for (int i = 0; i < subs.Length; ++i) {
                Program.SetTextInput(subs[i], true); 
            }
            Program.SetTextInput("n", true); 

            Program.Run(); 

            Program.GetTextOuput(); // initial map
            Program.GetTextOuput(); // requesting input
            Program.GetTextOuput(); // final map

            Int64 dust = Program.TryDequeueOutput(0); 
            return dust.ToString(); 
        }
    }
}
