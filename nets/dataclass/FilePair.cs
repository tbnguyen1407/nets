namespace nets.dataclass
{
    /// <summary>
    /// Describe the status of a pair of corresponding files in source and destination folder.
    /// Author: Hoang Nguyen Nhat Tao + Nguyen Thi Yen Duong
    /// </summary>
    public class FilePair
    {
        public string SrcFilePath { get; private set; }
        public string DesFilePath { get; private set; }

        public FileStatus FileStatusInSrc { get; private set; }
        public FileStatus FileStatusInDes { get; private set; }

        public FileType FilePairType { get; private set; }

        public FilePair(string srcFilePath, string desFilePath, FileStatus fileStatusInSrc, FileStatus fileStatusInDes, FileType filePairType)
        {
            SrcFilePath = srcFilePath;
            DesFilePath = desFilePath;

            FileStatusInSrc = fileStatusInSrc;
            FileStatusInDes = fileStatusInDes;

            FilePairType = filePairType;
        }
    }
}
