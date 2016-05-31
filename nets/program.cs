using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using nets.dataclass;
using nets.gui;
using nets.logic;
using nets.utility;

namespace nets
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Initialize();
            RunProcess();
        }

        #region PRIVATE HELPERS

        private static void Initialize()
        {
            JobQueueHandler.InitializeJobQueue();
            Application.EnableVisualStyles();
            RegisterObservers();
            LogicFacade.InitializeStorage();
        }

        /// <summary>
        ///  Register observers for the Logic component 
        /// </summary>
        private static void RegisterObservers()
        {
            LogicFacade.NoProfileEvent += GUIEventHandler.NoProfileEventHandler;
            LogicFacade.MultipleProfilesEvent += GUIEventHandler.MultipleProfileEventHandler;
            LogicFacade.ReportSyncErrorEvent += GUIEventHandler.ReportSyncException;
            Reconciler.UpdateProgressEvent += GUIEventHandler.UpdateProgressEventHandler;
            Reconciler.UpdateActionEvent += GUIEventHandler.UpdateActionEventHandler;
            Reconciler.UpdateButtonsEvent += GUIEventHandler.UpdateButtonsEventHandler;
            Reconciler.UpdateSyncJobListEvent += GUIEventHandler.UpdateSyncJobListEventHandler;
            Reconciler.SetAbortButtonEvent += GUIEventHandler.SetAbortButtonEventHandler;
        }

        private static void RunProcess()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                GUIEventHandler.syncRunningMode = RunningMode.MainApplication;
                GUIEventHandler.RunMainWindow();
            }
            else
            {
                if (JobQueueHandler.SyncInProgress())
                {
                    GUIEventHandler.ShowCannotSyncMessage();
                    return;
                }
                JobQueueHandler.CreateRightClickSyncFile();

                string[] subArgs = args[1].Split(new[] { '|' });
                switch (subArgs[0])
                {
                    case "--SmartSync":
                        List<string> folderPathList = subArgs.ToList<string>();
                        folderPathList.RemoveRange(0, 1);
                        GUIEventHandler.syncRunningMode = RunningMode.ContextMenuSmartSync;
                        GUIEventHandler.SyncHandler(ref folderPathList);
                        break;
                    case "--SyncWith":
                        GUIEventHandler.syncRunningMode = RunningMode.ContextMenuSyncWith;
                        GUIEventHandler.SyncWithEventHandler(subArgs[1]);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
