using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.IO;
using nets_wpf.DataStructures;
using nets_wpf.Logic;
using nets_wpf.Utility;
using System.Windows.Forms;

namespace nets_wpf.GUI
{

    #region CONSTRUCTOR

    /// <summary>
    /// Interaction logic for PageProfileDetails.xaml
    /// </summary>
    public partial class PageProfileDetails : System.Windows.Controls.UserControl
    {
        private RunningMode runningMode;
        public bool IsEditing { get; set; }
        private FolderBrowserDialog dialog_FolderBrowser;

        private System.Windows.Controls.ToolTip tltp_ErrorToolTip  = new System.Windows.Controls.ToolTip();

        public PageProfileDetails()
            : this(RunningMode.MainApplication)
        {
        }

        public PageProfileDetails(RunningMode runningMode)
        {
            this.runningMode = runningMode;
            InitializeComponent();
            this.runningMode = runningMode;
            SettingToRunningMode();
        }

        public PageProfileDetails(Profile profile)
            : this(profile, RunningMode.MainApplication)
        {
        }

        public PageProfileDetails(Profile profile, RunningMode runningMode)
        {
            this.runningMode = runningMode;
            InitializeComponent();
            SetProfileInfo(profile);
            SettingToRunningMode();
        }

        private void SettingToRunningMode()
        {
            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    btn_Sync.Content = "Sync";
                    btn_Cancel.Visibility = Visibility.Hidden;
                    break;
                case RunningMode.ContextMenuSmartSync:
                case RunningMode.ContextMenuSyncWith:
                    btn_SaveProfile.Visibility = Visibility.Hidden;
                    btn_Cancel.Visibility = Visibility.Visible;    
                    btn_Sync.Content = "OK";
                    break;
            }
            if (runningMode != RunningMode.MainApplication)
                tbx_ProfileName.IsEnabled = false;

            if (runningMode == RunningMode.ContextMenuSmartSync)
                tbx_SrcFolder.IsEnabled = false;

        }
    #endregion

        private void exp_AdvancedInfo_Expanded(object sender, RoutedEventArgs e)
        {
            this.exp_advancedInfo.Height += 140;
        }

        private void exp_AdvancedInfo_Collapsed(object sender, RoutedEventArgs e)
        {
            this.exp_advancedInfo.Height -= 140;
        }

        private void btn_BrowseSrcFolder_Click(object sender, RoutedEventArgs e)
        {
            dialog_FolderBrowser = new FolderBrowserDialog();
            DialogResult userResponse = dialog_FolderBrowser.ShowDialog();
            string selectedPath = dialog_FolderBrowser.SelectedPath.Trim();

            if (userResponse == DialogResult.OK || selectedPath != "")
                tbx_SrcFolder.Text = selectedPath;
        }

        private void btn_BrowseDesFolder_Click(object sender, RoutedEventArgs e)
        {
            dialog_FolderBrowser = new FolderBrowserDialog();
            DialogResult userResponse = dialog_FolderBrowser.ShowDialog();
            string selectedPath = dialog_FolderBrowser.SelectedPath.Trim();

            if (userResponse == DialogResult.OK || selectedPath != "")
                tbx_DesFolder.Text = selectedPath;
        }

        private void tbx_IncludePattern_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PopUpErrorMessage("Example: *.txt, *fish*, cat*.doc", tbx_IncludePattern);
        }

        private void tbx_ExcludePattern_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            pp_ErrorMessage.IsOpen = false;
        }

        private void tbx_IncludePattern_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            pp_ErrorMessage.IsOpen = false;
        }

        private void tbx_ExcludePattern_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("fdsaf");
            PopUpErrorMessage("Example: *.txt, *fish*, cat*.doc", tbx_ExcludePattern);
            PopUpErrorMessage("Example: *.txt, *fish*, cat*.doc", tbx_ExcludePattern);
        }

        private void tbx_IncludePattern_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PopUpErrorMessage("Example: *.txt, *fish*, cat*.doc", tbx_IncludePattern);
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void btn_Sync_Click(object sender, RoutedEventArgs e)
        {
            if (runningMode == RunningMode.MainApplication && JobQueueHandler.SyncInProgress())
            {
                GUIEventHandler.ShowCannotSyncMessage();
                return;
            }
            JobQueueHandler.CreateMainSyncFile();
            string normalizedSrcFolder = PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text);
            string normalizedDesFolder = PathOperator.NormalizeFolderPath(tbx_DesFolder.Text);
            string include = tbx_IncludePattern.Text;
            string exclude = tbx_ExcludePattern.Text;
            SyncMode syncMode = (rbtn_TwoWayMode.IsChecked == true) ? SyncMode.Equalize : SyncMode.Mirror;

            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    Thread thread = new Thread(() =>{
                        GUIEventHandler.SyncHandler(normalizedSrcFolder,normalizedDesFolder,syncMode,include, exclude);
                        System.Windows.Threading.Dispatcher.Run();
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();                 
                    break;
                case RunningMode.ContextMenuSyncWith:
                    GUIEventHandler.SyncHandler(normalizedSrcFolder, normalizedDesFolder, syncMode, include, exclude);
                    (Window.GetWindow(this)).Close();
                    break;
                case RunningMode.ContextMenuSmartSync:
                    LogicFacade.EnqueueSyncJob(normalizedSrcFolder,normalizedDesFolder,syncMode,include, exclude);
                    (Window.GetWindow(this)).Close();
                    break;
            }
        }

        private void btn_SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditing && JobQueueHandler.SyncInProgress())
            {
                DisplayMessageBox("Sorry! You are not allowed to edit any profile when syncing is in progress\r\n" +
                                "We would like to ensure that undeterministic thing will not occur\r\n" +
                                "We seek your understanding and sympathy\r\n" +
                                "Thank you!",
                                "Unable to edit profile");
                return;
            }

            string normalizedSrcFolder = PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text);
            string normalizedDesFolder = PathOperator.NormalizeFolderPath(tbx_DesFolder.Text);
            string trimmedProfileName = tbx_ProfileName.Text.Trim();

            // Validate currentProfile information)
            if (!FolderPathInfoIsValid(normalizedSrcFolder, normalizedDesFolder))
                return;
            if (FolderPathInfoIsConflicted(normalizedSrcFolder, normalizedDesFolder))
                return;
            if (!ProfileNameIsValid(trimmedProfileName))
                return;
            if (ProfileNameIsConflicted(trimmedProfileName))
                return;

            // Remove current profile in case of editing
            if (IsEditing)
                LogicFacade.DeleteProfile(LogicFacade.GetCurrentProfile().ProfileName);

            // Save the new profile
            Profile newProfile = new Profile(tbx_ProfileName.Text.Trim(),
                                             PathOperator.NormalizeFolderPath(tbx_SrcFolder.Text.Trim()),
                                             PathOperator.NormalizeFolderPath(tbx_DesFolder.Text.Trim()),
                                             (rbtn_TwoWayMode.IsChecked  == true) ? SyncMode.Equalize : SyncMode.Mirror,
                                             tbx_IncludePattern.Text.Trim(),
                                             tbx_ExcludePattern.Text.Trim());
            LogicFacade.SaveProfile(newProfile);
            LogicFacade.SetCurrentProfile(newProfile);

            // Confirmation if necessary (in cases other than MainApplication)
            switch (runningMode)
            {
                case RunningMode.MainApplication:
                    RefreshProfileList(tbx_ProfileName.Text.Trim());   
                    break;
                default:
                    DisplayMessageBox("Profile " + newProfile.ProfileName + " has successfully been saved!","");
                    break;
            }

            // Switch to IsEditing after saving
            IsEditing = true;
            RefreshProfileList(newProfile.ProfileName);
        }

        #region HELPER


        private void dragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.All;
            e.Handled = true;
        }

        private void dragDrop(object sender, System.Windows.DragEventArgs e)
        {
            Array a = (Array)e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (a != null)
            {
                string s = a.GetValue(0).ToString();
                (sender as System.Windows.Controls.AutoCompleteBox).Text = s;
            }
        }

        private void AutoCompleteBox_Populating(object sender, System.Windows.Controls.PopulatingEventArgs e)
        {
            //tbAssembly.com
            string text = (sender as AutoCompleteBox).Text;
            string dirname = System.IO.Path.GetDirectoryName(text);

            if (runningMode == RunningMode.MainApplication)
                tbx_SrcDes_TextChanged();

            if (Directory.Exists(dirname))
            {
                string[] dirs = Directory.GetDirectories(dirname);
                List<string> candidates = dirs.ToList<string>();

                (sender as AutoCompleteBox).ItemsSource = candidates;
                (sender as AutoCompleteBox).PopulateComplete();
            }
        }

        private void tbx_SrcDes_TextChanged()
        {
            if (LogicFacade.LoadSetting("autocomplete"))
                ModifyProfileNameFollowingSrcAndDes();
        }

        private void tbx_DesFolder_TextChanged(object sender, RoutedEventArgs e)
        {
            if (LogicFacade.LoadSetting("autocomplete") && runningMode == RunningMode.MainApplication)
                ModifyProfileNameFollowingSrcAndDes();
        }

        private void tbx_ExcludePattern_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PopUpErrorMessage("Example: *.txt, *fish*, cat*.doc", tbx_ExcludePattern);
        }

     

        private void PopUpErrorMessage(string message, UIElement targetElement)
        {
            txb_popUpContent.Text = message;
            pp_ErrorMessage.PlacementTarget = targetElement;
            pp_ErrorMessage.StaysOpen = false;
            pp_ErrorMessage.IsOpen = true;
        }

        private void DisplayMessageBox(string message, string header)
        {
            PopUpMessageBox messageBox = new PopUpMessageBox(message, header);
            messageBox.ChangeToPopUpMessage();
            messageBox.ShowDialog();
        }

        private void RefreshProfileList(string profileName)
        {
            DependencyObject parentControl = null;
            DependencyObject currentControl = this;
            while ((currentControl = VisualTreeHelper.GetParent(currentControl)) != null)
            {
                parentControl = currentControl;
                if (parentControl is PageStart)
                {
                    (parentControl as PageStart).RefreshProfileList(profileName);
                    break;
                }
            }
        }

     /// <summary>
        /// Set profile info for the current page
        /// </summary>
        /// <param name="profile"></param>
        public void SetProfileInfo(Profile profile)
        {
            tbx_SrcFolder.Text = profile.SrcFolder;
            tbx_DesFolder.Text = profile.DesFolder;

            tbx_ExcludePattern.Text = profile.ExcludePattern;
            tbx_IncludePattern.Text = profile.IncludePattern;

            tbx_ProfileName.Text = profile.ProfileName;

            switch (profile.SyncMode)
            {
                case SyncMode.Mirror:
                    rbtn_OneWayMode.IsChecked = true;
                    rbtn_TwoWayMode.IsChecked = false;
                    break;
                case SyncMode.Equalize:
                    rbtn_OneWayMode.IsChecked = false;
                    rbtn_TwoWayMode.IsChecked = true;
                    break;
            }
        }

        public void ModifyProfileNameFollowingSrcAndDes()
        {
            if (runningMode != RunningMode.MainApplication)
                return;

            string srcName = PathOperator.GetNameFromPath(tbx_SrcFolder.Text);
            string desName = PathOperator.GetNameFromPath(tbx_DesFolder.Text);

            tbx_ProfileName.Text = srcName + " & " + desName;
        }

        #endregion 

        /// <summary>
        /// Check if necessary information is filled
        /// </summary>
        /// <returns></returns>
        private bool InfoIsEnough()
        {
            if (string.IsNullOrEmpty(tbx_SrcFolder.Text))
            {
                DisplayMessageBox("Please specify the source folder!","");
                return false;
            }
            if (string.IsNullOrEmpty(tbx_DesFolder.Text))
            {
                DisplayMessageBox("Please specify the destination folder!","");
                return false;
            }
            return true;
        } 

       
       /// <summary>
        /// Check for valid folder paths (inside a profile scope only)
        /// </summary>
        /// <returns></returns>
        private bool FolderPathInfoIsValid(string normalizedSrcFolder, string normalizedDesFolder)
        {
            // Check for empty folder paths
            if (normalizedSrcFolder == String.Empty)
            {
                tbx_SrcFolder.Focus();
                PopUpErrorMessage("Source folder path cannot be empty.", tbx_SrcFolder);
                return false;
            }

            if (normalizedDesFolder == String.Empty)
            {
                tbx_DesFolder.Focus();
                PopUpErrorMessage("Destination folder path cannot be empty.", tbx_DesFolder);
                return false;
            }

            // Check for folder existence
            if (!Directory.Exists(normalizedSrcFolder))
            {
                tbx_SrcFolder.Focus();
                PopUpErrorMessage("Source folder does not exist.", tbx_SrcFolder);
                return false;
            }
            if (!Directory.Exists(normalizedDesFolder))
            {
                tbx_DesFolder.Focus();
                PopUpErrorMessage("Destination folder does not exist.", tbx_DesFolder);
                return false;
            }

            // Check for same source and destination folders
            if (normalizedSrcFolder.ToLower() == normalizedDesFolder.ToLower())
            {
                tbx_DesFolder.Focus();
                PopUpErrorMessage("Destination folder must be different from source folder.", tbx_DesFolder);
                return false;
            }

            // Check for possible recursion
            if (normalizedSrcFolder.ToLower().StartsWith(normalizedDesFolder) || normalizedDesFolder.ToLower().StartsWith(normalizedSrcFolder))
            {
                tbx_SrcFolder.Focus();
                PopUpErrorMessage("Source and destination folders cannot include each other.", tbx_SrcFolder);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check for conflicted folder paths (with other profiles)
        /// </summary>
        /// <returns></returns>
        private bool FolderPathInfoIsConflicted(string normalizedSrcFolder, string normalizedDesFolder)
        {
            string currentProfileName = LogicFacade.GetCurrentProfile().ProfileName;
            List<string> conflictedProfileNameList = LogicFacade.GetConflictedProfileNameList(normalizedSrcFolder, normalizedDesFolder);

            // remove currentProfile from conflictedProfileList in case of editing
            if (IsEditing)
                conflictedProfileNameList.Remove(currentProfileName);

            if (conflictedProfileNameList.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();

                errorMessage.Append("This profile is in conflict with the following profile(s):\n");
                foreach (string profileName in conflictedProfileNameList)
                    errorMessage.Append("  " + profileName + "\n");
                errorMessage.Append("Please delete existing profile(s) then try again!");

                PopUpErrorMessage(errorMessage.ToString(), tbx_SrcFolder);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check for valid profileName (inside a profile scope only)
        /// </summary>
        /// <returns></returns>
        private bool ProfileNameIsValid(string trimmedProfileName)
        {
            // Check for empty profileName
            if (trimmedProfileName == String.Empty)
            {
                tbx_ProfileName.Focus();
                PopUpErrorMessage("Profile name cannot be empty.", tbx_ProfileName);
                return false;
            }

            // Check for valid profileName (does not contain special symbols)
            if (trimmedProfileName.Contains("|") || trimmedProfileName.Contains(@"\") ||
                trimmedProfileName.Contains(":") || trimmedProfileName.Contains("\"") ||
                trimmedProfileName.Contains("<") || trimmedProfileName.Contains(">") ||
                trimmedProfileName.Contains("?") || trimmedProfileName.Contains("*") ||
                trimmedProfileName.Contains("/"))
            {
                PopUpErrorMessage("Profile name cannot contain the following characters: \r\n   " + "|, \\, :, \", >, <, ?, *, /", tbx_ProfileName);
                return false;
            }

            // ProfileName is valid
            tbx_ProfileName.Text = trimmedProfileName;
            return true;
        }

        /// <summary>
        /// Check for conflicted profileName (with other profiles)
        /// </summary>
        /// <returns></returns>
        private bool ProfileNameIsConflicted(string trimmedProfileName)
        {
            string curProfileName = LogicFacade.GetCurrentProfile().ProfileName;
            string newProfileName = trimmedProfileName;

            List<string> profileNameList = LogicFacade.LoadProfileNameList();

            // remove currentProfile from conflictedProfileList in case of editing
            if (IsEditing)
                profileNameList.Remove(curProfileName);

            if (profileNameList.Contains(newProfileName))
            {
                PopUpErrorMessage("A profile with the same name already exists.", tbx_ProfileName);
                return true;
            }

            return false;
        }
        
        //#endregion



    }
}
