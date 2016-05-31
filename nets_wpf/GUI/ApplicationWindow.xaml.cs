using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using nets_wpf.Utility;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        UserControl currentPage = new UserControl();

        public ApplicationWindow()
        {
            JobQueueHandler.CreateMainFile();
            InitializeComponent();
            currentPage = pageStart;
            currentPage.Visibility = Visibility.Visible;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void NonRectangularWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            JobQueueHandler.DeleteMainFile();
            this.Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void ClearSelectionInProfileList()
        {
            this.pageStart.ClearDataGridViewSelection();
        }

        private void PageStart_Click(object sender, RoutedEventArgs e)
        {
            ResetPage(pageStart);
        }

        private void PageSetting_Click(object sender, RoutedEventArgs e)
        {
            ResetPage(pageSetting);
        }

        private void PageIVLE_Click(object sender, RoutedEventArgs e)
        {
            ResetPage(pageIVLE);
        }

        private void ResetPage(UserControl newPage)
        {
            pageStart.Reset();
            pageIVLE.Reset();
            currentPage.Visibility = Visibility.Hidden;
            currentPage = newPage;
            currentPage.Visibility = Visibility.Visible;
        }
    }
}
