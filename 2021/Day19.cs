using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    public class Beacon
    {
        public List<ivec3[]> Basis = new List<ivec3[]>(); 

        private float DegreeToRadian( int degree )
        {
            return (float)degree * MathF.PI / 180.0f; 
        }

        private void ConstructOrients()
        {
            for (int rz = 0; rz <= 270; rz += 90)
            {
                float cb = MathF.Cos(DegreeToRadian(rz)); 
                float sb = MathF.Sin(DegreeToRadian(rz)); 

                for (int rx = 0; rx <= 270; rx += 90)
                {
                    float cp = MathF.Cos(DegreeToRadian(rx)); 
                    float sp = MathF.Sin(DegreeToRadian(rx)); 

                    for (int ry = 0; ry <= 270; ry += 90)
                    {
                        float ch = MathF.Cos(DegreeToRadian(ry)); 
                        float sh = MathF.Sin(DegreeToRadian(ry)); 
                        
                        // euler to matrix
                        var i = ( ch * cb + sh * sp * sb, cb * sh * sp - sb * ch, cp * sh ); 
                        var j = (cp * sb, cb * cp, -sp );
                        var k = (sb * ch * sp - sh * cb, sh * sb + cb * ch * sp, cp * ch ); 

                        var ii = ((int) MathF.Round(i.Item1), (int) MathF.Round(i.Item2), (int) MathF.Round(i.Item3)); 
                        var ij = ((int) MathF.Round(j.Item1), (int) MathF.Round(j.Item2), (int) MathF.Round(j.Item3)); 
                        var ik = ((int) MathF.Round(k.Item1), (int) MathF.Round(k.Item2), (int) MathF.Round(k.Item3)); 

                        ivec3[] basis = new ivec3[3]; 
                        basis[0] = new ivec3(ii); 
                        basis[1] = new ivec3(ij); 
                        basis[2] = new ivec3(ik); 

                        if (!Basis.Any(v => v[0] == basis[0] && v[1] == basis[1] && v[2] == basis[2] ))
                        {
                            Basis.Add( basis ); 
                        }
                    }
                }
            }
        }

        public int GetOrientIndex( int aIdx, int bIdx )
        {
            ivec3[] a = Basis[aIdx]; 
            ivec3[] b = Basis[bIdx]; 
            ivec3[] c = new ivec3[3]; 
            c[0] = Transform( a, b[0] ); 
            c[1] = Transform( a, b[1] ); 
            c[2] = Transform( a, b[2] ); 
            return Basis.FindIndex( basis => basis[0] == c[0] && basis[1] == c[1] && basis[2] == c[2] ); 
        }

        public ivec3 Transform( ivec3[] basis, ivec3 pos )
        {
            return pos.x * basis[0] + pos.y * basis[1] + pos.z * basis[2]; 
        }

        public ivec3 InvertTransform( ivec3[] basis, ivec3 pos )
        {
            return new ivec3( pos.Dot( basis[0] ), pos.Dot( basis[1] ), pos.Dot( basis[2] ) );
        }

        public Beacon( List<string> lines )
        {
            ConstructOrients(); 
            Positions = new List<ivec3>[ Basis.Count ]; 
            Positions[0] = new List<ivec3>(); 

            string header = lines[0]; 
            lines.RemoveAt(0); 

            header = header.Replace("---", ""); 
            header = header.Replace("scanner", ""); 
            header = header.Trim();
            BeaconID = int.Parse(header); 

            while (lines.Count > 0)
            {
                string line = lines[0]; 
                lines.RemoveAt(0); 
                if (string.IsNullOrEmpty(line))
                {
                    break; 
                }

                Positions[0].Add( ivec3.Parse(line) ); 
            }

            // now create the different potential orientations
            for (int i = 1; i < Positions.Length; ++i)
            {
                Positions[i] = new List<ivec3>(); 
                foreach (var pos in Positions[0])
                {
                    Positions[i].Add( Transform( Basis[i], pos )); 
                }
            }

            if (BeaconID == 0)
            {
                BeaconLink = 0; 
                RelativeOrientToLink = 0; 
                RelativeOffsetToLink = ivec3.ZERO; 
            }
        }

        public int GetPositionCount()
        {
            return Positions[0].Count; 
        }

        public List<ivec3> GetRelativeSet( int orientIndex, int basePoint )
        {
            ivec3 basePos = Positions[orientIndex][basePoint]; 
            List<ivec3> subset = new List<ivec3>( Positions[orientIndex].Count ); 
            
            foreach (ivec3 pos in Positions[orientIndex] )
            {
                subset.Add( pos - basePos ); 
            }

            return subset;
        }

        public List<ivec3> GetOriginSet()
        {
            if (HasLink())
            {
                List<ivec3> points = new List<ivec3>( GetPositionCount() ); 
                ivec3 offset = RelativeOffsetToLink; 
                ivec3[] basis = Basis[RelativeOrientToLink]; 

                foreach (ivec3 p in Positions[0] )
                {
                    points.Add( offset + Transform(basis, p ) ); 
                }

                return points; 
            }
            else
            {
                return Positions[0]; 
            }
        }


        public bool HasLink()
        {
            return (BeaconLink >= 0); 
        }

        public bool TryToFindLink( Beacon other )
        {
            // already got a link
            if (HasLink())
            { 
                return true;
            }

            if (LinksTried.Contains(other.BeaconID))
            {
                return false; 
            }

            // okay, so, I can keep other stationary, and rotate myself
            for (int i = 0; i < other.GetPositionCount(); ++i)
            {
                List<ivec3> otherRelSet = other.GetRelativeSet( 0, i ); 
                for (int orientIdx = 0; orientIdx < Basis.Count; ++orientIdx)
                {
                    for (int posIdx = 0; posIdx < GetPositionCount(); ++posIdx)
                    {
                        List<ivec3> relSet = GetRelativeSet( orientIdx, posIdx ); 
                        ivec3[] intersection = relSet.Intersect( otherRelSet ).ToArray(); 
                        if (intersection.Length >= 12) 
                        { 
                            BeaconLink = other.BeaconID; 
                            RelativeOrientToLink = orientIdx; 

                            int relSetIdx = relSet.FindIndex( v => v == intersection[0] ); 
                            int baseSetIdx = otherRelSet.FindIndex( v => v == intersection[0] ); 
                            RelativeOffsetToLink = other.Positions[0][baseSetIdx] - Positions[orientIdx][relSetIdx]; 
                            return true; 
                        }
                    }
                }
            }

            LinksTried.Add( other.BeaconID ); 
            return false; 
        }

        public int BeaconID = 0; 
        public List<ivec3>[] Positions; 
        private List<int> LinksTried = new List<int>(); 

        public int BeaconLink = -1; 
        public int RelativeOrientToLink = 0; // what orient did we use
        public ivec3 RelativeOffsetToLink; // where is this beacon relative to the one it is linked from, IN the links default orient
    }

    internal class Day19 : Day
    {
        List<Beacon> Beacons = new List<Beacon>(); 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
            string inputFile = "2021/inputs/19.txt"; 
            List<string> lines = Util.ReadFileToLines(inputFile); 

            while (lines.Count > 0)
            {
                Beacon beacon = new Beacon(lines); 
                Beacons.Add(beacon); 
            }

            // Create relations
            while (Beacons.Any( b => !b.HasLink()))
            {
                for (int i = 0; i < Beacons.Count; ++i)
                {
                    Beacon iBeacon = Beacons[i];
                    if (!iBeacon.HasLink())
                    {
                        continue;     
                    }

                    for (int j = 1; j < Beacons.Count; ++j)
                    {
                        Beacon jBeacon = Beacons[j]; 
                        if ((i == j) || jBeacon.HasLink())
                        {
                            continue; 
                        }

                        jBeacon.TryToFindLink( iBeacon ); 
                    }
                }
            }

            // Collapse the links so everything is relative to the root (0)
            bool allRooted = false;
            while (!allRooted)
            {
                allRooted = true; 
                for (int i = 1; i < Beacons.Count; ++i)
                {
                    Beacon b = Beacons[i]; 
                    if (b.BeaconLink != 0)
                    {
                        Beacon link = Beacons[b.BeaconLink]; 
                        ivec3 offset = b.RelativeOffsetToLink; // this is in my links space
                        ivec3[] basis = b.Basis[link.RelativeOrientToLink]; 

                        // find relative to whatever the person I'm relative to. 
                        b.BeaconLink = link.BeaconLink; 
                        b.RelativeOffsetToLink = link.RelativeOffsetToLink + link.Transform( basis, offset ); 
                        b.RelativeOrientToLink = link.GetOrientIndex( link.RelativeOrientToLink, b.RelativeOrientToLink ); 

                        allRooted = allRooted && (b.BeaconLink == 0); 
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            HashSet<ivec3> uniquePoints = new HashSet<ivec3>(); 
            foreach( Beacon b in Beacons )
            {
                List<ivec3> originSet = b.GetOriginSet();
                foreach (ivec3 p in originSet)
                { 
                    uniquePoints.Add( p ); 
                }
            }

            return uniquePoints.Count.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            int maxDist = 0; 
            for (int i = 0; i < Beacons.Count; ++i)
            { 
                Beacon iBeacon = Beacons[i]; 
                for (int j = i + 1; j < Beacons.Count; ++j) 
                {
                    Beacon jBeacon = Beacons[j]; 
                    maxDist = Math.Max( maxDist, (iBeacon.RelativeOffsetToLink - jBeacon.RelativeOffsetToLink).GetManhattanDistance() ); 
                }
            }

            return maxDist.ToString(); 
        }
    }
}
