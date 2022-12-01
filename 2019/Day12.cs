using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; 
using System.Text;
using System.Threading.Tasks;

namespace AoC2019
{
    internal class Day12 : Day
 {
        private string InputFile = "2019/inputs/12.txt"; 
        private ivec3[] Planets = new ivec3[0]; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            Planets = Util.ReadFileToLines(InputFile).Select(ivec3.Parse).ToArray(); 
        }

        //----------------------------------------------------------------------------------------------
        private ivec3 GetGravity(ivec3 p0, ivec3 p1)
        {
            return ivec3.Sign(p1 - p0); 
        }

        //----------------------------------------------------------------------------------------------
        private void ApplyGravity(ivec3[] pos, ivec3[] vel)
        {
            for (int i = 0; i < pos.Length; ++i) {
                for (int j = i + 1; j < pos.Length; ++j) {
                    ivec3 grav = GetGravity( pos[i], pos[j] ); 
                    vel[i] += grav; 
                    vel[j] -= grav; 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        private void ApplyVelocities(ivec3[] pos, ivec3[] vel)
        {
            for (int i = 0; i < pos.Length; ++i) {
                pos[i] += vel[i]; 
            }
        }

        //----------------------------------------------------------------------------------------------
        private int GetEnergy(ivec3[] pos, ivec3[] vel)
        {
            int energy = 0; 
            for (int i = 0; i < pos.Length; ++i) {
                energy += pos[i].GetManhattanDistance() * vel[i].GetManhattanDistance(); 
            }

            return energy; 
        }

        //----------------------------------------------------------------------------------------------
        private void DebugWrite( ivec3[] pos, ivec3[] vel )
        {
            for (int i = 0; i < pos.Length; ++i) {
                Util.WriteLine( $"pos={pos[i]}, vel={vel[i]}" ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            ivec3[] planets = new ivec3[Planets.Length]; 
            ivec3[] velocities = new ivec3[Planets.Length]; 
            Planets.CopyTo(planets, 0); 

            const int maxSteps = 1000; 
            for (int step = 0; step < maxSteps; ++step) {
                // Util.WriteLine( $"\nAfter step {step}:" ); 
                // DebugWrite( planets, velocities ); 

                ApplyGravity( planets, velocities ); 
                ApplyVelocities( planets, velocities ); 
            }

            return GetEnergy( planets, velocities ).ToString(); 
        }

        private HashSet<string> HackSet = new HashSet<string>(); 

        private bool HasBeenSeen(ivec3[] planets, ivec3[] velocities)
        {
            // oh there is definitly a better way to "key" this, but this will probably be fine for the numbers we're dealing with
            string key = $"{planets[0]}, {planets[1]}, {planets[2]}, {planets[3]} - {velocities[0]}, {velocities[1]}, {velocities[2]}, {velocities[3]}"; 
            if (HackSet.Contains(key)) {
                return true; 
            }

            HackSet.Add(key); 
            return false; 
        }

        //----------------------------------------------------------------------------------------------
        private bool IsInitialState( int[] init, int[] p, int[] v )
        {
            for (int i = 0; i < 4; ++i) {
                if ((init[i] != p[i]) || (v[i] != 0)) {
                    return false;
                }
            }

            return true; 
        }

        //----------------------------------------------------------------------------------------------
        public int StepsToRepeat( int a, int b, int c, int d )
        {
            int[] v = new int[4]; 
            int[] p = new int[4] { a, b, c, d }; 
            int[] initial = new int[] { a, b, c, d }; 

            int step = 0; 
            do {
                // velocities
                for (int i = 0; i < 4; ++i) {
                    for (int j = i + 1; j < 4; ++j) {
                        int vel = Math.Sign(p[j] - p[i]); 
                        v[i] += vel; 
                        v[j] -= vel; 
                    }
                }


                for (int i = 0; i < 4; ++i) {
                    p[i] += v[i]; 
                }

                ++step; 
            } while (!IsInitialState(initial, p, v)); 

            return step; 
        }

        //----------------------------------------------------------------------------------------------
        private Int64 Reduce( int[] steps )
        {
            Int64 gcd0 = Util.GCD( steps[0], steps[1] ); 
            Int64 totalSteps = (Int64) steps[0] * (Int64) steps[1]; 
            totalSteps /= gcd0; 

            Int64 gcd1 = Util.GCD( totalSteps, steps[2] ); 
            totalSteps = totalSteps * (Int64) steps[2]; 
            totalSteps /= gcd1; 

            return totalSteps;
        }
        
        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            ivec3[] planets = new ivec3[Planets.Length]; 
            ivec3[] velocities = new ivec3[Planets.Length]; 
            Planets.CopyTo(planets, 0); 

            int[] compSteps = new int[3]; 
            for (int i = 0; i < 3; ++i) {
                compSteps[i] = StepsToRepeat( planets[0][i], planets[1][i], planets[2][i], planets[3][i] ); 
            }

            Int64 steps = Reduce(compSteps); 

            // int steps = compSteps[0] * compSteps[1] * compSteps[2]; 
            return steps.ToString(); 
        }
    }
}
