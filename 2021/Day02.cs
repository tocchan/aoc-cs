using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day02 : Day
    {
        private string InputFileA = "2021/inputs/02.txt"; 
        private string InputFileB = "2021/inputs/02.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private (string, int) ParseInstruction( string instruction )
        {
            string[] parts = instruction.Split(' ', 2); 
            string dir = parts[0]; 
            int amount = int.Parse(parts[1]); 

            return (dir, amount); 
        }

        //----------------------------------------------------------------------------------------------
        private (int, int) ParseDirection( string line )
        {
            (string dir, int amount) = ParseInstruction(line); 
            return dir[0] switch
            {
                'f' => (amount, 0),
                'd' => (0, amount), 
                'u' => (0, -amount), 
                _ => (0, 0)
            }; 
        }

        //----------------------------------------------------------------------------------------------
        private (int, int) ComputePositionA( List<string> lines )
        {
            int x = 0;
            int y = 0;

            foreach (string line in lines)
            { 
                (int dx, int dy) = ParseDirection(line); 
                y += dy; 
                x += dx;
            }

            return (x, y); 
        }

        //----------------------------------------------------------------------------------------------
        private (int, int) ComputePositionB( List<string> lines )
        {
            int a = 0; 
            int x = 0;
            int y = 0;

            foreach (string line in lines)
            { 
                (int dx, int dy) = ParseDirection(line); 
                a += dy; 
                y += dx * a; 
                x += dx;
            }

            return (x, y); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFileA); 

            (int x, int y) = ComputePositionA(lines); 
         
            return (x * y).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFileB); 

            (int x, int y) = ComputePositionB(lines); 
         
            return (x * y).ToString(); 
        }
    }
}
