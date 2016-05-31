#region USING DIRECTIVES

using System.IO;
using nets.dataclass;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Provides operations on filesystem paths
    /// Author: Hoang Nguyen Nhat Tao + Tran Binh Nguyen
    /// </summary>
    static class PathOperator
    {
        #region MAIN METHODS

        /// <summary>
        /// Get filesystem type (file/folder)
        /// </summary>
        /// <param name="filesystemPath"></param>
        /// <returns></returns>
        public static FileType GetFileSystemType(string filesystemPath)
        {
            return File.Exists(filesystemPath) ? FileType.File : FileType.Folder;
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
                path = path.Replace(path.Substring(length/2 - 1, path.Length - length + 3), "...");
            return path;
        }

        /// <summary>
        /// Get the source sub path
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static string GetSrcSubPath(string srcFolder, string desFolder, string subPath)
        {
            string normalizedSrcFolder = NormalizeFolderPath(srcFolder);
            string normalizedDesFolder = NormalizeFolderPath(desFolder);
            string normalizedSubPath = NormalizeFolderPath(subPath);
            
            return
                (normalizedSubPath.ToLower().StartsWith(normalizedSrcFolder.ToLower()))
                ? normalizedSubPath
                : normalizedSubPath.Replace(normalizedDesFolder, normalizedSrcFolder);
        }

        /// <summary>
        /// Get the destination sub path
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static string GetDesSubPath(string srcFolder, string desFolder, string subPath)
        {
            string normalizedSrcFolder = NormalizeFolderPath(srcFolder);
            string normalizedDesFolder = NormalizeFolderPath(desFolder);
            string normalizedSubPath = NormalizeFolderPath(subPath);
            
            return
                (normalizedSubPath.ToLower().StartsWith(normalizedDesFolder.ToLower()))
                ? normalizedSubPath
                : normalizedSubPath.Replace(normalizedSrcFolder, normalizedDesFolder);
        }

        /// <summary>
        /// Compare filesystem paths
        /// </summary>
        /// <param name="filesystemPath1"></param>
        /// <param name="filesystemPath2"></param>
        /// <returns></returns>
        public static bool ComparePath(string filesystemPath1, string filesystemPath2)
        {
            string normalizedPath1 = NormalizeFolderPath(filesystemPath1);
            string normalizedPath2 = NormalizeFolderPath(filesystemPath2);
            return normalizedPath1.ToLower() == normalizedPath2.ToLower();
        }

        /// <summary>
        /// Normalize folder path by using standard path seperator and removing extra spaces
        /// </summary>
        /// <param name="formattedFolderPath"></param>
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

        /// <summary>
        /// Get name of file/folder from full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        #endregion
    }
}
