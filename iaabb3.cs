namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    public struct iaabb3
    {
        public ivec3 MinInclusive = ivec3.ONE; 
        public ivec3 MaxInclusive = -ivec3.ONE; 
        
        //----------------------------------------------------------------------------------------------
        public iaabb3() { }

        //----------------------------------------------------------------------------------------------
        public bool IsValid()
        {
            return MaxInclusive >= MinInclusive; 
        }

        //----------------------------------------------------------------------------------------------
        public static iaabb3 ThatContains( ivec3 a, ivec3 b )
        {
            iaabb3 ret; 
            ret.MinInclusive = ivec3.Min( a, b ); 
            ret.MaxInclusive = ivec3.Max( a, b ); 
            return ret; 
        }

        //----------------------------------------------------------------------------------------------
        public iaabb3 GetOverlap( iaabb3 other )
        {
            iaabb3 overlap; 
            overlap.MinInclusive = ivec3.Max( MinInclusive, other.MinInclusive ); 
            overlap.MaxInclusive = ivec3.Min( MaxInclusive, other.MaxInclusive ); 
            return overlap; 
        }

        //----------------------------------------------------------------------------------------------
        public ivec3 GetSize()
        { 
            return IsValid() ? (MaxInclusive - MinInclusive + ivec3.ONE) : ivec3.ZERO; 
        }


        //----------------------------------------------------------------------------------------------
        public Int64 GetVolume()
        {
            ivec3 size = GetSize(); 
            return (Int64)size.x * (Int64)size.y * (Int64)size.z; 
        }

        //----------------------------------------------------------------------------------------------
        public bool Intersects( iaabb3 cube )
        {
            return GetOverlap(cube).IsValid(); 
        }

        //----------------------------------------------------------------------------------------------
        public (iaabb3, iaabb3) Cut( int dim, int coord )
        {
            coord = Math.Clamp( coord, MinInclusive[dim] - 1, MaxInclusive[dim] ); 

            iaabb3 neg = this; 
            neg.MaxInclusive[dim] = coord; 

            iaabb3 pos = this; 
            pos.MinInclusive[dim] = coord + 1; 

            return (neg, pos); 
        }

        //----------------------------------------------------------------------------------------------
        // Cut functions may result in a non-valid region
        // All cut functions include the passed in coordinate as part of the first returned value (when it overlaps)
        public (iaabb3, iaabb3) CutX( int xCoord )
        {
            return Cut( 0, xCoord ); 
        }

        //----------------------------------------------------------------------------------------------
        public (iaabb3, iaabb3) CutY( int yCoord )
        {
            return Cut( 1, yCoord ); 
        }

        //----------------------------------------------------------------------------------------------
        public (iaabb3, iaabb3) CutZ( int zCoord )
        {
            return Cut( 2, zCoord ); 
        }

        //----------------------------------------------------------------------------------------------
        public iaabb3[] Subtract(iaabb3 cube)
        {
            iaabb3 overlap = GetOverlap(cube); 
            if (!overlap.IsValid())
            {
                return new iaabb3[] {this}; // no overlapo, resultant cube is just the original object
            }

            List<iaabb3> cubes = new List<iaabb3>(); 
            iaabb3 original = this; 

            for (int i = 0; i < 3; ++i)
            {
                (iaabb3 toAdd, original) = original.Cut( i, overlap.MinInclusive[i] - 1 ); 
                if (toAdd.IsValid())
                {
                    cubes.Add( toAdd ); 
                }

                (original, toAdd) = original.Cut( i, overlap.MaxInclusive[i] ); 
                if (toAdd.IsValid())
                {
                    cubes.Add( toAdd ); 
                }
            }

            return cubes.ToArray(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return $"{MinInclusive.x}..{MaxInclusive.x}, {MinInclusive.y}..{MaxInclusive.y}, {MinInclusive.z}..{MaxInclusive.z}";
        }
    }
}
