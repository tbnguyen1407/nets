using System;
using System.Drawing;
using System.Windows.Forms;
using nets.logic;
using nets.utility;

namespace nets.gui
{
    /// <summary>
    /// Display the settings of NETS
    /// Author: Tran Binh Nguyen + Hoang Nguyen Nhat Tao
    /// </summary>
    public class PageSettings : Panel
    {
        #region PRIVATE FIELDS

        private CheckBox cbx_SettingAutoComplete;
        private CheckBox cbx_SettingContextMenu;
        private CheckBox cbx_SettingDeleteToRecycleBin;
        private Button btn_Apply;
        private Button btn_SelectAll;
        private Button btn_SelectNone;
        private GroupBox gbx_AllButtons;
        private PictureBox pcb_ContextMenuPicture;
        private PictureBox pcb_SrcDesPicture;
        private PictureBox pcb_ProfileNamePicture;
        private PictureBox pcb_NextPicture;

        #endregion

        #region CONSTRUCTORS

        public PageSettings()
        {
            InitializeComponent();

            InitializeSettingAutoComplete();
            InitializeSettingContextMenu();
            InitializeSettingDeleteToRecycleBin();
        }
 
        #endregion

        #region INITIALIZE

        private void InitializeComponent()
        {
            cbx_SettingAutoComplete = new CheckBox();
            cbx_SettingContextMenu = new CheckBox();
            cbx_SettingDeleteToRecycleBin = new CheckBox();
            btn_Apply = new Button();
            btn_SelectAll = new Button();
            btn_SelectNone = new Button();
            gbx_AllButtons = new GroupBox();
            pcb_ContextMenuPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcb_ContextMenuPicture)).BeginInit();
            pcb_SrcDesPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcb_SrcDesPicture)).BeginInit();
            pcb_ProfileNamePicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcb_ProfileNamePicture)).BeginInit();
            pcb_NextPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcb_NextPicture)).BeginInit();
            SuspendLayout();
            // 
            // cbx_SettingAutoComplete
            // 
            cbx_SettingAutoComplete.Font = new Font("Cambria", 10.75F, FontStyle.Bold);
            cbx_SettingAutoComplete.Location = new Point(42, 10);
            cbx_SettingAutoComplete.Name = "cbx_SettingAutoComplete";
            cbx_SettingAutoComplete.Size = new Size(400, 50);
            cbx_SettingAutoComplete.TabIndex = 1;
            cbx_SettingAutoComplete.Text = "Enable suggestion mode for profile name";
            cbx_SettingAutoComplete.TextImageRelation = TextImageRelation.ImageBeforeText;
            cbx_SettingAutoComplete.UseVisualStyleBackColor = true;
            // 
            // cbx_SettingContextMenu
            // 
            cbx_SettingContextMenu.Font = new Font("Cambria", 10.75F, FontStyle.Bold);
            cbx_SettingContextMenu.Location = new Point(42, 125);
            cbx_SettingContextMenu.Name = "cbx_SettingContextMenu";
            cbx_SettingContextMenu.Size = new Size(400, 50);
            cbx_SettingContextMenu.TabIndex = 1;
            cbx_SettingContextMenu.Text = "Enable Context Menu Entries";
            cbx_SettingContextMenu.TextImageRelation = TextImageRelation.ImageBeforeText;
            cbx_SettingContextMenu.UseVisualStyleBackColor = true;
            // 
            // cbx_SettingDeleteToRecycleBin
            // 
            cbx_SettingDeleteToRecycleBin.Font = new Font("Cambria", 10.75F, FontStyle.Bold);
            cbx_SettingDeleteToRecycleBin.Location = new Point(42, 293);
            cbx_SettingDeleteToRecycleBin.Name = "cbx_DeleteToRecycleBin";
            cbx_SettingDeleteToRecycleBin.Size = new Size(400, 50);
            cbx_SettingDeleteToRecycleBin.TabIndex = 1;
            cbx_SettingDeleteToRecycleBin.Text = "Delete file to Recycle Bin (Safe Delete)";
            cbx_SettingDeleteToRecycleBin.TextImageRelation = TextImageRelation.ImageBeforeText;
            cbx_SettingDeleteToRecycleBin.UseVisualStyleBackColor = true;
            // 
            // btn_ApplySettings
            // 
            //btn_Apply.Location = new Point(583, 195);
            btn_Apply.Location = new Point(65, 68);
            btn_Apply.Name = "btn_Apply";
            btn_Apply.Size = new Size(90, 27);
            btn_Apply.Text = "Apply";
            btn_Apply.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_Apply.UseVisualStyleBackColor = true;
            btn_Apply.Click += BtnApplyClick;
            // 
            // btn_SelectAll
            // 
            //btn_SelectAll.Location = new Point(530, 195);
            btn_SelectAll.Location = new Point(10, 25);
            btn_SelectAll.Name = "btn_Apply";
            btn_SelectAll.Size = new Size(90, 27);
            btn_SelectAll.Text = "Select All";
            btn_SelectAll.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_SelectAll.UseVisualStyleBackColor = true;
            btn_SelectAll.Click += BtnSelectAllClick;
            // 
            // btn_SelectNone
            // 
            //btn_SelectNone.Location = new Point(640, 195);
            btn_SelectNone.Location = new Point(120, 25);
            btn_SelectNone.Name = "btn_Apply";
            btn_SelectNone.Size = new Size(90, 27);
            btn_SelectNone.Text = "Select None";
            btn_SelectNone.Font = new Font("Cambria", 10.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            btn_SelectNone.UseVisualStyleBackColor = true;
            btn_SelectNone.Click += BtnSelectNoneClick;
            //
            // gbx_AllButtons
            //
            gbx_AllButtons.Location = new Point(460, 170);
            gbx_AllButtons.Size = new Size(220, 107);
            gbx_AllButtons.Text = "Actions";
            gbx_AllButtons.Font = new Font("Cambria", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            gbx_AllButtons.UseCompatibleTextRendering = true;
            // 
            // pcb_ContextMenuPicture
            // 
            this.pcb_ContextMenuPicture.Image = Properties.Resources.rightClickMenu;
            this.pcb_ContextMenuPicture.Location = new System.Drawing.Point(42, 177);
            this.pcb_ContextMenuPicture.Name = "pcb_ContextMenuPicture";
            this.pcb_ContextMenuPicture.Size = new System.Drawing.Size(320, 100);
            this.pcb_ContextMenuPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pcb_ContextMenuPicture.TabIndex = 6;
            this.pcb_ContextMenuPicture.TabStop = false;
            // 
            // pcb_SrcDesPicture
            // 
            this.pcb_SrcDesPicture.Image = Properties.Resources.SrcDes;
            this.pcb_SrcDesPicture.Location = new System.Drawing.Point(40, 55);
            this.pcb_SrcDesPicture.Name = "pcb_ContextMenuPicture";
            this.pcb_SrcDesPicture.Size = new System.Drawing.Size(320, 90);
            this.pcb_SrcDesPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pcb_SrcDesPicture.TabIndex = 6;
            this.pcb_SrcDesPicture.TabStop = false;
            // 
            // pcb_ProfileNamePicture
            // 
            this.pcb_ProfileNamePicture.Image = global::nets.Properties.Resources.profileName;
            this.pcb_ProfileNamePicture.Location = new System.Drawing.Point(455, 73);
            this.pcb_ProfileNamePicture.Name = "pcb_ContextMenuPicture";
            this.pcb_ProfileNamePicture.Size = new System.Drawing.Size(320, 70);
            this.pcb_ProfileNamePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pcb_ProfileNamePicture.TabIndex = 6;
            this.pcb_ProfileNamePicture.TabStop = false;
            // 
            // pcb_NextPicture
            // 
            this.pcb_NextPicture.Image = global::nets.Properties.Resources.next;
            this.pcb_NextPicture.Location = new System.Drawing.Point(387, 60);
            this.pcb_NextPicture.Name = "pcb_ContextMenuPicture";
            this.pcb_NextPicture.Size = new System.Drawing.Size(90, 75);
            this.pcb_NextPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pcb_NextPicture.TabIndex = 6;
            this.pcb_NextPicture.TabStop = false;
            // 
            // PageSettings
            // 
            BackColor = SystemColors.Control;
            BorderStyle = BorderStyle.Fixed3D;
            ForeColor = Color.Black;
            Location = new Point(6, 6);
            Margin = new Padding(0);
            Size = new Size(803, 345);

            Controls.Add(cbx_SettingAutoComplete);
            Controls.Add(cbx_SettingContextMenu);
            Controls.Add(cbx_SettingDeleteToRecycleBin);
            gbx_AllButtons.Controls.Add(btn_Apply);
            gbx_AllButtons.Controls.Add(btn_SelectAll);
            gbx_AllButtons.Controls.Add(btn_SelectNone);
            Controls.Add(gbx_AllButtons);
            Controls.Add(this.pcb_ContextMenuPicture);
            Controls.Add(this.pcb_SrcDesPicture);
            Controls.Add(this.pcb_ProfileNamePicture);
            Controls.Add(this.pcb_NextPicture);

            ResumeLayout(false);
        }

        #endregion

        #region EVENT HANDLER DECLARATIONS

        private void BtnApplyClick(object sender, EventArgs e)
        {
            btn_Apply.Enabled = false;
            bool addSuccess = ApplySettingContextMenu();
            ApplySettingAutoComplete();
            ApplySettingDeleteToRecycleBin();
            btn_Apply.Enabled = true;

            if (addSuccess)
                MessageBox.Show("Settings successfully applied!",
                                "Successful",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            
        }

        private void BtnSelectAllClick(object sender, EventArgs e)
        {
            cbx_SettingAutoComplete.Checked = true;
            cbx_SettingContextMenu.Checked = true;
            cbx_SettingDeleteToRecycleBin.Checked = true;
        }

        private void BtnSelectNoneClick(object sender, EventArgs e)
        {
            cbx_SettingAutoComplete.Checked = false;
            cbx_SettingContextMenu.Checked = false;
            cbx_SettingDeleteToRecycleBin.Checked = false;
        }

        #endregion

        #region HELPERS

        private void InitializeSettingAutoComplete()
        {
            bool settingAutoComplete = LogicFacade.LoadSetting("autocomplete");
            cbx_SettingAutoComplete.Checked = settingAutoComplete;
        }

        private void InitializeSettingContextMenu()
        {
            bool settingContextMenu = LogicFacade.LoadSetting("contextmenu");
            cbx_SettingContextMenu.Checked = settingContextMenu;

            // Register/unregister context menu entries only if necessary
            if (settingContextMenu && !RegistryOperator.ContextMenuRegistryExists())
                RegistryOperator.RegisterContextMenu();
            if (!settingContextMenu && RegistryOperator.ContextMenuRegistryExists())
                RegistryOperator.UnregisterContextMenu();

        }

        private void InitializeSettingDeleteToRecycleBin()
        {
            bool settingDeleteToRecycleBin = LogicFacade.LoadSetting("deletetorecyclebin");
            cbx_SettingDeleteToRecycleBin.Checked = settingDeleteToRecycleBin;

        }

        private void ApplySettingAutoComplete()
        {
            PageProfileDetails.AutoCompleteProfileName = cbx_SettingAutoComplete.Checked;
            LogicFacade.SaveSetting("autocomplete", cbx_SettingAutoComplete.Checked ? "1" : "0");
        }

        private bool ApplySettingContextMenu()
        {
            bool operationSuccess = true;

            if (cbx_SettingContextMenu.Checked && !RegistryOperator.ContextMenuRegistryExists())
            {
                operationSuccess = RegistryOperator.RegisterContextMenu();
                if (!operationSuccess)
                {
                    MessageBox.Show("Sorry! We do not have enough privilege to access your registry!\n\r" +
                                    "NETS context menu entries can not be added.\n\r",
                                    "Fail to add context menu entries",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    cbx_SettingContextMenu.Checked = false;
                }
                LogicFacade.SaveSetting("contextmenu", (operationSuccess) ? "1" : "0");
            }
            else if (!cbx_SettingContextMenu.Checked && RegistryOperator.ContextMenuRegistryExists())
            {
                operationSuccess = RegistryOperator.UnregisterContextMenu();
                if (!operationSuccess)
                {
                    MessageBox.Show("Sorry! We do not have enough privilege to access your registry!\n\r" +
                                    "NETS context menu entries can not be removed.\n\r",
                                    "Fail to remove context menu entries",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    cbx_SettingContextMenu.Checked = true;
                }
                LogicFacade.SaveSetting("contextmenu", (operationSuccess) ? "0" : "1");
            }

            return operationSuccess;
        }

        private void ApplySettingDeleteToRecycleBin()
        {
            LogicFacade.SaveSetting("deletetorecyclebin", cbx_SettingDeleteToRecycleBin.Checked ? "1" : "0");
        }

        #endregion
    }
}