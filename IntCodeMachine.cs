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
        public int[] ResetIntCode = new int[0]; 
        public int[] IntCode = new int[0]; 
        public int Offset = 0; 

        private const int OP_ADD = 1; 
        private const int OP_MULT = 2; 
        private const int OP_HALT = 99; 

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
        public void Setup(int[] intCode)
        {
            IntCode = new int[intCode.Length]; 
            ResetIntCode = new int[intCode.Length]; 

            Array.Copy(intCode, 0, IntCode, 0, intCode.Length); 
            Array.Copy(intCode, 0, ResetIntCode, 0, intCode.Length); 

            Offset = 0; 
        }

        //----------------------------------------------------------------------------------------------
        public void Reset()
        {
            Array.Copy(ResetIntCode, 0, IntCode, 0, ResetIntCode.Length); 
            Offset = 0; 
        }

        //----------------------------------------------------------------------------------------------
        private void RunAdd()
        {
            int srcA = IntCode[Offset + 0]; 
            int srcB = IntCode[Offset + 1]; 
            int dst = IntCode[Offset + 2]; 
            Offset += 3; 

            IntCode[dst] = IntCode[srcA] + IntCode[srcB]; 
        }

        //----------------------------------------------------------------------------------------------
        private void RunMultiply()
        {
            int srcA = IntCode[Offset + 0]; 
            int srcB = IntCode[Offset + 1]; 
            int dst = IntCode[Offset + 2]; 
            Offset += 3; 

            IntCode[dst] = IntCode[srcA] * IntCode[srcB]; 
        }

        //----------------------------------------------------------------------------------------------
        private void Halt()
        {
            Offset = IntCode.Length; 
        }

        //----------------------------------------------------------------------------------------------
        public void RunNextOp()
        {
            int opCode = IntCode[Offset]; 
            ++Offset; 

            switch (opCode) {
                case OP_ADD: RunAdd(); break; 
                case OP_MULT: RunMultiply(); break; 
                case OP_HALT: Halt(); break; 
                default: break; // nop
            }
        }

        //----------------------------------------------------------------------------------------------
        public int GetParamCount(int op) => op switch
        {
            OP_ADD => 3,
            OP_MULT => 3, 
            OP_HALT => -1, 
            _ => 0
        };

        //----------------------------------------------------------------------------------------------
        public void Run()
        {
            while (Offset < IntCode.Length) {
                RunNextOp(); 
            }
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
