// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{  
	// import 'scanner/scannerlib.dart' show Token;	
	// import 'util/uri_extras.dart' show relativize;

   public class SourceMapBuilder 
   {
      const int VLQ_BASE_SHIFT = 5;
      const int VLQ_BASE_MASK = (1 << 5) - 1;
      const int VLQ_CONTINUATION_BIT = 1 << 5;
      const int VLQ_CONTINUATION_MASK = 1 << 5;
      const String BASE64_DIGITS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

      public Uri uri;      // final
      public Uri fileUri;  // final 

      public SourceFile targetFile;
      public List<SourceMapEntry> entries;

      public Map<String, int> sourceUrlMap;
      public List<String> sourceUrlList;
      public Map<String, int> sourceNameMap;
      public List<String> sourceNameList;

      public int previousTargetLine;
      public int previousTargetColumn;
      public int previousSourceUrlIndex;
      public int previousSourceLine;
      public int previousSourceColumn;
      public int previousSourceNameIndex;
      public bool firstEntryInLine;
      
      public SourceMapBuilder(Uri sourceMapUri, Uri p_fileUri, SourceFile compiledFile) 
      {
         this.uri = sourceMapUri;
         this.fileUri = p_fileUri;
         this.targetFile = compiledFile;

         entries = new List<SourceMapEntry>();

         sourceUrlMap = new Map<String, int>();
         sourceUrlList = new List<String>();
         sourceNameMap = new Map<String, int>();
         sourceNameList = new List<String>();

         previousTargetLine = 0;
         previousTargetColumn = 0;
         previousSourceUrlIndex = 0;
         previousSourceLine = 0;
         previousSourceColumn = 0;
         previousSourceNameIndex = 0;
         firstEntryInLine = true;
      }

      public void resetPreviousSourceLocation() 
      {
         previousSourceUrlIndex = 0;
         previousSourceLine = 0;
         previousSourceColumn = 0;
         previousSourceNameIndex = 0;
      }

      public void updatePreviousSourceLocation(SourceFileLocation sourceLocation) 
      {
         previousSourceLine = sourceLocation.getLine();
         previousSourceColumn = sourceLocation.getColumn();
         String sourceUrl = sourceLocation.getSourceUrl();
         previousSourceUrlIndex = indexOf(sourceUrlList, sourceUrl, sourceUrlMap);
         String sourceName = sourceLocation.getSourceName();
         if (sourceName != null) 
         {
            previousSourceNameIndex = indexOf(sourceNameList, sourceName, sourceNameMap);
         }
      }

      bool sameAsPreviousLocation(SourceFileLocation sourceLocation) 
      {
         if (sourceLocation == null) {
            return true;
         }
         int sourceUrlIndex =
            indexOf(sourceUrlList, sourceLocation.getSourceUrl(), sourceUrlMap);
         return
            sourceUrlIndex == previousSourceUrlIndex &&
            sourceLocation.getLine() == previousSourceLine &&
            sourceLocation.getColumn() == previousSourceColumn;
      }

      private bool sameLine(int position, int otherPosition) 
      {
         return targetFile.getLine(position) == targetFile.getLine(otherPosition);
      } 

      public void addMapping(int targetOffset, SourceFileLocation sourceLocation) 
      {
         if (!entries.isEmpty && sameLine(targetOffset, entries.last.targetOffset)) 
         {
            if (sameAsPreviousLocation(sourceLocation)) 
            {
               // The entry points to the same source location as the previous entry in
               // the same line, hence it is not needed for the source map.
               //
               // TODO(zarah): Remove this check and make sure that [addMapping] is not
               // called for this position. Instead, when consecutive lines in the
               // generated code point to the same source location, record this and use
               // it to generate the entries of the source map.
               return;
            }
         }

         if (sourceLocation != null) 
         {
            updatePreviousSourceLocation(sourceLocation);
         }
         entries.add(new SourceMapEntry(sourceLocation, targetOffset));
      }

      private void printStringListOn(List<String> strings, StringBuffer buffer)
      {
         bool first = true;
         buffer.write("[");
         foreach(String str in strings) 
         {
            if (!first) buffer.write(",");
            buffer.write("\u0022");
            writeJsonEscapedCharsOn.call(str, buffer);
            buffer.write("\u0022");
            first = false;
         }
         buffer.write("]");
      }

      public String build() 
      {
         resetPreviousSourceLocation();
         StringBuffer mappingsBuffer = new StringBuffer();
         entries.ForEach((SourceMapEntry entry) => writeEntry(entry, targetFile, mappingsBuffer));
         StringBuffer buffer = new StringBuffer();
         buffer.write("{\n");
         buffer.write("  \u0022version\u0022: 3,\n");
         if (uri != null && fileUri != null) {
            buffer.write(string.Format("  \u0022file\u0022: \u0022{0}\u0022,\n",Uri.relativize(uri, fileUri, false)));
         }
         buffer.write("  \u0022sourceRoot\u0022: \u0022\u0022,\n");
         buffer.write("  \u0022sources\u0022: ");
         if(uri != null) 
         {
            //sourceUrlList = sourceUrlList.map((url) => relativize(uri, Uri.parse(url), false)).toList();
            for(int t=0;t<sourceUrlList.length;t++) sourceUrlList[t] = Uri.relativize(uri, Uri.parse(sourceUrlList[t]), false);
         }
         printStringListOn(sourceUrlList, buffer);
         buffer.write(",\n");
         buffer.write("  \u0022names\u0022: ");
         printStringListOn(sourceNameList, buffer);
         buffer.write(",\n");
         buffer.write("  \u0022mappings\u0022: \u0022");
         buffer.write(mappingsBuffer);
         buffer.write("\u0022\n}\n");
         return buffer.toString();
      }

      public void writeEntry(SourceMapEntry entry, SourceFile targetFile, StringBuffer output) 
      {
         int targetLine = targetFile.getLine(entry.targetOffset);
         int targetColumn = targetFile.getColumn(targetLine, entry.targetOffset);

         if (targetLine > previousTargetLine) {
            for (int i = previousTargetLine; i < targetLine; ++i) {
               output.write(";");
            }
            previousTargetLine = targetLine;
            previousTargetColumn = 0;
            firstEntryInLine = true;
         }

         if (!firstEntryInLine) {
            output.write(",");
         }
         firstEntryInLine = false;

         encodeVLQ(output, targetColumn - previousTargetColumn);
         previousTargetColumn = targetColumn;

         if (entry.sourceLocation == null) return;

         String sourceUrl = entry.sourceLocation.getSourceUrl();
         int sourceLine = entry.sourceLocation.getLine();
         int sourceColumn = entry.sourceLocation.getColumn();
         String sourceName = entry.sourceLocation.getSourceName();

         int sourceUrlIndex = indexOf(sourceUrlList, sourceUrl, sourceUrlMap);
         encodeVLQ(output, sourceUrlIndex - previousSourceUrlIndex);
         encodeVLQ(output, sourceLine - previousSourceLine);
         encodeVLQ(output, sourceColumn - previousSourceColumn);

         if (sourceName != null) {
            int sourceNameIndex = indexOf(sourceNameList, sourceName, sourceNameMap);
            encodeVLQ(output, sourceNameIndex - previousSourceNameIndex);
         }

         // Update previous source location to ensure the next indices are relative
         // to those if [entry.sourceLocation].
         updatePreviousSourceLocation(entry.sourceLocation);
      }

      public int indexOf(List<String> list, String value, Map<String, int> map) 
      {         
         return map.putIfAbsent(value, ()=>
         {
            int index = list.length;
            list.add(value);
            return index;
         });
      }

      public static void encodeVLQ(StringBuffer output, int value) 
      {
         int signBit = 0;
         if (value < 0) {
            signBit = 1;
            value = -value;
         }
         value = (value << 1) | signBit;
         do {
            int digit = value & VLQ_BASE_MASK;
            value >>= VLQ_BASE_SHIFT;
            if (value > 0) {
               digit |= VLQ_CONTINUATION_BIT;
            }
            output.write(BASE64_DIGITS[digit]);
         } while (value > 0);
      }
   }  
}
