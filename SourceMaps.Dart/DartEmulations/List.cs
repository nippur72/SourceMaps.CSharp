// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   // emulating methods in List
   public class List<T> : System.Collections.Generic.List<T>
   {
      public int length
      {
         get { return Count; }
      }
      public bool isEmpty
      {
         get { return Count==0; }
      }
      public T last
      {
         get { return this[length-1]; }
      }
      public void add(T item)
      {
         this.Add(item);
      }   
   }
}
