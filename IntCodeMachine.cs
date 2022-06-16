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
        private const Int64 OP_ADD = 1; 
        private const Int64 OP_MULT = 2; 
        private const Int64 OP_INPUT = 3;
        private const Int64 OP_OUTPUT = 4;
        private const Int64 OP_JUMP_TRUE = 5; 
        private const Int64 OP_JUMP_FALSE = 6;
        private const Int64 OP_LESS = 7; 
        private const Int64 OP_EQUALS = 8;
        private const Int64 OP_ADJUST_RELATIVE_HEAD = 9;

        // special 
        private const Int64 OP_FIRST = OP_ADD; 
        private const Int64 OP_LAST = OP_ADJUST_RELATIVE_HEAD; 
        private const Int64 OP_HALT = 99; 

        public delegate Int64 ReadInputCB(); 
        public delegate void WriteOutputCB(Int64 val); 


        //----------------------------------------------------------------------------------------------
        public Int64[] ResetIntCode = new Int64[0]; 
        public Int64[] IntCode = new Int64[0]; 
        public Int64[] Memory = new Int64[0]; // any memory stored past the end of IntCode will be stored here.  Will default to zero if read without being written

        public Int64 Offset = 0; 
        public Int64 ParamOptions = 0; 
        public Int64 RelativeOffset = 0; 

        public Int64 LowestMemoryAddress = Int64.MaxValue; 
        public Int64 HighestMemoryAddress = 0; 

        public Queue<Int64> Inputs = new Queue<Int64>(); 
        public Queue<Int64> Outputs = new Queue<Int64>(); 
        public IntCodeMachine? InputProgram = null; 
        public IntCodeMachine? OutputProgram = null; 

        public ReadInputCB? OnReadInput;
        public WriteOutputCB? OnWriteOutput;

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine()
        {
        }

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine(Int64[] intCode)
        {
            Setup(intCode); 
        }

        public IntCodeMachine(int[] intCode)
        {
            Setup(intCode); 
        }

        public IntCodeMachine(IntCodeMachine toCopy)
        {
            Setup(toCopy.IntCode); 
        }

        //----------------------------------------------------------------------------------------------
        public IntCodeMachine(string src)
        {
            SetupFromSource(src); 
        }

        //----------------------------------------------------------------------------------------------
        public void Setup(Int64[] intCode)
        {
            IntCode = new Int64[intCode.Length]; 
            ResetIntCode = new Int64[intCode.Length]; 
            Array.Copy(intCode, 0, ResetIntCode, 0, intCode.Length); 

            // will setup the int code to the reset int code
            Reset(); 
        }

        //----------------------------------------------------------------------------------------------
        public void Setup(int[] intCode)
        {
            Int64[] longCode = new long[intCode.Length]; 
            for (int i = 0; i < intCode.Length; ++i) {
                longCode[i] = intCode[i]; 
            }

            Setup(longCode); 
        }

        //----------------------------------------------------------------------------------------------
        public void SetupFromSource( string src )
        {
            Setup( src.Split(',').Select(Int64.Parse).ToArray() ); 
        }

        //----------------------------------------------------------------------------------------------
        public void SetupFromFile( string filename )
        {
            string src = string.Join(null, Util.ReadFileToLines(filename)); 
            SetupFromSource( src ); 
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

            Inputs.Clear(); 
            Outputs.Clear();
            Memory = new Int64[0]; 

            Offset = 0; 
            RelativeOffset = 0; 
            LowestMemoryAddress = Int64.MaxValue; 
            HighestMemoryAddress = 0; 
        }

        //----------------------------------------------------------------------------------------------
        public void EnqueueInput( Int64 val )
        { 
            Inputs.Enqueue(val); 
        }

        //----------------------------------------------------------------------------------------------
        public bool DequeueInput( out Int64 val )
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
                    Int64 output;
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
        public void SetInputs( params Int64[] inputs )
        {
            Inputs.Clear(); 
            foreach (Int64 val in inputs) {
                EnqueueInput(val);
            }
        }

        //----------------------------------------------------------------------------------------------
        public void EnqueueOutput( Int64 val )
        {
            Outputs.Enqueue( val ); 
            if (OnWriteOutput != null) {
                OnWriteOutput( val ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public bool DequeueOutput( out Int64 val )
        {
            return Outputs.TryDequeue( out val ); 
        }

        //----------------------------------------------------------------------------------------------
        public bool HasOutput()
        {
            return Outputs.Count > 0; 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 ReadNextOp()
        {
            Int64 opCode = IntCode[Offset]; 
            ++Offset; 
            
            Int64 op = opCode % 100; 
            ParamOptions = opCode / 100; // todo: convert this to binary so I'm not dividing as much reading params

            return op; 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 ReadMemory(Int64 addr)
        {
            Debug.Assert(addr >= 0); // if negative, ignore it
            if (addr < IntCode.Length) {
                return IntCode[addr]; 
            } else {
                Int64 memAddr = addr - IntCode.Length; 
                return (memAddr < Memory.Length) ? Memory[memAddr] : 0; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private void GrowMemoryToFit(Int64 memAddr)
        {
            Int64 memSize = Math.Max( Memory.Length, 4096 ); 
            while (memAddr >= memSize) {
                memSize *= 2; 
            }

            Int64[] newMemory = new long[memSize]; 
            Memory.CopyTo(newMemory, 0); 
            Memory = newMemory; 
        }

        //----------------------------------------------------------------------------------------------
        private void WriteMemory(Int64 addr, Int64 val)
        {
            Debug.Assert(addr >= 0); 
            if (addr < IntCode.Length) {
                IntCode[addr] = val; 
            } else {
                Int64 memAddr = addr - IntCode.Length; 
                if (memAddr > Memory.Length) {
                    GrowMemoryToFit(memAddr); 
                }

                LowestMemoryAddress = Math.Min(memAddr, LowestMemoryAddress); 
                HighestMemoryAddress = Math.Max(memAddr, HighestMemoryAddress); 
                Memory[memAddr] = val; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private Int64 ReadParam(Int64 addr)
        {
            Int64 addrMode = ParamOptions % 10; 
            ParamOptions /= 10; 

            return addrMode switch {
                0 => ReadMemory(addr), 
                1 => addr, 
                2 => ReadMemory(RelativeOffset + addr),
                _ => addr
            }; 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 ReadParam()
        {
            Int64 addr = IntCode[Offset]; 
            ++Offset; 
            return ReadParam(addr); 
        }

        //----------------------------------------------------------------------------------------------
        private void WriteParam(Int64 val)
        {
            // todo: so far it has said write addresses are always direct, but man if I'm not skeptical
            Int64 addr = IntCode[Offset]; 
            ++Offset; 

            WriteMemory(addr, val); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunAdd()
        {
            Int64 srcA = IntCode[Offset + 0]; 
            Int64 srcB = IntCode[Offset + 1]; 
            Offset += 2; 

            WriteParam(ReadParam(srcA) + ReadParam(srcB)); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunMultiply()
        {
            Int64 srcA = IntCode[Offset + 0]; 
            Int64 srcB = IntCode[Offset + 1]; 
            Offset += 2; 

            WriteParam(ReadParam(srcA) * ReadParam(srcB)); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunInput()
        {
            Int64 addr = IntCode[Offset]; 
            Offset += 1; 

            Int64 input; 
            if (!DequeueInput(out input)) {
                Debug.Fail( "Program was not provided needed input." ); 
            }

            WriteMemory(addr, input); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunOutput()
        { 
            Int64 addr = IntCode[Offset]; 
            Offset += 1; 

            Int64 output = ReadParam(addr); 
            EnqueueOutput( output );             
        }

        //----------------------------------------------------------------------------------------------
        private void RunJumpTrue()
        {
            Int64 check = ReadParam();
            Int64 offset = ReadParam(); 
            if (check != 0) {
                Offset = offset;
            }
        }

        //----------------------------------------------------------------------------------------------
        private void RunJumpFalse()
        {
            Int64 check = ReadParam();
            Int64 offset = ReadParam(); 
            if (check == 0) {
                Offset = offset;
            }
        }

        //----------------------------------------------------------------------------------------------
        private void RunLess()
        {
            Int64 a = ReadParam(); 
            Int64 b = ReadParam(); 

            Int64 result = (a < b) ? 1 : 0; 
            WriteParam(result);
        }

        //----------------------------------------------------------------------------------------------
        private void RunEquals()
        { 
            Int64 a = ReadParam(); 
            Int64 b = ReadParam(); 

            Int64 result = (a == b) ? 1 : 0; 
            WriteParam(result);
        }

        //----------------------------------------------------------------------------------------------
        private void RunAdjustRelativeHead()
        {
            Int64 value = ReadParam(); 
            RelativeOffset += value; 
        }

        //----------------------------------------------------------------------------------------------
        private void Halt()
        {
            Offset = IntCode.Length; 
        }

        //----------------------------------------------------------------------------------------------
        public void RunNextOp()
        {
            Int64 opCode = ReadNextOp(); 
            switch (opCode) {
                case OP_ADD: RunAdd(); break; 
                case OP_MULT: RunMultiply(); break; 
                case OP_INPUT: RunInput(); break;
                case OP_OUTPUT: RunOutput(); break; 
                case OP_JUMP_TRUE: RunJumpTrue(); break; 
                case OP_JUMP_FALSE: RunJumpFalse(); break; 
                case OP_LESS: RunLess(); break;
                case OP_EQUALS: RunEquals(); break; 
                case OP_ADJUST_RELATIVE_HEAD: RunAdjustRelativeHead(); break;
                case OP_HALT: Halt(); break; 
                default: break; // nop
            }
        }

        //----------------------------------------------------------------------------------------------
        public bool IsOp(Int64 op) => ((op >= OP_FIRST) && (op <= OP_LAST)) || (op == OP_HALT); 

        //----------------------------------------------------------------------------------------------
        public Int64 GetParamCount(Int64 op) => op switch
        {
            OP_ADD => 3,
            OP_MULT => 3, 
            OP_INPUT => 1, 
            OP_OUTPUT => 1, 
            OP_JUMP_TRUE => 2,
            OP_JUMP_FALSE => 2,
            OP_LESS => 3,
            OP_EQUALS => 3,
            OP_ADJUST_RELATIVE_HEAD => 1,
            OP_HALT => -1, 
            _ => 0
        };

        //----------------------------------------------------------------------------------------------
        public string OpToString(Int64 op) => op switch 
        {
            OP_ADD => "add",
            OP_MULT => "mul", 
            OP_INPUT => "in ", 
            OP_OUTPUT => "out", 
            OP_JUMP_TRUE => "jpt",
            OP_JUMP_FALSE => "jpf",
            OP_LESS => "ls ",
            OP_EQUALS => "eql",
            OP_ADJUST_RELATIVE_HEAD => "rel",
            OP_HALT => "end", 
            _ => op.ToString()
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
        public void Set(Int64 idx, Int64 val)
        {
            IntCode[idx] = val; 
        }

        //----------------------------------------------------------------------------------------------
        public Int64 Get(Int64 idx)
        {
            return IntCode[idx]; 
        }

        private string ParamOptionToString(Int64 option) => option switch
        {
            0 => "@",
            1 => "",
            2 => "R",
            _ => "???"
        };

        //----------------------------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); 
            Int64 offset = 0; 

            int lineNumber = 1; 
            while (offset < IntCode.Length) {
                if (offset != 0) {
                    sb.Append('\n'); 
                }

                // add a line number
                string lineNumStr = lineNumber.ToString().PadLeft(6); 
                ++lineNumber; 

                string offsetStr = offset.ToString().PadRight(6); 
                Int64 op = IntCode[offset] % 100; 
                Int64 paramOptions = (IntCode[offset]) / 100; 
                ++offset; 

                sb.Append("[+cyan]" + lineNumStr + ". [+black]@" + offsetStr); 

                Int64 paramCount = 0; 
                if (op == OP_HALT) {
                    sb.Append("[red]"); 
                    sb.Append(OpToString(op)); 
                } else if (IsOp(op)) {
                    paramCount = GetParamCount(op); 
                    sb.Append("[+green]"); 
                    sb.Append(OpToString(op)); 
                    sb.Append(" [cyan]"); 
                } else {
                    sb.Append("[+black]"); 
                    sb.Append(op.ToString()); 

                    // keep appending bytes until we find the next op code
                    op = IntCode[offset] % 100; 
                    while (!IsOp(op)) {
                        ++offset;
                        sb.Append(' '); 
                        sb.Append(op.ToString()); 
                        op = IntCode[offset] % 100; 
                    }
                }

                for (Int64 i = 0; i < paramCount; ++i) {
                    Int64 paramOption = paramOptions % 10; 
                    paramOptions /= 10; 

                    sb.Append(ParamOptionToString(paramOption));
                    sb.Append(IntCode[offset + i]); 
                    sb.Append(' '); 
                }

                offset += paramCount; 
            }

            /*
            // append memory built into int-code
            sb.Append("\n\n[+white] : built-in memory @[+green]" + offset + "[+white] <[+black] "); 
            while (offset < IntCode.Length) {
                sb.Append(IntCode[offset]); 
                sb.Append(' '); 
                ++offset; 
            }
            sb.Append("[+white]>"); 
            */

            // append memory created during runtime
            if (LowestMemoryAddress <= HighestMemoryAddress) {
                sb.Append("\n\n[+white] : allocated memory @[+green]" + (IntCode.Length + LowestMemoryAddress) + "[+white] <[+black] "); 
                for (Int64 i = LowestMemoryAddress; i < HighestMemoryAddress + 1; ++i) {
                    sb.Append(Memory[i]); 
                    sb.Append(' '); 
                }
                sb.Append("[+white]>"); 
            }

            return sb.ToString(); 
        }
    }
}
