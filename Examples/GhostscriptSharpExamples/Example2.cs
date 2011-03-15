using System;
using System.Collections.Generic;
using System.Text;
using GhostscriptSharp;
using System.Runtime.InteropServices;

namespace Examples
{
   /// <summary>
   /// Similar to command line gs
   /// </summary>
   /// <remarks>Port of Ghostscript Example code at http://pages.cs.wisc.edu/~ghost/doc/cvs/API.htm</remarks>
   class Example2
   {
      public const String KernelDllName = "kernel32.dll";
      public const String GhostscriptDllDirectory = @"C:\Program Files\gs\gs8.71\bin";
      public static String start_string = "systemdict /start get exec" + System.Environment.NewLine;

      [DllImport(KernelDllName, SetLastError = true)]
      static extern int SetDllDirectory(string lpPathName);

      [DllImport(KernelDllName, EntryPoint = "RtlMoveMemory", SetLastError = true)]
      static extern int CopyMemory(IntPtr dest, IntPtr source, Int64 count);

      static void Main(string[] args)
      {            
         #region StdIn Handler
         StringBuilder sbInput = new StringBuilder();
         // This is very slow, especially because Ghostscript asks for input 1 character at a time
         API.StdinCallback stdin = (caller_handle, str, n) =>
               {
                  if (n == 0)
                  {
                     str = IntPtr.Zero;
                     return 0;
                  }
                  if (sbInput.Length == 0)
                  {
                     sbInput.AppendLine(Console.ReadLine());
                  }
                  if (sbInput.Length > 0)
                  {
                     int len = (sbInput.Length < n) ? sbInput.Length : n;
                     byte[] b = ASCIIEncoding.ASCII.GetBytes(sbInput.ToString(0, len));
                     GCHandle cHandle = GCHandle.Alloc(b, GCHandleType.Pinned);
                     IntPtr cPtr = cHandle.AddrOfPinnedObject();
                     Int64 copyLen = (long)len;
                     CopyMemory(str, cPtr, copyLen);
                     cPtr = IntPtr.Zero;
                     cHandle.Free();
                     sbInput.Remove(0, len);
                     return len;
                  }
                  return 0;
               };
         #endregion
         #region StdOut Handler
         API.StdoutCallback stdout = (caller_handle, buf, len) =>
               {
                  Console.Write(buf.Substring(0, len));
                  return len;
               };
         #endregion
         #region StdErr Handler
         API.StdoutCallback stderr = (caller_handle, buf, len) =>
            {
               Console.Error.Write(buf.Substring(0, len));
               return len;
            };
         #endregion

         string[] gsargv = new string[args.Length + 1];
         gsargv[0] = "GhostscriptSharp";	/* actual value doesn't matter */
         Array.Copy(args, 0, gsargv, 1, args.Length);

         IntPtr minst;
         int code, code1;
         int exit_code;

         SetDllDirectory(GhostscriptDllDirectory);

         code = API.CreateAPIInstance(out minst, IntPtr.Zero);
         if (code < 0)
         {
            System.Environment.Exit(1);
         }
         API.Set_Stdio(minst, stdin, stdout, stderr);
         code = API.InitAPI(minst, gsargv.Length, gsargv);
         if (code == 0)
         {
            API.RunString(minst, start_string, 0, out exit_code);
         }
         code1 = API.ExitAPI(minst);
         if ((code == 0) || (code == (int)API.GhostscriptErrorCode.e_Quit))
         {
            code = code1;
         }

         API.DeleteAPIInstance(minst);
         if ((code == 0) || (code == (int)API.GhostscriptErrorCode.e_Quit))
         {
            System.Environment.Exit(0);
         }
         System.Environment.Exit(1);
      }
   }
}
