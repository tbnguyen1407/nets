using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using nets.dataclass;

namespace nets.storage
{
    /// <summary>
    ///   Acts as a bridge between Logic and Storage
    ///   Author: Nguyen Hoang Hai + Tran Binh Nguyen
    /// </summary>
    public static class StorageFacade
    {
        #region FIELD DECLARATION
        
        public static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\nets\\";
        public static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\";

        public static readonly string ProfileStorageFile = AppDataFolder + "profiles\\nets_profiles.dat";
        public static readonly string LogFolder = AppDataFolder + "logs\\";
        public static readonly string MetaDataFolder = AppDataFolder + "metadata\\";
        public static readonly string SettingFilePath = ApplicationFolder + "settings.ini";

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Initialize storage at program startup
        /// </summary>
        public static void InitializeStorage()
        {
            ProfileHolder.GetInstance().InitializeProfile();
            SettingHolder.GetInstance().InitializeSetting();
            LoggerHolder.GetInstance().InitilizeLogger();
        }

        #endregion

        #region PROFILE-RELATED METHODS

        /// <summary>
        /// Save a new profile 
        /// </summary>
        /// <param name="profile"></param>
        public static void CreateAndSaveProfile(Profile profile)
        {
            ProfileHolder.GetInstance().SaveProfile(profile);
            ProfileHolder.GetInstance().FinalizeProfile();
        }

        /// <summary>
        /// Save an editted profile
        /// </summary>
        /// <param name="oldProfile"></param>
        /// <param name="newProfile"></param>
        public static void EditAndSaveProfile(Profile oldProfile, Profile newProfile)
        {
            string oldMetadataFile = MetaDataFolder + oldProfile.ProfileName + ".dat";
            string newMetadataFile = MetaDataFolder + newProfile.ProfileName + ".dat";

            List<string> profileNameListToUse = LoadConflictedProfileNameList(newProfile.SrcFolder, newProfile.DesFolder);
            
            bool IsMetadataReused = (profileNameListToUse.Count != 0);

            if (File.Exists(oldMetadataFile))
            {
                if (IsMetadataReused)
                    File.Move(oldMetadataFile, newMetadataFile);
                else
                    File.Delete(oldMetadataFile);
            }

            ProfileHolder.GetInstance().DeleteProfile(oldProfile.ProfileName);
            ProfileHolder.GetInstance().SaveProfile(newProfile);
            ProfileHolder.GetInstance().FinalizeProfile();
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
        ///   Remove a currentProfile from the currentProfile storage file
        /// </summary>
        /// <param name = "profileName">The currentProfile name to load</param>
        /// <returns></returns>
        public static void DeleteProfile(String profileName)
        {
            // Delete profile from hashtable in RAM
            ProfileHolder.GetInstance().DeleteProfile(profileName);

            // Delete corresponding metadata file
            string metaDataFilePath = MetaDataFolder + profileName + ".dat";
            File.Delete(metaDataFilePath);

            // Update changes to file
            ProfileHolder.GetInstance().FinalizeProfile();
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
            List<string> containingProfileNameList = new List<string>();
            List<string> profileNameList = ProfileHolder.GetInstance().LoadProfileNameList();

            string normalizedFolderPath = folderPath.ToLower();
            
            if (!normalizedFolderPath.EndsWith(@"\"))
                normalizedFolderPath += @"\";

            foreach (string profileName in profileNameList)
            {
                Profile profile = ProfileHolder.GetInstance().LoadProfile(profileName);
                string srcFolder = profile.SrcFolder.ToLower();
                string desFolder = profile.DesFolder.ToLower();
                if (normalizedFolderPath.StartsWith(srcFolder) || normalizedFolderPath.StartsWith(desFolder))
                    containingProfileNameList.Add(profileName);
            }
            return containingProfileNameList;
        }

        /// <summary>
        /// Load the name of profile containing 2 specific paths
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <returns></returns>
        public static string LoadContainingProfileName(string srcFolder, string desFolder)
        {
            List<string> profileNameList = ProfileHolder.GetInstance().LoadProfileNameList();

            foreach (string profileName in profileNameList)
            {
                Profile profile = LoadProfile(profileName);
                string parent_src = profile.SrcFolder;
                string parent_des = profile.DesFolder;
                if ((srcFolder.Contains(parent_src) && desFolder.Contains(parent_des) && srcFolder.Replace(parent_src, "") == desFolder.Replace(parent_des, "")) || // src_des is child
                    (srcFolder.Contains(parent_des) && desFolder.Contains(parent_src) && srcFolder.Replace(parent_des, "") == desFolder.Replace(parent_src, "")))
                    return profileName;
            }

            return null;
        }

        /// <summary>
        /// Load the list of profiles conflicting with the src and des specified
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        /// <returns></returns>
        public static List<string> LoadConflictedProfileNameList(string srcFolder, string desFolder)
        {
            string src = srcFolder.ToLower();
            string des = desFolder.ToLower();
            List<string> profileNameListToUse = new List<string>();
            List<string> profileNameList = ProfileHolder.GetInstance().LoadProfileNameList();

            foreach (string profileName in profileNameList)
            {
                Profile profile = ProfileHolder.GetInstance().LoadProfile(profileName);
                
                string profileSrcFolder = profile.SrcFolder.ToLower();
                string profileDesFolder = profile.DesFolder.ToLower();

                if ((src.Contains(profileSrcFolder) && des.Contains(profileDesFolder) && src.Replace(profileSrcFolder, "") == des.Replace(profileDesFolder, "")) || // src and des is child
                    (src.Contains(profileDesFolder) && des.Contains(profileSrcFolder) && src.Replace(profileDesFolder, "") == des.Replace(profileSrcFolder, "")) || // src and des is child
                    (profileSrcFolder.Contains(src) && profileDesFolder.Contains(des) && profileSrcFolder.Replace(src, "") == profileDesFolder.Replace(des, "")) || // src and des is parent
                    (profileSrcFolder.Contains(des) && profileDesFolder.Contains(src) && profileSrcFolder.Replace(des, "") == profileDesFolder.Replace(src, ""))) // src and des is parent
                    profileNameListToUse.Add(profileName);
            }
            return profileNameListToUse;
        }
 
        #endregion

        #region LOGGER-RELATED METHODS

        /// <summary>
        ///   Return the logger instance with specified name
        /// </summary>
        /// <param name = "loggerName">Name of the logger instance</param>
        /// <returns>The logger instance with specified name</returns>
        public static Logger LoadLogger(string loggerName)
        {
            return LoggerHolder.GetInstance().LoadLogger(loggerName);
        }

        /// <summary>
        ///   Save all logger instances' information to storage
        /// </summary>
        private static void SaveLogger()
        {
            LoggerHolder.GetInstance().FinalizeLogger();
        }

        #endregion

        #region METADATA-RELATED METHODS

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
            SettingHolder.GetInstance().FinalizeSetting();
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