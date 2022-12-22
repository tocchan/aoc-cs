using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day24 : Day
    {
        Int64[] Registers = new Int64[4]; 

        List<byte[]> Programs = new List<byte[]>(); 
        List<bool> IsDependent = new List<bool>(); 

        //----------------------------------------------------------------------------------------------
        enum EOpCode : byte
        {
            NOP, 
            INP, // a -> a
            ADD, // a b -> a
            MUL, // a, b -> a
            DIV, // a, b -> a
            MOD, // a, b -> a
            EQL, // a, b
        }

        //----------------------------------------------------------------------------------------------
        enum EVarType : byte
        {
            REG,
            DATA,
        }

        //----------------------------------------------------------------------------------------------
        void Reset()
        {
            for (int i = 0; i < Registers.Length; ++i)
            {
                Registers[i] = 0; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/24d.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            List<byte> program = new List<byte>(); 
            bool isDependent = false; 

            foreach (string line in lines)
            {
                // add support for blank lines & comments
                if (string.IsNullOrEmpty(line.Trim()) || (line[0] == '#'))
                {
                    continue; 
                }

                (string inst, string inputs) = line.Split(' ', 2); 

                EOpCode code = inst switch
                {
                    "inp" => EOpCode.INP, 
                    "add" => EOpCode.ADD, 
                    "mul" => EOpCode.MUL, 
                    "div" => EOpCode.DIV, 
                    "mod" => EOpCode.MOD, 
                    "eql" => EOpCode.EQL, 
                    _ => EOpCode.NOP,
                }; 

                if ((code == EOpCode.INP) && (program.Count > 0))
                {
                    Programs.Add(program.ToArray()); 
                    IsDependent.Add(isDependent); 

                    program.Clear(); 
                    isDependent = false; 
                }

                program.Add( (byte) code ); 
                string[] vars = inputs.Split(' '); 
                
                program.Add( (byte)(vars[0][0] - 'w') ); 
                if (vars.Length > 1)
                {
                    char c = vars[1][0]; 
                    if ((c >= 'w') && (c <= 'z'))
                    {
                        program.Add( (byte) EVarType.REG ); 
                        program.Add( (byte)(c - 'w')); 
                    }
                    else
                    {
                        program.Add( (byte) EVarType.DATA ); 
                        int val = int.Parse(vars[1]); 
                        program.AddRange( BitConverter.GetBytes(val) ); 
                        isDependent = isDependent || (val < 0); 
                    }
                }
            }

            if (program.Count > 0)
            {
                Programs.Add(program.ToArray()); 
                IsDependent.Add(isDependent); 
            }

        }

        Int64 ReadValue( byte[] opcodes, ref int ip )
        {
            EVarType type = (EVarType) opcodes[ip++]; 
            if (type == EVarType.REG)
            {
                byte reg = opcodes[ip++]; 
                return Registers[reg]; 
            }
            else
            {
                int val = BitConverter.ToInt32( opcodes.AsSpan<byte>(ip, 4) ); 
                ip += 4; 
                return val; 
            }
        }

        public void RunProgram( byte[] program, int[] inputs, ref int readIndex )
        {
            int ip = 0; 

            while (ip < program.Length)
            {
                EOpCode opCode = (EOpCode) program[ip++]; 
                byte reg = program[ip++]; 

                switch (opCode)
                {
                    case EOpCode.INP: {
                        Registers[reg] = inputs[readIndex++]; 
                    } break; 

                    case EOpCode.ADD: {
                        Int64 val0 = Registers[reg]; 
                        Int64 val1 = ReadValue( program, ref ip ); 
                        Registers[reg] = val0 + val1; 
                    } break; 

                    case EOpCode.MUL: {
                        Int64 val0 = Registers[reg]; 
                        Int64 val1 = ReadValue( program, ref ip ); 
                        Registers[reg] = val0 * val1; 
                    } break; 
                    
                    case EOpCode.DIV: {
                        Int64 val0 = Registers[reg]; 
                        Int64 val1 = ReadValue( program, ref ip ); 
                        Registers[reg] = val0 / val1; 
                    } break; 

                    case EOpCode.MOD: {
                        Int64 val0 = Registers[reg]; 
                        Int64 val1 = ReadValue( program, ref ip ); 
                        Registers[reg] = val0 % val1; 
                    } break; 

                    case EOpCode.EQL: {
                        Int64 val0 = Registers[reg]; 
                        Int64 val1 = ReadValue( program, ref ip ); 
                        Registers[reg] = (val0 == val1) ? 1 : 0; 
                    } break; 

                    default: break; 
                }
            }
        }

        void RunProgram( int[] inputs )
        {
            Reset(); 
            int readIndex = 0; 

            foreach( byte[] program in Programs)
            {
                RunProgram( program, inputs, ref readIndex ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        void TraceRegisters()
        {
            for (int i = 0; i < Registers.Length; ++i)
            {
                char regName = (char)('w' + i); 
                Util.WriteLine( $"{regName} = {Registers[i]}"); 
            }
            Util.WriteLine("\n"); 
        }

        //----------------------------------------------------------------------------------------------
        bool FindHighestInput( int[] input, int digit )
        {
            Int64 state = Registers[3]; 
            if (digit >= input.Length)
            {
                return state == 0; 
            }

            bool isDependent = IsDependent[digit];

            input[digit] = 9; 
            while (input[digit] > 0)
            {
                Registers[3] = state; // reset before testing this step
                int readIndex = digit; 

                if (isDependent)
                { 
                    // pick the highest digit that works (lowers the value)
                    RunProgram( Programs[digit], input, ref readIndex ); 
                    Int64 newState = Registers[3]; 
                    if (newState == (state / 26))
                    {
                        if (FindHighestInput( input, digit + 1 ))
                        {
                            return true; 
                        }
                    }
                }
                else
                {
                    // not much I can do here except be greedy, so just try...
                    RunProgram( Programs[digit], input, ref readIndex ); 
                    if (FindHighestInput( input, digit + 1 ))
                    {
                        return true; 
                    }
                }

                --input[digit]; 
            }

            return false; 
        }

        //----------------------------------------------------------------------------------------------
        bool FindHighestInput( int[] input )
        {
            // Run program until we hit a dependent variable
            // If we can pick a dependent variable, pick it, and move on.  If not, reduce a previoud dependent variable unless we can't reduce it anymore, then set it to 9 and try again;
            Reset(); 
            return FindHighestInput( input, 0 ); 
        }

        //----------------------------------------------------------------------------------------------
        bool FindLowestInput( int[] input, int digit )
        {
            Int64 state = Registers[3]; 
            if (digit >= input.Length)
            {
                return state == 0; 
            }

            bool isDependent = IsDependent[digit];

            input[digit] = 1; 
            while (input[digit] < 10)
            {
                Registers[3] = state; // reset before testing this step
                int readIndex = digit; 

                if (isDependent)
                { 
                    // pick the highest digit that works (lowers the value)
                    RunProgram( Programs[digit], input, ref readIndex ); 
                    Int64 newState = Registers[3]; 
                    if (newState == (state / 26))
                    {
                        if (FindLowestInput( input, digit + 1 ))
                        {
                            return true; 
                        }
                    }
                }
                else
                {
                    // not much I can do here except be greedy, so just try...
                    RunProgram( Programs[digit], input, ref readIndex ); 
                    if (FindLowestInput( input, digit + 1 ))
                    {
                        return true; 
                    }
                }

                ++input[digit]; 
            }

            return false; 
        }

        //----------------------------------------------------------------------------------------------
        bool FindLowestInput( int[] input )
        {
            // Run program until we hit a dependent variable
            // If we can pick a dependent variable, pick it, and move on.  If not, reduce a previoud dependent variable unless we can't reduce it anymore, then set it to 9 and try again;
            Reset(); 
            return FindLowestInput( input, 0 ); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            // string input = "13579246899999"; 
            string input    = "99999999999999"; 
            int[] inputs = input.ToCharArray().Select<char,int>(c => (int) c - '0').ToArray(); 

            FindHighestInput( inputs ); 
            string answer = string.Concat( inputs.Select( i => i.ToString() ) ); 

            return answer; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            string input    = "99999999999999"; 
            int[] inputs = input.ToCharArray().Select<char,int>(c => (int) c - '0').ToArray(); 

            FindLowestInput( inputs ); 
            string answer = string.Concat( inputs.Select( i => i.ToString() ) ); 

            return answer; 
        }
    }
}
