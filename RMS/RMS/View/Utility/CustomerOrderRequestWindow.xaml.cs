using RMS.UI.Model;
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
    /// Interaction logic for CustomerOrderRequestWindow.xaml
    /// </summary>
    public partial class CustomerOrderRequestWindow : Window
    {
        public CustomerOrderRequestWindow()
        {
            InitializeComponent();
        }

        public CustomerOrderRequestWindow(string request) : this()
        {
            txtSpecialRequest.Text = request;
        }

        string _specialRequest;

        public string SpecialRequest
        {
            get { return _specialRequest; }
            set { _specialRequest = value; }
        }

        //save special request from customer
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            _specialRequest = txtSpecialRequest.Text;
            this.DialogResult = true;
        }
    }
}
