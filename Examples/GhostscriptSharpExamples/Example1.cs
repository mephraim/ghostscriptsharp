using System;
using System.Collections.Generic;
using System.Text;
using GhostscriptSharp;
using System.Runtime.InteropServices;

namespace Examples
{
   /// <summary>
   /// Example of using GS DLL as a ps2pdf converter.
   /// </summary>
   /// <remarks>Port of Ghostscript Example code at http://pages.cs.wisc.edu/~ghost/doc/cvs/API.htm</remarks>
   class Example1
   {
      public const String KernelDllName = "kernel32.dll";
      public const String GhostscriptDllDirectory = @"C:\Program Files\gs\gs8.71\bin";

      [DllImport(KernelDllName, SetLastError = true)]
      static extern int SetDllDirectory(string lpPathName);

      static void Main(string[] args)
      {
         IntPtr minst;

         int code, code1;
         string[] gsargv = new string[10];
         gsargv[0] = "ps2pdf";	/* actual value doesn't matter */
         gsargv[1] = "-dNOPAUSE";
         gsargv[2] = "-dBATCH";
         gsargv[3] = "-dSAFER";
         gsargv[4] = "-sDEVICE=pdfwrite";
         gsargv[5] = "-sOutputFile=out.pdf";
         gsargv[6] = "-c";
         gsargv[7] = ".setpdfwrite";
         gsargv[8] = "-f";
         gsargv[9] = @"Files\input.ps";

         SetDllDirectory(GhostscriptDllDirectory);

         code = API.CreateAPIInstance(out minst, IntPtr.Zero);
         if (code < 0)
         {
            System.Environment.Exit(1);
         }
         code = API.InitAPI(minst, gsargv.Length, gsargv);
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
