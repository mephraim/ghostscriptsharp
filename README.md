# GhostscriptSharp
GhostscriptSharp is a simple C# wrapper for the Ghostscript library.

# Using GhostscriptSharp
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
* **settings** _a collection of settings for the output (see source for more details)_



