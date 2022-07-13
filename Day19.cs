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
    internal class Day19 : Day
 {
        private string InputFile = "inputs/19.txt"; 
        private IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        public bool IsInTractor(int x, int y)
        {
            Program.Reset(); 
            Program.EnqueueInputs(x, y); 
            Program.Run(); 

            return (Program.TryDequeueOutput(0) == 1); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Program.Reset(); 

            StringBuilder sb = new StringBuilder(); 

            int area = 50; 
            for (int y = 0; y < area; ++y) {
                for (int x = 0; x < area; ++x) {
                    Program.Reset(); 
                    Program.EnqueueInput(x); 
                    Program.EnqueueInput(y); 
                    Program.Run(); 

                    Int64 space = Program.TryDequeueOutput(0); 
                    sb.Append( (space == 0) ? '.' : '#' ); 
                }

                sb.Append('\n'); 
            }
            
            Util.WriteLine(sb.ToString()); 
            int pullAreas = sb.ToString().Count((char c) => c == '#'); 

            return pullAreas.ToString(); 
        }

     
        public bool IsWideEnough( int x, int y, int size )
        {
            for (int i = 0; i < size; ++i) {
                if (!IsInTractor(x + i, y)) {
                    return false; 
                }
            }

            return true; 
        }

        public bool IsTallEnough( int x, int y, int size )
        {
            for (int i = 0; i < size; ++i) {
                if (!IsInTractor(x, y + i)) {
                    return false; 
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int x = 0; 
            int y = 50; 

            // find the right wall first
            while (!IsInTractor(x, y)) {
                ++x; 
            }

            while (IsInTractor(x, y)) {
                ++x; 
            }
            --x; // right wall; 

            // get starting x
            int targetSize = 100; 
            while (true) {
                // move to the right wall; 
                while (IsInTractor(x, y)) {
                    ++x; 
                }
                --x; 

                // okay, see if we're wide enough
                int left = x - targetSize + 1; 
                if (IsWideEnough(left, y, targetSize)) {
                    if (IsTallEnough(left, y, targetSize) && IsTallEnough(x, y, targetSize)) {
                        break; 
                    }
                }

                ++y; 
            }

            // yay
            Int64 pX = x - targetSize + 1; 
            Int64 pY = y; 
            Int64 result = pX * 10000 + pY; 

            return result.ToString(); 
        }
    }
}
