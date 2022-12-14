using System.Collections;
using System.Text;

namespace AoC
{
   //----------------------------------------------------------------------------------------------
   // Doing this as a infinite sparse tiled grid would be awesome,
   // but for now, just going to make a growable sheet of paper, anytime we add a point, expand it to fit that new point
   // and copy the old 
   //----------------------------------------------------------------------------------------------
   public class IntCanvas : IEnumerable<(ivec2, int)>, System.ICloneable
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
      public IntCanvas(IntCanvas toCopy)
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
      IEnumerator<(ivec2, int)> IEnumerable<(ivec2, int)>.GetEnumerator()
      {
         ivec2 pos;
         int idx = 0;
         for (pos.y = 0; pos.y < Size.y; ++pos.y) {
            for (pos.x = 0; pos.x < Size.x; ++pos.x) {
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
      public ivec2 GetMaxSetPosition() => MaxSet; 

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
      public void DrawStraightLine(ivec2 p0, ivec2 p1, int val)
      {
         ivec2 dir = ivec2.Sign(p1 - p0); 
         ivec2 p = p0; 
         while (p != p1) {
            SetValue(p, val); 
            p += dir; 
         }
         SetValue(p1, val); 
      }

      //----------------------------------------------------------------------------------------------
      private void GrowToFit(ivec2 min, ivec2 max)
      {
         ivec2 maxSize = Min + Size;
         if ((min >= Min) && (max < maxSize)) {
            return;
         }

         ivec2 newMin = ivec2.Min(min, Min);
         ivec2 newMax = ivec2.Max(max, maxSize);
         ivec2 newSize = newMax - newMin + ivec2.ONE;
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
      private void GrowToFit(ivec2 pos)
      {
         if ((pos >= Min) && (pos < (Min + Size))) {
            return;
         }

         GrowToFit(pos, pos);

         ivec2 newMin = ivec2.Min(pos, Min);
         ivec2 newMax = ivec2.Max(pos, Min + Size + ivec2.ONE);

      }

      //----------------------------------------------------------------------------------------------
      public void Reserve(ivec2 min, ivec2 max)
      {
         GrowToFit(min, max);
      }

      //----------------------------------------------------------------------------------------------
      // Makes 
      public void SetSize(ivec2 size, ivec2 origin = default)
      {
         // may seem weird to get and just set it, but this will ensure 
         // min-set and max-set are set, and if they were _already_ set, will preserve their
         // value.  If not set, will result in being the default value.
         int val = GetValue(origin); 
         SetValue( origin, val ); 

         ivec2 max = origin + size - ivec2.ONE; 
         val = GetValue( max ); 
         SetValue( max, val ); 
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
      public string ToString(string pal)
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

      private bool IsInBounds(ivec2 p)
      {
         return (p >= MinSet) && (p <= MaxSet);
      }

      //----------------------------------------------------------------------------------------------
      private void EnqueueDir(PriorityQueue<ivec3, int> toVisit, IntCanvas visited, int v, ivec2 pos, Func<ivec2, int, int> h)
      {
         // this is a wall
         int w = h(pos, GetValue(pos));
         if (w < 0) {
            return;
         }
         w = v + w + 1; ; // always be at least one; 

         // hasn't been searched, so we can search it soon
         if (v < visited.GetValue(pos)) {
            toVisit.Enqueue(new ivec3(pos, w), w);
         }
      }

      //----------------------------------------------------------------------------------------------
      private ivec2 GetPreviousTile(IntCanvas weights, ivec2 p)
      {
         int minIdx = 0;
         int minWeight = int.MaxValue;


         ivec2[] dirs = { ivec2.RIGHT, ivec2.LEFT, ivec2.UP, ivec2.DOWN };
         for (int i = 0; i < 4; ++i) {
            int w = weights.GetValue(p + dirs[i]);
            if (w < minWeight) {
               minIdx = i;
               minWeight = w;
            }
         }

         return p + dirs[minIdx];
      }

      //----------------------------------------------------------------------------------------------
      private List<ivec2> ComputePath(ivec2 end, IntCanvas weights)
      {
         List<ivec2> path = new List<ivec2>();

         ivec2 cursor = end;
         while (weights.GetValue(cursor) != 1) {
            ivec2 prev = GetPreviousTile(weights, cursor);
            path.Add(cursor - prev);
            cursor = prev;
         }

         path.Reverse();
         return path;
      }

      //----------------------------------------------------------------------------------------------
      public List<ivec2> FindPathTo(ivec2 start, Func<ivec2, int, bool> check, Func<ivec2, int, int> h)
      {
         IntCanvas visited = new IntCanvas();
         visited.DefaultValue = int.MaxValue;
         visited.Reserve(MinSet, MaxSet);

         PriorityQueue<ivec3, int> toVisit = new PriorityQueue<ivec3, int>();
         toVisit.Enqueue(new ivec3(start, 1), 1);

         while (toVisit.Count > 0) {
            ivec3 tile = toVisit.Dequeue();
            ivec2 pos = tile.xy;

            // already visited this tile
            if (visited.GetValue(pos) != int.MaxValue) {
               continue;
            }

            if (check(pos, GetValue(pos))) {
               // found our destination, fall back to the origin
               return ComputePath(pos, visited);
            }

            // mark this as visited, and continue the algorithm
            visited.SetValue(pos, tile.z); // set the weight at this tile (we can eventually just "fall down the hill" to our destination
            EnqueueDir(toVisit, visited, tile.z, pos + ivec2.RIGHT, h);
            EnqueueDir(toVisit, visited, tile.z, pos + ivec2.LEFT, h);
            EnqueueDir(toVisit, visited, tile.z, pos + ivec2.UP, h);
            EnqueueDir(toVisit, visited, tile.z, pos + ivec2.DOWN, h);
         }

         return new List<ivec2>();
      }

      //----------------------------------------------------------------------------------------------
      public List<ivec2> FindPathTo(ivec2 start, ivec2 end, Func<ivec2, int, int> h)
      {
         return FindPathTo(start, bool (ivec2 pos, int val) => pos == end, h);
      }

      //----------------------------------------------------------------------------------------------
      public bool HasNeighbor(ivec2 pos, int val)
      {
         ivec2[] dirs = { ivec2.RIGHT, ivec2.LEFT, ivec2.UP, ivec2.DOWN };
         for (int i = 0; i < 4; ++i) {
            if (GetValue(pos + dirs[i]) == val) {
               return true;
            }
         }

         return false;
      }

      //----------------------------------------------------------------------------------------------
      public ivec2 GetModifiedSize() => MaxSet - MinSet + ivec2.ONE;

      //----------------------------------------------------------------------------------------------
      // Runs a function on all set values.  All gets during the callback
      // use the previous value, and will set a new value.
      public int Automata(Func<ivec2, int, int> rule)
      {
         int tilesChanged = 0;
         IntCanvas copy = new IntCanvas(this);

         ivec2 size = MaxSet - MinSet;
         for (int y = 0; y <= size.y; ++y) {
            for (int x = 0; x <= size.x; ++x) {
               ivec2 pos = MinSet + new ivec2(x, y);
               int oldValue = GetValue(pos);
               int newValue = rule(pos, oldValue);

               if (newValue != oldValue) {
                  copy.SetValue(pos, newValue);
                  ++tilesChanged;
               }
            }
         }

         copy.Data.CopyTo(Data, 0);
         return tilesChanged;
      }
   }
}
