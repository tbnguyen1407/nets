using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.dataclass;
using nets.logic;

namespace DetectorTest_SingleFile
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class File_NoChange_Deleted
    {
        string path2 = @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\One-file folder\";
        string folder1 = "Test1";
        string folder2 = "Test2";
        string dir1, dir2;
        
        public File_NoChange_Deleted()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestInitialize()]
        public void Initialize()
        {
            dir1 = Path.Combine(path2, folder1);
            dir2 = Path.Combine(path2, folder2);

            Function.initSame(folder1, folder2, 2);
            string filePath = Path.Combine(dir2, "a.txt");
            File.Delete(filePath);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Function.finalize(folder1, folder2, 2);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]

        /* ===================== */
        public void NoChange__Deleted()
        {
            FilterPattern filter = new FilterPattern("","");
            Detector detector = new Detector();
            List<FilePair> list = detector.FindDifferences(dir1, dir2, ref filter);

            if (list.Count == 0)
                Assert.AreEqual("1", "0", "NoChange-Deleted test case failed: No differences detected.");
            else
            {
                FilePair pair = list.ElementAt<FilePair>(0);

                Assert.AreEqual(FileStatus.NoChange, pair.FileStatusInSrc, 
                    "NoChange-Deleted test case failed: Source File Status is: " + pair.FileStatusInSrc);
                Assert.AreEqual(FileStatus.Deleted, pair.FileStatusInDes, 
                    "NoChange-Deleted test case failed: Destination File Status is: " + pair.FileStatusInDes);
           
            }
        }
    }
}
