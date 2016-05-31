using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.dataclass;
using nets.storage;

namespace nets_testing1.Storage
{
    [TestClass()]
    public class LoggerHolderTest
    {
        public static string logPath;
        public static LoggerHolder lh;

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get{return testContextInstance;}
            set{testContextInstance = value;}
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
            logPath = StorageFacade.LogFolder;
            TestHelp.CheckAndCreateFolder(logPath);
            Assert.IsTrue(Directory.Exists(logPath));
            lh = LoggerHolder.GetInstance();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            if (File.Exists(logPath))
                File.Delete(logPath);
        }


        [TestMethod()]
        public void SaveLoggerTest()
        {
            Logger l = lh.GetLogger("testlogger");
            l.Log(LogType.DEBUG, "test info");
            lh.SaveLogger();
            Assert.IsTrue(File.Exists(logPath + "testlogger.dat"));
            string s1 = TestHelp.GetFileContent(logPath + "testlogger.dat").ToString();
            //System.Windows.Forms.MessageBox.Show(s1);
            l.Log(LogType.FATAL, "this is something else");
            lh.SaveLogger();
            string s2 = TestHelp.GetFileContent(logPath + "testlogger.dat").ToString();
            //System.Windows.Forms.MessageBox.Show(s2);

            Assert.IsTrue(s2.Contains(s1.Trim()));
            Assert.IsTrue(s2.Contains(l.GetInfo().Trim()));
        }


        [TestMethod()]
        public void GetLoggerIndexTest()
        {
            Logger l = lh.GetLogger("testlogger");
            int i = lh.GetLoggerIndex("testlogger");
            Assert.IsTrue(i > -1);
            i = lh.GetLoggerIndex("newlogger");
            Assert.IsFalse(i > -1);
        }


        [TestMethod()]
        public void GetLoggerTest()
        {
            Logger l1 = lh.GetLogger("test");
            Logger l2 = lh.GetLogger("another test");
            Logger l3 = lh.GetLogger("test");
            Assert.AreSame(l1, l3);
            Assert.AreNotSame(l1, l2);
        }
    }
}
