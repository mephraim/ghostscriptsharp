using System;
using System.Collections.Generic;

using System.Text;

namespace GhostscriptSharp.Settings
{
   /// <summary>
   /// Native page sizes
   /// </summary>
   public enum GhostscriptPageSizes
   {
      UNDEFINED,
      paper11x17,
      ledger,
      legal,
      letter,
      lettersmall,
      archE,
      archD,
      archC,
      archB,
      archA,
      a0,
      a1,
      a2,
      a3,
      a4,
      a4small,
      a5,
      a6,
      a7,
      a8,
      a9,
      a10,
      isob0,
      isob1,
      isob2,
      isob3,
      isob4,
      isob5,
      isob6,
      c0,
      c1,
      c2,
      c3,
      c4,
      c5,
      c6,
      jisb0,
      jisb1,
      jisb2,
      jisb3,
      jisb4,
      jisb5,
      jisb6,
      b0,
      b1,
      b2,
      b3,
      b4,
      b5,
      flsa,
      flse,
      halfletter
   }

   /// <summary>
   /// Output document physical dimensions
   /// </summary>
   public class GhostscriptPageSize
   {
      private GhostscriptPageSizes _fixed;
      private System.Drawing.Size _manual;

      /// <summary>
      /// Custom document size
      /// </summary>
      public System.Drawing.Size Manual
      {
         set
         {
            this._fixed = GhostscriptPageSizes.UNDEFINED;
            this._manual = value;
         }
         get
         {
            return this._manual;
         }
      }

      /// <summary>
      /// Standard paper size
      /// </summary>
      public GhostscriptPageSizes Native
      {
         set
         {
            this._fixed = value;
            this._manual = new System.Drawing.Size(0, 0);
         }
         get
         {
            return this._fixed;
         }
      }

   }
}
