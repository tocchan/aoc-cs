using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
   

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day02 : Day
    {
        private string InputFile = "2019/inputs/02.txt"; 

        //----------------------------------------------------------------------------------------------
        // values
        int[] IntCode = new int[0]; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            String code = String.Join(",", lines); 
            
            IntCode = code.Split(',').Select(int.Parse).ToArray(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            IntCodeMachine machine = new IntCodeMachine(IntCode); 
            machine.Set(1, 12); 
            machine.Set(2, 2); 

            machine.Run(); 
            return machine.Get(0).ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            IntCodeMachine machine = new IntCodeMachine(IntCode); 

            for (int i = 0; i < 100; ++i) {
                for (int j = 0; j < 100; ++j) {
                    machine.Reset(); 
                    machine.Set(1, i); 
                    machine.Set(2, j); 

                    machine.Run();
                    Int64 result = machine.Get(0); 
                    if (result == 19690720) {
                        return (100 * i + j).ToString(); 
                    }
                }
            }

            return "break"; 
        }
    }
}
