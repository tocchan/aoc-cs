using System.Collections;
using System.Text;

namespace AoC
{
   public class IntMap
   {
      public ivec2 Size; 
      public int[] Data = new int[0]; 

      public IntMap( ivec2 size, int defValue = 0 )
      {
         Size = size; 
         int len = size.Product(); 
         Data = new int[len]; 
         Data.SetAll(defValue); 
      }

      public IntMap( string shape )
      {
         string[] lines = shape.Split('\n'); 
         Size = new ivec2(lines[0].Length, lines.Length); 
         Data = new int[Size.Product()]; 
         
         int y = 0;
         foreach (string line in lines) {
            int x = 0; 
            foreach (char c in line) {
               Set(x, y, c == ' ' ? 0 : 1); 
               ++x; 
            }
            ++y; 
         }
      }

      public int Height { get { return Size.y; } }
      public int Width { get { return Size.x; } }

      private int GetIndex( ivec2 p ) { return p.y * Width + p.x; }
      private int GetIndex( int x, int y ) { return y * Width + x; }

      public void Set( ivec2 p, int v ) => Data[GetIndex(p)] = v; 
      public void Set( int x, int y, int v ) => Data[GetIndex(x, y)] = v; 
      public int Get( ivec2 p ) => Data[GetIndex(p)]; 
      public int Get( int x, int y ) => Data[GetIndex(x, y)]; 
   }

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

      public IntCanvas(int defaultValue)
      {
         DefaultValue = defaultValue; 
      }

      //----------------------------------------------------------------------------------------------
      public IntCanvas(IntCanvas toCopy)
      {
         Min = toCopy.Min;
         Size = toCopy.Size;
         DefaultValue = toCopy.DefaultValue;
         MinSet = toCopy.MinSet; 
         MaxSet = toCopy.MaxSet; 

         Data = new int[Size.Product()];
         toCopy.Data.CopyTo(Data, 0);
      }

      //----------------------------------------------------------------------------------------------
      void Clear()
      {
         Data = new int[0];
         Min = new ivec2(0);
         Size = new ivec2(0);
         MinSet = new ivec2(int.MaxValue); 
         MaxSet = new ivec2(int.MinValue); 
      }

      //----------------------------------------------------------------------------------------------
      public void InitFromStringArray(string[] lines, Dictionary<char,int> values)
      {
         ivec2 size = new ivec2(lines[0].Length, lines.Length); 

         Clear(); 
         SetSize(size); 

         for (int y = 0; y < size.y; ++y) {
            string s = lines[y]; 
            for (int x = 0; x < size.x; ++x) {
               int v = DefaultValue;
               values.TryGetValue(s[x], out v); 

               SetValue( x, y, v ); 
            }
         }
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
      public int GetValue( ivec2 pos )
      {
         ivec2 lpos = pos - Min;
         if ((lpos >= ivec2.ZERO) && (lpos < Size)) {
            return GetLocal(lpos);
         } else {
            return DefaultValue;
         }
      }

      public int GetValue( int x, int y ) => GetValue( new ivec2( x, y ) ); 

      //----------------------------------------------------------------------------------------------
      public ivec2? GetOpenPosition(ivec2 p0, ivec2[] dirs, int openVal = 0)
      {
         foreach (ivec2 dir in dirs) {
            ivec2 np = p0 + dir; 
            if (GetValue(np) == openVal) {
               return np; 
            }
         }

         return null; 
      }

      //----------------------------------------------------------------------------------------------
      public ivec2 GetMinSetPosition() => MinSet; 
      public ivec2 GetMaxSetPosition() => MaxSet; 

      //----------------------------------------------------------------------------------------------
      // checks if this shape and map have overlap (non-zero values sharing a cell)
      public bool CollidesWith( ivec2 p, IntMap shape )
      {
         for (int y = 0; y < shape.Height; ++y) {
            for (int x = 0; x < shape.Width; ++x) {
               int mapValue = GetValue( p.x + x, p.y + y ); 
               int shapeValue = shape.Get( x, y ); 
               if ((mapValue != 0) && (shapeValue != 0)) {
                  return true; 
               }
            }
         }

         return false; 
      }

      //----------------------------------------------------------------------------------------------
      // Draws, assuming 0's are transparent
      public void DrawIntMap( ivec2 p, IntMap shape )
      {
         for (int y = 0; y < shape.Height; ++y) {
            for (int x = 0; x < shape.Width; ++x) {
               int shapeValue = shape.Get( x, y ); 
               if (shapeValue > 0) {
                  SetValue( p.x + x, p.y + y, shapeValue ); 
               }
            }
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
      public void DrawStraightPath(ivec2[] path, int val)
      {
         for (int i = 0; i < path.Length - 1; ++i) {
            DrawStraightLine(path[i], path[i + 1], val); 
         }
      }

      //----------------------------------------------------------------------------------------------
      private void GrowToFit(ivec2 min, ivec2 max)
      {
         ivec2 maxSize = Min + Size;
         if ((min >= Min) && (max < maxSize)) {
            return;
         }

         ivec2 newMin = ivec2.Min(min, Min);
         ivec2 newMax = ivec2.Max(max + ivec2.ONE, maxSize);
         ivec2 newSize = newMax - newMin;
         newSize = ivec2.Max(ivec2.CeilToPow2(newSize), new ivec2(16));

         ivec2 sizeChange = newSize - Size; 
         if (newMin.x < Min.x) {
            // grow left
            newMin.x = Min.x - sizeChange.x; 
         }
         if (newMin.y < Min.y) {
            newMin.y = Min.y - sizeChange.y; 
         }

         int[] newData = new int[newSize.Product()];
         for (int i = 0; i < newData.Length; ++i) {
            newData[i] = DefaultValue;
         }

         // copy old data ver
         ivec2 offset = Min - newMin;
         for (int y = 0; y < Size.y; ++y) {
            for (int x = 0; x < Size.x; ++x) {
               int oldIdx = y * Size.x + x;
               int newIdx = (y + offset.y) * newSize.x + (x + offset.x);
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

      //----------------------------------------------------------------------------------------------
      public string ToString(char defValue, char otherValue)
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
               sb.Append(i == DefaultValue ? defValue : otherValue);
            }
            sb.Append('\n');
         }

         return sb.ToString();
      }

      public void FloodFill(ivec2 pos, int value)
      {
         int oldValue = GetValue(pos); 
         if (oldValue == value) {
            return; 
         }

         Queue<ivec2> points = new Queue<ivec2>(); 
         points.Enqueue(pos);
         SetValue(pos, value); 

         while (points.Count > 0) {
            ivec2 point = points.Dequeue();
            SetValue(point, value); 

            foreach (ivec2 dir in ivec2.DIRECTIONS) {
               ivec2 next = point + dir;
               if (IsInBounds(next)) { 
                  int tile = GetValue(next);
                  if (tile == oldValue) {
                     SetValue(next, value); 
                     points.Enqueue(next);
                  }
               }
            }
         }
      }

      //----------------------------------------------------------------------------------------------
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
         for (int i = 0; i < 4; ++i) {
            if (GetValue(pos + ivec2.DIRECTIONS[i]) == val) {
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
      public int Automata(Func<IntCanvas, ivec2, int, int> rule)
      {
         int tilesChanged = 0;
         IntCanvas copy = new IntCanvas(this);

         ivec2 size = MaxSet - MinSet;
         for (int y = 0; y <= size.y; ++y) {
            for (int x = 0; x <= size.x; ++x) {
               ivec2 pos = MinSet + new ivec2(x, y);
               int oldValue = GetValue(pos);
               int newValue = rule(this, pos, oldValue);

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
