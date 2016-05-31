using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace frldrs_nets.UI
{

    /// <summary>
    /// This class is the centralized command center which has 
    /// the task of passing all the job to the logic in the below
    /// organization.
    /// Each use case is now broken into several smaller command
    /// objects.
    /// By making use of the command center, the SyncCommand Line UI
    /// and the Graphical one is united. We don't have to write 
    /// processing code to both of them. Rather, we just need to
    /// handle the event and pass it as the ICommand object.
    /// Here is the lists of commands:
    ///
    /// Global operations:
    /// exit                    Exit the shell.
    /// help [topic/command]    Display help message.
    /// start                   Run the operation. The software will ask for informations and then run the synchronization.
    /// 
    /// Profiles and settings operations
    /// save %p                Save current settings under the name %p.
    /// run %p+                Run a list of specified profiles.
    /// show                   Show all the profiles stored.
    /// </summary>
    
    class InteractiveAgent
    {
        private bool isPresent;
        CommandHandler Handler;

        public InteractiveAgent()
        {
            isPresent = true;
            Handler = new CommandHandler(this);
        }

        /// <summary>
        /// Main function that do the read-eval loop: read the
        /// command from the user, parse the command and then
        /// execute the command.
        /// </summary>
        public void Show()
        {
            while (isPresent)
            {
                Console.Write("\nNETS > ");
                string command = Console.ReadLine();
                try
                {
                    // parse the command
                    ParseResult result = CommandParser.ParseInteractiveCommand(command);
                    // execute the command
                    this.Handler.Execute(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid command!");
                    //Console.WriteLine("Exception {0}", e);
                }
            }
        }

        public void Close()
        {
            this.isPresent = false;
        }
    }
}
