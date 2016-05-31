using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace nets_wpf.Storage
{
    /// <summary>
    ///   Manages application settings
    /// </summary>
    public sealed class SettingHolder
    {
        private static readonly SettingHolder Instance = new SettingHolder();

        private static StreamWriter iniFileWriter;
        private static StreamReader iniFileReader;
        private readonly Hashtable settingTable = new Hashtable();
        private readonly string settingFilePath = StorageFacade.SettingFilePath;

        private SettingHolder()
        {
        }

        public static SettingHolder GetInstance()
        {
            return Instance;
        }

        public void InitializeSetting()
        {
            LoadToHashTable();
        }

        ///<summary>
        ///</summary>
        ///<param name = "property"></param>
        ///<param name = "value"></param>
        public void SaveSetting(string property, string value)
        {
            settingTable[property] = value;
            WriteBackToSettingFile();
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        /// <returns></returns>
        public bool LoadSetting(string property)
        {
            return settingTable[property].ToString() == "1";
        }

        /// <summary>
        /// </summary>
        private void LoadToHashTable()
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
        private void WriteBackToSettingFile()
        {
            File.Delete(settingFilePath);
            iniFileWriter = new StreamWriter(settingFilePath);

            ICollection properties = settingTable.Keys;
            foreach (string property in properties)
                iniFileWriter.WriteLine(property + "=" + settingTable[property]);

            iniFileWriter.Close();
        }

        /// <summary>
        ///   Check and create ini file
        /// </summary>
        private void CheckAndCreateSettingFile()
        {
            if (File.Exists(settingFilePath))
                return;

            iniFileWriter = new StreamWriter(settingFilePath);
            iniFileWriter.WriteLine("autocomplete=1");
            iniFileWriter.WriteLine("contextmenu=1");
            iniFileWriter.WriteLine("deletetorecyclebin=1");
            iniFileWriter.Close();
        }
    }
}