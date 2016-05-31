using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using nets_wpf.DataStructures;

namespace nets_wpf.Utility
{
    /// <summary>
    /// Perform operations and checking on files.
    /// </summary>
    public static class FileSystemOperator
    {
        #region PROPERTIES

        public static FilterPattern Filter { private get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Retrieve the universal last-modified time of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DateTime GetLastModifiedTime(string filePath)
        {
            DateTime now = DateTime.Now;
            TimeSpan localOffset = now - now.ToUniversalTime();
            return File.GetLastWriteTimeUtc(filePath) + localOffset;
        }

        /// <summary>
        /// Check if two files have the same content
        /// </summary>
        /// <param name="firstFilePath"></param>
        /// <param name="secondFilePath"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool HaveSameContent(string firstFilePath, string secondFilePath, FileType fileType)
        {
            if (Filter.IsExcluded(firstFilePath) || Filter.IsExcluded(secondFilePath))
                return true;

            string firstMeta = MetaDataManager.GetMetaData(firstFilePath, fileType);
            string secondMeta = MetaDataManager.GetMetaData(secondFilePath, fileType);
            return firstMeta == secondMeta;
        }

        /// <summary>
        /// Get the status of the file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static FileStatus GetFileStatus(string filePath, FileType fileType)
        {
            if (Filter.IsExcluded(filePath))
                return FileStatus.NoChange;

            FileStatus fileStatus;

            if ((fileType == FileType.File && File.Exists(filePath)) || (fileType == FileType.Folder && Directory.Exists(filePath)))
            {
                if (MetaDataManager.HaveMetaData(filePath, fileType))
                    fileStatus = IsUpdated(filePath, fileType) ? FileStatus.Updated : FileStatus.NoChange;
                else
                    fileStatus = FileStatus.New;
            }
            else
                fileStatus = MetaDataManager.HaveMetaData(filePath, fileType) ? FileStatus.Deleted : FileStatus.NotExist;

            return fileStatus;
        }

        

        /// <summary>
        /// Delete meta data of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        public static void DeleteMetaData(string filePath, FileType fileType)
        {
            if (Filter.IsExcluded(filePath))
                return;
            MetaDataManager.DeleteMetaData(filePath, fileType);
        }

        /// <summary>
        /// Delete a file or folder and correspoding metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void DeleteFilePermanent(string filePath, ref Logger logger, ref bool errorOccur)
        {
            if (Filter.IsExcluded(filePath))
                return;

            try
            {
                if (File.Exists(filePath))
                {
                    if (Filter.IsExcluded(filePath))
                        return;
                    File.Delete(filePath);
                    MetaDataManager.DeleteMetaData(filePath, FileType.File);
                }
                else if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, true);
                    MetaDataManager.DeleteMetaData(filePath, FileType.Folder);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (FileNotFoundException)
            {
                ExceptionHandler.HandleException("File unfound while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while deleting " + filePath, ref logger, ref errorOccur);
            }
        }

        /// <summary>
        /// Delete a file but keep it in Recycle Bin
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void DeleteFileToRecycleBin(string filePath, ref Logger logger, ref bool errorOccur)
        {
            if (Filter.IsExcluded(filePath))
                return;

            try
            {
                if (File.Exists(filePath))
                {
                    if (Filter.IsExcluded(filePath))
                        return;
                    FileSystem.DeleteFile(filePath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    MetaDataManager.DeleteMetaData(filePath, FileType.File);
                }
                else if (Directory.Exists(filePath))
                {
                    FileSystem.DeleteDirectory(filePath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    MetaDataManager.DeleteMetaData(filePath, FileType.Folder);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (FileNotFoundException)
            {
                ExceptionHandler.HandleException("File unfound while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while deleting " + filePath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while deleting " + filePath, ref logger, ref errorOccur);
            }
        }

        /// <summary>
        /// Copy a file to a destination folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="desFolderPath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void CopyFile(string filePath, string desFolderPath, ref Logger logger, ref bool errorOccur)
        {
            if (Filter.IsExcluded(filePath))
                return;

            string newFilePath = Path.Combine(desFolderPath, Path.GetFileName(filePath));
            try
            {
                if (File.Exists(filePath))
                {
                    if (Filter.IsExcluded(filePath))
                        return;

                    File.Copy(filePath, newFilePath, true);

                    string metaData = MetaDataManager.GetMetaData(filePath, FileType.File);

                    if (string.IsNullOrEmpty(metaData))
                    {
                        metaData = MetaDataManager.ConstructMetaData(filePath);
                        MetaDataManager.WriteToMetaData(filePath, metaData, FileType.File);
                    }

                    MetaDataManager.WriteToMetaData(newFilePath, metaData, FileType.File);
                }
                else if (Directory.Exists(filePath))
                {
                    if (Directory.Exists(newFilePath))
                        Directory.Delete(newFilePath, true);
                    Directory.CreateDirectory(newFilePath);
                    string metaData = MetaDataManager.GetMetaData(filePath, FileType.File);

                    if (string.IsNullOrEmpty(metaData))
                    {
                        metaData = MetaDataManager.ConstructMetaData(filePath);
                        MetaDataManager.WriteToMetaData(filePath, metaData, FileType.File);
                    }

                    MetaDataManager.WriteToMetaData(newFilePath, metaData, FileType.Folder);
                    foreach (string subFilePath in Directory.GetFileSystemEntries(filePath))
                        CopyFile(subFilePath, newFilePath, ref logger, ref errorOccur);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied while coping " + filePath + " to " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound while copying " + filePath + " to " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (FileNotFoundException)
            {
                ExceptionHandler.HandleException("File unfound while copying " + filePath + " to " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while copying " + filePath + " to " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while copying " + filePath + " to " + desFolderPath, ref logger, ref errorOccur);
            }
        }

        /// <summary>
        ///   Check if a file exists. If not create a new file
        /// </summary>
        /// <param name = "fileLocation"></param>
        public static void CheckAndCreateFile(string fileLocation)
        {
            if (File.Exists(fileLocation))
                return;

            CheckAndCreateFolder(Directory.GetParent(fileLocation).ToString());

            try
            {
                FileStream fs = File.Create(fileLocation);
                fs.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        ///   Check and create directory structure
        /// </summary>
        /// <param name = "folderLocation"></param>
        public static void CheckAndCreateFolder(string folderLocation)
        {
            string[] s = folderLocation.Split('\\');
            string cur = "";
            for (int i = 0; i < s.Length; i++)
            {
                string temp = cur + s[i] + @"\";
                cur = temp;
                if (!Directory.Exists(cur))
                    Directory.CreateDirectory(cur);
            }
        }

        #endregion

        #region PRIVATE HELPERS 

        /// <summary>
        /// Check if a file is updated from the last-time synchronization, assuming that the file has metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static bool IsUpdated(string filePath, FileType fileType)
        {
            if (Filter.IsExcluded(filePath))
                return false;

            string metaData = MetaDataManager.GetMetaData(filePath, fileType);
            string newMetaData = MetaDataManager.ConstructMetaData(filePath);
            bool returnValue = (metaData != newMetaData);

            if (returnValue)
                MetaDataManager.WriteToMetaData(filePath, newMetaData, fileType);

            return returnValue;
        }

        #endregion
    }
}