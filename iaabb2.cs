namespace AoC
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    public struct iaabb2
    {
        public ivec2 MinInclusive = ivec2.ONE; 
        public ivec2 MaxInclusive = -ivec2.ONE; 
        
        //----------------------------------------------------------------------------------------------
        public iaabb2() { }

        //----------------------------------------------------------------------------------------------
        public bool IsValid()
        {
            return MaxInclusive >= MinInclusive; 
        }

        //----------------------------------------------------------------------------------------------
        public static iaabb2 ThatContains( ivec2 a, ivec2 b )
        {
            iaabb2 ret; 
            ret.MinInclusive = ivec2.Min( a, b ); 
            ret.MaxInclusive = ivec2.Max( a, b ); 
            return ret; 
        }

        //----------------------------------------------------------------------------------------------
        public iaabb2 GetOverlap( iaabb2 other )
        {
            iaabb2 overlap; 
            overlap.MinInclusive = ivec2.Max( MinInclusive, other.MinInclusive ); 
            overlap.MaxInclusive = ivec2.Min( MaxInclusive, other.MaxInclusive ); 
            return overlap; 
        }

        //----------------------------------------------------------------------------------------------
        public ivec2 GetSize()
        { 
            return IsValid() ? (MaxInclusive - MinInclusive + ivec2.ONE) : ivec2.ZERO; 
        }


        //----------------------------------------------------------------------------------------------
        public Int64 GetArea()
        {
            ivec2 size = GetSize(); 
            return (Int64)size.x * (Int64)size.y;
        }

        //----------------------------------------------------------------------------------------------
        public bool Intersects( iaabb2 cube )
        {
            return GetOverlap(cube).IsValid(); 
        }

        //----------------------------------------------------------------------------------------------
        private (iaabb2, iaabb2) Cut( int dim, int coord )
        {
            coord = Math.Clamp( coord, MinInclusive[dim] - 1, MaxInclusive[dim] ); 

            iaabb2 neg = this; 
            neg.MaxInclusive[dim] = coord; 

            iaabb2 pos = this; 
            pos.MinInclusive[dim] = coord + 1; 

            return (neg, pos); 
        }

        //----------------------------------------------------------------------------------------------
        // Cut functions may result in a non-valid region
        // All cut functions include the passed in coordinate as part of the first returned value (when it overlaps)
        public (iaabb2, iaabb2) CutX( int xCoord )
        {
            return Cut( 0, xCoord ); 
        }

        //----------------------------------------------------------------------------------------------
        public (iaabb2, iaabb2) CutY( int yCoord )
        {
            return Cut( 1, yCoord ); 
        }

        //----------------------------------------------------------------------------------------------
        public iaabb2[] Subtract(iaabb2 cube)
        {
            iaabb2 overlap = GetOverlap(cube); 
            if (!overlap.IsValid())
            {
                return new iaabb2[] {this}; // no overlapo, resultant cube is just the original object
            }

            List<iaabb2> cubes = new List<iaabb2>(); 
            iaabb2 original = this; 

            for (int i = 0; i < 2; ++i)
            {
                (iaabb2 toAdd, original) = original.Cut( i, overlap.MinInclusive[i] - 1 ); 
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
            return $"{MinInclusive.x}..{MaxInclusive.x}, {MinInclusive.y}..{MaxInclusive.y}";
        }
    }
}
