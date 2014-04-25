using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScript
{
   public struct LineCol
   {
      public int line; 
      public int character;
   }

   public class Emitter
   {
      public SourceMapper sourceMapper = null;
      public EmitOptions emitOptions;

      public void createSourceMapper(Document document, string jsFileName, TextWriter jsFile, TextWriter sourceMapOut, Func<string,string> resolvePath) 
      {
         this.sourceMapper = new SourceMapper(jsFile, sourceMapOut, document, jsFileName, this.emitOptions, resolvePath);
      }

      public void setSourceMapperNewSourceFile(Document document) 
      {
         this.sourceMapper.setNewSourceFile(document, this.emitOptions);
      }

      public void recordSourceMappingNameStart(string name) 
      {
         if(this.sourceMapper!=null) 
         {
            var nameIndex = -1;
            if (name!=null) 
            {
               if (this.sourceMapper.currentNameIndex.Count > 0) {
                  var parentNameIndex = this.sourceMapper.currentNameIndex[this.sourceMapper.currentNameIndex.Count - 1];
                  if (parentNameIndex != -1) {
                        name = this.sourceMapper.names[parentNameIndex] + "." + name;
                  }
               }

               // Look if there already exists name
               /*var*/ nameIndex = this.sourceMapper.names.Count - 1;
               for (/*nameIndex*/; nameIndex >= 0; nameIndex--) {
                  if (this.sourceMapper.names[nameIndex] == name) {
                        break;
                  }
               }

               if (nameIndex == -1) {
                  nameIndex = this.sourceMapper.names.Count;
                  this.sourceMapper.names.Add(name);
               }
            }
            this.sourceMapper.currentNameIndex.Add(nameIndex);
         }
      }

      public void recordSourceMappingNameEnd() 
      {
         if (this.sourceMapper!=null) 
         {
            //this.sourceMapper.currentNameIndex.pop();
            this.sourceMapper.currentNameIndex.RemoveAt(this.sourceMapper.currentNameIndex.Count-1);
         }
      }

      private void recordSourceMappingStart(ISyntaxElement ast) 
      {
         if (this.sourceMapper!=null && ASTHelpers.isValidAstNode(ast)) 
         {
               this.recordSourceMappingSpanStart(ast);
         }
      }

      private void recordSourceMappingSpanStart(ISpan ast) 
      {
         if (this.sourceMapper!=null && ASTHelpers.isValidSpan(ast)) 
         {
               LineCol lineCol = new LineCol() { line = -1, character = -1 };
               var sourceMapping = new SourceMapping();
               sourceMapping.start.emittedColumn = this.emitState.column;
               sourceMapping.start.emittedLine = this.emitState.line;
               // REVIEW: check time consumed by this binary search (about two per leaf statement)
               var lineMap = this.document.lineMap();
               lineMap.fillLineAndCharacterFromPosition(ast.start(), lineCol);
               sourceMapping.start.sourceColumn = lineCol.character;
               sourceMapping.start.sourceLine = lineCol.line + 1;
               lineMap.fillLineAndCharacterFromPosition(ast.end(), lineCol);
               sourceMapping.end.sourceColumn = lineCol.character;
               sourceMapping.end.sourceLine = lineCol.line + 1;

               /*
               Debug.assert(!isNaN(sourceMapping.start.emittedColumn));
               Debug.assert(!isNaN(sourceMapping.start.emittedLine));
               Debug.assert(!isNaN(sourceMapping.start.sourceColumn));
               Debug.assert(!isNaN(sourceMapping.start.sourceLine));
               Debug.assert(!isNaN(sourceMapping.end.sourceColumn));
               Debug.assert(!isNaN(sourceMapping.end.sourceLine));
               */

               if (this.sourceMapper.currentNameIndex.Count > 0) {
                  sourceMapping.nameIndex = this.sourceMapper.currentNameIndex[this.sourceMapper.currentNameIndex.Count - 1];
               }
               // Set parent and child relationship
               var siblings = this.sourceMapper.currentMappings[this.sourceMapper.currentMappings.Count - 1];
               siblings.Add(sourceMapping);
               this.sourceMapper.currentMappings.Add(sourceMapping.childMappings);
               this.sourceMapper.increaseMappingLevel(ast);
         }
      }

      private void recordSourceMappingEnd(ISyntaxElement ast) 
      {
         if (this.sourceMapper!=null && ASTHelpers.isValidAstNode(ast)) {
               this.recordSourceMappingSpanEnd(ast);
         }
      }

      private void recordSourceMappingSpanEnd(ISpan ast) 
      {
         if (this.sourceMapper!=null && ASTHelpers.isValidSpan(ast)) 
         {
               // Pop source mapping childs
               //this.sourceMapper.currentMappings.pop();
               this.sourceMapper.currentMappings.RemoveAt(this.sourceMapper.currentMappings.Count-1);

               // Get the last source mapping from sibling list = which is the one we are recording end for
               var siblings = this.sourceMapper.currentMappings[this.sourceMapper.currentMappings.Count - 1];
               var sourceMapping = siblings[siblings.Count - 1];

               sourceMapping.end.emittedColumn = this.emitState.column;
               sourceMapping.end.emittedLine = this.emitState.line;

               /*
               Debug.assert(!isNaN(sourceMapping.end.emittedColumn));
               Debug.assert(!isNaN(sourceMapping.end.emittedLine));
               */ 

               this.sourceMapper.decreaseMappingLevel(ast);
         }
      }

      // Note: may throw exception.
      public List<OutputFile> getOutputFiles() 
      {
         // Output a source mapping.  As long as we haven't gotten any errors yet.
         List<OutputFile> result = new List<OutputFile>();

         if (this.sourceMapper != null) 
         {
               this.sourceMapper.emitSourceMapping();
               result.Add(this.sourceMapper.getOutputFile());
         }

         result.Add(this.outfile.getOutputFile());
         return result;
      }
   }
}