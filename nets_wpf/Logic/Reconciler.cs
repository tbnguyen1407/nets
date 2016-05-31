using System.Collections.Generic;
using System.IO;
using nets_wpf.DataStructures;
using nets_wpf.Utility;

namespace nets_wpf.Logic
{
    /// <summary>
    /// Propagate changes to two folders being synchronized
    /// </summary>
    public class Reconciler
    {
        #region PRIVATE FILED DECLARATION
        
        private List<FilePair> listOfConflicts = new List<FilePair>();
        
        #endregion

        #region DELEGATE AND EVENT OBSERVER

        public delegate void UpdateProgressHandler(float percent);
        public delegate void UpdateActionHandler(string action);
        public delegate void CheckSyncPausedHandler();
        public delegate void UpdateButtonsHandler();
        public delegate void UpdateSyncJobListHandler(int rowIndex, string newStatus);
        public delegate void CloseSyncProgressHandler();

        public static event UpdateProgressHandler UpdateProgressEvent;
        public static event UpdateActionHandler UpdateActionEvent;
        public static event CheckSyncPausedHandler CheckSyncPausedEvent;
        public static event UpdateButtonsHandler UpdateButtonsEvent;
        public static event UpdateSyncJobListHandler UpdateSyncJobListEvent;
        public static event CloseSyncProgressHandler CloseSyncProgressEvent;

        #endregion

        #region PROPERTIES

        public int NumOfJobs { get; set; }
        public float StartPercent { get; set; }
        public int JobNumber { get; set; }
        public bool Aborted = false;
        
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
            listOfConflicts = new List<FilePair>();

            NotifyUpdateSyncJobList(JobNumber, "Syncing");

            switch (syncMode)
            {
                case SyncMode.Mirror:
                    SyncOneWay(ref listOfDifferences, ref logger, ref errorOccur);
                    break;
                case SyncMode.Equalize:
                    SyncTwoWay(ref listOfDifferences, ref logger, ref errorOccur);
                    break;
            }

            NotifyUpdateSyncJobList(JobNumber, "Synced");
        }

        #endregion

        #region PRIVATE HELPERS

        private void SyncTwoWay(ref List<FilePair> listOfDifferences, ref Logger logger, ref bool errorOccur)
        {
            float percent = StartPercent;
            NotifyUpdateProgress(percent);

            if (listOfDifferences.Count == 0)
            {
                NotifyCheckSyncPaused();
                //if (Aborted) return;

                percent += ((float)0.97) / NumOfJobs;
                NotifyUpdateProgress(percent);
                return;
            }

            foreach (FilePair filePair in listOfDifferences)
            {
                NotifyCheckSyncPaused();
                //if (Aborted) return;

                //Conflicts: choose the newest version
                if ((filePair.FileStatusInSrc == FileStatus.New || filePair.FileStatusInSrc == FileStatus.Updated || filePair.FileStatusInSrc == FileStatus.Deleted) &&
                    (filePair.FileStatusInDes == FileStatus.New || filePair.FileStatusInDes == FileStatus.Updated || filePair.FileStatusInDes == FileStatus.Deleted))
                {
                    listOfConflicts.Add(filePair);
                    SolveConflict(filePair.SrcFilePath, filePair.FileStatusInSrc, filePair.DesFilePath, filePair.FileStatusInDes, ref logger, ref errorOccur);
                    goto UpdateProgressBar;
                }

                //file change in source
                switch (filePair.FileStatusInSrc)
                {
                    case FileStatus.Updated:
                    case FileStatus.New:
                        CopyFile(filePair.SrcFilePath, Directory.GetParent(filePair.DesFilePath).FullName, ref logger, ref errorOccur);
                        goto UpdateProgressBar;
                    case FileStatus.Deleted:
                        MetaDataManager.DeleteMetaData(filePair.SrcFilePath, filePair.FilePairType);
                        if (filePair.FileStatusInDes == FileStatus.NoChange)
                            DeleteFile(filePair.DesFilePath, ref logger, ref errorOccur);
                        goto UpdateProgressBar;
                    default:
                        if (filePair.FileStatusInDes == FileStatus.NotExist)
                        {
                            CopyFile(filePair.SrcFilePath, Directory.GetParent(filePair.DesFilePath).FullName, ref logger, ref errorOccur);
                            goto UpdateProgressBar;
                        }
                        break;
                }

                //file change in destination
                switch (filePair.FileStatusInDes)
                {
                    case FileStatus.Updated:
                    case FileStatus.New:
                        CopyFile(filePair.DesFilePath, Directory.GetParent(filePair.SrcFilePath).FullName, ref logger, ref errorOccur);
                        goto UpdateProgressBar;
                    case FileStatus.Deleted:
                        MetaDataManager.DeleteMetaData(filePair.DesFilePath, filePair.FilePairType);
                        if (filePair.FileStatusInSrc == FileStatus.NoChange)
                            DeleteFile(filePair.SrcFilePath, ref logger, ref errorOccur);
                        goto UpdateProgressBar;
                    default:
                        if (filePair.FileStatusInSrc == FileStatus.NotExist)
                        {
                            CopyFile(filePair.DesFilePath, Directory.GetParent(filePair.SrcFilePath).FullName, ref logger, ref errorOccur);
                            goto UpdateProgressBar;
                        }
                        break;
                }

                UpdateProgressBar:
                {
                    percent += ((float)0.97) / (listOfDifferences.Count * NumOfJobs);
                    NotifyUpdateProgress(percent);
                }
            }
        }

        /// <summary>
        /// Sync destination to be the same as source
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
                NotifyCheckSyncPaused();
                //if (Aborted) return;

                percent += ((float)0.97) / NumOfJobs;
                NotifyUpdateProgress(percent);
                return;
            }

            foreach (FilePair filePair in listOfDifferences)
            {
                NotifyCheckSyncPaused();
                //if (Aborted) return;

                switch (filePair.FileStatusInSrc)
                {
                    case FileStatus.New:
                    case FileStatus.Updated:
                    case FileStatus.NoChange:
                        CopyFile(filePair.SrcFilePath, Directory.GetParent(filePair.DesFilePath).FullName, ref logger, ref errorOccur);
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
        /// Solve conflicts occurring in the synchronization
        /// </summary>
        /// <param name="srcFilePath"></param>
        /// <param name="fileStatusInSrc"></param>
        /// <param name="desFilePath"></param>
        /// <param name="fileStatusInDes"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private static void SolveConflict(string srcFilePath, FileStatus fileStatusInSrc, string desFilePath, FileStatus fileStatusInDes, ref Logger logger, ref bool errorOccur)
        {
            if ((fileStatusInSrc == FileStatus.New || fileStatusInSrc == FileStatus.Updated) &&
                (fileStatusInDes == FileStatus.New || fileStatusInDes == FileStatus.Updated))
            {
                int difference = FileSystemOperator.GetLastModifiedTime(srcFilePath).CompareTo(FileSystemOperator.GetLastModifiedTime(desFilePath));
                // source is newer or destination is deleted
                if (difference >= 0) //when they have the same last modified date, copy src to des
                    CopyFile(srcFilePath, Directory.GetParent(desFilePath).FullName, ref logger, ref errorOccur);
                else // destination is newer
                    CopyFile(desFilePath, Directory.GetParent(srcFilePath).FullName, ref logger, ref errorOccur);
                return;
            }

            // destination is deleted
            if (fileStatusInSrc == FileStatus.New || fileStatusInSrc == FileStatus.Updated)
            {
                CopyFile(srcFilePath, Directory.GetParent(desFilePath).FullName, ref logger, ref errorOccur);
                return;
            }

            // source is deleted
            if (fileStatusInDes != FileStatus.New && fileStatusInDes != FileStatus.Updated)
                return;
            CopyFile(desFilePath, Directory.GetParent(srcFilePath).FullName, ref logger, ref errorOccur);
            return;
        }


        /// <summary>
        /// Copy file, log the action, update metadata
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="desFolderPath"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        private static void CopyFile(string filePath, string desFolderPath, ref Logger logger, ref bool errorOccur)
        {
            NotifyUpdateAction("COPY\n" + PathOperator.TrimPath(filePath, 57) + "\nTO\n" + PathOperator.TrimPath(desFolderPath, 57));
            if (logger != null) logger.Log(LogType.INFO, "COPY\n" + filePath + "\nTO\n" + desFolderPath);
            FileSystemOperator.CopyFile(filePath, desFolderPath, ref logger, ref errorOccur);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DeleteFile(string filePath, ref Logger logger, ref bool errorOccur)
        {
            bool safeDelete = LogicFacade.LoadSetting("deletetorecyclebin");
            string shortenedFilePath = PathOperator.TrimPath(filePath, 57);
            if (safeDelete)
            {
                if (logger != null) logger.Log(LogType.INFO, "MOVE TO RECYCLE BIN\n" + filePath);
                NotifyUpdateAction("MOVE TO RECYCLE BIN\n" + shortenedFilePath);
                FileSystemOperator.DeleteFileToRecycleBin(filePath, ref logger, ref errorOccur);
            }
            else
            {
                if (logger != null) logger.Log(LogType.INFO, "DELETE\n" + filePath);
                NotifyUpdateAction("DELETE\n" + shortenedFilePath);
                FileSystemOperator.DeleteFilePermanent(filePath, ref logger, ref errorOccur);
            }
        }

        #endregion

        #region GUI EVENT NOTIFIERS

        private static void NotifyUpdateProgress(float percent)
        {
            if (UpdateProgressEvent != null)
                UpdateProgressEvent(percent);
        }

        public static void NotifyUpdateAction(string action)
        {
            if (UpdateActionEvent != null)
                UpdateActionEvent(action);
        }

        private static void NotifyCheckSyncPaused()
        {
            if (CheckSyncPausedEvent != null)
                CheckSyncPausedEvent();
        }

        public static void NotifyUpdateButtons()
        {
            if (UpdateButtonsEvent != null)
                UpdateButtonsEvent();
        }

        private static void NotifyUpdateSyncJobList(int rowIndex, string newStatus)
        {
            if (UpdateSyncJobListEvent != null)
                UpdateSyncJobListEvent(rowIndex, newStatus);
        }

        public static bool NotifyCloseSyncProgress()
        {
            if (CloseSyncProgressEvent != null)
                CloseSyncProgressEvent();
            return false;
        }

        #endregion
    }
}
