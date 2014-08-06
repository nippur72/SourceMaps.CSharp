// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   // Note: to keep changes to original code as minimum as possible, some classes from the 
   // Dart typesystem are emulated instead of replacing directly in the code. Once sourcemaps
   // are proved to work, replacement with .NET types can occur.

   // StringBuffer is Dart equivalent of StringBuilder
   public class StringBuffer
   {
      private StringBuilder _sb = new StringBuilder();

      public void write(string s)
      {
         _sb.Append(s);
      }
      public void write(StringBuffer s)
      {
         _sb.Append(s.toString());
      }
      public void write(char s)
      {
         _sb.Append(s.ToString());
      }
      public String toString()
      {
         return _sb.ToString();
      }
   }
}
