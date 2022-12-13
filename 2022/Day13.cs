using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class ListTree
   {
      private int GetClosingBrace( string tok )
      {
         int open = 0; 
         for (int i = 0; i < tok.Length; ++i ) {
            if (tok[i] == '[') {
               ++open; 
            } else if (tok[i] == ']') {
               --open; 
               if (open == 0) {
                  return i; 
               }
            }
         }

         return -1; 
      }

      private bool GetNextChild( out string child, ref string token )
      {
         child = ""; 
         if (token.Length == 0) {
            return false; 
         }

         if (token[0] == ',') {
            token = token.Substring(1); 
         }

         if (token[0] == '[') {
            int closeIdx = GetClosingBrace(token); 
            child = token.Substring(0, closeIdx + 1); 
            token = token.Substring(closeIdx + 1); 
            return true; 
         }

         int idx = token.IndexOf(','); 
         if (idx >= 0) {
            child = token.Substring(0, idx); 
            token = token.Substring(idx + 1); 
         } else {
            child = token; 
            token = ""; 
         }
         return true; 
      }

      public ListTree( string val )
      {
         val = val.TrimStart(); 
         if (val.Length == 0) {
            return; 
         }

         if (val[0] == '[') {
            val = val.Substring(1, val.Length - 2); 
         }

         if (val.Length == 0) {
            return; 
         }

         string child; 
         while (GetNextChild(out child, ref val)) { 
            if (child[0] == '[') {
               Children.Add( new ListTree(child) ); 
            } else {
               Children.Add( new ListTree( int.Parse(child) ));
            }
         }
      }

      public ListTree( int v )
      {
         Value = v; 
      }

      public ListTree( ListTree child )
      {
         Value = null; 
         Children.Add(child); 
      }

      static public int Compare( ListTree lh, ListTree rh )
      {
         if (lh.IsValue() && rh.IsValue()) {
            return Math.Sign(lh.Value!.Value - rh.Value!.Value); 
         }

         if (lh.IsList() && rh.IsList()) {
            int len = Math.Min(lh.Children.Count, rh.Children.Count); 
            for (int i = 0; i < len; ++i) {
               int c = Compare(lh.Children[i], rh.Children[i]); 
               if (c != 0) {
                  return c; 
               }
            }
            return Math.Sign(lh.Children.Count - rh.Children.Count); 
         }

         if (lh.IsValue()) {
            if (rh.IsEmpty()) {
               return 1; 
            } else {
               int c =  Compare(lh, rh.Children[0]); 
               if ((c == 0) && (rh.Children.Count > 1)) {
                  return -1; 
               }
               return c; 
            }
         } else {  // rh.IsValue()
            if (lh.IsEmpty()) {
               return -1; 
            } else {
               int c = Compare(lh.Children[0], rh); 
               if ((c == 0) && (lh.Children.Count > 1)) {
                  return 1; 
               }
               return c; 
            }
         }

         /*
         if (lh.IsValue()) {
            ListTree nlh = new ListTree(lh); 
            return Compare(nlh, rh); 
         } else {
            ListTree nrh = new ListTree(rh); 
            return Compare(lh, nrh); 
         }
         */
      }

      public bool IsValue() => Value.HasValue; 
      public bool IsList() => !Value.HasValue; 
      public bool IsEmpty() => IsList() && (Children.Count == 0); 

      public int? GetChildValue()
      {
         if (IsValue()) {
            return Value; 
         } else if (Children.Count > 0) {
            return Children[0].GetChildValue(); 
         } else {
            return null; 
         }
      }

      public List<ListTree> Children = new(); 
      public int? Value; 
      public string Debug = ""; 
   }

   internal class Day13 : Day
   {
      private string InputFile = "2022/inputs/13.txt";

      List<ListTree> Trees = new(); 
      

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         for (int i = 0; i < lines.Count; i += 3) {
            ListTree lh = new ListTree(lines[i + 0]); 
            ListTree rh = new ListTree(lines[i + 1]); 

            lh.Debug = lines[i + 0]; 
            rh.Debug = lines[i + 1]; 

            Trees.Add(lh); 
            Trees.Add(rh); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         int sum = 0; 
         for (int i = 0; i < Trees.Count; i += 2) {
            int c = ListTree.Compare(Trees[i], Trees[i + 1]); 
            if (c == 0) {
               Util.WriteLine("Boo"); 
            }

            int idx = (i / 2) + 1; 
            Util.WriteLine( $"\n{idx}: \n {Trees[i].Debug}\n {Trees[i + 1].Debug}\n ..." + ((c < 0) ? "right order" : "wrong order") ); 

            if (c < 0) { 
               sum += idx; 
            }
         }

         return sum.ToString();
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         string s0 = "[[2]]"; 
         string s1 = "[[6]]"; 

         ListTree sp0 = new ListTree(s0); 
         sp0.Debug = s0; 

         ListTree sp1 = new ListTree(s1); 
         sp1.Debug = s1; 

         Trees.Add( sp0 ); 
         Trees.Add( sp1 ); 

         Trees.Sort( ListTree.Compare ); 
         
         int idx0 = 0; 
         int idx1 = 0; 
         for (int i = 0; i < Trees.Count; ++i) {
            if (Trees[i].Debug == s0) {
               idx0 = i + 1; 
            } else if (Trees[i].Debug == s1) {
               idx1 = i + 1; 
            }
         }

         return (idx0 * idx1).ToString(); 
      }
   }
}
