using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GhostscriptSharp
{
	/// <summary>
	/// Ghostscript settings
	/// </summary>
	public class GhostscriptSettings : ICloneable
   {
      #region Constants

      public const Settings.GhostscriptDevices DefaultDevice = GhostscriptSharp.Settings.GhostscriptDevices.pdfwrite;
      public const Settings.GhostscriptPageSizes DefaultPageSize = GhostscriptSharp.Settings.GhostscriptPageSizes.letter;

      #endregion

		protected Settings.GhostscriptDevices _device;
      protected Settings.GhostscriptPages _pages = new Settings.GhostscriptPages();
      protected System.Drawing.Size _resolution;
      protected Settings.GhostscriptPageSize _size = new Settings.GhostscriptPageSize();

		public bool Quiet { get; set; }

		public Settings.GhostscriptDevices Device
		{
			get { return this._device; }
			set { this._device = value; }
		}

		public Settings.GhostscriptPages Page
		{
			get { return this._pages; }
			set { this._pages = value; }
		}

		public System.Drawing.Size Resolution
		{
			get { return this._resolution; }
			set { this._resolution = value; }
		}

		public Settings.GhostscriptPageSize Size
		{
			get { return this._size; }
			set { this._size = value; }
		}

      protected int _numRenderingThreads;
      /// <summary>
      /// Number of threads to use for rendering. Default is the number of logical processors on the system.
      /// </summary>
      public int NumRenderingThreads 
      {
         get
         {
            if (_numRenderingThreads < 1)
            {
               try
               {
                  _numRenderingThreads = System.Environment.ProcessorCount;
               }
               catch // if the user doesn't have access...
               {
                  _numRenderingThreads = 1;
               }
            }
            return _numRenderingThreads;
         }
         set
         {
            _numRenderingThreads = value;
         }
      }

		public GhostscriptSettings()
		{
			this.Quiet = true;
         this._numRenderingThreads = -1;
         this.Device = DefaultDevice;
         this.Size.Native = DefaultPageSize;
		}

      public IEnumerable<String> GetGhostscriptArgs()
      {
         List<String> args = new List<string>();

         // Quiet?
         if (this.Quiet)
         {
            args.Add("-q");
            args.Add("-dQUIET");
         }

         // Output device
         String sDevice = (this.Device == GhostscriptSharp.Settings.GhostscriptDevices.UNDEFINED) ? DefaultDevice.ToString() : this.Device.ToString();
         args.Add(String.Format("-sDEVICE={0}", sDevice));

         // Pages to output
         if (this.Page.AllPages)
         {
            args.Add("-dFirstPage=1");
         }
         else
         {
            args.Add(String.Format("-dFirstPage={0}", this.Page.Start));
            if (this.Page.End >= this.Page.Start)
            {
               args.Add(String.Format("-dLastPage={0}", this.Page.End));
            }
         }

         // Page size
         if (this.Size.Native == GhostscriptSharp.Settings.GhostscriptPageSizes.UNDEFINED && this.Size.Manual != System.Drawing.Size.Empty)
         {
            args.Add(String.Format("-dDEVICEWIDTHPOINTS={0}", this.Size.Manual.Width));
            args.Add(String.Format("-dDEVICEHEIGHTPOINTS={0}", this.Size.Manual.Height));
         }
         else
         {
            String sSize = (this.Size.Native == GhostscriptSharp.Settings.GhostscriptPageSizes.UNDEFINED) ? GhostscriptSettings.DefaultPageSize.ToString() : this.Size.Native.ToString();
            args.Add(String.Format("-sPAPERSIZE={0}", sSize));
         }

         // Page resolution
         if (this.Resolution != System.Drawing.Size.Empty)
         {
            args.Add(String.Format("-dDEVICEXRESOLUTION={0}", this.Resolution.Width));
            args.Add(String.Format("-dDEVICEYRESOLUTION={0}", this.Resolution.Height));
         }

         // Multithreaded Rendering
         if (NumRenderingThreads > 1)
         {
            args.Add(String.Format("-dNumRenderingThreads={0}", NumRenderingThreads));
         }

         // Additional device args
         args.AddRange(Settings.GhostscriptDeviceUtils.GetDefaultArgs(this.Device));

         return args;
      }

      #region ICloneable Members

      public object Clone()
      {
         return this.MemberwiseClone();
      }

      #endregion
   }
}