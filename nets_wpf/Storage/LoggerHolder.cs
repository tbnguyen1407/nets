using System.Collections.Generic;
using System.Linq;
using System.IO;
using nets_wpf.DataStructures;
using nets_wpf.Utility;

namespace nets_wpf.Storage
{
    /// <summary>
    ///   Manage Logger objects, reduce burden on StorageOperator
    /// </summary>
    public class LoggerHolder
    {
        private static readonly LoggerHolder Instance = new LoggerHolder();
        private readonly List<Logger> LoggerList;

        private LoggerHolder()
        {
            LoggerList = new List<Logger>();
            InitilizeLogs();
        }

        /// <summary>
        ///   Implement singleton
        /// </summary>
        /// <returns></returns>
        public static LoggerHolder GetInstance()
        {
            return Instance;
        }

        /// <summary>
        ///   Get the logger of specified name. If name doesnot exist then create a new logger with that name.
        /// </summary>
        /// <param name = "loggerName"></param>
        /// <returns>The logger of specified name</returns>
        public Logger GetLogger(string loggerName)
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
        public void SaveLogger()
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

        private static void InitilizeLogs()
        {
            string logFolderPath = StorageFacade.LogFolder;
            if (!Directory.Exists(logFolderPath))
                return;
            Directory.Delete(logFolderPath, true);
            Directory.CreateDirectory(logFolderPath);
        }

        /// <summary>
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
        /*
        public void Report(string loggerName, LogType logType, string logMsg, Exception e)
        {
            if (loggerName.Equals("reconciler progress logger"))
            {
                float percent = float.Parse(logMsg);
                GUIEventHandler.SyncProgressUpdateStatus(float.Parse(logMsg));
            }
            else if (loggerName.Equals("reconciler action logger"))
            {
                GUIEventHandler.SyncProgressUpdateStatus(logMsg);
            }
            else if (loggerName.Equals("reconciler sync folders logger"))
            {
                GUIEventHandler.SyncFoldersUpdateStatus(logMsg);
            }
            else if (loggerName.Equals("reconciler completed job logger"))
            {
                GUIEventHandler.CompletedJobUpdateTextBox(logMsg);
            }
        }*/
    }
}