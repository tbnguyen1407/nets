#region USING DIRECTIVES

using System.Windows.Forms;
using nets.storage;
using System.IO;
using System.Diagnostics;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Manages and ensures proper operation of sync job
    /// Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public static class JobQueueHandler
    {
        #region FIELD DECLARATION

        private const string jobQueuefolderPath = @"\JobQueue";
        private static readonly string mainFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\Main.dat";
        private static readonly string mainSyncFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\SyncInMain.dat";
        private static readonly string rightClickSyncFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\RightClickSync.dat";

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Initialize job queue
        /// </summary>
        public static void InitializeJobQueue()
        {
            int numRunningInstances = GetNumberOfRunningInstances();
            switch (numRunningInstances)
            {
                case 1:
                    DeleteMainSyncFile();
                    DeleteMainFile();
                    DeleteRightClickSyncFile();
                    break;
                case 2:
                    if (File.Exists(rightClickSyncFile) && File.Exists(mainFile))
                    {
                        PrompUserToRestartNETS();
                        Process.GetCurrentProcess().Kill();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check if a sync job is in progress
        /// </summary>
        /// <returns></returns>
        public static bool SyncInProgress()
        {
            return File.Exists(mainSyncFile) || File.Exists(rightClickSyncFile);
        }

        /// <summary>
        /// Check if main window is running 
        /// </summary>
        /// <returns></returns>
        public static bool MainIsRunning()
        {
            return File.Exists(mainFile);
        }

        /// <summary>
        /// Create the main file when main window starts
        /// </summary>
        public static void CreateMainFile()
        {
            FileSystemOperator.CheckAndCreateFile(mainFile);
        }

        /// <summary>
        /// Delete the main file
        /// </summary>
        public static void DeleteMainFile()
        {
            if (File.Exists(mainFile))
                File.Delete(mainFile);
        }

        /// <summary>
        /// Create main sync file when a sync job starts in main window
        /// </summary>
        public static void CreateMainSyncFile()
        {
            FileSystemOperator.CheckAndCreateFile(mainSyncFile);
        }

        /// <summary>
        /// Delete main sync file
        /// </summary>
        public static void DeleteMainSyncFile()
        {
            if (File.Exists(mainSyncFile))
                File.Delete(mainSyncFile);
        }

        /// <summary>
        /// Create sync file when a sync job starts from r-click
        /// </summary>
        public static void CreateRightClickSyncFile()
        {
            FileSystemOperator.CheckAndCreateFile(rightClickSyncFile);
        }

        /// <summary>
        /// Delete r-click sync file
        /// </summary>
        public static void DeleteRightClickSyncFile()
        {
            if (File.Exists(rightClickSyncFile))
                File.Delete(rightClickSyncFile);
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Get the number of running instances of a process
        /// </summary>
        /// <returns></returns>
        private static int GetNumberOfRunningInstances()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] instanceList = Process.GetProcessesByName(currentProcessName);
            return instanceList.Length;
        }

        /// <summary>
        /// Prompt user to restart program
        /// </summary>
        private static void PrompUserToRestartNETS()
        {
            MessageBox.Show("NETS was terminated incorrectly since the last time.\r\n" +
                            "Please close all NETS windows before starting NETS again.\r\n",
                            "NETS incorrectly terminated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }

        #endregion
    }
}
