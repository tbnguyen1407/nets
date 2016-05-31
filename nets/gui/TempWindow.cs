using System.Drawing;
using System.Windows.Forms;
using nets.dataclass;
using nets.utility;

namespace nets.gui
{
    /// <summary>
    /// Temporary window to contain other GUI panels
    /// Author: Hoang Nguyen Nhat Tao + Tran Binh Nguyen
    /// </summary>
    public partial class TempWindow : Form
    {
        #region PRIVATE CONSTRUCTORS

        /// <summary>
        /// Default temp window
        /// </summary>
        private TempWindow()
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
        }

        #endregion

        #region PUBLIC CONSTRUCTORS

        /// <summary>
        /// temp window displaying a given panel
        /// </summary>
        /// <param name="panel">panel to be displayed</param>
        public TempWindow(Panel panel)
            : this()
        {
            ClientSize = new Size(panel.Size.Width, panel.Size.Height);
            Controls.Add(panel);
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Do the clean up resources when temp window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TempWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            JobQueueHandler.DeleteRightClickSyncFile();
            JobQueueHandler.DeleteMainSyncFile();
        }

        #endregion
    }
}
