using System.Windows.Forms;
using System.Diagnostics;

namespace nets.gui
{
    /// <summary>
    /// Display information about developer team
    /// Author: Nguyen Thi Yen Duong
    /// </summary>
    public partial class PageAboutUs
    {
        #region CONSTRUCTORS

        public PageAboutUs()
        {
            InitializeComponent();
        }

        #endregion

        #region COMPONENT EVENT HANDLERS

        private void llb_GoogleHost_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.nothing-else-to-sync.info");
        }

        private void llb_NETDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.microsoft.com/downloads/details.aspx?FamilyId=333325FD-AE52-4E35-B531-508D977D32A6&displaylang=en");
        }

        #endregion
    }
}
