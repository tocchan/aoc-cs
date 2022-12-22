using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day06 : Day
    {
        private string InputFile = "2021/inputs/06.txt"; 
        // private string DebugInputFile = "2021/inputs/06d.txt"; 
        
        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public void RunDay( Int64[] fishAlive )
        {
            Int64 birthingFish = fishAlive[0]; 
            for (int i = 0; i < fishAlive.Length - 1; ++i)
            {
                fishAlive[i] = fishAlive[i + 1]; 
            }

            fishAlive[8] = birthingFish; 
            fishAlive[6] += birthingFish; 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 RunForDays( int days )
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 

            List<int> initialFish = lineInput[0].Split(',').Select(int.Parse).ToList(); 

            Int64[] fishAlive = new Int64[9]; // index is days in cycle left... 
            foreach (var fish in initialFish)
            {
                ++fishAlive[fish]; 
            }

            for (int i = 0; i < days; ++i)
            {
                // Int64 totalFish = fishAlive.Sum();
                // Console.WriteLine( $"Day {i}: {totalFish}");

                RunDay(fishAlive); 
            }

            Int64 total = fishAlive.Sum(); 
            return total;
        }
     
        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            return RunForDays(80).ToString(); 
        }


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            return RunForDays(256).ToString();
        }
    }
}
