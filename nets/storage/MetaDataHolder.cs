using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using nets.utility;

namespace nets.storage
{
    /// <summary>
    ///   Manages MetaData information
    ///   Author: Nguyen Hoang Hai + Tran Binh Nguyen
    /// </summary>
    public sealed class MetaDataHolder
    {
        #region FIELD DECLARATION

        private static readonly MetaDataHolder Instance = new MetaDataHolder();
        private Hashtable metaDataTable = new Hashtable();

        #endregion

        #region CONSTRUCTORS

        private MetaDataHolder()
        {
        }

        public static MetaDataHolder GetInstance()
        {
            return Instance;
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Load metadata from file to hashtable
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="desFolder"></param>
        public void InitializeMetaData(string srcFolder, string desFolder)
        {
            // Clear the table
            metaDataTable.Clear();

            // Identify which currentProfile to use metadata
            string profileName = StorageFacade.LoadContainingProfileName(srcFolder, desFolder);
            
            if (profileName == null)
                return;
            
            string metaDataFilePath = StorageFacade.MetaDataFolder + profileName + ".dat";

            // Load metatadata files to table
            FileSystemOperator.CheckAndCreateFile(metaDataFilePath);
            List<string> metaDataList = StorageOperator.Load(metaDataFilePath);
            foreach (string metaDataPair in metaDataList)
                SaveMetaData(metaDataPair.Split('|')[0], metaDataPair.Split('|')[1]);
        }

        /// <summary>
        ///   Write back metadata from hashtable to file
        /// </summary>
        /// <param name = "srcFolder"></param>
        /// <param name = "desFolder"></param>
        public void FinalizeMetaData(string srcFolder, string desFolder)
        {
            // Identify which currentProfile had been used
            string profileName = StorageFacade.LoadContainingProfileName(srcFolder, desFolder);
            if (profileName == null)
                return;
            string metaDataFilePath = StorageFacade.MetaDataFolder + profileName + ".dat";

            // Write back metadata from table to file)
            File.Delete(metaDataFilePath);
            FileSystemOperator.CheckAndCreateFile(metaDataFilePath);

            ICollection keys = metaDataTable.Keys;
            foreach (string key in keys)
            {
                string metaDataPair = key + "|" + LoadMetaData(key);
                StorageOperator.Add(metaDataPair, metaDataFilePath);
            }
        }

        /// <summary>
        ///   Save metadata of a filesystem path to hashtable
        /// </summary>
        /// <param name = "filesystemPath"></param>
        /// <param name = "metaData"></param>
        public void SaveMetaData(string filesystemPath, string metaData)
        {
            if (String.IsNullOrEmpty(metaData) || String.IsNullOrEmpty(filesystemPath))
                return;

            if (metaDataTable.ContainsKey(filesystemPath))
                metaDataTable.Remove(filesystemPath);

            metaDataTable.Add(filesystemPath, metaData);
        }

        /// <summary>
        ///   Load metadata of a filesystem path from hashtable
        /// </summary>
        /// <param name = "filesystemPath"></param>
        public string LoadMetaData(string filesystemPath)
        {
            return (string) metaDataTable[filesystemPath];
        }

        /// <summary>
        ///   Delete metadata of a filesystem from hashtable
        /// </summary>
        /// <param name = "filesystemPath"></param>
        public void DeleteMetaData(string filesystemPath)
        {
            ICollection keyList = metaDataTable.Keys;
            Hashtable tempTable = metaDataTable.Clone() as Hashtable;

            foreach (string key in keyList)
                if (key.StartsWith(filesystemPath))
                    tempTable.Remove(key);

            metaDataTable = tempTable;
        }

        #endregion
    }
}