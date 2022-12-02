using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day02 : Day
   {
      private string InputFile = "2022/inputs/02.txt";
      private List<string> Lines; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Lines = Util.ReadFileToLines(InputFile);
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int theirScore = 0; 
         int myScore = 0; 
         foreach (string line in Lines) {
            int them = line[0] - 'A'; 
            int me = line[2] - 'X'; 
            
            theirScore += them + 1; 
            myScore += me + 1; 

            int result = me - them; 
            if (result < 0) {
               result += 3; 
            }

            switch (result) {
               case 0: 
                  theirScore += 3; 
                  myScore += 3; 
                  break; 

               case 1: 
                  myScore += 6; 
                  break; 

               case 2: 
                  theirScore += 6; 
                  break; 

               default: 
                  break; 
            }
         }

         return myScore.ToString(); 
      }


      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         int theirScore = 0; 
         int myScore = 0; 
         foreach (string line in Lines) {
            int them = line[0] - 'A'; 
            int desiredResult = line[2] - 'X'; 

            desiredResult = desiredResult - 1; 
            int me = them + desiredResult; 
            if (me < 0) {
               me += 3; 
            } else if (me >= 3) {
               me = 0; 
            }
            
            theirScore += them + 1; 
            myScore += me + 1; 

            int result = desiredResult; 
            if (result < 0) {
               result += 3; 
            }

            switch (result) {
               case 0: 
                  theirScore += 3; 
                  myScore += 3; 
                  break; 

               case 1: 
                  myScore += 6; 
                  break; 

               case 2: 
                  theirScore += 6; 
                  break; 

               default: 
                  break; 
            }
         }

         return myScore.ToString(); 
      }
   }
}
