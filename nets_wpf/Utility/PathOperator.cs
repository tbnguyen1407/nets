using System.IO;
using nets_wpf.DataStructures;

namespace nets_wpf.Utility
{
    static class PathOperator
    {
        public static FileType GetFileSystemType(string filesystemPath)
        {
            if (File.Exists(filesystemPath))
                return FileType.File;
            else //if (Directory.Exists(filesystemPath))
                return FileType.Folder;
            //return null;
        }

        /// <summary>
        /// Trim a filesystem path to a specific length with "..." in the middle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TrimPath(string path, int length)
        {
            if (path.Length > length)
                path = path.Replace(path.Substring(length / 2 - 1, path.Length - length + 3), "...");
            return path;
        }

        public static bool ComparePath(string filesystemPath1, string filesystemPath2)
        {
            string normalizedPath1 = NormalizeFolderPath(filesystemPath1);
            string normalizedPath2 = NormalizeFolderPath(filesystemPath2);
            return normalizedPath1.ToLower() == normalizedPath2.ToLower();
        }

        /// <summary>
        /// Normalize folder path by using standard path seperator and removing extra spaces
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string NormalizeFolderPath(string folderPath)
        {
            folderPath = folderPath.Trim();
            if (folderPath == string.Empty)
                return folderPath;

            while (folderPath.Contains(@"\\"))
                folderPath = folderPath.Replace(@"\\", @"\");

            while (folderPath.Contains(@" \"))
                folderPath = folderPath.Replace(@" \", @"\");

            while (folderPath.Contains(@"\ "))
                folderPath = folderPath.Replace(@"\ ", @"\");

            if (folderPath.Substring(folderPath.Length - 1) != @"\")
                folderPath += @"\";

            return folderPath;
        }

        /// <summary>
        /// Find the relative path of a child file/folder w.r.t. a parent folder
        /// </summary>
        /// <param name="childPath"></param>
        /// <param name="parentPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string childPath, string parentPath)
        {
            return childPath.Replace(parentPath, "");
        }

        /// <summary>
        /// Create a path from an absolute path and a relative path
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="relativeFilePath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string rootPath, string relativeFilePath)
        {
            return rootPath + relativeFilePath;
        }

        /// <summary>
        /// Get the corresponding subpath
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="desPath"></param>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static string GetCorrespondingPath(string srcPath, string desPath, string subPath)
        {
            string parentPath;
            string nonParentPath;

            if (subPath.Contains(srcPath))
            {
                parentPath = srcPath;
                nonParentPath = desPath;
            }
            else
            {
                parentPath = desPath;
                nonParentPath = srcPath;
            }

            return subPath.Replace(parentPath, nonParentPath);
        }

        public static string GetNameFromPath(string path)
        {
            string name = "";
            if (path == null || path.Trim() == "")
                return "";

            while (path.EndsWith(@"\"))
                path = path.Substring(0, path.Length - 1);

            try
            {
                string folderName = Path.GetFileName(path);
                if (folderName == null || folderName.Trim() == "")
                {
                    string root = Path.GetPathRoot(path);
                    if (root != null && root.Trim() != "")
                        name = root.Substring(0, 1);
                }
                else name = folderName;
            }
            catch
            {
            }
            return name;
        }   
    }
}
