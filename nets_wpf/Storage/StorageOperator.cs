using System.Collections.Generic;
using System.IO;

namespace nets_wpf.Storage
{
    /// <summary>
    ///   This is the only class that provides access to storage file operations: Add, Remove, Load
    /// </summary>
    public static class StorageOperator
    {
        /// <summary>
        ///   Add a new storage entry into the storage file at fileLocation
        /// </summary>
        /// <param name = "objectData">Content of the storage entry</param>
        /// <param name = "fileLocation">Location of the storage file</param>
        public static void Add(string objectData, string fileLocation)
        {
            StreamWriter sw = File.AppendText(fileLocation);
            sw.WriteLine(objectData);
            sw.Close();
        }

        /// <summary>
        ///   Load the whole storage file at fileLocation
        /// </summary>
        /// <param name = "fileLocation">Location of the storage file</param>
        /// <returns>A list of strings of storage entries</returns>
        public static List<string> Load(string fileLocation)
        {
            List<string> objectDataList = new List<string>();
            StreamReader sr = new StreamReader(fileLocation);
            string line;

            while ((line = sr.ReadLine()) != null)
                objectDataList.Add(line);
            sr.Close();

            return objectDataList;
        }
    }
}