using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using nets_wpf.DataStructures;
using nets_wpf.Logic;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for PageProfileList.xaml
    /// </summary>
    public partial class PageProfileList : UserControl
    {
        private List<string> profileList = new List<string>();
        private string[] profileNameList;
        private readonly string folderPath;

        public PageProfileList()
        {
            InitializeComponent();
        }

        public PageProfileList( double top,double left)
        {
            InitializeComponent();
        }

        public PageProfileList(string folderPath, ref List<string> profileNameList, ref List<string> profileList)
        {
            this.folderPath = folderPath;
            InitializeComponent();
            lb_AskUser.Text = "Multiple profiles containing " + folderPath + " is detected. Which profile do you want to sync?";
            this.profileNameList = profileNameList.ToArray();
            for (int i = 0; i < profileList.Count; i++)
              lbx_ProfileList.Items.Add(profileList[i] );

            lbx_ProfileList.SelectedIndex = 0;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            int selectedProfileIndex = lbx_ProfileList.SelectedIndex;
            Profile profile = LogicFacade.LoadProfile(profileNameList[selectedProfileIndex].Trim());
            string correspondingFolderPath = PathOperator.GetCorrespondingPath(profile.SrcFolder, profile.DesFolder, this.folderPath);
            string normalizedCorrespondingFolderPath = PathOperator.NormalizeFolderPath(correspondingFolderPath);
            LogicFacade.EnqueueSyncJob(this.folderPath, normalizedCorrespondingFolderPath, profile.SyncMode, profile.IncludePattern, profile.ExcludePattern);
            Window.GetWindow(this).Close();
        }

        private void btn_Abort_Click(object sender, RoutedEventArgs e)
        {
            JobQueueHandler.DeleteRightClickSyncFile();
            Window.GetWindow(this).Close();
        }
    }
}
