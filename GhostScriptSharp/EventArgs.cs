using System;
using System.Collections.Generic;
using System.Text;

namespace GhostscriptSharp
{
   public class StdOutputEventArgs : EventArgs
   {
      protected readonly String _output;
      public String Output { get { return _output; } }

      public StdOutputEventArgs(String output)
         : base()
      {
         _output = output;
      }
   }
}
