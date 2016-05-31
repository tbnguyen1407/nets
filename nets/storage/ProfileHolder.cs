#region USING DIRECTIVES

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nets.dataclass;
using nets.utility;

#endregion

namespace nets.storage
{
    /// <summary>
    /// Author: Tran Binh Nguyen
    /// </summary>
    public sealed class ProfileHolder
    {
        #region FIELD DECLARATION

        private static readonly ProfileHolder Instance = new ProfileHolder();
        private readonly Hashtable profileTable = new Hashtable();

        #endregion

        #region CONSTRUCTORS

        private ProfileHolder()
        {
        }

        public static ProfileHolder GetInstance()
        {
            return Instance;
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Load profile list from file to RAM at program start-up
        /// </summary>
        public void InitializeProfile()
        {
            profileTable.Clear();
            string profileStorageFile = StorageFacade.ProfileStorageFile;

            FileSystemOperator.CheckAndCreateFile(profileStorageFile);

            List<string> profileListString = StorageOperator.Load(profileStorageFile);
            foreach (string profileDataString in profileListString)
            {
                string[] profileData = profileDataString.Split('|');

                Profile profile = new Profile(profileData[0], profileData[1], profileData[2],
                                              (SyncMode) Enum.Parse(typeof (SyncMode), profileData[3]),
                                              profileData[4], profileData[5]);

                profileTable.Add(profile.ProfileName, profile);
            }
        }

        /// <summary>
        /// Write back profile list from RAM to file
        /// </summary>
        public void FinalizeProfile()
        {
            string ProfileFilePath = StorageFacade.ProfileStorageFile;

            File.Delete(ProfileFilePath);
            FileSystemOperator.CheckAndCreateFile(ProfileFilePath);

            ICollection keys = profileTable.Keys;
            foreach (string key in keys)
            {
                Profile profile = (Profile) profileTable[key];
                StorageOperator.Add(profile.ToString(), ProfileFilePath);
            }
        }

        /// <summary>
        /// Save a profile
        /// </summary>
        /// <param name="profile"></param>
        public void SaveProfile(Profile profile)
        {
            if (profileTable.ContainsKey(profile.ProfileName))
            {
                string message = "Profile name " + profile.ProfileName + 
                                 " already existed. ProfileHolder.SaveProfile() exception";
                throw new Exception(message);
            }
            profileTable.Add(profile.ProfileName, profile);
        }

        /// <summary>
        /// Load profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public Profile LoadProfile(string profileName)
        {
            if (!profileTable.ContainsKey(profileName))
            {
                string message = "Profile name " + profileName + 
                                 " not existed. ProfileHolder.LoadProfile() exception";
                throw new Exception(message);
            }
            return (Profile) profileTable[profileName];
        }

        /// <summary>
        /// Delete profile
        /// </summary>
        /// <param name="profileName"></param>
        public void DeleteProfile(string profileName)
        {
            if (!profileTable.ContainsKey(profileName))
            {
                string message = "Profile name " + profileName + 
                                 " not existed. ProfileHolder.DeleteProfile() exception";
                throw new Exception(message);
            }
            profileTable.Remove(profileName);
        }

        /// <summary>
        /// Load name list of stored profiles
        /// </summary>
        /// <returns></returns>
        public List<string> LoadProfileNameList()
        {
            return profileTable.Keys.Cast<string>().ToList();
        }

        #endregion
    }
}