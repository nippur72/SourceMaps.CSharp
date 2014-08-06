// this source maps is based on Dart2Js implementation. See the file Dart.original.cs.

using System;
using System.Collections.Generic;
using System.Text;

namespace SourceMaps
{
   public class Uri : System.Uri 
   {
      public Uri(string uri) : base(uri)
      {
      }

      public static Uri parse(string uri)
      {
         return new Uri(uri);   
      }

      public static string relativize(Uri u1, Uri u2, bool flag)
      {
         return ((System.Uri)u1).MakeRelativeUri((System.Uri)u2).ToString();         
      }
   }
}
