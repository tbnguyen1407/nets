using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using nets.dataclass;
using nets.utility;

namespace nets.gui
{
    /// <summary>
    /// Main application window
    /// Author: Hoang Nguyen Nhat Tao + Tran Binh Nguyen
    /// </summary>
    public partial class ApplicationWindow : Form
    {
        #region PRIVATE FIELDS

        private readonly PageStart panel_PageStart = new PageStart();
        private readonly PageIVLE panel_PageIVLE = new PageIVLE();
        private readonly PageSettings panel_PageSettings = new PageSettings();
        private readonly PageAboutUs panel_PageAboutUs = new PageAboutUs();
        private static Thread thread;

        #endregion

        #region PUBLIC CONSTRUCTORS

        public ApplicationWindow()
        {
            JobQueueHandler.CreateMainFile();
            InitializeComponent();
            AddPageToTabControl();
        }

        public ApplicationWindow(PageStart panel_PageStart, PageIVLE panel_PageIVLE, PageSettings panel_PageSettings)
        {
            JobQueueHandler.CreateMainFile();
            InitializeComponent();
            
            this.panel_PageStart = panel_PageStart;
            this.panel_PageIVLE = panel_PageIVLE;
            this.panel_PageSettings = panel_PageSettings;
            AddPageToTabControl();

            this.ClientSize = new Size(tabControl.Size.Width + 24, tabControl.Size.Height + this.toolStrip.Height + 96); //26
        }

        #endregion

        #region PRIVATE HELPERS 

        private void AddPageToTabControl()
        {
            panel_PageStart.Location = new Point(1, 0);
            panel_PageIVLE.Location = new Point(1, 0);
            panel_PageSettings.Location = new Point(1, 0);
            panel_PageAboutUs.Location = new Point(1, 0);

            panel_PageIVLE.Size = panel_PageStart.Size;
            panel_PageSettings.Size = panel_PageStart.Size;
            panel_PageAboutUs.Size = panel_PageStart.Size;
            tabControl.ClientSize = new Size(panel_PageStart.Size.Width + 10, panel_PageStart.Size.Height + 5);
            
            tab_PageStart.Controls.Add(panel_PageStart);
            tab_PageIVLE.Controls.Add(panel_PageIVLE);
            tab_PageSettings.Controls.Add(panel_PageSettings);
            tab_PageAboutUs.Controls.Add(panel_PageAboutUs);

            tabControl.Controls.Add(tab_PageStart);
            tabControl.Controls.Add(tab_PageIVLE);
            tabControl.Controls.Add(tab_PageSettings);
            tabControl.Controls.Add(tab_PageAboutUs);

            Controls.Add(tabControl);
        }

        private void SwitchToTab(Tab tab_Page)
        {
            switch (tab_Page)
            {
                case Tab.PageProfiles:
                    this.tabControl.SelectedTab = tab_PageStart;
                    break;
                case Tab.PageIVLE:
                    this.tabControl.SelectedTab = tab_PageIVLE;
                    break;
                case Tab.PageSettings:
                    this.tabControl.SelectedTab = tab_PageSettings;
                    break;
                case Tab.PageAbout:
                    this.tabControl.SelectedTab = tab_PageAboutUs;
                    break;
            }
        }

        private void btn_TabProfiles_Click(object sender, EventArgs e)
        {
            SwitchToTab(Tab.PageProfiles);
        }

        private void btn_TabIVLE_Click(object sender, EventArgs e)
        {
            SwitchToTab(Tab.PageIVLE);
        }

        private void btn_TabSettings_Click(object sender, EventArgs e)
        {
            SwitchToTab(Tab.PageSettings);
        }

        private void btn_TabHelp_Click(object sender, EventArgs e)
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string helpFilePath = applicationPath + "\\nets.chm";

            if (!File.Exists(helpFilePath))
                MessageBox.Show(@"Help file not found in the application folder!", "Error");
            else
            {
                Process process = new Process {StartInfo = {FileName = helpFilePath}};
                process.Start();
            }
        }

        private void btn_TabAboutUs_Click(object sender, EventArgs e)
        {
            SwitchToTab(Tab.PageAbout);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            DialogResult userResponse = MessageBox.Show("Are you sure you want to exit?",
                                                        "Exit program",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);
            if (userResponse == DialogResult.No) return;
            this.Close();
        }

        private void dragEnter(object sender, DragEventArgs e)
        {
            IDataObject dataObject = e.Data;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                Array a = (Array)dataObject.GetData(DataFormats.FileDrop);
                for (int i = 0; i < a.Length; i++)
                {
                    if (Directory.Exists(a.GetValue(i).ToString()))
                        continue;
                    e.Effect = DragDropEffects.None;
                    return;
                }
                e.Effect = DragDropEffects.All;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void DragDropFoldersToSync(object sender, DragEventArgs e)
        {
            if (JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();

            Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

            if (a == null || a.Length == 0)
                return;

            Activate();
            e.Effect = DragDropEffects.None;

            List<string> folderPathList = new List<string>();
            for (int i = 0; i < a.Length; i++)
                folderPathList.Add(a.GetValue(i).ToString());

            thread = new Thread(() => GUIEventHandler.SyncHandler(ref folderPathList));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ApplicationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            JobQueueHandler.DeleteMainFile();
        }

        #endregion
    }
}
