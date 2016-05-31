#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using nets.dataclass;
using nets.gui;
using nets.utility;

#endregion

namespace nets.ivle
{
    /// <summary>
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class IvleHandler
    {
        #region CONSTANT URL STRINGS

        private const string IVLE_HOMEPAGE = "https://ivle.nus.edu.sg"; // Ivle login page
        private const string IVLE_WORKBINPREFIX = "/workbin/workbin.aspx?WorkbinId="; // Ivle workbin base address. Workbin Url = IvleHomeUrl + Prefix + WorkbinId
        private const string IVLE_FOLDERPATTERN = "/workbin/workbin.aspx?WorkbinId={0}&nodeSelectedIndex={1}"; // A workbin folder url pattern. {0} is to be replaced by workbin id and {1} is for folder id.
        public const string IVLE_FILEPATTERN = "/workbin/file_download.aspx?workbinid={0}&dwFolderId={1}&dwFileId={2}"; // File download url pattern. {0} is workbin id, {1} is folder id and {2} is file id

        #endregion

        #region FIELD DECLARATION

        private readonly string LocalFolder;
        private readonly ExtendedWebClient ivleClient; // webClient to handle network requests
        
        private readonly List<IvleWorkbin> workbinList;
        private string Password;
        private string Username;
        private string htmlDocText;
        private bool IsLoggedIn;
        private List<string> workbinIdList;
        public PageIVLE outputPage { get; set; } // Instance of the page to output message (the page must have WriteToOutput())
        // Replace pageIvleInstance.WriteToOutput() with Console.WriteLine() to write to console

        #endregion

        #region CONSTRUCTORS

        public IvleHandler(string username, string password, string localFolder)
            : this()
        {
            Username = username;
            Password = password;
            LocalFolder = localFolder;
        }

        private IvleHandler()
        {
            ivleClient = new ExtendedWebClient();
            IsLoggedIn = false;
            htmlDocText = "";
            workbinIdList = new List<string>();
            workbinList = new List<IvleWorkbin>();
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        ///   Request username and password (Console)
        /// </summary>
        public void CredentialRequest()
        {
            Console.Write("NUSNET ID: ");
            Username = Console.ReadLine();
            if (Username == null || Username.Equals(""))
                throw new Exception("Invalid User ID.");
            Username = Username.ToUpper();

            Console.Write("Password: ");
            var password = new StringBuilder();
            while (true)
            {
                var keyinfo = Console.ReadKey(true);
                if (keyinfo.Key != ConsoleKey.Enter)
                    password.Append(keyinfo.KeyChar);
                else
                    break;
            }
            Password = password.ToString();
        }

        /// <summary>
        ///   Log into the IVLE Homepage
        /// </summary>
        public bool LogIn()
        {
            WriteToOutput("Logging into IVLE...\n");

            Credential.SetCredential(Username, Password);
            ivleClient.SetRequest(IVLE_HOMEPAGE, Credential.FormFields, Credential.FormValues);

            try
            {
                htmlDocText = ivleClient.GetResponseString();
                IsLoggedIn = htmlDocText.IndexOf("Workspace") != -1;

                if (!IsLoggedIn)
                    WriteToOutput("Error: Incorrect NUSNET ID or password!\n");
                else
                    WriteToOutput("Logged in with username " + Username + "\n");
            }
            catch (Exception)
            {
                WriteToOutput("Error: No network connection!\n");
            }

            return IsLoggedIn;
        }

        /// <summary>
        ///   Get workbin ids from workspace.aspx html
        /// </summary>
        public List<string> GetWorkbinIds()
        {
            // Get list of workbinID
            if (!IsLoggedIn || htmlDocText == "")
            {
                WriteToOutput("Error: Not logged in!\n");
                return null;
            }

            WriteToOutput("Retrieving workbins...Please wait...\n");

            workbinIdList = IvleParser.GetWorkbinIds(htmlDocText);
            InfoManager.AddUser(Username, workbinIdList);

            // Get all workbin info
            foreach (string workbinId in workbinIdList)
                GetWorkbinInfo(workbinId);

            // Output list of workbins for user to choose
            List<string> moduleOutputList = new List<string>();

            foreach (IvleWorkbin workbin in workbinList)
                moduleOutputList.Add(workbin.ModuleCode + ": " + workbin.WorkbinName);

            WriteToOutput(workbinList.Count + " workbin(s) retrieved. Please check the workbin(s) you want and press Sync!\n");
            return moduleOutputList;
        }

        #region @BENNY: NEW METHODS TO SUPPORT BASIC CANCELLATION
        
        /// <summary>
        /// Get file information
        /// </summary>
        /// <param name="moduleOutputList"></param>
        public void GetFiles(List<string> moduleOutputList)
        {
            WriteToOutput("Getting workbin information...\n");
            
            try
            {
                foreach (IvleWorkbin workbin in workbinList.Where(workbin => moduleOutputList.Contains(workbin.ModuleCode + ": " + workbin.WorkbinName)))
                {
                    foreach (IvleWorkbinFolder folder in workbin.FoldersList)
                        GetFiles(folder);
                    InfoManager.AddWorkbin(workbin);
                }
            }
            catch (Exception)
            {
                WriteToOutput("Error: No network connection!");
            }
        }

        /// <summary>
        /// Detect differences
        /// </summary>
        /// <param name="moduleOutputList"></param>
        /// <returns></returns>
        public LocalUpdater DetectDifferences(List<string> moduleOutputList)
        {
            WriteToOutput("Detecting new and updated folders/files...\n");
            LocalUpdater updater = new LocalUpdater(ivleClient, LocalFolder);

            try
            {
                foreach (IvleWorkbin workbin in workbinList.Where(workbin => moduleOutputList.Contains(workbin.ModuleCode + ": " + workbin.WorkbinName)))
                {
                    UpdateDetector detector = new UpdateDetector(workbin, LocalFolder) { ivleHandler = this };
                    detector.FindDifferences();
                    // Add to the queue and execute
                    updater.NewFolders.AddRange(detector.NewFolders);
                    updater.NewFiles.AddRange(detector.NewFiles);
                }
            }
            catch (Exception)
            {
                WriteToOutput("Error: No network connection!");
            }
            return updater;
        }

        #endregion

        /// <summary>
        ///   Sync a module list chosen by user
        /// </summary>
        /// <param name = "moduleOutputList"></param>
        public void Sync(List<string> moduleOutputList)
        {
            try
            {
                //List<Thread> threadList = new List<Thread>();

                foreach (IvleWorkbin workbin in workbinList.Where(workbin => moduleOutputList.Contains(workbin.ModuleCode + ": " + workbin.WorkbinName)))
                {
                    WriteToOutput("\tProcessing " + workbin.ModuleCode + " " + workbin.WorkbinName + "...\n");
                    Sync(workbin);

                    //Thread workbinThread = new Thread(() => SyncWorkbin(workbin));
                    //workbinThread.SetApartmentState(ApartmentState.STA);
                    //threadList.Add(workbinThread);
                    //workbinThread.Start();
                }
                //Wait for all threads to finish
                //foreach (Thread thread in threadList)
                //    thread.Join();
            }
            catch (Exception)
            {
                WriteToOutput("Error: No network connection!");
            }
        }

        private void Sync(IvleWorkbin ivleWorkbin)
        {
            try
            {
                // Get workbin file list recursively)
                foreach (IvleWorkbinFolder folder in ivleWorkbin.FoldersList)
                    GetFiles(folder);

                InfoManager.AddWorkbin(ivleWorkbin);

                // Detect new and updated files
                UpdateDetector detector = new UpdateDetector(ivleWorkbin, LocalFolder) {ivleHandler = this};
                detector.FindDifferences();

                // Add to the queue and execute
                LocalUpdater updater = new LocalUpdater(ivleClient, LocalFolder);
                updater.NewFolders.AddRange(detector.NewFolders);
                updater.NewFiles.AddRange(detector.NewFiles);
                updater.Synchronize();
            }
            catch (Exception)
            {
                WriteToOutput("Error: Unexpected error occured!");
            }
        }

        /// <summary>
        ///   collect information for all workbins
        /// </summary>
        public void GetWorkbinInfo()
        {
            WriteToOutput("Getting workbin information...");

            try
            {
                //List<Thread> listThread = new List<Thread>();

                // Start parsing each workbin in a seperate thread
                foreach (string workbinId in workbinIdList)
                {
                    GetWorkbinInfo(workbinId);
                    //string id = workbinId;
                    //Thread workbinThread = new Thread(() => GetWorkbinInfo(id));
                    //workbinThread.SetApartmentState(ApartmentState.STA);
                    //listThread.Add(workbinThread);
                    //workbinThread.Start();
                }

                // Wait for all threads to finish
                //foreach (Thread thread in listThread)
                //{
                //    thread.Join();
                //}
            }
            catch (Exception)
            {
                WriteToOutput("Error: No network connection!");
            }
        }

        /// <summary>
        ///   retrieve information about a specific workbin
        /// </summary>
        /// <param name = "workbinId"></param>
        private void GetWorkbinInfo(string workbinId)
        {
            try
            {
                // TODO: CHECK IF WORKBIN ALREADY PARSED BEFORE PRINTING

                ExtendedWebClient newClient = new ExtendedWebClient(ivleClient._cookies);
                newClient.SetRequest(IVLE_HOMEPAGE + IVLE_WORKBINPREFIX + workbinId);
                string newHtmlDocText = newClient.GetResponseString();

                IvleWorkbin newIvleWorkbin = IvleParser.GetWorkbinStructure(newHtmlDocText);
                newIvleWorkbin.WorkbinId = workbinId;
                workbinList.Add(newIvleWorkbin);
                InfoManager.AddWorkbin(newIvleWorkbin);

                //Console.WriteLine(newWorkbin.ToString());
                //newWorkbin.PrintFolderTree();
            }
            catch (Exception)
            {
            }
        }
 
        /// <summary>
        ///   Collect all the files inside a workbin.
        ///   Requirement: workbin html is already parsed so
        ///   that folder structure is present.
        /// </summary>
        /// <param name = "listOfWorkbins"></param>
        public void GetFiles(List<IvleWorkbin> listOfWorkbins)
        {
            WriteToOutput("Please wait, getting file information...");

            try
            {
                List<Thread> listThread = new List<Thread>();

                // Start parsing each workbin in a seperate thread
                foreach (IvleWorkbin workbin in listOfWorkbins)
                {
                    IvleWorkbin curIvleWorkbin = workbin;
                    Thread workbinThread = new Thread(() => GetFiles(curIvleWorkbin));
                    workbinThread.SetApartmentState(ApartmentState.STA);
                    listThread.Add(workbinThread);
                    workbinThread.Start();
                }

                // Wait for all threads to finish
                foreach (Thread thread in listThread)
                {
                    thread.Join();
                }
            }
            catch (Exception)
            {
                WriteToOutput("Error: Error getting file information\n");
            }
        }

        /// <summary>
        ///   Collect all files in a workbin.
        /// </summary>
        private void GetFiles(IvleWorkbin ivleWorkbin)
        {
            if (ivleWorkbin == null)
                return;

            try
            {
                foreach (IvleWorkbinFolder folder in ivleWorkbin.FoldersList)
                    GetFiles(folder);
                InfoManager.AddWorkbin(ivleWorkbin);
            }
            catch (WebException)
            {
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///   Collect all the files inside a folder.
        ///   Requirement: the folder structures is ready.
        /// </summary>
        /// <param name="ivleWorkbinFolder"></param>
        private void GetFiles(IvleWorkbinFolder ivleWorkbinFolder)
        {
            try
            {
                ExtendedWebClient newClient = new ExtendedWebClient(ivleClient._cookies);
                newClient.SetRequest(ConstructUrl(ivleWorkbinFolder));
                string newHtmlDocText = newClient.GetResponseString();

                var fileList = IvleParser.GetFileList(ivleWorkbinFolder, newHtmlDocText);
                foreach (var file in fileList)
                {
                    ivleWorkbinFolder.Files.Add(file);
                    file.Container = ivleWorkbinFolder;
                    //Console.WriteLine(file.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///   Helper method to construct Url of a given object.
        /// </summary>
        /// <param name="obj">Object that is either workbin, folder or file to construct url.</param>
        /// <returns>Url of the object.</returns>
        public static string ConstructUrl(Object obj)
        {
            var sb = new StringBuilder();
            if (obj is IvleWorkbin)
                return IVLE_HOMEPAGE + IVLE_WORKBINPREFIX + ((IvleWorkbin) obj).WorkbinId;
            if (obj is IvleWorkbinFolder)
                return
                    sb.AppendFormat(IVLE_HOMEPAGE + IVLE_FOLDERPATTERN, ((IvleWorkbinFolder) obj).Container.WorkbinId,
                                    ((IvleWorkbinFolder) obj).FolderId).ToString();
            if (obj is IvleWorkbinFile)
                return
                    sb.AppendFormat(IVLE_HOMEPAGE + IVLE_FILEPATTERN, ((IvleWorkbinFile) obj).Container.Container.WorkbinId,
                                    ((IvleWorkbinFile) obj).Container.FolderId, ((IvleWorkbinFile) obj).FileId).ToString();
            // Object is not of valid type.
            throw new Exception("ConstructUrl: Type error!");
        }

        /// <summary>
        /// Write to output
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToOutput(string msg)
        {
            outputPage.WriteToOutput(msg);
        }

        #endregion

        #region PRIVATE HELPERS

        private void PrintWorkbinIds()
        {
            foreach (string workbinId in workbinIdList)
                WriteToOutput(workbinId + "\n");
        }

        private void PrintWorkbinInfo()
        {
            foreach (IvleWorkbin workbin in workbinList)
            {

                WriteToOutput(workbin + "\n");
                //WriteToOutput(workbin.ToString());
                //Console.WriteLine(workbin.ToString());
                //workbin.PrintFolderTree(outputStr);
                //outputStr.WriteLine("\n\n\n");
                //WriteToOutput("\n\n\n");
                //Console.Write("\n\n\n");
            }
        }

        private void Download(string url, string localpath, string filename)
        {
            ivleClient.DownloadFile(url, localpath, filename);
        }

        /// <summary>
        ///   ACCELERATOR TO SUPPORT: INTERNET DOWNLOAD MANAGER, FLASHGET, ETC.
        /// </summary>
        /// <param name = "url"></param>
        /// <param name = "localPath"></param>
        /// <param name = "fileName"></param>
        /// <param name = "accelerator"></param>
        private void DownloadFileWithAccelerator(string url, string localPath, string fileName, string accelerator)
        {
            var proc = new Process
                           {
                               StartInfo =
                                   {
                                       FileName = "idman",
                                       Arguments = "/d " + url +
                                                   "/p " + localPath + "/f " + fileName + "/q /n"
                                   }
                           };
            proc.Start();
        }

        #endregion
    }
}