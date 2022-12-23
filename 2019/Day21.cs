using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   internal class Day21 : Day
   {
      private string InputFile = "2019/inputs/21.txt";
      private IntCodeMachine Program = new IntCodeMachine();

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Program.SetupFromFile(InputFile);
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         Program.Reset();

         Util.WriteLine(Program.ToString());
         return "";
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Program.Reset();
         return ""; 
      }
   }
}
