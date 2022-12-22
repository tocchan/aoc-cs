using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day10 : Day
    {
        private string InputFile = "2021/inputs/10.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public enum LineStatus
        {
            OK, 
            CORRUPTED, 
            INCOMPLETE, 
        }

        //----------------------------------------------------------------------------------------------
        private int GetCharScore(char c)
        {
            return c switch
            {
                ')' => 3, 
                ']' => 57, 
                '}' => 1197,
                '>' => 25137,
                _ => 0
            }; 
        }

        //----------------------------------------------------------------------------------------------
        private char GetClosingChar(char c) => c switch
        {
            '(' => ')',
            '[' => ']', 
            '<' => '>', 
            '{' => '}',
            _ => ' '
        };

        private bool IsOpenChar(char c) => GetClosingChar(c) != ' '; 


        //----------------------------------------------------------------------------------------------
        private LineStatus CheckLine( string line, out char badChar, out string closer )
        {
            Stack<char> expectedClose = new Stack<char>(); 
            badChar = ' '; 
            closer = ""; 

            for (int i = 0; i < line.Length; ++i)
            {
                char c = line[i]; 
                char close = GetClosingChar(c); 
                if (close != ' ')
                {
                    // was an open
                    expectedClose.Push(close); 
                }
                else  if (expectedClose.Count == 0) 
                { 
                    return LineStatus.INCOMPLETE; 
                }
                else if (expectedClose.Peek() == c)
                {
                    expectedClose.Pop(); 
                }
                else
                {
                    badChar = c; 
                    return LineStatus.CORRUPTED; 
                }
            }


            if (expectedClose.Count > 0)
            {
                closer = new string( expectedClose.ToArray() ); 
                return LineStatus.INCOMPLETE; 
            }
            else 
            { 
                return LineStatus.OK; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 

            int score = 0; 
            foreach (string line in lineInput)
            {
                char badChar = (char) 0; 
                string closer; 
                LineStatus status = CheckLine( line, out badChar, out closer );  
                if (status == LineStatus.CORRUPTED)
                {
                    score += GetCharScore(badChar); 
                }
            }

            return score.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 ScoreAutoComplete( string line )
        {
            Int64 score = 0; 
            foreach (char c in line)
            {
                score *= 5; 
                score += c switch
                {
                    ')' => 1, 
                    ']' => 2, 
                    '}' => 3, 
                    '>' => 4, 
                    _ => 0
                };
            }

            return score; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lineInput = Util.ReadFileToLines(InputFile); 

            List<Int64> autoScores = new List<Int64>(); 
            foreach (string line in lineInput)
            {
                char badChar = (char) 0; 
                string closer; 
                LineStatus status = CheckLine( line, out badChar, out closer );  
                if (status == LineStatus.INCOMPLETE)
                {
                    autoScores.Add( ScoreAutoComplete(closer) ); 
                }
            }

            autoScores.Sort(); 
            int idx = autoScores.Count / 2; 
            return autoScores[idx].ToString(); 
        }
    }
}

