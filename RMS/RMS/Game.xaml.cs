using System.Windows;

namespace RMS.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestData gameTestMe = new TestData();

        public MainWindow()
        {
            InitializeComponent();
            gameSidePanelL0.Content = gameTestMe.UserName;
        }

        private void gameSidePanelL1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gameSidePanelL1.Content.ToString() == "Log In")
            {
                if (gameTestMe.TestUserName(gameSidePanelT0.Text.ToString()))
                {
                    gameSidePanelL0.Content = gameTestMe.UserName;
                }
                else
                {
                    gameSidePanelL0.Content = "Unknown User! Try again.";
                }

                // Disable things
                gameSidePanelT0.IsEnabled = false;
                gameSidePanelT0.Visibility = Visibility.Hidden;

                // Enable things
                gameSidePanelL0.IsEnabled = true;
                gameSidePanelL0.Visibility = Visibility.Visible;
                gameSidePanelL1.Content = "Log Out";
            }
            else
            {
                // Disable things
                gameSidePanelL0.IsEnabled = false;
                gameSidePanelL0.Visibility = Visibility.Hidden;

                // Enable things
                gameSidePanelT0.IsEnabled = true;
                gameSidePanelT0.Visibility = Visibility.Visible;
                gameSidePanelL1.Content = "Log In";
            }
        }
    }
}
