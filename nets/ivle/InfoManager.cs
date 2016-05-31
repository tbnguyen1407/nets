#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using nets.dataclass;
using nets.utility;

#endregion

namespace nets.ivle
{
    /// <summary>
    ///   Facade to other functional classes.
    ///   Provide mechanism to access local data.
    ///   For safety purpose, only one password can be saved
    /// Author: Vu An Hoa
    /// </summary>
    internal static class InfoManager
    {
        #region FIELD DECLARATION

        // All information files will be stored at this folder.
        // public static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\nets\";
        public const string AppDataFolder = @"";

        // Additional helper classes to manage different type of information.
        private static readonly UserInfoManager UserMan = new UserInfoManager();
        private static readonly WorkbinInfoManager WorkbinMan = new WorkbinInfoManager();
        private static readonly SecureVault Vault = new SecureVault("evil.veil");

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="workbinIds"></param>
        public static void AddUser(string userid, List<String> workbinIds)
        {
            UserProfile profile = new UserProfile(userid) {WorkbinIds = workbinIds};
            UserMan.AddProfile(profile);
            UserMan.SaveDataFile();
        }

        /// <summary>
        /// Load workbinId list of a userId
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<string> LoadWorkbinIds(string userid)
        {
            UserProfile profile = UserMan.LoadProfile(userid);
            return profile.WorkbinIds;
        }

        /// <summary>
        /// Load a workbin
        /// </summary>
        /// <param name="workbinid"></param>
        /// <returns></returns>
        public static IvleWorkbin LoadWorkbin(string workbinid)
        {
            return WorkbinMan.LoadWorkbin(workbinid);
        }

        /// <summary>
        /// Add a workbin
        /// </summary>
        /// <param name="wb"></param>
        public static void AddWorkbin(IvleWorkbin wb)
        {
            WorkbinMan.SaveWorkbin(wb);
            WorkbinMan.SaveDataFile();
        }

        /// <summary>
        /// Save password for current user
        /// </summary>
        /// <param name="password"></param>
        public static void SaveUserPassword(string password)
        {
            Vault.AddSecureInfo(password);
        }

        /// <summary>
        /// Get saved password
        /// </summary>
        /// <returns></returns>
        public static string GetPassword()
        {
            return Vault.GetSecureValue();
        }

        #endregion
    }

    #region HELPER CLASSES

    /// <summary>
    ///   Main class to manage user profiles.
    ///   TODO: provide mechanism to store the currentProfile back
    /// </summary>
    public class UserInfoManager
    {
        #region FIELD DECLARATION

        private const string UserProfileDataFilePath = InfoManager.AppDataFolder + @"ivle_user_profiles.xml";
        private readonly bool IsAltered;
        private readonly XmlDocument ProfileDocument;

        #endregion

        #region CONSTRUCTORS

        // CONDITION: _profileDocument is not null after constructor.
        public UserInfoManager()
        {
            ProfileDocument = XmlTool.LoadXmlDoc(UserProfileDataFilePath, "<Users></Users>");
            IsAltered = false;
        }

        #endregion

        #region DESTRUCTORS

        // Always save the data file upon exit.
        ~UserInfoManager()
        {
            SaveDataFile();
        }

        #endregion

        #region MAIN METHODS

        public void SaveDataFile()
        {
            if (!IsAltered)
                return;
            ProfileDocument.Save(UserProfileDataFilePath);
        }

        /// <summary>
        ///   Add a new currentProfile to the xml document.
        /// </summary>
        /// <param name = "userProfile">User currentProfile to add.</param>
        /// <returns>True if new currentProfile is added. False if the currentProfile already exists.</returns>
        //public bool AddProfile(UserProfile userProfile)
        //{
        //    // Check if user information is already stored.
        //    var userNode = (XmlElement) _profileDocument.SelectSingleNode("//User[@id=\'" + userProfile.UserId + "\']");
        //    if (userNode == null)
        //    {
        //        // Add the user to the currentProfile document.
        //        userNode = _profileDocument.CreateElement("User");
        //        userNode.SetAttribute("id", userProfile.UserId);
        //        _profileDocument.FirstChild.AppendChild(userNode);
        //        // Set the local root
        //        var localPathNode = _profileDocument.CreateElement("LocalRoot");
        //        localPathNode.InnerText = userProfile.LocalRootPath;
        //        userNode.AppendChild(localPathNode);
        //        // Adding workbin ids for this user.
        //        foreach (var id in userProfile.WorkbinIds)
        //        {
        //            var workbinNode = _profileDocument.CreateElement("Workbin");
        //            workbinNode.InnerText = id;
        //            userNode.AppendChild(workbinNode);
        //        }
        //        return true;
        //    }
        //    return false;
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public bool AddProfile(UserProfile userProfile)
        {
            // Check if user information is already stored.
            var userNode = XmlTool.FindDataNode(ProfileDocument, "Id", userProfile.UserId);

            if (userNode == null)
            {
                // Create a new node for user
                userNode = ProfileDocument.CreateElement("User");
                // & Add the user node to the document
                ProfileDocument.FirstChild.AppendChild(userNode);
            }

            // Set the user id
            XmlTool.AddElementToDoc(userNode, "Id", userProfile.UserId);
            XmlTool.AddElementToDoc(userNode, "LocalRoot", userProfile.LocalRootPath);

            // Adding workbin ids for this user.
            foreach (var id in userProfile.WorkbinIds)
                XmlTool.AddElementToDoc(userNode, "Workbin", id);

            return true;
        }

        /// <summary>
        ///   Load a user currentProfile from program database.
        /// </summary>
        /// <param name = "userId">The UserProfile object for this user.</param>
        /// <returns>Null if the user information is not cached.</returns>
        public UserProfile LoadProfile(string userId)
        {
            XmlElement userNode = (XmlElement) XmlTool.FindDataNode(ProfileDocument.FirstChild, "User/Id", userId).ParentNode;

            // User currentProfile is not cached.
            if (userNode == null)
                return null;

            // Collect information
            UserProfile profile = new UserProfile(userId) {LocalRootPath = XmlTool.GetDatum(userNode, "LocalRoot"), WorkbinIds = XmlTool.GetAllData(userNode, "workbin")};

            return profile;
        }
        
        //public UserProfile LoadProfile(string userId)
        //{
        //    var userNode = (XmlElement)_profileDocument.SelectSingleNode("//User[@id=\'" + userId + "\']");
        //    // User currentProfile is not cached.
        //    if (userNode == null)
        //    {
        //        return null;
        //    }
        //    // Parse for information
        //    var currentProfile = new UserProfile(userId);
        //    // Root path can get from inner text of <LocalRoot> </LocalRoot>
        //    currentProfile.LocalRootPath = userNode.SelectSingleNode("./LocalRoot").InnerText;
        //    // Collect user workbins
        //    foreach (XmlNode node in userNode.SelectNodes("./Workbin"))
        //    {
        //        currentProfile.WorkbinIds.Add(node.InnerText);
        //    }
        //    return currentProfile;
        //}

        /// <summary>
        ///   Load the most recent currentProfile.
        /// </summary>
        /// <returns>Null if no such information is found. The most recent user if cached.</returns>
        public UserProfile LoadMostRecentProfile()
        {
            return null;
        }

        /// <summary>
        ///   Load the list of all users' ID (i.e NUSNET account).
        ///   This will support the GUI. The user can select from
        ///   the cache information.
        /// </summary>
        /// <returns>The list of all users' IDs cached.</returns>
        public List<string> LoadUserIdList()
        {
            //var userIdList = new List<string>();

            //// list of all <User> node
            //XmlNodeList userNodeList = _profileDocument.SelectNodes("//User");
            //if (userNodeList != null)
            //{
            //    foreach (XmlElement node in userNodeList)
            //    {
            //        userIdList.Add(node.GetAttribute("id"));
            //    }
            //}

            //return userIdList;

            return XmlTool.GetAllData((XmlElement) ProfileDocument.FirstChild, "User/Id");
        }

        /// <summary>
        ///   Remove the cache for the user with userId.
        ///   NOTE: THIS METHOD MIGHT NOT BE FREQUENTLY USED.
        ///   IN THE FUTURE, WE MIGHT LIMIT THE NUMBER OF USER IDS 
        ///   TO BE STORED FOR INSTANCE, THE 5 MOST RECENTLY ONES. 
        ///   AND THIS METHOD WILL COME IN HANDY.
        /// </summary>
        /// <param name = "userId">The UserProfile tag to load</param>
        /// <returns>A bool value indicating whether the UserProfile storage file exists or not</returns>
        public void RemoveProfile(string userId)
        {
            XmlNode node = ProfileDocument.SelectSingleNode("//User[@id=\'" + userId + "\']");
            if (node == null)
                return;
            ProfileDocument.FirstChild.RemoveChild(node);
        }

        #endregion
    }

    /// <summary>
    ///   Encapsulation of a user currentProfile.
    ///   This is to support multiple user environment.
    /// </summary>
    public class UserProfile
    {
        // TODO: FOR NEXT RELEASE
        // Note: This version do not support storage of UserName.
        // Note: Password is optional to store. Useful to have. But need to develop a secure system.
        // Only to communicate with the user of the software.

        #region FIELD DECLARATION

        public readonly string UserName;
        public readonly string UserId; // User Id should be unique to determine the currentProfile!
        public readonly string Password;
        public string LocalRootPath; // Path to the local root of all ivle workbins.
        public List<string> WorkbinIds;

        #endregion

        #region CONSTRUCTORS

        public UserProfile(string userId)
            :this()
        {
            UserId = userId;
        }

        private UserProfile()
        {
            WorkbinIds = new List<string>();
        }

        #endregion
    }

    /// <summary>
    ///   Manage the workbin information.
    /// </summary>
    public class WorkbinInfoManager
    {
        // Workbin is store in XmlFormat. This is natural since workbin folder should be in tree format.

        #region FIELD DECLARATION

        private const string WorkbinFilePath = InfoManager.AppDataFolder + @"workbins.xml";
        private readonly bool IsAltered;
        private readonly XmlDocument WorkbinDocument;

        #endregion

        #region CONSTRUCTORS

        public WorkbinInfoManager()
        {
            WorkbinDocument = XmlTool.LoadXmlDoc(WorkbinFilePath, "<Workbins></Workbins>");
            IsAltered = false;
        }

        #endregion

        #region DESTRUCTORS

        // Always save the data file upon exit.
        ~WorkbinInfoManager()
        {
            SaveDataFile();
        }

        #endregion

        #region MAIN METHODS

        public void SaveDataFile()
        {
            if (IsAltered)
            {
                WorkbinDocument.Save(WorkbinFilePath);
            }
        }

        /// <summary>
        ///   Save the workbin object to a file.
        ///   TODO: IMPLEMENT.
        /// </summary>
        /// <param name = "ivleWorkbin">Workbin object to store.</param>
        public void SaveWorkbin(IvleWorkbin ivleWorkbin)
        {
            XmlElement workbinNode = FindWorkbinNode(ivleWorkbin.WorkbinId);

            // Remove this node if it exists.
            if (workbinNode != null)
            {
                WorkbinDocument.DocumentElement.RemoveChild(workbinNode);
            }

            // Add the workbin node.
            workbinNode = XmlTool.AddElementToDoc(WorkbinDocument.DocumentElement, "Workbin", "");

            // Add attributes
            XmlTool.AddElementToDoc(workbinNode, "Id", ivleWorkbin.WorkbinId);
            XmlTool.AddElementToDoc(workbinNode, "Name", ivleWorkbin.WorkbinName);
            XmlTool.AddElementToDoc(workbinNode, "ModuleCode", ivleWorkbin.ModuleCode);
            XmlTool.AddElementToDoc(workbinNode, "ModuleName", ivleWorkbin.ModuleName);

            // Add the module code.
            //XmlElement nodeAtt = _workbinDocument.CreateElement("ModuleCode");
            //nodeAtt.InnerText = workbin.ModuleCode;
            //workbinNode.AppendChild(nodeAtt);

            //// Add the module tag.
            //nodeAtt = _workbinDocument.CreateElement("ModuleName");
            //nodeAtt.InnerText = workbin.ModuleName;
            //workbinNode.AppendChild(nodeAtt);

            // Recursively build the file system tree structure.
            workbinNode.AppendChild(PutFolder(ivleWorkbin.RootFolder));
        }

        /// <summary>
        ///   Reconstruct the workbin object from last storage.
        /// </summary>
        /// <param name = "workbinId">ID of the workbin to be loaded.</param>
        public IvleWorkbin LoadWorkbin(string workbinId)
        {
            // Reconstruct workbin: file table is not reconstructed since it is to be updated.
            IvleWorkbin wb = new IvleWorkbin();
            XmlElement workbinNode = FindWorkbinNode(workbinId);
            if (workbinNode == null)
                return null;

            // Basic information.
            wb.WorkbinId = workbinId;
            wb.WorkbinName = XmlTool.GetDatum(workbinNode, "Name");
            wb.ModuleCode = XmlTool.GetDatum(workbinNode, "ModuleCode");
            wb.ModuleName = XmlTool.GetDatum(workbinNode, "ModuleName");

            //wb.WorkbinName = workbinNode.GetAttribute("tag");
            //wb.ModuleCode = workbinNode.SelectSingleNode("./ModuleCode").InnerText;
            //wb.ModuleName = workbinNode.SelectSingleNode("./ModuleName").InnerText;

            // Construct the file system structure.
            XmlElement objNode = (XmlElement)workbinNode.SelectSingleNode("./Folder");
            if (objNode != null)
                wb.RootFolder = GetFolder(objNode, wb);

            return wb;
        }

        #endregion

        #region HELPERS

        /// <summary>
        ///   Find the node for this workbin.
        /// </summary>
        /// <param name = "workbinId">Workbin Id.</param>
        /// <returns>Xml Node in the document corresponding to this workbin id.</returns>
        private XmlElement FindWorkbinNode(string workbinId)
        {
            XmlElement idNode = XmlTool.FindDataNode(WorkbinDocument.FirstChild, "Workbin/Id", workbinId);
            return ((idNode != null) ? (XmlElement)idNode.ParentNode : null);
            //return (XmlElement)_workbinDocument.SelectSingleNode("//Workbin[@id=\'" + workbinId + "\']");
        }

        /// <summary>
        ///   Generate the XML Node for a particular folder rooting at a point.
        /// </summary>
        /// <param name = "folder"></param>
        /// <returns></returns>
        private XmlElement PutFolder(IvleWorkbinFolder folder)
        {
            XmlElement rootNode = WorkbinDocument.CreateElement("Folder");

            //rootNode.SetAttribute("id", folder.FolderId);
            //rootNode.SetAttribute("name", folder.FolderName);

            XmlTool.AddElementToDoc(rootNode, "Id", folder.FolderId);
            XmlTool.AddElementToDoc(rootNode, "Name", folder.FolderName);

            foreach (IvleWorkbinFile file in folder.Files)
            {
                // OLD IMPLEMENTATION!
                //var fileNode = _workbinDocument.CreateElement("File");
                //fileNode.SetAttribute("id", file.FileId);
                //fileNode.SetAttribute("name", file.FileName);
                //rootNode.AppendChild(fileNode);

                //var tempNode = _workbinDocument.CreateElement("Date");
                //tempNode.InnerText = file.UploadDate;
                //fileNode.AppendChild(tempNode);

                //tempNode = _workbinDocument.CreateElement("Size");
                //tempNode.InnerText = file.FileSize;
                //fileNode.AppendChild(tempNode);

                //tempNode = _workbinDocument.CreateElement("Status");
                //tempNode.InnerText = (file.IsNew ? "New" : "Downloaded");
                //fileNode.AppendChild(tempNode);

                XmlElement fileNode = XmlTool.AddElementToDoc(rootNode, "File", "");
                XmlTool.AddElementToDoc(fileNode, "Id", file.FileId);
                XmlTool.AddElementToDoc(fileNode, "Name", file.FileName);
                XmlTool.AddElementToDoc(fileNode, "Date", file.UploadDate);
                XmlTool.AddElementToDoc(fileNode, "Size", file.FileSize);
            }

            foreach (IvleWorkbinFolder subfolder in folder.SubFolders)
                rootNode.AppendChild(PutFolder(subfolder));

            return rootNode;
        }

        /// <summary>
        ///   Construct the folder from a folder node.
        /// </summary>
        /// <param name = "folderNode">The XML Folder node</param>
        /// <param name = "ivleWorkbin">Workbin that this folder should belong to.</param>
        /// <returns></returns>
        private static IvleWorkbinFolder GetFolder(XmlElement folderNode, IvleWorkbin ivleWorkbin)
        {
            // Invalid input
            if (!folderNode.Name.Equals("Folder"))
                throw new Exception("GetFolder: Input is not a <Folder> node.");

            // The node to return
            // WFolder folder = new WFolder(folderNode.GetAttribute("id"),folderNode.GetAttribute("tag"), workbin);
            IvleWorkbinFolder folder = new IvleWorkbinFolder(XmlTool.GetDatum(folderNode, "Id"), XmlTool.GetDatum(folderNode, "Name"), ivleWorkbin);

            // Add the files.
            XmlNodeList fileNodeList = folderNode.SelectNodes("./File");
            if (fileNodeList != null)
            {
                foreach (XmlElement node in fileNodeList)
                {
                    // Add the file found at this node.
                    //WFile file = new WFile(node.GetAttribute("id"),node.GetAttribute("tag"));
                    IvleWorkbinFile file = new IvleWorkbinFile(XmlTool.GetDatum(node, "Id"), XmlTool.GetDatum(node, "Name")) {Container = folder};
                    folder.Files.Add(file);
                }
            }

            // Recursive call to add the folders.
            XmlNodeList folderNodeList = folderNode.SelectNodes("./Folder");
            if (folderNodeList != null)
            {
                foreach (XmlElement node in folderNodeList)
                {
                    // Add the constructed subfolder.
                    folder.SubFolders.Add(GetFolder(node, ivleWorkbin));
                }
            }

            // Set the correct parent references.
            folder.FixParentReferenceInSubfolders();

            return folder;
        }

        #endregion
    }

    /// <summary>
    ///   Log all the download file information.
    /// </summary>
    public class DownloadLogManager
    {
        #region FIELD DECLARATION

        // Path to log file. Log file is always to append.
        private const string LogPath = InfoManager.AppDataFolder + @"\download_log.xml";
        private bool IsAltered;
        private readonly XmlDocument LogDocument;

        #endregion

        #region CONSTRUCTURS

        public DownloadLogManager()
        {
            LogDocument = new XmlDocument();
            IsAltered = false;

            if (!File.Exists(LogPath))
                File.WriteAllText(LogPath, "<Log></Log>");
            LogDocument.LoadXml(LogPath);
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        ///   Write a download log to the log storage file // KEEP HERE FOR COMPATIBILITY PURPOSE ONLY
        ///   TODO: CHANGE ACCORDINGLY.
        /// </summary>
        public void AddLog(IvleWorkbinFile file)
        {
            XmlNode node = LogDocument.CreateElement("Date");
            LogDocument.AppendChild(node);
        }

        #endregion
    }

    #endregion
}