#region USING DIRECTIVES

using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using nets.dataclass;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Perform operations and checking on files.
    /// Author: Dang Thu Giang + Nguyen Thi Yen Duong + Hoang Nguyen Nhat Tao + Tran Binh Nguyen
    /// </summary>
    public static class FileSystemOperator
    {
        #region PROPERTIES

        public static FilterPattern Filter { private get; set; }
        public static bool SafeDelete { get; set; }
        
        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Retrieve the universal last-modified time of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DateTime GetLastModifiedTime(string filePath)
        {
            return File.GetLastWriteTimeUtc(filePath);
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
            if (fileType == FileType.File && Filter.IsExcluded(filePath))
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
        /// Record new and updated files in a folder
        /// </summary>
        /// <param name="formattedFolderPath"></param>
        /// <param name="fileList"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void RecordNewAndUpdatedFiles(string folderPath, ref List<string> fileList, ref Logger logger, ref bool errorOccur)
        {
            string[] subFiles = null;

            try
            {
                subFiles = Directory.GetFileSystemEntries(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied for " + folderPath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound: " + folderPath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while accessing " + folderPath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while accessing " + folderPath, ref logger, ref errorOccur);
            }

            if (subFiles == null)
                return;

            foreach (string subFile in subFiles)
            {
                FileStatus fileStatus;
                if (Directory.Exists(subFile))
                {
                    fileStatus = GetFileStatus(subFile, FileType.Folder);
                    if (fileStatus == FileStatus.New)
                        fileList.Add(subFile);
                    else
                        RecordNewAndUpdatedFiles(subFile, ref fileList, ref logger, ref errorOccur);
                }
                else
                {
                    fileStatus = GetFileStatus(subFile, FileType.File);
                    if (fileStatus == FileStatus.New || fileStatus == FileStatus.Updated)
                        fileList.Add(subFile);
                    else
                        DeleteFile(subFile, ref logger, ref errorOccur);
                }
            }
        }

        /// <summary>
        /// Delete any empty folders that are not newly created in a folder
        /// </summary>
        /// <param name="formattedFolderPath"></param>
        /// <param name="excludeFolderPaths"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void DeleteEmptyFolders(string folderPath, ref List<string> excludeFolderPaths, ref Logger logger, ref bool errorOccur)
        {
            try
            {
                string[] subDirs = Directory.GetDirectories(folderPath);
                foreach (string subDir in subDirs)
                {
                    if (!excludeFolderPaths.Contains(subDir))
                        DeleteEmptyFolders(subDir, ref excludeFolderPaths, ref logger, ref errorOccur);
                }
                if (Directory.GetFileSystemEntries(folderPath).Length == 0 && !excludeFolderPaths.Contains(folderPath))
                    DeleteFile(folderPath, ref logger, ref errorOccur);
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied for " + folderPath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound: " + folderPath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while accessing " + folderPath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while accessing " + folderPath, ref logger, ref errorOccur);
            }
        }

        /// <summary>
        /// Delete meta data of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        public static void DeleteMetaData(string filePath, FileType fileType)
        {
            if (fileType == FileType.File && Filter.IsExcluded(filePath))
                return;
            MetaDataManager.DeleteMetaData(filePath, fileType);
        }

        /// <summary>
        /// Delete a file or folder and correspoding metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void DeleteFile(string filePath, ref Logger logger, ref bool errorOccur)
        {
            try
            {
                if (SafeDelete)
                    DeleteFileToRecycleBin(filePath);
                else
                    DeleteFilePermanent(filePath);
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
        /// <param name="desFilePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void CopyFile(string filePath, string desFilePath, ref Logger logger, ref bool errorOccur)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    if (Filter.IsExcluded(filePath) || Filter.IsExcluded(desFilePath))
                        return;

                    File.Copy(filePath, desFilePath, true);

                    string metaData = MetaDataManager.GetMetaData(filePath, FileType.File);

                    if (string.IsNullOrEmpty(metaData))
                    {
                        metaData = MetaDataManager.ConstructMetaData(filePath);
                        MetaDataManager.WriteToMetaData(filePath, metaData, FileType.File);
                    }

                    MetaDataManager.WriteToMetaData(desFilePath, metaData, FileType.File);
                }
                else if (Directory.Exists(filePath))
                {
                    if (Directory.Exists(desFilePath))
                        Directory.Delete(desFilePath, true);
                    Directory.CreateDirectory(desFilePath);
                    
                    string metaData = MetaDataManager.GetMetaData(filePath, FileType.Folder);

                    if (string.IsNullOrEmpty(metaData))
                    {
                        metaData = MetaDataManager.ConstructMetaData(filePath);
                        MetaDataManager.WriteToMetaData(filePath, metaData, FileType.Folder);
                    }

                    MetaDataManager.WriteToMetaData(desFilePath, metaData, FileType.Folder);
                    foreach (string subFilePath in Directory.GetFileSystemEntries(filePath))
                    {
                        string subDesFilePath = Path.Combine(desFilePath, Path.GetFileName(subFilePath));
                        CopyFile(subFilePath, subDesFilePath, ref logger, ref errorOccur);
                    }

                    Directory.SetLastWriteTimeUtc(desFilePath, Directory.GetLastWriteTimeUtc(filePath));
                }
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied while coping " + filePath + " to " + desFilePath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Directory unfound while copying " + filePath + " to " + desFilePath, ref logger, ref errorOccur);
            }
            catch (FileNotFoundException)
            {
                ExceptionHandler.HandleException("File unfound while copying " + filePath + " to " + desFilePath, ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error occurred while copying " + filePath + " to " + desFilePath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknown error occured while copying " + filePath + " to " + desFilePath, ref logger, ref errorOccur);
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
            if (fileType == FileType.File && Filter.IsExcluded(filePath))
                return false;

            string metaData = MetaDataManager.GetMetaData(filePath, fileType);
            string newMetaData = MetaDataManager.ConstructMetaData(filePath);
            bool returnValue = (metaData != newMetaData);

            if (returnValue)
                MetaDataManager.WriteToMetaData(filePath, newMetaData, fileType);

            return returnValue;
        }

        /// <summary>
        /// Delete a file or folder and correspoding metadata
        /// </summary>
        /// <param name="filePath"></param>
        private static void DeleteFilePermanent(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (Filter.IsExcluded(filePath))
                    return;
                FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently);
                MetaDataManager.DeleteMetaData(filePath, FileType.File);
            }
            else if (Directory.Exists(filePath))
            {
                FileSystem.DeleteDirectory(filePath, UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently);
                MetaDataManager.DeleteMetaData(filePath, FileType.Folder);
            }
        }

        /// <summary>
        /// Delete a file but keep it in Recycle Bin
        /// </summary>
        /// <param name="filePath"></param>
        private static void DeleteFileToRecycleBin(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (Filter.IsExcluded(filePath))
                    return;
                FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                MetaDataManager.DeleteMetaData(filePath, FileType.File);
            }
            else if (Directory.Exists(filePath))
            {
                FileSystem.DeleteDirectory(filePath, UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                MetaDataManager.DeleteMetaData(filePath, FileType.Folder);
            }
        }

        #endregion
    }
}