#region USING DIRECTIVES

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;

#endregion

namespace nets_contextmenuhandler
{
    [Guid("33612C08-B156-4ad2-9599-049A685B8CD0")]
    public class ContextMenuManager : ColumnProvider, IShellExtInit, IContextMenu, IPersistFile, IQueryInfo, IExtractIcon
    {
        #region FIELD DECLARATION

        protected const string Guid = "{33612C08-B156-4ad2-9599-049A685B8CD0}";
        protected string FileNames;
        protected uint m_hDrop = 0;
        const int S_OK    = 0;
        const int S_FALSE = 1;
        protected string IconFile;
        private const uint E_NOTIMPL = 0x80004001;
        private const uint QITIPF_DEFAULT = 0;
        private const uint GIL_SIMULATEDOC   = 0x0001;     // simulate this document icon for this
        private const uint GIL_PERINSTANCE   = 0x0002;     // icons from this class are per instance (each file has its own)
        private const uint GIL_PERCLASS      = 0x0004;      // icons from this class per class (shared for all files of this type)
        private const uint GIL_NOTFILENAME   = 0x0008;     // location is not a filename, must call ::ExtractIcon
        private const uint GIL_DONTCACHE     = 0x0010;      // this icon should not be cached
        private const uint GIL_SHIELD        = 0x0200;      // icon should be "stamped" with the LUA shield
        private const uint GIL_FORCENOSHIELD = 0x0400;
        private const uint IDS_PROJNAME      = 100;
        private const uint IDI_ZERO_BYTES    = 100;
        private const uint IDR_TXTICONSHLEXT = 101;
        private const uint IDI_UNDER_4K      = 101;
        private const uint IDI_UNDER_8K      = 102;
        private const uint IDI_OVER_8K       = 103;
        private string tip = string.Empty;

        #endregion

        #region CONSTRUCTORS

        public ContextMenuManager()
        {
        }

        #endregion

        #region MAIN METHODS

        public override int Initialize(LPCSHCOLUMNINIT psci)
        {
            return S_OK;
        }

        int IShellExtInit.Initialize(IntPtr pidlFolder, IntPtr lpdobj, uint hKeyProgID) //Uses IShellExtInit for IColumnProvider and IContextMenu
        {
            try
            {
                if (lpdobj != (IntPtr)0)
                {
                    // Get info about the directory
                    IDataObject dataObject = (IDataObject)Marshal.GetObjectForIUnknown(lpdobj);
                    FORMATETC fmt = new FORMATETC();
                    fmt.cfFormat = CLIPFORMAT.CF_HDROP;
                    fmt.ptd = 0;
                    fmt.dwAspect = DVASPECT.DVASPECT_CONTENT;
                    fmt.lindex = -1;
                    fmt.tymed = TYMED.TYMED_HGLOBAL;
                    STGMEDIUM medium = new STGMEDIUM();
                    dataObject.GetData(ref fmt, ref medium);
                    m_hDrop = medium.hGlobal;
                }
            }
            catch (Exception e)
            {
            }
            return 0;
        }

        public override int GetColumnInfo(int dwIndex, out SHCOLUMNINFO psci)//Using IShellExtInit.Initialize and IColumnProvider
        {
            psci = new SHCOLUMNINFO();
            try
            {
                if (dwIndex > 1)
                    return S_FALSE;
                if (dwIndex == 0)
                {
                    psci.scid.fmtid = GetType().GUID;
                    psci.scid.pid = 0; // Each Column should have a different ID
                    // Cast to a ushort, because a VARTYPE is ushort and a VARENUM is int
                    psci.vt = (ushort)VarEnum.VT_BSTR;
                    psci.fmt = LVCFMT.LEFT;
                    psci.cChars = 20;
                    psci.csFlags = SHCOLSTATE.TYPE_STR;
                    psci.wszTitle = "Column1";
                    psci.wszDescription = "Provides column1 information";
                }
                if (dwIndex == 1)
                {
                    psci.scid.fmtid = GetType().GUID;
                    psci.scid.pid = 1; // Each Column should have a different ID
                    // Cast to a ushort, because a VARTYPE is ushort and a VARENUM is int
                    psci.vt = (ushort)VarEnum.VT_BSTR;
                    psci.fmt = LVCFMT.LEFT;
                    psci.cChars = 20;
                    psci.csFlags = SHCOLSTATE.TYPE_STR;
                    psci.wszTitle = "Column2";
                    psci.wszDescription = "Provides column2 information";
                }
            }
            catch (Exception ex)
            {
                return S_FALSE;
            }
            return S_OK;
        }

        public override int GetItemData(LPCSHCOLUMNID pscid, LPCSHCOLUMNDATA pscd, out object pvarData)//Using  IColumnProvider
        {
            pvarData = string.Empty;
            try
            {
                // Ignore directories
                if (((FileAttributes)pscd.dwFileAttributes | FileAttributes.Directory) == FileAttributes.Directory)
                    return S_FALSE;

                // Only service known columns(we do not have more then 2 columns)
                if (pscid.fmtid != GetType().GUID || pscid.pid > 1 || pscd.pwszExt != "nets shell extension")
                    return S_FALSE;

                pvarData = "tooltip";
            }
            catch (Exception ex)
            {
                return S_FALSE;
            }
            return S_OK;
        }

        int IContextMenu.QueryContextMenu(uint hMenu, uint iMenu, int idCmdFirst, int idCmdLast, uint uFlags)//Using IShellExtInit.Initialize and IContextMenu
        {
            // Create the popup to insert
            uint handleMenuPopup = Win32Helpers.CreatePopupMenu();
            StringBuilder szFile = new StringBuilder(50000);

            int id = 1;
            if ((uFlags & 0xf) == 0 || (uFlags & (uint)CMF.CMF_EXPLORE) != 0)
            {
                uint nselected = Win32Helpers.DragQueryFile(m_hDrop, 0xffffffff, null, 0);
                
                for (uint uFile = 0; uFile < nselected; uFile++)
                {
                    // Get the next filename.
                    Win32Helpers.DragQueryFile(m_hDrop, uFile, szFile, szFile.Capacity + 1);
                    FileNames += szFile.ToString() + "|";
                }

                id = PopulateMenu(handleMenuPopup, idCmdFirst + id);

                // Add a separator
                MENUITEMINFO seperator1 = new MENUITEMINFO();
                seperator1.cbSize = 48;
                seperator1.fMask = (uint)MIIM.TYPE;
                seperator1.fType = (uint)MF.SEPARATOR;
                Win32Helpers.InsertMenuItem(hMenu, iMenu, 1, ref seperator1);

                // Add the popup to the context menu
                MENUITEMINFO menuItemInfo = new MENUITEMINFO();
                menuItemInfo.cbSize = 48;
                menuItemInfo.fMask = (uint)MIIM.TYPE | (uint)MIIM.STATE | (uint)MIIM.SUBMENU;
                menuItemInfo.hSubMenu = (int)handleMenuPopup;
                menuItemInfo.fType = (uint)MF.STRING;
                menuItemInfo.dwTypeData = "NETS (Nothing Else To Sync)";
                menuItemInfo.fState = (uint)MF.ENABLED;
                Win32Helpers.InsertMenuItem(hMenu, (uint)iMenu + 1, 1, ref menuItemInfo);

                // Add a separator
                MENUITEMINFO seperator = new MENUITEMINFO();
                seperator.cbSize = 48;
                seperator.fMask = (uint)MIIM.TYPE;
                seperator.fType = (uint)MF.SEPARATOR;
                Win32Helpers.InsertMenuItem(hMenu, iMenu + 2, 1, ref seperator);
            }
            return id;
        }

        void AddMenuItem(uint hMenu, string text, int id, uint position)
        {
            MENUITEMINFO menuItemInfo = new MENUITEMINFO();
            menuItemInfo.cbSize = 48;
            menuItemInfo.fMask = (uint)MIIM.ID | (uint)MIIM.TYPE | (uint)MIIM.STATE;
            menuItemInfo.wID = id;
            menuItemInfo.fType = (uint)MF.STRING;
            menuItemInfo.dwTypeData = text;
            menuItemInfo.fState = (uint)MF.ENABLED;
            Win32Helpers.InsertMenuItem(hMenu, position, 1, ref menuItemInfo);
        }

        int PopulateMenu(uint hMenu, int id)
        {
            AddMenuItem(hMenu, "Smart Sync", id, 0);
            AddMenuItem(hMenu, "Sync With...", ++id, 1);
            return id++;
        }

        void IContextMenu.GetCommandString(int idCmd, uint uFlags, int pwReserved, StringBuilder commandString, int cchMax)
        {
            switch (uFlags)
            {
                case (uint)GCS.VERB:
                    commandString = new StringBuilder("...");
                    break;
                case (uint)GCS.HELPTEXT:
                    commandString = new StringBuilder("...");
                    break;
            }
        }

        void IContextMenu.InvokeCommand(IntPtr pici)
        {
            try
            {
                Type typInvokecommandinfo = Type.GetType("nets_contextmenuhandler.INVOKECOMMANDINFO");
                INVOKECOMMANDINFO ici = (INVOKECOMMANDINFO)Marshal.PtrToStructure(pici, typInvokecommandinfo);
                
                string applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\nets.exe";
                FileNames = FileNames.Substring(0, FileNames.Length - 1);

                switch (ici.verb - 1)
                {
                    case 0: // SmartSync
                        Process.Start(applicationPath, "\"--SmartSync|" + FileNames + "\"");
                        break;
                    case 1: // QuickSync
                        Process.Start(applicationPath, "\"--SyncWith|" + FileNames + "\"");
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public uint Load(string pszFileName, uint dwMode) //Using IPersistFile
        {
            IconFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Icon.bmp");
            tip = "My tool-tip";
            return S_OK;
        }

        public uint IsDirty()
        {
            return E_NOTIMPL;
        }

        public uint Save(string pszFileName, bool fRemember)
        {
            return E_NOTIMPL;
        }

        public uint SaveCompleted(string pszFileName)
        {
            return E_NOTIMPL;
        }

        public uint GetCurFile(out string ppszFileName)
        {
            ppszFileName = null;
            return E_NOTIMPL;
        }

        public uint GetClassID(out Guid pClassID)
        {
            pClassID = new Guid("33612C08-B156-4ad2-9599-049A685B8CD0");
            return E_NOTIMPL;
        }

        public uint GetInfoFlags(out uint dwFlags) //Using IQueryInfo 
        {
            dwFlags = QITIPF_DEFAULT;
            return S_OK;
        }

        public uint GetInfoTip(uint dwFlags, out IntPtr pszInfoTip) //Using IQueryInfo and IPersistFile.Load
        {
            pszInfoTip = Marshal.StringToCoTaskMemUni(tip);
            return S_OK;
        }

        public uint GetIconLocation(int uFlags, IntPtr szIconFile, int cchMax, IntPtr piIndex, UIntPtr pwFlags)//Using IExtractIcon and IPersistFile.Load
        {
            try
            {
                IconHandlerReturnFlags Flags;
                Flags = IconHandlerReturnFlags.PerInstance | IconHandlerReturnFlags.DontCache | IconHandlerReturnFlags.NotFilename;
                //pwFlags = (UIntPtr)Flags;
                return S_OK;
            }
            catch (Exception e)
            {
                return S_FALSE;
            }
        }
        public uint Extract(string pszFile, uint nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, uint nIconSize) //Using IExtractIcon
        {
            try
            {
                Bitmap Image = new Bitmap(IconFile);
                if (null != phiconLarge)
                { 
                    Bitmap Large;
                    Large = (Bitmap)Image.GetThumbnailImage(32, 32, null, IntPtr.Zero);
                    phiconLarge = Large.GetHicon();
                    Large.Dispose();
                  
                }
                if (null != phiconSmall)
                {   
                    Bitmap Small;
                    Small =(Bitmap)Image.GetThumbnailImage(16, 16, null, IntPtr.Zero);
                    phiconSmall = Small.GetHicon();
                    Small.Dispose(); 
                }
                Image.Dispose();
                return S_OK;
            }
            catch (Exception e)
            {
                return S_FALSE;
            }
        }

        [System.Runtime.InteropServices.ComRegisterFunctionAttribute()]
        public static void DllRegisterServer(Type t)
        {
            try
            {
                // For Winnt set me as an approved shellex
                RegistryKey root;
                RegistryKey rk;
                root = Registry.LocalMachine;
                rk = root.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Approved", true);
                rk.SetValue(Guid, "nets shell extension");
                rk.Close();
                root.Close();

                root = Registry.ClassesRoot;

                rk = root.CreateSubKey(@"Directory\shellex\ContextMenuHandlers\00000000nets");
                rk.SetValue("", Guid);
                rk.Close();

                // Tell Explorer to refresh
                SHChangeNotify(SHCNE_ASSOCCHANGED, 0, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception e)
            {
            }
        }

        [System.Runtime.InteropServices.ComUnregisterFunctionAttribute()]
        public static void DllUnregisterServer(Type t)
        {
            try
            {
                RegistryKey root;
                RegistryKey rk;

                // Remove ShellExtenstions registration
                root = Registry.LocalMachine;
                rk = root.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Approved", true);
                rk.DeleteValue(Guid);
                rk.Close();
                root.Close();

                // Delete regkey
                root = Registry.ClassesRoot;
                root.DeleteSubKey(@"Directory\shellex\ContextMenuHandlers\00000000nets");
                root.Close();

                SHChangeNotify(SHCNE_ASSOCCHANGED, 0, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception e)
            {
            }
        }

        #endregion
    }
}