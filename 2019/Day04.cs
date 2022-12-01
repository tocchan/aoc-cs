using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day04 : Day
    {
        private string InputFile = "2019/inputs/04.txt"; 

        //----------------------------------------------------------------------------------------------
        // values
        int MinValue = 0; 
        int MaxValue = 0;

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            (MinValue, MaxValue) = lines[0].Split('-').Select(int.Parse).ToArray();; 
        }

        private bool IsIncreasing( string password )
        {
            for (int i = 1; i < password.Length; ++i) {
                if (password[i] < password[i - 1]) {
                    return false;
                }
            }

            return true; 
        }

        private bool HasDouble( string password )
        {
            for (int i = 1; i < password.Length; ++i) {
                if (password[i] == password[i - 1]) {
                    return true; 
                }
            }

            return false; 
        }

        private bool HasOnlyDouble( string password )
        {
            for (int i = 1; i < password.Length; ++i) {
                if ( (password[i] == password[i - 1])
                    && ((i == 1) || (password[i - 2] != password[i]))
                    && ((i == password.Length - 1) || (password[i] != password[i + 1]))) {

                    return true; 
                }
            }

            return false; 
        }

        private bool IsValidPassword( int passwd )
        {
            string password = passwd.ToString();
            if (password.Length != 6) {
                return false; 
            }

            // increases throughout
            if (!IsIncreasing(password)) {
                return false;
            }

            // has double
            if (!HasDouble( password )) {
                return false; 
            }

            return true; 
        }

        private bool IsValidPassword2( int passwd )
        {
            string password = passwd.ToString();
            if (password.Length != 6) {
                return false; 
            }

            // increases throughout
            if (!IsIncreasing(password)) {
                return false;
            }

            // has double
            if (!HasOnlyDouble( password )) {
                return false; 
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int numPasswords = 0; 
            for (int i = MinValue; i <= MaxValue; ++i) {
                if (IsValidPassword(i)) {
                    ++numPasswords; 
                }
            }

            return numPasswords.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int numPasswords = 0; 
            for (int i = MinValue; i <= MaxValue; ++i) {
                if (IsValidPassword2(i)) {
                    ++numPasswords; 
                }
            }

            return numPasswords.ToString(); 
        }
    }
}
