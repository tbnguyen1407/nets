using System;
using System.Collections.Generic;
using System.IO;
using nets_wpf.DataStructures;
using nets_wpf.Storage;
using nets_wpf.Utility;

namespace nets_wpf.Logic
{
    #region SYNC DELEGATE

    public delegate void DoJobDelegate();
    public delegate void SyncDelegate(string srcFolderPath, string desFolderPath, SyncMode syncMode, string includePattern, string excludePattern);

    #endregion

    /// <summary>
    /// Acts as a bridge between UI and Logic
    /// </summary>
    public static class LogicFacade
    {
        #region PRIVATE FIELD DECLARATION
        
        private static readonly Detector detector = new Detector();
        private static readonly Reconciler reconciler = new Reconciler();
        private static Profile currentProfile = new Profile();
        private static Queue<Profile> syncJobs = new Queue<Profile>();
        private static Logger logger = StorageFacade.GetLogger("SYNC JOB LOGGER");
        private static bool errorOccur;

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
            reconciler.JobNumber = 0;

            errorOccur = false;
            
            Synchronize(srcFolder, desFolder, syncMode, includePattern, excludePattern, true);

            Reconciler.NotifyUpdateButtons();
            if (errorOccur) NotifyOfReportSyncError();
        }

        public static void AbortSync()
        {
            reconciler.Aborted = true;
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
                        string correspondingFolderPath = PathOperator.GetCorrespondingPath(containingProfile.SrcFolder, containingProfile.DesFolder, normalizedFolderPath);
                        if (!Directory.Exists(correspondingFolderPath))
                            Directory.CreateDirectory(correspondingFolderPath);
                        string normalizedCorrespondingFolderPath = PathOperator.NormalizeFolderPath(correspondingFolderPath);
                        syncJobs.Enqueue(new Profile("", normalizedFolderPath, normalizedCorrespondingFolderPath, containingProfile.SyncMode, containingProfile.IncludePattern, containingProfile.ExcludePattern));
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
            //syncJobs.Enqueue(new Profile("", srcFolderPath, desFolderPath, syncMode, new FilterPattern(includePattern, excludePattern)));
            syncJobs.Enqueue(new Profile("", srcFolderPath, desFolderPath, syncMode, includePattern, excludePattern));
        }

        /// <summary>
        /// Add a new sync job with specified info into the waiting list
        /// </summary>
        /// <param name="srcFolderPath"></param>
        /// <param name="desFolderPath"></param>
        /// <param name="syncMode"></param>
        /// <param name="filterPattern"></param>
        public static void EnqueueSyncJob(string srcFolderPath, string desFolderPath, SyncMode syncMode, FilterPattern filterPattern)
        {
            //syncJobs.Enqueue(new Profile("", srcFolderPath, desFolderPath, syncMode, filterPattern));
            syncJobs.Enqueue(new Profile("", srcFolderPath, desFolderPath, syncMode, filterPattern.IncludePattern, filterPattern.ExcludePattern));
        }

        /// <summary>
        /// Execute the waiting sync job list one by one
        /// </summary>
        public static void DoSyncJobs()
        {
            reconciler.NumOfJobs = syncJobs.Count;
            reconciler.StartPercent = (float)0.0;
            reconciler.JobNumber = 0;
            Profile profile;
            errorOccur = false;
            System.Threading.Thread.Sleep(100);
            while (syncJobs.Count > 1)
            {
                profile = syncJobs.Dequeue();
                Synchronize(profile.SrcFolder, profile.DesFolder, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern, false);

                reconciler.JobNumber++;
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
            Reconciler.NotifyUpdateAction("Initializing...");
            if (logger != null) logger.LogSyncJob(srcFolder, desFolder);
            InitializeMetaData(srcFolder, desFolder);
            FileSystemOperator.Filter = new FilterPattern(includePattern, excludePattern);

            // Detecting differences
            Reconciler.NotifyUpdateAction("Detecting differences...");
            List<FilePair> listOfDifferences = detector.GetDifferences(srcFolder, desFolder, ref logger, ref errorOccur);

            // Reconciling
            reconciler.Synchronize(ref listOfDifferences, syncMode, ref logger, ref errorOccur);

            // Finalizing
            if (isLastJob) Reconciler.NotifyUpdateAction("Finalizing...");
            if (logger != null) logger.EndLog();
            FinalizeMetaData(srcFolder, desFolder);
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
        /// <param name="profile"></param>
        public static void SaveProfile(Profile profile)
        {
            StorageFacade.SaveProfile(profile);
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
        /// <param name="folderPath"></param>
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
