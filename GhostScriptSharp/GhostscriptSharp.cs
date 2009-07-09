using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GhostscriptSharp
{
    /// <summary>
    /// Wraps the Ghostscript API with a C# interface
    /// </summary>
    public class GhostscriptWrapper
    {
        #region Hooks into Ghostscript DLL
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        private static extern int CreateAPIInstance(out IntPtr pinstance, IntPtr caller_handle);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_init_with_args")]
        private static extern int InitAPI(IntPtr instance, int argc, string[] argv);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_exit")]
        private static extern int ExitAPI(IntPtr instance);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_delete_instance")]
        private static extern void DeleteAPIInstance(IntPtr instance);
        #endregion

        /// <summary>
        /// Generates a thumbnail jpg for the pdf at the input path and saves it 
        /// at the output path
        /// </summary>
        public static void GeneratePageThumb(string inputPath, string outputPath, int page, int width, int height)
        {
            GeneratePageThumbs(inputPath, outputPath, page, page, width, height);
        }

        /// <summary>
        /// Generates a collection of thumbnail jpgs for the pdf at the input path 
        /// starting with firstPage and ending with lastPage.
        /// Put "%d" somewhere in the output path to have each of the pages numbered
        /// </summary>
        public static void GeneratePageThumbs(string inputPath, string outputPath, int firstPage, int lastPage, int width, int height)
        {
            CallAPI(GetArgs(inputPath, outputPath, firstPage, lastPage, width, height));
        }

        /// <summary>
        /// Calls the Ghostscript API with a collection of arguments to be passed to it
        /// </summary>
        private static void CallAPI(string[] args)
        {
            // Get a pointer to an instance of the Ghostscript API and run the API with the current arguments
            IntPtr gsInstancePtr;
            CreateAPIInstance(out gsInstancePtr, IntPtr.Zero);
            InitAPI(gsInstancePtr, args.Length, args);

            Cleanup(gsInstancePtr);
        }

        /// <summary>
        /// Frees up the memory used for the API arguments and clears the Ghostscript API instance
        /// </summary>
        private static void Cleanup(IntPtr gsInstancePtr)
        {
            ExitAPI(gsInstancePtr);
            DeleteAPIInstance(gsInstancePtr);
        }

        /// <summary>
        /// Returns an array of arguments to be sent to the Ghostscript API
        /// </summary>
        /// <param name="inputPath">Path to the source file</param>
        /// <param name="outputPath">Path to the output file</param>
        /// <param name="firstPage">The page of the file to start on</param>
        /// <param name="lastPage">The page of the file to end on</param>
        private static string[] GetArgs(string inputPath, string outputPath, int firstPage, int lastPage, int width, int height)
        {
            return new[]
            {
                // Keep gs from writing information to standard output
                "-q",                     
                "-dQUIET",
               
                "-dPARANOIDSAFER",       // Run this command in safe mode
                "-dBATCH",               // Keep gs from going into interactive mode
                "-dNOPAUSE",             // Do not prompt and pause for each page
                "-dNOPROMPT",            // Disable prompts for user interaction           
                "-dMaxBitmap=500000000", // Set high for better performance
                
                // Set the starting and ending pages
                String.Format("-dFirstPage={0}", firstPage),
                String.Format("-dLastPage={0}", lastPage),   
                
                // Configure the output anti-aliasing, resolution, etc
                "-dAlignToPixels=0",
                "-dGridFitTT=0",
                "-sDEVICE=jpeg",
                "-dTextAlphaBits=4",
                "-dGraphicsAlphaBits=4",
                String.Format("-r{0}x{1}", width, height),

                // Set the input and output files
                String.Format("-sOutputFile={0}", outputPath),
                inputPath
            };
        }
    }
}
