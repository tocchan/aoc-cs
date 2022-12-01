using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day11 : Day
 {
        private string InputFile = "2019/inputs/11.txt"; 
        private IntCodeMachine AIProgram = new IntCodeMachine(); 
        private IntHeatMap2D Hull = new IntHeatMap2D(); 


        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            AIProgram.SetupFromFile(InputFile); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            Dictionary<ivec2, bool> canvas = new Dictionary<ivec2, bool>(); 
            canvas[ivec2.ZERO] = true; 

            ivec2 min = ivec2.ZERO; 
            ivec2 max = ivec2.ZERO; 

            ivec2 cursor = ivec2.ZERO; 
            ivec2 direction = ivec2.UP; 

            AIProgram.OnReadInput = () => { 
                bool val; 
                if (canvas.TryGetValue(cursor, out val)) {
                    return val ? 1 : 0; 
                } else {
                    return 0; 
                }
            }; 

            bool isPainting = true; 
            AIProgram.OnWriteOutput = (Int64 val) => {
                if (isPainting) {
                    canvas[cursor] = (val == 0) ? false : true; 
                } else {

                    if (val == 0) {
                        direction.RotateLeft(); 
                    } else {
                        direction.RotateRight(); 
                    }
                    min = ivec2.Min(min, cursor); 
                    max = ivec2.Max(max, cursor); 

                    cursor += direction; 
                }
                
                isPainting = !isPainting; 
            };

            AIProgram.Run(); 

            // paint it out
            Hull.Init( max - min + ivec2.ONE, 0 ); 
            foreach ((ivec2 pos, bool val) in canvas) {
                if (val) {
                    Hull.Set(pos - min, 1); 
                }
            }
            
            return canvas.Count.ToString(); 
        }

        
        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            char white = '█'; 
            char black = ' '; 

            int height = Hull.GetHeight(); 
            List<string> strings = new List<string>(); 
            for (int y = 0; y < height; ++y) {
                StringBuilder row = new StringBuilder(); 
                for (int x = 0; x < Hull.GetWidth(); ++x) {
                    int val = Hull.Get(x, y); 
                    row.Append( (val > 0) ? white : black ); 
                }

                strings.Add(row.ToString()); 
            }

            string hullImage = "\n" + string.Join('\n', strings);                         
            return hullImage; 
        }
    }
}
