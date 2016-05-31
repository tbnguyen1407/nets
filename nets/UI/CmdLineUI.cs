using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace frldrs_nets.UI
{
    /// <summary>
    /// Main class for ICommand Line User Interface.
    /// This interface supports two modes of operations:
    /// - Direct input from console:
    /// Usage: nets [options]
    /// 
    /// When there is no options, the interactive mode is invoked.
    /// - Interactive prompt:
    /// This offers an easy way for user that cannot remember
    /// all the command line options.
    /// The program will parse the user input and execute directly.
    /// For instance, a flow of interaction can be: ("NETS > " is
    /// the internal prompt)
    /// C:\> nets
    /// NETS > help
    /// ...
    /// 
    /// NETS > set source folder "C:\Documents\"
    /// Source folder set!
    /// 
    /// NETS > set destination folder "D:\Documents"
    /// Destination folder set!
    /// 
    /// NETS > sync
    /// ...
    /// Synchronization complete!
    /// 
    /// NETS > quit
    /// Bye, bye!
    /// </summary>
    class CmdLineUI
    {
        private CommandHandler Handler;

        public CmdLineUI()
        {
        }

        public void Show(string[] args)
        {
            Console.Write("__   _ ____ ______  ____   \n");
            Console.Write("||\\  | |      ||    |_  \\ \n");
            Console.Write("|| \\ | |===   ||      \\\\   \n");
            Console.Write("||  \\| |___   ||    \\___|  \n");
            Console.Write("NETS - Nothing Else To Sync\nVersion 0.0\n");
            ParseResult result = CommandParser.Parse(args);

            Handler = new CommandHandler(this);
            Handler.Execute(result);
        }
    }
}
