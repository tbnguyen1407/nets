using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for TempWindow.xaml
    /// </summary>
    public partial class TempWindow : Window
    {
        public TempWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public TempWindow(UserControl userControl)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.gd_TempWindow.Children.Add(userControl);          
        }

        private void TempWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            JobQueueHandler.DeleteRightClickSyncFile();
        }
    }
}
