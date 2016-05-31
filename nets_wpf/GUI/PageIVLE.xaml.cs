using System;
using System.Windows;
using System.Windows.Controls;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for PageIVLE.xaml
    /// </summary>
    public partial class PageIVLE : UserControl
    {
        public PageIVLE()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            exd_viewLog.IsExpanded = false;
        }

        private void ChooseFolder(object sender, RoutedEventArgs e)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.ShowDialog();
            tbx_saveTo.Text = folder.SelectedPath;
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            //string localRoot = tbx_saveTo.Text;
            ////   var handler = new IvleHandler();
            //handler.UserId = tbx_userName.Text;
            //handler.UserPassword = tbx_password.Password;

            //handler.LogIn();
            //handler.GetWorkbinIds();
            //handler.GetAllWorkbinsInfo();
            //handler.GetAllFiles();

            //handler.Sync((String.IsNullOrEmpty(localRoot) || !Directory.Exists(localRoot)) ? @"D:\Ivle\" : localRoot);
        }

        private void exd_viewLog_Collapsed(object sender, RoutedEventArgs e)
        {
            exd_viewLog.Height -= 200;
            cv_main.Height -= 200;
            //Window.GetWindow(this).Height -= 80;

        }

        private void exd_viewLog_Expanded(object sender, RoutedEventArgs e)
        {

            exd_viewLog.Height += 200;
            cv_main.Height += 200;
            //Window.GetWindow(this).Height -= 80;
        }
    }
}
