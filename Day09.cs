using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day09 : Day
 {
        private string InputFile = "inputs/07.txt"; 
        IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            // Program.EnqueueInput(1); 
            // Program.EnqueueInput(0); 
            // Program.Run();

            Util.WriteLine(Program.ToString()); 
            Util.WriteLine(""); 

            return ""; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            return ""; 
        }
    }
}
