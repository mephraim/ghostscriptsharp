using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;

namespace GhostscriptSharp
{
   public class GhostscriptException : ExternalException
   {
      protected readonly API.GhostscriptErrorCode gsErrorCode;
      public API.GhostscriptErrorCode GsErrorCode
      {
         get { return gsErrorCode; }
      }

      public GhostscriptException()
         : base()
      {
      }

      public GhostscriptException(string message)
         : base(message)
      {
         gsErrorCode = API.GhostscriptErrorCode.UNKNOWN;
      }

      public GhostscriptException(string message, Exception inner)
         : base(message, inner)
      {
      }

      public GhostscriptException(string message, int errorCode)
         : base(message, errorCode)
      {
         //ex.ErrorCode
         if (Enum.IsDefined(typeof(API.GhostscriptErrorCode), errorCode))
         {
            gsErrorCode = (API.GhostscriptErrorCode)errorCode;
         }
         else
         {
            gsErrorCode = API.GhostscriptErrorCode.UNKNOWN;
         }
      }
   }
}
