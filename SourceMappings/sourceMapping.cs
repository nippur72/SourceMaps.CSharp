
//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TypeScript 
{
    public class SourceMapPosition 
    {
        public int sourceLine;
        public int sourceColumn;
        public int emittedLine;
        public int emittedColumn;
    }

    public class SourceMapping 
    {
        public SourceMapPosition start = new SourceMapPosition();
        public SourceMapPosition end = new SourceMapPosition();
        public int nameIndex = -1;
        public List<SourceMapping> childMappings = new List<SourceMapping>();
    }

    public class SourceMapEntry 
    {
        public string emittedFile;
        public int emittedLine;
        public int emittedColumn;
        public string sourceFile;
        public int sourceLine;
        public int sourceColumn;
        public string sourceName; 
        
        public SourceMapEntry
        (
            string emittedFile,
            int emittedLine,
            int emittedColumn,
            string sourceFile,
            int sourceLine,
            int sourceColumn,
            string sourceName
       ) 
       {
            this.emittedFile   = emittedFile;
            this.emittedLine   = emittedLine;
            this.emittedColumn = emittedColumn;
            this.sourceFile    = sourceFile;
            this.sourceLine    = sourceLine;
            this.sourceColumn  = sourceColumn;
            this.sourceName    = sourceName;

            /*Debug.Assert(isFinite(emittedLine));
            Debug.Assert(isFinite(emittedColumn));
            Debug.Assert(isFinite(sourceColumn));
            Debug.Assert(isFinite(sourceLine));*/
        }
    }

    public class SourceMapper 
    {
        static string MapFileExtension = ".map";

        private string jsFileName;
        private string sourceMapPath;
        private string sourceMapDirectory;
        private string sourceRoot;

        public List<string> names = new List<string>();

        private List<ISpan> mappingLevel = new List<ISpan>();

        // Below two arrays represent the information about sourceFile at that index.
        private List<string> tsFilePaths = new List<string>();
        private List<List<SourceMapping>> allSourceMappings = new List<List<SourceMapping>>();

        public List<List<SourceMapping>> currentMappings;
        public List<int> currentNameIndex;

        private List<SourceMapEntry> sourceMapEntries = new List<SourceMapEntry>();

        private TextWriter jsFile;
        private TextWriter sourceMapOut;

        public SourceMapper
        (
            TextWriter jsFile,
            TextWriter sourceMapOut,
            Document document,
            string jsFilePath,
            EmitOptions emitOptions,
            Func<string,string> resolvePath // (path: string) => string
        ) 
        {
            this.jsFile = jsFile;
            this.sourceMapOut = sourceMapOut;

            this.setSourceMapOptions(document, jsFilePath, emitOptions, resolvePath);
            this.setNewSourceFile(document, emitOptions);
        }

        public OutputFile getOutputFile() {
            var result = this.sourceMapOut.getOutputFile();
            result.sourceMapEntries = this.sourceMapEntries;

            return result;
        }

        public void increaseMappingLevel(ISpan ast) {
            this.mappingLevel.Add(ast);
        }

        public void decreaseMappingLevel(ISpan ast) 
        {
            Debug.Assert(this.mappingLevel.Count > 0, "Mapping level should never be less than 0. This suggests a missing start call.");
            
            var expectedAst = this.mappingLevel[this.mappingLevel.Count-1]; 
            this.mappingLevel.RemoveAt(this.mappingLevel.Count-1); // .pop()
            
            /*
            var expectedAstInfo = (<ISyntaxElement>expectedAst).kind ? SyntaxKind[(<ISyntaxElement>expectedAst).kind()] : [expectedAst.start(), expectedAst.end()];
            
            var astInfo = (<ISyntaxElement>ast).kind ? SyntaxKind[(<ISyntaxElement>ast).kind()] : [ast.start(), ast.end()];
            
            Debug.Assert(
                ast == expectedAst,
                "Provided ast is not the expected ISyntaxElement, Expected: " + expectedAstInfo + " Given: " + astInfo);
            */ 
        }

        public void setNewSourceFile(Document document, EmitOptions emitOptions) {
            // Set new mappings
            List<SourceMapping> sourceMappings = new List<SourceMapping>();
            this.allSourceMappings.Add(sourceMappings);
            this.currentMappings = new List<List<SourceMapping>>();  
            this.currentMappings.Add(sourceMappings);
            this.currentNameIndex = new List<int>();

            // Set new source file path
            this.setNewSourceFilePath(document, emitOptions);
        }

        private void setSourceMapOptions(Document document, string jsFilePath, EmitOptions emitOptions, Func<string,string> resolvePath) 
        {
            throw new NotImplementedException();

            /*
            // Decode mapRoot and sourceRoot

            // Js File Name = pretty name of js file
            var prettyJsFileName = TypeScript.getPrettyName(jsFilePath, false, true);
            var prettyMapFileName = prettyJsFileName + SourceMapper.MapFileExtension;
            this.jsFileName = prettyJsFileName;

            // Figure out sourceMapPath and sourceMapDirectory
            if (emitOptions.sourceMapRootDirectory()) {
                // Get the sourceMap Directory
                this.sourceMapDirectory = emitOptions.sourceMapRootDirectory();
                if (document.emitToOwnOutputFile()) {
                    // For modules or multiple emit files the mapRoot will have directory structure like the sources
                    // So if src\a.ts and src\lib\b.ts are compiled together user would be moving the maps into mapRoot\a.js.map and mapRoot\lib\b.js.map
                    this.sourceMapDirectory = this.sourceMapDirectory + switchToForwardSlashes(getRootFilePath((document.fileName)).replace(emitOptions.commonDirectoryPath(), ""));
                }

                if (isRelative(this.sourceMapDirectory)) {
                    // The relative paths are relative to the common directory
                    this.sourceMapDirectory = emitOptions.commonDirectoryPath() + this.sourceMapDirectory;
                    this.sourceMapDirectory = convertToDirectoryPath(switchToForwardSlashes(resolvePath(this.sourceMapDirectory)));
                    this.sourceMapPath = getRelativePathToFixedPath(getRootFilePath(jsFilePath), this.sourceMapDirectory + prettyMapFileName);
                }
                else {
                    this.sourceMapPath = this.sourceMapDirectory + prettyMapFileName;
                }
            }
            else {
                this.sourceMapPath = prettyMapFileName;
                this.sourceMapDirectory = getRootFilePath(jsFilePath);
            }
            this.sourceRoot = emitOptions.sourceRootDirectory();
            */
        }

        private void setNewSourceFilePath(Document document, EmitOptions emitOptions) 
        {
            throw new NotImplementedException();
            /*
            var tsFilePath = switchToForwardSlashes(document.fileName);
            if (emitOptions.sourceRootDirectory()) {
                // Use the relative path corresponding to the common directory path
                tsFilePath = getRelativePathToFixedPath(emitOptions.commonDirectoryPath(), tsFilePath);
            }
            else {
                // Source locations relative to map file location
                tsFilePath = getRelativePathToFixedPath(this.sourceMapDirectory, tsFilePath);
            }
            this.tsFilePaths.Add(tsFilePath);
            */
        }
        

        // Generate source mapping.
        // Creating files can cause exceptions, they will be caught higher up in TypeScriptCompiler.emit
        public void emitSourceMapping() 
        {
            /*
            Debug.Assert(
                this.mappingLevel.length === 0,
                "Mapping level is not 0. This suggest a missing end call. Value: " +
                this.mappingLevel.map(item => ['Node of type', SyntaxKind[(<ISyntaxElement>item).kind()], 'at', item.start(), 'to', item.end()].join(' ')).join(', '));
            */

            // Output map file name into the js file
            this.jsFile.WriteLine("//# sourceMappingURL=" + this.sourceMapPath);

            // Now output map file
            var mappingsString = "";

            var prevEmittedColumn = 0;
            var prevEmittedLine = 0;
            var prevSourceColumn = 0;
            var prevSourceLine = 0;
            var prevSourceIndex = 0;
            var prevNameIndex = 0;
            var emitComma = false;

            SourceMapPosition recordedPosition = null;
            for (var sourceIndex = 0; sourceIndex < this.tsFilePaths.Count; sourceIndex++) 
            {
                Action<SourceMapPosition, int> recordSourceMapping = (SourceMapPosition mappedPosition, int nameIndex) => {

                    if (recordedPosition != null &&
                        recordedPosition.emittedColumn == mappedPosition.emittedColumn &&
                        recordedPosition.emittedLine == mappedPosition.emittedLine) {
                        // This position is already recorded
                        return;
                    }

                    // Record this position
                    if (prevEmittedLine != mappedPosition.emittedLine) {
                        while (prevEmittedLine < mappedPosition.emittedLine) {
                            prevEmittedColumn = 0;
                            mappingsString = mappingsString + ";";
                            prevEmittedLine++;
                        }
                        emitComma = false;
                    }
                    else if (emitComma) {
                        mappingsString = mappingsString + ",";
                    }

                    this.sourceMapEntries.Add(new SourceMapEntry(
                        this.jsFileName,
                        mappedPosition.emittedLine + 1,
                        mappedPosition.emittedColumn + 1,
                        this.tsFilePaths[sourceIndex],
                        mappedPosition.sourceLine,
                        mappedPosition.sourceColumn + 1,
                        nameIndex >= 0 ? this.names[nameIndex] : null));

                    // 1. Relative Column
                    mappingsString = mappingsString + Base64VLQFormat.encode(mappedPosition.emittedColumn - prevEmittedColumn);
                    prevEmittedColumn = mappedPosition.emittedColumn;

                    // 2. Relative sourceIndex 
                    mappingsString = mappingsString + Base64VLQFormat.encode(sourceIndex - prevSourceIndex);
                    prevSourceIndex = sourceIndex;

                    // 3. Relative sourceLine 0 based
                    mappingsString = mappingsString + Base64VLQFormat.encode(mappedPosition.sourceLine - 1 - prevSourceLine);
                    prevSourceLine = mappedPosition.sourceLine - 1;

                    // 4. Relative sourceColumn 0 based 
                    mappingsString = mappingsString + Base64VLQFormat.encode(mappedPosition.sourceColumn - prevSourceColumn);
                    prevSourceColumn = mappedPosition.sourceColumn;

                    // 5. Relative namePosition 0 based
                    if (nameIndex >= 0) {
                        mappingsString = mappingsString + Base64VLQFormat.encode(nameIndex - prevNameIndex);
                        prevNameIndex = nameIndex;
                    }

                    emitComma = true;
                    recordedPosition = mappedPosition;
                };

                // Record starting spans
                Action<List<SourceMapping>> recordSourceMappingSiblings = null;

                recordSourceMappingSiblings = (List<SourceMapping> sourceMappings) => {
                    for (var i = 0; i < sourceMappings.Count; i++) {
                        var sourceMapping = sourceMappings[i];
                        recordSourceMapping(sourceMapping.start, sourceMapping.nameIndex);
                        recordSourceMappingSiblings(sourceMapping.childMappings);
                        recordSourceMapping(sourceMapping.end, sourceMapping.nameIndex);
                    }
                };

                recordSourceMappingSiblings(this.allSourceMappings[sourceIndex]);
            }

            // Write the actual map file
            this.sourceMapOut.Write(JSON.stringify(new {
                version = 3,
                file = this.jsFileName,
                sourceRoot = this.sourceRoot,
                sources = this.tsFilePaths,
                names = this.names,
                mappings = mappingsString
            }));

            // Closing files could result in exceptions, report them if they occur
            this.sourceMapOut.Close();
        }
    }
}