#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using nets.ivle;
using nets.Properties;
using nets.utility;

#endregion

namespace nets.gui
{
    /// <summary>
    /// Display IVLE page
    /// Author: Tran Binh Nguyen
    /// </summary>
    public class PageIVLE : Panel
    {
        #region FIELD DECLARATION

        private Button btn_BrowseLocalFolder;
        private Button btn_Cancel;
        private Button btn_Start;
        private Button btn_SelectAll;
        private Button btn_SelectNone;
        private CheckedListBox clbx_ModuleCheckedListBox;
        private GroupBox gbx_AccountInfo;
        private GroupBox gbx_ModuleList;
        private GroupBox gbx_Progress;
        private IvleHandler ivleHandler;
        private Label lb_LocalFolder;
        private Label lb_Password;
        private Label lb_Username;
        private List<string> moduleList;
        private List<string> moduleListToSync;
        private RichTextBox rtbx_Progress;
        private IvleOutput outputStr;
        private TextBox tbx_LocalFolder;
        private TextBox tbx_Password;
        private TextBox tbx_Username;

        private ToolTip tltp_ErrorToolTip;

        private BackgroundWorker ivleLoginWorker;
        private BackgroundWorker ivleGetModuleListWorker;
        private BackgroundWorker ivleSyncWorker;
        private bool IsLoggedIn;
        private bool InProgress;

        public delegate void OutputInvoker(string msg);

        #endregion

        #region CONSTRUCTORS

        public PageIVLE()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        #endregion

        #region INITIALIZE 

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // lb_Username
            // 
            lb_Username = new Label();
            lb_Username.AutoSize = true;
            lb_Username.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            lb_Username.Location = new Point(20, 30);
            lb_Username.Name = "lb_Username";
            lb_Username.Size = new Size(79, 25);
            lb_Username.TabIndex = 75;
            lb_Username.Text = "Username";
            lb_Username.ForeColor = Color.Black;
            // 
            // lb_Password
            // 
            lb_Password = new Label();
            lb_Password.AutoSize = true;
            lb_Password.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            lb_Password.Location = new Point(20, 65);
            lb_Password.Name = "lb_Password";
            lb_Password.Size = new Size(106, 25);
            lb_Password.TabIndex = 75;
            lb_Password.Text = "Password";
            lb_Password.ForeColor = Color.Black;
            // 
            // lb_LocalFolder
            // 
            lb_LocalFolder = new Label();
            lb_LocalFolder.AutoSize = true;
            lb_LocalFolder.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            lb_LocalFolder.Location = new Point(20, 100);
            lb_LocalFolder.Name = "lb_LocalFolder";
            lb_LocalFolder.Size = new Size(79, 25);
            lb_LocalFolder.TabIndex = 75;
            lb_LocalFolder.Text = "Save to";
            lb_LocalFolder.ForeColor = Color.Black;
            // 
            // tbx_Username
            // 
            tbx_Username = new TextBox();
            tbx_Username.AllowDrop = true;
            tbx_Username.Font = new Font("Consolas", 9F);
            tbx_Username.Location = new Point(100, 30);
            tbx_Username.Name = "tbx_Username";
            tbx_Username.Size = new Size(185, 30);
            tbx_Username.TabIndex = 70;
            tbx_Username.ForeColor = Color.Black;
            tbx_Username.TextChanged += tbx_Username_TextChanged;
            tbx_Username.GotFocus += tbx_Username_GotFocus;
            tbx_Username.LostFocus += tbx_Username_LostFocus;
            tbx_Username.MouseHover += tbx_Username_GotFocus;
            // 
            // tbx_Password
            // 
            tbx_Password = new TextBox();
            tbx_Password.AllowDrop = true;
            tbx_Password.Font = new Font("Consolas", 9F);
            tbx_Password.Location = new Point(100, 65);
            tbx_Password.Name = "tbx_Password";
            tbx_Password.Size = new Size(185, 30);
            tbx_Password.TabIndex = 70;
            tbx_Password.UseSystemPasswordChar = true;
            tbx_Password.ForeColor = Color.Black;
            tbx_Password.TextChanged += tbx_Password_TextChanged;
            // 
            // tbx_LocalFolder
            // 
            tbx_LocalFolder = new TextBox();
            tbx_LocalFolder.AllowDrop = true;
            tbx_LocalFolder.Font = new Font("Consolas", 9F);
            tbx_LocalFolder.Location = new Point(100, 100);
            tbx_LocalFolder.Name = "tbx_LocalFolder";
            tbx_LocalFolder.Size = new Size(155, 30);
            tbx_LocalFolder.TabIndex = 70;
            tbx_LocalFolder.ForeColor = Color.Black;
            tbx_LocalFolder.DragEnter += dragEnter;
            tbx_LocalFolder.DragDrop += dragDrop;
            tbx_LocalFolder.TextChanged += tbx_LocalFolder_TextChanged;
            // 
            // btn_BrowseLocalFolder
            // 
            btn_BrowseLocalFolder = new Button();
            btn_BrowseLocalFolder.BackgroundImage = Resources.btn_BrowseFolder;
            btn_BrowseLocalFolder.BackgroundImageLayout = ImageLayout.Stretch;
            btn_BrowseLocalFolder.FlatStyle = FlatStyle.Flat;
            btn_BrowseLocalFolder.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_BrowseLocalFolder.ForeColor = SystemColors.Control;
            btn_BrowseLocalFolder.Location = new Point(255, 95);
            btn_BrowseLocalFolder.Name = "btn_BrowseLocalFolder";
            btn_BrowseLocalFolder.Size = new Size(34, 34);
            btn_BrowseLocalFolder.TabIndex = 76;
            btn_BrowseLocalFolder.UseVisualStyleBackColor = true;
            btn_BrowseLocalFolder.Click += btn_BrowseLocalFolder_Click;
            // 
            // btn_Start
            // 
            btn_Start = new Button();
            btn_Start.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_Start.Location = new Point(210, 150);
            btn_Start.Name = "btn_Start";
            btn_Start.Size = new Size(75, 27);
            btn_Start.TabIndex = 76;
            btn_Start.Text = "Log In";
            btn_Start.ForeColor = Color.Black;
            btn_Start.UseVisualStyleBackColor = true;
            btn_Start.Click += btn_Start_Click;
            // 
            // btn_Cancel
            // 
            btn_Cancel = new Button();
            btn_Cancel.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_Cancel.Location = new Point(130, 150);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new Size(75, 27);
            btn_Cancel.ForeColor = Color.Black;
            btn_Cancel.TabIndex = 76;
            btn_Cancel.Text = "Cancel";
            btn_Cancel.UseVisualStyleBackColor = true;
            btn_Cancel.Enabled = false;
            btn_Cancel.Click += btn_Cancel_Click;
            //
            // btn_SelectAll
            //
            btn_SelectAll = new Button();
            btn_SelectAll.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_SelectAll.Location = new Point(400, 30);
            btn_SelectAll.Name = "btn_SelectAll";
            btn_SelectAll.Size = new Size(90, 30);
            btn_SelectAll.TabIndex = 76;
            btn_SelectAll.Text = "Select All";
            btn_SelectAll.ForeColor = Color.Black;
            btn_SelectAll.UseVisualStyleBackColor = true;
            btn_SelectAll.Click += BtnSelectAllClick;
            //
            // btn_SelectNone
            //
            btn_SelectNone = new Button();
            btn_SelectNone.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_SelectNone.Location = new Point(400, 70);
            btn_SelectNone.Name = "btn_SelectNone";
            btn_SelectNone.Size = new Size(90, 30);
            btn_SelectNone.TabIndex = 76;
            btn_SelectNone.Text = "Select None";
            btn_SelectNone.ForeColor = Color.Black;
            btn_SelectNone.UseVisualStyleBackColor = true;
            btn_SelectNone.Click += BtnSelectNoneClick;
            //
            // tltp_ErrorToolTip
            // 
            tltp_ErrorToolTip = new ToolTip();
            tltp_ErrorToolTip.IsBalloon = true;
            //
            // rtbx_Output
            //
            rtbx_Progress = new RichTextBox();
            rtbx_Progress.Font = new Font("Cambria", 10.25F, FontStyle.Regular);
            rtbx_Progress.Location = new Point(10, 30);
            rtbx_Progress.Name = "rtbx_Progress";
            rtbx_Progress.Size = new Size(785, 114);
            rtbx_Progress.TabIndex = 0;
            rtbx_Progress.BorderStyle = BorderStyle.None;
            rtbx_Progress.Text = "";
            rtbx_Progress.ForeColor = Color.Black;
            rtbx_Progress.BackColor = Color.White;
            rtbx_Progress.ReadOnly = true;
            outputStr = new IvleOutput(rtbx_Progress);
            //
            // clbx_ModuleCheckedListBox 
            //
            clbx_ModuleCheckedListBox = new CheckedListBox();
            clbx_ModuleCheckedListBox.Font = new Font("Cambria", 10.25F, FontStyle.Regular);
            clbx_ModuleCheckedListBox.Location = new Point(20, 30);
            clbx_ModuleCheckedListBox.Size = new Size(370, 170);
            clbx_ModuleCheckedListBox.BorderStyle = BorderStyle.None;
            clbx_ModuleCheckedListBox.ForeColor = Color.Black;
            clbx_ModuleCheckedListBox.CheckOnClick = true;
            //
            // gbx_Account
            //
            gbx_AccountInfo = new GroupBox();
            gbx_AccountInfo.Text = "NUSNET ACCOUNT";
            gbx_AccountInfo.Font = new Font("Cambria", 11.25F, FontStyle.Bold);
            gbx_AccountInfo.Location = new Point(3, 2);
            gbx_AccountInfo.Size = new Size(300, 140);
            gbx_AccountInfo.BackColor = SystemColors.Control;
            gbx_AccountInfo.ForeColor = Color.Black;
            gbx_AccountInfo.UseCompatibleTextRendering = true;
            gbx_AccountInfo.Controls.Add(lb_Username);
            gbx_AccountInfo.Controls.Add(lb_Password);
            gbx_AccountInfo.Controls.Add(lb_LocalFolder);
            gbx_AccountInfo.Controls.Add(tbx_Username);
            gbx_AccountInfo.Controls.Add(tbx_Password);
            gbx_AccountInfo.Controls.Add(tbx_LocalFolder);
            gbx_AccountInfo.Controls.Add(btn_BrowseLocalFolder);
            //
            // gbx_ModuleList
            //
            gbx_ModuleList = new GroupBox();
            gbx_ModuleList.Text = "MODULE LIST";
            gbx_ModuleList.Font = new Font("Cambria", 11.25F, FontStyle.Bold);
            gbx_ModuleList.Location = new Point(310, 2);
            gbx_ModuleList.Size = new Size(500, 200);
            gbx_ModuleList.ForeColor = Color.Black;
            gbx_ModuleList.Controls.Add(clbx_ModuleCheckedListBox);
            gbx_ModuleList.Controls.Add(btn_SelectAll);
            gbx_ModuleList.Controls.Add(btn_SelectNone);
            //
            // gbx_Progress
            //
            gbx_Progress = new GroupBox();
            gbx_Progress.Text = "PROGRESS";
            gbx_Progress.Font = new Font("Cambria", 11.25F, FontStyle.Bold);
            gbx_Progress.Location = new Point(3, 200);
            gbx_Progress.Size = new Size(807, 156);
            gbx_Progress.ForeColor = Color.Black;
            gbx_Progress.Controls.Add(rtbx_Progress);
            // 
            // PageIVLE
            // 
            Location = new Point(6, 6);
            Margin = new Padding(0);
            Size = new Size(803, 345);
            Text = "PageIVLE";
            ForeColor = Color.Black;
            BackColor = SystemColors.Control;

            Controls.Add(gbx_AccountInfo);
            Controls.Add(gbx_ModuleList);
            Controls.Add(gbx_Progress);
            Controls.Add(btn_Start);
            Controls.Add(btn_Cancel);

            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeBackgroundWorker()
        {
            ivleLoginWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            ivleLoginWorker.DoWork += IvleLoginWorkerDoWork;
            ivleLoginWorker.RunWorkerCompleted += IvleLoginWorkerRunCompleted;

            ivleGetModuleListWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            ivleGetModuleListWorker.DoWork += IvleGetModuleListWorkerDoWork;
            ivleGetModuleListWorker.RunWorkerCompleted += IvleGetModuleListWorkerRunCompleted;

            ivleSyncWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            ivleSyncWorker.DoWork += IvleSyncWorkerDoWork;
            ivleSyncWorker.RunWorkerCompleted += IvleSyncWorkerRunCompleted;
        }

        #endregion

        #region COMPONENT EVENT HANDLERS

        private void btn_Start_Click(object sender, EventArgs e)
        {
            btn_Start.Enabled = false;

            if (!IsLoggedIn)
            {
                string normalizedUsername = tbx_Username.Text.Trim();
                string normalizedPassword = tbx_Password.Text.Trim();
                string normalizedLocalFolder = PathOperator.NormalizeFolderPath(tbx_LocalFolder.Text);

                // Validate info
                if (!InfoIsValid(normalizedUsername, normalizedPassword, normalizedLocalFolder))
                {
                    btn_Start.Enabled = true;
                    return;
                }

                ivleHandler = new IvleHandler(normalizedUsername, tbx_Password.Text, normalizedLocalFolder) { outputPage = this };

                // Logging in
                if (!ivleLoginWorker.IsBusy)
                {
                    InProgress = true;
                    ivleLoginWorker.RunWorkerAsync();
                }
            }
            else // Already logged in
            {
                // Read user choices
                moduleListToSync = new List<string>();

                foreach (string moduleCode in clbx_ModuleCheckedListBox.CheckedItems)
                    moduleListToSync.Add(moduleCode);

                // Proceed to sync
                if (!ivleSyncWorker.IsBusy)
                {
                    InProgress = true;
                    ivleSyncWorker.RunWorkerAsync();
                }

                //btn_Sync.Enabled = false;
                btn_Cancel.Text = "Cancel"; 
                btn_Cancel.Enabled = true;
            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // IsLoggedIn is alway true
            btn_Cancel.Enabled = false;

            if (InProgress)
            {
                if (ivleGetModuleListWorker.IsBusy)
                    ivleGetModuleListWorker.CancelAsync();
                if (ivleSyncWorker.IsBusy)
                    ivleSyncWorker.CancelAsync();
                WriteToOutput("Cancelling...Waiting for current operation to complete...\n");
            }
            else // Logged in but not doing anything -> log out
            {
                IsLoggedIn = false;
                InProgress = false;

                WriteToOutput("Account " + tbx_Username.Text.Trim() + " logged out successfully!\n");
                ResetWhenLogOut();
            }

        }

        private void btn_BrowseLocalFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            if (folder.SelectedPath != "")
                tbx_LocalFolder.Text = folder.SelectedPath;
        }

        private void tbx_Username_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_Username);
        }

        private void tbx_Username_GotFocus(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.SetToolTip(tbx_Username, "Examples: u0807231, u0909999, etc");
        }

        private void tbx_Username_LostFocus(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_Username);
        }

        private void tbx_Password_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_Password);
        }

        private void tbx_LocalFolder_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_LocalFolder);
        }

        private void BtnSelectNoneClick(object sender, EventArgs e)
        {
            for (int i = 0; i < clbx_ModuleCheckedListBox.Items.Count; i++)
                clbx_ModuleCheckedListBox.SetItemChecked(i, false);
        }

        private void BtnSelectAllClick(object sender, EventArgs e)
        {
            for (int i = 0; i < clbx_ModuleCheckedListBox.Items.Count; i++)
                clbx_ModuleCheckedListBox.SetItemChecked(i, true);
        }

        private void dragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void dragDrop(object sender, DragEventArgs e)
        {
            Array a = (Array) e.Data.GetData(DataFormats.FileDrop);

            if (a == null)
                return;
            string s = a.GetValue(0).ToString();
            FindForm().Activate();
            ((TextBox) sender).Text = s;
        }

        #endregion

        #region BACKGROUNDWORKER EVENT HANDLERS

        private void IvleLoginWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ivleHandler.LogIn();
        }

        private void IvleLoginWorkerRunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InProgress = false;

            if (!(bool)e.Result) // Logging in failed
            {
                tbx_Password.Clear();
                tbx_Password.Focus();
                btn_Start.Enabled = true;
            }
            else // Logging in successfully
            {
                IsLoggedIn = true;

                if (!ivleGetModuleListWorker.IsBusy)
                {
                    InProgress = true;
                    ivleGetModuleListWorker.RunWorkerAsync();
                }
                
                btn_Cancel.Text = "Log Out";
                btn_Cancel.Enabled = true;
            }
        }

        private void IvleGetModuleListWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            moduleList = ivleHandler.GetWorkbinIds();
        }

        private void IvleGetModuleListWorkerRunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InProgress = false;

            clbx_ModuleCheckedListBox.Items.Clear();
            
            foreach (string moduleEntry in moduleList)
                clbx_ModuleCheckedListBox.Items.Add(moduleEntry, CheckState.Checked);

            btn_Start.Text = "Sync";
            btn_Start.Enabled = true;                       
            btn_Cancel.Text = "Log Out";
            btn_Cancel.Enabled = true;
        }

        private void IvleSyncWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Read user module choices
            foreach (string moduleEntry in clbx_ModuleCheckedListBox.CheckedItems)
                moduleListToSync.Add(moduleEntry);
            // @BENNY: TO SUPPORT BASIC CANCELLATION, THE SYNC JOB MUST BE BROKEN INTO SMALLER PIECES
            
            // Getting file list
            ivleHandler.GetFiles(moduleListToSync);
            
            if (((BackgroundWorker)sender).CancellationPending)
            {
                e.Cancel = true;
                e.Result = false;
                return;
            }

            // Detecting file differences -> THIS RUNS TOO FAST
            LocalUpdater updater = ivleHandler.DetectDifferences(moduleListToSync);

            if (((BackgroundWorker)sender).CancellationPending)
            {
                e.Cancel = true;
                e.Result = false;
                return;
            }

            // Do the actual sync -> THIS IS TOO BIG
            updater.Synchronize();
        }

        private void IvleSyncWorkerRunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_Start.Enabled = true;

            if (e.Error != null)
                WriteToOutput(e.Error.Message + "\n");
            else if (e.Cancelled)
            {
                if (InProgress) // else continue with the current session
                {
                    btn_Cancel.Text = "Log Out";
                    btn_Cancel.Enabled = true;
                    WriteToOutput("IVLE Sync cancelled by user!\n");
                }
                else // Cancel starts a new session
                {
                    // Reset state
                    IsLoggedIn = false;
                    //WriteToOutput("Account " + tbx_Username.Text.Trim() + " logged out successfully!\n");
                }
                InProgress = false;
            }
            else //succeeded
            {
                InProgress = true;
                clbx_ModuleCheckedListBox.Enabled = true;
                btn_Cancel.Text = "Log out";
                MessageBox.Show("IVLE Sync completed successfully!");
            }
        }

        private void ResetWhenLogOut()
        {
            // Reset fields
            tbx_Username.Clear();
            tbx_Password.Clear();
            tbx_LocalFolder.Clear();
            clbx_ModuleCheckedListBox.Items.Clear();

            btn_Start.Text = "Log In";
            btn_Start.Enabled = true;
            btn_Cancel.Text = "Cancel";
            btn_Cancel.Enabled = false;
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Write to progress richtextbox
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToOutput(string msg)
        {
            if (InvokeRequired)
            {
                OutputInvoker lInvoker = WriteToOutput_Invoked;
                Invoke(lInvoker, msg);
            }
            else
                WriteToOutput_Invoked(msg);
        }

        private void WriteToOutput_Invoked(string msg)
        {
            rtbx_Progress.Select(rtbx_Progress.Text.Length, rtbx_Progress.Text.Length);
            rtbx_Progress.ScrollToCaret();

            outputStr.Write(msg);
        }

        /// <summary>
        /// Check for valid input information
        /// </summary>
        /// <param name="normalizedUsername"></param>
        /// <param name="password"></param>
        /// <param name="normalizedLocalFolder"></param>
        /// <returns></returns>
        private bool InfoIsValid(string normalizedUsername, string password, string normalizedLocalFolder)
        {
            // Check for empty Username
            if (normalizedUsername == String.Empty)
            {
                ShowToolTip("Username cannnot be empty!", tbx_Username);
                return false;
            }

            //Check for valid Username format
            //if (!Regex.Match(normalizedUsername, @"^[u|U]\d{7}$").Success)
            //{
            //    ShowToolTip(@"Invalid NUSNET username! (Ex: U0707384)", tbx_Username);
            //    return false;
            //}

            // Check for empty Password field
            if (password == String.Empty)
            {
                ShowToolTip("Password cannnot be empty!", tbx_Password);
                return false;
            }

            // Check for valid LocalFolder
            if (!Directory.Exists(normalizedLocalFolder))
            {
                ShowToolTip("Invalid folder path!", tbx_LocalFolder);
                return false;
            }

            // Info is valid
            tbx_Username.Text = normalizedUsername;
            tbx_LocalFolder.Text = normalizedLocalFolder;
            return true;
        }

        /// <summary>
        /// Show tooltip on wrong input
        /// </summary>
        /// <param name="message"></param>
        /// <param name="window"></param>
        private void ShowToolTip(string message, IWin32Window window)
        {
            for (int i = 0; i < 2; i++)
                tltp_ErrorToolTip.Show(message, window, 5000);
        }

        #endregion
    }
}