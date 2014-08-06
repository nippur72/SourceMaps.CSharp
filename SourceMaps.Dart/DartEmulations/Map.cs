// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   // maps are Dart equivalent of dictionary
   public class Map<K,V> : Dictionary<K,V>
   {
      public V putIfAbsent(K value, Func<V> func)
      {
         if(!this.ContainsKey(value))
         {
            this[value] = func();
         }
         return this[value];
      }
   } 
}
