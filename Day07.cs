using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day07 : Day
 {
        private string InputFile = "inputs/07.txt"; 

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

            Int64 maxPower = 0; 
            foreach (int[] perm in Util.GetPermutations(5)) {
                // int[] perm = new int[] { 0, 1, 2, 3, 4 }; 
                Int64 power = 0; 
                for (int i = 0; i < perm.Length; ++i) {
                    machine.Reset(); 
                    machine.SetInputs( perm[i], power ); 
                    machine.Run(); 
                    machine.DequeueOutput( out power ); 
                }

                maxPower = Math.Max(power, maxPower); 
            }

            return maxPower.ToString();
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            // IntCodeMachine machine = new IntCodeMachine("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5"); 
            IntCodeMachine machine = new IntCodeMachine(IntCode); 

            Int64 maxPower = 0; 
            foreach (int[] perm in Util.GetPermutations(5, 9)) {

                // setup machines with their permuation code
                IntCodeMachine[] amplifiers = new IntCodeMachine[5]; 
                for (int i = 0; i < amplifiers.Length; ++i) {
                    amplifiers[i] = new IntCodeMachine( machine.IntCode ); 
                    amplifiers[i].EnqueueInput( perm[i] ); 
                }

                // set initial input for the first machine, and setup machine links
                amplifiers[0].EnqueueInput( 0 ); 
                for (int i = 0; i < amplifiers.Length; ++i) {
                    int next = (i + 1) % 5; 
                    amplifiers[i].PipeTo( amplifiers[next] ); 
                }

                // run the last amplifier until it halts
                amplifiers[4].Run(); 

                // get the final output and see if it is the max
                Int64 output;
                amplifiers[4].DequeueOutput( out output ); 
                maxPower = Math.Max(output, maxPower); 
            }

            return maxPower.ToString(); 
        }
    }
}
