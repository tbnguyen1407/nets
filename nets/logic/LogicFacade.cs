using System;
using System.Collections.Generic;
using System.IO;
using nets.dataclass;
using nets.utility;
using nets.storage;
using System.Threading;

namespace nets.logic
{
    /// <summary>
    /// Acts as a bridge between UI and Logic
    /// Author: Hoang Nguyen Nhat Tao + Nguyen Thi Yen Duong + Nguyen Thi Thu Giang + Tran Binh Nguyen
    /// </summary>
    public static class LogicFacade
    {
        #region PRIVATE FIELD DECLARATION
        
        private static readonly Detector detector = new Detector();
        private static readonly Reconciler reconciler = new Reconciler();
        private static Profile currentProfile = new Profile();
        private static Queue<Profile> syncJobs = new Queue<Profile>();
        private static Logger logger = StorageFacade.LoadLogger("SYNC JOB LOGGER");
        private static bool errorOccur;
        private static int jobNumber;

        #endregion

        #region OBSERVER DELEGATES AND EVENTS

        public delegate void NoProfileHandler(string folderPath);
        public delegate void MultipleProfilesHandler(string folderPath, ref List<string> profileNameList);
        public delegate void ReportSyncErrorHandler();
        
        public static event NoProfileHandler NoProfileEvent;
		public static event MultipleProfilesHandler MultipleProfilesEvent;
        public static event ReportSyncErrorHandler ReportSyncErrorEvent;

        #endregion

        #region LOGIC-RELATED METHODS

        #region PUBLIC METHODS

        /// <summary>
        /// Sync two folders based on specified sync mode and include, exclude pattern
        /// </summary>
        /// <param name="srcFolder">path of the source folder</param>
        /// <param name="desFolder">path of the destination folder</param>
        /// <param name="syncMode">sync mode: one way/two way</param>
        /// <param name="includePattern">pattern of files to be included in sync</param>
        /// <param name="excludePattern">pattern of files to be excluded in sync</param>
        public static void Sync(string srcFolder, string desFolder, SyncMode syncMode, string includePattern, string excludePattern)
        {
            reconciler.NumOfJobs = 1;
            reconciler.StartPercent = (float)0.0;
            jobNumber = 0;

            FileSystemOperator.SafeDelete = LoadSetting("deletetorecyclebin");
            errorOccur = false;

            Thread.Sleep(600);

            Synchronize(srcFolder, desFolder, syncMode, includePattern, excludePattern, true);

            Reconciler.NotifyUpdateButtons();
            if (errorOccur) NotifyOfReportSyncError();
        }

        /// <summary>
        /// Get the currently used profile
        /// </summary>
        /// <returns></returns>
        public static Profile GetCurrentProfile()
        {
            return currentProfile;
        }

        /// <summary>
        /// Set the currently used currentProfile
        /// </summary>
        /// <param name="newProfile"></param>
        public static void SetCurrentProfile(Profile newProfile)
        {
            currentProfile = newProfile;
        }

        /// <summary>
        /// Construct a list of jobs to be synced from given folder paths
        /// </summary>
        /// <param name="folderPaths">given folder path list</param>
        public static void PrepareSyncJob(ref List<string> folderPaths)
        {
            syncJobs = new Queue<Profile>();
            List<string> listOfContainingProfiles;

            foreach (string folderPath in folderPaths)
            {
                string normalizedFolderPath = PathOperator.NormalizeFolderPath(folderPath);
                listOfContainingProfiles = LoadContainingProfileNameList(normalizedFolderPath);

                switch (listOfContainingProfiles.Count)
                {
                    case 0:
                        NotifyOfNoProfile(normalizedFolderPath);
                        break;
                    case 1:
                        Profile containingProfile = LoadProfile(listOfContainingProfiles[0]);
                        string normalizedSrcPath = PathOperator.GetSrcSubPath(containingProfile.SrcFolder, containingProfile.DesFolder, normalizedFolderPath);
                        string normalizedDesPath = PathOperator.GetDesSubPath(containingProfile.SrcFolder, containingProfile.DesFolder, normalizedFolderPath);
                        EnqueueSyncJob(normalizedSrcPath, normalizedDesPath, containingProfile.SyncMode, containingProfile.IncludePattern, containingProfile.ExcludePattern);
                        break;
                    default:
                        NotifyOfMultipleProfiles(normalizedFolderPath, ref listOfContainingProfiles);
                        break;
                }
            }
        }

        /// <summary>
        /// Add a new sync job with specified info into the waiting list
        /// </summary>
        /// <param name="srcFolderPath"></param>
        /// <param name="desFolderPath"></param>
        /// <param name="syncMode"></param>
        /// <param name="includePattern"></param>
        /// <param name="excludePattern"></param>
        public static void EnqueueSyncJob(string srcFolderPath, string desFolderPath, SyncMode syncMode, string includePattern, string excludePattern)
        {
            string normalizedSrcPath = PathOperator.NormalizeFolderPath(srcFolderPath);
            string normalizedDesPath = PathOperator.NormalizeFolderPath(desFolderPath);

            try
            {
                if (!Directory.Exists(normalizedSrcPath))
                    Directory.CreateDirectory(normalizedSrcPath);
                if (!Directory.Exists(normalizedDesPath))
                    Directory.CreateDirectory(normalizedDesPath);
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied for " + srcFolderPath + " or " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Either " + srcFolderPath + " or " + desFolderPath + " can not be found", ref logger, ref errorOccur);
            }
            catch (IOException)
            {
                ExceptionHandler.HandleException("IO error has occured while creating " + srcFolderPath + " or " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknow exception has occured while accessing " + srcFolderPath + " and " + desFolderPath, ref logger, ref errorOccur);
            }

            syncJobs.Enqueue(new Profile("", normalizedSrcPath, normalizedDesPath, syncMode, includePattern, excludePattern));
        }

        /// <summary>
        /// Execute the waiting sync job list one by one
        /// </summary>
        public static void DoSyncJobs()
        {
            reconciler.NumOfJobs = syncJobs.Count;
            reconciler.StartPercent = (float)0.0;
            jobNumber = 0;

            FileSystemOperator.SafeDelete = LoadSetting("deletetorecyclebin");
            Profile profile;
            errorOccur = false;

            Thread.Sleep(600);

            while (syncJobs.Count > 1)
            {
                profile = syncJobs.Dequeue();
                Synchronize(profile.SrcFolder, profile.DesFolder, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern, false);

                jobNumber++;
                reconciler.StartPercent += ((float)(0.97)) / reconciler.NumOfJobs;
            }

            profile = syncJobs.Dequeue();
            Synchronize(profile.SrcFolder, profile.DesFolder, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern, true);

            Reconciler.NotifyUpdateButtons();
            if (errorOccur) NotifyOfReportSyncError();
        }

        /// <summary>
        /// Check to see if there is still job in the queue
        /// </summary>
        /// <returns></returns>
        public static bool HasJobToSync()
        {
            return syncJobs.Count > 0;
        }

        /// <summary>
        /// Retrieve the waiting sync job list
        /// </summary>
        /// <returns></returns>
        public static Queue<Profile> GetSyncJobs()
        {
            return syncJobs;
        }

        #endregion

        #region PRIVATE HELPERS

        private static void Synchronize(string srcFolder, string desFolder, SyncMode syncMode, string includePattern, string excludePattern, bool isLastJob)
        {
            // Initializing
            Reconciler.NotifySetAbortButton(false);
            Thread.Sleep(100);
            Reconciler.NotifyUpdateSyncJobList(jobNumber, "Syncing");
            Reconciler.NotifyUpdateAction("Initializing...");
            if (logger != null) logger.LogSyncJob(srcFolder, desFolder);
            InitializeMetaData(srcFolder, desFolder);
            FileSystemOperator.Filter = new FilterPattern(includePattern, excludePattern);
            Reconciler.NotifySetAbortButton(true);
            Thread.Sleep(100);

            // Detecting differences
            Reconciler.NotifyUpdateAction("Detecting differences...");
            List<FilePair> listOfDifferences = detector.GetDifferences(srcFolder, desFolder, ref logger, ref errorOccur);

            // Reconciling
            reconciler.Synchronize(ref listOfDifferences, syncMode, ref logger, ref errorOccur);

            // Finalizing
            Reconciler.NotifySetAbortButton(false);
            Thread.Sleep(100);
            if (isLastJob) Reconciler.NotifyUpdateAction("Finalizing...");
            if (logger != null) logger.EndLog();
            FinalizeMetaData(srcFolder, desFolder);
            Reconciler.NotifyUpdateSyncJobList(jobNumber, "Synced");
            Reconciler.NotifySetAbortButton(true);
        }

        #endregion

        #endregion

        #region STORAGE-RELATED METHODS

        #region PUBLIC METHODS

        /// <summary>
        /// Initialize storage at program startup
        /// </summary>
        public static void InitializeStorage()
        {
            StorageFacade.InitializeStorage();
        }

        /// <summary>
        /// Save currentProfile
        /// </summary>
        /// <param name="oldProfile"></param>
        /// <param name="newProfile"></param>
        public static void EditAndSaveProfile(Profile oldProfile, Profile newProfile)
        {
            StorageFacade.EditAndSaveProfile(oldProfile, newProfile);
        }

        /// <summary>
        /// Load currentProfile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static Profile LoadProfile(string profileName)
        {
            return StorageFacade.LoadProfile(profileName);
        }

        /// <summary>
        /// Delete currentProfile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static void DeleteProfile(String profileName)
        {
            StorageFacade.DeleteProfile(profileName);
        }

        /// <summary>
        /// Retrieve the currentProfile name list
        /// </summary>
        /// <returns></returns>
        public static List<String> LoadProfileNameList()
        {
            return StorageFacade.LoadProfileNameList();
        }

        /// <summary>
        /// Load a list of conflicted profiles
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <returns></returns>
        public static List<string> GetConflictedProfileNameList(string srcFolder, string desFolder)
        {
            return StorageFacade.LoadConflictedProfileNameList(srcFolder, desFolder);
        }

        /// <summary>
        /// Save a setting value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SaveSetting(string property, string value)
        {
            if (property == "deletetorecyclebin")
                FileSystemOperator.SafeDelete = (value == "1");
            StorageFacade.SaveSetting(property, value);
        }

        /// <summary>
        /// Load a setting value
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool LoadSetting(string property)
        {
            return StorageFacade.LoadSetting(property);
        }

        /// <summary>
        /// Create and save a new profile
        /// </summary>
        /// <param name="newProfile"></param>
        public static void CreateAndSaveProfile(Profile newProfile)
        {
            StorageFacade.CreateAndSaveProfile(newProfile);
        }

        #endregion

        #region PRIVATE HELPERS

        /// <summary>
        /// Initialize metadata at the beginning of a sync job
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        private static void InitializeMetaData(string srcFolder, string desFolder)
        {
            StorageFacade.InitializeMetaData(srcFolder, desFolder);
        }

        /// <summary>
        /// Finalize metadata at the end of a sync job
        /// </summary>
        private static void FinalizeMetaData(string srcFolder, string desFolder)
        {
            StorageFacade.FinalizeMetaData(srcFolder, desFolder);
        }

        /// <summary>
        /// Retrieve the currentProfile name list containing a specific path
        /// </summary>
        /// <param name="formattedFolderPath"></param>
        /// <returns></returns>
        private static List<string> LoadContainingProfileNameList(string folderPath)
        {
            return StorageFacade.LoadContainingProfileNameList(folderPath);
        }

        #endregion

        #endregion

        #region OBSERVER EVENT NOTIFIERS

        private static void NotifyOfNoProfile(string folderPath)
        {
            if (NoProfileEvent != null)
                NoProfileEvent(folderPath);
        }

        private static void NotifyOfMultipleProfiles(string folderPath, ref List<string> profileNameList)
        
        {
            if (MultipleProfilesEvent != null)
                MultipleProfilesEvent(folderPath, ref profileNameList);
        }

        private static void NotifyOfReportSyncError()
        {
            if (ReportSyncErrorEvent != null)
                ReportSyncErrorEvent();
        }

        #endregion
    }
}
