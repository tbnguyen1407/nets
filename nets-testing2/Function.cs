using System;
using System.IO;
using nets.dataclass;
using nets.logic;

namespace DetectorTest_SingleFile
{
    internal class Function
    {
        private static string tempFolder =
            @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\Temp";

        private static string path1 =
            @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\Same\";

        private static string path2 =
            @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\One-file folder\";

        private static string path3 =
            @"D:\Course Modules\Year 3\Semester 2\CS3215 SE Project\NETS offline\Test cases\Diff\";

        private static string StorageRoot = tempFolder;
        private static string MetaDataPath = StorageRoot + "\\nets_metadata.dat";
        private static string ProfilePath = StorageRoot + "\\nets_profiles.dat";
        private static string LogPath = StorageRoot + "\\nets_log.dat";

        /* ----------------------------------------------------
             * Method: initSame(folder1, folder2, rootpath)
             * Initialize two test folders
             * - Back up in temporary folder
             * - Make two folder equal
             * ----------------------------------------------------
             */

        public static void initSame(string folder1, string folder2, int rootpath)
        {
            string path;
            deleteFolderContent(tempFolder);

            if (rootpath == 1)
                path = path1;
            else
                path = path2;

            string dir1 = Path.Combine(path, folder1);
            string dir2 = Path.Combine(path, folder2);
            if (!Directory.Exists(dir2))
                Directory.CreateDirectory(dir2);
            else
                deleteFolderContent(dir2);

            string dest1 = Path.Combine(tempFolder, folder1);

            // Backup folder
            copyFolder(dir1, dest1);

            // Mirror from folder1 to folder2
            LogicFacade.Sync(dir1, dir2, SyncMode.Mirror, null);
        }

        /* ----------------------------------------------------
            * Method: init(folder1, folder2, folder3) //special init
            * Initialize two test folders
            * - Back up in temporary folder
            * - Make two folder equal
            * ----------------------------------------------------
            */

        public static void initAdvance(string folder1, string folder2)
        {
            initSame(folder1, folder2, 2);
            string folder3 = "Test3";

            string dir1 = Path.Combine(path2, folder1);
            string dir2 = Path.Combine(path2, folder2);
            string dir3 = Path.Combine(path2, folder3);
            if (Directory.Exists(dir3))
                deleteFolderContent(dir3);
            else
                Directory.CreateDirectory(dir3);

            copyFolder(dir1, dir3);

            // Edit file text in dir3
            string filePath = Path.Combine(dir3, "a.txt");
            updateFile(filePath);

            // Mirror folder3 -> folder2
            LogicFacade.Sync(dir1, dir2, SyncMode.Mirror, null);

            // Delete folder3
            deleteFolderContent(dir3);
            Directory.Delete(dir3);
        }

        /* ----------------------------------------------------
             * Method: init(folder1, folder2)
             * Initialize two test folders
             * - Back up in temporary folder
             * - Make two folder equal
             * ----------------------------------------------------
             */

        public static void init(string folder1, string folder2)
        {
            deleteFolderContent(tempFolder);

            string dir1 = Path.Combine(path3, folder1);
            string dir2 = Path.Combine(path3, folder2);

            string dest1 = Path.Combine(tempFolder, folder1);
            string dest2 = Path.Combine(tempFolder, folder2);

            // Backup folder
            copyFolder(dir1, dest1);
            copyFolder(dir2, dest2);
        }

        /* ----------------------------------------------------
             * Method: finalize(folder1, folder2, rootpath)
             * - Restore 2 folders to its original state
             * - Delete temporary folders, profiles, log files, metadata file
             * ----------------------------------------------------
             */

        public static void finalize(string folder1, string folder2)
        {
            string dir1 = Path.Combine(path3, folder1);
            string dir2 = Path.Combine(path3, folder2);

            string dest1 = Path.Combine(tempFolder, folder1);
            string dest2 = Path.Combine(tempFolder, folder2);

            // Delete folder content
            deleteFolderContent(dir1);
            deleteFolderContent(dir2);

            // Restore backup
            copyFolder(dest1, dir1);
            copyFolder(dest2, dir2);

            // Delete temporary folder
            deleteFolderContent(tempFolder);
        }

        /* ----------------------------------------------------
             * Method: finalize(folder1, folder2, rootpath)
             * - Restore 2 folders to its original state
             * - Delete temporary folders, profiles, log files, metadata file
             * ----------------------------------------------------
             */

        public static void finalize(string folder1, string folder2, int rootpath)
        {
            string path;
            if (rootpath == 1)
                path = path1;
            else
                path = path2;

            string dir1 = Path.Combine(path, folder1);
            string dir2 = Path.Combine(path, folder2);

            string dest1 = Path.Combine(tempFolder, folder1);
            string dest2 = Path.Combine(tempFolder, folder2);

            // Delete folder
            deleteFolderContent(dir1);
            deleteFolderContent(dir2);
            Directory.Delete(dir2);

            // Restore backup
            copyFolder(dest1, dir1);

            // Delete temporary folder
            deleteFolderContent(tempFolder);
        }


        /* -------------------------------------
             * Method: copyFolder(source, destination)
             * Copy all content in source folder to destination folder
             * -------------------------------------
             */

        public static void copyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Copy all files
            string[] files = Directory.GetFiles(sourceFolder);
            string fileName, destFile;
            foreach (string file in files)
            {
                fileName = Path.GetFileName(file);
                destFile = Path.Combine(destFolder, fileName);
                File.Copy(file, destFile);
            }

            // Copy all sub-folders
            String[] folders = Directory.GetDirectories(sourceFolder);
            string folName, destFol;
            foreach (string folder in folders)
            {
                folName = Path.GetFileName(folder);
                destFol = Path.Combine(destFolder, folName);
                copyFolder(folder, destFol);
            }
        }

        /* -------------------------------------
             * Method: deleteFolderContent(folder)
             * Format the chosen folder, usually the temporary folder
             * -------------------------------------
             */

        public static void deleteFolderContent(string folder2Del)
        {
            // Delete all files
            string[] files = Directory.GetFiles(folder2Del);

            foreach (string file in files)
            {
                File.Delete(file);
            }

            // Delete all sub-folders
            String[] folders = Directory.GetDirectories(folder2Del);
            foreach (string folder in folders)
            {
                deleteFolderContent(folder);
                // Delete the empty folder
                Directory.Delete(folder);
            }
        }


        public static void updateFile(string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath);
            StreamWriter sw = File.AppendText(filePath);
            for (int i = 0; i < 500; i++)
                sw.Write("1234567890\n");
            sw.Close();
        }
    }
}
