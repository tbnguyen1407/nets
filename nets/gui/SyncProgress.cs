using System;
using System.Windows.Forms;
using nets.dataclass;
using nets.utility;

namespace nets.gui
{
    #region DELEGATES
        
    public delegate void ProgressBarInvoker(float value);
    public delegate void LabelInvoker(string str);
    public delegate void dgvUpdateJobListInvoker(int rowIndex, string newStatus);
    public delegate void ChangeButtonsInvoker();
    public delegate void SetAbortButtonInvoker(bool enabled);

    #endregion

    /// <summary>
    /// Form to display progress bar and sync job infos
    /// Author: Hoang Nguyen Nhat Tao + Nguyen Hoang Hai
    /// </summary>
    public partial class SyncProgress : Form
    {
        #region PRIVATE CONSTRUCTORS

        private SyncProgress()
        {
            switch (GUIEventHandler.syncRunningMode)
            {
                case RunningMode.MainApplication:
                    JobQueueHandler.CreateMainSyncFile();
                    break;
                default:
                    JobQueueHandler.CreateRightClickSyncFile();
                    break;
            }

            InitializeComponent();
            FormButtonDisabler.DisableCloseButton(this.Handle.ToInt32());
        }

        #endregion

        #region PUBLIC CONSTRUCTORS
        
        /// <summary>
        /// Construct a sync progress window given a sync job list
        /// </summary>
        /// <param name="jobList">given job list</param>
        public SyncProgress(Profile[] jobList)
            : this()
        {
            PopulateJobList(jobList);
        }

        /// <summary>
        /// Construct a sync progress window given a sync job with source and destination folder
        /// </summary>
        /// <param name="srcFolder">source folder</param>
        /// <param name="desFolder">destination folder</param>
        public SyncProgress(string srcFolder, string desFolder)
            : this()
        {
            string profileInfo = "Src: " + PathOperator.TrimPath(srcFolder, 45) + "\n" +
                                 "Des: " + PathOperator.TrimPath(desFolder, 45);
            dgv_SyncJobList.Rows.Add(profileInfo, "Pending");
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Set the content of the label in sync progress window
        /// </summary>
        /// <param name="content">content to be set</param>
        public void SetLabelText(string content)
        {
            if (InvokeRequired)
            {
                LabelInvoker lInvoker = SetLabelText_Invoked;
                Invoke(lInvoker, content);
            }
            else
                SetLabelText_Invoked(content);
        }

        /// <summary>
        /// Set the percentage of the progress bar in sync progress window
        /// </summary>
        /// <param name="value">percentage to be set</param>
        public void SetProgressBarValue(float value)
        {
            if (InvokeRequired)
            {
                ProgressBarInvoker pbInvoker = SetProgressBarValue_Invoked;
                Invoke(pbInvoker, value);
            }
            else
                SetProgressBarValue_Invoked(value);
        }

        /// <summary>
        /// Set the Abort Button to either enabled or disabled
        /// </summary>
        /// <param name="enabled">enabled or disabled</param>
        public void SetAbortButton(bool enabled)
        {
            if (InvokeRequired)
            {
                SetAbortButtonInvoker invoker = SetAbortButton_Invoked;
                Invoke(invoker, enabled);
            }
            else SetAbortButton_Invoked(enabled);
        }

        /// <summary>
        /// Update the status of a job in job list given its row index and new status
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="newStatus">new status</param>
        public void UpdateJobList(int rowIndex, string newStatus)
        {
            if (InvokeRequired)
            {
                dgvUpdateJobListInvoker invoker = UpdateJobList_Invoked;
                Invoke(invoker, rowIndex, newStatus);
            }
            else
                UpdateJobList_Invoked(rowIndex, newStatus);
        }

        /// <summary>
        /// Hide Pause and Abort buttons and show the Close button
        /// </summary>
        public void ChangeAppearanceAfterFinishing()
        {
            if (InvokeRequired)
            {
                ChangeButtonsInvoker changeButtonsInvoker = ChangeAppearanceAfterFinishing_Invoked;
                Invoke(changeButtonsInvoker);
            }
            else ChangeAppearanceAfterFinishing_Invoked();
        }

        #endregion 

        #region PRIVATE HELPERS

        private void SetLabelText_Invoked(string content)
        {
            lb_SyncStatus.Text = content;
        }

        private void SetProgressBarValue_Invoked(float value)
        {
            SpProgressBar.Value = (int)(value * 100);
        }

        private void SetAbortButton_Invoked(bool enabled)
        {
            btn_AbortSync.Enabled = enabled;
        }

        private void PopulateJobList(Profile[] jobList)
        {
            foreach (Profile job in jobList)
            {
                Profile profile = job;
                string profileInfo = "Src: " + PathOperator.TrimPath(profile.SrcFolder, 45) + "\n" +
                                     "Des: " + PathOperator.TrimPath(profile.DesFolder, 45);
                dgv_SyncJobList.Rows.Add(profileInfo, "Pending");
            }
        }

        private void UpdateJobList_Invoked(int rowIndex, string newStatus)
        {
            dgv_SyncJobList.Rows[rowIndex].Cells[1].Value = newStatus;
            dgv_SyncJobList.ClearSelection();
            dgv_SyncJobList.Rows[rowIndex].Cells[1].Selected = true;
            dgv_SyncJobList.CurrentCell = dgv_SyncJobList.Rows[rowIndex].Cells[0];
        }

        private void btn_PauseContinue_Click(object sender, EventArgs e)
        {
            if (btn_PauseSync.Text.Equals("Pause"))
            {
                GUIEventHandler.PauseSyncProgress();
                btn_PauseSync.Text = "Continue";
                return;
            }

            if (!btn_PauseSync.Text.Equals("Continue"))
                return;
            GUIEventHandler.ContinueProgress();
            btn_PauseSync.Text = "Pause";
            return;
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            if (btn_PauseSync.Text.Equals("Pause"))
                btn_PauseContinue_Click(sender, e);

            DialogResult userResponse = MessageBox.Show("Aborting the sync job now may cause unwanted results\r\n" +
                                                        "Are you sure you want to abort?",
                                                        "Abort sync job",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
            if (userResponse == DialogResult.No)
                btn_PauseContinue_Click(sender, e);
            else
                GUIEventHandler.AbortSyncProgress();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HidePauseAndAbort()
        {
            btn_PauseSync.Hide();
            btn_AbortSync.Hide();
        }

        private void DisplayCloseAndViewLog()
        {
            btn_Close.Visible = true;
            linklb_ViewLog.Visible = true;
        }

        private void linklb_ViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GUIEventHandler.ShowLog();
        }

        private void ChangeAppearanceAfterFinishing_Invoked()
        {
            SetProgressBarValue((float)1.0);
            HidePauseAndAbort();
            DisplayCloseAndViewLog();
            SetLabelText("Sync Completed!");
            dgv_SyncJobList.ClearSelection();
        }

        private void SyncProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            JobQueueHandler.DeleteMainSyncFile();
            JobQueueHandler.DeleteRightClickSyncFile();
        }

        #endregion
        
    }
}
