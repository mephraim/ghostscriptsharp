using System;
using System.Collections.Generic;

using System.Text;

namespace GhostscriptSharp.Settings
{
   /// <summary>
   /// Output devices for GhostScript
   /// </summary>
   public enum GhostscriptDevices
   {
      UNDEFINED,
      png16m,
      pnggray,
      png256,
      png16,
      pngmono,
      pngalpha,
      jpeg,
      jpeggray,
      tiffgray,
      tiff12nc,
      tiff24nc,
      tiff32nc,
      tiffsep,
      tiffcrle,
      tiffg3,
      tiffg32d,
      tiffg4,
      tifflzw,
      tiffpack,
      faxg3,
      faxg32d,
      faxg4,
      bmpmono,
      bmpgray,
      bmpsep1,
      bmpsep8,
      bmp16,
      bmp256,
      bmp16m,
      bmp32b,
      pcxmono,
      pcxgray,
      pcx16,
      pcx256,
      pcx24b,
      pcxcmyk,
      psdcmyk,
      psdrgb,
      pdfwrite,
      pswrite,
      epswrite,
      pxlmono,
      pxlcolor
   }

   public class GhostscriptDeviceUtils
   {
      private GhostscriptDeviceUtils()
      {
      }

      public static IEnumerable<String> GetDefaultArgs(GhostscriptDevices device)
      {
         switch (device)
         {
            case GhostscriptDevices.jpeg:
               return new String[] { 
                  "-dAlignToPixels=0",
                  "-dGridFitTT=0",
                  "-dTextAlphaBits=4",
                  "-dGraphicsAlphaBits=4"
               };
            default:
               return new String[0];
         }
      }
   }
}