using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using nets.dataclass;
using nets.logic;
using nets.utility;
using System.Threading;

namespace nets.gui
{
    /// <summary>
    /// Handle events occuring in other window forms
    /// Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public static class GUIEventHandler
    {
        #region PRIVATE FIELDS

        private static SyncProgress syncStatus;
        private static ApplicationWindow mainWindow;
        private static TempWindow tempWindow;
        private static Thread thread;

        #endregion

        #region PROPERTIES

        public static RunningMode syncRunningMode;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Start the main window
        /// </summary>
        public static void RunMainWindow()
        {
            if (JobQueueHandler.MainIsRunning())
            {
                MessageBox.Show("NETS is already running!\r\n" + "Please close it before running a new one.",
                                "Program already running",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }
            JobQueueHandler.CreateMainFile();

            mainWindow = new ApplicationWindow(new PageStart(), new PageIVLE(), new PageSettings());
            mainWindow.ShowDialog();
        }

        /// <summary>
        /// Handle the case when a folder is not in any profile in "Smart Sync"
        /// </summary>
        /// <param name="formattedFolderPath">path of the folder</param>
        public static void NoProfileEventHandler(string folderPath)
        {
            Profile defaultProfile = new Profile {SrcFolder = folderPath};
            tempWindow = new TempWindow(new PageProfileDetails(defaultProfile, RunningMode.ContextMenuSmartSync));
            tempWindow.ShowDialog();
        }

        /// <summary>
        /// Handle the event when user right click "Sync With..."
        /// </summary>
        /// <param name="formattedFolderPath">path of the folder</param>
        public static void SyncWithEventHandler(string folderPath)
        {
            Profile defaultProfile = new Profile {SrcFolder = folderPath};
            tempWindow = new TempWindow(new PageProfileDetails(defaultProfile, RunningMode.ContextMenuSyncWith));
            tempWindow.ShowDialog();
        }

        /// <summary>
        /// Handle the case when a folder is many profiles in "Smart Sync"
        /// </summary>
        /// <param name="formattedFolderPath">path of the folder</param>
        /// <param name="profileNameList">list of profile names</param>
        public static void MultipleProfileEventHandler(string folderPath, ref List<string> profileNameList)
        {
            List<string> profileList = new List<string>();
            string formattedFolderPath = PathOperator.TrimPath(folderPath, 50);

            foreach (string profileName in profileNameList)
            {
                Profile profile = LogicFacade.LoadProfile(profileName);

                string formattedSrcPath = PathOperator.TrimPath(profile.SrcFolder, 50);
                string formattedDesPath = PathOperator.TrimPath(profile.DesFolder, 50);
                string formattedProfileName = PathOperator.TrimPath(profile.ProfileName, 45);
                string profileInfo = " " + formattedProfileName + "\n" +
                                     "   Src: " + formattedSrcPath + "\n" +
                                     "   Des: " + formattedDesPath;

                profileList.Add(profileInfo);
            }

            tempWindow = new TempWindow(new PageProfileList(folderPath, formattedFolderPath, ref profileNameList, ref profileList));
            tempWindow.ShowDialog();
        }

        /// <summary>
        /// Handler the event of syncing a profile
        /// </summary>
        /// <param name="profileName">name of the profile</param>
        public static void SyncHandler(string profileName)
        {
            Profile profile = LogicFacade.LoadProfile(profileName);
            SyncHandler(profile.SrcFolder, profile.DesFolder, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern);
        }

        /// <summary>
        /// Handle the event of syncing given sync infos
        /// </summary>
        /// <param name="srcFolderPath">path of the source folder</param>
        /// <param name="desFolderPath">path of the destination folder</param>
        /// <param name="syncMode">sync mode (oneway/twoway)</param>
        /// <param name="includePattern">pattern of files to be included</param>
        /// <param name="excludePattern">pattern of files to be excluded</param>
        public static void SyncHandler(string srcFolderPath, string desFolderPath, SyncMode syncMode,
                                       string includePattern, string excludePattern)
        {
            thread = new Thread(() => LogicFacade.Sync(srcFolderPath, desFolderPath, syncMode, includePattern, excludePattern));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            JobQueueHandler.CreateMainSyncFile();
            DisplaySyncProgress(srcFolderPath, desFolderPath);
        }

        /// <summary>
        /// Handle the event of "Smart Sync" given a list of folder paths
        /// </summary>
        /// <param name="folderPathList">list of folder paths</param>
        public static void SyncHandler(ref List<string> folderPathList)
        {
            LogicFacade.PrepareSyncJob(ref folderPathList);

            if (!LogicFacade.HasJobToSync())
                return;

            Profile[] jobList = LogicFacade.GetSyncJobs().ToArray();

            thread = new Thread(LogicFacade.DoSyncJobs);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            DisplaySyncProgress(jobList);
        }

        /// <summary>
        /// Abort the current sync job
        /// </summary>
        public static void AbortSyncProgress()
        {
            try
            {
                thread.Resume();
                thread.Abort();
            }
            catch(Exception) { }
            if (syncStatus != null) syncStatus.Close();
        }

        /// <summary>
        /// Pause the current sync job
        /// </summary>
        public static void PauseSyncProgress()
        {
            if (syncStatus == null)
                return;
            thread.Suspend();
        }

        /// <summary>
        /// Continue the current sync job
        /// </summary>
        public static void ContinueProgress()
        {
            if (thread != null && thread.IsAlive)
                thread.Resume();
        }

        /// <summary>
        /// Update the percentage shown in the progress bar
        /// </summary>
        /// <param name="percent">percentage to be shown in the progress bar</param>
        public static void UpdateProgressEventHandler(float percent)
        {
            if (syncStatus != null)
                syncStatus.SetProgressBarValue(percent);
        }

        /// <summary>
        /// Update the action in the label
        /// </summary>
        /// <param name="action">action to be shown on the label</param>
        public static void UpdateActionEventHandler(string action)
        {
            if (syncStatus != null)
                syncStatus.SetLabelText(action);
        }

        /// <summary>
        /// Update status of a job in the sync job list
        /// </summary>
        /// <param name="rowIndex">index of the row containing that job</param>
        /// <param name="newStatus">status to be updated</param>
        public static void UpdateSyncJobListEventHandler(int rowIndex, string newStatus)
        {
            if (syncStatus != null)
                syncStatus.UpdateJobList(rowIndex, newStatus);
        }

        /// <summary>
        /// Update the buttons
        /// </summary>
        public static void UpdateButtonsEventHandler()
        {
            if (syncStatus != null)
                syncStatus.ChangeAppearanceAfterFinishing();
        }

        /// <summary>
        /// Set the state of the Abort button (enabled/disabled)
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetAbortButtonEventHandler(bool enabled)
        {
            if (syncStatus != null)
                syncStatus.SetAbortButton(enabled);
        }

        /// <summary>
        /// Report the exception found during sync job if available
        /// </summary>
        public static void ReportSyncException()
        {
            DialogResult userResponse = MessageBox.Show("The sync job has been completed\r\n" +
                                                        "However, some error has occured in the sync progress\r\n" +
                                                        "Do you want to view log to check the error?",
                                                        "Error notification",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
            if (userResponse == DialogResult.Yes)
                ShowLog();
        }

        /// <summary>
        /// Inform the user that no more sync job can be performed
        /// </summary>
        public static void ShowCannotSyncMessage()
        {
            MessageBox.Show("Sorry! We cannot sync now because a sync job is still running.\r\n" +
                            "Please make sure it is completed or aborted before syncing more.\r\n" +
                            "Thank you!",
                            "Sync Job Incompleted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Show the log
        /// </summary>
        public static void ShowLog()
        {
            string logPath = "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                             @"\nets\logs\SYNC JOB LOGGER.dat" + "\"";
            Process.Start("wordpad.exe", logPath);
        }

        #endregion

        #region PRIVATE HELPERS

        /// <summary>
        /// Show the sync progress
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        private static void DisplaySyncProgress(string srcFolder, string desFolder)
        {
            syncStatus = new SyncProgress(srcFolder, desFolder);
            syncStatus.ShowDialog();
        }

        /// <summary>
        /// Show the sync progress
        /// </summary>
        /// <param name="jobList"></param>
        private static void DisplaySyncProgress(Profile[] jobList)
        {
            syncStatus = new SyncProgress(jobList);
            syncStatus.ShowDialog();
        }

        #endregion
    }
}