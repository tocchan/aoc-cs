using AoC; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day08 : Day
 {
        private string InputFile = "2019/inputs/08.txt"; 

        //----------------------------------------------------------------------------------------------
        // values
        List<IntHeatMap2D> Layers = new List<IntHeatMap2D>(); 
        int LayerWidth = 25; 
        int LayerHeight = 6; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string password = Util.ReadFileToLines(InputFile)[0];

            int area = LayerWidth * LayerHeight; 
            int numAreas = password.Length / area; 
            int offset = 0; 
            for (int i = 0; i < numAreas; ++i) {
                IntHeatMap2D layer = new IntHeatMap2D(new ivec2(LayerWidth, LayerHeight)); 
                for (int pi = 0; pi < area; ++pi) {
                    layer[pi] = (int)(password[offset] - '0'); 
                    ++offset; 
                }

                Layers.Add(layer); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            int minZeroLayer = 0; 
            int minZeroes = Layers[0].Count(0); 

            for (int i = 1; i < Layers.Count; ++i) {
                int numZeroes = Layers[i].Count(0);
                if (numZeroes < minZeroes) {
                    minZeroes = numZeroes; 
                    minZeroLayer = i; 
                }
            }

            IntHeatMap2D layer = Layers[minZeroLayer]; 
            int checksum = layer.Count(1) * layer.Count(2); 

            return checksum.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private char GetColorChar( int c ) => c switch 
        {
            0 => ' ', 
            1 => '█', 
            2 => ' ', 
            _ => ' '
        };

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            IntHeatMap2D image = new IntHeatMap2D(new ivec2(LayerWidth, LayerHeight)); 
            int area = LayerWidth * LayerHeight; 

            for (int i = 0; i < area; ++i) {
                int finalColor = 2; 
                for (int li = 0; (li < Layers.Count) && (finalColor == 2); ++li) {
                    finalColor = Layers[li].Get(i); 
                }

                image.Set(i, finalColor); 
            }

            StringBuilder answer = new StringBuilder();
            answer.Append('\n'); 
            for (int y = 0; y < LayerHeight; ++y) {
                for (int x = 0; x < LayerWidth; ++x) {
                    int color = image.Get(x, y); 
                    answer.Append( GetColorChar(color) ); 
                }
                answer.Append('\n'); 
            }

            return answer.ToString(); 
        }
    }
}
