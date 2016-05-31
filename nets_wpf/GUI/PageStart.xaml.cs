using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Threading;
using nets_wpf.DataStructures;
using nets_wpf.Logic;
using nets_wpf.Storage;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for PageStart.xaml
    /// </summary>
    public delegate void DeleteMessageBoxResult(string folderPath);

    
    public partial class PageStart : System.Windows.Controls.UserControl
    {
        List<string> profileNameList = new List<string>();
        Profile currentProfile = new Profile();
        DataTable db_profileList = new DataTable();
        public static event DeleteMessageBoxResult deleteMessageBoxResult;

        private static Thread thread;

        public PageStart()
        {
            AddColumn();
            PopulateProfileList();
            InitializeComponent();
            DisplayProfileDetails();
        }

        public void Reset()
        {
            pageProfileDetails.exp_advancedInfo.IsExpanded = false;
        }

        private void AddColumn()
        {
            db_profileList.Columns.Add(new System.Data.DataColumn("ProfileName", typeof(string)));
            db_profileList.Columns.Add(new System.Data.DataColumn("ProfileInfo", typeof(string)));
        }

        /// <summary>
        /// Populate profile list with saved profiles
        /// </summary>
        private void PopulateProfileList()
        {
            Profile profile;
            profileNameList = LogicFacade.LoadProfileNameList();
            db_profileList.Clear();

            string profileInfo = "";
            foreach (string profileName in profileNameList)
            {
                profile = LogicFacade.LoadProfile(profileName);
                string formattedSrcPath = TrimPath(profile.SrcFolder, 30);
                string formattedDesPath = TrimPath(profile.DesFolder, 30);

                profileInfo = "   Src:  " + formattedSrcPath + "\n" +
                                      "   Des: " + formattedDesPath;

                DataRow newRow = db_profileList.NewRow();
                newRow["ProfileName"] = "   "+profile.ProfileName;
                newRow["ProfileInfo"] = profileInfo;
                db_profileList.Rows.Add(newRow);
            }
        }

        /// <summary>
        /// Refresh profile list and select the correct profile
        /// </summary>
        public void RefreshProfileList(string profileName)
        {
            PopulateProfileList();
            int selectedIndex = profileNameList.IndexOf(profileName);
            dgv_profileList.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// Display the selected profile details
        /// </summary>
        private void DisplayProfileDetails()
        {
            currentProfile = (profileNameList.Count > 0) ? LogicFacade.LoadProfile(profileNameList[0]) : new Profile();
            LogicFacade.SetCurrentProfile(currentProfile);
            this.pageProfileDetails.SetProfileInfo(currentProfile);
            dgv_profileList.SelectedIndex = (profileNameList.Count > 0) ? 0: -1;
        }

        /// <summary>
        /// Set profile details
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="isNewProfile"></param>
        public void SetProfileDetails(Profile profile, bool isNewProfile)
        {
            if (isNewProfile)
            {
                dgv_profileList.SelectedIndex = -1;
                pageProfileDetails.IsEditing = false;
            }
            else pageProfileDetails.IsEditing = true;
            LogicFacade.SetCurrentProfile(profile);
            pageProfileDetails.SetProfileInfo(profile);
        }

        /// <summary>
        /// Clear profile list
        /// </summary>
        public void ClearDataGridViewSelection()
        {
            dgv_profileList.SelectedIndex = -1;
        }

        private string TrimPath(string path, int length)
        {
            if (path.Length > length)
                path = path.Replace(path.Substring(length / 2 - 1, path.Length - length + 3), "...");
            return path;
        }

        private void dgv_profileList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_profileList_CellEnter(sender, e);
        }

        private void dgv_profileList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int cellRow = e.RowIndex;

            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];
            SetProfileDetails(LogicFacade.LoadProfile(profileName), false);
            this.pageProfileDetails.IsEditing = true;
        }

        public System.Data.DataTable ProfileListView
        {
            get { return db_profileList; } 
        }


        private void dvg_btn_RunProfile_Click(object sender, RoutedEventArgs e)
        {
            int cellRow = dgv_profileList.SelectedIndex;

            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];
            if (!FolderPathIsValid(profileName))
                return;

            if (JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();

            if (!FolderPathIsValid(profileName))
                return;

            thread = new Thread(() => GUIEventHandler.SyncHandler(profileName));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();        ;
        }

        private bool FolderPathIsValid(string profileName)
        {
            Profile profile = StorageFacade.LoadProfile(profileName);
            if (!Directory.Exists(profile.SrcFolder))
            {
                DisplayMessageBox("Error: This profile can not run because its source folder no longer exists!","");
                return false;
            }
            if (!Directory.Exists(profile.DesFolder))
            {
                DisplayMessageBox("Error: This profile can not run because its destination folder no longer exists!","");
                return false;
            }
            return true;
        }

        private void btn_NewProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile newProfile = new Profile();
            LogicFacade.SetCurrentProfile(newProfile);
            pageProfileDetails.SetProfileInfo(newProfile);
            pageProfileDetails.IsEditing = false;
            ClearDataGridViewSelection();
        }

        private void dgv_profileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int cellRow = dgv_profileList.SelectedIndex;
           
            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];
            SetProfileDetails(LogicFacade.LoadProfile(profileName), false);
        }

        private void btn_RunProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();
            
            int cellRow = dgv_profileList.SelectedIndex;

            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];
            if (!FolderPathIsValid(profileName))
                return;

            Thread thread = new Thread(delegate() { GUIEventHandler.SyncHandler(profileName); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void btn_DelProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (JobQueueHandler.SyncInProgress())
            {
                DisplayMessageBox("Sorry! You are not allowed to delete any currentProfile when syncing is in progress\r\n" +
                                "We would like to ensure that undeterministic thing will not occur\r\n" +
                                "We seek your understanding and sympathy\r\n" +
                                "Thank you!",
                                "Unable to delete currentProfile");
                return;
            }
            
            int cellRow = dgv_profileList.SelectedIndex;

            if (cellRow == -1)
                return;

            Profile profile = LogicFacade.LoadProfile(profileNameList[cellRow]);
            double messageBoxTop =  Window.GetWindow(this).Top + 200;
            double messageBoxLeft = Window.GetWindow(this).Left +260;
            PopUpMessageBox messageBox = new PopUpMessageBox("Are you sure you want to permanently delete profile \n" +
                                                       profile.ProfileName + "\n   Src: " + profile.SrcFolder + "\n   Des: " + profile.DesFolder,"Delete Profile");
         
            bool? result = messageBox.ShowDialog();
            if (result==true)
                DeleteProfileAt(cellRow);
            
        }

        private void DeleteProfileAt(int selectedRowIndex)
        {
            string profileName = profileNameList[selectedRowIndex];
            LogicFacade.DeleteProfile(profileName);

            profileNameList.RemoveAt(selectedRowIndex);
            db_profileList.Rows.RemoveAt(selectedRowIndex);

            SetProfileDetails(new Profile(), true);
            this.pageProfileDetails.IsEditing = false;
        }

        private void txb_DropFolderToSyncArea_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {

            if (JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();
            
            Array a = (Array)e.Data.GetData(System.Windows.DataFormats.FileDrop);

            if (a == null || a.Length == 0)
                return;

            e.Effects = System.Windows.DragDropEffects.Move;

            List<string> folderPathList = new List<string>();
            for (int i = 0; i < a.Length; i++)
                folderPathList.Add(a.GetValue(i).ToString());

            Thread thread = new Thread(() =>
            {
                GUIEventHandler.SyncHandler(ref folderPathList);
                System.Windows.Threading.Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
                       
            thread.Start();
        }

        private void txb_DropFolderToSyncArea_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.All;
            e.Handled = true;
        }

        private void DisplayMessageBox(string message, string header)
        {
            PopUpMessageBox messageBox = new PopUpMessageBox(message, header);
            messageBox.ChangeToPopUpMessage();
            messageBox.ShowDialog();
        }
    }
}
