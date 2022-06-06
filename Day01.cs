using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day01 : Day
    {
        private string InputFileA = "inputs/01.txt"; 
        private string InputFileB = "inputs/01.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFileA); 
            List<int> values = lines.Select(int.Parse).ToList(); 

            int count = 0; 
            for (int i = 1; i < values.Count; ++i)
            {
                if (values[i] > values[i - 1])
                {
                    ++count; 
                }
            }

            return count.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFileB); 
            List<int> values = lines.Select(int.Parse).ToList(); 

            int count = 0; 
            int prevSum = values[0] + values[1] + values[2]; 

            for (int i = 3; i < values.Count; ++i)
            {
                int sum = values[i] + values[i - 1] + values[i - 2]; 
                if (sum > prevSum)
                {
                    ++count; 
                }
                prevSum = sum;
            }

            return count.ToString(); 
        }
    }
}
