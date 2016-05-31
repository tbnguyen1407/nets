using System.Collections.Generic;
using System.Text;

namespace nets.dataclass
{
    /// <summary>
    ///   This class encapsulates the essential information
    ///   of a folder in an IVLE workbin. This information 
    ///   includes: Folder tag and ID (Folder tag is used
    ///   to make local copy and ID is used to form the URL
    ///   to this folder); its subfolders and the list of
    ///   all files that it contains.
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class IvleWorkbinFolder
    {
        public IvleWorkbin Container;
        public List<IvleWorkbinFile> Files;
        public string FolderId;
        public string FolderName;
        public bool IsClose;
        public IvleWorkbinFolder Parent;
        public List<IvleWorkbinFolder> SubFolders;
        private string Path;

        public IvleWorkbinFolder(string folderId, string folderName, IvleWorkbin container)
            : this()
        {
            FolderName = folderName;
            FolderId = folderId;
            Container = container;
        }

        private IvleWorkbinFolder()
        {
            SubFolders = new List<IvleWorkbinFolder>();
            Files = new List<IvleWorkbinFile>();
            IsClose = false;
        }

        // Collect all files containing inside this folder and its descendant.
        public List<IvleWorkbinFile> CollectAllFilesInside()
        {
            List<IvleWorkbinFile> result = new List<IvleWorkbinFile>();
            result.AddRange(Files);
            foreach (IvleWorkbinFolder folder in SubFolders)
                result.AddRange(folder.CollectAllFilesInside());
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            return sb.AppendFormat("Folder name = {0}\nIVLE ID = {1}\n", FolderName, FolderId).ToString();
        }

        /// <summary>
        ///   Get the full path of this folder on the workbin file system. 
        ///   The path is to be of form:
        ///   /workbin_module_code/workbin_name/[path...]/[this folder's tag]
        ///   This path is supposed to be identical on both server and local.
        /// </summary>
        /// <returns>_path to this folder</returns>
        public string GetPath()
        {
            return Path ?? (Path = (Parent == null) ? Container.GetPath() : Parent.GetPath() + FolderName + "/");
        }

        // Folder information.
        public List<IvleWorkbinFolder> CollectAllSubFoldersInside()
        {
            List<IvleWorkbinFolder> result = new List<IvleWorkbinFolder>();
            result.AddRange(SubFolders);
            foreach (IvleWorkbinFolder folder in SubFolders)
                result.AddRange(folder.CollectAllSubFoldersInside());
            return result;
        }

        public void FixParentReferenceInSubfolders()
        {
            foreach (IvleWorkbinFolder folder in SubFolders)
            {
                folder.Parent = this;
                folder.FixParentReferenceInSubfolders();
            }
        }
    }
}