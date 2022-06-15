using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    public class IntCodeMachine
    {
        //----------------------------------------------------------------------------------------------
        // Constants
        private const int OP_ADD = 1; 
        private const int OP_MULT = 2; 
        private const int OP_INPUT = 3;
        private const int OP_OUTPUT = 4;
        private const int OP_JUMP_TRUE = 5; 
        private const int OP_JUMP_FALSE = 6;
        private const int OP_LESS = 7; 
        private const int OP_EQUALS = 8;
        private const int OP_HALT = 99; 

        public delegate int ReadInputCB(); 
        public delegate void WriteOutputCB(int val); 


        //----------------------------------------------------------------------------------------------
        public int[] ResetIntCode = new int[0]; 
        public int[] IntCode = new int[0]; 
        public int Offset = 0; 
        public int ParamOptions = 0; 

        public Queue<int> Inputs = new Queue<int>(); 
        public Queue<int> Outputs = new Queue<int>(); 
        public IntCodeMachine? InputProgram = null; 
        public IntCodeMachine? OutputProgram = null; 

        public ReadInputCB? OnReadInput;
        public WriteOutputCB? OnWriteOutput;

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine()
        {
        }

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine(int[] intCode)
        {
            Setup(intCode); 
        }

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine(string src)
        {
            Setup( src.Split(',').Select(int.Parse).ToArray() ); 
        }

        //----------------------------------------------------------------------------------------------
        public void Setup(int[] intCode)
        {
            IntCode = new int[intCode.Length]; 
            ResetIntCode = new int[intCode.Length]; 

            Array.Copy(intCode, 0, IntCode, 0, intCode.Length); 
            Array.Copy(intCode, 0, ResetIntCode, 0, intCode.Length); 

            Offset = 0; 
        }

        //----------------------------------------------------------------------------------------------
        public void PipeTo( IntCodeMachine machine )
        {
            machine.InputProgram = this; 
            OutputProgram = machine; 
        }

        //----------------------------------------------------------------------------------------------
        public void Reset()
        {
            Array.Copy(ResetIntCode, 0, IntCode, 0, ResetIntCode.Length); 
            Offset = 0; 

            Inputs.Clear(); 
            Outputs.Clear(); 
        }

        //----------------------------------------------------------------------------------------------
        public void EnqueueInput( int val )
        { 
            Inputs.Enqueue(val); 
        }

        //----------------------------------------------------------------------------------------------
        public bool DequeueInput( out int val )
        {
            // callback method
            val = 0; 
            if (OnReadInput != null) {
                val = OnReadInput.Invoke();
                return true; 
            }

            // queue method
            if (Inputs.Count == 0) {
                // piping method
                if (InputProgram != null) {
                    // run my src until I have something
                    InputProgram.RunUntil( () => { return InputProgram.HasOutput(); } ); 
                    
                    // get that thing
                    int output;
                    InputProgram.DequeueOutput( out output ); 
                    EnqueueInput(output); 
                } else {
                    return false; 
                }
            }

            val = Inputs.Dequeue(); 
            return true; 
        }
        

        //----------------------------------------------------------------------------------------------
        public void SetInputs( params int[] inputs )
        {
            Inputs.Clear(); 
            foreach (int val in inputs) {
                EnqueueInput(val);
            }
        }

        //----------------------------------------------------------------------------------------------
        public void EnqueueOutput( int val )
        {
            Outputs.Enqueue( val ); 
            if (OnWriteOutput != null) {
                OnWriteOutput( val ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public bool DequeueOutput( out int val )
        {
            return Outputs.TryDequeue( out val ); 
        }

        //----------------------------------------------------------------------------------------------
        public bool HasOutput()
        {
            return Outputs.Count > 0; 
        }

        //----------------------------------------------------------------------------------------------
        private int ReadNextOp()
        {
            int opCode = IntCode[Offset]; 
            ++Offset; 
            
            int op = opCode % 100; 
            ParamOptions = opCode / 100; // todo: convert this to binary so I'm not dividing as much reading params

            return op; 
        }

        //----------------------------------------------------------------------------------------------
        private int ReadParam(int addr)
        {
            bool isAddr = (ParamOptions & 1) == 0; 
            ParamOptions /= 10; 

            return isAddr ? IntCode[addr] : addr; 
        }

        //----------------------------------------------------------------------------------------------
        private int ReadParam()
        {
            int addr = IntCode[Offset]; 
            ++Offset; 
            return ReadParam(addr); 
        }

        //----------------------------------------------------------------------------------------------
        private void WriteParam(int val)
        {
            int addr = IntCode[Offset]; 
            ++Offset; 

            IntCode[addr] = val;
        }

        //----------------------------------------------------------------------------------------------
        private void RunAdd()
        {
            int srcA = IntCode[Offset + 0]; 
            int srcB = IntCode[Offset + 1]; 
            int dst = IntCode[Offset + 2]; 
            Offset += 3; 

            IntCode[dst] = ReadParam(srcA) + ReadParam(srcB); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunMultiply()
        {
            int srcA = IntCode[Offset + 0]; 
            int srcB = IntCode[Offset + 1]; 
            int dst = IntCode[Offset + 2]; 
            Offset += 3; 

            IntCode[dst] = ReadParam(srcA) * ReadParam(srcB); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunInput()
        {
            int addr = IntCode[Offset]; 
            Offset += 1; 

            int input; 
            if (!DequeueInput(out input)) {
                Debug.Fail( "Program was not provided needed input." ); 
            }

            IntCode[addr] = input; 
        }

        //----------------------------------------------------------------------------------------------
        private void RunOutput()
        { 
            int addr = IntCode[Offset]; 
            Offset += 1; 

            int output = ReadParam(addr); 
            EnqueueOutput( output );             
        }

        //----------------------------------------------------------------------------------------------
        private void RunJumpTrue()
        {
            int check = ReadParam();
            int offset = ReadParam(); 
            if (check != 0) {
                Offset = offset;
            }
        }

        //----------------------------------------------------------------------------------------------
        private void RunJumpFalse()
        {
            int check = ReadParam();
            int offset = ReadParam(); 
            if (check == 0) {
                Offset = offset;
            }
        }

        //----------------------------------------------------------------------------------------------
        private void RunLess()
        {
            int a = ReadParam(); 
            int b = ReadParam(); 

            int result = (a < b) ? 1 : 0; 
            WriteParam(result);
        }

        //----------------------------------------------------------------------------------------------
        private void RunEquals()
        { 
            int a = ReadParam(); 
            int b = ReadParam(); 

            int result = (a == b) ? 1 : 0; 
            WriteParam(result);
        }


        //----------------------------------------------------------------------------------------------
        private void Halt()
        {
            Offset = IntCode.Length; 
        }

        //----------------------------------------------------------------------------------------------
        public void RunNextOp()
        {
            int opCode = ReadNextOp(); 
            switch (opCode) {
                case OP_ADD: RunAdd(); break; 
                case OP_MULT: RunMultiply(); break; 
                case OP_INPUT: RunInput(); break;
                case OP_OUTPUT: RunOutput(); break; 
                case OP_JUMP_TRUE: RunJumpTrue(); break; 
                case OP_JUMP_FALSE: RunJumpFalse(); break; 
                case OP_LESS: RunLess(); break;
                case OP_EQUALS: RunEquals(); break; 
                case OP_HALT: Halt(); break; 
                default: break; // nop
            }
        }

        //----------------------------------------------------------------------------------------------
        public int GetParamCount(int op) => op switch
        {
            OP_ADD => 3,
            OP_MULT => 3, 
            OP_INPUT => 1, 
            OP_OUTPUT => 1, 
            OP_JUMP_TRUE => 2,
            OP_JUMP_FALSE => 2,
            OP_LESS => 3,
            OP_EQUALS => 3,
            OP_HALT => -1, 
            _ => 0
        };

        //----------------------------------------------------------------------------------------------
        public void Run()
        {
            while (!IsHalted()) {
                RunNextOp(); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public void RunUntil( Func<bool> predicate )
        {
            while (!predicate() && !IsHalted()) {
                RunNextOp(); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public bool IsHalted()
        {
            return (Offset >= IntCode.Length); 
        }

        //----------------------------------------------------------------------------------------------
        public void Set(int idx, int val)
        {
            IntCode[idx] = val; 
        }

        //----------------------------------------------------------------------------------------------
        public int Get(int idx)
        {
            return IntCode[idx]; 
        }

        //----------------------------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); 
            int offset = 0; 

            while (offset < IntCode.Length) {
                if (offset != 0) {
                    sb.Append('\n'); 
                }

                int op = IntCode[offset]; 
                ++offset; 

                int paramCount = 0; 
                if (op == OP_HALT) {
                    paramCount = IntCode.Length - offset;
                    sb.Append("[red]"); 
                    sb.Append(op.ToString()); 
                    sb.Append("\n[+black]"); 
                } else {
                    paramCount = GetParamCount(op); 
                    sb.Append("[+green]"); 
                    sb.Append(op.ToString()); 
                    sb.Append(" [cyan]"); 
                }

                for (int i = 0; i < paramCount; ++i) {
                    sb.Append(IntCode[offset + i]); 
                    sb.Append(' '); 
                }

                offset += paramCount; 
            }

            return sb.ToString(); 
        }
    }
}
