﻿using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day14 : Day
   {
      private string InputFile = "2022/inputs/14.txt";

      List<ivec2[]> Paths = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            Paths.Add( line.Split("->").Select(ivec2.Parse).ToArray() ); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public bool DropSand(IntCanvas canvas, ivec2 p)
      {
         if (canvas.GetValue(p) != 0) {
            return false; 
         }

         while (true) {
            ivec2 pn = p + ivec2.DOWN; 
            if (canvas.GetValue(pn) != 0) { 
               pn = pn + ivec2.LEFT; 
               if (canvas.GetValue(pn) != 0) {
                  pn = pn + 2 * ivec2.RIGHT; 
                  if (canvas.GetValue(pn) != 0) {
                     break; 
                  }
               }
            }
            p = pn; 
            if (p.y > canvas.GetMaxSetPosition().y) {
               return false; 
            }
         }

         canvas.SetValue(p, 1); 
         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         IntCanvas canvas = new IntCanvas(); 
         
         foreach (ivec2[] path in Paths) {
            for (int i = 0; i < path.Length - 1; ++i) { 
               ivec2 p0 = path[i]; 
               ivec2 p1 = path[i + 1]; 
               canvas.DrawStraightLine(p0, p1, 2); 
            }
         }

         int count = 0; 
         ivec2 origin = new ivec2(500, 0); 
         while (DropSand(canvas, origin)) {
            ++count;
         }

         return count.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public bool DropSandB(IntCanvas canvas, ivec2 p, int floor)
      {
         if (canvas.GetValue(p) != 0) {
            return false; 
         }


         while (true) {
            ivec2 pn = p + ivec2.DOWN; 
            if (pn.y >= floor) { 
               break; 
            }

            if (canvas.GetValue(pn) != 0) { 
               pn = pn + ivec2.LEFT; 
               if (canvas.GetValue(pn) != 0) {
                  pn = pn + 2 * ivec2.RIGHT; 
                  if (canvas.GetValue(pn) != 0) {
                     break; 
                  }
               }
            }
            p = pn; 
         }

         canvas.SetValue(p, 1); 
         return true; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         IntCanvas canvas = new IntCanvas(); 
         
         foreach (ivec2[] path in Paths) {
            for (int i = 0; i < path.Length - 1; ++i) { 
               ivec2 p0 = path[i]; 
               ivec2 p1 = path[i + 1]; 
               canvas.DrawStraightLine(p0, p1, 2); 
            }
         }

         int floor = canvas.GetMaxSetPosition().y + 2; 

         int count = 0; 
         ivec2 origin = new ivec2(500, 0); 
         while (DropSandB(canvas, origin, floor)) {
            ++count;
         }

         return count.ToString(); 
      }
   }
}
