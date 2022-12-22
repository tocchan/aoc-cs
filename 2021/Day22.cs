using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal struct CoreInput
    {

        public bool On; 
        public iaabb3 Cube; 

        //----------------------------------------------------------------------------------------------
        public CoreInput( string line )
        {
            (string on, line) = line.Split(' ', 2 ); 
            On = (on == "on"); 

            string[] coords = line.Split(','); 

            ivec3 minCoord = ivec3.ZERO;
            ivec3 maxCoord = ivec3.ZERO; 
            for (int i = 0; i < 3; ++i)
            {
                string coord = coords[i]; 
                coord = coord.Split('=', 2)[1]; 
                (minCoord[i], maxCoord[i], _) = coord.Split("..", 2).Select(int.Parse).ToList(); 
            }

            Cube = iaabb3.ThatContains( minCoord, maxCoord ); 
        }
    }

   

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    internal class Day22 : Day
    {
        List<CoreInput> Inputs = new List<CoreInput>(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/22.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 
            foreach (string line in lines)
            {
                Inputs.Add( new CoreInput(line) ); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<iaabb3> cubes = new List<iaabb3>(); 

            Queue<iaabb3> newCubes = new Queue<iaabb3>(); 
            foreach (CoreInput input in Inputs)
            {
                if (input.On)
                {
                    newCubes.Enqueue( input.Cube ); 
                    while (newCubes.Count > 0)
                    {
                        iaabb3 cube = newCubes.Dequeue(); 
                        
                        iaabb3? intersection = FindIntersection( cube, cubes ); 
                        if (intersection == null)
                        {
                            cubes.Add(cube); 
                        }
                        else 
                        {
                            iaabb3[] splitCubes = cube.Subtract( intersection.Value ); 
                            foreach (var splitCube in splitCubes)
                            {
                                newCubes.Enqueue( splitCube ); 
                            }
                        }
                    }
                }
                else // Off case
                {
                    for (int i = cubes.Count - 1; i >= 0; --i)
                    {
                        iaabb3 cube = cubes[i]; 
                        if (cube.Intersects(input.Cube))
                        {
                            cubes.RemoveAt(i); 
                            iaabb3[] splitCubes = cube.Subtract( input.Cube ); 
                            cubes.AddRange( splitCubes ); 
                        }
                    }
                }
            }

            // Cool, got all my cubes, cull down to part 1 range
            iaabb3 clipRegion = iaabb3.ThatContains( new ivec3(-50, -50, -50), new ivec3(50, 50, 50) ); 
            Int64 onCount = 0; 
            foreach (iaabb3 cube in cubes)
            {
                iaabb3 overlap = cube.GetOverlap( clipRegion ); 
                onCount += overlap.GetVolume(); 
            }

            return onCount.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<iaabb3> cubes = new List<iaabb3>(); 

            Queue<iaabb3> newCubes = new Queue<iaabb3>(); 
            foreach (CoreInput input in Inputs)
            {
                if (input.On)
                {
                    newCubes.Enqueue( input.Cube ); 
                    while (newCubes.Count > 0)
                    {
                        iaabb3 cube = newCubes.Dequeue(); 
                        
                        iaabb3? intersection = FindIntersection( cube, cubes ); 
                        if (intersection == null)
                        {
                            cubes.Add(cube); 
                        }
                        else 
                        {
                            iaabb3[] splitCubes = cube.Subtract( intersection.Value ); 
                            foreach (var splitCube in splitCubes)
                            {
                                newCubes.Enqueue( splitCube ); 
                            }
                        }
                    }
                }
                else // Off case
                {
                    for (int i = cubes.Count - 1; i >= 0; --i)
                    {
                        iaabb3 cube = cubes[i]; 
                        if (cube.Intersects(input.Cube))
                        {
                            cubes.RemoveAt(i); 
                            iaabb3[] splitCubes = cube.Subtract( input.Cube ); 
                            cubes.AddRange( splitCubes ); 
                        }
                    }
                }
            }

            Int64 onCount = 0; 
            foreach (iaabb3 cube in cubes)
            {
                onCount += cube.GetVolume(); 
            }

            return onCount.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        private iaabb3? FindIntersection(iaabb3 cube, List<iaabb3> cubes)
        {
            foreach (iaabb3 iter in cubes)
            {
                if (iter.Intersects(cube))
                {
                    return iter; 
                }
            }

            return null; 
        }
    }
}
