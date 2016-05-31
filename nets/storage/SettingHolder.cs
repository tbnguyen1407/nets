#region USING DIRECTIVES

using System.Collections;
using System.Collections.Generic;
using System.IO;
using nets.utility;

#endregion

namespace nets.storage
{
    /// <summary>
    ///   Manages application settings
    ///   Author: Tran Binh Nguyen
    /// </summary>
    public sealed class SettingHolder
    {
        #region FIELD DECLARATION

        private static readonly SettingHolder Instance = new SettingHolder();

        private static StreamWriter iniFileWriter;
        private static StreamReader iniFileReader;
        private readonly string settingFilePath = StorageFacade.SettingFilePath;
        private readonly Hashtable settingTable = new Hashtable();

        #endregion

        #region CONSTRUCTORS

        private SettingHolder()
        {
        }

        public static SettingHolder GetInstance()
        {
            return Instance;
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Load settings from file to RAM at program startup
        /// </summary>
        public void InitializeSetting()
        {
            CheckAndCreateSettingFile();
            settingTable.Clear();

            iniFileReader = new StreamReader(settingFilePath);

            List<string> lines = new List<string>();
            while (!iniFileReader.EndOfStream)
                lines.Add(iniFileReader.ReadLine());

            foreach (string line in lines)
                settingTable.Add(line.Split('=')[0].Trim(), line.Split('=')[1].Trim());

            iniFileReader.Close();
        }

        /// <summary>
        ///   Save settings to ini file
        /// </summary>
        public void FinalizeSetting()
        {
            File.Delete(settingFilePath);
            iniFileWriter = new StreamWriter(settingFilePath);

            ICollection properties = settingTable.Keys;
            foreach (string property in properties)
                iniFileWriter.WriteLine(property + "=" + settingTable[property]);

            iniFileWriter.Close();
        }

        ///<summary>
        ///</summary>
        ///<param name = "property"></param>
        ///<param name = "value"></param>
        public void SaveSetting(string property, string value)
        {
            settingTable[property] = value;
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        /// <returns></returns>
        public bool LoadSetting(string property)
        {
            return settingTable[property].ToString() == "1";
        }

        #endregion

        #region HELPERS

        /// <summary>
        ///   Check and create settings.ini file
        /// </summary>
        private void CheckAndCreateSettingFile()
        {
            if (File.Exists(settingFilePath))
                return;
            
            FileSystemOperator.CheckAndCreateFile(settingFilePath);
            StorageOperator.Add("autocomplete=1", settingFilePath);
            StorageOperator.Add("contextmenu=1", settingFilePath);
            StorageOperator.Add("deletetorecyclebin=1", settingFilePath);
        }

        #endregion
    }
}