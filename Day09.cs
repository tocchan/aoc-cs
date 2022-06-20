using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day09 : Day
 {
        private string InputFile = "inputs/09.txt"; 
        IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            // Util.WriteLine(Program.ToString()); 

            Program.Reset(); 
            Program.EnqueueInput(1); 
            Program.Run();

            return Program.TryDequeueOutput().ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            Program.Reset(); 
            Program.EnqueueInput(2); 
            Program.Run(); 

            return Program.TryDequeueOutput().ToString(); 
        }
    }
}
