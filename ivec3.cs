using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    public struct ivec3
    {
        public int x = 0; 
        public int y = 0;
        public int z = 0; 

        public static readonly ivec3 ZERO = new ivec3(0, 0, 0); 
        public static readonly ivec3 ONE = new ivec3(1, 1, 1); 

        public static readonly ivec3 LEFT = new ivec3(-1, 0, 0); 
        public static readonly ivec3 RIGHT = new ivec3(1, 0, 0); 
        public static readonly ivec3 UP = new ivec3(0, -1, 0); 
        public static readonly ivec3 DOWN = new ivec3(0, 1, 0); 


        public ivec3( int v )
        {
            x = v; 
            y = v; 
            z = v; 
        }

        public ivec3( int xv, int yv, int zv )
        {
            x = xv; 
            y = yv; 
            z = zv; 
        }

        public ivec3( (int,int,int) t )
        {
            (x, y, z) = t; 
        }

        public int this[int i]
        {
            get => i switch
            {
                0 => x,
                1 => y, 
                2 => z, 
                _ => 0
            };

            set
            {
                switch (i)
                {
                    case 0: x = value; break; 
                    case 1: y = value; break; 
                    case 2: z = value; break; 
                    default: break; 
                }
            }
        }

        public int Dot( ivec3 v ) => x * v.x + y * v.y + z * v.z; 
        public int GetLengthSquared() => x * x + y * y + z * z; 
        public float GetLength() => MathF.Sqrt( (float) GetLengthSquared() ); 
        public int Sum() { return x + y + z; }
        public int Product() { return x * y * z; }
        public int GetManhattanDistance() { return ivec3.Abs(this).Sum(); }

        public static ivec3 operator +( ivec3 v ) => v; 
        public static ivec3 operator -( ivec3 v ) => new ivec3( -v.x, -v.y, -v.z ); 
        public static ivec3 operator +( ivec3 a, ivec3 b ) => new ivec3( a.x + b.x, a.y + b.y, a.z + b.z); 
        public static ivec3 operator -( ivec3 a, ivec3 b ) => new ivec3( a.x - b.x, a.y - b.y, a.z - b.z ); 
        public static ivec3 operator *( ivec3 a, ivec3 b ) => new ivec3( a.x * b.x, a.y * b.y, a.z * b.z ); 
        public static ivec3 operator *( int a, ivec3 v ) => new ivec3( a * v.x, a * v.y, a * v.z ); 
        public static ivec3 operator *( ivec3 v, int a ) => new ivec3( a * v.x, a * v.y, a * v.z ); 
        public static bool operator ==( ivec3 a, ivec3 b ) => (a.x == b.x) && (a.y == b.y) && (a.z == b.z); 
        public static bool operator !=( ivec3 a, ivec3 b ) => (a.x != b.x) || (a.y != b.y) || (a.z != b.z); 
        public static bool operator < ( ivec3 a, ivec3 b ) => (a.x < b.x) && (a.y < b.y) && (a.z < b.z); 
        public static bool operator <=( ivec3 a, ivec3 b ) => (a.x <= b.x) && (a.y <= b.y) && (a.z <= b.z); 
        public static bool operator > ( ivec3 a, ivec3 b ) => (a.x > b.x) && (a.y > b.y) && (a.z > b.z); 
        public static bool operator >=( ivec3 a, ivec3 b ) => (a.x >= b.x) && (a.y >= b.y) && (a.z >= b.z); 

        public static ivec3 Sign( ivec3 v ) => new ivec3( Math.Sign(v.x), Math.Sign(v.y), Math.Sign(v.z) ); 
        public static ivec3 Min( ivec3 a, ivec3 b ) => new ivec3( Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z) ); 
        public static ivec3 Max( ivec3 a, ivec3 b ) => new ivec3( Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z) ); 
        public static ivec3 Abs( ivec3 v ) => new ivec3( Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z) ); 

        public static ivec3 Max( IEnumerable<ivec3> list )
        {
            ivec3 ret = list.First(); 
            foreach (ivec3 v in list)
            {
                ret = ivec3.Max( ret, v ); 
            }

            return ret; 
        }

        public static int Dot( ivec3 a, ivec3 b ) => a.Dot(b); 

        public static ivec3 Parse( string s )
        {
            string[] parts = s.Split(',', 3); 
            int x = int.Parse(parts[0]); 
            int y = int.Parse(parts[1]); 
            int z = int.Parse(parts[2]); 
            return new ivec3( x, y, z );  
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if ((obj == null) || !obj.GetType().Equals(GetType()))
            {
                return false; 
            }

            ivec3 other = (ivec3) obj; 
            return (x == other.x) && (y == other.y) && (z == other.z); 
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x.GetHashCode(), HashCode.Combine(y.GetHashCode(), z.GetHashCode()));
        }

        public override string ToString()
        {
            return $"{x},{y},{z}"; 
        }
    }
}
