using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    public struct line2
    {
        public vec2 start = vec2.ZERO; 
        public vec2 end = vec2.ZERO;

        public vec2 direction = vec2.RIGHT; 
        public double distance = 0.0f; 
        public double idistance = double.PositiveInfinity; 

        public line2() { }

        public line2( vec2 a, vec2 b )
        {
            start = a; 
            end = b; 

            if (start != end) {
                distance = (b - a).GetLength(); 
                idistance = 1.0 / distance; 
                direction = (b - a) * idistance; 
            }
        }

        public vec2 GetNearestPointTo( vec2 point )
        {
            vec2 disp = point - start; 

            // project onto the line
            double dist = disp.Dot(direction); 
            dist = Math.Clamp(dist, 0, distance); 

            return start + dist * direction;
        }

        public double GetDistanceTo(vec2 point)
        { 
            vec2 p = GetNearestPointTo(point); 
            return (p - point).GetLength(); 
        }

        public bool IsTouching(vec2 point, double epsilon = double.Epsilon)
        {
            vec2 p = GetNearestPointTo(point); 
            return point.IsNear(p, epsilon); 
        }

    }
}
