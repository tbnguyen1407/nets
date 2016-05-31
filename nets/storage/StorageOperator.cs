#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.IO;
using nets.utility;

#endregion

namespace nets.storage
{
    /// <summary>
    ///   This is the only class that provides access to storage file operations: Add, Remove, Load
    ///   Author: Nguyen Hoang Hai + Tran Binh Nguyen
    /// </summary>
    public static class StorageOperator
    {
        #region MAIN METHODS

        /// <summary>
        ///   Add a new storage entry into the storage file at fileLocation
        /// </summary>
        /// <param name = "objectData">Content of the storage entry</param>
        /// <param name = "fileLocation">Location of the storage file</param>
        public static void Add(string objectData, string fileLocation)
        {
            try
            {
                StreamWriter sw = File.AppendText(fileLocation);
                sw.WriteLine(objectData);
                sw.Close();
            }
            catch (Exception)
            {
                string message = fileLocation + " not found at StorageOperator.Add()";
                throw new Exception(message);
            }
        }

        /// <summary>
        ///   Load the whole storage file at fileLocation
        /// </summary>
        /// <param name = "fileLocation">Location of the storage file</param>
        /// <returns>A list of strings of storage entries</returns>
        public static List<string> Load(string fileLocation)
        {
            List<string> objectDataList = new List<string>();

            try
            {
                StreamReader sr = new StreamReader(fileLocation);
                string line;

                while ((line = sr.ReadLine()) != null)
                    objectDataList.Add(line);
                sr.Close();

            }
            catch (Exception)
            {
                string message = fileLocation + " not found at StorageOperator.Load()";
                throw new Exception(message);
            }

            return objectDataList;
        }

        #endregion
    }
}