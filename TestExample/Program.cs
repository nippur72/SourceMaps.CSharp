using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SourceMaps;

using Uri = SourceMaps.Uri;

using System.IO;
using System.Text.RegularExpressions;

namespace TestExample
{
   class Program
   {
      public static string LoadFromFile(string Nome)
      {
         StreamReader sr = new StreamReader(Nome);
         string buf = sr.ReadToEnd();
         sr.Close();
         return buf;
      }

      public static void SaveToFile(string Nome, string Testo)
      {
         StreamWriter sw = new StreamWriter(Nome);
         sw.Write(Testo);
         sw.Close();
      }

      static void Main(string[] args)
      {
         Test2();
      }

      static void Test2()
      {
         SourceMaps.List<SourceFile> sources = new SourceMaps.List<SourceFile>();

         sources.Add( new SourceFile("http://www.mysite.com/source1.txt", LoadFromFile(@"..\..\..\Website\source1.txt")) );
         sources.Add( new SourceFile("http://www.mysite.com/source2.txt", LoadFromFile(@"..\..\..\Website\source2.txt")) );

         string destfile = "";
         foreach(var sf in sources) destfile+=sf.content.ToLower();                   
         destfile +="\r\n//# sourceMappingURL=myapp.js.map";

         SaveToFile(@"..\..\..\Website\myapp.js",destfile);

         SourceFile target = new SourceFile("http://www.mysite.com/myapp.js",destfile);  

         // build tokens/source map entries
         SourceMaps.List<SourceMapEntry> sme = new SourceMaps.List<SourceMapEntry>();

         // this will map every single character, no name specified
         int cumulative_l = 0;
         foreach(var sf in sources)
         {
            for(int cx=0;cx<sf.content.Length;cx++)
            {
               string c = ""; //sf.content[cx].ToString();
               var tok = new TokenSourceFileLocation(sf, c, cx);  
               sme.add( new SourceMapEntry(tok,cx+cumulative_l));
            }
            cumulative_l += sf.length;
         }         

         // this maps only identifiers
         /*
         Regex R = new Regex(@"[_a-zA-Z][_a-zA-Z0-9]*");
         int cumulative_l = 0;
         foreach(var sf in sources)
         {
            MatchCollection mc = R.Matches(sf.content);
            foreach(Match match in mc)
            {
               var keyword = match.Value.ToLower();
               
               var tok = new TokenSourceFileLocation(sf, match.Value.ToLower(), match.Index);  
               sme.add( new SourceMapEntry(tok,match.Index+cumulative_l));               
            }
            cumulative_l += sf.length;
         }
         */

         Uri sourceMapUri = Uri.parse("http://www.mysite.com/myapp.map");
         Uri fileUri      = Uri.parse("http://www.mysite.com/myapp.js");

         SourceMapBuilder sourceMapBuilder = new SourceMapBuilder(sourceMapUri, fileUri, target);
         foreach(var e in sme) sourceMapBuilder.addMapping(e.targetOffset,e.sourceLocation);
         String sourceMap = sourceMapBuilder.build(); 

         SaveToFile(@"..\..\..\Website\myapp.js.map",sourceMap);
      }

      void Test1()
      {
         /*
         //                                       //01234567890123456
         SourceFile f1 = new SourceFile("http://www.mysite.com/pippo.cs","class pippo {}\r\n");
         SourceFile f2 = new SourceFile("http://www.mysite.com/pluto.cs","class pluto {}\r\n");

         //                                       //       1         2             3         4
         //                                       //0123456012345678901    23456789012345678901    23456789
         SourceFile fx = new SourceFile("myapp.js","function pippo() {}\r\n function pluto() {}\r\n");  
         
         var t1 = new TokenSourceFileLocation(f1, "class", 0);  
         var t2 = new TokenSourceFileLocation(f1, "pippo", 6);
         var t3 = new TokenSourceFileLocation(f1, "{}", 12);

         var t4 = new TokenSourceFileLocation(f2, "class", 0);  
         var t5 = new TokenSourceFileLocation(f2, "pluto", 6);
         var t6 = new TokenSourceFileLocation(f2, "{}", 12);

         SourceMapEntry[] sme = new SourceMapEntry[] 
         {
            new SourceMapEntry(t1,  0),
            new SourceMapEntry(t2, 12),
            new SourceMapEntry(t3, 20),
            new SourceMapEntry(t4, 23),
            new SourceMapEntry(t5, 32),
            new SourceMapEntry(t6, 40)
         };

         Uri sourceMapUri = Uri.parse("http://www.mysite.com/myapp.map");
         Uri fileUri      = Uri.parse("http://www.mysite.com/myapp.js");
                  
         SourceMapBuilder sourceMapBuilder = new SourceMapBuilder(sourceMapUri, fileUri, fx);
         foreach(var e in sme) sourceMapBuilder.addMapping(e.targetOffset,e.sourceLocation);
         String sourceMap = sourceMapBuilder.build(); 

         Console.Write(sourceMap);
         Console.ReadLine();
         */
      }
   }
}
