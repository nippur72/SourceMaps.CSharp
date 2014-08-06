// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   public class SourceFile
   {
      public String filename;
      
      public String content;

      private List<int> lineStarts;

      private List<int> lineStartsFromString(String text) 
      {
         var starts = new List<int>();
         starts.Add(0);
         var index = 0;
         while (index < text.Length) {
            index = text.IndexOf('\n', index) + 1;
            if (index <= 0) break;
            starts.add(index);
         }
         starts.add(text.Length + 1); // One additional line start at the end.
         return starts;
      }

      public SourceFile(String filename, String content)
      {
         this.filename = filename;
         this.content = content;
         lineStarts = lineStartsFromString(content);
      }

      /**
      * Returns the line number for the offset [position] in the string
      * representation of this source file.
      */
      public int getLine(int position) 
      {
         List<int> starts = lineStarts;
         if (position < 0 || starts.last <= position) 
         {
            throw new Exception(string.Format("bad position #{0} in file {1} with length {2}.", position, filename, length));
         }
      
         int first = 0;
         int count = starts.length;
         while (count > 1) 
         {
            int step = count / 2;
            int middle = first + step;
            int lineStart = starts[middle];
            if (position < lineStart) 
            {
               count = step;
            } 
            else 
            {
               first = middle;
               count -= step;
            }
         }
         return first;
      }

      /**
      * Returns the column number for the offset [position] in the string
      * representation of this source file.
      */
      public int getColumn(int line, int position) 
      {
         return position - lineStarts[line];
      }
   
      public int length
      {
         get { return content.Length; }
      }
   }	
}
