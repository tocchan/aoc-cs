using System.Collections;
using System.Text;

namespace AoC
{
   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   public class IntHeatMap2D : IEnumerable<(ivec2, int)>, System.ICloneable
   {
      public IntHeatMap2D? HackParent = null;

      //----------------------------------------------------------------------------------------------
      public IntHeatMap2D()
      {
      }

      public IntHeatMap2D(List<string> lines)
      {
         SetFromTightBlock(lines);
      }

      public IntHeatMap2D(IntHeatMap2D src)
      {
         Resize(src.GetSize());

         BoundsValue = src.BoundsValue;
         src.Data.CopyTo(Data, 0);
      }

      //----------------------------------------------------------------------------------------------
      public IntHeatMap2D(ivec2 size, int defValue = 0, int borderValue = -1)
      {
         Init(size, defValue, borderValue);
      }

      //----------------------------------------------------------------------------------------------
      object ICloneable.Clone()
      {
         return new IntHeatMap2D(this);
      }

      //----------------------------------------------------------------------------------------------
      public IntHeatMap2D GetSubRegion(int startX, int startY, int width, int height)
      {
         IntHeatMap2D map = new IntHeatMap2D(new ivec2(width, height), 0, GetBoundsValue());
         map.Copy(this, 1, 1, width, height, 0, 0);
         return map;
      }

      //----------------------------------------------------------------------------------------------
      public void Init(int width, int height, int defValue, int borderValue = -1)
      {
         Resize(width, height);
         SetAll(defValue);
         SetBoundsValue(borderValue);
      }

      public void Init(ivec2 size, int defValue, int borderValue = -1) => Init(size.x, size.y, defValue, borderValue);

      //----------------------------------------------------------------------------------------------
      public void InitFromString(string str, int borderValue = -1)
      {
         str = str.Trim();
         String[] lines = str.Split('\n');
         for (int i = 0; i < lines.Length; ++i) {
            lines[i] = lines[i].Trim();
         }

         Init(lines[0].Length, lines.Length, 0, borderValue);

         for (int y = 0; y < lines.Length; ++y) {
            for (int x = 0; x < lines[0].Length; ++x) {
               Set(x, y, lines[y][x]); // if this asserts, the string wasn't square
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public bool ContainsPoint( ivec2 pos )
      {
         return (pos >= ivec2.ZERO) && (pos < Size); 
      }

      //----------------------------------------------------------------------------------------------
      public void Resize(int width, int height, bool keep = false)
      {
         int[] newData = new int[width * height];

         if (keep && (Data != null)) {
            int copyWidth = Math.Min(width, Size.x);
            int copyHeight = Math.Min(height, Size.y);
            for (int y = 0; y < copyHeight; ++y) {
               for (int x = 0; x < copyWidth; ++x) {
                  int srcIdx = y * Size.x + x;
                  int dstIdx = y * Size.y + x;
                  newData[dstIdx] = Data[srcIdx];
               }
            }
         }

         Size = new ivec2(width, height); 
         Data = newData;
      }

      //----------------------------------------------------------------------------------------------
      public void Resize(ivec2 size, bool keep = false) => Resize(size.x, size.y, keep);

      //----------------------------------------------------------------------------------------------
      public void Copy(IntHeatMap2D src, int dstX, int dstY)
      {
         int width = Math.Min(GetWidth() - dstX, src.GetWidth());
         int height = Math.Min(GetHeight() - dstY, src.GetHeight());

         ivec2 srcPos;
         ivec2 dstPos;
         for (int y = 0; y < height; ++y) {
            srcPos.y = y;
            dstPos.y = dstY + y;

            for (int x = 0; x < width; ++x) {
               srcPos.x = x;
               dstPos.x = x + dstX;

               int val = src.Get(srcPos);
               Set(dstPos, val);
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public void Copy(IntHeatMap2D src, int srcX, int srcY, int width, int height, int dstX, int dstY)
      {
         width = Math.Min(GetWidth() - dstX, width);
         height = Math.Min(GetHeight() - dstY, height);

         ivec2 srcPos;
         ivec2 dstPos;
         for (int y = 0; y < height; ++y) {
            srcPos.y = srcY + y;
            dstPos.y = dstY + y;

            for (int x = 0; x < width; ++x) {
               srcPos.x = srcX + x;
               dstPos.x = dstX + x;

               int val = src.Get(srcPos);
               Set(dstPos, val);
            }
         }
      }


      //----------------------------------------------------------------------------------------------
      public void SetFromTightBlock(List<string> lines, int boundsValue = int.MaxValue, int minValue = '0')
      {
         BoundsValue = boundsValue;
         int width = lines[0].Length;
         int height = lines.Count;

         Resize(width, height);

         int idx = 0;
         foreach (string line in lines) {
            foreach (char c in line) {
               Data[idx] = c - minValue;
               ++idx;
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public int GetIndex(int x, int y) => y * Size.x + x;
      public int GetIndex(ivec2 p) => GetIndex(p.x, p.y);

      public ivec2 FromIndex(int idx) => new ivec2( idx / Size.y, idx % Size.y ); 

      //----------------------------------------------------------------------------------------------
      public void Set(int x, int y, int value)
      {
         if ((y >= 0) && (y < Size.y) && (x >= 0) && (x < Size.x)) {
            int idx = GetIndex(x, y);
            Data[idx] = value;
         }
      }
      public void Set(ivec2 pos, int value) => Set(pos.x, pos.y, value);

      //----------------------------------------------------------------------------------------------
      public void Set(int offset, int val)
      {
         if ((offset >= 0) && (offset < Data.Length)) {
            Data[offset] = val;
         }
      }

      //----------------------------------------------------------------------------------------------
      public void SetAll(int value)
      {
         for (int i = 0; i < Data.Length; ++i) {
            Data[i] = value;
         }
      }

      //----------------------------------------------------------------------------------------------
      public int Get(int offset)
      {
         return ((offset >= 0) && (offset < Data.Length)) ? Data[offset] : BoundsValue;
      }


      public int Get(int x, int y)
      {
         if ((y >= 0) && (y < Size.y) && (x >= 0) && (x < Size.x)) {
            int idx = GetIndex(x, y);
            return Data[idx];
         } else {
            return BoundsValue;
         }
      }
      public int Get(ivec2 p) => Get(p.x, p.y);

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
               yield return (pos, Data[idx]);
               ++idx;
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public IEnumerable<(ivec2, int)> GetRegionEnumerator(ivec2 minInclusive, ivec2 maxInclusive)
      {
         ivec2 min = ivec2.Max(ivec2.ZERO, minInclusive);
         ivec2 max = ivec2.Min(GetSize() - ivec2.ONE, maxInclusive);

         ivec2 p;
         for (p.y = min.y; p.y <= max.y; ++p.y) {
            for (p.x = min.x; p.x <= max.x; ++p.x) {
               int val = Data[GetIndex(p)]; // no bounds check needed - I make sure all iterations are in here; 
               yield return (p, val);
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public IEnumerable<ivec2> FindLocations(Func<ivec2, bool> predicate)
      {
         ivec2 pos;
         for (pos.y = 0; pos.y < Size.y; ++pos.y) {
            for (pos.x = 0; pos.x < Size.x; ++pos.x) {
               if (predicate(pos)) {
                  yield return pos;
               }
            }
         }
      }

      //----------------------------------------------------------------------------------------------
      public ivec2? FindLocation(Func<ivec2, int, bool> search)
      {
         foreach ((ivec2 pos, int val) in this) {
            if (search(pos, val)) {
               return pos;
            }
         }

         return null;
      }

      //----------------------------------------------------------------------------------------------
      public ivec2 GetSize()
      {
         ivec2 ret;
         ret.x = Size.x;
         ret.y = Size.y;
         return ret;
      }

      //----------------------------------------------------------------------------------------------
      public int GetWidth() => Size.x;
      public int GetHeight() => Size.y;

      //----------------------------------------------------------------------------------------------
      // Operators
      public int this[int x, int y]
      {
         get => Get(x, y);
         set => Set(x, y, value);
      }

      //----------------------------------------------------------------------------------------------
      public int this[ivec2 p]
      {
         get => Get(p.x, p.y);
         set => Set(p.x, p.y, value);
      }

      //----------------------------------------------------------------------------------------------
      public int this[int offset]
      {
         get => Get(offset);
         set => Set(offset, value);
      }

      //----------------------------------------------------------------------------------------------
      // Runs a function on the map, returning the new value for each cell.
      // Value changes apply at the very end.  
      public void CellStep(Func<ivec2, int, int> func)
      {
         int[] newData = new int[Size.x * Size.y];

         int idx = 0;
         ivec2 p;
         for (p.y = 0; p.y < Size.y; ++p.y) {
            for (p.x = 0; p.x < Size.x; ++p.x) {
               newData[idx] = func(p, Data[idx]);
               ++idx;
            }
         }

         Data = newData;
      }

      //----------------------------------------------------------------------------------------------
      public IntHeatMap2D FloodFill(ivec2 start, Func<int, int> costFunc)
      {
         IntHeatMap2D fill = new IntHeatMap2D(GetSize(), -1, -1);

         fill.Set(start, 0);

         PriorityQueue<ivec2, int> points = new PriorityQueue<ivec2, int>();

         points.Enqueue(start, 0);
         while (points.Count > 0) {
            ivec2 point = points.Dequeue();
            int currentCost = fill.Get(point);

            foreach (ivec2 dir in ivec2.DIRECTIONS) {
               ivec2 next = point + dir;
               int type = Get(next);
               int cost = costFunc(type);
               if (cost >= 1) {
                  int totalCost = cost + currentCost;
                  int curCost = fill.Get(next);
                  if ((curCost < 0) || (totalCost < curCost)) {
                     fill.Set(next, totalCost);
                     points.Enqueue(next, totalCost);
                  }
               }
            }
         }

         return fill;
      }

      //----------------------------------------------------------------------------------------------
      public ivec2 GetLowestNeighbor(ivec2 p)
      {
         ivec2 lowest = p;
         int lowestVal = Get(p);

         foreach (ivec2 dir in ivec2.DIRECTIONS) {
            ivec2 newPos = dir + p;
            int v = Get(newPos);
            if ((v >= 0) && (v < lowestVal)) {
               lowestVal = v;
               lowest = newPos;
            }
         }

         return lowest;
      }

      //----------------------------------------------------------------------------------------------
      // On a flood fill map, will "fall down hill" from dst to the src point
      public List<ivec2> GetSlopePath(ivec2 dst)
      {
         List<ivec2> path = new List<ivec2>();
         if (Get(dst) < 0) {
            return path;
         }

         path.Add(dst);
         while (Get(dst) > 0) {
            dst = GetLowestNeighbor(dst);
            path.Add(dst);
         }

         path.Reverse();
         return path;
      }

      
      //----------------------------------------------------------------------------------------------
      // returns a heat map with the cost it takes to get to the end
      // cost function is the cost it takes to go from one tile to another (going _toward_ the 
      // end point provided).  Returning -1 means nont possible.
      public IntHeatMap2D DijkstraFlood(ivec2 end, Func<ivec2,ivec2,int> cost, out int[] paths) 
      {
         PriorityQueue<ivec2, int> points = new PriorityQueue<ivec2, int>();
         BitArray visited = new BitArray(Size.x * Size.y); 
         IntHeatMap2D costs = new IntHeatMap2D(Size, int.MaxValue, int.MaxValue);
         paths = new int[Size.x * Size.y]; 
         paths.SetAll(-1); 

         int srcIdx = GetIndex(end); 
         costs[srcIdx] = 0;

         // start algorithm from the end
         points.Enqueue(end, Get(end));
         while (points.Count > 0) {
            ivec2 src = points.Dequeue();
            srcIdx = GetIndex(src); 

            // only count first person to get here; 
            if (visited[srcIdx]) {
               continue; // already visited, leave
            }
            visited[srcIdx] = true; 

            int srcCost = costs[srcIdx];
            foreach (ivec2 dir in ivec2.DIRECTIONS) {
               ivec2 dst = src + dir;
               if (!ContainsPoint(dst)) {
                  continue; 
               }

               int dstIdx = GetIndex(dst);
               int dstCost = cost(dst, src); 
               int newCost = dstCost + srcCost;
               if ((dstCost >= 0) && (newCost < costs[dstIdx])) {
                  costs[dstIdx] = newCost;
                  paths[dstIdx] = srcIdx; 
                  points.Enqueue(dst, newCost);
               }
            }
         }

         return costs;         
      }

      //----------------------------------------------------------------------------------------------
      public IntHeatMap2D DijkstraFlood(ivec2 end, Func<ivec2,ivec2,int> cost)
      {
         int[] unused; 
         return DijkstraFlood(end, cost, out unused); 
      }

      //----------------------------------------------------------------------------------------------
      public List<ivec2> FindPathDijkstra(ivec2 start, ivec2 end, Func<ivec2,ivec2,int> cost) 
      {
         int[] paths; 
         DijkstraFlood(end, cost, out paths); 

         List<ivec2> path = new List<ivec2>();
         int idx = GetIndex(start); 
         if (paths[idx] < 0) {
            return path; 
         }

         path.Add(start); 
         int endIdx = GetIndex(end); 

         while (idx != endIdx) {
            idx = paths[idx]; 
            path.Add(FromIndex(idx)); 
         }
         
         return path; 
      }

      //----------------------------------------------------------------------------------------------
      public List<ivec2> FindPathDijkstra(ivec2 start, ivec2 end)
      {
         return FindPathDijkstra(start, end, (ivec2 s, ivec2 e) => Get(e)); 
      }

      //----------------------------------------------------------------------------------------------
      public int SumValuesAlong(List<ivec2> path)
      {
         int total = 0;
         foreach (ivec2 pos in path) {
            total += Get(pos.x, pos.y);
         }

         return total;
      }

      //----------------------------------------------------------------------------------------------
      public int Count(int v)
      {
         int count = 0;
         for (int i = 0; i < Data.Length; ++i) {
            if (Data[i] == v) {
               ++count;
            }
         }

         return count;
      }

      //----------------------------------------------------------------------------------------------
      public override int GetHashCode()
      {
         int hash = 0;
         foreach (int v in Data) {
            hash = HashCode.Combine(hash, v);
         }

         return hash;
      }

      //----------------------------------------------------------------------------------------------
      public override bool Equals(object? obj)
      {
         IntHeatMap2D? hm = obj as IntHeatMap2D;
         if (hm == null) {
            return false;
         }

         if (hm.GetSize() != GetSize()) {
            return false;
         }

         for (int i = 0; i < Data.Length; ++i) {
            if (Data[i] != hm.Data[i]) {
               return false;
            }
         }

         return true;
      }

      //----------------------------------------------------------------------------------------------
      public static bool IsEqual(IntHeatMap2D? a, IntHeatMap2D? b)
      {
         if ((a == null) && (b == null)) {
            return true;
         } else if ((a == null) || (b == null)) {
            return false;
         } else {
            return a.Equals(b);
         }
      }

      //----------------------------------------------------------------------------------------------
      public string ToString(int elemSize)
      {
         StringBuilder sb = new StringBuilder(); 
         int width = GetWidth(); 
         int last = width - 1; 
         int boundsValue = GetBoundsValue(); 
         string blank = string.Concat(Enumerable.Repeat("-", elemSize));

         for (int y = 0; y < GetHeight(); ++y) {
            for (int x = 0; x < width; ++x) {
               int v = Get(x, y); 

               if (v == boundsValue) {
                  sb.Append(blank); 
               } else {
                  sb.Append(v.ToString().PadLeft(elemSize)); 
               }

               if (x != last) {
                  sb.Append(','); 
               }
            }

            sb.Append('\n'); 
         }

         return sb.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string ToString()
      {
         return ToString(3); 
      }

      //----------------------------------------------------------------------------------------------
      public void SetBoundsValue(int v) => BoundsValue = v;
      public int GetBoundsValue() => BoundsValue;

      //----------------------------------------------------------------------------------------------
      private ivec2 Size = ivec2.ZERO; 
      private int[] Data = new int[0];
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
