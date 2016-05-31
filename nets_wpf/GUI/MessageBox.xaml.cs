using System.Windows;
using System.Windows.Input;

namespace nets_wpf.GUI
{
    /// <summary>
    /// Interaction logic for ErrorMessage.xaml
    /// </summary>
    public partial class PopUpMessageBox : Window
    {
        public string Result;

        public PopUpMessageBox(string message, string title)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            tbx_Message.Text = message;
            gb_MainContainer.Header = title;
        }

        public void ChangeToPopUpMessage()
        {
            this.btn_Cancel.Visibility = Visibility.Hidden;
            this.btn_OK.HorizontalAlignment = HorizontalAlignment.Center;
            this.btn_OK.Content = "OK";
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

