using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using nets_wpf.Utility;
using nets_wpf.Logic;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for PageSetting.xaml
    /// </summary>
    public partial class PageSetting : UserControl
    {
        public PageSetting()
        {
            InitializeComponent();
            InitializeSettingAutoComplete();
            InitializeSettingContextMenu();
            InitializeSettingDeleteToRecycleBin();
        }

        #region EVENT HANDLER DECLARATIONS

        private void BtnApplyClick(object sender, EventArgs e)
        {
            bool addSuccess = ApplySettingContextMenu();
            ApplySettingAutoComplete();
            ApplySettingDeleteToRecycleBin();

            if (addSuccess)
                MessageBox.Show("Settings successfully applied!");
        }

        #endregion

        #region PRIVATE HELPERS

        private void InitializeSettingAutoComplete()
        {
            bool settingAutoComplete = LogicFacade.LoadSetting("autocomplete");
            cbx_SettingAutoComplete.IsChecked = settingAutoComplete;
        }

        private void InitializeSettingContextMenu()
        {
            bool settingContextMenu = LogicFacade.LoadSetting("contextmenu");
            cbx_SettingContextMenu.IsChecked = settingContextMenu;

            // Register/unregister context menu entries only if necessary
            if (settingContextMenu && !RegistryOperator.ContextMenuRegistryExists())
                RegistryOperator.RegisterContextMenu();
            if (!settingContextMenu && RegistryOperator.ContextMenuRegistryExists())
                RegistryOperator.UnregisterContextMenu();

        }

        private void InitializeSettingDeleteToRecycleBin()
        {
            bool settingDeleteToRecycleBin = LogicFacade.LoadSetting("deletetorecyclebin");
            cbx_SettingDeleteToRecycleBin.IsChecked = settingDeleteToRecycleBin;

        }

        private void ApplySettingAutoComplete()
        {
            LogicFacade.SaveSetting("autocomplete", (bool)cbx_SettingAutoComplete.IsChecked ? "1" : "0");
        }

        private bool ApplySettingContextMenu()
        {
            bool operationSuccess; // = cbx_SettingContextMenu.Checked;

            if (cbx_SettingContextMenu.IsChecked ==true && !RegistryOperator.ContextMenuRegistryExists())
            {
                operationSuccess = RegistryOperator.RegisterContextMenu();
                if (!operationSuccess)
                {
                    MessageBox.Show("Failed to add context menu entries!", "Error");
                    cbx_SettingContextMenu.IsChecked = false;
                }
            }
            else if (!cbx_SettingContextMenu.IsChecked==true && RegistryOperator.ContextMenuRegistryExists())
            {
                operationSuccess = RegistryOperator.UnregisterContextMenu();
                if (!operationSuccess)
                {
                    MessageBox.Show("Failed to remove context menu entries!", "Error");
                    cbx_SettingContextMenu.IsChecked = false;
                }
            }
            else
            {
                // Nothing changed -> reverse selection
                cbx_SettingContextMenu.IsChecked = !cbx_SettingContextMenu.IsChecked;
                return true;
            }

            LogicFacade.SaveSetting("contextmenu", operationSuccess ? "1" : "0");
            return operationSuccess;
        }

        private void btn_ApplySetting_Click(object sender, RoutedEventArgs e)
        {
            bool addSuccess = ApplySettingContextMenu();
            ApplySettingAutoComplete();
            ApplySettingDeleteToRecycleBin();

            if (addSuccess)
                MessageBox.Show("Settings successfully applied!");
        }

        private void ApplySettingDeleteToRecycleBin()
        {
            LogicFacade.SaveSetting("deletetorecyclebin", cbx_SettingDeleteToRecycleBin.IsChecked ==true? "1" : "0");
        }

        #endregion



    }
}
