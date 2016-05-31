using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using nets.dataclass;
using nets.logic;

namespace nets_testing1.Logic
{
    //[TestClass()]
    public class DetectorTest
    {
        static Detector detector;
        static string src = TestHelp.src;
        static string dest = TestHelp.des;
        static FilterPattern pattern;
        static List<FilePair> diff;

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get{return testContextInstance;}
            set{ testContextInstance = value;}
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Assert.IsTrue(Directory.Exists(src));
            Assert.IsTrue(Directory.Exists(dest));
            detector = new Detector();
            pattern = new FilterPattern("", "");
        }

        [TestMethod()]
        public void InvalidInputsTest()
        {
            var logger = new Logger("blah");
            bool errorOccur = true;
            
            try
            {
                
                diff = detector.GetDifferences("", dest, ref logger, ref errorOccur);
                Assert.Fail("invalid src, throw LogicException");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid input");
            }

            try
            {
                diff = detector.GetDifferences(src, "", ref logger, ref errorOccur);
                Assert.Fail("invalid src, throw LogicException");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid input");
            }
        }

        [TestMethod()]
        public void DetectIdenticalFolder()
        {
            string fldName = "IdenticalFolder";
            
            Assert.IsTrue(Directory.Exists(src + @"\" + fldName));
            Assert.IsTrue(Directory.Exists(dest + @"\" + fldName));

            diff = detector.FindDifferences(src + @"\" + fldName, dest + @"\" + fldName, ref pattern);
            Assert.IsTrue(diff.Count == 0);
        }

        [TestMethod()]
        public void DetectOneDifference()
        {
            string fldName = "OneDifference";

            Assert.IsTrue(Directory.Exists(src + @"\" + fldName));
            Assert.IsTrue(Directory.Exists(dest + @"\" + fldName));

            diff = detector.FindDifferences(src + @"\" + fldName, dest + @"\" + fldName, ref pattern);
            Assert.IsTrue(diff.Count == 1);
        }

        [TestMethod()]
        public void DetectTwoDifferences()
        {
            string fldName = "TwoDifferences";

            Assert.IsTrue(Directory.Exists(src + @"\" + fldName));
            Assert.IsTrue(Directory.Exists(dest + @"\" + fldName));

            diff = detector.FindDifferences(src + @"\" + fldName, dest + @"\" + fldName, ref pattern);
            Assert.IsTrue(diff.Count == 2);
        }

        [TestMethod()]
        // 5000 similar files, files with 5000 differencs
        public void DetectManyDifferences()
        {
            string fldName = "ManyDifferences";

            Assert.IsTrue(Directory.Exists(src + @"\" + fldName));
            Assert.IsTrue(Directory.Exists(dest + @"\" + fldName));

            diff = detector.FindDifferences(src + @"\" + fldName, dest + @"\" + fldName, ref pattern);
            Assert.IsTrue(diff.Count == 5000);
        }

        [TestMethod()]
        // 0 similar file, 10000 difference files
        public void DetectOnlyDifferences()
        {
            string fldName = "OnlyDifferences";

            Assert.IsTrue(Directory.Exists(src + @"\" + fldName));
            Assert.IsTrue(Directory.Exists(dest + @"\" + fldName));

            diff = detector.FindDifferences(src + @"\" + fldName, dest + @"\" + fldName, ref pattern);
            Assert.IsTrue(diff.Count == 10000);
        }
    }
}
