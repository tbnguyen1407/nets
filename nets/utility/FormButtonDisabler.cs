#region USING DIRECTIVES

using System.Runtime.InteropServices;
using System;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public static class FormButtonDisabler
    {
        #region FIELD DECLARATION

        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;
        private const int MF_DISABLED = 0x2;

        #endregion

        #region MAIN METHODS

        [DllImport("user32.Dll")]
        public static extern IntPtr RemoveMenu(int hMenu, int nPosition, long wFlags);

        [DllImport("User32.Dll")]
        public static extern IntPtr GetSystemMenu(int hWnd, bool bRevert);

        [DllImport("User32.Dll")]
        public static extern IntPtr GetMenuItemCount(int hMenu);

        [DllImport("User32.Dll")]
        public static extern IntPtr DrawMenuBar(int hwnd);

        public static void DisableCloseButton(int hWnd)
        {
            //Obtain the handle to the form's system menu
            IntPtr hMenu = GetSystemMenu(hWnd, false);

            // Get the count of the items in the system menu
            IntPtr menuItemCount = GetMenuItemCount(hMenu.ToInt32());

            // Remove the close menuitem
            RemoveMenu(hMenu.ToInt32(), menuItemCount.ToInt32() - 1, MF_DISABLED | MF_BYPOSITION);

            // Remove the Separator
            RemoveMenu(hMenu.ToInt32(), menuItemCount.ToInt32() - 2, MF_DISABLED | MF_BYPOSITION);

            // redraw the menu bar
            DrawMenuBar(hWnd);
        }

        #endregion
    }
}
