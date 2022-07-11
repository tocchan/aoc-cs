using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day16 : Day
 {
        private string InputFile = "inputs/16.txt"; 
        private int[] InputSignal = new int[0]; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            InputSignal = Util.ReadFileToLines(InputFile)[0].Select(int(char c) => (int)(c - '0')).ToArray(); 
        }

        //----------------------------------------------------------------------------------------------
        private string SignalToString( int[] signal )
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < signal.Length; ++i) {
                sb.Append((char)('0' + (char)signal[i])); 
            }
            return sb.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private int ComputeDigit( int digit, int[] signal )
        {
            int value = 0; 
            int offset = 1;
            int length = (digit + 1); 
            
            // any function I was coming up with just resulted in a bunch of mods/divides
            // and probalby a nested loop would end up being better

            int idx = 0; 
            while (idx < signal.Length) {

                // zeroes, don't care
                idx += (length - offset); 

                // ones
                offset = 0; 
                length = Math.Min(signal.Length - idx, length); 
                while (offset < length) {
                    value += signal[idx]; 
                    ++idx; 
                    ++offset; 
                }
                offset = 0; 

                // zeroes
                idx += length; 

                // negatives
                length = Math.Min(signal.Length - idx, length); 
                while (offset < length) {
                    value -= signal[idx]; 
                    ++offset; 
                    ++idx; 
                }
                offset = 0; 
            }

            return Math.Abs(value) % 10; 
        }

        //----------------------------------------------------------------------------------------------
        private void RunPhase(int[] dst, int[] src)
        {
            for (int i = 0; i < dst.Length; ++i) {
                dst[i] = ComputeDigit(i, src); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int[] signal = new int[InputSignal.Length]; 
            int[] back = new int[InputSignal.Length]; 
            InputSignal.CopyTo(signal, 0); 

            int numPhases = 100; 
            for (int i = 0; i < numPhases; ++i) {
                RunPhase(back, signal);
                int[] t = back; 
                back = signal;
                signal = t; 

                // Util.WriteLine($"Phase {i+1}: {SignalToString(signal)}"); 
            }

            return SignalToString(signal).Substring(0, 8); 
        }

        //----------------------------------------------------------------------------------------------
        private void RunPhase2(int[] dst, int[] src, int offset)
        {
            int curSum = 0; 
            for (int i = src.Length - 1; i >= offset; --i) {
                curSum = (curSum + src[i]) % 10; 
                dst[i] = curSum; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int msgOffset = int.Parse(SignalToString(InputSignal).Substring(0, 7)); 
    
            int numSets = 10000; 
            int[] signal = new int[numSets * InputSignal.Length]; 
            int[] back = new int[signal.Length]; 

            for (int i = 0; i < numSets; ++i) {
                InputSignal.CopyTo(signal, i * InputSignal.Length); 
            }

            int numPhases = 100; 
            for (int phase = 0; phase < numPhases; ++phase) {
                RunPhase2(back, signal, msgOffset); 

                int[] t = back; 
                back = signal; 
                signal = t; 
            }

            string result = ""; 
            for (int i = 0; i < 8; ++i) {
                result += signal[i + msgOffset].ToString(); 
            }

            return result; 
        }
    }
}
