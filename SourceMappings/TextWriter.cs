using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScript 
{
    public enum OutputFileType {
        JavaScript,
        SourceMap,
        Declaration
    }

    public class TextWriter {
        private string contents = "";
        public  bool onNewLine = true;
        private string name;
        private bool writeByteOrderMark;
        private OutputFileType outputFileType;

        TextWriter(string name, bool writeByteOrderMark, OutputFileType outputFileType) 
        {
           this.name               = name;
           this.writeByteOrderMark = writeByteOrderMark;
           this.outputFileType     = outputFileType;
        }

        public void Write(string s) {
            this.contents += s;
            this.onNewLine = false;
        }

        public void WriteLine(string s) {
            this.contents += s;
            this.contents += "\r\n";
            this.onNewLine = true;
        }

        public void Close() {
        }
                
        public OutputFile getOutputFile() {
            throw new NotImplementedException();
            //return new OutputFile(this.name, this.writeByteOrderMark, this.contents, this.outputFileType);
        }
        
    }
}