using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScript
{
    public class ISyntaxElement : ISpan
    {
    }

    public class ASTHelpers
    {
       public static bool isValidAstNode(ISyntaxElement ast)
       {
          throw new NotImplementedException();
       } 
       
       public static bool isValidSpan(ISpan ast)
       {
          throw new NotImplementedException();
       } 
    }

    public class Missing
    {
       public static string getPrettyName(string modPath, bool quote=true, bool treatAsFileName=false)
       { 
           /*
           var modName = treatAsFileName ? switchToForwardSlashes(modPath) : trimModName(stripStartAndEndQuotes(modPath));
           var components = this.getPathComponents(modName);
           return components.length ? (quote ? quoteStr(components[components.length - 1]) : components[components.length - 1]) : modPath;
           */
           // TODO
           return modPath;
       }
    }

    public class Errors
    {
      public static void argument(string msg)
      {
         throw new Exception(msg);
      }
    }

    public enum EmitOptions 
    {
    }

    public class Document
    {
    }

    public class OutputFile
    {
       public List<SourceMapEntry> sourceMapEntries;
    }

    public class JSON
    {
       public static string stringify(object ob)
       {
          throw new NotImplementedException();
       }
    }
}
