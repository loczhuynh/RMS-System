using RMS.Server.BL;
using RMS.UI.View.Utility;
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

namespace RMS.UI.View.Employee
{
    /// <summary>
    /// Interaction logic for PaymentMethodWindow.xaml
    /// </summary>
    public partial class PaymentMethodWindow : Window
    {
        public PaymentMethodWindow()
        {
            InitializeComponent();
        }

        int _idOrder;       
        double _amount;
        decimal _total, _tax;
        List<MenuOrderBL> _lstMenuOrder;
        public PaymentMethodWindow(int idOrder, double amount, List<MenuOrderBL> lstMenuOrder, decimal total, decimal tax)
            : this()
        {
            _idOrder = idOrder;
            _amount = amount;
            _lstMenuOrder = lstMenuOrder;
            _total = total;
            _tax = tax;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCashPayment_Click(object sender, RoutedEventArgs e)
        {
            //process payment by cash
            try
            {
                ProcessPayment("cash");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreditPayment_Click(object sender, RoutedEventArgs e)
        {
            //process payment by credit
            try
            {
                ProcessPayment("card");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessPayment(string paymentMethod)
        {
            //check print receipt option to determine.
            //insert payment to DB
            int paymentId = ProgramStart.EmployeeController.InsertPayment(_idOrder, paymentMethod, _amount);
            if (paymentId < 0)
            {
                MessageBox.Show("There is an error with this payment. Please try it again");
                return;
            }

            //update order status to close the order.
            ProgramStart.EmployeeController.UpdateOrderStatus(_idOrder, 3); //order done, close it

            //update tip for order if any.
            if (_totalTip > 0)
                ProgramStart.CustomerController.UpdateOrderTip(_idOrder, _totalTip);


            if (rdoPrintAndEmail.IsChecked.HasValue
                 || rdoPrintAndEmail.IsChecked.Value)
            {
                //email receipt to customer.
                //build receipt
                List<string> lstContent = new List<string>();
                foreach (MenuOrderBL m in _lstMenuOrder)
                {
                    lstContent.Add(string.Format("{0} {1}", m.ItemName, m.Price));
                }
                lstContent.Add("-----------------------------------------------");
                lstContent.Add(string.Format("    Total: {0:C}", _total));
                lstContent.Add(string.Format("      Tax: {0:C}", _tax));
                lstContent.Add(string.Format("      Tip: {0:C}", _totalTip));
                lstContent.Add(string.Format("Sub Total: {0:C}", _amount));

                CustomerInfoWindow frm = new CustomerInfoWindow(lstContent);
                frm.ShowDialog();

            }
            MessageBox.Show("Thank you. See you again!");
            //complete, close form
            this.Close();
        }

        decimal _totalTip = 0;
        public decimal TipAmount
        {
            get
            {
                return _totalTip;
            }
            set
            {
                _totalTip = value;
            }
        }
    }

       
}
