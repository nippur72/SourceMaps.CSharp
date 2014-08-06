// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   public abstract class SourceFileLocation 
   {
      public SourceFile sourceFile;

      public SourceFileLocation(SourceFile sourceFile) 
      {
         this.sourceFile = sourceFile;         
         //if(!isValid()) throw new Exception("SourceFileLocation is not valid");
      }

      public int line =-1;

      public abstract int offset { get; }

      public String getSourceUrl() { return sourceFile.filename; }

      public int getLine() 
      {
         if (line == -1) line = sourceFile.getLine(offset);
         return line;
      }

      public int getColumn() { return sourceFile.getColumn(getLine(), offset); }

      abstract public String getSourceName(); 

      public bool isValid() 
      {
         return offset < sourceFile.length;
      }
   }   
}
