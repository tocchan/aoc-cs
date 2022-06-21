using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    public struct vec2
    {
        public float x = 0; 
        public float y = 0;

        public static readonly vec2 ZERO = new vec2(0, 0); 
        public static readonly vec2 ONE = new vec2(1, 1); 

        public static readonly vec2 LEFT = new vec2(-1, 0); 
        public static readonly vec2 RIGHT = new vec2(1, 0); 
        public static readonly vec2 UP = new vec2(0, -1); 
        public static readonly vec2 DOWN = new vec2(0, 1); 

        public vec2( float v )
        {
            x = v; 
            y = v; 
        }

        public vec2( float xv, float yv )
        {
            x = xv; 
            y = yv;
        }

        public void Normalize()
        { 
            float l = GetLength(); 
            if (l != 0.0f) {
                x /= l; 
                y /= l; 
            }
        }

        public vec2 GetNormalized()
        {
            vec2 v = this; 
            v.Normalize(); 
            return v; 
        }

        public float Sum() => x + y; 
        public float Product() => x * y; 

        public bool IsNearZero(float epsilon = float.Epsilon)
        {
            return (GetManhattanDistance() <= epsilon); 
        }

        public bool IsNear(vec2 other, float epsilon = float.Epsilon)
        {
            return (this - other).IsNearZero(epsilon); 
        }

        public float Dot( vec2 v ) => x * v.x + y * v.y; 
        public float GetLengthSquared() => x * x + y * y; 
        public float GetLength() => MathF.Sqrt( GetLengthSquared() ); 

        public float GetManhattanDistance() => Abs(this).Sum();

        public float this[int i]
        {
            get { return (i == 0) ? x : y; }
            set { if (i == 0) { x = value; } else { y = value; } }
        }

        public static vec2 operator +( vec2 v ) => v; 
        public static vec2 operator -( vec2 v ) => new vec2( -v.x, -v.y ); 
        public static vec2 operator +( vec2 a, vec2 b ) => new vec2( a.x + b.x, a.y + b.y ); 
        public static vec2 operator -( vec2 a, vec2 b ) => new vec2( a.x - b.x, a.y - b.y ); 
        public static vec2 operator *( vec2 a, vec2 b ) => new vec2( a.x * b.x, a.y * b.y ); 
        public static vec2 operator *( float a, vec2 v ) => new vec2( a * v.x, a * v.y ); 
        public static vec2 operator *( vec2 v, float a ) => new vec2( a * v.x, a * v.y ); 
        public static vec2 operator /( vec2 v, float a ) => new vec2( v.x / a, v.y / a ); 

        public static bool operator ==( vec2 a, vec2 b ) => (a.x == b.x) && (a.y == b.y); 
        public static bool operator !=( vec2 a, vec2 b ) => (a.x != b.x) || (a.y != b.y); 
        public static bool operator < ( vec2 a, vec2 b ) => (a.x < b.x) && (a.y < b.y); 
        public static bool operator <=( vec2 a, vec2 b ) => (a.x <= b.x) && (a.y <= b.y); 
        public static bool operator > ( vec2 a, vec2 b ) => (a.x > b.x) && (a.y > b.y); 
        public static bool operator >=( vec2 a, vec2 b ) => (a.x >= b.x) && (a.y >= b.y); 

        public static vec2 Sign( vec2 v ) => new vec2( Math.Sign(v.x), Math.Sign(v.y) ); 
        public static vec2 Min( vec2 a, vec2 b ) => new vec2( Math.Min(a.x, b.x), Math.Min(a.y, b.y) ); 
        public static vec2 Max( vec2 a, vec2 b ) => new vec2( Math.Max(a.x, b.x), Math.Max(a.y, b.y) ); 
        public static vec2 Abs( vec2 v ) => new vec2( Math.Abs(v.x), Math.Abs(v.y) ); 
        public static vec2 Max( IEnumerable<vec2> list )
        {
            vec2 ret = list.First(); 
            foreach (vec2 v in list)
            {
                ret = vec2.Max( ret, v ); 
            }

            return ret; 
        }

        public static float Dot( vec2 a, vec2 b ) => a.x * b.x + a.y * b.y; 

        public static vec2 Parse( string s )
        {
            string[] parts = s.Split(',', 2); 
            float x = float.Parse(parts[0]); 
            float y = float.Parse(parts[1]); 
            return new vec2( x, y );  
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if ((obj == null) || !obj.GetType().Equals(GetType()))
            {
                return false; 
            }

            vec2 other = (vec2) obj; 
            return (x == other.x) && (y == other.y); 
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x.GetHashCode(), y.GetHashCode());
        }

        public override string ToString()
        {
            return $"{x},{y}"; 
        }

        public static ivec2 RoundToInt(vec2 p)
        {
            return new ivec2( (int) Math.Round(p.x), (int) Math.Round(p.y) ); 
        }
    }
}
