using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day04 : Day
   {
      private string InputFile = "2024/inputs/04.txt";

      //----------------------------------------------------------------------------------------------

      IntMap WordSearch = new IntMap(new ivec2(1, 1)); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         string words = Util.ReadFileToString(InputFile); 
         WordSearch.SetupWithValues(words); 
      }

      //----------------------------------------------------------------------------------------------
      public int FindWord(ivec2 pos, ivec2 dir, string word) 
      {
         ivec2 loc = pos; 
         
         foreach (char c in word) 
         {
            int? val = WordSearch.TryGet(loc); 
            if (!val.HasValue || val.Value != (int)c) {
               return 0; 
            }

            loc += dir; 
         }

         return 1; 
      }

      //----------------------------------------------------------------------------------------------
      public int FindWord(ivec2 pos, string word) 
      {
         int count = 0; 
         foreach (ivec2 dir in ivec2.CARDINAL) {
            count += FindWord(pos, dir, word); 
         }

         return count; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int countWords = 0; 
         for (int y = 0; y < WordSearch.Height; ++y) {
            for (int x = 0; x < WordSearch.Width; ++x) {
               countWords += FindWord( new ivec2(x, y), "XMAS" ); 
            }
         }
         
         return countWords.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int countWords = 0; 
         for (int y = 0; y < WordSearch.Height - 2; ++y) {
            for (int x = 0; x < WordSearch.Width - 2; ++x) {
               ivec2 p = new ivec2(x, y); 
               if ((FindWord(p, new ivec2(1, 1), "MAS") > 0)
                  || (FindWord(p, new ivec2(1, 1), "SAM") > 0)) {
               
                  ivec2 op = p + new ivec2(0, 2); 
                  if ((FindWord(op, new ivec2(1, -1), "MAS") > 0)
                     || (FindWord(op, new ivec2(1, -1), "SAM") > 0)) {

                     ++countWords; 
                  }
               }
            }
         }
         
         return countWords.ToString(); 
      }
   }
}
