using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day25 : Day
   {
      private string InputFile = "2022/inputs/25.txt";


      string[] Lines = new string[0]; 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         Lines = Util.ReadFileToLines(InputFile).ToArray(); 
      }

      //----------------------------------------------------------------------------------------------
      Int64 FromSNAFU(string s)
      {
         Int64 val = 0; 
         Int64 mul = 1; 
         for (int idx = s.Length - 1; idx >= 0; --idx) { 
            Int64 v = s[idx] switch {
               '=' => -2, 
               '-' => -1, 
               '0' => 0, 
               '1' => 1, 
               '2' => 2,
               _ => 0
            };

            val += v * mul;
            mul *= 5; 
         }

         return val; 
      }

      //----------------------------------------------------------------------------------------------
      string ToSNAFU(Int64 v)
      {
         // get the largest power of 5 that is greater that v
         Int64 av = Math.Abs(v); 
         Int64 sign = Math.Sign(v); 
         Int64 pow = 1; 
         Int64 maxVal = 2; 

         Int64 len = 1; 
         while (maxVal < av) { // greatest I can store at each level
            pow *= 5; 
            maxVal += pow * 2; 
            ++len; 
         }

         // round av to closet pow
         string res = ""; 
         while (pow > 0) { 
            av = Math.Abs(v); 
            Int64 d = Math.Sign(v) * (av + (pow / 2)) / pow;  // 0, 1, or 2
            Int64 newVal = d * pow; 
            v = v - newVal; 

            res += d switch {
               -2 => "=", 
               -1 => "-",
               0 => "0",
               1 => "1", 
               2 => "2", 
               _ => "1", 
            }; 

            pow /= 5; 
         }

         return res; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         string s0 = ToSNAFU(10); // "22"

         Int64 total = 0; 
         foreach (string l in Lines) {
            total += FromSNAFU(l); 
         }

         return ToSNAFU(total); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         return "[red]M[green]e[red]r[green]r[red]y [green]C[red]h[green]r[red]i[green]s[red]t[green]m[red]a[green]s[white]![-]"; 
      }
   }
}
