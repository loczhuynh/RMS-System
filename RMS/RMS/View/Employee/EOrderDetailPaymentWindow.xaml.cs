using RMS.Server.BL;
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
    /// Interaction logic for EOrderDetailPaymentWindow.xaml
    /// </summary>
    public partial class EOrderDetailPaymentWindow : Window
    {
        const string TOTAL_PRICE = "Total Price: ";
        const string TOTAL_TAX = "Tax: ";
        const string TOTAL_TIP = "Tip: ";
        const string SUB_TOTAL = "Sub Total: ";
        public EOrderDetailPaymentWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        public EOrderDetailPaymentWindow(int orderId) : this()
        {
            // TODO: Complete member initialization
            this._currentOrderId = orderId;

            gridOrdersDetail.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

            //update subtotal, price, tax
            UpdatePaymentValue();
        }

        decimal _totalPrice = 0;
        decimal _tax = 0;
        decimal _subTotal = 0;
        private void UpdatePaymentValue()
        {
            try
            {
                _totalPrice = 0;
                _tax = 0;
                _subTotal = 0;
                foreach (MenuOrderBL m in _lstMenuOrders)
                {
                    _totalPrice += m.Price;
                }

                _tax = _totalPrice * (0.0825M);
                _subTotal = _tax + _totalPrice + _totalTip;

                //update GUI
                txtSubTotal.Text = SUB_TOTAL + string.Format("{0:C}", _subTotal);
                txtTax.Text = TOTAL_TAX + string.Format("{0:C}", _tax);
                txtTotalPrice.Text = TOTAL_PRICE + string.Format("{0:C}", _totalPrice);
                txtTip.Text = TOTAL_TIP + string.Format("{0:C}", _totalTip);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        int _currentOrderId = -1;
        List<MenuOrderBL> _lstMenuOrders = new List<MenuOrderBL>();
        public List<MenuOrderBL> OpenMenuOrders
        {
            get
            {
                _lstMenuOrders = ProgramStart.EmployeeController.GetAllMenuOrderForOrder(_currentOrderId);

                return _lstMenuOrders;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //close the from
            this.Close();
        }

        //invoke other form to process payment
        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PaymentMethodWindow frm = new PaymentMethodWindow(_currentOrderId, (double)_subTotal, _lstMenuOrders, _totalPrice, _tax);
                frm.TipAmount = _totalTip;
                frm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Allow customer or wait staff enter tip when check out
        decimal _totalTip = 0;
        private void btnLeaveTip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtLeaveTip.Text != string.Empty)
                {
                    _totalTip = Convert.ToDecimal(txtLeaveTip.Text);
                    UpdatePaymentValue();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
