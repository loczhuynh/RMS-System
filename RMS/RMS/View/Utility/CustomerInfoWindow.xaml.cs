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
    /// Interaction logic for CustomerInfoWindow.xaml
    /// </summary>
    public partial class CustomerInfoWindow : Window
    {
        public CustomerInfoWindow()
        {
            InitializeComponent();
        }

        List<string> _lstContent = new List<string>();
        public CustomerInfoWindow(List<string> lstContent) : this()
        {
            _lstContent = lstContent;

            txtReceipt.Text = txtReceipt.Text + Environment.NewLine + MyUtility.BuildPrintReceipt(_lstContent);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    MessageBox.Show("Please enter Email Address.");
                    return;
                }

                //send receipt by email to customer
                string body = MyUtility.BuildReceiptBody(_lstContent);
                MyUtility.SendEmailMailToCustomer("rms_restaurant@yahoo.com", txtEmail.Text, "", "", "RMS Restaurant Receipt", body);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
