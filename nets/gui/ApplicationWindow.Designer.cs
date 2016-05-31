using System.Collections.Generic;
using System.Drawing;
using nets.storage;
using System.Windows.Forms;
using System.Threading;

namespace nets.gui
{
    partial class ApplicationWindow
    {
        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationWindow));
            toolStrip = new ToolStrip();
            statusBar = new StatusStrip();
            lb_ProgramStatus = new ToolStripStatusLabel();

            btn_TabProfiles = new Button();
            btn_TabIVLE = new Button();
            btn_TabSettings = new Button();
            btn_TabHelp = new Button();
            btn_TabAboutUs = new Button();
            btn_Exit = new Button();

            tabControl = new TabControl();
            tab_PageStart = new TabPage();
            tab_PageIVLE = new TabPage();
            tab_PageSettings = new TabPage();
            tab_PageAboutUs = new TabPage();

            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.AutoSize = false;
            toolStrip.BackColor = System.Drawing.Color.Black;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip.Location = new System.Drawing.Point(0, 0);
            toolStrip.MaximumSize = new System.Drawing.Size(850, 70);
            toolStrip.MinimumSize = new System.Drawing.Size(850, 70);
            toolStrip.Name = "toolStrip";
            toolStrip.RenderMode = ToolStripRenderMode.Professional;
            toolStrip.Size = new System.Drawing.Size(850, 70);
            toolStrip.Stretch = true;
            toolStrip.TabIndex = 0;
            toolStrip.TabStop = true;
            toolStrip.Text = "toolStrip1";
            toolStrip.Visible = false;
            // 
            // tabControl
            // 
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new System.Drawing.Size(0, 1);
            tabControl.Location = new System.Drawing.Point(12, 83);
            tabControl.Margin = new Padding(0);
            tabControl.Name = "tabControl";
            tabControl.Padding = new System.Drawing.Point(0, 0);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(828, 323);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.TabIndex = 0;
            // 
            // tab_PageStart
            // 
            tab_PageStart.Location = new System.Drawing.Point(4, 22);
            tab_PageStart.Name = "tab_PageStart";
            tab_PageStart.Padding = new Padding(3);
            tab_PageStart.Size = new System.Drawing.Size(258, 224);
            tab_PageStart.TabIndex = 0;
            tab_PageStart.Text = "tab_PageStart";
            tab_PageStart.UseVisualStyleBackColor = true;
            //
            // tab_PageIVLE
            //
            tab_PageIVLE.Location = new System.Drawing.Point(4, 22);
            tab_PageIVLE.Name = "tab_PageIVLE";
            tab_PageIVLE.Padding = new Padding(3);
            tab_PageIVLE.Size = new Size(258, 224);
            tab_PageIVLE.TabIndex = 1;
            tab_PageIVLE.Text = "tab_PageIVLE";
            tab_PageIVLE.UseVisualStyleBackColor = true;
            // 
            // tab_PageSettings
            // 
            tab_PageSettings.Location = new System.Drawing.Point(4, 22);
            tab_PageSettings.Name = "tab_PageSettings";
            tab_PageSettings.Padding = new Padding(3);
            tab_PageSettings.Size = new System.Drawing.Size(192, 74);
            tab_PageSettings.TabIndex = 2;
            tab_PageSettings.Text = "tab_PageSettings";
            tab_PageSettings.UseVisualStyleBackColor = true;
            // 
            // tab_PageAboutUs
            // 
            tab_PageAboutUs.Location = new System.Drawing.Point(4, 22);
            tab_PageAboutUs.Name = "tab_PageAboutUs";
            tab_PageAboutUs.Padding = new Padding(3);
            tab_PageAboutUs.Size = new System.Drawing.Size(192, 74);
            tab_PageAboutUs.TabIndex = 2;
            tab_PageAboutUs.Text = "tab_PageAboutUs";
            tab_PageAboutUs.UseVisualStyleBackColor = true;

            // 
            // statusBar
            // 
            statusBar.AutoSize = false;
            statusBar.Items.AddRange(new ToolStripItem[] {
            lb_ProgramStatus});
            statusBar.Location = new System.Drawing.Point(0, 422);
            statusBar.Name = "statusBar";
            statusBar.MaximumSize = new System.Drawing.Size(852, 70);
            statusBar.MinimumSize = new System.Drawing.Size(852, 70);
            statusBar.Size = new System.Drawing.Size(852, 70);
            statusBar.TabIndex = 6;
            statusBar.Text = "statusBar";
            statusBar.AllowDrop = true;
            statusBar.DragEnter += dragEnter;
            statusBar.DragDrop += DragDropFoldersToSync;

            // 
            // lb_ProgramStatus
            // 
            lb_ProgramStatus.BackColor = System.Drawing.SystemColors.Control;
            lb_ProgramStatus.Alignment = ToolStripItemAlignment.Left;
            lb_ProgramStatus.Font = new System.Drawing.Font("Cambria", 10.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lb_ProgramStatus.Name = "lb_ProgramStatus";
            lb_ProgramStatus.Size = new System.Drawing.Size(60, 76);
            lb_ProgramStatus.Text = "";
            lb_ProgramStatus.Text += "   Drag and drop your folders here to sync!";   
                              
            // 
            // btn_TabIVLE
            // 
            btn_TabIVLE.BackgroundImageLayout = ImageLayout.None;
            btn_TabIVLE.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_TabIVLE.Image = global::nets.Properties.Resources.btn_TabIVLE;
            btn_TabIVLE.Location = new System.Drawing.Point(149, 11);
            btn_TabIVLE.Name = "btn_PageIVLE";
            btn_TabIVLE.Size = new System.Drawing.Size(133, 60);
            btn_TabIVLE.TabIndex = 7;
            btn_TabIVLE.Text = "IVLE ";
            btn_TabIVLE.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_TabIVLE.UseVisualStyleBackColor = true;
            btn_TabIVLE.Click += new System.EventHandler(btn_TabIVLE_Click);
            // 
            // btn_Exit
            // 
            btn_Exit.BackgroundImageLayout = ImageLayout.None;
            btn_Exit.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_Exit.Image = ((System.Drawing.Image)(resources.GetObject("btn_Exit.Image")));
            btn_Exit.Location = new System.Drawing.Point(705, 11);
            btn_Exit.Name = "btn_Exit";
            btn_Exit.Size = new System.Drawing.Size(133, 60);
            btn_Exit.TabIndex = 5;
            btn_Exit.Text = "Exit";
            btn_Exit.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_Exit.UseVisualStyleBackColor = true;
            btn_Exit.Click += new System.EventHandler(btn_Exit_Click);
            // 
            // btn_TabAboutUs
            // 
            btn_TabAboutUs.BackgroundImageLayout = ImageLayout.None;
            btn_TabAboutUs.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //btn_TabAboutUs.Image = ((System.Drawing.Image)(resources.GetObject("btn_TabAboutUs.Image")));
            btn_TabAboutUs.Image = global::nets.Properties.Resources.netsLogo;
            btn_TabAboutUs.Location = new System.Drawing.Point(566, 11);
            btn_TabAboutUs.Name = "btn_TabAbout";
            btn_TabAboutUs.Size = new System.Drawing.Size(133, 60);
            btn_TabAboutUs.TabIndex = 4;
            btn_TabAboutUs.Text = "About";
            btn_TabAboutUs.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_TabAboutUs.UseVisualStyleBackColor = true;
            btn_TabAboutUs.Click += new System.EventHandler(btn_TabAboutUs_Click);
            // 
            // btn_TabHelp
            // 
            btn_TabHelp.BackgroundImageLayout = ImageLayout.None;
            btn_TabHelp.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_TabHelp.Image = ((System.Drawing.Image)(resources.GetObject("btn_TabHelp.Image")));
            btn_TabHelp.Location = new System.Drawing.Point(427, 11);
            btn_TabHelp.Name = "btn_TabHelp";
            btn_TabHelp.Size = new System.Drawing.Size(133, 60);
            btn_TabHelp.TabIndex = 3;
            btn_TabHelp.Text = "Help";
            btn_TabHelp.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_TabHelp.UseVisualStyleBackColor = true;
            btn_TabHelp.Click += new System.EventHandler(btn_TabHelp_Click);
            // 
            // btn_TabSettings
            // 
            btn_TabSettings.BackgroundImageLayout = ImageLayout.None;
            btn_TabSettings.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_TabSettings.Image = global::nets.Properties.Resources.btn_TabSettings;
            btn_TabSettings.Location = new System.Drawing.Point(288, 11);
            btn_TabSettings.Name = "btn_TabSettings";
            btn_TabSettings.Size = new System.Drawing.Size(133, 60);
            btn_TabSettings.TabIndex = 2;
            btn_TabSettings.Text = "Settings";
            btn_TabSettings.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_TabSettings.UseVisualStyleBackColor = true;
            btn_TabSettings.Click += new System.EventHandler(btn_TabSettings_Click);
            // 
            // btn_TabProfiles
            // 
            btn_TabProfiles.BackgroundImageLayout = ImageLayout.None;
            btn_TabProfiles.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_TabProfiles.Image = global::nets.Properties.Resources.btn_TabProfiles;
            btn_TabProfiles.Location = new System.Drawing.Point(10, 11);
            btn_TabProfiles.Name = "btn_TabProfiles";
            btn_TabProfiles.Size = new System.Drawing.Size(133, 60);
            btn_TabProfiles.TabIndex = 1;
            btn_TabProfiles.Text = "Profiles";
            btn_TabProfiles.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_TabProfiles.UseVisualStyleBackColor = true;
            btn_TabProfiles.Click += new System.EventHandler(btn_TabProfiles_Click);
            // 
            // ApplicationWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlDark;
            ClientSize = new System.Drawing.Size(852, 503);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Controls.Add(btn_TabIVLE);
            Controls.Add(statusBar);
            Controls.Add(btn_Exit);
            Controls.Add(btn_TabAboutUs);
            Controls.Add(btn_TabHelp);
            Controls.Add(btn_TabSettings);
            Controls.Add(btn_TabProfiles);
            Controls.Add(tabControl);
            Controls.Add(toolStrip);
            ForeColor = System.Drawing.Color.Black;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = ((System.Drawing.Icon)(resources.GetObject("nets")));
            MaximizeBox = false;
            Name = "ApplicationWindow";
            Text = "NETS (Nothing Else To Sync)";
            this.FormClosing += new FormClosingEventHandler(ApplicationWindow_FormClosing);
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
        }

        private ToolStrip toolStrip;
        private StatusStrip statusBar;
        private ToolStripStatusLabel lb_ProgramStatus;

        private TabControl tabControl;
        private TabPage tab_PageStart;
        private TabPage tab_PageIVLE;
        private TabPage tab_PageSettings;
        private TabPage tab_PageAboutUs;

        private Button btn_TabProfiles;
        private Button btn_TabIVLE;
        private Button btn_TabSettings;
        private Button btn_TabHelp;
        private Button btn_TabAboutUs;
        private Button btn_Exit;

        #endregion
    }
}