namespace nets.gui
{
    partial class SyncProgress
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SyncProgress));
            this.panel_SyncProgress = new System.Windows.Forms.Panel();
            this.gbx_Action = new System.Windows.Forms.GroupBox();
            this.lb_SyncStatus = new System.Windows.Forms.Label();
            this.gbx_SyncCompleted = new System.Windows.Forms.GroupBox();
            this.dgv_SyncJobList = new System.Windows.Forms.DataGridView();
            this.col_Profile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Close = new System.Windows.Forms.Button();
            this.linklb_ViewLog = new System.Windows.Forms.LinkLabel();
            this.btn_AbortSync = new System.Windows.Forms.Button();
            this.btn_PauseSync = new System.Windows.Forms.Button();
            this.SpProgressBar = new System.Windows.Forms.ProgressBar();
            this.panel_SyncProgress.SuspendLayout();
            this.gbx_Action.SuspendLayout();
            this.gbx_SyncCompleted.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncJobList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_SyncProgress
            // 
            this.panel_SyncProgress.BackColor = System.Drawing.SystemColors.Control;
            this.panel_SyncProgress.Controls.Add(this.gbx_Action);
            this.panel_SyncProgress.Controls.Add(this.gbx_SyncCompleted);
            this.panel_SyncProgress.Controls.Add(this.btn_Close);
            this.panel_SyncProgress.Controls.Add(this.linklb_ViewLog);
            this.panel_SyncProgress.Controls.Add(this.btn_AbortSync);
            this.panel_SyncProgress.Controls.Add(this.btn_PauseSync);
            this.panel_SyncProgress.Controls.Add(this.SpProgressBar);
            this.panel_SyncProgress.ForeColor = System.Drawing.Color.Black;
            this.panel_SyncProgress.Location = new System.Drawing.Point(-1, -1);
            this.panel_SyncProgress.Name = "panel_SyncProgress";
            this.panel_SyncProgress.Size = new System.Drawing.Size(449, 420);
            this.panel_SyncProgress.TabIndex = 0;
            // 
            // gbx_Action
            // 
            this.gbx_Action.Controls.Add(this.lb_SyncStatus);
            this.gbx_Action.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbx_Action.Location = new System.Drawing.Point(12, 248);
            this.gbx_Action.Name = "gbx_Action";
            this.gbx_Action.Size = new System.Drawing.Size(424, 90);
            this.gbx_Action.TabIndex = 9;
            this.gbx_Action.TabStop = false;
            this.gbx_Action.Text = "Action";
            // 
            // lb_SyncStatus
            // 
            this.lb_SyncStatus.AutoSize = true;
            this.lb_SyncStatus.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_SyncStatus.Location = new System.Drawing.Point(8, 26);
            this.lb_SyncStatus.Name = "lb_SyncStatus";
            this.lb_SyncStatus.Size = new System.Drawing.Size(112, 14);
            this.lb_SyncStatus.TabIndex = 3;
            this.lb_SyncStatus.Text = "Initializing...";
            // 
            // gbx_SyncCompleted
            // 
            this.gbx_SyncCompleted.BackColor = System.Drawing.SystemColors.Control;
            this.gbx_SyncCompleted.Controls.Add(this.dgv_SyncJobList);
            this.gbx_SyncCompleted.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbx_SyncCompleted.Location = new System.Drawing.Point(12, 5);
            this.gbx_SyncCompleted.Name = "gbx_SyncCompleted";
            this.gbx_SyncCompleted.Size = new System.Drawing.Size(424, 237);
            this.gbx_SyncCompleted.TabIndex = 7;
            this.gbx_SyncCompleted.TabStop = false;
            this.gbx_SyncCompleted.Text = "Sync Job List";
            this.gbx_SyncCompleted.UseCompatibleTextRendering = true;
            // 
            // dgv_SyncJobList
            // 
            this.dgv_SyncJobList.AllowUserToAddRows = false;
            this.dgv_SyncJobList.AllowUserToDeleteRows = false;
            this.dgv_SyncJobList.AllowUserToResizeColumns = false;
            this.dgv_SyncJobList.AllowUserToResizeRows = false;
            this.dgv_SyncJobList.BackgroundColor = System.Drawing.Color.White;
            this.dgv_SyncJobList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgv_SyncJobList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_SyncJobList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_SyncJobList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SyncJobList.ColumnHeadersVisible = false;
            this.dgv_SyncJobList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Profile,
            this.col_Status});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_SyncJobList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_SyncJobList.Location = new System.Drawing.Point(6, 23);
            this.dgv_SyncJobList.MultiSelect = false;
            this.dgv_SyncJobList.Name = "dgv_SyncJobList";
            this.dgv_SyncJobList.ReadOnly = true;
            this.dgv_SyncJobList.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_SyncJobList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5);
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            this.dgv_SyncJobList.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_SyncJobList.RowTemplate.Height = 40;
            this.dgv_SyncJobList.RowTemplate.ReadOnly = true;
            this.dgv_SyncJobList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_SyncJobList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_SyncJobList.Size = new System.Drawing.Size(412, 207);
            this.dgv_SyncJobList.TabIndex = 0;
            this.dgv_SyncJobList.Tag = "";
            // 
            // col_Profile
            // 
            this.col_Profile.HeaderText = "Job Info";
            this.col_Profile.Name = "col_Profile";
            this.col_Profile.ReadOnly = true;
            this.col_Profile.Width = 334;
            // 
            // col_Status
            // 
            this.col_Status.HeaderText = "Status";
            this.col_Status.Name = "col_Status";
            this.col_Status.ReadOnly = true;
            this.col_Status.Width = 78;
            // 
            // btn_Close
            // 
            this.btn_Close.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.Location = new System.Drawing.Point(182, 382);
            this.btn_Close.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(74, 26);
            this.btn_Close.TabIndex = 6;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Visible = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // linklb_ViewLog
            // 
            this.linklb_ViewLog.BackColor = System.Drawing.SystemColors.Control;
            this.linklb_ViewLog.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linklb_ViewLog.LinkColor = System.Drawing.Color.Blue;
            this.linklb_ViewLog.Location = new System.Drawing.Point(369, 387);
            this.linklb_ViewLog.Name = "linklb_ViewLog";
            this.linklb_ViewLog.Size = new System.Drawing.Size(76, 23);
            this.linklb_ViewLog.TabIndex = 5;
            this.linklb_ViewLog.TabStop = true;
            this.linklb_ViewLog.Text = "View Log";
            this.linklb_ViewLog.Visible = false;
            this.linklb_ViewLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linklb_ViewLog_LinkClicked);
            // 
            // btn_AbortSync
            // 
            this.btn_AbortSync.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AbortSync.Location = new System.Drawing.Point(228, 382);
            this.btn_AbortSync.Margin = new System.Windows.Forms.Padding(2);
            this.btn_AbortSync.Name = "btn_AbortSync";
            this.btn_AbortSync.Size = new System.Drawing.Size(74, 26);
            this.btn_AbortSync.TabIndex = 3;
            this.btn_AbortSync.Text = "Abort";
            this.btn_AbortSync.UseVisualStyleBackColor = true;
            this.btn_AbortSync.Click += new System.EventHandler(this.btn_Abort_Click);
            // 
            // btn_PauseSync
            // 
            this.btn_PauseSync.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_PauseSync.Location = new System.Drawing.Point(139, 382);
            this.btn_PauseSync.Name = "btn_PauseSync";
            this.btn_PauseSync.Size = new System.Drawing.Size(75, 26);
            this.btn_PauseSync.TabIndex = 1;
            this.btn_PauseSync.Text = "Pause";
            this.btn_PauseSync.UseVisualStyleBackColor = true;
            this.btn_PauseSync.Click += new System.EventHandler(this.btn_PauseContinue_Click);
            // 
            // SpProgressBar
            // 
            this.SpProgressBar.Location = new System.Drawing.Point(12, 353);
            this.SpProgressBar.Name = "SpProgressBar";
            this.SpProgressBar.Size = new System.Drawing.Size(424, 16);
            this.SpProgressBar.TabIndex = 0;
            // 
            // SyncProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(447, 418);
            this.Controls.Add(this.panel_SyncProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SyncProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NETS (Nothing Else To Sync)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SyncProgress_FormClosing);
            this.panel_SyncProgress.ResumeLayout(false);
            this.gbx_Action.ResumeLayout(false);
            this.gbx_Action.PerformLayout();
            this.gbx_SyncCompleted.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncJobList)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panel_SyncProgress;
        private System.Windows.Forms.Button btn_PauseSync;
        private System.Windows.Forms.ProgressBar SpProgressBar;
        private System.Windows.Forms.Button btn_AbortSync;
        private System.Windows.Forms.LinkLabel linklb_ViewLog;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.GroupBox gbx_SyncCompleted;
        private System.Windows.Forms.GroupBox gbx_Action;
        private System.Windows.Forms.Label lb_SyncStatus;
        private System.Windows.Forms.DataGridView dgv_SyncJobList;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FolderPair;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Profile;

        #endregion
    }
}