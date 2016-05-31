using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nets.dataclass;
using nets.logic;
using nets.storage;

namespace nets_testing1.IntergratedTest
{
    [TestClass]
    public class IntergratedTest
    {
        static string src = TestHelp.src;
        static string des = TestHelp.des;
        static Detector d;
        static Reconciler r;
        static FilterPattern fp;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            d = new Detector();
            r = new Reconciler();
            fp = new FilterPattern("", "");
            TestHelp.TestClassInitialize();
            Assert.IsTrue(Directory.Exists(TestHelp.src));
            Assert.IsTrue(Directory.Exists(TestHelp.des));
        }
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestHelp.TestClassCleanup();
            Assert.IsFalse(Directory.Exists(StorageFacade.AppDataFolder));
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestHelp.TestClassInitialize();
            Assert.IsTrue(Directory.Exists(TestHelp.src));
            Assert.IsTrue(Directory.Exists(TestHelp.des));
        }

        [TestMethod()]
        public void Save_And_Retrieve_Profile()
        {
            //valid profile name
            Profile p1 = new Profile("testprofile", src, des, SyncMode.Equalize, "", "");
            LogicFacade.SaveProfile(p1);
            Profile p2 = LogicFacade.LoadProfile("testprofile");
            Assert.IsTrue(p2.SrcFolder.Equals(src));
            Assert.IsTrue(p2.DesFolder.Equals(des));

            //invalid profile name: name existed
            Profile p3 = new Profile("name_existed", src, des, SyncMode.Equalize, "", "");
            LogicFacade.SaveProfile(p3);
            Profile p4 = new Profile("name_existed", des, src, SyncMode.Equalize, "", "");
            LogicFacade.SaveProfile(p4);
            Profile p5 = LogicFacade.LoadProfile("name_existed");
            Assert.IsTrue(p5.SrcFolder.Equals(des));
            Assert.IsTrue(p5.DesFolder.Equals(src));

            //invalid profile name with ','
            Profile p6 = new Profile("test,profile", src, des, SyncMode.Equalize, "", "");
            LogicFacade.SaveProfile(p1);
            Profile p7 = LogicFacade.LoadProfile("test,profile");
            //Assert.IsTrue(p7.ProfileName.Equals("test,profile"));
        }

        public void Save_And_Retrieve_Metadata()
        {
        }

        [TestMethod]
        public void Basic_Sync()
        {
            // src: src.txt
            // des: des\des.txt
            TestHelp.CheckAndCreateFile(src + "src.txt");
            Directory.CreateDirectory(des + @"des\");
            TestHelp.CheckAndCreateFile(des + @"des\des.txt");
            Profile p = new Profile("testprofile", src, des, SyncMode.Equalize, fp);
            
            LogicFacade.Sync(p);
            
            Assert.IsTrue(File.Exists(des + "src.txt"));
            Assert.IsTrue(File.Exists(src + @"des\des.txt"));

            File.Delete(des + "src.txt");
            File.Delete(src + @"des\des.txt");

            //src: src.txt, des\
            //des: des\des.txt
            LogicFacade.Sync(p);
            Assert.IsFalse(File.Exists(src + "src.txt"));
            Assert.IsFalse(File.Exists(des + @"des\des.txt"));
            Assert.IsTrue(Directory.Exists(des + @"des\"));
        }

        [TestMethod()]
        public void Three_Flds_Sync()
        {
            string mid = TestHelp.AppDataFolder + @"Test\Test3\";
            TestHelp.CheckAndCreateFolder(mid);
            Assert.IsTrue(Directory.Exists(mid));

            TestHelp.CheckAndCreateFile(src + "src.txt");
            TestHelp.CheckAndCreateFile(mid + "mid.txt");
            TestHelp.CheckAndCreateFile(des + "des.txt");

            Profile p1 = new Profile("src_mid", src, mid, SyncMode.Equalize, fp);
            Profile p2 = new Profile("des_mid", des, mid, SyncMode.Equalize, fp);

            LogicFacade.Sync(p1);
            LogicFacade.Sync(p2);
            Assert.IsTrue(File.Exists(des + "src.txt"));
            Assert.IsFalse(File.Exists(src + "des.txt"));
            LogicFacade.Sync(p1);
            Assert.IsTrue(File.Exists(src + "des.txt"));
            
            //stt:      3 folders are similiar
            //action:   del 1 file in mid folder, sync with src and then des
            File.Delete(mid + "src.txt");
            LogicFacade.Sync(p1);
            LogicFacade.Sync(p2);
            Assert.IsFalse(File.Exists(mid + "src.txt"));
            Assert.IsFalse(File.Exists(src + "src.txt"));
            Assert.IsFalse(File.Exists(des + "src.txt"));

            TestHelp.CleanDirectory(mid);
        }
    }
}
