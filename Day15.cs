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
    internal class Day15 : Day
 {
        private string InputFile = "inputs/15.txt"; 
        private IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        private int GetMoveCode(ivec2 dir)
        {
            if (dir == ivec2.UP) {
                return 1; 
            } else if (dir == ivec2.DOWN) {
                return 2; 
            } else if (dir == ivec2.LEFT) {
                return 3; 
            } else if (dir == ivec2.RIGHT) {
                return 4;
            }

            return 0;
        }

        //----------------------------------------------------------------------------------------------
        private ivec2 GetDirection(int val) => val switch {
            1 => ivec2.UP, 
            2 => ivec2.DOWN, 
            3 => ivec2.LEFT, 
            4 => ivec2.RIGHT, 
            _ => ivec2.ZERO
        }; 

        //----------------------------------------------------------------------------------------------
        private void Move(ivec2 dir)
        {
            int moveCode = GetMoveCode(dir); 
            Int64 result = Program.Call(moveCode)[0]; 
            
            switch (result) {
                case 0: Map.SetValue(Position + dir, 2); break;
                case 1: Position += dir; Map.SetValue(Position, 1); break; 
                case 2: Position += dir; Map.SetValue(Position, 3); TargetPos = Position; break; 
            }
        }

        ivec2 Position; 
        IntCanvas Map = new IntCanvas(); 
        ivec2 TargetPos; 

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Position = new ivec2(0); 
            Map = new IntCanvas(); 

            Map.SetValue(ivec2.ZERO, 1);

            /*
            Program.OnReadInput = Int64() => {
                
                // draw map and get input
                System.Console.Clear(); 
                int oldVal = map.SetValue(position, 4); 
                Util.WriteLine(map.ToString(" .█oD")); 

                ConsoleKeyInfo key = System.Console.ReadKey();
                int dir = key.Key switch {
                    ConsoleKey.UpArrow => 1, 
                    ConsoleKey.DownArrow => 2,
                    ConsoleKey.LeftArrow => 3,
                    ConsoleKey.RightArrow => 4,
                    _ => 1
                };
                map.SetValue(position, oldVal); 

                lastDir = dir switch {
                    1 => ivec2.UP, 
                    2 => ivec2.DOWN, 
                    3 => ivec2.LEFT,
                    4 => ivec2.RIGHT,
                    _ => ivec2.ZERO
                };

                return dir; 
            };

            Program.OnWriteOutput = (Int64 val) => {
                switch (val) {
                    case 0: map.SetValue(position + lastDir, 2); break;
                    case 1: position += lastDir; map.SetValue(position, 1); break; 
                    case 2: position += lastDir; map.SetValue(position, 3); break; 
                }
            };
            */

            while (true) {
                List<ivec2> path = Map.FindPathTo( Position, 
                    bool(ivec2 pos, int val) => val == 0, 
                    int(ivec2 pos, int v) => ((v == 2) ? -1 : 0) );  
                
                if (path.Count == 0) {
                    break; 
                }

                foreach (ivec2 dir in path) {
                    Move(dir); 
                }
            }

            List<ivec2> finalPath = Map.FindPathTo(ivec2.ZERO, TargetPos, int(ivec2 pos, int v) => ((v == 2) ? -1 : 0) );

            // draw map and get input
            System.Console.Clear(); 
            Map.SetValue(Position, 4); 
            Util.WriteLine(Map.ToString(" .█oD")); 
            Map.SetValue(Position, 1); 

            return finalPath.Count.ToString(); 
        }

       
        
        //----------------------------------------------------------------------------------------------
        private int Rule( ivec2 pos, int val )
        {
            if ((val == 1) && Map.HasNeighbor(pos, 3)) {
                return 3; 
            }

            return val; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            System.Console.Clear(); 
            Console.CursorVisible = false; 

            int steps = 0; 
            while (Map.Automata(Rule) != 0) {
                ++steps; 

                // draw map and get input
                Console.SetCursorPosition(0, 0); 
                Util.WriteLine(Map.ToString(" .█oD")); 

                Thread.Sleep(15); 
            }

            // draw map and get input
            System.Console.Clear(); 
            Map.SetValue(Position, 4); 
            Util.WriteLine(Map.ToString(" .█oD")); 

            return steps.ToString(); 
        }
    }
}
