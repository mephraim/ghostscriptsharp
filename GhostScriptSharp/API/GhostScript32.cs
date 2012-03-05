using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GhostscriptSharp.API
{
    internal class GhostScript32
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
        /// Calls the Ghostscript API with a collection of arguments to be passed to it
        /// </summary>
        public static void CallAPI(string[] args)
        {
            // Get a pointer to an instance of the Ghostscript API and run the API with the current arguments
            IntPtr gsInstancePtr;
            lock (resourceLock)
            {
                CreateAPIInstance(out gsInstancePtr, IntPtr.Zero);
                try
                {
                    int result = InitAPI(gsInstancePtr, args.Length, args);

                    if (result < 0)
                    {
                        throw new ExternalException("Ghostscript conversion error", result);
                    }
                }
                finally
                {
                    Cleanup(gsInstancePtr);
                }
            }
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
        /// GS can only support a single instance, so we need to bottleneck any multi-threaded systems.
        /// </summary>
        private static object resourceLock = new object();
    }
}
