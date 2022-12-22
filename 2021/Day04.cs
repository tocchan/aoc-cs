using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day04 : Day
    {
        // private string DebugInputFile = "2021/inputs/04.txt"; 
        private string InputFile = "2021/inputs/04.txt"; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        public void ParseBoards( List<int[,]> boards, List<string> input )
        {
            int count = input.Count; 
            int offset = 2; 
            while ((offset + 5) <= count)
            {
                int[,] board = new int[5,5]; 

                for (int y = 0; y < 5; ++y) {
                    string[] rowNums = input[offset + y].Split(' ', StringSplitOptions.RemoveEmptyEntries); 
                    List<int> row = rowNums.Select(int.Parse).ToList(); 
                    for (int x = 0; x < 5; ++x)
                    {
                        board[x, y] = row[x]; 
                    }
                }
                
                boards.Add(board); 

                offset += 6; // skip whitespace line
            }
        }

        //----------------------------------------------------------------------------------------------
        private bool MarkBoard( out int score, int[,] board, bool[,] marks, int call )
        {
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    if (board[x, y] == call)
                    {
                        marks[x, y] = true;     
                        return CheckForWin( out score, board, marks, x, y ); 
                    }
                }
            }
           
            score = 0; 
            return false; 
        }

        //----------------------------------------------------------------------------------------------
        private bool CheckForWin( out int score, int[,] board, bool[,] marks, int x, int y )
        {
            score = 0; 
            if (!CheckRow( board, marks, y ) && !CheckColumn( board, marks, x ))
            {
                return false; 
            }

            int sum = 0; 
            for (int iy = 0; iy < 5; ++iy)
            {
                for (int ix = 0; ix < 5; ++ix)
                {
                    if (!marks[ix, iy])
                    {
                        sum += board[ix, iy]; 
                    }
                }
            }
            
            score = sum; 
            return true; 
        }

        //----------------------------------------------------------------------------------------------
        private bool CheckColumn(int[,] board, bool[,] marks, int x)
        {
            for (int y = 0; y < 5; ++y)
            {
                if (marks[x, y] == false)
                {
                    return false; 
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        private bool CheckRow(int[,] board, bool[,] marks, int y)
        {
            for (int x = 0; x < 5; ++x)
            {
                if (marks[x, y] == false)
                {
                    return false; 
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            
            List<int> callNumbers = lines[0].Split(',').Select(int.Parse).ToList(); 

            List<int[,]> boards = new List<int[,]>();
            ParseBoards( boards, lines ); 

            List<bool[,]> marks = new List<bool[,]>(); 
            foreach (var board in boards)
            {
                bool[,] mark = new bool[5,5]; 
                marks.Add(mark); 
            }; 


            foreach (int call in callNumbers)
            {
                for (int i = 0; i < boards.Count; ++i)
                {
                    int score; 
                    if (MarkBoard( out score, boards[i], marks[i], call ))
                    {
                        return (score * call).ToString(); 
                    }
                    
                }
            }

            return ""; 
        }

        


        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            
            List<int> callNumbers = lines[0].Split(',').Select(int.Parse).ToList(); 

            List<int[,]> boards = new List<int[,]>();
            ParseBoards( boards, lines ); 

            List<bool[,]> marks = new List<bool[,]>(); 
            foreach (var board in boards)
            {
                bool[,] mark = new bool[5,5]; 
                marks.Add(mark); 
            }; 


            foreach (int call in callNumbers)
            {
                for (int i = boards.Count - 1; i >= 0; --i)
                {
                    int score; 
                    if (MarkBoard( out score, boards[i], marks[i], call ))
                    {
                        if (boards.Count == 1)
                        {
                            return (score * call).ToString(); 
                        } 
                        else 
                        { 
                            boards.RemoveAt(i); 
                            marks.RemoveAt(i); 
                        }
                    }
                    
                }
            }

            return ""; 
        }
    }
}
