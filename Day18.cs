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
    internal class Day18PathInfo
    {
        public Dictionary<char,ivec2> KeyPositions = new Dictionary<char, ivec2>(); 
        public Dictionary<char,ivec2> DoorPositions = new Dictionary<char, ivec2>(); 
        public Dictionary<char,Day18KeyInfo> KeyInfo = new Dictionary<char, Day18KeyInfo>(); 
        public ivec2 StartPosition; 

        public int CurrentDistance = 0; 
        public int BestDistance = int.MaxValue; 
        public Stack<char> CurrentPath = new Stack<char>(); 
        public Stack<char> BestPath = new Stack<char>();


        public void GetInitialState( IntHeatMap2D map )
        {
            int numPlayers = 0; 
            foreach ((ivec2 p, int v) in map) {
                if ((v <= 'Z') && (v >= 'A')) {
                    char doorId = (char) v; 
                    doorId = char.ToLower(doorId); 
                    DoorPositions[doorId] = p; 
                } else if ((v <= 'z') && (v >= 'a')) {
                    char keyId = (char) v; 
                    KeyPositions[keyId] = p; 
                } else if (v == '@') {
                    StartPosition = p; 
                    ++numPlayers; 

                    // treat the start position as just another key to generalize the algorithm
                    KeyPositions[(char)('0' + numPlayers)] = p; 
                }
            }
        }
    }

    internal class Day18KeyInfo
    {
        public char Key; 
        public Dictionary<char, (string, int)> KeyPaths = new Dictionary<char, (string, int)>(); 
    }

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day18 : Day
    {
        private string InputFile = "inputs/18.txt"; 
        private IntHeatMap2D OriginalMap = new IntHeatMap2D(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            OriginalMap.InitFromString(Util.ReadFileToString(InputFile)); 
        }

        //----------------------------------------------------------------------------------------------
        private string UpdateKey( string keys, char newKey )
        {
            int insertIdx = 0; 
            for (; insertIdx < keys.Length; ++insertIdx) {
                if (newKey < keys[insertIdx]) {
                    break; 
                }
            }

            return keys.Insert(insertIdx, newKey.ToString()); 
        }

        //----------------------------------------------------------------------------------------------
        private bool HasKeys( string acquiredKeys, string locks )
        {
            foreach (char c in locks) {
                if (!acquiredKeys.Contains(c)) {
                    return false; 
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        Dictionary<string,int> GetShortestCache = new Dictionary<string, int>(); 

        private int GetShortestPath( Day18PathInfo pathInfo, string startKeys, string acquiredKeys )
        {
            // end case
            if (acquiredKeys.Length == pathInfo.KeyInfo.Count) {
                return 0; 
            }

            int minDist = int.MaxValue; 
            string cacheKey = startKeys + acquiredKeys; 
            if (GetShortestCache.TryGetValue(cacheKey, out minDist)) {
                return minDist; 
            }


            minDist = int.MaxValue; 
            for (int i = 0; i < startKeys.Length; ++i) {
                char startKey = startKeys[i]; 
           
                Day18KeyInfo keyInfo = pathInfo.KeyInfo[startKey]; 
               
                // where can I go from where I am
                foreach ((char key, (string locks, int dist)) in keyInfo.KeyPaths) {
                    // if I was already here, or we don't have the keys to get here, jsut skip it
                    if (acquiredKeys.Contains(key) || !HasKeys(acquiredKeys, locks)) {
                        continue; 
                    }

                    StringBuilder newStartKeys = new StringBuilder(startKeys);
                    newStartKeys[i] = key; 
                    string newAcquiredKeys = UpdateKey( acquiredKeys, key ); 

                    int remainingDist = GetShortestPath( pathInfo, newStartKeys.ToString(), newAcquiredKeys ); 
                    if (remainingDist < int.MaxValue) {
                        minDist = Math.Min( minDist, dist + remainingDist ); 
                    }
                }
            }

            GetShortestCache[cacheKey] = minDist; 
            return minDist; 
        }

        
        //----------------------------------------------------------------------------------------------
        private Day18PathInfo GetInitialState()
        {
            Day18PathInfo pathInfo = new Day18PathInfo(); 
            pathInfo.GetInitialState(OriginalMap); 

            foreach ((char key, ivec2 pos) in pathInfo.KeyPositions) {
                // get path to every other key
                IntHeatMap2D keyMap = OriginalMap.FloodFill( pos, (int val) => (val == '#') ? -1 : 1 ); 
                Day18KeyInfo keyInfo = new Day18KeyInfo(); 
                keyInfo.Key = key; 

                foreach ((char otherKey, ivec2 otherPos) in pathInfo.KeyPositions) {
                    if (otherKey == key) {
                        continue; 
                    }

                    List<ivec2> path = keyMap.GetSlopePath(otherPos); 
                    if (path.Count == 0) { // not path found, don't add any information for it
                        continue; 
                    }

                    string keysRequired = ""; 
                    foreach (ivec2 pathPos in path) {
                        int space = OriginalMap.Get(pathPos); 
                        if ((space >= 'A') && (space <= 'Z')) {
                            keysRequired += Char.ToLower((char)space); 
                        }
                    }
                    keysRequired = String.Concat(keysRequired.OrderBy(c => c)); 


                    keyInfo.KeyPaths[otherKey] = (keysRequired, path.Count - 1);  
                }

                pathInfo.KeyInfo[key] = keyInfo; 
            }

            return pathInfo; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Day18PathInfo pathInfo = GetInitialState(); 
            int shortest = GetShortestPath( pathInfo, "1", "1" ); 

            return shortest.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            ivec2 startPos = OriginalMap.FindLocation((ivec2 pos, int v) => v == '@')!.Value; 

            // update the map
            OriginalMap.Set(startPos, '#'); 
            OriginalMap.Set(startPos + ivec2.UP, '#'); 
            OriginalMap.Set(startPos + ivec2.LEFT, '#'); 
            OriginalMap.Set(startPos + ivec2.RIGHT, '#'); 
            OriginalMap.Set(startPos + ivec2.DOWN, '#'); 

            OriginalMap.Set(startPos + new ivec2(1, 1), '@'); 
            OriginalMap.Set(startPos + new ivec2(1, -1), '@'); 
            OriginalMap.Set(startPos + new ivec2(-1, 1), '@'); 
            OriginalMap.Set(startPos + new ivec2(-1, -1), '@'); 

            Day18PathInfo pathInfo = GetInitialState(); 
            int shortest = GetShortestPath( pathInfo, "1234", "1234" ); 
            return shortest.ToString(); 
        }
    }
}
