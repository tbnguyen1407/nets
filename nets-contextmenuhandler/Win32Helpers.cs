#region USING DIRECTIVES

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace nets_contextmenuhandler
{
    public class Win32Helpers
    {
        [DllImport("kernel32.dll")]
        internal static extern Boolean SetCurrentDirectory([MarshalAs(UnmanagedType.LPTStr)]string lpPathName);
        [DllImport("kernel32.dll")]
        internal static extern uint GetFileAttributes([MarshalAs(UnmanagedType.LPTStr)]string lpPathName);
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        public enum IconSize : uint
        {
            Large = 0x0, //32x32
            Small = 0x1  //16x16        
        }
        [DllImport("kernel32.dll")]
        internal static extern Boolean CreateProcess
        (
            string lpApplicationName,
            string lpCommandLine,
            uint lpProcessAttributes,
            uint lpThreadAttributes,
            Boolean bInheritHandles,
            uint dwCreationFlags,
            uint lpEnvironment,
            string lpCurrentDirectory,
            StartupInfo lpStartupInfo,
            ProcessInformation lpProcessInformation
        );
        [DllImport("shell32")]
        internal static extern uint DragQueryFile(uint hDrop, uint iFile, StringBuilder buffer, int cch);
        [DllImport("user32")]
        internal static extern uint CreatePopupMenu();
        [DllImport("user32")]
        internal static extern int MessageBox(int hWnd, string text, string caption, int type);
        [DllImport("user32")]
        internal static extern int InsertMenuItem(uint hmenu, uint uposition, uint uflags, ref MENUITEMINFO mii);
    }
}
