using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day13: Day
 {
        private string InputFile = "2019/inputs/13.txt"; 
        private IntCanvas Canvas = new IntCanvas(); 
        private IntCodeMachine Program = new IntCodeMachine(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Program.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            while (Program.RunUntilHasOutput(3)) {
                int x = (int) Program.TryDequeueOutput(0); 
                int y = (int) Program.TryDequeueOutput(1); 
                int id = (int) Program.TryDequeueOutput(2); 

                Canvas.SetValue(x, y, id); 
            }

            Util.WriteLine(Canvas.ToString(" █░-o")); 
            return Canvas.Count(2).ToString();
        }

       
        
        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            Program.Reset(); 
            Program.Set(0, 2); 
            Canvas = new IntCanvas(); 

            Int64 ballX = -1; 
            Int64 numBlocks = 0; 
            Int64 paddleX = -1; 
            Int64 score = 0; 

            Program.OnReadInput = Int64() => {
                int dir = Math.Sign(ballX - paddleX); 
                return dir; 
            };
            
            while (Program.RunUntilHasOutput(3)) {
                int x = (int) Program.TryDequeueOutput(0); 
                int y = (int) Program.TryDequeueOutput(1); 
                int id = (int) Program.TryDequeueOutput(2); 

                if (id == 4) {
                    ballX = x; 
                } else if (id == 3) {
                    paddleX = x; 
                }

                if ((x == -1) && (y == 0)) {
                    score = id; 
                }

                int oldVal = Canvas.SetValue(x, y, id); 
                if ((id == 2) && (oldVal != 2)) {
                    ++numBlocks; 
                } else if ((id != 2) && (oldVal == 2)) {
                    --numBlocks; 
                }
            }

            return score.ToString(); 
        }
    }
}
