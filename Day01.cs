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

            int fuelSum = 0; 
            for (int i = 0; i < values.Count; ++i)
            {
                int value = values[i]; 
                int fuel = (value / 3) - 2; 
                fuelSum += fuel; 
            }

            return fuelSum.ToString(); 
        }

        private int CalcFuelB(int mass)
        {
            int fuelCost = (mass / 3) - 2; 
            if (fuelCost <= 0)
            {
                return 0; 
            }

            return fuelCost + CalcFuelB(fuelCost); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFileB); 
            List<int> values = lines.Select(int.Parse).ToList(); 

            int fuelSum = 0; 
            for (int i = 0; i < values.Count; ++i)
            {
                int value = values[i]; 
                int fuel = CalcFuelB(value); 
                // Util.WriteLine(fuel.ToString()); 

                fuelSum += fuel; 
            }

            return fuelSum.ToString(); 
        }
    }
}
