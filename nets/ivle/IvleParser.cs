#region USING DIRECTIVES

using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using nets.dataclass;
using nets.utility;

#endregion

namespace nets.ivle
{
    /// <summary>
    /// This class is a static class to handle all the
    /// data extractions from the HTML documents that we
    /// retrieved. The main functionalities includes:
    /// 1/ Retrieve a list of workbin ID from the Html
    /// document of the workspace (in string format);
    /// 2/ Retrieve the workbin information from the 
    /// workbin page HTML (string);
    /// 3/ Retrieve a list of files (and their info) on
    /// a Workbin Folder's HTML document.
    /// Author: Vu An Hoa
    /// </summary>
    class IvleParser
    {
        #region FIELD DECLARATION

        static readonly char[] specialCharacters = { '*', '\"', '/', '\\', ':', ';', '|', '=', ',' };

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Get the list of all workbin IDs from the Html source
        /// of the workspace.
        /// </summary>
        /// <param name="workspaceHtml">Html string of the workspace</param>
        /// <returns>List of workbin Ids</returns>
        public static List<string> GetWorkbinIds(string workspaceHtml)
        {
            workspaceHtml = PreprocessHtml(workspaceHtml);
            List<string> result = new List<string>();
            const string pattern = "/workbin/workbin.aspx?WorkbinID=";
            int t2 = 0;
            while (true)
            {
                int t1 = workspaceHtml.IndexOf(pattern, t2);
                if (t1 == -1)
                    break;
                t2 = workspaceHtml.IndexOf('\'', t1);
                string workbinId = workspaceHtml.Substring(t1, t2 - t1).Substring(pattern.Length);
                result.Add(workbinId);
            }
            return result;
        }

        /// <summary>
        /// Parse a workbin html (of form workbin.aspx?WorkbinId=...)
        /// for the information: module tag, module code, workbin tag,
        /// folder tree,... BUT FILES ARE NOT PARSED.
        /// </summary>
        /// <param name="workbinHtml"></param>
        public static IvleWorkbin GetWorkbinStructure(string workbinHtml)
        {
            IvleWorkbin result = new IvleWorkbin();

            workbinHtml = PreprocessHtml(workbinHtml);

            // Fetch the module tag and module code and workbin id
            StringFormatPattern pattern = new StringFormatPattern("<font class='popupBannerHeaderTop-txt'>{0}<font class='popupBannerHeaderBottom-txt'>Workbin : {1}</font>");
            int i1 = workbinHtml.IndexOf("<font class='popupBannerHeaderTop-txt'>");
            int i2 = workbinHtml.IndexOf("</font>", i1) + 7;
            string[] m = pattern.MatchInput(workbinHtml.Substring(i1, i2 - i1));

            // Replace the character / by - to overcome the case when the module has two codes and are separated by the / character.
            // And also remove special characters (safety purpose).
            result.ModuleCode = RemoveSpecialCharacters(m[0].Replace('/', '-'));
            result.WorkbinName = RemoveSpecialCharacters(m[1].Trim());

            // Capture the folder structure from javascript source
            // Get the javascript that is used to generate the folder structure
            // We should remove all the <font>, <img>, etc. nodes (formatting nodes) to simplify our task.
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(workbinHtml);

            // Find the module code and workbin tag here
            //HtmlNodeCollection fontNodes = Doc.DocumentNode.SelectNodes("//font");
            //Console.WriteLine(fontNodes.Count);
            //foreach (HtmlNode node in fontNodes)
            //{
            //    if (node.NextSibling!=null && node.NextSibling.InnerText.Contains("Workbin"))
            //    {
            //        Console.WriteLine(node.OuterHtml);
            //        Console.WriteLine(node.NextSibling.OuterHtml);
            //        result.ModuleCode = node.InnerText;
            //        string temp = node.NextSibling.InnerText;
            //        result.WorkbinName = temp.Substring(temp.IndexOf(':')).Trim();
            //    }
            //}

            string folderTreeJavaScript = doc.GetElementbyId("ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_Dtree").FirstChild.InnerText;
            //Console.WriteLine(FolderTreeJavaScript);

            // Construct the folder tree from the script
            List<string> treeBuildCommands = FindAll(folderTreeJavaScript, "d.add(", ");");
            // Note that the number of folders = number of add commands & node id numbering is in sequence & root = 0
            IvleWorkbinFolder[] folders = new IvleWorkbinFolder[treeBuildCommands.Count];
            // Parent[i] stores the index of the parent of FoldersList[i]
            int[] parent = new int[treeBuildCommands.Count];

            StringFormatPattern commandPattern = new StringFormatPattern("d.add({0},{1},'{2}','{3}',{4});");
            StringFormatPattern folderUrlPattern = new StringFormatPattern("workbin.aspx?WorkbinId={0}&nodeSelectedIndex={1}");

            // Fetch folder information
            foreach (string command in treeBuildCommands)
            {
                //Console.WriteLine(command);   // Print out the javascript command for testing.

                // The javascript method "add" essentially has 5 arguments:
                // current node id; parent id; node label; node associated url; node tag; etc.
                // Note : after this, {4} might be of form a, b, c, ... where the rest are further arguments, we don't care about the rest and rule them out.
                string[] args = commandPattern.MatchInput(command);


                // Set the reference to parent
                int folderNodeNumber = int.Parse(args[0]);
                parent[folderNodeNumber] = int.Parse(args[1]);

                // get folder ID
                string[] a = folderUrlPattern.MatchInput(args[3]);
                // A[0] = workbin ID ; A[1] = folder ID
                string folderName = args[4].Substring(1, args[4].IndexOf('\'', 1) - 1);

                folders[folderNodeNumber] = new IvleWorkbinFolder(a[1], folderName, result);
                //Console.WriteLine(FoldersList[folderNodeNumber].ToString());
            }

            // Build the tree by setting appropriate references.
            for (int i = 1; i < folders.Length; i++)
            {
                // Add this folder to the list of its parent's subfolders
                folders[parent[i]].SubFolders.Add(folders[i]);
                // Set the parent of the current folder
                if (parent[i] >= 0) // Set if not root folder
                    folders[i].Parent = folders[parent[i]];
            }

            // Set the root folder
            result.RootFolder = folders[0];
            result.FoldersList = folders.ToList();
            return result;
        }

        /// <summary>
        /// Get the list of files from a folder Html document.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderHtmlDocument"></param>
        /// <returns></returns>
        public static List<IvleWorkbinFile> GetFileList(IvleWorkbinFolder folder, string folderHtmlDocument)
        {
            // List of all files found in the folder
            List<IvleWorkbinFile> result = new List<IvleWorkbinFile>();

            folderHtmlDocument = PreprocessHtml(folderHtmlDocument);

            // Get file table
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(folderHtmlDocument);
            const string fileFolderTableElementID = "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_dgFileFolder";
            HtmlNode fileFolderTable = doc.GetElementbyId(fileFolderTableElementID);

            // [DEBUG] CHECK CASE: FOLDER TABLE MUST EXISTS. THIS HAPPENS ONLY WHEN THE FOLDER CONTAINS SOME FILES.
            if (fileFolderTable != null)
            {
                // [DEBUG] CHECK CASE: FOLDER IS EMPTY.
                bool noFile = false;
                foreach (HtmlNode node in fileFolderTable.Descendants())
                {
                    if (node.InnerText.Equals("No data found!"))
                    {
                        noFile = true;
                        break;
                    }
                }

                if (!noFile)
                {
                    // Construct the visual table from HTML
                    // Select the Table Rows using Xpath
                    HtmlNodeCollection tableRows = fileFolderTable.SelectNodes("./tr");
                    int numberOfRows = tableRows.Count;
                    //Console.WriteLine("Number of rows: {0}", NumberOfRows);

                    // Answer should be 9 columns
                    int numberOfColumns = tableRows.First().SelectNodes("//th").Count + 1;
                    //Console.WriteLine("Number of headers: {0}", NumberOfColumns);

                    // Add one additional column for any URL detected from the table row
                    // This URL can be used to distinguish between FILE from FOLDER
                    string[,] table = new string[numberOfRows, numberOfColumns + 1];

                    int i = 0;
                    foreach (HtmlNode tableRow in tableRows)
                    {
                        int j = 0;
                        //Console.WriteLine(tableRow.OuterHtml);
                        foreach (HtmlNode tableData in tableRow.ChildNodes)
                        {
                            table[i, j] = tableData.InnerText.Trim();

                            // BUG FOUND: SOMETIMES FILE WITH COMMENTS DOES NOT PARSE CORRECTLY.
                            if (i > 0 && j == 6)
                            {
                                // Remove file/folder description if any.
                                // First child is the <a> file_name </a> and hence is the one that we want.
                                //table[i, j] = tableData.FirstChild.InnerText;
                                HtmlNode anchorNode = tableData.SelectSingleNode(".//a") ?? tableData.SelectSingleNode(".//A");
                                if (anchorNode != null)
                                {
                                    //Console.WriteLine(anchorNode.OuterHtml);

                                    // I know where the problem is: WHEN THE FILE IS NEW
                                    // THERE IS NODE CALLED <IMG...> TO INDICATE IT IS NEW!
                                    // ALSO NEED TO REMOVE IT.

                                    // Taking the InnerText might not be correct.
                                    // But the OuterHtml is verified to be correct.
                                    // So the problem might be due to the InnerText
                                    // is not properly set in HAP.

                                    table[i, j] = anchorNode.InnerText;

                                    // TAKE THE URL FROM HERE!
                                    //table[i, numberOfColumns] = anchorNode.GetAttributeValue("href","");
                                }
                            }

                            //if (j == 6 || j == 7 || j == 9)
                            //{
                            //    Console.Write(Table[i, j] + ",");
                            //}
                            j++;
                        }
                        table[i, numberOfColumns] = i != 0 ? tableRow.SelectNodes(".//@href").Last().GetAttributeValue("href", "") : "URL";
                        //Console.WriteLine(Table[i, NumberOfColumns]);
                        i++;
                    }

                    // These columns are for file/folder tag, size, date and url
                    int[] usefulColumns = { 6, 7, 9, numberOfColumns };
                    //string pattern = "/workbin/file_download.aspx?workbinid={0}&dwFolderId={1}&dwFileId={2}";
                    StringFormatPattern fileUrlPattern = new StringFormatPattern(IvleHandler.IVLE_FILEPATTERN);
                    for (i = 1; i < numberOfRows; i++)
                    {
                        string fileName = RemoveSpecialCharacters(table[i, usefulColumns[0]]);
                        string fileSize = table[i, usefulColumns[1]].Trim();
                        string fileDate = table[i, usefulColumns[2]].Trim();
                        string downloadUrl = table[i, usefulColumns[3]].Trim();

                        // IN CASE DATE = OPEN/CLOSE, THIS OBJECT MUST BE A SUBFOLDER
                        if (fileDate.Equals("Open") || fileDate.Equals("Close")) continue;

                        string[] a = fileUrlPattern.MatchInput(downloadUrl);
                        IvleWorkbinFile file = new IvleWorkbinFile(a[2], fileName, fileSize, fileDate);
                        result.Add(file);
                    }

                    // folder.Files = result;
                    // Extract file tag, file download link, file size and upload date
                }
            }

            return result;
        }


        #endregion

        #region HELPERS

        /// <summary>
        /// Preprocess the Html string. To reduce the workoad
        /// for the parser.
        /// </summary>
        /// <param name="htmltext">Input Html</param>
        /// <returns>"Good" Html</returns>
        private static string PreprocessHtml(string htmltext)
        {
            // REMOVE ALL br related tags
            htmltext = htmltext.Replace("<br>", "");
            htmltext = htmltext.Replace("</br>", "");
            htmltext = htmltext.Replace("<br />", "");
            htmltext = htmltext.Replace("<br/>", "");

            return htmltext;
        }

        /// <summary>
        /// Remove all special characters from the input string
        /// so that the input will become a valid file/folder tag.
        /// The input is also trimmed.
        /// </summary>
        /// <param name="input">A File/Folder tag (might contain unacceptable characters).</param>
        /// <returns>A valid file tag.</returns>
        private static string RemoveSpecialCharacters(string input)
        {
            foreach (char c in specialCharacters)
                input = input.Replace(c.ToString(), "");
            return input.Trim();
        }

        /// <summary>
        /// Find all occurences of form [start][somestring][end]
        /// in an input string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="startMarker">Start marker of the pattern.</param>
        /// <param name="endMarker">End marker of the pattern.</param>
        /// <returns>List of all substrings of first argument that starts with the second argument and ends with the third.</returns>
        private static List<string> FindAll(string input, string startMarker, string endMarker)
        {
            List<string> result = new List<string>();
            int currentPosition = 0;
            while (true)
            {
                // find the index of the startMarker from currentPosition
                int temp = input.IndexOf(startMarker, currentPosition);
                if (temp == -1)
                    break;
                currentPosition = input.IndexOf(endMarker, temp) + endMarker.Length;
                result.Add(input.Substring(temp, currentPosition - temp));
            }
            return result;
        }

        #endregion
    }
}
