using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using nets_wpf.DataStructures;
using nets_wpf.Utility;

namespace nets_wpf.Storage
{
    public sealed class ProfileHolder
    {
        private static readonly ProfileHolder Instance = new ProfileHolder();
        private readonly Hashtable profileTable = new Hashtable();

        private ProfileHolder()
        {
        }

        public static ProfileHolder GetInstance()
        {
            return Instance;
        }

        public void InitializeProfile()
        {
            profileTable.Clear();
            string profileStorageFile = StorageFacade.ProfileStorageFile;

            FileSystemOperator.CheckAndCreateFile(profileStorageFile);

            List<string> profileListString = StorageOperator.Load(profileStorageFile);
            foreach (string profileDataString in profileListString)
            {
                string[] profileData = profileDataString.Split('|');

                /*
                Profile profile = new Profile(profileData[0], profileData[1], profileData[2],
                                   (SyncMode)Enum.Parse(typeof(SyncMode), profileData[3]),
                                   new FilterPattern(profileData[4], profileData[5]));*/

                Profile profile = new Profile(profileData[0], profileData[1], profileData[2],
                                              (SyncMode)Enum.Parse(typeof(SyncMode), profileData[3]),
                                              profileData[4], profileData[5]);

                SaveProfile(profile);
            }
        }

        private void FinalizeProfile()
        {
            string ProfileFilePath = StorageFacade.ProfileStorageFile;

            File.Delete(ProfileFilePath);
            FileSystemOperator.CheckAndCreateFile(ProfileFilePath);

            ICollection keys = profileTable.Keys;
            foreach (string key in keys)
            {
                Profile profile = (Profile)profileTable[key];
                StorageOperator.Add(profile.ToString(), ProfileFilePath);
            }
        }

        public void SaveProfile(Profile newProfile)
        {
            if (profileTable.ContainsKey(newProfile.ProfileName))
                throw new Exception("Create currentProfile that name already existed! Logic fails! Contact Yen Duong (girl) @ yenduong@comp.nus.edu.sg for more info.");

            List<string> profileNameList = LoadProfileNameListToUse(newProfile.SrcFolder, newProfile.DesFolder);

            // Find and initialize metadata storage file
            string metadataFilePath = StorageFacade.MetaDataFolder + newProfile.ProfileName + ".dat";
            FileSystemOperator.CheckAndCreateFile(metadataFilePath);

            // Copy metadata
            StreamWriter sw = File.AppendText(metadataFilePath);

            foreach (string profileName in profileNameList)
            {
                // Load from old metadata
                List<string> metadata = StorageOperator.Load(
                    StorageFacade.MetaDataFolder + profileName + ".dat");
                foreach (string metaDataPair in metadata)
                {
                    sw.WriteLine(metaDataPair);
                }
                DeleteProfile(profileName);
            }
            sw.Close();

            // Add currentProfile
            profileTable.Add(newProfile.ProfileName, newProfile);
            FinalizeProfile();
        }

        public Profile LoadProfile(string profileName)
        {
            return (Profile) profileTable[profileName];
        }

        public void DeleteProfile(string profileName)
        {
            profileTable.Remove(profileName);
            string metaDataFilePath = StorageFacade.MetaDataFolder + profileName + ".dat";
            if (File.Exists(metaDataFilePath))
                File.Delete(metaDataFilePath);
            FinalizeProfile();
        }

        public List<string> LoadProfileNameList()
        {
            List<string> profileNameList = new List<string>();
            foreach (string profileName in profileTable.Keys)
                profileNameList.Add(profileName);
            return profileNameList;
        }

        public List<string> LoadContainingProfileNameList(string folderPath)
        {
            List<string> containingProfileNameList = new List<string>();
            string normalizedFolderPath = folderPath.ToLower();
            if (!normalizedFolderPath.EndsWith(@"\"))
                normalizedFolderPath += @"\";

            foreach (string profileName in profileTable.Keys)
            {
                Profile profile = LoadProfile(profileName);
                string srcFolder = profile.SrcFolder.ToLower();
                string desFolder = profile.DesFolder.ToLower();
                if (normalizedFolderPath.StartsWith(srcFolder) || normalizedFolderPath.StartsWith(desFolder))
                    containingProfileNameList.Add(profileName);
            }
            return containingProfileNameList;
        }

        public List<string> LoadProfileNameListToUse(string srcFolder, string desFolder)
        {
            string src = srcFolder.ToLower();
            string des = desFolder.ToLower();
            List<string> profileNameListToUse = new List<string>();
            List<string> profileNameList = LoadProfileNameList();

            foreach (string profileName in profileNameList)
            {
                Profile profile = (Profile) profileTable[profileName];
                string profileSrcFolder = profile.SrcFolder.ToLower();
                string profileDesFolder = profile.DesFolder.ToLower();

                if ((src.Contains(profileSrcFolder) && des.Contains(profileDesFolder) && src.Replace(profileSrcFolder, "") == des.Replace(profileDesFolder, "")) || // src and des is child
                    (src.Contains(profileDesFolder) && des.Contains(profileSrcFolder) && src.Replace(profileDesFolder, "") == des.Replace(profileSrcFolder, "")) || // src and des is child
                    (profileSrcFolder.Contains(src) && profileDesFolder.Contains(des) && profileSrcFolder.Replace(src, "") == profileDesFolder.Replace(des, "")) || // src and des is parent
                    (profileSrcFolder.Contains(des) && profileDesFolder.Contains(src) && profileSrcFolder.Replace(des, "") == profileDesFolder.Replace(src, "")))   // src and des is parent
                    
                    profileNameListToUse.Add(profileName);
            }
            return profileNameListToUse;
        }

        public string LoadContainingProfileName(string srcFolder, string desFolder)
        {
            List<string> profileNameList = LoadProfileNameList();
            
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
    }
}
