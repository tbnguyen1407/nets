using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.storage;
using nets.utility;

namespace nets_testing1.Storage
{
    [TestClass()]
    public class StorageOperatorTest
    {
        public static string storePath;

        private TestContext testContextInstance;
        public TestContext TestContext
        { get{return testContextInstance; }
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

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestHelp.CleanDirectory(StorageFacade.AppDataFolder);
            TestHelp.CheckAndCreateFolder(StorageFacade.AppDataFolder);
            Assert.IsTrue(Directory.Exists(StorageFacade.AppDataFolder));
            storePath = StorageFacade.AppDataFolder + @"storagetest.txt";
            TestHelp.CheckAndCreateFile(storePath);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestHelp.CleanDirectory(StorageFacade.AppDataFolder);
        }

        [TestMethod()]
        public void AddOneEntry()
        {
            string s1 = "keyword1, this is a string";
            string s2 = "onlykeyword";
            StorageOperator.Add(s1, storePath);
            StorageOperator.Add(s2, storePath);
            string s = TestHelp.GetFileContent(storePath).ToString();
            Assert.IsTrue(s.Contains(s1));
            //Assert.IsNotNull(StorageOperator.Load(s2, storePath));
        }

        /*[TestMethod()]
        public void AddListContainNull()
        {
            List<string> s = new List<string>();
            s.Add("123");
            s.Add(null);
            StorageOperator.Add(s, storePath);
            Assert.IsNotNull(StorageOperator.Load("123", storePath));
        }*/

        [TestMethod()]
        public void Remove()
        {
            string s1 = "keyword1; this is a string";
            StorageOperator.Add(s1, storePath);
            string s2 = "keyword2; how do you know that it is a string?";
            StorageOperator.Add(s2, storePath);
            //StorageOperator.Remove("keyword2", storePath);

            string s = TestHelp.GetFileContent(storePath).ToString();
            Assert.IsFalse(s.Contains("keyword2"));
            Assert.IsTrue(s.Contains("keyword1"));
        }

        [TestMethod()]
        public void LoadNullAndEmpty()
        {
            Assert.IsNull(StorageOperator.Load(null, storePath));
            StorageOperator.Add(";test", storePath);
            Assert.IsNotNull(StorageOperator.Load("", storePath));
        }

        [TestMethod()]
        public void LoadOneEntry()
        {
            string s1 = "keyword1;this is a string";
            StorageOperator.Add(s1, storePath);
            string s2 = StorageOperator.Load("keyword1", storePath);
            Assert.IsTrue(s1.Contains(s2));
            s2 = StorageOperator.Load("keyword2", storePath);
            Assert.IsNull(s2);
        }

        [TestMethod()]
        public void LoadWholeFile()
        {
            string s1 = "keyword1; this is a string";
            StorageOperator.Add(s1, storePath);
            string s2 = "keyword2; how do you know that it is a string?";
            StorageOperator.Add(s2, storePath);
            string s3 = TestHelp.GetFileContent(storePath).ToString();
            Assert.IsTrue(s3.Contains(s1));
            Assert.IsTrue(s3.Contains(s2));
        }

        [TestMethod()]
        public void AddRemoveLoad()
        {
            string s1 = "keyword1; this is a string";
            StorageOperator.Add(s1, storePath);
            string s2 = "keyword2; how do you know that it is a string?";
            StorageOperator.Add(s2, storePath);
            StorageOperator.Remove("keyword2", storePath);
            
            Assert.IsNull(StorageOperator.Load("keyword2", storePath));
            Assert.IsNotNull(StorageOperator.Load("keyword1", storePath));
        }

        [TestMethod()]
        public void CheckAndCreateFileTest1()
        {
            if (File.Exists(storePath))
                File.Delete(storePath);
            FileSystemOperator.CheckAndCreateFile(storePath);
            Assert.IsTrue(File.Exists(storePath));

            try
            {
                FileSystemOperator.CheckAndCreateFile("");
                Assert.Fail("create file at whose path is empty");
            }
            catch (Exception e)
            {
            }
        }

        [TestMethod()]
        public void CheckAndCreateFileTest2()
        {
            string parentDir = StorageFacade.AppDataFolder;
            string fileDir1 = parentDir + @"Test1\test.txt";
            string fileDir2 = parentDir + @"Test2\Test22\Test222\test.txt";
            string fileDir3 = parentDir + "test.txt";
            Assert.IsTrue(Directory.Exists(parentDir));
            
            FileSystemOperator.CheckAndCreateFile(fileDir1);
            Assert.IsTrue(Directory.Exists(parentDir + @"Test1\"));
            Assert.IsTrue(File.Exists(fileDir1));

            FileSystemOperator.CheckAndCreateFile(fileDir2);
            Assert.IsTrue(Directory.Exists(parentDir + @"Test2\Test22\Test222\"));
            Assert.IsTrue(File.Exists(fileDir2));

            FileSystemOperator.CheckAndCreateFile(fileDir3);
            Assert.IsTrue(File.Exists(fileDir3));
        }
        /*[TestMethod()]
        public void DeleteAndCreateFileTest()
        {
            FileOperator.CheckAndCreateFile(storePath);
            StorageOperator.Add("123", storePath);
            StorageOperator.DeleteAndCreateFile(storePath);
            string s = GetFileContent(storePath).ToString();
            Assert.IsFalse(s.Contains("123"));

            if (File.Exists(storePath))
                File.Delete(storePath);
            StorageOperator.DeleteAndCreateFile(storePath);
            Assert.IsTrue(File.Exists(storePath));
        }*/
    }
}
