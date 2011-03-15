using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace GhostscriptSharp
{
   public class GhostscriptManager : IDisposable
   {
      #region Constants

      public const String KernelDllName = "kernel32.dll";

      #endregion

      #region Hooks into the kernel32 DLL (for loading unmanaged code)
      [DllImport(KernelDllName, SetLastError = true)]
      static extern IntPtr LoadLibrary(string lpFileName);

      [DllImport(KernelDllName, SetLastError = true)]
      static extern int FreeLibrary(IntPtr hModule);

      [DllImport(KernelDllName, SetLastError = true)]
      static extern int SetDllDirectory(string lpPathName);
      #endregion

      #region Globals

      protected static object resourceLock = new object();

      protected static String[] defaultArgs;
      protected static String[] DefaultArgs
      {
         get
         {
            if (defaultArgs == null)
            {
               defaultArgs = new String[] {
                  "-dPARANOIDSAFER",      // Run in safe mode
                  "-dBATCH",              // Exit after completing commands
                  "-dNOPAUSE",            // Do not pause for each page
                  "-dNOPROMPT"            // Don't prompt for user input
               };
            }
            return defaultArgs;
         }
      }

      protected static String ghostscriptLibraryPath;
      public static String GhostscriptLibraryPath
      {
         get { return ghostscriptLibraryPath == null ? String.Empty : ghostscriptLibraryPath; }
         set { ghostscriptLibraryPath = value; }
      }

      protected static GhostscriptManager _instance;
      public static GhostscriptManager GetInstance()
      {
         lock (resourceLock)
         {
            if (_instance == null)
            {
               _instance = new GhostscriptManager();
            }
            return _instance;
         }
      }

      #endregion

      protected IntPtr libraryHandle;

      protected bool revisionInfoLoaded;
      protected String productName;
      protected String copyright;
      protected Int32 revision;
      protected Int32 revisionDate;

      protected GhostscriptSettings settings;
      public GhostscriptSettings Settings
      {
         get { return settings; }
      }

      protected GhostscriptManager()
      {
         revisionInfoLoaded = false;
         libraryHandle = IntPtr.Zero;

         LoadGhostscriptLibrary();

         this.settings = new GhostscriptSettings();
      }

      protected void LoadGhostscriptLibrary()
      {
         SetDllDirectory(GhostscriptLibraryPath);
         libraryHandle = LoadLibrary(API.GhostscriptDllName);
      }

      protected void UnloadGhostscriptLibrary()
      {
         if (libraryHandle != IntPtr.Zero)
         {
            FreeLibrary(libraryHandle);
            libraryHandle = IntPtr.Zero;
         }
      }

      /// <summary>
      /// Run the Ghostscript interpreter providing the output file and input file(s)
      /// </summary>
      /// <param name="outputPath">The path to create the output file. Put '%d' the path to create multiple numbered files, one for each page</param>
      /// <param name="inputPaths">One or more input files</param>
      public void DoConvert(String outputPath, params String[] inputPaths)
      {
         IntPtr gsInstancePtr;
         lock (resourceLock)
         {
            API.CreateAPIInstance(out gsInstancePtr, IntPtr.Zero);
            try
            {
               if (StdOut != null || StdErr != null)
               {
                  API.StdoutCallback stdout;
                  #region Set StdOut
                  if (StdOut != null)
                  {
                     stdout = (caller_handle, buf, len) =>
                     {
                        StdOut(this, new StdOutputEventArgs(buf.Substring(0, len)));
                        return len;
                     };
                  }
                  else
                  {
                     stdout = EmptyStdoutCallback;
                  }
                  #endregion
                  API.StdoutCallback stderr;
                  #region Set StdErr
                  if (StdErr != null)
                  {
                     stderr = (caller_handle, buf, len) =>
                     {
                        StdOut(this, new StdOutputEventArgs(buf.Substring(0, len)));
                        return len;
                     };
                  }
                  else
                  {
                     stderr = EmptyStdoutCallback;
                  }
                  #endregion
                  API.Set_Stdio(gsInstancePtr, EmptyStdinCallback, stdout, stderr);
               }
               String[] args = null;
               {
                  List<String> lArgs = new List<string>();
                  lArgs.Add("GhostscriptSharp"); // First arg is ignored, corresponds to argv[0]
                  lArgs.AddRange(GhostscriptManager.DefaultArgs);
                  lArgs.AddRange(this.Settings.GetGhostscriptArgs());
                  lArgs.Add(String.Format("-sOutputFile={0}", outputPath));
                  lArgs.AddRange(inputPaths);
                  args = lArgs.ToArray();
               }
               int result = API.InitAPI(gsInstancePtr, args.Length, args);
               if (result < 0)
               {
                  throw new GhostscriptException("Ghostscript conversion error", result);
               }
            }
            finally
            {
               API.ExitAPI(gsInstancePtr);
               API.DeleteAPIInstance(gsInstancePtr);
            }
         }
      }

      #region Revision Info Properties

      /// <summary>
      /// Name of the product obtained from the Ghostscript DLL e.g. "GPL Ghostscript"
      /// </summary>
      public String ProductName
      {
         get
         {
            if (!revisionInfoLoaded)
            {
               LoadRevisionInfo();
            }
            return productName;
         }
      }

      /// <summary>
      /// Copyright Information obtained from the Ghostscript DLL
      /// </summary>
      public String Copyright
      {
         get
         {
            if (!revisionInfoLoaded)
            {
               LoadRevisionInfo();
            }
            return copyright;
         }
      }

      /// <summary>
      /// Revision Number of the Ghostscript DLL e.g. 871 for v8.71
      /// </summary>
      public Int32 Revision
      {
         get
         {
            if (!revisionInfoLoaded)
            {
               LoadRevisionInfo();
            }
            return revision;
         }
      }

      /// <summary>
      /// Revision Date of the Ghostscript DLL in the format yyyyMMdd
      /// </summary>
      public Int32 RevisionDate         
      {
         get
         {
            if (!revisionInfoLoaded)
            {
               LoadRevisionInfo();
            }
            return revisionDate;
         }
      }

      /// <summary>
      /// Get Ghostscript Library revision info
      /// </summary>
      /// <param name="strProduct"></param>
      /// <param name="strCopyright"></param>
      /// <param name="intRevision"></param>
      /// <param name="intRevisionDate"></param>
      protected void LoadRevisionInfo()
      {
         API.GS_Revision rev;

         API.GetRevision(out rev, API.GS_Revision_Size_Bytes);
         this.productName = rev.strProduct;
         this.copyright = rev.strCopyright;
         this.revision = rev.intRevision;
         this.revisionDate = rev.intRevisionDate;
         this.revisionInfoLoaded = true;
      }

      #endregion

      #region stdin, stdout, stderr handlers

      //public delegate void StdinReader(StringBuilder input);
      public delegate void StdOutputHandler(object sender, StdOutputEventArgs args);

      //public event StdinReader StdIn;
      public event StdOutputHandler StdOut;
      public event StdOutputHandler StdErr;

      /// <summary>
      /// "Default" implementation of StdinCallback - gives Ghostscript EOF whenever it requests input
      /// </summary>
      /// <param name="caller_handle"></param>
      /// <param name="buf"></param>
      /// <param name="len"></param>
      /// <returns>0 (EOF) whenever GS requests input</returns>
      protected API.StdinCallback EmptyStdinCallback = new API.StdinCallback(delegate(IntPtr caller_handle, IntPtr buf, Int32 len)
      {
         return 0; // return EOF always
      });
      //protected API.StdinCallback EmptyStdinCallback = (caller_handle, buf, len) =>
      //{
      //   return 0; // return EOF always
      //};

      /// <summary>
      /// "Default" implementation of StdoutCallback - does nothing with output, returns all characters handled
      /// </summary>
      /// <param name="caller_handle"></param>
      /// <param name="buf"></param>
      /// <param name="len"></param>
      /// <returns>len (the number of characters handled) whenever GS outputs anything</returns>
      protected API.StdoutCallback EmptyStdoutCallback = (caller_handle, buf, len) =>
      {
         return len; // return all bytes handled
      };

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         UnloadGhostscriptLibrary();
      }

      #endregion

      #region Convenience Methods

      /// <summary>
      /// Convert a postscript file to a pdf
      /// </summary>
      /// <param name="outputPath">The path to create the output file. Put '%d' the path to create multiple numbered files, one for each page</param>
      /// <param name="inputPaths">One or more input files</param>
      public static void PsToPdf(String outputPath, params String[] inputPaths)
      {
         GhostscriptManager gsm = GhostscriptManager.GetInstance();
         bool libraryLoaded = (gsm.libraryHandle != IntPtr.Zero);
         if (!libraryLoaded)
         {
            gsm.LoadGhostscriptLibrary();
         }
         GhostscriptSettings oldSettings = gsm.Settings;
         gsm.settings = new GhostscriptSettings();
         gsm.Settings.Device = GhostscriptSharp.Settings.GhostscriptDevices.pdfwrite;
         gsm.Settings.Page.AllPages = true;
         gsm.Settings.Quiet = true;
         gsm.DoConvert(outputPath, inputPaths);
         if (!libraryLoaded)
         {
            gsm.UnloadGhostscriptLibrary();
         }
         gsm.settings = oldSettings;
      }

      #endregion
   }
}
