﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day05 : Day
 {
        private string InputFile = "inputs/05.txt"; 

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
            int programOutput = 0; 
            IntCodeMachine machine = new IntCodeMachine(IntCode); 
            machine.OnReadInput = () => 1; 
            machine.OnWriteOutput = (int output) => { 
                Util.WriteLine( "IntCode Out: " + output ); 
                programOutput = output; 
            }; 

            machine.Run(); 
            return programOutput.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int programOutput = 0; 
            IntCodeMachine machine = new IntCodeMachine(IntCode); 
            machine.OnReadInput = () => 5; 
            machine.OnWriteOutput = (int output) => { 
                Util.WriteLine( "IntCode Out: " + output ); 
                programOutput = output; 
            }; 

            machine.Run(); 
            return programOutput.ToString(); 
        }
    }
}
