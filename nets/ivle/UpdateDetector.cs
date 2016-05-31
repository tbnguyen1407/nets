#region USING DIRECTIVES

using System.Collections.Generic;
using System.IO;
using nets.dataclass;

#endregion

namespace nets.ivle
{
    /// <summary>
    ///   Main functionality: Detect the update on IVLE.
    ///   Input sources:
    ///   IVLE WORKBIN STRUCTURE
    ///   FILE HISTORY (FILE UPLOAD DATE ON IVLE)
    ///   LOCAL FILE SYSTEM
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class UpdateDetector
    {
        #region FIELD DECLARATION
        
        private readonly string IvleRootFolder;
        private readonly IvleWorkbin IvleWorkbin;

        public List<IvleWorkbinFile> NewFiles { get; private set; }
        public List<IvleWorkbinFolder> NewFolders { get; private set; } // List of new folders to be created.

        // @BENNY: QUICK (& DIRTY) TRICK TO DISPLAY MSG TO OUTPUT
        public IvleHandler ivleHandler { get; set; }

        #endregion

        #region CONSTRUCTORS

        public UpdateDetector(IvleWorkbin ivleWorkbin, string localIvleRootFolder)
            :this()
        {
            IvleWorkbin = ivleWorkbin;
            IvleRootFolder = (new DirectoryInfo(localIvleRootFolder)).FullName;
        }

        private UpdateDetector()
        {
            NewFiles = new List<IvleWorkbinFile>();
            NewFolders = new List<IvleWorkbinFolder>();
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        ///   Find differences from the database
        ///   and the workbin.
        /// </summary>
        public void FindDifference2()
        {
            // compare folder tree of this two.
            IvleWorkbin wb = InfoManager.LoadWorkbin(IvleWorkbin.WorkbinId);
        }

        /// <summary>
        ///   Find differences between Ivle Workbin and the local folder 
        ///   from the last synchronization.
        ///   TODO: FIND DIFFERENCES FROM THE DATA FILE INSTEAD OF NAVIGATING THROUGH
        ///   DIRECTORIES AND FILE => SAVE I/O TIME.
        /// </summary>
        /// <returns></returns>
        public void FindDifferences()
        {
            // Recursive call to compare.
            FindDifferences(IvleWorkbin.RootFolder, IvleRootFolder + IvleWorkbin.GetPath());
        }

        #endregion

        #region HELPERS

        /// <summary>
        ///   Helper method for GetDifferences.
        ///   Match the WFolder root with the DirectoryInfo directory.
        /// </summary>
        /// <param name = "root"></param>
        /// <param name = "directory"></param>
        private void FindDifferences(IvleWorkbinFolder root, string directory)
        {
            // Check for new files.
            foreach (IvleWorkbinFile file in root.Files)
            {
                string fileFullPath = directory + "\\" + file.FileName;

                if (File.Exists(fileFullPath))
                    file.IsNew = false;
                else
                {
                    // @BENNY: QUICK (& DIRTY) TRICK TO DISPLAY MSG TO OUTPUT
                    string fileToOutput = fileFullPath.Replace(IvleRootFolder, "").Replace(@"\/", @"/").Replace(@"/\", @"/").Replace(@"\", @"/");
                    ivleHandler.WriteToOutput("\t\tNew file: " + fileToOutput + "\n");
                    //Console.Write("New File: {0} ...To be downloaded.\n", fileFullPath);
                    NewFiles.Add(file);
                }
            }

            // Check for new folders.
            foreach (IvleWorkbinFolder folder in root.SubFolders)
            {
                string subDir = directory + "\\" + folder.FolderName;
                if (Directory.Exists(subDir))
                    FindDifferences(folder, subDir);
                else
                {
                    // This folder is new, add to the list -> to be created.
                    string folderToOutput = subDir.Replace(IvleRootFolder, "").Replace(@"\/", @"/").Replace(@"/\", @"/").Replace(@"\", @"/");
                    ivleHandler.WriteToOutput("\t\tNew folder: " + folderToOutput + "\n");
                    NewFolders.Add(folder);
                    // This folder is new, we just add all files ...
                    NewFiles.AddRange(folder.CollectAllFilesInside());
                    // and add all subfolders inside it.
                    NewFolders.AddRange(folder.CollectAllSubFoldersInside());
                }
            }
        }

        #endregion
    }
}