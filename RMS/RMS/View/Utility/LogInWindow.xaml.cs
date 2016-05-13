using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RMS.UI.View.Utility
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        int _employeeType = -1;

        public int EmployeeType
        {
            get { return _employeeType; }
            set { _employeeType = value; }
        }

        public string UserName
        {
            get { return txtUserName.Text; }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtUserName.Text == string.Empty)
                {
                    MessageBox.Show("Please enter user name");
                    txtUserName.Focus();
                    return;
                }

                if (txtPassword.Password == string.Empty)
                {
                    MessageBox.Show("Please enter password");
                    txtPassword.Focus();
                    return;
                }
                
                bool bIsLogIn = ProgramStart.EmployeeController.IsEmployeeLogInSuccess(txtUserName.Text, txtPassword.Password, out _employeeType);
                if (bIsLogIn)
                    this.DialogResult = true;
                else
                    MessageBox.Show("Invalid User Name or Password.");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
