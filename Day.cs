using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal abstract class Day
    {
        abstract public void ParseInput(); 
        abstract public string RunA();
        abstract public string RunB();

        public static Day Create( string? num, System.Type defType )
        {
            int dayNum;
            if (string.IsNullOrEmpty(num) || (num.Length > 2) || !int.TryParse(num, out dayNum))
            {
                return (Day) Activator.CreateInstance(defType)!;
            }

            num = (num.Length < 2) ? $"0{num}" : num; 
            string className = $"AoC2021.Day{num}"; 
            System.Type? type = Type.GetType(className); 
            type = (type == null) ? defType : type; 
            return (Day) Activator.CreateInstance(type)!; 
        }
    }
}
