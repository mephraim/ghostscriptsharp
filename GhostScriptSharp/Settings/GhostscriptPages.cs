using System;
using System.Collections.Generic;

using System.Text;

namespace GhostscriptSharp.Settings
{
   /// <summary>
   /// Which pages to output
   /// </summary>
   public class GhostscriptPages
   {
      private bool _allPages = true;
      private int _start;
      private int _end;

      /// <summary>
      /// Output all pages avaialble in document
      /// </summary>
      public bool AllPages
      {
         set
         {
            this._start = -1;
            this._end = -1;
            this._allPages = true;
         }
         get
         {
            return this._allPages;
         }
      }

      /// <summary>
      /// Start output at this page (1 for page 1)
      /// </summary>
      public int Start
      {
         set
         {
            this._allPages = false;
            this._start = value;
         }
         get
         {
            return this._start;
         }
      }

      /// <summary>
      /// Page to stop output at
      /// </summary>
      public int End
      {
         set
         {
            this._allPages = false;
            this._end = value;
         }
         get
         {
            return this._end;
         }
      }
   }
}
