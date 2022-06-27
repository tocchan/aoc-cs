using System.Collections;
using System.Text;

namespace AoC2021
{
    //----------------------------------------------------------------------------------------------
    // Doing this as a infinite sparse tiled grid would be awesome,
    // but for now, just going to make a growable sheet of paper, anytime we add a point, expand it to fit that new point
    // and copy the old 
    //----------------------------------------------------------------------------------------------
    public class IntCanvas : IEnumerable<(ivec2,int)>, System.ICloneable
    {
        //----------------------------------------------------------------------------------------------
        private int[] Data = new int[0];  
        private ivec2 Min = new ivec2(0); 
        private ivec2 Size = new ivec2(0); 
        private int DefaultValue = 0;

        ivec2 MinSet = new ivec2(int.MaxValue); 
        ivec2 MaxSet = new ivec2(int.MinValue);

        //----------------------------------------------------------------------------------------------
        public IntCanvas()
        { 
        }

        //----------------------------------------------------------------------------------------------
        public IntCanvas( IntCanvas toCopy )
        {
            Min = toCopy.Min; 
            Size = toCopy.Size; 
            DefaultValue = toCopy.DefaultValue;

            Data = new int[Size.Product()]; 
            toCopy.Data.CopyTo(Data, 0); 
        }

         //----------------------------------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return new IntCanvas(this);
        }

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
            for (pos.y = 0; pos.y < Size.y; ++pos.y)
            {
                for (pos.x = 0; pos.x < Size.x; ++pos.x)
                {
                    yield return (pos + Min, Data[idx]); 
                    ++idx; 
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        public int GetValue(ivec2 pos)
        {
            ivec2 lpos = pos - Min; 
            if ((lpos >= ivec2.ZERO) && (lpos < Size)) {
                return GetLocal(lpos); 
            } else {
                return DefaultValue; 
            }
        }

        //----------------------------------------------------------------------------------------------
        public int SetValue(ivec2 pos, int val)
        {
            GrowToFit(pos); 

            MinSet = ivec2.Min(pos, MinSet); 
            MaxSet = ivec2.Max(pos, MaxSet); 

            ivec2 lpos = pos - Min; 
            return SetLocal(lpos, val); 
        }

        public int SetValue(int x, int y, int val) => SetValue(new ivec2(x, y), val); 

        //----------------------------------------------------------------------------------------------
        private void GrowToFit(ivec2 pos)
        {
            if ((pos >= Min) && (pos < (Min + Size))) {
                return; 
            }

            ivec2 newMin = ivec2.Min(pos, Min); 
            ivec2 newMax = ivec2.Max(pos, Min + Size + ivec2.ONE); 
            ivec2 newSize = newMax - newMin; 

            newMin = ivec2.FloorToBoundary(newMin, 64); 
            newSize = ivec2.CeilToBoundary(newSize, 64); 


            int[] newData = new int[newSize.Product()]; 
            for (int i = 0; i < newData.Length; ++i) {
                newData[i] = DefaultValue; 
            }

            // copy old data ver
            ivec2 oldOffset = Min - newMin; 
            for (int y = 0; y < Size.y; ++y) {
                for (int x = 0; x < Size.x; ++x) {
                    int oldIdx = y * Size.x + x; 
                    int newIdx = (y + oldOffset.y) * newSize.x + (x + oldOffset.x); 
                    newData[newIdx] = Data[oldIdx]; 
                }
            }

            Min = newMin; 
            Size = newSize; 
            Data = newData; 
        }

        //----------------------------------------------------------------------------------------------
        private int GetIndex(ivec2 lpos)
        {
            return lpos.y * Size.x + lpos.x; 
        }

        //----------------------------------------------------------------------------------------------
        private int GetLocal(ivec2 lpos) 
        { 
            int idx = GetIndex(lpos); 
            return Data[idx]; 
        }

        //----------------------------------------------------------------------------------------------
        private int SetLocal(ivec2 lpos, int val) 
        {
            int idx = GetIndex(lpos); 
            int oldVal = Data[idx]; 
            Data[idx] = val;

            return oldVal;
        }

        //----------------------------------------------------------------------------------------------
        public int Count(int val)
        {
            if (val == DefaultValue) {
                return int.MaxValue;
            }

            int count = 0; 
            for (int i = 0; i < Data.Length; ++i) {
                if (Data[i] == val) {
                    ++count;
                }
            }

            return count; 
        }


        //----------------------------------------------------------------------------------------------
        public string ToString( string pal )
        {
            // nothing was set in this canvas
            if (MinSet > MaxSet) {
                return "<empty>"; 
            }

            ivec2 size = MaxSet - MinSet + ivec2.ONE; 

            StringBuilder sb = new StringBuilder(); 
            
            int idx = 0; 
            for (int y = 0; y < size.y; ++y) {
                for (int x = 0; x < size.x; ++x) {
                    ivec2 pos = MinSet + new ivec2(x, y); 
                    int i = GetValue(pos); 
                    ++idx; 
                    sb.Append(pal[i]); 
                }
                sb.Append('\n'); 
            }

            return sb.ToString(); 
        }
    }
}
