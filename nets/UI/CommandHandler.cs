//#define UNITTEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if !UNITTEST
using frldrs_nets.Logic;
#endif

namespace frldrs_nets.UI
{
    /// <summary>
    /// Static class to handle the execution of commands.
    /// </summary>
    class CommandHandler
    {
        private Object Caller;

        public CommandHandler(Object caller)
        {
            if (caller is CmdLineUI || caller is InteractiveAgent)
                this.Caller = caller;
        }

        /// <summary>
        /// Main functionality.
        /// Execute the command with parameters given in the ParseResult object.
        /// </summary>
        /// <param name="parameters"> contains all the parameters for a command.</param>
        /// <returns></returns>
        public bool Execute(ParseResult parameters)
        {
            switch (parameters.CmdType)
            {
                // GENERAL COMMANDS
                case CommandType.InitInteractiveMode:
                    (new InteractiveAgent()).Show();
                    break;

                case CommandType.Exit:
                    if (Caller is InteractiveAgent)
                        ((InteractiveAgent)Caller).Close();
                    Environment.Exit(0);
                    break;

                case CommandType.Help:
                    if (Caller is CmdLineUI)
                        HelpCenter.ShowCmdLineHelp(parameters.HelpTopics.ToArray());
                    else
                        HelpCenter.ShowInteractiveHelp(parameters.HelpTopics.ToArray());
                    break;

                // PROFILE COMMANDS
                case CommandType.Run:
                    foreach (string profileName in parameters.ProfileNames)
#if !UNITTEST
                        LogicFacade.ExecuteProfile(profileName);
#else
                        Console.WriteLine("Executing profile {0}... Done!", profileName);
#endif
                    break;

                case CommandType.Save:
                    try
                    {
#if !UNITTEST
                        LogicFacade.SaveCurrentProfile();
#else
                        Console.WriteLine("Saving current profiles...");
#endif
                    }
                    catch (Exception)
                    {
                        Console.Write("Profile save failed!\n");
                    }
                    break;

                case CommandType.Show:
                    if (parameters.ProfileNames == null || parameters.ProfileNames.Count == 0)
                    {
#if !UNITTEST
                        // Retrieve the list of all profile and show their names.
                        Console.WriteLine("Saved profiles: ");
                        List<string> profiles = LogicFacade.GetProfileNameList();
                        if (profiles == null || profiles.Count == 0)
                            Console.WriteLine("No profile found!");
                        else
                            foreach (string profileName in profiles)
                                Console.WriteLine(profileName);
#else
                        Console.WriteLine("Retrieve and display the list of profiles...");
#endif
                    }
                    else
                    {
#if !UNITTEST
                        // retrieve and display information for each profile selected
                        foreach (string profileName in parameters.ProfileNames)
                        {
                            Console.WriteLine("Profile {0}: ", profileName);
                            Console.Write("Source folder: {0}\nDestination folder: {1}\nOperation: {2}",
                                LogicFacade.GetProfileInfo(InfoType.FirstFolder, profileName),
                                LogicFacade.GetProfileInfo(InfoType.SecondFolder, profileName),
                                LogicFacade.GetProfileInfo(InfoType.SyncMode, profileName));
                        }
#else
                        Console.WriteLine("Retrieve and display the settings for chosen profiles...");
#endif
                    }
                    break;

                case CommandType.Start:
                    Start();
                    break;

                case CommandType.Generic:
#if !UNITTEST
                    // set the source, destination and mode of operation
                    LogicFacade.SetCurrentProfile(InfoType.FirstFolder, parameters.SourceFolder);
                    LogicFacade.SetCurrentProfile(InfoType.SecondFolder, parameters.DestinationFolder);
                    LogicFacade.SetCurrentProfile(InfoType.SyncMode, parameters.Mode);

                    // save the profile if it is set
                    if (parameters.ProfileNames.Count != 0)
                    {
                        string profileName = parameters.ProfileNames.First();
                        LogicFacade.SetCurrentProfile(InfoType.ProfileName, profileName);
                        if (LogicFacade.SaveCurrentProfile())
                            Console.Write("Profile " + profileName + " is saved.\n");
                        else
                            throw new ApplicationException("Cannot save profile under the name " + profileName + ".");
                    }
                    LogicFacade.ExecuteCurrentProfile();
#endif
                    break;

                default:
                    throw new ApplicationException("Invalid command type!");
            }
            return true;
        }

        /// <summary>
        /// Run a new synchronization in a query-response manner.
        /// </summary>
        private void Start()
        {
            // ask for source folder
            string sourceFolder, destinationFolder;
            while (true)
            {
                Console.Write("Source folder: ");
                sourceFolder = Console.ReadLine();
                if (!Directory.Exists(sourceFolder))
                    Console.Write("Source directory does not exist!\n");
                else
                    break;
            }
            // ask for destination folder
            while (true)
            {
                Console.Write("Destination folder: ");
                destinationFolder = Console.ReadLine();
                if (!Directory.Exists(destinationFolder))
                    Console.Write("Source directory does not exist!\n");
                else
                    break;
            }
            Console.Write("What type of synchronization you would like to perform?\n[1] Mirror (one-way synchronization)\n[2] Equalize (two-way synchronization)\nThe default operation is Mirror (either hit [Enter] or [1]). Otherwise, the second mode will be chosen.\n");
            Console.Write("Your choice: ");
            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.Enter || k.Key == ConsoleKey.D1)
            {
                Console.WriteLine("Mirror");
#if !UNITTEST
                LogicFacade.SetCurrentProfile(InfoType.SyncMode, SyncMode.Mirror.ToString());
#endif
            }
            else
            {
                Console.WriteLine("Equalize");
#if !UNITTEST
                LogicFacade.SetCurrentProfile(InfoType.SyncMode, SyncMode.Equalize.ToString());
#endif
            }
#if !UNITTEST
            LogicFacade.SetCurrentProfile(InfoType.FirstFolder, sourceFolder);
            LogicFacade.SetCurrentProfile(InfoType.SecondFolder, destinationFolder);
            LogicFacade.ExecuteCurrentProfile();
#endif
            Console.WriteLine();
#if !UNITTEST
			LogicFacade.PrintDifferences();
#endif
			Console.WriteLine();
            Console.WriteLine("Finished!");
        }
    }
}
