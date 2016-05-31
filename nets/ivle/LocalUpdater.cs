#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.IO;
using nets.dataclass;
using nets.utility;

#endregion

namespace nets.ivle
{
    /// <summary>
    ///   Update the local file system based on the output of
    ///   the UpdateDetector. This class has a combine 
    ///   functionality of 'Reconciler' and 'FileProcessor'.
    /// 
    ///   This class does not handle download files by itself but
    ///   use the method of the caller.
    /// 
    ///   The mode of synchronization is always one-way.
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class LocalUpdater
    {
        #region FIELD DECLARATION

        private readonly ExtendedWebClient DownloadManager;
        private readonly string LocalRootPath;
        public List<IvleWorkbinFile> NewFiles;
        public List<IvleWorkbinFolder> NewFolders;

        #endregion

        #region CONSTRUCTORS

        public LocalUpdater(ExtendedWebClient downloadManager, string localRootPath)
            :this()
        {
            DownloadManager = downloadManager;
            LocalRootPath = localRootPath;
        }

        private LocalUpdater()
        {
            NewFiles = new List<IvleWorkbinFile>();
            NewFolders = new List<IvleWorkbinFolder>();
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        ///   Synchronize with local.
        ///   @TODO: SUPPORT WRITE LOG.
        /// </summary>
        public void Synchronize()
        {
            if (NewFolders.Count == 0)
                Console.WriteLine("No new folder!");
            if (NewFiles.Count == 0)
            {
                Console.WriteLine("No new file!");
                return;
            }
            // Create new folders.
            UpdateFolderStructure();
            // Download new files.
            UpdateNewFiles();
        }

        /// <summary>
        /// Check if there is any job to do
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return (NewFiles.Count == 0) && (NewFolders.Count == 0);
        }

        /// <summary>
        ///   Retrieve the last upload date of a file.
        ///   Use to check for remote file updation.
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
        ///   Delete a file or folder and correspoding meta data if existing.
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                //To be changed accordingly
                //DeleteMetaData(filePath, FileType.File);
            }
            else if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
                //DeleteMetaData(filePath, FileType.Folder);
            }
        }

        /// <summary>
        ///   Copy a file to a destination
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destinationFolderPath"></param>
        public static void CopyFile(string filePath, string destinationFolderPath)
        {
            if (!Directory.Exists(destinationFolderPath))
                Console.WriteLine("Invalid input");

            CopyFileWithoutCheck(filePath, destinationFolderPath);
        }

        #endregion

        #region HELPERS

        /// <summary>
        ///   Update the local folder structure.
        ///   For the sake of simplicity, this version does not handle
        ///   deletion operations.
        ///   TODO: TAKE CARE OF THE CASE WHEN REMOTE FOLDER IS DELETED.
        /// </summary>
        private void UpdateFolderStructure()
        {
            foreach (IvleWorkbinFolder folder in NewFolders)
                Directory.CreateDirectory(LocalRootPath + folder.GetPath());
        }

        /// <summary>
        ///   Download new files from the remote server.
        /// </summary>
        private void UpdateNewFiles()
        {
            // TODO: CONVERT THIS TO ADD FILE TO DOWNLOAD QUEUE AND START MANAGE DOWNLOAD USING THREAD TO MAXIMIZE PARALLELISM.
            foreach (IvleWorkbinFile file in NewFiles)
            {
                Console.Write("Downloading file {0}...", /*_localRootPath +*/ file.GetPath());
                try
                {
                    DownloadManager.DownloadFile(IvleHandler.ConstructUrl(file),
                                                  LocalRootPath + file.Container.GetPath(),
                                                  file.FileName);
                    file.IsNew = false;

                    // write log code here
                    Console.WriteLine("Completed!");
                }
                catch (IOException)
                {
                    Console.WriteLine("IO Exception: Check your connection.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Unexpected error!");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destinationFolderPath"></param>
        private static void CopyFileWithoutCheck(string filePath, string destinationFolderPath)
        {
            string newFilePath = Path.Combine(destinationFolderPath, Path.GetFileName(filePath));

            if (File.Exists(filePath))
            {
                File.Copy(filePath, newFilePath, true);

                // TODO: REWRITE
                //string metaData = GetMetaData(filePath, FileType.File);

                //if (string.IsNullOrEmpty(metaData))
                //{
                //    //metaData = ConstructMetaData(filePath, FileType.File);
                //    //WriteToMetaData(filePath, metaData, FileType.File);
                //}

                //WriteToMetaData(desFilePath, metaData, FileType.File); 
            }
            else if (Directory.Exists(filePath))
            {
                if (Directory.Exists(newFilePath))
                    Directory.Delete(newFilePath, true);
                Directory.CreateDirectory(newFilePath);
                //string metaData = GetMetaData(filePath, FileType.File);

                //if (string.IsNullOrEmpty(metaData))
                //{
                //    metaData = ConstructMetaData(filePath, FileType.Folder);
                //    WriteToMetaData(filePath, metaData, FileType.File);                  
                //}

                //WriteToMetaData(desFilePath, metaData, FileType.Folder);
                foreach (string subFilePath in Directory.GetFileSystemEntries(filePath))
                    CopyFile(subFilePath, newFilePath);
            }
            else //invalid input
                Console.WriteLine("Invalid input");
        }

        #endregion
    }
}