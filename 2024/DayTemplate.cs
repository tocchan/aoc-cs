using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class DayTemplate : Day
   {
      private string InputFile = "2024/inputs/01.txt";


      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      class Input
      {
         public Input(string line)
         {

         }
      }

      private List<Input> Inputs = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Input input = new Input(line); 
            Inputs.Add(input); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         return ""; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return ""; 
      }
   }
}
