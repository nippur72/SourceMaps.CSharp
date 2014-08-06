// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   public class TokenSourceFileLocation : SourceFileLocation 
   {
      public Token token;  // final
      public String name;  // final

      public TokenSourceFileLocation(SourceFile sourceFile, String name, int offs) : base(sourceFile)
      {
         this.token = new Token();
         this.token.charOffset = offs;
         this.name  = name;       
      }

      public TokenSourceFileLocation(SourceFile sourceFile, Token token, String name) : base(sourceFile)
      {
         this.token = token;
         this.name  = name;       
      }

      public override int offset 
      {
         get { return token.charOffset; }
      }

      public override String getSourceName() 
      {
         return name;
      }
   }
}
