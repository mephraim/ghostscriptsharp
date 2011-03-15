# GhostscriptSharp
GhostscriptSharp is a simple C# wrapper for the Ghostscript library.

# Using the Ghostscript API
The API Class provides a basic interface to the Ghostscript API, as well as a GhostscriptErrorCode enum and a GS_Revision struct

## Static Methods

See the [Ghostscript API Documentation](http://pages.cs.wisc.edu/~ghost/doc/cvs/API.htm "Ghostscript API") for full information on these methods.

See the GhostscriptSharpExamples solution for ports of the Example code shown on the API page.

`Int32 GetRevision(out GS_Revision pr, Int32 len)`<br/>
`Int32 CreateAPIInstance(out IntPtr pinstance, IntPtr caller_handle)`<br/>
`void DeleteAPIInstance(IntPtr instance)`<br/>
`Int32 Set_Stdio(IntPtr instance, StdinCallback stdin, StdoutCallback stdout, StdoutCallback stderr)`<br/>
`Int32 InitAPI(IntPtr instance, Int32 argc, string[] argv)`<br/>
`Int32 RunStringBegin(IntPtr instance, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 RunStringContinue(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 RunStringEnd(IntPtr instance, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 RunStringWithLength(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 RunString(IntPtr instance, String str, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 RunFile(IntPtr instance, String file_name, Int32 user_errors, out Int32 pexit_code)`<br/>
`Int32 ExitAPI(IntPtr instance)`

# Using GhostscriptWrapper
The GhostscriptWrapper class contains 3 static methods that can be used to generate jpg images from a PDF file.

## Generate an Image for a Single Page

This method will generate a single thumbnail for a single page at the given output path

`GeneratePageThumb(string inputPath, string outputPath, int page, int width, int height)`

### Parameters

* **inputPath** _a path to the PDF file_
* **outputPath** _the path where you would like the output jpg_
* **page** _the page number for the page you want to create an image from_
* **width** _the width of the image_
* **height** _the height of the image_

## Generate Multiple Images for Multiple Pages

This method generates a collection of thumbnail jpgs for the PDF at the input path, starting with firstPage and ending with lastPage.
Put "%d" somewhere in the output path to have each of the pages numbered.

`GeneratePageThumbs(string inputPath, string outputPath, int firstPage, int lastPage, int width, int height)`

### Parameters

* **inputPath** _a path to the PDF file_
* **ouputPath** _the path where you would like the output jpgs to go (put '%d' somewhere in the path to have the jpgs numbered)_
* **firstPage** _the first page to start generating the thumbnails from_
* **lastPage** _the last page to end the thumbnail generator_
* **width** _the width of the image_
* **height** _the height of the image_

## Generate Output Based on Settings

This method generates Ghostscript output at the given output path based on a collection of GhostscriptSettings. 
(See the source for a list of possible settings)

`GenerateOutput(string inputPath, string outputPath, GhostscriptSettings settings)`

### Parameters

* **inputPath** _a path to the PDF file_
* **outputPath** _the path where you would like the output jpg_
* **settings** _a collection of settings for the output (see source for more details)

# Using GhostscriptManager
The GhostscriptManager class provides an OO interface to converting files and obtaining Ghostscript output.

* Set the static **GhostscriptLibraryPath** property to automatically load the gsdll32 library from a specified path
* Use the **Settings** property to specify the output device, page size, and other parameters
* Use the **StdOut** and **StdErr** events to obtain Ghostscript output

`void DoConvert(String outputPath, params String[] inputPaths)`

### Parameters
* **outputPath** The path where you would like your converted file(s) saved. Put "%d" somewhere in the path to generate multiple numbered files.
* **inputPaths** One or more input file(s) in PS or PDF format





