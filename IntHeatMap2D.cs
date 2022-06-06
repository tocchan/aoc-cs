using System.Collections;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    public class IntHeatMap2D : IEnumerable<(ivec2,int)>, System.ICloneable
    {
        public IntHeatMap2D? HackParent = null; 

        //----------------------------------------------------------------------------------------------
        public IntHeatMap2D()
        {
        }

        public IntHeatMap2D( List<string> lines )
        {
            SetFromTightBlock( lines ); 
        }

        public IntHeatMap2D( IntHeatMap2D src )
        {
            Resize( src.GetSize() ); 

            BoundsValue = src.BoundsValue; 
            src.Data.CopyTo( Data, 0 ); 
        }

        public IntHeatMap2D( ivec2 size, int defValue = 0, int borderValue = -1 )
        {
            Init( size, defValue, borderValue );
        }

        //----------------------------------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return new IntHeatMap2D(this);
        }

        public IntHeatMap2D GetSubRegion( int startX, int startY, int width, int height )
        {
            IntHeatMap2D map = new IntHeatMap2D( new ivec2(width, height), 0, GetBoundsValue() ); 
            map.Copy( this, 1, 1, width, height, 0, 0 ); 
            return map; 
        }

        //----------------------------------------------------------------------------------------------
        public void Init( int width, int height, int defValue, int borderValue = -1 )
        {
            Resize( width, height ); 
            SetAll( defValue ); 
            SetBoundsValue( borderValue ); 
        }

        public void Init( ivec2 size, int defValue, int borderValue = -1 ) => Init( size.x, size.y, defValue, borderValue ); 

        //----------------------------------------------------------------------------------------------
        public void Resize( int width, int height, bool keep = false )
        {
            int[] newData = new int[width * height]; 

            if (keep && (Data != null))
            {
                int copyWidth = Math.Min( width, Width ); 
                int copyHeight = Math.Min( height, Height ); 
                for (int y = 0; y < copyHeight; ++y)
                {
                    for (int x = 0; x < copyWidth; ++x)
                    {
                        int srcIdx = y * Width + x; 
                        int dstIdx = y * width + x; 
                        newData[dstIdx] = Data[srcIdx]; 
                    }
                }
            }

            Width = width; 
            Height = height; 
            Data = newData; 
        }
        public void Resize( ivec2 size, bool keep = false ) => Resize( size.x, size.y, keep ); 

        //----------------------------------------------------------------------------------------------
        public void Copy( IntHeatMap2D src, int dstX, int dstY )
        {
            int width = Math.Min( GetWidth() - dstX, src.GetWidth() ); 
            int height = Math.Min( GetHeight() - dstY, src.GetHeight() ); 

            ivec2 srcPos; 
            ivec2 dstPos; 
            for (int y = 0; y < height; ++y)
            {
                srcPos.y = y; 
                dstPos.y = dstY + y; 

                for (int x = 0; x < width; ++x)
                {
                    srcPos.x = x; 
                    dstPos.x = x + dstX; 

                    int val = src.Get(srcPos); 
                    Set(dstPos, val); 
                }
            }
        }
        public void Copy( IntHeatMap2D src, int srcX, int srcY, int width, int height, int dstX, int dstY )
        {
            width = Math.Min( GetWidth() - dstX, width ); 
            height = Math.Min( GetHeight() - dstY, height ); 

            ivec2 srcPos; 
            ivec2 dstPos; 
            for (int y = 0; y < height; ++y)
            {
                srcPos.y = srcY + y; 
                dstPos.y = dstY + y; 

                for (int x = 0; x < width; ++x)
                {
                    srcPos.x = srcX + x; 
                    dstPos.x = dstX + x; 

                    int val = src.Get(srcPos); 
                    Set(dstPos, val); 
                }
            }
        }


        //----------------------------------------------------------------------------------------------
        public void SetFromTightBlock( List<string> lines, int boundsValue = int.MaxValue )
        {
            BoundsValue = boundsValue; 
            int width = lines[0].Length; 
            int height = lines.Count; 

            Resize( width, height ); 

            int idx = 0; 
            foreach (string line in lines)
            {
                foreach (char c in line)
                {
                    Data[idx] = c - '0'; 
                    ++idx; 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        private int GetIndex( int x, int y )
        {
            return y * Width + x; 
        }
        private int GetIndex( ivec2 p ) => GetIndex( p.x, p.y ); 

        //----------------------------------------------------------------------------------------------
        public void Set( int x, int y, int value )
        {
            if ((y >= 0) && (y < Height) && (x >= 0) && (x < Width))
            {
                int idx = GetIndex(x, y); 
                Data[idx] = value;
            }
        }
        public void Set( ivec2 pos, int value ) => Set( pos.x, pos.y, value ); 

        //----------------------------------------------------------------------------------------------
        public void SetAll( int value )
        {
            for (int i = 0; i < Data.Length; ++i)
            {
                Data[i] = value; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public int Get( int x, int y )
        {
            if ((y >= 0) && (y < Height) && (x >= 0) && (x < Width))
            {
                int idx = GetIndex(x, y); 
                return Data[idx]; 
            }
            else
            {
                return BoundsValue; 
            }
        }
        public int Get( ivec2 p ) => Get( p.x, p.y ); 

        //----------------------------------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        //----------------------------------------------------------------------------------------------
        IEnumerator<(ivec2,int)> IEnumerable<(ivec2,int)>.GetEnumerator()
        {
            ivec2 pos; 
            int idx = 0; 
            for (pos.y = 0; pos.y < Height; ++pos.y)
            {
                for (pos.x = 0; pos.x < Width; ++pos.x)
                {
                    yield return (pos, Data[idx]); 
                    ++idx; 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        public IEnumerable<(ivec2,int)> GetRegionEnumerator( ivec2 minInclusive, ivec2 maxInclusive )
        {
            ivec2 min = ivec2.Max( ivec2.ZERO, minInclusive ); 
            ivec2 max = ivec2.Min( GetSize() - ivec2.ONE, maxInclusive ); 

            ivec2 p; 
            for (p.y = min.y; p.y <= max.y; ++p.y)
            {
                for (p.x = min.x; p.x <= max.x; ++p.x)
                {
                    int val = Data[GetIndex(p)]; // no bounds check needed - I make sure all iterations are in here; 
                    yield return (p, val); 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        public ivec2 GetSize()
        {
            ivec2 ret; 
            ret.x = Width; 
            ret.y = Height; 
            return ret; 
        }

        //----------------------------------------------------------------------------------------------
        public int GetWidth() => Width; 
        public int GetHeight() => Height; 

        public int this[int x, int y]
        {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public int this[ivec2 p]
        {
            get => Get(p.x, p.y); 
            set => Set(p.x, p.y, value); 
        }

        //----------------------------------------------------------------------------------------------
        // Runs a function on the map, returning the new value for each cell.
        // Value changes apply at the very end.  
        public void CellStep( Func<ivec2, int, int> func )
        {
            int[] newData = new int[Width * Height]; 

            int idx = 0; 
            ivec2 p; 
            for (p.y = 0; p.y < Height; ++p.y)
            {
                for (p.x = 0; p.x < Width; ++p.x)
                {
                    newData[idx] = func(p, Data[idx]); 
                    ++idx; 
                }
            }

            Data = newData; 
        }

        //----------------------------------------------------------------------------------------------
        public List<ivec2> FindPathDijkstra(ivec2 start, ivec2 end)
        {
            PriorityQueue<ivec2, int> points = new PriorityQueue<ivec2, int>(); 
            ivec2[] prevMap = new ivec2[Width * Height]; 
            int[] costs = new int[Width * Height]; 

            for (int i = 0; i < Width * Height; ++i)
            {
                costs[i] = int.MaxValue; 
            }

            prevMap[ GetIndex(end) ] = end; 
            costs[GetIndex(end)] = 0; 

            ivec2[] dirs =
            {
                ivec2.LEFT, 
                ivec2.RIGHT, 
                ivec2.UP,
                ivec2.DOWN, 
            };

            // Start the algorithm - from the end, so I don't have to reverse it when I finish
            points.Enqueue( end, Get(end) ); 
            while (points.Count > 0)
            {
                ivec2 src = points.Dequeue(); 
                if (src == start)
                {
                    break; 
                }

                int srcIdx = GetIndex(src); 
                int srcCost = costs[srcIdx]; 
                foreach (ivec2 dir in dirs)
                {
                    ivec2 dst = src + dir; 
                    int dstIdx = GetIndex(dst); 
                    int dstCost = Get(dst); 
                    if ((dstCost != BoundsValue) && (costs[dstIdx] == int.MaxValue))
                    {
                        dstCost += srcCost; 
                        costs[dstIdx] = dstCost;
                        prevMap[dstIdx] = src; 

                        points.Enqueue( dst, dstCost ); 
                    }
                }
            }

            // okay, have my path, follow it to the start
            List<ivec2> path = new List<ivec2>(); 

            ivec2 pos = start; 
            path.Add(pos); 

            while (prevMap[GetIndex(pos)] != pos)
            {
                pos = prevMap[GetIndex(pos)]; 
                path.Add(pos); 
            }

            return path; 
        }

        //----------------------------------------------------------------------------------------------
        public int SumValuesAlong( List<ivec2> path )
        {
            int total = 0; 
            foreach (ivec2 pos in path)
            {
                total += Get( pos.x, pos.y ); 
            }

            return total; 
        }

        //----------------------------------------------------------------------------------------------
        public int Count( int v )
        {
            int count = 0; 
            for (int i = 0; i < Data.Length; ++i)
            {
                if (Data[i] == v)
                {
                    ++count;
                }
            }

            return count;
        }

        //----------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            int hash = 0; 
            foreach (int v in Data)
            {
                hash = HashCode.Combine( hash, v ); 
            }

            return hash; 
        }

        //----------------------------------------------------------------------------------------------
        public override bool Equals(object? obj)
        {
            IntHeatMap2D? hm = obj as IntHeatMap2D;
            if (hm == null)
            { 
                return false; 
            }
            
            if (hm.GetSize() != GetSize())
            {
                return false; 
            }

            for (int i = 0; i < Data.Length; ++i)
            {
                if (Data[i] != hm.Data[i])
                {
                    return false; 
                }
            }

            return true; 
        }

        public static bool IsEqual( IntHeatMap2D? a, IntHeatMap2D? b )
        {
            if ((a == null) && (b == null))
            {
                return true; 
            }
            else if ((a == null) || (b == null))
            {
                return false; 
            }
            else
            {
                return a.Equals(b); 
            }
        }

        //----------------------------------------------------------------------------------------------
        public void SetBoundsValue( int v ) => BoundsValue = v; 
        public int GetBoundsValue() => BoundsValue;

        //----------------------------------------------------------------------------------------------
        private int[] Data = new int[0];  
        private int Width = 0; 
        private int Height = 0; 
        private int BoundsValue = int.MaxValue;
    }

    public class IntHeatMap2DComparer : IEqualityComparer<IntHeatMap2D>
    {
        public bool Equals(IntHeatMap2D? a, IntHeatMap2D? b)
        {
            return IntHeatMap2D.IsEqual(a, b); 
        }

        public int GetHashCode(IntHeatMap2D hm)
        {
            return hm.GetHashCode(); 
        }
    }

}
