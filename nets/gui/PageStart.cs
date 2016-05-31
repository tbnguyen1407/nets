using System.Collections.Generic;
using System.Windows.Forms;
using nets.dataclass;
using nets.logic;
using System;
using System.IO;
using System.Threading;
using nets.utility;

namespace nets.gui
{
    /// <summary>
    /// The Profiles Panel in the main application window
    /// Author: Hoang Nguyen Nhat Tao + Tran Binh Nguyen
    /// </summary>
    public partial class PageStart
    {
        #region FIELD DECLARATION

        List<string> profileNameList = new List<string>();
        Profile currentProfile = new Profile();
        PageProfileDetails panel_ProfileDetails;
        private static Thread thread;

        #endregion

        #region CONSTRUCTORS

        public PageStart()
        {
            InitializeComponent();
            PopulateProfileList();
            DisplayProfileDetails();
            dgv_profileList.ClearSelection();
        }

        #endregion

        #region COMPONENT EVENT HANDLERS

        private void dgv_profileList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_profileList_CellEnter(sender, e);
        }

        private void dgv_profileList_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int cellRow = e.RowIndex;
            int cellCol = e.ColumnIndex;

            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];

            switch (cellCol)
            {
                case 0:
                    SetProfileDetails(LogicFacade.LoadProfile(profileName), false);
                    panel_ProfileDetails.IsEditing = true;
                    break;
                default:
                    break;
            }
        }

        private void dgv_profileList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int cellRow = e.RowIndex;
            int cellCol = e.ColumnIndex;

            if (cellRow == -1)
                return;

            string profileName = profileNameList[cellRow];

            switch (cellCol)
            {
                case 1:
                    SetProfileDetails(LogicFacade.LoadProfile(profileName), false);
                    panel_ProfileDetails.IsEditing = true;

                    if (!FolderPathIsValid(profileName))
                        return;

                    if (JobQueueHandler.SyncInProgress())
                    {
                        GUIEventHandler.ShowCannotSyncMessage();
                        return;
                    }
                    JobQueueHandler.CreateMainSyncFile();

                    thread = new Thread(() => GUIEventHandler.SyncHandler(profileName));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();

                    break;
                default:
                    break;
            }
        }

        private static bool FolderPathIsValid(string profileName)
        {
            Profile profile = LogicFacade.LoadProfile(profileName);
            if (!Directory.Exists(profile.SrcFolder))
            {
                MessageBox.Show("We can not run the profile " + profileName + " because its source folder no longer exists!",
                                "Folder not found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
                return false;
            }
            if (!Directory.Exists(profile.DesFolder))
            {
                MessageBox.Show("We can not run the profile " + profileName + " because its destination folder no longer exists!",
                                "Folder not found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
                return false;
            }
            return true;
        }

        private void btn_NewProfile_Click(object sender, EventArgs e)
        {
            Profile newProfile = new Profile();
            LogicFacade.SetCurrentProfile(newProfile);
            panel_ProfileDetails.SetProfileInfo(newProfile);
            panel_ProfileDetails.IsEditing = false;
            dgv_profileList.ClearSelection();
        }

        private void btn_DeleteProfile_Click(object sender, EventArgs e)
        {
            if (JobQueueHandler.SyncInProgress())
            {
                MessageBox.Show("Sorry! You are not allowed to delete any currentProfile when syncing is in progress\r\n" +
                                "We would like to ensure that undeterministic thing will not occur\r\n" +
                                "We seek your understanding and sympathy\r\n" +
                                "Thank you!",
                                "Unable to delete currentProfile",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (dgv_profileList.SelectedRows.Count == 0)
                return;

            int selectedRowIndex = dgv_profileList.SelectedRows[0].Index;

            DialogResult userResponse = MessageBox.Show("Are you sure you want to permanently delete this currentProfile?\r\n\n" + 
                                                        dgv_profileList.Rows[selectedRowIndex].Cells[0].Value,
                                                        "Delete Profile",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question,
                                                        MessageBoxDefaultButton.Button1);

            if (userResponse == DialogResult.No)
                return;
            DeleteProfileAt(selectedRowIndex);
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Populate currentProfile list with saved profiles
        /// </summary>
        private void PopulateProfileList()
        {
            Profile profile;
            profileNameList = LogicFacade.LoadProfileNameList();

            dgv_profileList.Rows.Clear();

            foreach (string profileName in profileNameList)
            {
                profile = LogicFacade.LoadProfile(profileName);
                string formattedSrcPath = PathOperator.TrimPath(profile.SrcFolder, 28);
                string formattedDesPath = PathOperator.TrimPath(profile.DesFolder, 28);
                string formattedProfileName = PathOperator.TrimPath(profile.ProfileName, 32);

                string profileInfo = " " + formattedProfileName + "\n" +
                                     "   Src: " + formattedSrcPath + "\n" +
                                     "   Des: " + formattedDesPath;

                dgv_profileList.Rows.Add(profileInfo);
            }
        }

        /// <summary>
        /// Refresh currentProfile list and select the correct currentProfile
        /// </summary>
        public void RefreshProfileList(string profileName)
        {
            PopulateProfileList();
            int selectedIndex = profileNameList.IndexOf(profileName);
            dgv_profileList.Rows[selectedIndex].Cells[0].Selected = true;
        }

        /// <summary>
        /// Display the selected currentProfile details
        /// </summary>
        private void DisplayProfileDetails()
        {
            currentProfile = (profileNameList.Count > 0) ? LogicFacade.LoadProfile(profileNameList[0]) : new Profile();
            LogicFacade.SetCurrentProfile(currentProfile);
            panel_ProfileDetails = new PageProfileDetails(currentProfile);
            gbx_ProfileDetails.Controls.Add(panel_ProfileDetails);
        }

        /// <summary>
        /// Set currentProfile details
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="isNewProfile"></param>
        private void SetProfileDetails(Profile profile, bool isNewProfile)
        {
            if (isNewProfile)
                dgv_profileList.ClearSelection();
            LogicFacade.SetCurrentProfile(profile);
            panel_ProfileDetails.SetProfileInfo(profile);
        }

        private void DeleteProfileAt(int selectedRowIndex)
        {
            string profileName = profileNameList[selectedRowIndex];
            LogicFacade.DeleteProfile(profileName);

            profileNameList.RemoveAt(selectedRowIndex);
            dgv_profileList.Rows.RemoveAt(selectedRowIndex);

            SetProfileDetails(new Profile(), true);
            panel_ProfileDetails.IsEditing = false;
        }

        #endregion
    }
}
