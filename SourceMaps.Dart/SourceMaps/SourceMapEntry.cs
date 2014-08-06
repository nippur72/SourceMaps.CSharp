// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   public class SourceMapEntry 
   {
      public SourceFileLocation sourceLocation;
      public int targetOffset;

      public SourceMapEntry(SourceFileLocation sourceLocation, int targetOffset)
      {
         this.sourceLocation = sourceLocation;
         this.targetOffset   = targetOffset;
      }
   }  
}
