#region USING DIRECTIVES

using System.Collections.Generic;
using System.IO;
using System.Linq;
using nets.dataclass;
using nets.utility;

#endregion

namespace nets.storage
{
    /// <summary>
    ///   Manage Logger objects, reduce burden on StorageOperator
    ///   Author: Nguyen Hoang Hai
    /// </summary>
    public class LoggerHolder
    {
        #region FIELD DECLARATION

        private static readonly LoggerHolder Instance = new LoggerHolder();
        private readonly List<Logger> LoggerList;

        #endregion

        #region CONSTRUCTORS

        private LoggerHolder()
        {
            LoggerList = new List<Logger>();
        }

        /// <summary>
        ///   Implement singleton
        /// </summary>
        /// <returns></returns>
        public static LoggerHolder GetInstance()
        {
            return Instance;
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        ///   Get the logger of specified name. If name doesnot exist then create a new logger with that name.
        /// </summary>
        /// <param name = "loggerName"></param>
        /// <returns>The logger of specified name</returns>
        public Logger LoadLogger(string loggerName)
        {
            int i = GetLoggerIndex(loggerName);
            if (i == -1)
            {
                Logger logger = new Logger(loggerName);
                LoggerList.Add(logger);
                return logger;
            }
            return LoggerList.ElementAt(i);
        }

        /// <summary>
        ///   Save all loggers in the LoggerList back to storage
        /// </summary>
        public void FinalizeLogger()
        {
            for (int i = 0; i < LoggerList.Count; i++)
            {
                Logger logger = LoggerList.ElementAt(i);
                string info = logger.GetInfo();
                string logpath = StorageFacade.LogFolder + logger.GetName() + ".dat";
                FileSystemOperator.CheckAndCreateFile(logpath);
                StorageOperator.Add(info, logpath);
            }
        }

        /// <summary>
        /// Initialize logs
        /// </summary>
        public void InitilizeLogger()
        {
            string logFolderPath = StorageFacade.LogFolder;
            if (!Directory.Exists(logFolderPath))
                return;
            Directory.Delete(logFolderPath, true);
            Directory.CreateDirectory(logFolderPath);
        }

        /// <summary>
        /// Get index of a logger in the logger list
        /// </summary>
        /// <param name = "loggerName"></param>
        /// <returns></returns>
        public int GetLoggerIndex(string loggerName)
        {
            int i = 0;
            bool valid = false;
            while (!valid && i < LoggerList.Count)
            {
                Logger logger = LoggerList.ElementAt(i);
                if (logger.GetName().Equals(loggerName))
                    valid = true;
                else
                    i++;
            }
            return valid ? i : -1;
        }

        #endregion
    }
}