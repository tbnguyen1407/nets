using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.dataclass;
using nets.logic;

namespace DetectorTest_SingleFile
{
    /// <summary>
    /// Summary description for New_NotExist
    /// </summary>
    [TestClass]
    public class New_NotExist
    {
        string path1 = @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\One-file folder\";
        string path = @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\Same\";
        string folder1 = "Test1";
        string folder2 = "Test2";
        string dir1, dir2,dir3;

        public New_NotExist()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestInitialize()]
        public void Initialize()
        {
            dir1 = Path.Combine(path1, folder1);
            dir2 = Path.Combine(path, folder2);
            if (Directory.Exists(dir2))
                Function.deleteFolderContent(dir2);
            else
                Directory.CreateDirectory(dir2);

            dir3 = Path.Combine(path, "Test3");
            if (Directory.Exists(dir3))
                Function.deleteFolderContent(dir3);
            else
                Directory.CreateDirectory(dir3);

            string filePath1 = Path.Combine(dir1, "a.txt");
            string filePath2 = Path.Combine(dir2, "a.txt");
            File.Copy(filePath1, filePath2);
            
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Function.deleteFolderContent(dir2);
            Directory.Delete(dir2);
            Function.deleteFolderContent(dir3);
            Directory.Delete(dir3);
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
        public void New__NotExist()
        {
            FilterPattern filter = new FilterPattern("","");
            Detector detector = new Detector();
            List<FilePair> list = detector.FindDifferences(dir1, dir3, ref filter);

            if (list.Count == 0)
                Assert.AreEqual("1", "0", "No difference list created.");
            else
            {
                foreach (FilePair filePair in list)
                {
                    Assert.AreEqual(FileStatus.New, filePair.FileStatusInSrc,
                        "Source File Status is: " + filePair.FileStatusInSrc);
                    Assert.AreEqual(FileStatus.NotExist, filePair.FileStatusInDes,
                        "Destination File Status is: " + filePair.FileStatusInDes);
                }
            }
        }
    }
}
