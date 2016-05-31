using System.IO;
using System.Text;
using System.Diagnostics;
using nets.storage;

namespace nets_testing1
{
    public static class TestHelp
    {
        public static readonly string AppDataFolder = StorageFacade.AppDataFolder;
        public static readonly string src = AppDataFolder + @"Test\Test1\";
        public static readonly string des = AppDataFolder + @"Test\Test2\";

        public static void TestClassInitialize()
        {
            CleanDirectory(AppDataFolder + @"\Test\");
            CheckAndCreateFolder(src);
            CheckAndCreateFolder(des);
            Debug.Assert(Directory.Exists(src));
            Debug.Assert(Directory.Exists(des));
        }

        public static void TestClassCleanup()
        {
            CleanDirectory(AppDataFolder);
            Debug.Assert(!Directory.Exists(AppDataFolder));
        }

        public static void CleanDirectory(string dir)
        {
            if (Directory.Exists(dir)) {
                string[] files = Directory.GetFiles(dir);
                string[] dirs = Directory.GetDirectories(dir);
                for (int i = 0; i < files.Length; i++)
                    File.Delete(files[i]);
                for (int i = 0; i < dirs.Length; i++)
                    CleanDirectory(dirs[i]);
                Directory.Delete(dir);
            }
        }

        public static void CleanFile(string filedir)
        {
            if (File.Exists(filedir))
                File.Delete(filedir);
        }

        public static bool CheckAndCreateFile(string fileLocation)
        {
            if (File.Exists(fileLocation))
                return true;
            CheckAndCreateFolder(Directory.GetParent(fileLocation).ToString());
            StreamWriter sw = File.CreateText(fileLocation);
            sw.WriteLine(fileLocation);
            sw.Close();
            return false;
        }

        public static void CheckAndCreateFolder(string fldLocation)
        {
            string[] s = fldLocation.Split('\\');
            string cur = "";
            for (int i = 0; i < s.Length; i++)
            {
                string temp = cur + s[i] + @"\";
                cur = temp;
                if (!Directory.Exists(cur))
                    Directory.CreateDirectory(cur);

            }
        }

        public static StringBuilder GetFileContent(string fileName)
        {
            StreamReader sr = File.OpenText(fileName);
            StringBuilder sb = new StringBuilder();
            string s;
            while ((s = sr.ReadLine()) != null)
                sb.Append(s + "\n");
            sr.Close();
            return sb;
        }

        public static void SetFileContent(string filePath, string content)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            StreamWriter sw = File.CreateText(filePath);
            sw.WriteLine(content);
            sw.Close();
        }

        public static void ClearMetaData()
        {
            string metadatapath = StorageFacade.MetaDataFolder;
            if (Directory.Exists(metadatapath))
            {
                string[] txt_files = Directory.GetFiles(metadatapath);
                for (int i = 0; i < txt_files.Length; i++)
                    File.Delete(txt_files[i]);
            }
        }
    }
}
