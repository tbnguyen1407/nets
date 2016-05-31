using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using nets_wpf.Storage;

namespace nets_wpf.Utility
{
    public static class JobQueueHandler
    {
        private const string jobQueuefolderPath = @"\JobQueue";
        private static readonly string mainFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\Main.dat";
        private static readonly string mainSyncFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\SyncInMain.dat";
        private static readonly string rightClickSyncFile = StorageFacade.AppDataFolder + jobQueuefolderPath + @"\RightClickSync.dat";

        public static bool SyncInProgress()
        {
            return File.Exists(mainSyncFile) || File.Exists(rightClickSyncFile);
        }

        public static bool MainIsRunning()
        {
            return File.Exists(mainFile);
        }

        public static void CreateMainFile()
        {
            FileSystemOperator.CheckAndCreateFile(mainFile);
        }

        public static void DeleteMainFile()
        {
            if (File.Exists(mainFile))
                File.Delete(mainFile);
        }

        public static void CreateMainSyncFile()
        {
            FileSystemOperator.CheckAndCreateFile(mainSyncFile);
        }

        public static void DeleteMainSyncFile()
        {
            if (File.Exists(mainSyncFile))
                File.Delete(mainSyncFile);
        }

        public static void CreateRightClickSyncFile()
        {
            FileSystemOperator.CheckAndCreateFile(rightClickSyncFile);
        }

        public static void DeleteRightClickSyncFile()
        {
            if (File.Exists(rightClickSyncFile))
                File.Delete(rightClickSyncFile);
        }

        private static int GetNumberOfRunningInstances()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] instanceList = Process.GetProcessesByName(currentProcessName);
            return instanceList.Length;
        }

        public static void InitializeJobQueue()
        {
            int numRunningInstances = JobQueueHandler.GetNumberOfRunningInstances();
            switch (numRunningInstances)
            {
                case 1:
                    JobQueueHandler.DeleteMainSyncFile();
                    JobQueueHandler.DeleteMainFile();
                    JobQueueHandler.DeleteRightClickSyncFile();
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

        private static void PrompUserToRestartNETS()
        {
            MessageBox.Show("NETS was terminated incorrectly the last time.\r\n" +
                            "Please close all NETS windows before starting NETS again.\r\n" +
                            "Thank you!",
                            "NETS incorrectly terminated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}
