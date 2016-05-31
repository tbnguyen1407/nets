using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using nets.dataclass;
using nets.utility;

namespace nets.logic
{
    /// <summary>
    /// Detects differences between two folders being synchronized
    /// Author: Hoang Nguyen Nhat Tao + Nguyen Thi Yen Duong
    /// </summary>
    public class Detector
    {
        #region PRIVATE FIELD DECLARATION

        private List<FilePair> listOfDifferences = new List<FilePair>();
        
        #endregion

        #region PUBLIC METHODS

        public List<FilePair> GetDifferences(string srcFolderPath, string desFolderPath, ref Logger logger, ref bool errorOccur)
        {
            listOfDifferences = new List<FilePair>();
            RecordDifferences(srcFolderPath, desFolderPath, ref logger, ref errorOccur);
            return listOfDifferences;
        }

        #endregion

        #region PRIVATE HELPERS

        private void RecordDifferences(string srcFolderPath, string desFolderPath, ref Logger logger, ref bool errorOccur)
        {
            List<string> srcSubFiles = new List<string>();
            List<string> desSubFiles = new List<string>();
            FilePair filePair;

            try
            {
                srcSubFiles = Directory.GetFileSystemEntries(srcFolderPath).ToList();
                desSubFiles = Directory.GetFileSystemEntries(desFolderPath).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                ExceptionHandler.HandleException("Access denied for " + srcFolderPath + " or " + desFolderPath, ref logger, ref errorOccur);
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHandler.HandleException("Either " + srcFolderPath + " or " + desFolderPath + " can not be found", ref logger, ref errorOccur);
            }
            catch (Exception)
            {
                ExceptionHandler.HandleException("Unknow exception has occured while accessing " + srcFolderPath + " and " + desFolderPath, ref logger, ref errorOccur);
            }

            foreach (string filePath in srcSubFiles)
            {
                filePair = ConstructFilePair(filePath, srcFolderPath, desFolderPath, true);
                
                if (!(filePair.FileStatusInDes == FileStatus.Deleted || filePair.FileStatusInDes == FileStatus.NotExist))
                {
                    desSubFiles.Remove(filePair.DesFilePath);

                    switch (filePair.FilePairType)
                    {
                        case FileType.File:
                            if (FileSystemOperator.HaveSameContent(filePair.SrcFilePath, filePair.DesFilePath, FileType.File))
                                continue;
                            break;
                        case FileType.Folder:
                            RecordDifferences(filePair.SrcFilePath, filePair.DesFilePath, ref logger, ref errorOccur);
                            continue;
                    }
                }

                listOfDifferences.Add(filePair);
            }

            // Just need to consider files/folders in destination but not in source
            foreach (string filePath in desSubFiles)
            {
                filePair = ConstructFilePair(filePath, desFolderPath, srcFolderPath, false);
                listOfDifferences.Add(filePair);
            }
		}

        private static FilePair ConstructFilePair(string srcFilePath, string srcFolderPath, string desFolderPath, bool isSrc)
        {
            string relativeFilePath = PathOperator.GetRelativePath(srcFilePath, srcFolderPath);
            string desFilePath = PathOperator.GetAbsolutePath(desFolderPath, relativeFilePath);
            FileType filePairType = (File.Exists(srcFilePath)) ? FileType.File : FileType.Folder;
            FileStatus fileStatusInSrc = FileSystemOperator.GetFileStatus(srcFilePath, filePairType);
            FileStatus fileStatusInDes = FileSystemOperator.GetFileStatus(desFilePath, filePairType);

            return isSrc ?
                new FilePair(srcFilePath, desFilePath, fileStatusInSrc, fileStatusInDes, filePairType) :
                new FilePair(desFilePath, srcFilePath, fileStatusInDes, fileStatusInSrc, filePairType);
        }

        #endregion
    }
}
