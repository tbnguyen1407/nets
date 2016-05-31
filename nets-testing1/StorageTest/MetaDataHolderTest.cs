using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.storage;

namespace nets_testing1.Storage
{
    [TestClass()]
    public class MetaDataHolderTest
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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

        static MetaDataHolder mdh;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            mdh = MetaDataHolder.GetInstance();
            Assert.IsNotNull(mdh);
            TestHelp.CleanDirectory(StorageFacade.MetaDataFolder);
        }


        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestHelp.CleanDirectory(StorageFacade.MetaDataFolder);
            TestHelp.CheckAndCreateFolder(StorageFacade.MetaDataFolder);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestHelp.CleanDirectory(StorageFacade.MetaDataFolder);
        }

        [TestMethod()]
        public void Replace_Child_Entry_By_Parent_Entry()
        {
            string c_src = @"g:\Data";
            string c_des = @"G:\Data";
            string p_src = @"g:\";
            string p_des = @"G:\";
            string c_entry = c_src + "|" + c_des;
            string p_entry = p_src + "|" + p_des;
       
            //create child
            StorageFacade.InitializeMetaData(c_src, c_des);
            //Assert.IsTrue(mdh.Test_SearchEntry(c_src, c_des));
            //Assert.IsTrue(mdh.Test_SearchEntry(c_des, c_src));
            //Assert.IsFalse(mdh.Test_SearchEntry(p_src, p_des));
            //Assert.IsFalse(mdh.Test_SearchEntry(p_des, p_src));

            //create true parent
            //StorageFacade.InitializeMetadata(p_entry);
            //Assert.IsFalse(mdh.Test_SearchEntry(c_src, c_des));
            //Assert.IsFalse(mdh.Test_SearchEntry(c_des, c_src));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_src, p_des));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_des, p_src));

            //use child at other
            string other_c_entry = c_src + @"\BackUp" + "|" + c_des + @"\BackUp";
            StorageFacade.InitializeMetaData(c_src + @"\BackUp", c_des + @"\BackUp");
            //Assert.IsFalse(mdh.Test_SearchEntry(c_src + @"\BackUp", c_des + @"\BackUp"));
            //Assert.IsFalse(mdh.Test_SearchEntry(c_des + @"\BackUp", c_src + @"\BackUp"));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_src, p_des));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_des, p_src));

            //use non-proper child
            string nonproper_c_entry = c_src + "|" + c_des + @"\BackUp";
            StorageFacade.InitializeMetaData(c_src, c_des + @"\BackUp");
            //Assert.IsTrue(mdh.Test_SearchEntry(c_src, c_des + @"\BackUp"));
            //Assert.IsTrue(mdh.Test_SearchEntry(c_des + @"\BackUp", c_src));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_src, p_des));
            //Assert.IsTrue(mdh.Test_SearchEntry(p_des, p_src));
        }
    }
}
