using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day03 : Day
    {
        private string InputFileA = "2021/inputs/03.txt"; 
        private string InputFileB = "2021/inputs/03.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFileA); 
            int[] oneCount = new int[lines[0].Length];
            int threshold = lines.Count / 2; 

            foreach (string line in lines)
            {
                for (int i = 0; i < line.Length; ++i)
                {
                    if (line[i] == '1')
                    {
                        ++oneCount[i]; 
                    }
                }
            }

            string gamma = ""; 
            string epsilon = ""; 
            for (int i = 0; i < oneCount.Length; ++i)
            {
                if (oneCount[i] > threshold)
                {
                    gamma += '1'; 
                }
                else
                {
                    gamma += '0';
                }
            }

            for (int i = 0; i < gamma.Length; ++i)
            {
                epsilon += ( (gamma[i] == '0') ? '1' : '0' ); 
            }

            int gammaVal = (int) Util.BinaryStringToInt(gamma);
            int epsilonVal = (int) Util.BinaryStringToInt(epsilon); 
            return (gammaVal * epsilonVal).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        List<string> FilterList( List<string> original, int colIndex, char filter )
        {
            return original.Where( x => x[colIndex] == filter ).ToList(); 
        }

        //----------------------------------------------------------------------------------------------
        int SumColumn( List<string> lines, int colIndex )
        {
            int count = 0;
            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i][colIndex] == '1')
                {
                    ++count; 
                }
            }

            return count; 
        }

        //----------------------------------------------------------------------------------------------
        string FilterByBitCriteria( List<string> lines, Func<int, int, char> getFilterCharFunc )
        {
            int colIndex = 0; 
            while ((lines.Count > 1) && (colIndex < lines[0].Length))
            {
                int oneCount = SumColumn( lines, colIndex );
                int zeroCount = lines.Count - oneCount; 

                char filterChar = getFilterCharFunc(oneCount, zeroCount);
                lines = FilterList( lines, colIndex, filterChar ); 
                ++colIndex; 
            }

            return lines[0]; // assume input will always give me a line :)
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFileB); 

            Func<int, int, char> oxygenCriteria = (a, b) => (a >= b) ? '1' : '0';
            Func<int, int, char> c02Criteria = (a, b) => (a < b) ? '1' : '0';

            string oxygen = FilterByBitCriteria( lines, oxygenCriteria ); 
            string c02 = FilterByBitCriteria( lines, c02Criteria ); 

            int oxygenVal = (int) Util.BinaryStringToInt(oxygen); 
            int c02Val = (int) Util.BinaryStringToInt(c02); 

            return (oxygenVal * c02Val).ToString(); 
        }
    }
}
