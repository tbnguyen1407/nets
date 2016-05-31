using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using nets_wpf.DataStructures;

namespace nets_wpf.Storage
{
    /// <summary>
    ///   Acts as a bridge between Logic and Storage
    /// </summary>
    public static class StorageFacade
    {
        #region PRIVATE FIELD DECLARATION
        
        public static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\nets\";
        private static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static readonly string ProfileStorageFile = AppDataFolder + "\\profiles\\nets_profiles.dat";
        public static readonly string LogFolder = AppDataFolder + "logs\\";
        public static readonly string MetaDataFolder = AppDataFolder + "metadata\\";
        public static readonly string SettingFilePath = ApplicationFolder + "\\settings.ini";

        #endregion

        #region STORAGEFACADE MAIN METHODS

        /// <summary>
        /// Initialize storage at program startup
        /// </summary>
        public static void InitializeStorage()
        {
            ProfileHolder.GetInstance().InitializeProfile();
            SettingHolder.GetInstance().InitializeSetting();
        }

        /// <summary>
        /// Initialize metadata at the beginning of a sync job
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        public static void InitializeMetaData(string srcFolder, string desFolder)
        {
            MetaDataHolder.GetInstance().InitializeMetaData(srcFolder, desFolder);
        }

        /// <summary>
        /// Finalize metadata at the end of a sync job
        /// </summary>
        public static void FinalizeMetaData(string srcFolder, string desFolder)
        {
            MetaDataHolder.GetInstance().FinalizeMetaData(srcFolder, desFolder);
            SaveLogger();
        }

        #endregion

        #region PROFILE-RELATED METHODS

        /// <summary>
        /// Add a new currentProfile entry to currentProfile storage file
        /// </summary>
        /// <param name="profile"></param>
        public static void SaveProfile(Profile profile)
        {
            ProfileHolder.GetInstance().SaveProfile(profile);
        }

        /// <summary>
        ///   Load a currentProfile from currentProfile storage file
        /// </summary>
        /// <param name = "profileName"></param>
        /// <returns></returns>
        public static Profile LoadProfile(string profileName)
        {
            return ProfileHolder.GetInstance().LoadProfile(profileName);
        }

        /// <summary>
        ///   Load the list of all profiles from the currentProfile storage file
        /// </summary>
        /// <returns></returns>
        public static List<String> LoadProfileNameList()
        {
            return ProfileHolder.GetInstance().LoadProfileNameList();
        }

        /// <summary>
        ///   Load the list of all profiles containing specific folder path
        /// </summary>
        /// <returns>The list of all matching currentProfile names</returns>
        public static List<String> LoadContainingProfileNameList(string folderPath)
        {
            return ProfileHolder.GetInstance().LoadContainingProfileNameList(folderPath);
        }

        /// <summary>
        /// Load the list of profiles conflicting with the src and des specified
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <returns></returns>
        public static List<string> LoadConflictedProfileNameList(string srcFolder, string desFolder)
        {
            return ProfileHolder.GetInstance().LoadProfileNameListToUse(srcFolder, desFolder);
        }
 
        /// <summary>
        ///   Remove a currentProfile from the currentProfile storage file
        /// </summary>
        /// <param name = "profileName">The currentProfile name to load</param>
        /// <returns></returns>
        public static void DeleteProfile(String profileName)
        {
            ProfileHolder.GetInstance().DeleteProfile(profileName);
        }

        #endregion

        #region LOGGER-RELATED METHODS

        /// <summary>
        ///   Return the logger instance with specified name
        /// </summary>
        /// <param name = "loggerName">Name of the logger instance</param>
        /// <returns>The logger instance with specified name</returns>
        public static Logger GetLogger(string loggerName)
        {
            return LoggerHolder.GetInstance().GetLogger(loggerName);
        }

        /// <summary>
        ///   Save all logger instances' information to storage
        /// </summary>
        /// <returns>true if the logger storage file exists, false otherwise</returns>
        private static void SaveLogger()
        {
            LoggerHolder.GetInstance().SaveLogger();
        }

        #endregion

        #region METADATA-RELATED METHODS

        /// <summary>
        ///   Store the metadata of a filesystem path to the metadata hashtable
        /// </summary>
        /// <param name = "filesystemPath">The absolute filesystem path</param>
        /// <param name = "metaData">The metadata of the filesystem path</param>
        public static void SaveMetaData(string filesystemPath, string metaData)
        {
            MetaDataHolder.GetInstance().SaveMetaData(filesystemPath, metaData);
        }

        /// <summary>
        ///   Load the metadata of a filesystem path from the metadata hashtable
        /// </summary>
        /// <param name = "filesystemPath">The absolute filesystem path</param>
        public static string LoadMetaData(string filesystemPath)
        {
            return MetaDataHolder.GetInstance().LoadMetaData(filesystemPath);
        }

        /// <summary>
        ///   Remove the metadata of a filesystem path from the metadata hashtable
        /// </summary>
        /// <param name = "filesystemPath">The absolute filesystem path</param>
        public static void DeleteMetaData(string filesystemPath)
        {
            MetaDataHolder.GetInstance().DeleteMetaData(filesystemPath);
        }

        #endregion

        #region SETTING-RELATED METHODS

        /// <summary>
        /// Save a setting value
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SaveSetting(string property, string value)
        {
            SettingHolder.GetInstance().SaveSetting(property, value);
        }

        /// <summary>
        /// Load a setting value
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool LoadSetting(string property)
        {
            return SettingHolder.GetInstance().LoadSetting(property);
        }

        #endregion
    }
}