using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using nets.dataclass;
using nets.logic;
using nets.utility;
using System.Threading;

namespace nets.gui
{
    /// <summary>
    /// For users to fill in profile information (source folder, destination folder, sync mode, etc)
    /// Author: Hoang Nguyen Nhat Tao + Tran Binh Nguyen + Nguyen Thi Yen Duong
    /// </summary>
    class PageProfileDetails : Panel
    {
        #region PRIVATE FIELD DECLARATION
        
        private GroupBox gbx_BasicInfo;
        private GroupBox gbx_AdvancedInfo;

        private Label lb_SrcFolder;
        private Label lb_DesFolder;
        private Label lb_SyncMode;
        private Label lb_ExcludePattern;
        private Label lb_IncludePattern;
        private Label lb_ProfileName;

        private TextBox tbx_SrcFolder;
        private TextBox tbx_DesFolder;
        private TextBox tbx_ExcludePattern;
        private TextBox tbx_IncludePattern;
        private TextBox tbx_ProfileName;

        private Button btn_BrowseSrcFolder;
        private Button btn_BrowseDesFolder;
        private Button btn_Abort;
        private Button btn_SaveProfile;
        private Button btn_Sync;
        
        private RadioButton rbtn_OneWayMode;
        private RadioButton rbtn_TwoWayMode;

        private ToolTip tltp_ErrorToolTip;
        private FolderBrowserDialog dialog_FolderBrowser;
        
        private readonly RunningMode runningMode;
        private Thread thread;

        #endregion

        #region PROPERTIES

        public bool IsEditing { private get; set; }
        public static bool AutoCompleteProfileName { private get; set; }

        #endregion

        #region PUBLIC CONSTRUCTORS

        public PageProfileDetails()
            : this(RunningMode.MainApplication)
        {
        }

        private PageProfileDetails(RunningMode runningMode)
        {
            this.runningMode = runningMode;
            AutoCompleteProfileName = LogicFacade.LoadSetting("autocomplete");
            InitializeComponent();
        }

        public PageProfileDetails(Profile profile)
            : this(profile, RunningMode.MainApplication)
        {
        }

        public PageProfileDetails(Profile profile, RunningMode runningMode)
        {
            this.runningMode = runningMode;
            AutoCompleteProfileName = LogicFacade.LoadSetting("autocomplete");
            InitializeComponent();
            SetProfileInfo(profile);
        }

        #endregion

        #region INITIALIZE
         
        private void InitializeComponent()
        {
            gbx_BasicInfo = new GroupBox();
            gbx_AdvancedInfo = new GroupBox();

            btn_Abort = new Button();
            btn_SaveProfile = new Button();
            btn_Sync = new Button();
            
            lb_SrcFolder = new Label();
            lb_DesFolder = new Label();
            tbx_SrcFolder = new TextBox();
            tbx_DesFolder = new TextBox();
            tltp_ErrorToolTip = new ToolTip();

            btn_BrowseDesFolder = new Button();
            btn_BrowseSrcFolder = new Button();

            lb_ProfileName = new Label();
            tbx_ProfileName = new TextBox();

            lb_SyncMode = new Label();
            rbtn_OneWayMode = new RadioButton();
            rbtn_TwoWayMode = new RadioButton();

            lb_ExcludePattern = new Label();
            lb_IncludePattern = new Label();
            tbx_ExcludePattern = new TextBox();
            tbx_IncludePattern = new TextBox();
            
            dialog_FolderBrowser = new FolderBrowserDialog();

            gbx_BasicInfo.SuspendLayout();
            gbx_AdvancedInfo.SuspendLayout();

            SuspendLayout();

            if (runningMode != RunningMode.MainApplication)
                Controls.Add(btn_Abort);
            if (runningMode == RunningMode.MainApplication)
                Controls.Add(btn_SaveProfile);
            Controls.Add(btn_Sync);
            Controls.Add(gbx_AdvancedInfo);
            Controls.Add(gbx_BasicInfo);
            Dock = DockStyle.Fill;
            Location = new Point(0, 0);
            Size = new Size(460, 349);
            TabIndex = 20;
            // 
            // btn_Abort
            // 
            btn_Abort.Location = new Point(372, 296);
            btn_Abort.Name = "btn_Abort";
            btn_Abort.Size = new Size(75, 27);
            btn_Abort.TabIndex = 77;
            btn_Abort.Text = "Abort";
            btn_Abort.Click += btn_Abort_Click;
            btn_Abort.Font = new Font("Cambria", 10.25F, FontStyle.Bold);
            btn_Abort.UseVisualStyleBackColor = true;
            // 
            // btn_SaveProfile
            // 
            btn_SaveProfile.Location = new Point(372, 296);
            btn_SaveProfile.Name = "btn_SaveProfile";
            btn_SaveProfile.Size = new Size(75, 27);
            btn_SaveProfile.TabIndex = 76;
            btn_SaveProfile.Text = "Save";
            btn_SaveProfile.Font = new Font("Cambria", 10.25F, FontStyle.Bold);
            btn_SaveProfile.UseVisualStyleBackColor = true;
            btn_SaveProfile.Click += btn_SaveProfile_Click;
            // 
            // btn_Sync
            // 
            btn_Sync.Location = new Point(291, 296);
            btn_Sync.Name = "btn_Sync";
            btn_Sync.Size = new Size(75, 27);
            btn_Sync.TabIndex = 75;
            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    btn_Sync.Text = "Sync";
                    break;
                case RunningMode.ContextMenuSmartSync:
                case RunningMode.ContextMenuSyncWith:
                    btn_Sync.Text = "OK";
                    break;
            }
            btn_Sync.Font = new Font("Cambria", 10.25F, FontStyle.Bold);
            btn_Sync.UseVisualStyleBackColor = true;
            btn_Sync.Click += btn_Sync_Click;
            // 
            // gbx_BasicInfo
            // 
            gbx_BasicInfo.Controls.Add(btn_BrowseDesFolder);
            gbx_BasicInfo.Controls.Add(btn_BrowseSrcFolder);
            gbx_BasicInfo.Controls.Add(lb_SrcFolder);
            gbx_BasicInfo.Controls.Add(tbx_SrcFolder);
            gbx_BasicInfo.Controls.Add(lb_DesFolder);
            gbx_BasicInfo.Controls.Add(tbx_DesFolder);
            gbx_BasicInfo.Font = new Font("Cambria", 11.25F, FontStyle.Bold);
            gbx_BasicInfo.Location = new Point(12, 12);
            gbx_BasicInfo.Name = "gbx_BasicInfo";
            gbx_BasicInfo.Size = new Size(434, 92);
            gbx_BasicInfo.TabIndex = 0;
            gbx_BasicInfo.TabStop = false;
            gbx_BasicInfo.Text = "Basic Information";
            gbx_BasicInfo.ForeColor = Color.Black;
            gbx_BasicInfo.BackColor = SystemColors.Control;
            // 
            // gbx_AdvancedInfo
            // 
            gbx_AdvancedInfo.Controls.Add(lb_ProfileName);
            gbx_AdvancedInfo.Controls.Add(tbx_ProfileName);
            gbx_AdvancedInfo.Controls.Add(lb_SyncMode);
            gbx_AdvancedInfo.Controls.Add(rbtn_OneWayMode);
            gbx_AdvancedInfo.Controls.Add(rbtn_TwoWayMode);
            gbx_AdvancedInfo.Controls.Add(tbx_ExcludePattern);
            gbx_AdvancedInfo.Controls.Add(tbx_IncludePattern);
            gbx_AdvancedInfo.Controls.Add(lb_ExcludePattern);
            gbx_AdvancedInfo.Controls.Add(lb_IncludePattern);
            gbx_AdvancedInfo.Font = new Font("Cambria", 11.25F, FontStyle.Bold);
            gbx_AdvancedInfo.Location = new Point(12, 125);
            gbx_AdvancedInfo.Name = "gbx_AdvancedInfo";
            gbx_AdvancedInfo.Size = new Size(434, 162);
            gbx_AdvancedInfo.TabIndex = 1;
            gbx_AdvancedInfo.TabStop = false;
            gbx_AdvancedInfo.Text = "Advanced Options";
            gbx_AdvancedInfo.ForeColor = Color.Black;
            gbx_AdvancedInfo.BackColor = SystemColors.Control;
            // 
            // lb_ProfileName
            // 
            lb_ProfileName.AutoSize = true;
            lb_ProfileName.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_ProfileName.Location = new Point(23, 123);
            lb_ProfileName.Name = "lb_ProfileName";
            lb_ProfileName.Size = new Size(36, 13);
            lb_ProfileName.TabIndex = 75;
            lb_ProfileName.Text = "Profile";
            // 
            // tbx_ProfileName
            // 
            tbx_ProfileName.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tbx_ProfileName.Location = new Point(95, 121);
            tbx_ProfileName.Name = "tbx_ProfileName";
            tbx_ProfileName.Size = new Size(300, 20);
            tbx_ProfileName.TabIndex = 74;
            tbx_ProfileName.GotFocus += tbx_ProfileName_GotFocus;
            tbx_ProfileName.LostFocus += tbx_ProfileName_LostFocus;
            tbx_ProfileName.TextChanged += tbx_ProfileName_TextChanged;
            if (runningMode != RunningMode.MainApplication)
                tbx_ProfileName.Enabled = false;
            // 
            // lb_SyncMode
            // 
            lb_SyncMode.AutoSize = true;
            lb_SyncMode.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_SyncMode.Location = new Point(21, 31);
            lb_SyncMode.Name = "lb_SyncMode";
            lb_SyncMode.Size = new Size(61, 13);
            lb_SyncMode.TabIndex = 71;
            lb_SyncMode.Text = "Sync Mode";
            // 
            // rbtn_OneWayMode
            // 
            rbtn_OneWayMode.AutoSize = true;
            rbtn_OneWayMode.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            rbtn_OneWayMode.Location = new Point(94, 29);
            rbtn_OneWayMode.Name = "rbtn_OneWayMode";
            rbtn_OneWayMode.Size = new Size(67, 17);
            rbtn_OneWayMode.TabIndex = 72;
            rbtn_OneWayMode.TabStop = true;
            rbtn_OneWayMode.Text = "One-way";
            rbtn_OneWayMode.UseVisualStyleBackColor = true;
            // 
            // rbtn_TwoWayMode
            // 
            rbtn_TwoWayMode.AutoSize = true;
            rbtn_TwoWayMode.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            rbtn_TwoWayMode.Location = new Point(168, 29);
            rbtn_TwoWayMode.Name = "rbtn_TwoWayMode";
            rbtn_TwoWayMode.Size = new Size(68, 17);
            rbtn_TwoWayMode.TabIndex = 73;
            rbtn_TwoWayMode.TabStop = true;
            rbtn_TwoWayMode.Text = "Two-way";
            rbtn_TwoWayMode.UseVisualStyleBackColor = true;
            // 
            // tbx_ExcludePattern
            // 
            tbx_ExcludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tbx_ExcludePattern.Location = new Point(95, 88);
            tbx_ExcludePattern.Name = "tbx_ExcludePattern";
            tbx_ExcludePattern.Size = new Size(300, 20);
            tbx_ExcludePattern.TabIndex = 70;
            tbx_ExcludePattern.AllowDrop = true;
            tbx_ExcludePattern.Text = "";
            tbx_ExcludePattern.Enabled = true;
            tbx_ExcludePattern.GotFocus += tbx_ExcludePattern_GotFocus;
            tbx_ExcludePattern.LostFocus += tbx_ExcludePattern_LostFocus;
            tbx_ExcludePattern.MouseHover += tbx_ExcludePattern_GotFocus;
            // 
            // tbx_IncludePattern
            // 
            tbx_IncludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tbx_IncludePattern.Location = new Point(95, 55);
            tbx_IncludePattern.Name = "tbx_IncludePattern";
            tbx_IncludePattern.Size = new Size(300, 20);
            tbx_IncludePattern.TabIndex = 69;
            tbx_IncludePattern.AllowDrop = true;
            tbx_IncludePattern.Text = "";
            tbx_IncludePattern.Enabled = true;
            tbx_IncludePattern.GotFocus += tbx_IncludePattern_GotFocus;
            tbx_IncludePattern.LostFocus += tbx_IncludePattern_LostFocus;
            tbx_IncludePattern.MouseHover += tbx_IncludePattern_GotFocus;
            // 
            // lb_ExcludePattern
            // 
            lb_ExcludePattern.AutoSize = true;
            lb_ExcludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_ExcludePattern.Location = new Point(22, 91);
            lb_ExcludePattern.Name = "lb_ExcludePattern";
            lb_ExcludePattern.Size = new Size(48, 13);
            lb_ExcludePattern.TabIndex = 68;
            lb_ExcludePattern.Text = "Exclude";
            // 
            // lb_IncludePattern
            // 
            lb_IncludePattern.AutoSize = true;
            lb_IncludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_IncludePattern.Location = new Point(22, 62);
            lb_IncludePattern.Name = "lb_IncludePattern";
            lb_IncludePattern.Size = new Size(42, 13);
            lb_IncludePattern.TabIndex = 67;
            lb_IncludePattern.Text = "Include";
            // 
            // btn_BrowseDesFolder
            // 
            btn_BrowseDesFolder.BackgroundImage = global::nets.Properties.Resources.btn_BrowseFolder;
            btn_BrowseDesFolder.BackgroundImageLayout = ImageLayout.Stretch;
            btn_BrowseDesFolder.FlatStyle = FlatStyle.Flat;
            btn_BrowseDesFolder.Location = new Point(395, 53);
            btn_BrowseDesFolder.Name = "btn_BrowseDesFolder";
            btn_BrowseDesFolder.Size = new Size(34, 34);
            btn_BrowseDesFolder.TabIndex = 68;
            btn_BrowseDesFolder.UseVisualStyleBackColor = true;
            btn_BrowseDesFolder.ForeColor = SystemColors.Control;
            btn_BrowseDesFolder.Click += btn_BrowseForDes_Click;
            // 
            // btn_BrowseSrcFolder
            // 
            btn_BrowseSrcFolder.BackgroundImage = global::nets.Properties.Resources.btn_BrowseFolder;
            btn_BrowseSrcFolder.BackgroundImageLayout = ImageLayout.Stretch;
            btn_BrowseSrcFolder.FlatStyle = FlatStyle.Flat;
            btn_BrowseSrcFolder.ForeColor = SystemColors.Control;
            btn_BrowseSrcFolder.Location = new Point(395, 18);
            btn_BrowseSrcFolder.Name = "btn_BrowseSrcFolder";
            btn_BrowseSrcFolder.Size = new Size(34, 34);
            btn_BrowseSrcFolder.TabIndex = 67;
            btn_BrowseSrcFolder.UseVisualStyleBackColor = true;
            btn_BrowseSrcFolder.Click += btn_BrowseForSrc_Click;
            // 
            // lb_SrcFolder
            // 
            lb_SrcFolder.AutoSize = true;
            lb_SrcFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_SrcFolder.Location = new Point(19, 28);
            lb_SrcFolder.Name = "lb_SrcFolder";
            lb_SrcFolder.Size = new Size(41, 13);
            lb_SrcFolder.TabIndex = 59;
            lb_SrcFolder.Text = "Source";
            // 
            // tbx_SrcFolder
            // 
            tbx_SrcFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tbx_SrcFolder.Location = new Point(95, 25);
            tbx_SrcFolder.Name = "tbx_SrcFolder";
            tbx_SrcFolder.Size = new Size(300, 20);
            tbx_SrcFolder.BackColor = Color.White;
            tbx_SrcFolder.ForeColor = Color.Black;
            tbx_SrcFolder.TabIndex = 60;
            tbx_SrcFolder.AllowDrop = true;
            tbx_SrcFolder.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbx_SrcFolder.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            tbx_SrcFolder.DragDrop += dragDropForSrcDes;
            tbx_SrcFolder.DragEnter += dragEnter;
            tbx_SrcFolder.GotFocus += tbx_SrcFolder_GotFocus;
            tbx_SrcFolder.LostFocus += tbx_SrcFolder_LostFocus;
            if (runningMode == RunningMode.MainApplication)
                tbx_SrcFolder.TextChanged += tbx_SrcFolder_TextChanged;
            if (runningMode == RunningMode.ContextMenuSmartSync)
                tbx_SrcFolder.Enabled = false;
            // 
            // lb_DesFolder
            // 
            lb_DesFolder.AutoSize = true;
            lb_DesFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            lb_DesFolder.Location = new Point(20, 61);
            lb_DesFolder.Name = "lb_DesFolder";
            lb_DesFolder.Size = new Size(60, 13);
            lb_DesFolder.TabIndex = 61;
            lb_DesFolder.Text = "Destination";
            // 
            // tbx_DesFolder
            // 
            tbx_DesFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tbx_DesFolder.Location = new Point(95, 58);
            tbx_DesFolder.Name = "tbx_DesFolder";
            tbx_DesFolder.Size = new Size(300, 20);
            tbx_DesFolder.BackColor = Color.White;
            tbx_DesFolder.ForeColor = Color.Black;
            tbx_DesFolder.TabIndex = 62;
            tbx_DesFolder.AllowDrop = true;
            tbx_DesFolder.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbx_DesFolder.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
            tbx_DesFolder.DragDrop += dragDropForSrcDes;
            tbx_DesFolder.DragEnter += dragEnter;
            tbx_DesFolder.GotFocus += tbx_DesFolder_GotFocus;
            tbx_DesFolder.LostFocus += tbx_DesFolder_LostFocus;
            if (runningMode == RunningMode.MainApplication)
                tbx_DesFolder.TextChanged += tbx_DesFolder_TextChanged;
            //
            // tltp_ErrorToolTip
            //
            tltp_ErrorToolTip.IsBalloon = false;
            tltp_ErrorToolTip.UseAnimation = true;
            tltp_ErrorToolTip.UseFading = true;
            tltp_ErrorToolTip.IsBalloon = true;
            //
            // PageProfileDetails
            //
            ResumeLayout(false);
            PerformLayout();
            gbx_AdvancedInfo.ResumeLayout(false);
            gbx_AdvancedInfo.PerformLayout();
            gbx_BasicInfo.ResumeLayout(false);
            gbx_BasicInfo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        #region COMPONENT EVENT HANDLERS

        private void tbx_SrcFolder_GotFocus(object sender, EventArgs e)
        {
            lb_SrcFolder.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
        }

        private void tbx_SrcFolder_LostFocus(object sender, EventArgs e)
        {
            lb_SrcFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tltp_ErrorToolTip.Hide(tbx_SrcFolder);
        }

        private void tbx_SrcFolder_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_SrcFolder);
            if (AutoCompleteProfileName)
                ModifyProfileNameFollowingSrcAndDes();
        }

        private void tbx_DesFolder_GotFocus(object sender, EventArgs e)
        {
            lb_DesFolder.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
        }

        private void tbx_DesFolder_LostFocus(object sender, EventArgs e)
        {
            lb_DesFolder.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tltp_ErrorToolTip.Hide(tbx_DesFolder);
        }

        private void tbx_DesFolder_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_DesFolder);
            if (AutoCompleteProfileName)
                ModifyProfileNameFollowingSrcAndDes();
        }

        private void tbx_IncludePattern_GotFocus(object sender, EventArgs e)
        {
            lb_IncludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            tltp_ErrorToolTip.SetToolTip(tbx_IncludePattern, "Example: *.txt, *fish*, cat*.doc");
        }

        private void tbx_IncludePattern_LostFocus(object sender, EventArgs e)
        {
            lb_IncludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tltp_ErrorToolTip.Hide(tbx_IncludePattern);
        }

        private void tbx_ExcludePattern_GotFocus(object sender, EventArgs e)
        {
            lb_ExcludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            tltp_ErrorToolTip.SetToolTip(tbx_ExcludePattern, "Example: *.txt, *fish*, cat*.doc");
        }

        private void tbx_ExcludePattern_LostFocus(object sender, EventArgs e)
        {
            lb_ExcludePattern.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tltp_ErrorToolTip.Hide(tbx_ExcludePattern);
        }

        private void tbx_ProfileName_GotFocus(object sender, EventArgs e)
        {
            lb_ProfileName.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
        }

        private void tbx_ProfileName_LostFocus(object sender, EventArgs e)
        {
            lb_ProfileName.Font = new Font("Consolas", 8.5F, FontStyle.Regular);
            tltp_ErrorToolTip.Hide(tbx_ProfileName);
        }

        private void tbx_ProfileName_TextChanged(object sender, EventArgs e)
        {
            tltp_ErrorToolTip.Hide(tbx_ProfileName);
        }

        private void btn_BrowseForSrc_Click(object sender, EventArgs e)
        {
            dialog_FolderBrowser = new FolderBrowserDialog();
            DialogResult userResponse = dialog_FolderBrowser.ShowDialog();
            string selectedPath = dialog_FolderBrowser.SelectedPath.Trim();

            if (userResponse == DialogResult.OK || selectedPath != "")
                tbx_SrcFolder.Text = selectedPath;
        }

        private void btn_BrowseForDes_Click(object sender, EventArgs e)
        {
            dialog_FolderBrowser = new FolderBrowserDialog();
            DialogResult userResponse = dialog_FolderBrowser.ShowDialog();
            string selectedPath = dialog_FolderBrowser.SelectedPath.Trim();

            if (userResponse == DialogResult.OK || selectedPath != "")
                tbx_DesFolder.Text = selectedPath;
        }

        private void btn_Sync_Click(object sender, EventArgs e)
        {
            string normalizedSrcFolder = PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text);
            string normalizedDesFolder = PathOperator.NormalizeFolderPath(tbx_DesFolder.Text);

            if (!FolderPathInfoIsValid(normalizedSrcFolder, normalizedDesFolder))
                return;

            if (runningMode == RunningMode.MainApplication && JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();

            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    thread = new Thread(
                        () => GUIEventHandler.SyncHandler(PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text),
                                                          PathOperator.NormalizeFolderPath(tbx_DesFolder.Text),
                                                          (rbtn_TwoWayMode.Checked) ? SyncMode.TwoWay : SyncMode.OneWay,
                                                          tbx_IncludePattern.Text.Trim(), tbx_ExcludePattern.Text.Trim()));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    break;
                case RunningMode.ContextMenuSyncWith:
                    GUIEventHandler.SyncHandler(PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text),
                                                PathOperator.NormalizeFolderPath(tbx_DesFolder.Text),
                                                (rbtn_TwoWayMode.Checked) ? SyncMode.TwoWay : SyncMode.OneWay,
                                                tbx_IncludePattern.Text, tbx_ExcludePattern.Text);
                    (Parent as Form).Close();
                    break;
                case RunningMode.ContextMenuSmartSync:
                    LogicFacade.EnqueueSyncJob(PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text),
                                               PathOperator.NormalizeFolderPath(tbx_DesFolder.Text),
                                               (rbtn_TwoWayMode.Checked) ? SyncMode.TwoWay : SyncMode.OneWay,
                                               tbx_IncludePattern.Text, tbx_ExcludePattern.Text);

                    (Parent as Form).Close();
                    break;
            }
        }

        private void btn_SaveProfile_Click(object sender, EventArgs e)
        {
            if (IsEditing && JobQueueHandler.SyncInProgress())
            {
                MessageBox.Show("Sorry! Editing profile information during a running sync job is not allowed!\r\n",
                                "Unable to edit profile",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                btn_SaveProfile.Enabled = true;
                return;
            }

            string normalizedSrcFolder = PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text);
            string normalizedDesFolder = PathOperator.NormalizeFolderPath(tbx_DesFolder.Text);
            string trimmedProfileName = tbx_ProfileName.Text.Trim();

            // Validate currentProfile information)
            if (!FolderPathInfoIsValid(normalizedSrcFolder, normalizedDesFolder))
                return;
            if (FolderPathInfoIsConflicted(normalizedSrcFolder, normalizedDesFolder))
                return;
            if (!ProfileNameIsValid(trimmedProfileName))
                return;
            if (ProfileNameIsConflicted(trimmedProfileName))
                return;

            Profile newProfile = new Profile(tbx_ProfileName.Text.Trim(),
                                             PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text.Trim()),
                                             PathOperator.NormalizeFolderPath(tbx_DesFolder.Text.Trim()),
                                             (rbtn_TwoWayMode.Checked) ? SyncMode.TwoWay : SyncMode.OneWay,
                                             tbx_IncludePattern.Text.Trim(),
                                             tbx_ExcludePattern.Text.Trim());

            // Take action accordingly
            if (!IsEditing) // CreateAndSave
                LogicFacade.CreateAndSaveProfile(newProfile);
            else // EditAndSave
            {
                Profile oldProfile = LogicFacade.GetCurrentProfile();
                LogicFacade.EditAndSaveProfile(oldProfile, newProfile);
            }

            // Confirmation if necessary (in cases other than MainApplication)
            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    ((PageStart) Parent.Parent).RefreshProfileList(tbx_ProfileName.Text.Trim());
                    break;
                default:
                    MessageBox.Show("Profile " + newProfile.ProfileName + " has successfully been saved!");
                    break;
            }

            // Switch to IsEditing after saving
            IsEditing = true;
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            ((Form) Parent).Close();
        }

        private static void dragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void dragDropForSrcDes(object sender, DragEventArgs e)
        {
            Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

            if (a == null)
                return;
            string s = a.GetValue(0).ToString();
            this.FindForm().Activate();
            ((TextBox) sender).Text = s;
        }

        #endregion

        #region PRIVATE METHODS TO VALIDATE PROFILE INFORMATION

        /// <summary>
        /// Check for valid folder paths (inside a currentProfile scope only)
        /// </summary>
        /// <returns></returns>
        private bool FolderPathInfoIsValid(string normalizedSrcFolder, string normalizedDesFolder)
        {
            // Check for empty folder paths
            if (normalizedSrcFolder == String.Empty)
            {
                tbx_SrcFolder.Focus();
                ShowToolTip("Source folder path cannot be empty.", tbx_SrcFolder);
                return false;
            }

            if (normalizedDesFolder == String.Empty)
            {
                tbx_DesFolder.Focus();
                ShowToolTip("Destination folder path cannot be empty.", tbx_DesFolder);
                return false;
            }

            // Check for folder existence
            if (!Directory.Exists(normalizedSrcFolder))
            {
                tbx_SrcFolder.Focus();
                ShowToolTip("Source folder does not exist.", tbx_SrcFolder);
                return false;
            }
            if (!Directory.Exists(normalizedDesFolder))
            {
                tbx_DesFolder.Focus();
                ShowToolTip("Destination folder does not exist.", tbx_DesFolder);
                return false;
            }

            // Check for same source and destination folders)
            if (normalizedSrcFolder.ToLower() == normalizedDesFolder.ToLower())
            {
                tbx_DesFolder.Focus();
                ShowToolTip("Destination folder must be different from source folder.", tbx_DesFolder);
                return false;
            }

            // Check for possible recursion
            if (normalizedSrcFolder.ToLower().StartsWith(normalizedDesFolder.ToLower()) || normalizedDesFolder.ToLower().StartsWith(normalizedSrcFolder.ToLower()))
            {
                tbx_SrcFolder.Focus();
                ShowToolTip("Source and destination folders cannot include each other.", tbx_SrcFolder);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check for conflicted folder paths (with other profiles)
        /// </summary>
        /// <returns></returns>
        private bool FolderPathInfoIsConflicted(string normalizedSrcFolder, string normalizedDesFolder)
        {
            string currentProfileName = LogicFacade.GetCurrentProfile().ProfileName;
            List<string> conflictedProfileNameList = LogicFacade.GetConflictedProfileNameList(normalizedSrcFolder, normalizedDesFolder);

            // remove currentProfile from conflictedProfileList in case of editing
            if (IsEditing)
                conflictedProfileNameList.Remove(currentProfileName);

            if (conflictedProfileNameList.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();

                errorMessage.Append("This currentProfile is in conflict with the following currentProfile(s):\n");
                foreach (string profileName in conflictedProfileNameList)
                    errorMessage.Append("  " + profileName + "\n");
                errorMessage.Append("Please delete existing currentProfile(s) then try again!");

                ShowToolTip(errorMessage.ToString(), tbx_SrcFolder);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check for valid profileName (inside a currentProfile scope only)
        /// </summary>
        /// <returns></returns>
        private bool ProfileNameIsValid(string trimmedProfileName)
        {
            // Check for empty profileName
            if (trimmedProfileName == String.Empty)
            {
                tbx_ProfileName.Focus();
                ShowToolTip("Profile name cannot be empty.", tbx_ProfileName);
                return false;
            }

            // Check for valid profileName (does not contain special symbols)
            if (trimmedProfileName.Contains("|") || trimmedProfileName.Contains(@"\") || 
                trimmedProfileName.Contains(":") || trimmedProfileName.Contains("\"") ||
                trimmedProfileName.Contains("<") || trimmedProfileName.Contains(">")  ||
                trimmedProfileName.Contains("?") || trimmedProfileName.Contains("*")  ||
                trimmedProfileName.Contains("/"))
            {
                ShowToolTip("Profile name cannot contain the following characters: \r\n   " + "|, \\, :, \", >, <, ?, *, /", tbx_ProfileName);
                return false;
            }

            // ProfileName is valid
            tbx_ProfileName.Text = trimmedProfileName;
            return true;
        }

        /// <summary>
        /// Check for conflicted profileName (with other profiles)
        /// </summary>
        /// <returns></returns>
        private bool ProfileNameIsConflicted(string trimmedProfileName)
        {
            string curProfileName = LogicFacade.GetCurrentProfile().ProfileName;
            string newProfileName = trimmedProfileName;

            List<string> profileNameList = LogicFacade.LoadProfileNameList();

            // remove currentProfile from conflictedProfileList in case of editing
            if (IsEditing)
                profileNameList.Remove(curProfileName);

            if (profileNameList.Contains(newProfileName))
            {
                ShowToolTip("A currentProfile with the same name already exists.", tbx_ProfileName);
                return true;
            }

            return false;
        }
        
        #endregion

        #region PRIVATE HELPERS

        /// <summary>
        /// Set currentProfile info for the current page
        /// </summary>
        /// <param name="profile"></param>
        public void SetProfileInfo(Profile profile)
        {
            tbx_SrcFolder.Text = profile.SrcFolder;
            tbx_DesFolder.Text = profile.DesFolder;

            tbx_ExcludePattern.Text = profile.ExcludePattern;
            tbx_IncludePattern.Text = profile.IncludePattern;

            tbx_ProfileName.Text = profile.ProfileName;

            switch (profile.SyncMode)
            {
                case SyncMode.OneWay:
                    rbtn_OneWayMode.Checked = true;
                    rbtn_TwoWayMode.Checked = false;
                    break;
                case SyncMode.TwoWay:
                    rbtn_OneWayMode.Checked = false;
                    rbtn_TwoWayMode.Checked = true;
                    break;
            }
        }

        private void ModifyProfileNameFollowingSrcAndDes()
        {
            if (runningMode != RunningMode.MainApplication)
                return;

            string srcName = PathOperator.GetNameFromPath(tbx_SrcFolder.Text);
            string desName = PathOperator.GetNameFromPath(tbx_DesFolder.Text);

            tbx_ProfileName.Text = srcName + " & " + desName;
        }

        private void ShowToolTip(string message, IWin32Window window)
        {
            for (int i = 0; i < 2; i++)
                tltp_ErrorToolTip.Show(message, window, 5000);
        }

        #endregion
    }
} 
