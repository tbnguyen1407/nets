using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace frldrs_nets.UI
{
    /// <summary>
    /// This class is a temporary storage for the parsing parameters.
    /// The UI will access this object for information and take approriate
    /// action.
    /// </summary>
    class ParseResult
    {
        // type of the command
        public CommandType CmdType;

        // a list of profile names
        public List<string> ProfileNames = new List<string>();

        // a list of help topics
        public List<string> HelpTopics = new List<string>();

        public string SourceFolder, DestinationFolder;

        // Operation mode
        public string Mode;

        override public string ToString()
        {
            string result = "SyncCommand Type: " + CmdType;
            result += ("\nSource folder: " + SourceFolder);
            result += ("\nDestination folder: " + DestinationFolder);
            result += ("\nOperation Mode: " + Mode);
            result += ("\nProfile name: " + ((ProfileNames!=null && ProfileNames.Count>0)?ProfileNames.First():""));
            return result;
        }
    }

    enum CommandType
    {
        Generic,            // generic operation (mirror, equalize, etc.)
        Help,               // help related
        InitInteractiveMode,// initialize the interactive shell
        Exit,               // exit command
        Start,              // start an operation
        Save,               // save the current setting
        Run,                // run a profile
        Show                // display settings
    }
}
