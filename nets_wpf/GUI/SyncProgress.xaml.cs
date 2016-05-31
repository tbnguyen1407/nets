using System;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using nets_wpf.DataStructures;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for SyncProgress.xaml
    /// </summary>

    public delegate void ProgressBarInvoker(float value);
    public delegate void LabelInvoker(string str);
    public delegate void BtnTextInvoker(string str);
    public delegate void dgvUpdateJobListInvoker(int rowIndex, string newStatus);
    public delegate void ChangeButtonsInvoker();

    public partial class SyncProgress : Window
    {
       
        private bool SyncEnabler = true;
        private System.Data.DataTable db_profileListSync = new System.Data.DataTable();

            public SyncProgress()
            {
                AddColumn();
                InitializeComponent();
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                SyncEnabler = true;
            }

            public SyncProgress(Profile[] jobList)
                : this()
            {
                PopulateJobList(jobList);
            }

            public SyncProgress(string srcFolder, string desFolder)
                : this()
            {
                string profileInfo = "\nSrc: " + PathOperator.TrimPath(srcFolder, 35) + "\n" +
                                     "Des: " + PathOperator.TrimPath(desFolder, 35) +"\n";
                System.Data.DataRow newRow = this.db_profileListSync.NewRow();
                newRow["ProfileInfo"] = profileInfo;
                newRow["SyncStatus"] = "\n  Pending";
                db_profileListSync.Rows.Add(newRow);
                //dgv_SyncJobList.Rows.Add(profileInfo, "Pending");               
            }

            private void AddColumn()
            {
                db_profileListSync.Columns.Add(new System.Data.DataColumn("ProfileInfo", typeof(string)));
                db_profileListSync.Columns.Add(new System.Data.DataColumn("SyncStatus", typeof(string)));
            }

            public void SetLabelText(string content)
            {
                if (!(Dispatcher.Thread == Thread.CurrentThread))
                {
                    LabelInvoker lInvoker =
                        new LabelInvoker(SetLabelText_Invoked);
                    Dispatcher.Invoke(lInvoker, content);
                }
                else
                    SetLabelText_Invoked(content);
            }

            public void SetLabelText_Invoked(string content)
            {
                this.lb_SyncStatus.Text = content;
            }

            public void SetProgressBarValue(float value)
            {
                if (!(Dispatcher.Thread == Thread.CurrentThread))
                {
                    ProgressBarInvoker pbInvoker = new ProgressBarInvoker(SetProgressBarValue_Invoked);
                    Dispatcher.Invoke(pbInvoker, value);
                }
                else
                    SetProgressBarValue_Invoked(value);
            }

            public void SetProgressBarValue_Invoked(float value)
            {
                this.SpProgressBar.Value = (int)(value * 100);
                this.lb_ProgressBarStatus.Content =  ((int)(value * 100)) +"%";
            }

            public void SetBtnText(string str)
            {
                if (!(Dispatcher.Thread == Thread.CurrentThread))
                {
                    BtnTextInvoker btInvoker = new BtnTextInvoker(SetBtnText_Invoked);
                    Dispatcher.Invoke(btInvoker, str);
                }
                else
                    SetBtnText_Invoked(str);
            }

            private void SetBtnText_Invoked(string str)
            {
                this.btn_PauseSync.Content = str;
            }

            public void PopulateJobList(Profile[] jobList)
            {
                foreach (Profile job in jobList)
                {
                    Profile profile = job;
                    string profileInfo = "\nSrc: " + PathOperator.TrimPath(profile.SrcFolder, 40) + "\n" +
                                         "Des: " + PathOperator.TrimPath(profile.DesFolder, 40 ) +"\n";
                    System.Data.DataRow newRow = this.db_profileListSync.NewRow();
                    newRow["ProfileInfo"] = profileInfo;
                    newRow["SyncStatus"] = "\n    Pending";
                    db_profileListSync.Rows.Add(newRow);
                }
            }

            public void UpdateJobList(int rowIndex, string newStatus)
            {
                if (!(Dispatcher.Thread == Thread.CurrentThread))
                {
                    dgvUpdateJobListInvoker invoker = new dgvUpdateJobListInvoker(UpdateJobList_Invoked);
                    Dispatcher.Invoke(invoker, rowIndex, newStatus);
                    //Invoke(invoker, new object[] { rowIndex, newStatus });
                }
                else
                    UpdateJobList_Invoked(rowIndex, newStatus);
            }

            public void UpdateJobList_Invoked(int rowIndex, string newStatus)
            {
                
                System.Data.DataRow newRow = db_profileListSync.Rows[rowIndex];
                if (newRow == null)
                    return;
                newRow["SyncStatus"] = "\n  " + newStatus;
                dgv_SyncJobList.SelectedIndex = rowIndex;           
            }

            private void btn_PauseContinue_Click(object sender, RoutedEventArgs e)
            {
                if (this.btn_PauseSync.Content.Equals("Pause"))
                {
                    this.SyncEnabler = false;
                    this.btn_PauseSync.Content = "Continue";
                    return;
                }

                if (this.btn_PauseSync.Content.Equals("Continue"))
                {
                    this.SyncEnabler = true;
                    this.btn_PauseSync.Content = "Pause";
                    return;
                }
            }

            private void btn_Abort_Click(object sender, RoutedEventArgs e)
            {
                SetLabelText("Aborting...Please wait...");
                btn_PauseContinue_Click(sender, e);
                PopUpMessageBox messageBox = new PopUpMessageBox("Aborting the sync job now may cause undeterministic results\r\n" +
                                                            "Are you sure you want to abort?",
                                                            "Abort sync job");
                if ( messageBox.ShowDialog() == false)
                    btn_PauseContinue_Click(sender, e);
                else
                    GUIEventHandler.AbortSyncProgress();
            }

            public bool GetSyncEnabler()
            {
                return this.SyncEnabler;
            }

            private void btn_Close_Click(object sender, RoutedEventArgs e)
            {
                GUIEventHandler.CloseSyncProgress();
            }

            public void HidePauseAndAbort()
            {
                btn_PauseSync.Visibility = Visibility.Hidden;
                btn_AbortSync.Visibility = Visibility.Hidden;
            }

            public void DisplayCloseAndViewLog()
            {
                btn_Close.Visibility = Visibility.Visible;
                linklb_ViewLog.Visibility = Visibility.Visible;
            }

            //private void linklb_ViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            //{
            //    string logPath = "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\nets\logs\reconciler action logger.dat" + "\"";
            //    System.Diagnostics.Process.Start("wordpad.exe", logPath);
            //}

            public void ChangeAppearanceAfterFinishing()
            {
                if (!(Dispatcher.Thread == Thread.CurrentThread))
                {
                    ChangeButtonsInvoker changeButtonsInvoker = new ChangeButtonsInvoker(ChangeAppearanceAfterFinishing_Invoked);
                    Dispatcher.Invoke(changeButtonsInvoker);
                }
                else ChangeAppearanceAfterFinishing_Invoked();
            }

            public void ChangeAppearanceAfterFinishing_Invoked()
            {
                SetProgressBarValue((float)1.0);
                HidePauseAndAbort();
                DisplayCloseAndViewLog();
                SetLabelText("SYNC COMPLETED!");
                dgv_SyncJobList.SelectedIndex = -1;
                //dgv_SyncJobList.ClearSelection();
            }

            public System.Data.DataTable SyncResult
            {
                get { return db_profileListSync; }
            }

            void NonRectangularWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                DragMove();
            }

            private void control_SyncProcess_Closed(object sender, EventArgs e)
            {
                JobQueueHandler.DeleteMainSyncFile();
                JobQueueHandler.DeleteRightClickSyncFile();
            }

            private void linklb_ViewLog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                GUIEventHandler.ShowLog();
            }
        }
    }
