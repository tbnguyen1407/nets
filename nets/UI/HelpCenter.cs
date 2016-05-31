using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace frldrs_nets.UI
{
    static class HelpCenter
    {
        /// <summary>
        /// Locate the text of the help file. The content of the help
        /// file will be fetched in order to be displayed. Change accordingly
        /// for specific system.
        /// </summary>
        private static string ApplicationFolder = Path.GetDirectoryName(Application.ExecutablePath);
        private static string HelpFileLocation = ApplicationFolder + @"\CommandLineHelp.txt";
        private static string InteractiveHelpFileLocation = ApplicationFolder + @"\InteractiveHelp.txt";
        private static string HelpContent;

        /// <summary>
        /// Load the help content from the disk if it is not available.
        /// </summary>
        /// <param name="helpFile"></param>
        private static void LoadHelp(string helpFile)
        {
            if (string.IsNullOrEmpty(HelpContent))
            {
                FileInfo fi = new FileInfo(helpFile);
                StreamReader reader = fi.OpenText();
                HelpContent = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Print the command line help
        /// </summary>
        /// <param name="topics"></param>
        public static void ShowCmdLineHelp(string[] topics)
        {
            LoadHelp(HelpFileLocation);
            Console.WriteLine(HelpContent);
        }

        public static void ShowInteractiveHelp(string[] topics)
        {
            LoadHelp(InteractiveHelpFileLocation);
            if (topics == null || topics.Length == 0)
            {
                // display the general help
                Console.WriteLine(HelpContent);
            }
            else
            {
                // display the help for topic
            }
        }
    }
}
