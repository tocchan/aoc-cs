using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   internal class Day20 : Day
   {
      private string InputFile = "2022/inputs/20.txt";
      
      //----------------------------------------------------------------------------------------------
      //----------------------------------------------------------------------------------------------
      internal class llp 
      {
         public llp? prev = null; 
         public llp? next = null; 
         public Int64 value; 

         public llp(Int64 v)
         {
            value = v; 
         }

         public llp get_prev( Int64 count = 1 )
         {
            if (count < 0) {
               return get_next(-count); 
            }

            llp? iter = this; 
            while (count > 0) {
               --count; 
               iter = iter!.prev; 
            }

            return iter!; 
         }

         public llp get_next( Int64 count = 1 ) 
         {
            if (count < 0) {
               return get_prev(-count); 
            }

            llp? iter = this; 
            while (count > 0) {
               --count; 
               iter = iter!.next; 
            }

            return iter!; 
         }

         public override string ToString()
         {
            return $"{value}  ({prev!.value} <- o -> {next!.value})"; 
         }

         public Int64 GetCount()
         {
            llp? iter = this; 

            Int64 count = 0; 
            do {
               ++count; 
               iter = iter!.next; 
            } while (iter != this); 

            return count;
         }
      }


      //----------------------------------------------------------------------------------------------
      llp? Append( llp? head, llp? item )
      {
         if (head == null) {
            head = item; 
            item!.prev = item; 
            item!.next = item; 
            return item; 
         }

         item!.prev = head!.prev; 
         head!.prev = item; 
         item!.next = head; 
         item!.prev!.next = item; 

         return head; 
      }

      //----------------------------------------------------------------------------------------------
      llp? Remove( llp? head, llp? item )
      {
         llp next = item!.next!; 

         item.prev!.next = item.next; 
         item.next!.prev = item.prev; 
         item.prev = null; 
         item.next = null;
         
         if (item == head) {
            if (item == next) {
               return null; 
            } else {
               return next; 
            }
         } else {
            return head; 
         }
      }

      //----------------------------------------------------------------------------------------------
      llp? InsertBefore( llp? head, llp? item, llp? before )
      {
         llp? oldPrev = 
         item!.prev = before!.prev; 
         before.prev = item; 
         item.next = before; 
         item.prev!.next = item; 

         if (item == head) {
            return item; 
         } else {
            return head; 
         }
      }

      

      //----------------------------------------------------------------------------------------------
      public llp? Head; 
      public llp? Start; 
      public List<llp> Values = new(); 

      public llp? Head2; 
      public llp? Start2; 
      public List<llp> Values2 = new(); 

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);
         Int64[] nums = lines.Select(Int64.Parse).ToArray(); 

         Int64 key = 811589153; 
         foreach (Int64 n in nums) {
            llp lp = new llp(n); 
            llp lp2 = new llp(n * key);

            if (n == 0) {
               Start = lp;
               Start2 = lp2; 
            } 

            Values.Add(lp); 
            Values2.Add(lp2); 

            Head = Append( Head, lp ); 
            Head2 = Append( Head2, lp2 ); 
         }
      }

      //----------------------------------------------------------------------------------------------
      void PrintList( llp? head )
      {
         string result = ""; 
         llp? iter = head; 
         do {
            result += iter!.value.ToString() + ", "; 
            iter = iter!.next; 
         } while (iter != head);

         Util.WriteLine(result); 
      }

      //----------------------------------------------------------------------------------------------
      private llp? Mix( List<llp> values, llp? head ) 
      {
         Int64 moveCount = values.Count - 1; 
         Int64 halfCount = values.Count / 2; 
         foreach (llp v in values) {
            Int64 move = v.value; 
            
            // figure out the least I have to move
            Int64 nm = move % moveCount; 
            if (nm <= -halfCount) {
               nm += moveCount;  
            } else if (nm > halfCount) {
               nm -= moveCount; 
            }
            move = nm; 

            // get where I want to insert at
            llp? ins = v.next; 
            head = Remove(head, v); 

            // get the item to insert be fore
            if (move > 0) {
               ins = ins!.get_next(move);  
            } else if (move < 0) {
               ins = ins!.get_prev(-move);
            }

            // insert it back
            head = InsertBefore(head, v, ins);
         }

         return head; 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         //PrintList(Head); 
         Mix(Values, Head); 

         // okay, from 0, compute how far I have to move.  
         llp n0 = Start!.get_next(1000); 
         llp n1 = Start!.get_next(2000); 
         llp n2 = Start!.get_next(3000); 
         Int64 result = n0.value + n1.value + n2.value; 
         return result.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         for (int i = 0; i < 10; ++i) {
            Mix( Values2, Head2 ); 
         }
         
         // okay, from 0, compute how far I have to move.  
         llp n0 = Start2!.get_next(1000); 
         llp n1 = Start2!.get_next(2000); 
         llp n2 = Start2!.get_next(3000); 
         Int64 result = n0.value + n1.value + n2.value; 
         return result.ToString(); 
      }
   }
}
