using System;
using System.Collections.Generic;
using System.Text;
using GhostscriptSharp;
using System.Runtime.InteropServices;

namespace Examples
{
   /// <summary>
   /// Shows how to feed GhostscriptSharp piecemeal
   /// </summary>
   /// <remarks>When feeding Ghostscript piecemeal buffers, one can use the normal operators to configure things and invoke library routines. The following example 
   /// would cause Ghostscript to open and process the file named "example.pdf" as if it had been passed as an argument to gsapi_init_with_args().</remarks>
   /// <example>code = gsapi_run_string(minst, "(example.pdf) .runlibfile", 0, &exit_code);</example>
   /// <remarks>Port of Ghostscript Example code at http://pages.cs.wisc.edu/~ghost/doc/cvs/API.htm</remarks>
   class Example3
   {
      public const String KernelDllName = "kernel32.dll";
      public const String GhostscriptDllDirectory = @"C:\Program Files\gs\gs8.71\bin";
      public static String command = "1 2 add == flush" + System.Environment.NewLine;

      [DllImport(KernelDllName, SetLastError = true)]
      static extern int SetDllDirectory(string lpPathName);

      [DllImport(KernelDllName, EntryPoint = "RtlMoveMemory", SetLastError = true)]
      static extern int CopyMemory(IntPtr dest, IntPtr source, Int64 count);

      static void Main(string[] args)
      {
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
         code = API.InitAPI(minst, gsargv.Length, gsargv);
         if (code == 0)
         {
            API.RunStringBegin(minst, 0, out exit_code);
            API.RunStringContinue(minst, command, Convert.ToUInt16(command.Length), 0, out exit_code);
            API.RunStringContinue(minst, "qu", 2u, 0, out exit_code);
            API.RunStringContinue(minst, "it", 2u, 0, out exit_code);
            API.RunStringEnd(minst, 0, out exit_code);
         }
         code1 = API.ExitAPI(minst);
         if ((code == 0) || (code == (int)API.GhostscriptErrorCode.e_Quit))
         {
            code = code1;
         }

         API.DeleteAPIInstance(minst);
         int returnValue = 1;
         if ((code == 0) || (code == (int)API.GhostscriptErrorCode.e_Quit))
         {
            returnValue = 0;
         }
         Console.WriteLine("Example3 completed with exit value {0}", returnValue);
         Console.WriteLine("Press any key to exit.");
         Console.ReadKey();
         System.Environment.Exit(returnValue);
      }
   }
}
