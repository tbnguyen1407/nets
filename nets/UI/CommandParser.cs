using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace frldrs_nets.UI
{
    /// <summary>
    /// This static class contains all the operations to parse the command line
    /// and interactive commands.
    /// </summary>
    static class CommandParser
    {
        
        /// <summary>
        /// Parse the command line input to get the data.
        /// </summary>
        /// <param name="args"></param>
        static public ParseResult Parse(string[] args)
        {
            ParseResult result = new ParseResult();
            if (args.Length == 0)
                result.CmdType = CommandType.InitInteractiveMode;
            else if (args[0].Equals("-h") || args[0].Equals("--help"))
            {
                // a help invoke
                result.CmdType = CommandType.Help;
                for (int i = 1; i < args.Length; i++)
                    result.HelpTopics.Add(args[i]);
            }
            else if (args[0].Equals("-r") || args[0].Equals("--run-profile"))
            {
                // a Run Profiles command
                result.CmdType = CommandType.Run;
                for (int i = 1; i < args.Length; i++)
                    result.ProfileNames.Add(args[i]);
            }
            else
            {
                result.Mode = "Mirror";
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-m") || args[i].Equals("--mirror"))
                    {
                    }
                    else if (args[i].Equals("-e") || args[i].Equals("--equalize"))
                    {
                        result.Mode = "Equalize";
                    }
                    else if (args[i].Equals("-p") || args[i].Equals("--profile-name"))
                    {
                        Debug.Assert(i + 1 < args.Length);
                        result.ProfileNames.Add(args[++i]);
                    }
                    else if (Directory.Exists(args[i]))
                    {
                        result.SourceFolder = args[i];
                        Debug.Assert(i + 1 < args.Length);
                        result.DestinationFolder = args[++i];
                    }
                    else
                        throw new ApplicationException("Unknown command");
                }
            }
            return result;
        }

        /// <summary>
        /// Parse the command of the interactions into executable form.
        /// </summary>
        /// <param name="command"></param>
        static public ParseResult ParseInteractiveCommand(string command)
        {
            command.Trim();
            ParseResult result = new ParseResult();

            // Identify the category of the command
            try
            {
                result.CmdType = (CommandType)Enum.Parse(typeof(CommandType), FirstToken(ref command), true);
            }
            catch (Exception)
            {
                throw new Exception("Invalid command!");
            }
            // Test the parse result
            Console.WriteLine(result.CmdType);

            // Parse the parameters based on the command type
            switch (result.CmdType)
            {
                case CommandType.Exit: case CommandType.Start:
                    break;
                case CommandType.Help:
                    // load the help topics that the user wants
                    while (!string.IsNullOrEmpty(command))
                        result.HelpTopics.Add(FirstToken(ref command));
                    break;
                case CommandType.Run:
                    // load the profiles that the user wants to run
                    while (!string.IsNullOrEmpty(command))
                        result.ProfileNames.Add(FirstToken(ref command));
                    break;
                case CommandType.Save:
                        result.ProfileNames.Add(command);
                    break;
                case CommandType.Show:
                    // load the profiles that the user wants to see
                    while (!string.IsNullOrEmpty(command))
                        result.ProfileNames.Add(FirstToken(ref command));
                    break;
                default:
                    break;
            }            
            return result;
        }


        /// <summary>
        /// This function extract the first word of the string and remove it 
        /// from the string.
        /// </summary>
        /// <param name="tokens">A long sentence to extract the word.</param>
        /// <returns>The first word. For instance, with input "This is a new string", the output will be "This" and the original string becomes "is a new string".</returns>
        static private string FirstToken(ref string tokens)
        {
            if (tokens == null)
                return null;
            else
            {
                tokens = tokens.TrimStart(' ');
                if (tokens.Equals(""))
                    return null;

                int k = tokens.IndexOf(' ');
                // the (non-empty) string has no more space character so it is a word.
                if (k == -1) 
                {
                    string result = tokens;
                    tokens = "";
                    return result;
                }
                else
                {
                    string result = tokens.Substring(0, k);
                    tokens = tokens.Substring(k).TrimStart(' ');
                    return result;
                }
            }
        }
    }
}
