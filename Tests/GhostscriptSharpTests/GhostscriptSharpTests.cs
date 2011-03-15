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
        private readonly string MANAGER_SINGLE_FILE_LOCATION    = "man_output.jpg";
        private readonly string MANAGER_MULTIPLE_FILE_LOCATION = "man_output%d.jpg";

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

        [Test]
        public void CheckRevision()
        {
            Console.WriteLine("Check Revision Test");
            Console.WriteLine("Creating GhostsciprtManager...");
            using (GhostscriptManager gs = GhostscriptManager.GetInstance())
            {
                Assert.IsTrue(!String.IsNullOrEmpty(gs.ProductName));
                Console.WriteLine("Product: {0}", gs.ProductName);
                Assert.IsTrue(!String.IsNullOrEmpty(gs.Copyright));
                Console.WriteLine("Copyright: {0}", gs.Copyright);
                Assert.IsTrue(gs.Revision > 0);
                Console.WriteLine("Revision: {0}", gs.Revision);
                Assert.IsTrue(gs.RevisionDate > 19000101);
                Console.WriteLine("Revision Date: {0}", gs.RevisionDate);
            }
        }

        [Test]
        public void ManagerGenerateSinglePageThumbnail()
        {
            using (GhostscriptManager gs = GhostscriptManager.GetInstance())
            {
                gs.Settings.Device = GhostscriptSharp.Settings.GhostscriptDevices.jpeg;
                gs.Settings.Page.Start = 1;
                gs.Settings.Page.End = 1;
                gs.Settings.Resolution = new System.Drawing.Size(100, 100);
                gs.Settings.Size.Native = GhostscriptSharp.Settings.GhostscriptPageSizes.a7;
                gs.DoConvert(MANAGER_SINGLE_FILE_LOCATION, TEST_FILE_LOCATION);
            }
            Assert.IsTrue(File.Exists(MANAGER_SINGLE_FILE_LOCATION));
        }

        [Test]
        public void ManagerGenerateMultiplePageThumbnails()
        {
            using (GhostscriptManager gs = GhostscriptManager.GetInstance())
            {
                gs.Settings.Device = GhostscriptSharp.Settings.GhostscriptDevices.jpeg;
                gs.Settings.Page.Start = 1;
                gs.Settings.Page.End = MULTIPLE_FILE_PAGE_COUNT;
                gs.Settings.Resolution = new System.Drawing.Size(100, 100);
                gs.Settings.Size.Native = GhostscriptSharp.Settings.GhostscriptPageSizes.a7;
                gs.DoConvert(MANAGER_MULTIPLE_FILE_LOCATION, TEST_FILE_LOCATION);
            }
            for (var i = 1; i <= MULTIPLE_FILE_PAGE_COUNT; i++)
                Assert.IsTrue(File.Exists(String.Format("man_output{0}.jpg", i)));
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(SINGLE_FILE_LOCATION))
            {
                File.Delete(SINGLE_FILE_LOCATION);
            }
            for (var i = 1; i <= MULTIPLE_FILE_PAGE_COUNT; i++)
            {
                String path = String.Format("output{0}.jpg", i);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            if (File.Exists(MANAGER_SINGLE_FILE_LOCATION))
            {
                File.Delete(MANAGER_SINGLE_FILE_LOCATION);
            }
            for (var i = 1; i <= MULTIPLE_FILE_PAGE_COUNT; i++)
            {
                String path = String.Format("man_output{0}.jpg", i);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
