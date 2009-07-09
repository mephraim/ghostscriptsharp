using System;
using NUnit.Framework;
using GhostscriptSharp;
using System.IO;

namespace GhostscriptSharpTests
{
    [TestFixture]
    public class GhostscriptSharpTests
    {
        private readonly string TEST_FILE_LOCATION      = "test.pdf";
        private readonly string SINGLE_FILE_LOCATION    = "output.jpg";
        private readonly string MULTIPLE_FILE_LOCATION  = "output%d.jpg";

        private readonly int MULTIPLE_FILE_PAGE_COUNT = 10;

        [Test]
        public void GenerateSinglePageThumbnail()
        {
            GhostscriptWrapper.GeneratePageThumb(TEST_FILE_LOCATION, SINGLE_FILE_LOCATION, 1, 100, 100);
            Assert.IsTrue(File.Exists(SINGLE_FILE_LOCATION));
        }

        [Test]
        public void GenerateMultiplePageThumbnails()
        {
            GhostscriptWrapper.GeneratePageThumbs(TEST_FILE_LOCATION, MULTIPLE_FILE_LOCATION, 1, MULTIPLE_FILE_PAGE_COUNT, 100, 100);
            for (var i = 1; i <= MULTIPLE_FILE_PAGE_COUNT; i++)
                Assert.IsTrue(File.Exists(String.Format("output{0}.jpg", i)));
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete(SINGLE_FILE_LOCATION);
            for (var i = 1; i <= MULTIPLE_FILE_PAGE_COUNT; i++)
                File.Delete(String.Format("output{0}.jpg", i));
        }
    }
}
