using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2024
{
   internal class Day01 : Day
   {
      private string InputFile = "2024/inputs/01.txt";

      private List<Int64> LeftList = new(); 
      private List<Int64> RightList = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Int64[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
            LeftList.Add(values[0]); 
            RightList.Add(values[1]); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         List<Int64> lh = new List<Int64>(LeftList); 
         List<Int64> rh = new List<Int64>(RightList); 

         Int64 sum = 0; 
         while (lh.Count > 0) 
         {
            Int64 sl = lh.Min(); 
            Int64 sr = rh.Min(); 

            lh.Remove(sl); 
            rh.Remove(sr); 

            sum += Math.Abs(sl - sr); 
         }
         
         return sum.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 sum = 0; 
         foreach (Int64 val in LeftList) 
         {
            sum += val * RightList.Count((Int64 v) => v == val); 
         }

         return sum.ToString(); 
      }
   }
}

