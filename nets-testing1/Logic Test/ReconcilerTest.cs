using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using nets.dataclass;
using nets.logic;

namespace nets_testing1.Logic
{
    [TestClass()]
    public class ReconcilerTest
    {
        public static Reconciler r;
        public static string src;
        public static string des;
        List<FilePair> list;
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value;}
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
            r = new Reconciler();
            src = TestHelp.src;
            des = TestHelp.des;
            TestHelp.TestClassInitialize();
            Assert.IsTrue(Directory.Exists(src));
            Assert.IsTrue(Directory.Exists(des));
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestHelp.TestClassCleanup();
            Assert.IsFalse(Directory.Exists(TestHelp.AppDataFolder));
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestHelp.TestClassInitialize();
            Assert.IsTrue(Directory.Exists(src));
            Assert.IsTrue(Directory.Exists(des));
        }


        [TestMethod()]
        // testing new and updated filepair
        public void EQ_Using_Last_Modified_Date()
        {
            DateTime d = File.GetLastAccessTime(src);
            string newfile = src + "newfile.txt";
            string updatedfile = des + "updatedfile.txt";
            
            TestHelp.CheckAndCreateFile(src + newfile);
            TestHelp.CheckAndCreateFile(des + newfile);
            TestHelp.CheckAndCreateFile(src + updatedfile);
            TestHelp.CheckAndCreateFile(des + updatedfile);
            
            File.SetLastWriteTimeUtc(src + newfile, d);
            File.SetLastWriteTimeUtc(des + updatedfile, d);
            
            list = new List<FilePair>();
            list.Add(new FilePair(src, des, FileStatus.New, FileStatus.New, FileType.File));
            list.Add(new FilePair(src, des, FileStatus.Updated, FileStatus.Updated, FileType.File));
            //r.Synchronize(ref list, SyncMode.Equalize);

            Assert.AreNotEqual(File.GetLastWriteTimeUtc(src + newfile), d);
            Assert.AreNotEqual(File.GetLastWriteTimeUtc(des + updatedfile), d);
        }

        [TestMethod()]
        public void EQ_Using_No_Change()
        {
            DateTime d = DateTime.Now;

            string nochange = "same_LMD.txt";

            TestHelp.CheckAndCreateFile(src + nochange);
            TestHelp.CheckAndCreateFile(des + nochange);

            File.SetLastWriteTimeUtc(src + nochange, d);
            File.SetLastWriteTimeUtc(des + nochange, d);

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, nochange, FileStatus.NoChange, FileStatus.NoChange, FileType.File));
            r.Synchronize(ref list, SyncMode.Equalize);

            string s1 = TestHelp.GetFileContent(src + nochange).ToString();
            string s2 = TestHelp.GetFileContent(des + nochange).ToString();

            Assert.AreEqual(File.GetLastWriteTimeUtc(src + nochange), File.GetLastWriteTimeUtc(des + nochange));
            Assert.AreNotEqual(s1, s2);
        }

        [TestMethod()]
        public void EQ_Using_Delete()
        {
            string srcFile = "srcfile.txt";
            string desFile = "desfile.txt";

            TestHelp.CheckAndCreateFile(src + srcFile);
            TestHelp.CheckAndCreateFile(des + desFile);

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, srcFile, FileStatus.NoChange, FileStatus.Deleted, FileType.File));
            list.Add(new FilePair(src, des, desFile, FileStatus.Deleted, FileStatus.NoChange, FileType.File));
            r.Synchronize(ref list, SyncMode.Equalize);

            Assert.IsFalse(File.Exists(src + srcFile));
            Assert.IsFalse(File.Exists(des + desFile));
        }

        [TestMethod()]
        public void EQ_Using_Moving_File()
        {
            DateTime d = new DateTime(2000, 1, 1);

            string movingfile1 = "update_nochange.txt";
            string movingfile2 = "update_delete.txt";
            string movingfile3 = "new_notexisted.txt";

            TestHelp.CheckAndCreateFile(src + movingfile1);
            TestHelp.CheckAndCreateFile(des + movingfile1);
            TestHelp.CheckAndCreateFile(des + movingfile2);
            TestHelp.CheckAndCreateFile(src + movingfile3);

            string sr1_string = TestHelp.GetFileContent(src + movingfile1).ToString();
            string sr2_string = TestHelp.GetFileContent(des + movingfile1).ToString();
            File.SetLastWriteTimeUtc(des + movingfile1, d);

            Assert.AreNotEqual(sr1_string, sr2_string);
            Assert.AreNotEqual(File.GetLastWriteTimeUtc(src + movingfile1), File.GetLastWriteTimeUtc(des + movingfile1));
            Assert.IsFalse(File.Exists(src + movingfile2));
            Assert.IsFalse(File.Exists(des + movingfile3));

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, movingfile1, FileStatus.Updated, FileStatus.NoChange, FileType.File));
            list.Add(new FilePair(src, des, movingfile2, FileStatus.Deleted, FileStatus.Updated, FileType.File));
            list.Add(new FilePair(src, des, movingfile3, FileStatus.New, FileStatus.NotExist, FileType.File));
            r.Synchronize(ref list, SyncMode.Equalize);

            sr1_string = TestHelp.GetFileContent(src + movingfile1).ToString();
            sr2_string = TestHelp.GetFileContent(des + movingfile1).ToString();

            Assert.AreEqual(sr1_string, sr2_string);
            Assert.AreEqual(File.GetLastWriteTimeUtc(src + movingfile1), File.GetLastWriteTimeUtc(des + movingfile1));
            Assert.IsTrue(File.Exists(src + movingfile2));
            Assert.IsTrue(File.Exists(des + movingfile3));
        }

        [TestMethod()]
        // testing new and updated filepair
        public void Mirror_Using_Last_Modified_Date()
        {
            DateTime d = File.GetLastAccessTime(src);
            string newfile = "newfile.txt";
            string updatedfile = "updatedfile.txt";

            TestHelp.CheckAndCreateFile(src + newfile);
            TestHelp.CheckAndCreateFile(des + newfile);
            TestHelp.CheckAndCreateFile(src + updatedfile);
            TestHelp.CheckAndCreateFile(des + updatedfile);

            File.SetLastWriteTimeUtc(src + newfile, d);
            File.SetLastWriteTimeUtc(des + updatedfile, d);

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, newfile, FileStatus.New, FileStatus.New, FileType.File));
            list.Add(new FilePair(src, des, updatedfile, FileStatus.Updated, FileStatus.Updated, FileType.File));
            r.Synchronize(ref list, SyncMode.Mirror);

            Assert.AreNotEqual(File.GetLastWriteTimeUtc(src + newfile), d);
            Assert.AreNotEqual(File.GetLastWriteTimeUtc(des + updatedfile), d);
        }

        [TestMethod()]
        public void Mirror_Using_No_Change()
        {
            DateTime d = DateTime.Now;

            string nochange = "same_LMD.txt";
            string onlyindes1 = "only_in_des1.txt";
            string onlyindes2 = "only_in_des2.txt";

            TestHelp.CheckAndCreateFile(src + nochange);
            TestHelp.CheckAndCreateFile(des + nochange);
            TestHelp.CheckAndCreateFile(des + onlyindes1);
            TestHelp.CheckAndCreateFile(des + onlyindes2);
            Assert.IsTrue(File.Exists(src + nochange));
            Assert.IsTrue(File.Exists(des + nochange));
            Assert.IsTrue(File.Exists(des + onlyindes1));
            Assert.IsTrue(File.Exists(des + onlyindes2));

            File.SetLastWriteTimeUtc(src + nochange, d);
            File.SetLastWriteTimeUtc(des + nochange, d);

            string s1 = TestHelp.GetFileContent(src + nochange).ToString();
            string s2 = TestHelp.GetFileContent(des + nochange).ToString();

            Assert.AreNotEqual(s1, s2);


            list = new List<FilePair>();
            list.Add(new FilePair(src, des, nochange, FileStatus.NoChange, FileStatus.Updated, FileType.File));
            list.Add(new FilePair(src, des, onlyindes1, FileStatus.NotExist, FileStatus.NoChange, FileType.File));
            list.Add(new FilePair(src, des, onlyindes2, FileStatus.Deleted, FileStatus.Updated, FileType.File));
            r.Synchronize(ref list, SyncMode.Mirror);

            s1 = TestHelp.GetFileContent(src + nochange).ToString();
            s2 = TestHelp.GetFileContent(des + nochange).ToString();
            
            Assert.AreEqual(File.GetLastWriteTimeUtc(src + nochange), File.GetLastWriteTimeUtc(des + nochange));
            Assert.AreEqual(s1, s2);
            Assert.IsFalse(File.Exists(src + onlyindes1));
            Assert.IsFalse(File.Exists(src + onlyindes2));
        }

        [TestMethod()]
        public void Mirror_Using_Delete()
        {
            string updatedFile = "updated.txt";
            string nochangefile = "nochange.txt";

            TestHelp.CheckAndCreateFile(des + updatedFile);
            TestHelp.CheckAndCreateFile(des + nochangefile);

            Assert.IsFalse(File.Exists(src + updatedFile));
            Assert.IsFalse(File.Exists(src + nochangefile));
            Assert.IsTrue(File.Exists(des + updatedFile));
            Assert.IsTrue(File.Exists(des + nochangefile));

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, updatedFile, FileStatus.Deleted, FileStatus.Updated, FileType.File));
            list.Add(new FilePair(src, des, nochangefile, FileStatus.Deleted, FileStatus.NoChange, FileType.File));
            r.Synchronize(ref list, SyncMode.Mirror);

            Assert.IsFalse(File.Exists(src + updatedFile));
            Assert.IsFalse(File.Exists(src + nochangefile));
            Assert.IsFalse(File.Exists(des + updatedFile));
            Assert.IsFalse(File.Exists(des + nochangefile));
        }

        /*[TestMethod()]
        public void Mirror_Using_Moving_File()
        {
            DateTime d = File.GetLastAccessTime(src);

            string movingfile1 = "nochange_update.txt";
            string movingfile2 = "nochange_delete.txt";
            string movingfile3 = "new_notexist.txt";

            TestHelp.CheckAndCreateFile(src + movingfile1);
            TestHelp.CheckAndCreateFile(des + movingfile1);
            TestHelp.CheckAndCreateFile(des + movingfile2);
            TestHelp.CheckAndCreateFile(src + movingfile3);


            string sr1_string = TestHelp.GetFileContent(src + movingfile1).ToString();
            string sr2_string = TestHelp.GetFileContent(des + movingfile1).ToString();
            File.SetLastWriteTimeUtc(des + movingfile1, d);

            Assert.AreNotEqual(sr1_string, sr2_string);
            Assert.AreNotEqual(File.GetLastWriteTimeUtc(src + movingfile1), File.GetLastWriteTimeUtc(des + movingfile1));
            Assert.IsFalse(File.Exists(src + movingfile2));

            list = new List<FilePair>();
            list.Add(new FilePair(src, des, movingfile1, FileStatus.Updated, FileStatus.NoChange, FileType.File));
            list.Add(new FilePair(src, des, movingfile2, FileStatus.Deleted, FileStatus.Updated, FileType.File));
            r.Synchronize(ref list, SyncMode.Mirror);

            sr1_string = TestHelp.GetFileContent(src + movingfile1).ToString();
            sr2_string = TestHelp.GetFileContent(des + movingfile1).ToString();

            Assert.AreEqual(sr1_string, sr2_string);
            Assert.AreEqual(File.GetLastWriteTimeUtc(src + movingfile1), File.GetLastWriteTimeUtc(des + movingfile1));
        }*/

        /*[TestMethod()]
        public void IncompatibleFilePair()
        {
            string newsrc = src + @"Test3\";
            if (!Directory.Exists(newsrc))
                Directory.CreateDirectory(newsrc);
            TestHelp.CheckAndCreateFile(des + "a.txt");
            Detector d = new Detector();
            FilterPattern f = new FilterPattern("", "");
            
            List<FilePair> pairs = d.FindDifferences(src, des, ref f);
            r.Synchronize(ref pairs, SyncMode.Equalize);
            Assert.IsTrue(File.Exists(src + "a.txt"));

            File.Delete(src + "a.txt");
            pairs = d.FindDifferences(src, des, ref f);
            r.Synchronize(ref pairs, SyncMode.Equalize);
            Assert.IsTrue(!File.Exists(src + "a.txt"));

            pairs = d.FindDifferences(newsrc, des, ref f);
            r.Synchronize(ref pairs, SyncMode.Equalize);
            Assert.IsTrue(File.Exists(newsrc + "a.txt"));
        }*/

        //
        // testing validity of filepair not implemented
        // testing mirror mode not implemented
    }
}
