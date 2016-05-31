using System.Collections.Generic;
using System.Text;

namespace nets.dataclass
{
    /// <summary>
    ///   Class to encapsulate workbin information including
    ///   workbin ID, workbin tag, module code, and its own
    ///   folder structure.
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class IvleWorkbin
    {
        public List<IvleWorkbinFolder> FoldersList; // list of folders to perform iterations.
        public string ModuleCode;
        public string ModuleName;

        public IvleWorkbinFolder RootFolder; // reference to root folder so that we can perform recursive operation (for instance, display folder tree).

        public string WorkbinId;
        public string WorkbinName;
        private string Path;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Module: {0} {1}\nWorkbin: {2}\nWorkbin ID: {3}", ModuleCode, ModuleName, WorkbinName, WorkbinId);
            return sb.ToString();
        }

        // Path to the folder of this workbin
        public string GetPath()
        {
            return Path ?? (Path = "/" + ModuleCode + "/" + WorkbinName + "/");
        }

        /// <summary>
        ///   Print the folder structure of this workbin to the
        ///   screen.
        /// </summary>
        public string PrintFolderTree()
        {
            return PrintFolderAppend(RootFolder, "");
        }

        /// <summary>
        ///   Print the folder structure starting from root
        ///   and append each line with start = a string of
        ///   white space. This method is used as helper 
        ///   method for the PrintFolderTree() to print the
        ///   entire folder structure of this workbin.
        /// </summary>
        /// <param name = "root">A folder to be the root.</param>
        /// <param name = "start">Prefix string to append to the beginning of each printed line.</param>
        private static string PrintFolderAppend(IvleWorkbinFolder root, string start)
        {
            StringBuilder sb = new StringBuilder();

            // Print the root folder
            sb.Append(start + "-+" + root.FolderName + " (" + root.FolderId + ")\n");
            //outputStr.Write(start + "-+" + root.FolderName + " (" + root.FolderId + ")\n");
            
            // Print all the files
            foreach (IvleWorkbinFile file in root.Files)
                sb.Append(start + "    > " + file.FileName + "\n");
                //outputStr.WriteLine(start + "    > " + file.FileName);
            
            // Recursive call on subfolders)
            foreach (IvleWorkbinFolder subfolder in root.SubFolders)
                PrintFolderAppend(subfolder, start + " |");
            return sb.ToString();
        }
    }
}