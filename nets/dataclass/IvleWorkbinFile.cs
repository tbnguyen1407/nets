using System.Text;

namespace nets.dataclass
{
    /// <summary>
    ///   Encapsulation of a file in an IVLE folder.
    ///   Information about files includes: File ID, Folder ID, 
    ///   Workbin ID, Name, Size, Upload/Download Date.
    /// Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class IvleWorkbinFile
    {
        // File ID and the 
        public IvleWorkbinFolder Container;
        public string FileId;

        // To check and sync with the local information
        public string FileName;
        public string FileSize;
        public bool IsNew;
        public string UploadDate;
        private string Path;

        public IvleWorkbinFile(string fileId, string fileName, string fileSize, string uploadDate)
            : this(fileId, fileName)
        {
            FileSize = fileSize;
            UploadDate = uploadDate;
        }

        public IvleWorkbinFile(string fileId, string fileName)
            : this()
        {
            FileName = fileName;
            FileId = fileId;
        }


        private IvleWorkbinFile()
        {
            IsNew = true;
        }

        /// <summary>
        ///   Get the full path of this file.
        ///   _path should be identical on both server and local.
        ///   Use this information to decide on old and new file.
        /// </summary>
        /// <returns>_path to this file.</returns>
        public string GetPath()
        {
            return Path ?? (Path = Container.GetPath() + FileName);
        }

        public override string ToString()
        {
            return (new StringBuilder()).AppendFormat("File name: {0} ({1})\tUpload date: {2}\nWorkbin ID: {3}\nFolder ID: {4}\nFile ID: {5}\n", FileName, FileSize, UploadDate, Container.Container, Container, FileId).ToString();
        }
    }
}