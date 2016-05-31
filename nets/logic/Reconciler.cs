using System.Collections.Generic;
using nets.dataclass;
using nets.utility;
using System.IO;

namespace nets.logic
{
    /// <summary>
    /// Propagate changes to two folders being synchronized
    /// Author: Nguyen Thi Yen Duong + Hoang Nguyen Nhat Tao
    /// </summary>
    public class Reconciler
    {
        #region DELEGATE AND EVENT OBSERVER

        public delegate void UpdateProgressHandler(float percent);
        public delegate void UpdateActionHandler(string action);
        public delegate void UpdateButtonsHandler();
        public delegate void UpdateSyncJobListHandler(int rowIndex, string newStatus);
        public delegate void SetAbortButtonHandler(bool enabled);
        
        public static event UpdateProgressHandler UpdateProgressEvent;
        public static event UpdateActionHandler UpdateActionEvent;
        public static event UpdateButtonsHandler UpdateButtonsEvent;
        public static event UpdateSyncJobListHandler UpdateSyncJobListEvent;
        public static event SetAbortButtonHandler SetAbortButtonEvent; 
        
        #endregion

        #region PROPERTIES

        public int NumOfJobs { get; set; }
        public float StartPercent { get; set; }
        
        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Sync two folders with chosen Sync Mode and Filter Patterns
        /// </summary>
        /// <param name="listOfDifferences"></param>
        /// <param name="syncMode"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public void Synchronize(ref List<FilePair> listOfDifferences, SyncMode syncMode, ref Logger logger, ref bool errorOccur)
        {
            switch (syncMode)
            {
                case SyncMode.OneWay:
                    SyncOneWay(ref listOfDifferences, ref logger, ref errorOccur);
                    break;
                case SyncMode.TwoWay:
                    SyncTwoWay(ref listOfDifferences, ref logger, ref errorOccur);
                    break;
            }
        }

        #endregion

        #region PRIVATE HELPERS

        /// <summary>
        /// Handle two-way sync
        /// </summary>
        /// <param name="listOfDifferences"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private void SyncTwoWay(ref List<FilePair> listOfDifferences, ref Logger logger, ref bool errorOccur)
        {
            float percent = StartPercent;
            NotifyUpdateProgress(percent);

            if (listOfDifferences.Count == 0)
            {
                percent += ((float)0.97) / NumOfJobs;
                NotifyUpdateProgress(percent);
                return;
            }

            foreach (FilePair filePair in listOfDifferences)
            {
                // deleted - no change/updated
                if (filePair.FileStatusInSrc == FileStatus.Deleted)
                {
                    FileSystemOperator.DeleteMetaData(filePair.SrcFilePath, filePair.FilePairType);
                    SolveDeletedConflict(filePair.SrcFilePath, filePair.DesFilePath, filePair.FileStatusInDes, filePair.FilePairType, ref logger, ref errorOccur);
                    goto UpdateProgressBar;
                }

                // no change/updated - deleted
                if (filePair.FileStatusInDes == FileStatus.Deleted)
                {
                    FileSystemOperator.DeleteMetaData(filePair.DesFilePath, filePair.FilePairType);
                    SolveDeletedConflict(filePair.DesFilePath, filePair.SrcFilePath, filePair.FileStatusInSrc, filePair.FilePairType, ref logger, ref errorOccur);
                    goto UpdateProgressBar;
                }

                // updated - no change  /  new - not exist
                if ((filePair.FileStatusInSrc == FileStatus.Updated && filePair.FileStatusInDes == FileStatus.NoChange) ||
                    (filePair.FileStatusInSrc == FileStatus.New && filePair.FileStatusInDes == FileStatus.NotExist))
                {
                    CopyFile(filePair.SrcFilePath, filePair.DesFilePath, ref logger, ref errorOccur);
                    goto UpdateProgressBar;
                }

                // no change - updated  /  not exist - new
                if ((filePair.FileStatusInSrc == FileStatus.NoChange && filePair.FileStatusInDes == FileStatus.Updated) ||
                    (filePair.FileStatusInSrc == FileStatus.NotExist && filePair.FileStatusInDes == FileStatus.New))
                {
                    CopyFile(filePair.DesFilePath, filePair.SrcFilePath, ref logger, ref errorOccur);
                    goto UpdateProgressBar;
                }

                // new - new  /  updated - updated
                if (filePair.FileStatusInSrc == FileStatus.New || filePair.FileStatusInDes == FileStatus.Updated)
                {
                    int difference = FileSystemOperator.GetLastModifiedTime(filePair.SrcFilePath).CompareTo(FileSystemOperator.GetLastModifiedTime(filePair.DesFilePath));
                    if (difference >= 0)            //same last modified date --> copy from src to des
                        CopyFile(filePair.SrcFilePath, filePair.DesFilePath, ref logger, ref errorOccur);
                    else                            // destination newer
                        CopyFile(filePair.DesFilePath, filePair.SrcFilePath, ref logger, ref errorOccur);
                }

                UpdateProgressBar:
                {
                    percent += ((float)0.97) / (listOfDifferences.Count * NumOfJobs);
                    NotifyUpdateProgress(percent);
                }
            }
        }

        /// <summary>
        /// Solve the conflict: updated - deleted and vice versa
        /// </summary>
        /// <param name="deletedPath"></param>
        /// <param name="existedPath"></param>
        /// <param name="existPathStatus"></param>
        /// <param name="fileType"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private static void SolveDeletedConflict(string deletedPath, string existedPath, FileStatus existPathStatus, FileType fileType, ref Logger logger, ref bool errorOccur)
        {
            switch (fileType)
            {
                case FileType.File:
                    if (existPathStatus == FileStatus.NoChange)
                    {
                        FileSystemOperator.DeleteMetaData(deletedPath, fileType);
                        DeleteFile(existedPath, ref logger, ref errorOccur);
                    }
                    else
                        CopyFile(existedPath, deletedPath, ref logger, ref errorOccur);
                    break;
                case FileType.Folder:
                    List<string> newAndUpdatedFiles = new List<string>();
                    FileSystemOperator.RecordNewAndUpdatedFiles(existedPath, ref newAndUpdatedFiles, ref logger, ref errorOccur);
                    foreach (string newAndUpdatedFile in newAndUpdatedFiles)
                    {
                        string relativePath = PathOperator.GetRelativePath(newAndUpdatedFile, existedPath);
                        string correspondingPath = PathOperator.GetAbsolutePath(deletedPath, relativePath);
                        try
                        {
                            FileSystemOperator.CheckAndCreateFolder(Path.GetDirectoryName(correspondingPath));
                            CopyFile(newAndUpdatedFile, correspondingPath, ref logger, ref errorOccur);
                        }
                        catch (PathTooLongException) { ExceptionHandler.HandleException(correspondingPath + " is too long! Error occured while manipulating it!", ref logger, ref errorOccur); }
                    }
                    FileSystemOperator.DeleteEmptyFolders(existedPath, ref newAndUpdatedFiles, ref logger, ref errorOccur);
                    break;
            }
        }

        /// <summary>
        /// Handle one-way sync
        /// </summary>
        /// <param name="listOfDifferences"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private void SyncOneWay(ref List<FilePair> listOfDifferences, ref Logger logger, ref bool errorOccur)
        {
            float percent = StartPercent;
            NotifyUpdateProgress(percent);

            if (listOfDifferences.Count == 0)
            {
                percent += ((float)0.97) / NumOfJobs;
                NotifyUpdateProgress(percent);
                return;
            }

            foreach (FilePair filePair in listOfDifferences)
            {
                switch (filePair.FileStatusInSrc)
                {
                    case FileStatus.New:
                    case FileStatus.Updated:
                    case FileStatus.NoChange:
                        CopyFile(filePair.SrcFilePath, filePair.DesFilePath, ref logger, ref errorOccur);
                        break;
                    case FileStatus.NotExist:
                        DeleteFile(filePair.DesFilePath, ref logger, ref errorOccur);
                        break;
                    case FileStatus.Deleted:
                        FileSystemOperator.DeleteMetaData(filePair.SrcFilePath, filePair.FilePairType);
                        DeleteFile(filePair.DesFilePath, ref logger, ref errorOccur);
                        break;
                }

                percent += ((float)0.97) / (listOfDifferences.Count * NumOfJobs);
                NotifyUpdateProgress(percent);
            }
        }
        
        /// <summary>
        /// Copy file, log the action, update metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="desFilePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private static void CopyFile(string filePath, string desFilePath, ref Logger logger, ref bool errorOccur)
        {
            NotifyUpdateAction("COPY\n" + PathOperator.TrimPath(filePath, 57) + "\nTO\n" + PathOperator.TrimPath(desFilePath, 57));
            if (logger != null) logger.Log(LogType.INFO, "COPY\n" + filePath + "\nTO\n" + desFilePath);
            FileSystemOperator.CopyFile(filePath, desFilePath, ref logger, ref errorOccur);
        }

        /// <summary>
        /// Delete file, log action and delete metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private static void DeleteFile(string filePath, ref Logger logger, ref bool errorOccur)
        {
            NotifyUpdateAction("DELETE\n" + PathOperator.TrimPath(filePath, 57));
            if (logger != null) logger.Log(LogType.INFO, "DELETE\n" + filePath);
            FileSystemOperator.DeleteFile(filePath, ref logger, ref errorOccur);
        }

        #endregion

        #region GUI EVENT NOTIFIERS

        /// <summary>
        /// Notify GUI to update progress bar
        /// </summary>
        /// <param name="percent"></param>
        private static void NotifyUpdateProgress(float percent)
        {
            if (UpdateProgressEvent != null)
                UpdateProgressEvent(percent);
        }

        /// <summary>
        /// Notify GUI to update action
        /// </summary>
        /// <param name="action"></param>
        public static void NotifyUpdateAction(string action)
        {
            if (UpdateActionEvent != null)
                UpdateActionEvent(action);
        }

        /// <summary>
        /// Notify GUI to update buttons
        /// </summary>
        public static void NotifyUpdateButtons()
        {
            if (UpdateButtonsEvent != null)
                UpdateButtonsEvent();
        }

        /// <summary>
        /// Notify GUI to update sync job list
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="newStatus"></param>
        public static void NotifyUpdateSyncJobList(int rowIndex, string newStatus)
        {
            if (UpdateSyncJobListEvent != null)
                UpdateSyncJobListEvent(rowIndex, newStatus);
        }

        /// <summary>
        /// Notify GUI to update Abort button
        /// </summary>
        /// <param name="enabled"></param>
        public static void NotifySetAbortButton(bool enabled)
        {
            if (SetAbortButtonEvent != null)
                SetAbortButtonEvent(enabled);
        }

        #endregion
    }
}
