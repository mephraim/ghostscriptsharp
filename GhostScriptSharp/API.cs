/* ================================= MIT LICENSE =================================
 * 
 * Copyright (c) 2009 Matthew Ephraim
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * ================================= MIT LICENSE ================================= */

/* ===============================================================================
 * 
 * This code adapted from Matthew Ephraim's GhostscriptSharp February 2011
 * 
 * https://github.com/mephraim/ghostscriptsharp/blob/master/GhostScriptSharp/GhostscriptSharp.cs
 * 
 * =============================================================================== */

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostscriptSharp
{
   public class API
   {
      #region Ghostscript Error Codes
      public enum GhostscriptErrorCode :int // aka Int32
      {
         NoErrors = 0,
         e_unknownerror = -1,
         e_dictfull = -2,
         e_dictstackoverflow = -3,
         e_dictstackunderflow = -4,
         e_execstackoverflow = -5,
         e_interrupt = -6,
         e_invalidaccess = -7,
         e_invalidexit = -8,
         e_invalidfileaccess = -9,
         e_invalidfont = -10,
         e_invalidrestore = -11,
         e_ioerror = -12,
         e_limitcheck = -13,
         e_nocurrentpoint = -14,
         e_rangecheck = -15,
         e_stackoverflow = -16,
         e_stackunderflow = -17,
         e_syntaxerror = -18,
         e_timeout = -19,
         e_typecheck = -20,
         e_undefined = -21,
         e_undefinedfilename = -22,
         e_undefinedresult = -23,
         e_unmatchedmark = -24,
         e_VMerror = -25,
         // Level 1 Errors
         e_configurationerror = -26,
         e_undefinedresource = -27,
         e_unregistered = -28,
         // Level 2 Errors
         e_invalidcontext = -29,
         e_invalidid = -30,
         // Level 3 Errors

         e_Fatal = -100,
         e_Quit = -101,
         e_NeedInput = -106,
         e_Info = -110,
         UNKNOWN = -9999
      };
      #endregion

      #region Constants

      public const String GhostscriptDllName = "gsdll32.dll";

      #endregion

      #region Hooks into Ghostscript DLL
      /// <summary>
      /// Returns the revision number and strings of the Ghostscript interpreter library.
      /// </summary>
      /// <param name="pr"></param>
      /// <param name="len"></param>
      /// <returns>0 for success, something else for error</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_revision")]
      public static extern Int32 GetRevision(out GS_Revision pr, Int32 len);

      /// <summary>
      /// Create a new instance of Ghostscript. The Ghostscript API supports only one instance.
      /// </summary>
      /// <param name="pinstance">Instance pointer, provided to most other API calls</param>
      /// <param name="caller_handle">Will be provided to callback functions</param>
      /// <returns></returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_new_instance")]
      public static extern Int32 CreateAPIInstance(out IntPtr pinstance, IntPtr caller_handle);

      /// <summary>
      /// Destroy an instance of Ghostscript. If Ghostscript has been initialized with InitAPI, you must call ExitAPI before this.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_delete_instance")]
      public static extern void DeleteAPIInstance(IntPtr instance);

      /// <summary>
      /// Set the callback functions for stdio
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="stdin">The callback for stdin reads. Should return the number of characters read, 0 for EOF, or -1 for error.</param>
      /// <param name="stdout">The callback for stdout writes. Should return the number of characters written.</param>
      /// <param name="stderr">The callback for stderr writes. Should return the number of characters written.</param>
      /// <returns></returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_set_stdio")]
      public static extern Int32 Set_Stdio(IntPtr instance, StdinCallback stdin, StdoutCallback stdout, StdoutCallback stderr);

      /// <summary>
      /// Initialize and run the interpreter with arguments. See Ghostscript API for details on arguments.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="argc">The number of arguments supplied</param>
      /// <param name="argv">Array of arguments supplied. Argv[0] is ignored (as in C programs)</param>
      /// <returns></returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_init_with_args")]
      public static extern Int32 InitAPI(IntPtr instance, Int32 argc, string[] argv);

      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_string_begin")]
      public static extern Int32 RunStringBegin(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="str">The string to interpret</param>
      /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode, or e_NeedInput when ready for another string</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_string_continue")]
      public static extern Int32 RunStringContinue(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);
      
      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_string_end")]
      public static extern Int32 RunStringEnd(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="str">The string to interpret</param>
      /// <param name="length">The length of the string to interpret. This must be no greater than 65535</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_string_with_length")]
      public static extern Int32 RunStringWithLength(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="str">The string to interpret. There is a 65535 charactere limit</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_string")]
      public static extern Int32 RunString(IntPtr instance, String str, Int32 user_errors, out Int32 pexit_code);

      /// <summary>
      /// After initializing the interpreter, clients may pass it strings to be interpreted. To pass input in arbitrary chunks, first call RunStringBegin, then RunStringContinue as many times as desired, stopping if it returns anything other than e_NeedInput. Then call RunStringEnd to indicate end of file.
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <param name="file_name">File name of the file to interpret</param>
      /// <param name="user_errors">0 if errors should be handled normally, if set negative, the function will directly return error codes bypassing the interpreted language</param>
      /// <param name="pexit_code">Set to the exit code for the interpreted in case of quit or fatal error</param>
      /// <returns>Error code corresponding to GhostscriptErrorCode</returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_run_file")]
      public static extern Int32 RunFile(IntPtr instance, String file_name, Int32 user_errors, out Int32 pexit_code);

      /// <summary>
      /// Exit the interpreter. This must be called if InitAPI has been called, just before calling DeleteAPIInstance
      /// </summary>
      /// <param name="instance">The instance given by CreateAPIInstance</param>
      /// <returns></returns>
      [DllImport(GhostscriptDllName, EntryPoint = "gsapi_exit")]
      public static extern Int32 ExitAPI(IntPtr instance);
      #endregion

      #region Handle stdio

      /// <summary>
      /// Callback used by Ghostscript API when reading from stdin
      /// </summary>
      /// <param name="caller_handle">The caller_handle supplied to CreateAPIInstance</param>
      /// <param name="buf">The value to provide to Ghostscript from stdin</param>
      /// <param name="len">The number of characters provided</param>
      /// <returns>0 for EOF, or -1 for error, number of characters provided for success</returns>
      public delegate Int32 StdinCallback(IntPtr caller_handle, IntPtr buf, Int32 len);

      /// <summary>
      /// Callback used by Ghostscript API when writing to stdout or stderr
      /// </summary>
      /// <param name="caller_handle">The caller_handle supplied to CreateAPIInstance</param>
      /// <param name="buf">The string output generated by Ghostscript. Note that the buf may be longer than the provided output, use buf.ToString(0, len)</param>
      /// <param name="len">The length of the generated output</param>
      /// <returns></returns>
      public delegate Int32 StdoutCallback(IntPtr caller_handle, String buf, Int32 len);

      #endregion

      public const Int32 GS_Revision_Size_Bytes = 16;

      [StructLayout(LayoutKind.Sequential)]
      public struct GS_Revision
      {
         public string strProduct;
         public string strCopyright;
         public Int32 intRevision;
         public Int32 intRevisionDate;
      }
   }
}
