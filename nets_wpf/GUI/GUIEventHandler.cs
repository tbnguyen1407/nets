using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using nets_wpf.DataStructures;
using nets_wpf.Logic;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    public static class GUIEventHandler
    {
        private static SyncProgress syncStatus;
        private static ApplicationWindow mainWindow;
        private static TempWindow tempWindow;
        private static Thread thread;
        public static RunningMode syncRunningMode;

        public static void RunMainWindow()
        {
            if (JobQueueHandler.MainIsRunning())
            {
               DisplayMessageBox("NETS is already running!\r\n" + "Please close it before running a new one.",
                                "Program already running");
                return;
            }
            JobQueueHandler.CreateMainFile();

            mainWindow = new ApplicationWindow();
            mainWindow.ShowDialog();
        }

        /// <summary>
        /// </summary>
        /// <param name = "folderPath"></param>
        public static void NoProfileEventHandler(string folderPath)
        {
            Profile defaultProfile = new Profile { SrcFolder = folderPath };
            tempWindow = new TempWindow(new PageProfileDetails(defaultProfile, RunningMode.ContextMenuSmartSync));
            tempWindow.ShowDialog();
        }

        /// <summary>
        /// </summary>
        /// <param name = "folderPath"></param>
        public static void SyncWithEventHandler(string folderPath)
        {
            Profile defaultProfile = new Profile { SrcFolder = folderPath };
            tempWindow = new TempWindow(new PageProfileDetails(defaultProfile, RunningMode.ContextMenuSyncWith));
            tempWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="profileNameList"></param>
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

            tempWindow = new TempWindow(new PageProfileList(formattedFolderPath, ref profileNameList, ref profileList));
            tempWindow.ShowDialog();
            //Application.Run(new TempWindow(new PageProfileList(folderPath, ref profileNameList, ref profileList)));
        }

        /// <summary>
        /// </summary>
        /// <param name = "profileName"></param>
        public static void SyncHandler(string profileName)
        {
            Profile profile = LogicFacade.LoadProfile(profileName);
            SyncHandler(profile.SrcFolder, profile.DesFolder, profile.SyncMode, profile.IncludePattern,
                        profile.ExcludePattern);
        }

        /// <summary>
        /// </summary>
        /// <param name = "srcFolderPath"></param>
        /// <param name = "desFolderPath"></param>
        /// <param name = "syncMode"></param>
        /// <param name = "includePattern"></param>
        /// <param name = "excludePattern"></param>
        public static void SyncHandler(string srcFolderPath, string desFolderPath, SyncMode syncMode,
                                       string includePattern, string excludePattern)
        {
            /*
            if (JobQueueHandler.SyncInProgress())
            {
                ShowCannotSyncMessage();
                return;
            }
            */
            //JobQueueHandler.CreateMainSyncFile();

            thread = new Thread(delegate() { LogicFacade.Sync(srcFolderPath, desFolderPath, syncMode, includePattern, excludePattern); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
 
            JobQueueHandler.CreateMainSyncFile();
            DisplaySyncProgress(srcFolderPath, desFolderPath);
        }

        /// <summary>
        /// </summary>
        /// <param name = "folderPathList"></param>
        public static void SyncHandler(ref List<string> folderPathList)
        {
            //dummy window to keep application continue when temp window closed
            BackgroundWindow bgWindow = new BackgroundWindow();
            bgWindow.Show();
            bgWindow.Hide();
            LogicFacade.PrepareSyncJob(ref folderPathList);

            if (!LogicFacade.HasJobToSync())
                return;

            Profile[] jobList = LogicFacade.GetSyncJobs().ToArray();

            thread = new Thread(delegate() { LogicFacade.DoSyncJobs(); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Thread.Sleep(100);
            DisplaySyncProgress(jobList);            
            bgWindow.Close();
        }

        private static bool GetSyncEnabler()
        {
            return syncStatus == null || syncStatus.GetSyncEnabler();
        }

        public static bool GetSyncAborter()
        {
            return syncStatus == null;
        }

        public static void CloseSyncProgress()
        {
            if (syncStatus != null)
                syncStatus.Close();
            syncStatus = null;
        }

        public static void AbortSyncProgress()
        {
            thread.Abort();
            if (syncStatus != null) syncStatus.Close();
        }

        private static void DisplaySyncProgress(string srcFolder, string desFolder)
        {
            syncStatus = new SyncProgress(srcFolder, desFolder);
            syncStatus.ShowDialog();
        }

        private static void DisplaySyncProgress(Profile[] jobList)
        {
            syncStatus = new SyncProgress(jobList);
            syncStatus.ShowDialog();
        }

        public static void UpdateProgressEventHandler(float percent)
        {
            if (syncStatus != null)
                syncStatus.SetProgressBarValue(percent);
        }

        public static void UpdateActionEventHandler(string action)
        {
            if (syncStatus != null)
                syncStatus.SetLabelText(action);
        }

        public static void UpdateSyncJobListEventHandler(int rowIndex, string newStatus)
        {
            if (syncStatus != null)
                syncStatus.UpdateJobList(rowIndex, newStatus);
        }

        public static void CheckSyncPausedEventHandler()
        {
            if (!GetSyncEnabler())
                syncStatus.SetLabelText("Sync Paused");
            while (!GetSyncEnabler())
            {
            }
        }

        public static void UpdateButtonsEventHandler()
        {
            if (syncStatus != null)
                syncStatus.ChangeAppearanceAfterFinishing();
        }


        public static void ReportSyncException()
        {
            PopUpMessageBox messageBox = new PopUpMessageBox("The sync job has been completed\r\n" +
                                                        "However, some error has occured in the sync progress\r\n" +
                                                        "Do you want to view log to check the error?",
                                                        "Error notification");
            bool? userResponse = messageBox.ShowDialog();
            
            if ((bool) userResponse)
                ShowLog();
        }

        public static void ShowCannotSyncMessage()
        {
            DisplayMessageBox("Sorry! We cannot sync now because a sync job is still running.\r\n" +
                            "Please make sure it is completed or aborted before syncing more.\r\n" +
                            "Thank you!",
                            "Sync Job Incompleted");
        }

        public static void ShowLog()
        {
            string logPath = "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                             @"\nets\logs\SYNC JOB LOGGER.dat" + "\"";
            Process.Start("wordpad.exe", logPath);
        }

        public static void DisplayMessageBox(string message, string header)
        {
            PopUpMessageBox messageBox = new PopUpMessageBox(message, header);
            messageBox.ChangeToPopUpMessage();
            messageBox.ShowDialog();
        }
    }
}
