using System;
using System.IO;
using System.Security.Cryptography;
using nets_wpf.DataStructures;
using nets_wpf.Storage;

namespace nets_wpf.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class MetaDataManager
    {
        private static readonly string[] excludedExtensions =
        {
            ".exe", ".iso",
            ".mp3", ".wav", ".wma",
            ".jpg", ".bmp", ".png", ".tiff",
            ".mp4", ".avi", ".flv", ".mpg", ".wmv"
        };

        /// <summary>
        /// Construct meta data of a file/folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ConstructMetaData(string filePath)
        {
            return FileSystemOperator.GetLastModifiedTime(filePath).ToString();
        }

        /// <summary>
        /// Check if the meta data of a file exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool HaveMetaData(string filePath, FileType fileType)
        {
            string metaData = GetMetaData(filePath, fileType);

            bool returnValue = (metaData != null);

            if ((!returnValue) && ((fileType == FileType.File && File.Exists(filePath)) || (fileType == FileType.Folder && Directory.Exists(filePath))))
                WriteToMetaData(filePath, MetaDataManager.ConstructMetaData(filePath), fileType);

            return returnValue;
        }

        /// <summary>
        /// Add or update metadata of file with a new metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="metaData"></param>
        /// <param name="fileType"></param>
        public static void WriteToMetaData(string filePath, string metaData, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.File:
                    StorageFacade.SaveMetaData(filePath, metaData);
                    break;
                case FileType.Folder:
                    StorageFacade.SaveMetaData(filePath + @"\", metaData);
                    break;
            }
        }

        /// <summary>
        /// Delete metadata of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        public static void DeleteMetaData(string filePath, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.File:
                    StorageFacade.DeleteMetaData(filePath);
                    break;
                case FileType.Folder:
                    StorageFacade.DeleteMetaData(filePath + @"\");
                    break;
            }
        }

        /// <summary>
        /// Retrieve metadata of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string GetMetaData(string filePath, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.File:
                    return StorageFacade.LoadMetaData(filePath);
                case FileType.Folder:
                    return StorageFacade.LoadMetaData(filePath + @"\");
            }

            return null;
        }

        /// <summary>
        /// Compute checksum of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string ComputeCheckSum(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    SHA1Managed sha = new SHA1Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// Check if we should calculate checksum for a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool CheckSumIsUsed(string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            if (file.Length > 50000)
                return false;

            foreach (string extension in excludedExtensions)
                if (file.Extension == extension)
                    return false;

            return true;
        }
    }
}
