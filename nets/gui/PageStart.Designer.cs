using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace nets.gui
{
    partial class PageStart : Panel
    {
        private GroupBox gbx_ProfileList;
        private GroupBox gbx_ProfileDetails;
        private DataGridView dgv_profileList;
        private DataGridViewTextBoxColumn col_ProfileName;
        private DataGridViewButtonColumn col_RunProfile;
        private Button btn_NewProfile;
        private Button btn_DeleteProfile;
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageStart));
            
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            
            this.gbx_ProfileDetails = new GroupBox();
            this.gbx_ProfileList = new GroupBox();
            
            this.dgv_profileList = new DataGridView();
            this.col_ProfileName = new DataGridViewTextBoxColumn();
            this.col_RunProfile = new DataGridViewButtonColumn();
            
            btn_NewProfile = new Button();
            btn_DeleteProfile = new Button();
            
            this.gbx_ProfileList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_profileList)).BeginInit();
            this.SuspendLayout();
            // 
            // gbx_ProfileDetails
            // 
            this.gbx_ProfileDetails.BackColor = SystemColors.Control;
            this.gbx_ProfileDetails.Font = new Font("Cambria", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.gbx_ProfileDetails.ForeColor = Color.Black;
            this.gbx_ProfileDetails.Location = new Point(345, 2);
            this.gbx_ProfileDetails.Name = "gbx_ProfileDetails";
            this.gbx_ProfileDetails.Size = new Size(465, 355);
            this.gbx_ProfileDetails.TabIndex = 0;
            this.gbx_ProfileDetails.TabStop = false;
            this.gbx_ProfileDetails.Text = "PROFILE DETAILS";
            this.gbx_ProfileDetails.UseCompatibleTextRendering = true;
            // 
            // gbx_ProfileList
            // 
            this.gbx_ProfileList.BackColor = SystemColors.Control;
            this.gbx_ProfileList.Controls.Add(this.dgv_profileList);
            this.gbx_ProfileList.Controls.Add(this.btn_NewProfile);
            this.gbx_ProfileList.Controls.Add(this.btn_DeleteProfile);
            this.gbx_ProfileList.Font = new Font("Cambria", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.gbx_ProfileList.ForeColor = Color.Black;
            this.gbx_ProfileList.Location = new Point(3, 2);
            this.gbx_ProfileList.Name = "gbx_ProfileList";
            this.gbx_ProfileList.Size = new Size(336, 355);
            this.gbx_ProfileList.TabIndex = 0;
            this.gbx_ProfileList.TabStop = false;
            this.gbx_ProfileList.Text = "PROFILE LIST";
            this.gbx_ProfileList.UseCompatibleTextRendering = true;
            // 
            // dgv_profileList
            // 
            this.dgv_profileList.AllowUserToAddRows = false;
            this.dgv_profileList.AllowUserToDeleteRows = false;
            this.dgv_profileList.AllowUserToResizeColumns = false;
            this.dgv_profileList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dgv_profileList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_profileList.BackgroundColor = Color.White;
            this.dgv_profileList.BorderStyle = BorderStyle.None;
            this.dgv_profileList.CellBorderStyle = DataGridViewCellBorderStyle.RaisedHorizontal;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Cambria", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.ControlDark;
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            this.dgv_profileList.Columns.AddRange(new DataGridViewColumn[] {
            this.col_ProfileName,
            this.col_RunProfile});
            this.dgv_profileList.GridColor = Color.White;
            this.dgv_profileList.Location = new Point(6, 28);
            this.dgv_profileList.MultiSelect = false;
            this.dgv_profileList.Name = "dgv_profileList";
            this.dgv_profileList.ReadOnly = true;
            this.dgv_profileList.RowHeadersVisible = false;
            this.dgv_profileList.ColumnHeadersVisible = false;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.dgv_profileList.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_profileList.RowTemplate.DefaultCellStyle.BackColor = Color.White;
            this.dgv_profileList.RowTemplate.DefaultCellStyle.SelectionBackColor = SystemColors.ControlDark;
            this.dgv_profileList.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.White;
            this.dgv_profileList.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dgv_profileList.RowTemplate.Height = 60;
            this.dgv_profileList.RowTemplate.ReadOnly = true;
            this.dgv_profileList.RowTemplate.Resizable = DataGridViewTriState.False;
            this.dgv_profileList.ScrollBars = ScrollBars.Vertical;
            this.dgv_profileList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //this.dgv_profileList.Size = new Size(324, 320);
            this.dgv_profileList.Size = new Size(324, 280);
            this.dgv_profileList.TabIndex = 0;
            this.dgv_profileList.CellClick += new DataGridViewCellEventHandler(this.dgv_profileList_CellClick);
            this.dgv_profileList.CellEnter += new DataGridViewCellEventHandler(this.dgv_profileList_CellEnter);
            this.dgv_profileList.CellContentClick += new DataGridViewCellEventHandler(this.dgv_profileList_CellContentClick);
            //this.dgv_profileList.CellMouseDown += this.dgv_CellMouseDown;
            //this.dgv_profileList.CellMouseEnter += this.dgv_CellMouseEnter;
            //this.dgv_profileList.CellMouseLeave += this.dgv_CellMouseLeave;
            //this.dgv_profileList.CellMouseUp += this.dgv_CellMouseUp;
            // 
            // col_ProfileName
            // 
            this.col_ProfileName.HeaderText = "Profile Name";
            this.col_ProfileName.Name = "col_ProfileName";
            this.col_ProfileName.ReadOnly = true;
            this.col_ProfileName.Width = 224;


            //nhattao
            // 
            // col_RunProfile
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle3.NullValue")));
            dataGridViewCellStyle3.Padding = new Padding(10, 16, 32, 16);
            dataGridViewCellStyle3.Font = new Font("Cambria", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.col_RunProfile.Tag = false;
            this.col_RunProfile.ToolTipText = "Run";
            this.col_RunProfile.Text = "  Run";
            this.col_RunProfile.UseColumnTextForButtonValue = true;
            this.col_RunProfile.FlatStyle = FlatStyle.Standard;
            this.col_RunProfile.DefaultCellStyle = dataGridViewCellStyle3;
            this.col_RunProfile.HeaderText = "";
            this.col_RunProfile.Name = "col_RunProfile";
            this.col_RunProfile.ReadOnly = true;
            this.col_RunProfile.Resizable = DataGridViewTriState.False;
            this.col_RunProfile.SortMode = DataGridViewColumnSortMode.Automatic;
            this.col_RunProfile.ToolTipText = "Run Profile";
            this.col_RunProfile.Width = 100;

            //nhat tao
            /*
            // 
            // col_DeleteProfile
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle4.NullValue")));
            dataGridViewCellStyle4.Padding = new Padding(3, 15, 23, 15);
            this.col_DeleteProfile.ToolTipText = "Delete";
            this.col_DeleteProfile.Text = "Del";
            this.col_DeleteProfile.UseColumnTextForButtonValue = true;
            this.col_DeleteProfile.FlatStyle = FlatStyle.Standard;
            this.col_DeleteProfile.DefaultCellStyle = dataGridViewCellStyle4;
            this.col_DeleteProfile.HeaderText = "";
            this.col_DeleteProfile.Name = "col_DeleteProfile";
            this.col_DeleteProfile.ReadOnly = true;
            this.col_DeleteProfile.Resizable = DataGridViewTriState.False;
            this.col_DeleteProfile.SortMode = DataGridViewColumnSortMode.Automatic;
            this.col_DeleteProfile.ToolTipText = "Run Profile";
            this.col_DeleteProfile.Width = 60;
            */
            /*
            // 
            // col_DeleteProfile
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle4.NullValue")));
            this.col_DeleteProfile.DefaultCellStyle = dataGridViewCellStyle4;
            this.col_DeleteProfile.HeaderText = "Del";
            this.col_DeleteProfile.Name = "col_DeleteProfile";
            this.col_DeleteProfile.ReadOnly = true;
            this.col_DeleteProfile.Resizable = DataGridViewTriState.False;
            this.col_DeleteProfile.SortMode = DataGridViewColumnSortMode.Automatic;
            this.col_DeleteProfile.ToolTipText = "Run Profile";
            this.col_DeleteProfile.Width = 60;
            */


            // 
            // btn_NewProfile
            // 
            btn_NewProfile.Location = new Point(226, 317);
            btn_NewProfile.Name = "btn_NewProfile";
            btn_NewProfile.Size = new Size(105, 27);
            btn_NewProfile.Text = "New Profile";
            btn_NewProfile.Click += btn_NewProfile_Click;
            btn_NewProfile.TabIndex = 1;
            btn_NewProfile.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btn_NewProfile.UseVisualStyleBackColor = true;

            // 
            // btn_DeleteProfile
            // 
            btn_DeleteProfile.Location = new Point(115, 317);
            btn_DeleteProfile.Name = "btn_DeleteProfile";
            btn_DeleteProfile.Size = new Size(105, 27);
            btn_DeleteProfile.Text = "Delete Profile";
            btn_DeleteProfile.Click += btn_DeleteProfile_Click;
            btn_DeleteProfile.TabIndex = 2;
            btn_DeleteProfile.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btn_DeleteProfile.UseVisualStyleBackColor = true;


            // 
            // PageStart
            // 
            this.BackColor = SystemColors.Control;
            this.Controls.Add(this.gbx_ProfileDetails);
            this.Controls.Add(this.gbx_ProfileList);
            this.ForeColor = Color.Black;
            this.Location = new Point(12, 73);
            this.Margin = new Padding(0);
            this.Name = "PageStart";
            this.Size = new Size(814, 365);
            this.TabIndex = 1;
            this.gbx_ProfileList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_profileList)).EndInit();
            gbx_ProfileList.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
