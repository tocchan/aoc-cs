using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day08 : Day
    {
        private string InputFile = "2021/inputs/08.txt"; 
     
        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        public int ProcessLineA( string line )
        {
            (string digitString, string displayString) = line.Split('|', 2); 
            string[] display = displayString.Split(' ', StringSplitOptions.RemoveEmptyEntries); 

            // part A is simple, just count things that are 1, 4, 7 or 8
            int count = 0; 
            foreach (string d in display)
            {
                int len = d.Length; 
                if ((len == 2) || (len == 3) || (len == 4) || (len == 7))
                {
                    ++count;
                }
            }

            return count; 
        }

        public string FindSequenceWithLength( string[] digits, int len )
        {
            return digits.Where( s => s.Length == len ).First(); 
        }

        public int GetDigitValue( string digit, string[] sequencess )
        {
            for (int i = 0; i < sequencess.Length; ++i)
            {
                if (sequencess[i] == digit)
                {
                    return i; 
                }
            }

            return 0; 
        }

        string Subtract( string a, string b )
        {
            string ret = ""; 
            foreach (char c in a)
            {
                if (!b.Contains(c)) 
                { 
                    ret += c; 
                }
            }

            return ret; 
        }

        string Intersect( string a, string b )
        {
            string ret = "";
            foreach (char c in a)
            {
                if (b.Contains(c))
                {
                    ret += c; 
                }
            }

            return ret; 
        }

        string[] GetIntersections( string[] values, string against, int len )
        {
            List<string> ret = new List<string>(); 
            foreach (string value in values)
            {
                if (value == against)
                {
                    continue; 
                }

                string intersection = Intersect( value, against ); 
                if (intersection.Length == len)
                {
                    ret.Add(value);
                }
            }

            return ret.ToArray(); 
        }

        string[] GetIntersectionsOfLength( string[] values, string against, int intLen, int len )
        {
            List<string> ret = new List<string>(); 
            foreach (string value in values)
            {
                if (value.Length != len)
                {
                    continue; 
                }

                string intersection = Intersect( value, against ); 
                if (intersection.Length == intLen)
                {
                    ret.Add(value);
                }
            }

            return ret.ToArray(); 
        }

        public int ProcessLineB( string line )
        {
            string[] parts = line.Split('|', 2);
            string[] digits = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries); 
            string[] display = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries); 

            for (int i = 0; i < digits.Length; ++i)
            {
                digits[i] = String.Concat(digits[i].OrderBy(c => c)); 
            }

            for (int i = 0; i < display.Length; ++i)
            {
                display[i] = String.Concat(display[i].OrderBy(c => c)); 
            }

            string[] sequences = new string[10]; 
            for (int i = 0; i < 10; ++i)
            {
                sequences[i] = ""; 
            }

            sequences[1] = FindSequenceWithLength( digits, 2 ); 
            sequences[7] = FindSequenceWithLength( digits, 3 ); 
            sequences[4] = FindSequenceWithLength( digits, 4 ); 
            sequences[8] = FindSequenceWithLength( digits, 7 ); 

            /*
               0 0
              1   2
               3 3
              4   5
               6 6
            */
            char[] mapping = new char[7]; 
            mapping[0] = Subtract( sequences[7], sequences[1] )[0];  // 7 has one more value than 1, so will tell us

            // 3 is the only sequence that intersects with 1 fully is is length 5
            {
                sequences[3] = GetIntersectionsOfLength( digits, sequences[1], 2, 5 )[0];  
            }

            // 2 would be the only one that intersects with 1 once, 5 & 6 intersect 1 with the same value
            {
                string[] twoFiveOrSix = GetIntersections( digits, sequences[1], 1 ); 
                char[] intersections = new char[3]; 
                intersections[0] = Intersect( twoFiveOrSix[0], sequences[1] )[0]; 
                intersections[1] = Intersect( twoFiveOrSix[1], sequences[1] )[0]; 
                intersections[2] = Intersect( twoFiveOrSix[2], sequences[1] )[0]; 

                // the first one is the pair
                if ((intersections[0] == intersections[1]) || (intersections[0] == intersections[2]))
                {
                    mapping[5] = intersections[0]; 
                    int twoIdx = (intersections[0] == intersections[1]) ? 2 : 1; 
                    int otherIdx = (twoIdx == 2 ? 1 : 2); 
                    mapping[2] = intersections[twoIdx]; 

                    sequences[2] = (intersections[0] == intersections[1]) ? twoFiveOrSix[2] : twoFiveOrSix[1]; 
                    if (twoFiveOrSix[0].Length == 6)
                    {
                        sequences[6] = twoFiveOrSix[0]; 
                        sequences[5] = twoFiveOrSix[otherIdx]; 
                    }
                    else
                    {
                        sequences[6] = twoFiveOrSix[otherIdx]; 
                        sequences[5] = twoFiveOrSix[0]; 
                    }
                }
                else // first one is unique
                {
                    sequences[2] = twoFiveOrSix[0]; 
                    if (twoFiveOrSix[1].Length == 6)
                    {
                        sequences[6] = twoFiveOrSix[1]; 
                        sequences[5] = twoFiveOrSix[2]; 
                    }
                    else
                    {
                        sequences[6] = twoFiveOrSix[2]; 
                        sequences[5] = twoFiveOrSix[1]; 
                    }

                    mapping[2] = intersections[0]; 
                    mapping[5] = intersections[1]; 
                }
            }

            // 8 & 9 fully intersect with 3, 8 is known, so this will give us 9
            {
                string[] eightAndNine = GetIntersections( digits, sequences[3], 5 ); 
                sequences[9] = (eightAndNine[0].Length == 7) ? eightAndNine[1] : eightAndNine[0]; 
            }

            // 0 is all that remains
            {
                List<string> allDigits = digits.ToList(); 
                foreach (string s in sequences)
                {
                    allDigits.Remove(s); 
                }

                sequences[0] = allDigits[0]; 
            }

            int count = 0; 
            int power = 1; 
            for (int i = display.Length - 1; i >= 0; --i)
            {
                int digitVal = GetDigitValue(display[i], sequences); 
                count += digitVal * power; 
                power *= 10; 
            }

             return count; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            
            int count = 0; 
            foreach (string line in lineInput)
            {
                count += ProcessLineA( line );
            }


            return count.ToString(); 
        }


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 
            
            int count = 0; 
            foreach (string line in lineInput)
            {
                count += ProcessLineB( line );
            }


            return count.ToString(); 
        }
    }
}

