using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.dataclass;
using nets.logic;

namespace DetectorTest_SingleFile
{
    /// <summary>
    /// Summary description for Difference
    /// </summary>
    [TestClass]
    public class Difference
    {
        string path = @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\Diff\";
        string folder1 = "Test1";
        string folder2 = "Test2";
        string expected = "No Differences.";

        public Difference()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void DiffFolder()
        {
            string dir1 = System.IO.Path.Combine(path, folder1);
            string dir2 = System.IO.Path.Combine(path, folder2);

            Function.init(folder1, folder2);

            FilterPattern filter = new FilterPattern("","");
            Detector detector = new Detector();
            List<FilePair> list = detector.FindDifferences(dir1, dir2, ref filter);
            string actual;

            if (list.Count == 0)
                actual = "Wrong";
            else
            {
                actual = "Wrong.";
            }

            Assert.AreEqual(expected, actual, "Same folder test case failed.");
            Function.finalize(folder1, folder2, 1);
        }
    }
}
