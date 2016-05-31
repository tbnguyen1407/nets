using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using nets.dataclass;
using nets.logic;
using nets.utility;

namespace nets.gui
{
    /// <summary>
    /// Display a list of profiles for users to choose
    /// When there are multiple profiles containing
    /// a folder in "Smart Sync"
    /// Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public class PageProfileList : Panel
    {
        #region PRIVATE FIELDS

        private Button btn_Abort;
        private Button btn_OK;
        private Label lb_AskUser;
        private DataGridView dgv_ProfileList;
        private DataGridViewTextBoxColumn col_Profile;
        private readonly string[] profileNameList;
        private readonly string folderPath;

        #endregion

        #region PUBLIC CONSTRUCTORS

        public PageProfileList(string folderPath, string formattedFolderPath, ref List<string> profileNameList, ref List<string> profileList)
        {
            this.folderPath = folderPath;
            InitializeComponent(formattedFolderPath);
            this.profileNameList = profileNameList.ToArray();
            foreach (string t in profileList)
                dgv_ProfileList.Rows.Add(t);
        }

        #endregion

        #region INITIALIZE COMPONENTS

        private void InitializeComponent(string folderpath)
        {
            lb_AskUser = new Label();
            btn_OK = new Button();
            btn_Abort = new Button();
            dgv_ProfileList = new DataGridView();
            col_Profile = new DataGridViewTextBoxColumn();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();

            SuspendLayout();
            Controls.Add(btn_Abort);
            Controls.Add(btn_OK);
            Controls.Add(dgv_ProfileList);
            Controls.Add(lb_AskUser);
            Location = new Point(2, 1);
            Name = "";
            Size = new Size(418, 374);
            TabIndex = 0;

            // 
            // lb_AskUser
            // 
            lb_AskUser.AutoSize = true;
            lb_AskUser.Font = new Font("Consolas", 10.25F, FontStyle.Regular);
            lb_AskUser.Location = new Point(8, 15);
            lb_AskUser.MaximumSize = new Size(400, 3000);
            lb_AskUser.MinimumSize = new Size(400, 3000);
            lb_AskUser.Name = "lb_AskUser";
            lb_AskUser.Size = new Size(400, 1000);
            lb_AskUser.TabIndex = 0;
            lb_AskUser.Text = folderpath + " is bounded with multiple profiles. Please select one profile to continue.";
            lb_AskUser.TextAlign = ContentAlignment.TopCenter;
            
            // 
            // btn_OK
            // 
            btn_OK.Location = new Point(250, 328);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new Size(75, 28);
            btn_OK.TabIndex = 2;
            btn_OK.Text = "OK";
            btn_OK.Font = new Font("Cambria", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_OK.UseVisualStyleBackColor = true;
            btn_OK.Click += btn_OK_Click;

            // 
            // btn_Abort
            // 
            btn_Abort.Location = new Point(332, 328);
            btn_Abort.Name = "btn_Abort";
            btn_Abort.Size = new Size(75, 28);
            btn_Abort.TabIndex = 3;
            btn_Abort.Text = "Abort";
            btn_Abort.Font = new Font("Cambria", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_Abort.UseVisualStyleBackColor = true;
            btn_Abort.Click += btn_Cancel_Click;

            // 
            // dgv_ProfileList
            // 
            dgv_ProfileList.MultiSelect = false;
            dgv_ProfileList.AllowUserToAddRows = false;
            dgv_ProfileList.AllowUserToDeleteRows = false;
            dgv_ProfileList.AllowUserToResizeColumns = false;
            dgv_ProfileList.AllowUserToResizeRows = false;
            dgv_ProfileList.BackgroundColor = System.Drawing.Color.White;
            dgv_ProfileList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dgv_ProfileList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgv_ProfileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgv_ProfileList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_ProfileList.ColumnHeadersVisible = false;
            dgv_ProfileList.Columns.AddRange(new DataGridViewColumn[] {col_Profile});
            dgv_ProfileList.Location = new Point(11, 76);
            dgv_ProfileList.Name = "dgv_ProfileList";
            dgv_ProfileList.ReadOnly = true;
            dgv_ProfileList.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Font = new Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = Color.Black;
            dataGridViewCellStyle4.Padding = new Padding(5);
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.ControlDark;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgv_ProfileList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            dgv_ProfileList.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_ProfileList.RowTemplate.DefaultCellStyle.Font = new Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dgv_ProfileList.RowTemplate.DefaultCellStyle.ForeColor = Color.Black;
            dgv_ProfileList.RowTemplate.DefaultCellStyle.Padding = new Padding(5);
            dgv_ProfileList.RowTemplate.DefaultCellStyle.SelectionBackColor = SystemColors.ControlDark;
            dgv_ProfileList.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv_ProfileList.RowTemplate.Height = 53;
            dgv_ProfileList.RowTemplate.ReadOnly = true;
            dgv_ProfileList.ScrollBars = ScrollBars.Vertical;
            dgv_ProfileList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_ProfileList.Size = new Size(395, 240);
            dgv_ProfileList.TabIndex = 0;
            // 
            // col_Profile
            // 
            this.col_Profile.HeaderText = "Job Info";
            this.col_Profile.Name = "col_Profile";
            this.col_Profile.ReadOnly = true;
            this.col_Profile.Width = 395;
            
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region EVENT HANDLERS

        private void btn_OK_Click(object sender, EventArgs e)
        {
            int selectedProfileIndex = dgv_ProfileList.SelectedRows[0].Index;
            Profile profile = LogicFacade.LoadProfile(profileNameList[selectedProfileIndex].Trim());
            string normalizedSrcPath = PathOperator.GetSrcSubPath(profile.SrcFolder, profile.DesFolder, this.folderPath);
            string normalizedDesPath = PathOperator.GetDesSubPath(profile.SrcFolder, profile.DesFolder, this.folderPath);
            LogicFacade.EnqueueSyncJob(normalizedSrcPath, normalizedDesPath, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern);
            ((Form) Parent).Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ((Form) Parent).Close();
        }

        #endregion 
    }
}