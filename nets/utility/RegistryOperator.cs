#region USING DIRECTIVES

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Provides operations on Windows Registry
    /// Author: Tran Binh Nguyen
    /// </summary>
    public static class RegistryOperator
    {
        #region MAIN METHODS

        /// <summary>
        /// Register context menu entries
        /// </summary>
        /// <returns></returns>
        public static bool RegisterContextMenu()
        {
            string regAsmPath = GetRegAsmPath();

            if (regAsmPath == null)
            {
                MessageBox.Show("Please install .NET Framework 2.0 to use the right-click features!", 
                                ".NET Framework 2.0 required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
                                             {
                                                 FileName = regAsmPath,
                                                 Arguments =
                                                     "nets-contextmenuhandler.dll /tlb:nets-contextmenuhandler.tlb /codebase",
                                                 WindowStyle = ProcessWindowStyle.Hidden
                                             };
            Process.Start(startInfo);
            Process p = Process.Start(startInfo);
            p.WaitForExit();

            try
            {
                bool entryExists = ContextMenuRegistryExists();
                return entryExists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Register context menu entries
        /// </summary>
        /// <returns></returns>
        public static bool UnregisterContextMenu()
        {
            string regAsmPath = GetRegAsmPath();

            if (regAsmPath == null)
            {
                MessageBox.Show("Please install .NET Framework 2.0 to remove the right-click features!",
                                ".NET Framework 2.0 required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
                                             {
                                                 FileName = regAsmPath,
                                                 Arguments = "nets-contextmenuhandler.dll /unregister",
                                                 WindowStyle = ProcessWindowStyle.Hidden
                                             };
            Process p = Process.Start(startInfo);
            p.WaitForExit();

            try
            {
                bool entryNotExists = !ContextMenuRegistryExists();
                return entryNotExists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check for registry entry existence
        /// </summary>
        /// <returns></returns>
        public static bool ContextMenuRegistryExists()
        {
            RegistryKey root = Registry.ClassesRoot;
            RegistryKey key = root.OpenSubKey(@"Directory\shellex\ContextMenuHandlers\00000000nets");
            return (key != null);
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Find RegAsm.exe path
        /// </summary>
        /// <returns></returns>
        private static string GetRegAsmPath()
        {
            string regAsmPath = null;

            string dotNetPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\Microsoft.NET\\Framework\\v2.0.50727\\RegAsm.exe";
            string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\RegAsm.exe";

            if (File.Exists(applicationPath))
                regAsmPath = applicationPath;
            else if (File.Exists(dotNetPath))
                regAsmPath = dotNetPath;

            return regAsmPath;
        }

        #endregion
    }
}