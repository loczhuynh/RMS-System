using RMS.Client.BL;
using RMS.Server.BL;
using RMS.UI.Model;
using RMS.UI.View.Utility;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CEmployeeWindow.xaml
    /// </summary>
    public partial class EMainWindow : Window
    {
        const int MANAGER = 1;
        const int KITCHEN = 2;
        const int WAITSTAFF = 3;
        public EMainWindow()
        {
            DataContext = this;
            InitializeComponent();   
        
        }
      
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //handle event when a button is clicked
            gridKitchenOrders.AddHandler(Button.ClickEvent, new RoutedEventHandler(gridKitchenOrders_ButtonMOuseClick), true);
        }

        private void gridKitchenOrders_ButtonMOuseClick(object sender, RoutedEventArgs e)
        {
            //do stuff here to change the status of an order.
            //update the order status
            if (_currentOrderId != 0) //has current order
            {
                //update status of this order to database, so wait staff will know, and come to pick up 
                ProgramStart.EmployeeController.UpdateOrderStatus(_currentOrderId, 2); //2: Ready_Pickup

                //then, update GUI.
                gridKitchenOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
            }
        }


        public List<OrderBL> OpenKitchenOrders
        {
            get
            {
                List<OrderBL> orders = ProgramStart.EmployeeController.GetAllOpenKitchenOrders();
                return orders;
            }
        }

        public List<OrderBL> OrderTableStatus
        {
            get
            {
                List<OrderBL> orders = ProgramStart.EmployeeController.GetAllOrderTableStatus();
                return orders;
            }
        }

        public List<TableBL> AvailableTables
        {
            get
            {
                List<TableBL> tables = ProgramStart.EmployeeController.GetAllAvailableTables();
                return tables;
            }
        }

        List<MessageNotification> _lstMessageNotification = new List<MessageNotification>();
        public List<MessageNotification> MessageNotification
        {
            get
            {
                return _lstMessageNotification;
            }
        }

        int _currentOrderId = -1;
        public List<MenuOrderBL> OpenMenuOrders
        {
            get
            {
                List<MenuOrderBL> menuOrders = ProgramStart.EmployeeController.GetAllMenuOrderForKitchen(_currentOrderId);

                return menuOrders;
            }
        }

        private void gridKitchenOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Ensure row was clicked and not empty space
                DataGrid item = (DataGrid)e.OriginalSource;
                OrderBL currentRow = (OrderBL)item.CurrentItem;

                if (currentRow != null)
                {
                    _currentOrderId = currentRow.idOrder;
                    //update data source for ticket orders detail grid
                    gridKitchenOrdersDetail.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gridKitchenOrdersDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Ensure row was clicked and not empty space
                if (e.OriginalSource is TextBlock
                    || e.OriginalSource is Microsoft.Windows.Themes.ButtonChrome)
                {
                    DataGrid item = (DataGrid)e.Source;
                    MenuOrderBL currentRow = (MenuOrderBL)item.CurrentItem;

                    if (currentRow != null)
                    {
                        //update status for menu order
                        ProgramStart.EmployeeController.UpdateOrderMenuStatus(currentRow.idMenuOrder, "Ready for Pick Up");
                        //update data source for ticket orders detail grid
                        gridKitchenOrdersDetail.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

                        ////then, update GUI.
                        //gridKitchenOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

                        //add to message notification list
                        MessageNotification msg = new MessageNotification(currentRow.idMenuOrder, 
                                                                            string.Format("{0} is ready to pick up", currentRow.ItemName));
                        _lstMessageNotification.Add(msg);
                        _lstMessageNotification.Sort(new MessageNotification());

                        //update GUI
                        //gridMessageNotify.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
                        RefershDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefershDataSource()
        {
            //RefreshMessageNotifyControl
            var temp = gridMessageNotify.ItemsSource;
            gridMessageNotify.ItemsSource = null;
            gridMessageNotify.ItemsSource = temp;

            //refresh Kitchen Order Detail
            //var kitchenSource = gridKitchenOrdersDetail.ItemsSource;
            //gridKitchenOrdersDetail.ItemsSource = null;
            //gridKitchenOrdersDetail.ItemsSource = kitchenSource;
        }

        #region Food Menu

        public IList<MenuItemBL> AllFoodMenuItem1
        {
            get
            {
                IList<MenuItemBL> menu = ProgramStart.EmployeeController.RetrieveCategory(1); //1: for Entree category
                return menu;
            }
        }

        public IList<MenuItemBL> AllFoodMenuItem2
        {
            get
            {
                IList<MenuItemBL> menu = ProgramStart.EmployeeController.RetrieveCategory(2); //2: for Drink category
                return menu;
            }
        }

        public IList<MenuItemBL> AllFoodMenuItem3
        {
            get
            {
                IList<MenuItemBL> menu = ProgramStart.EmployeeController.RetrieveCategory(3); //3: for Appertize category
                return menu;
            }
        }

        public IList<MenuItemBL> AllFoodMenuItem4
        {
            get
            {
                IList<MenuItemBL> menu = ProgramStart.EmployeeController.RetrieveCategory(4); //1: for Dessert category
                return menu;
            }
        }

        private void FoodMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                MenuItemBL menuItem = button.DataContext as MenuItemBL;
                if (menuItem != null)
                {
                    if (button.Background == Brushes.YellowGreen) //has code number for button background color
                    {
                        ProgramStart.EmployeeController.DeactivateItem(menuItem);
                        button.Background = Brushes.Blue;
                    }
                    else
                    {
                        ProgramStart.EmployeeController.ActivateItem(menuItem);
                        button.Background = Brushes.YellowGreen;
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void gridKitchenOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Ensure row was clicked and not empty space
                if (e.OriginalSource is TextBlock
                    || e.OriginalSource is Microsoft.Windows.Themes.ButtonChrome)
                {
                    DataGrid item = (DataGrid)e.Source;
                    OrderBL currentRow = (OrderBL)item.CurrentItem;

                    if (currentRow != null)
                    {
                        //update status for menu order
                        ProgramStart.EmployeeController.UpdateOrderStatus(currentRow.idOrder, 2); //ready to pick up
                        //update data source for ticket orders grid
                        gridKitchenOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// This event will handle Pay method for customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridTableOccupy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Ensure row was clicked and not empty space
                if (e.OriginalSource is TextBlock
                    || e.OriginalSource is Microsoft.Windows.Themes.ButtonChrome)
                {
                    DataGrid item = (DataGrid)e.Source;
                    OrderBL currentRow = (OrderBL)item.CurrentItem;

                    if (currentRow != null)
                    {
                        ////update status for menu order
                        ProgramStart.EmployeeController.UpdateOrderStatus(currentRow.idOrder, 2); //ready to pick up
                        ////update data source for ticket orders grid
                        gridKitchenOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

                        //display Pay GUI for each order
                        int orderId = currentRow.idOrder;
                        EOrderDetailPaymentWindow orderPayment = new EOrderDetailPaymentWindow(orderId);
                        orderPayment.ShowDialog();
                        //refresh the available list.
                        gridTableOccupy.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //update message notify, and order Grid as well.
        private void tabTableStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridMessageNotifyForWaitStaff.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
        }

        //used to handle Log In / Out
        private void hyperLogIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogInWindow frm = new LogInWindow();
                frm.ShowDialog();

                if (frm.EmployeeType > 0) //Log in successs
                {
                    txtEmployeeLogIn.Visibility = Visibility.Hidden;
                    txtEmployeeName.Visibility = Visibility.Visible;
                    txtEmployeeLogOut.Visibility = Visibility.Visible;
                    txtEmployeeName.Text = string.Format("Welcome back {0}", frm.UserName);
                }

                if (frm.EmployeeType == MANAGER)
                {
                    //manager, enable all.
                    tabReport.IsEnabled = true;
                    tabTableStatus.IsEnabled = true;
                    tabMenu.IsEnabled = true;
                    tabCompensate.IsEnabled = true;
                    tabKitchen.IsEnabled = true;
                    tabManageReport.Visibility = Visibility.Visible;
                    
                }
                else if (frm.EmployeeType == KITCHEN)                   
                {
                    //kitchen, enable all.               
                       
                    tabMenu.IsEnabled = true;
                    tabKitchen.IsEnabled = true;
                }
                else if (frm.EmployeeType == WAITSTAFF)
                {
                    //waitstaff, enable all.               
                    tabTableStatus.IsEnabled = true;
                    tabCompensate.IsEnabled = true;                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void hyperLogOut_Click(object sender, RoutedEventArgs e)
        {
            txtEmployeeLogOut.Visibility = System.Windows.Visibility.Hidden;
            txtEmployeeName.Visibility = System.Windows.Visibility.Hidden;
            txtEmployeeLogIn.Visibility = System.Windows.Visibility.Visible;

            tabReport.IsEnabled = false;
            tabTableStatus.IsEnabled = false;
            tabMenu.IsEnabled = false;
            tabCompensate.IsEnabled = false;
            tabKitchen.IsEnabled = false;
            tabManageReport.Visibility = Visibility.Hidden;
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //invoke customer GUI to place order for customer
                CMainWindow frm = new CMainWindow();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            //refresh all data source
            gridKitchenOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
            gridAvailableTables.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
            gridTableOccupy.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
            gridPaidTable.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
        }

        #region Report
        List<OrderBL> _lstAllClosedOrder = new List<OrderBL>();
        public List<OrderBL> AllClosedOrders
        {
            get
            {
                _lstAllClosedOrder = ProgramStart.EmployeeController.GetAllClosedOrder(dateRevenue.SelectedDate.Value);
                return _lstAllClosedOrder;
            }
        }

        int _currentCloseOrderId = -1;
        public List<MenuOrderBL> AllClosedMenuOrders
        {
            get
            {
                List<MenuOrderBL> menuOrders = ProgramStart.EmployeeController.GetAllMenuOrderForKitchen(_currentCloseOrderId);

                return menuOrders;
            }
        }       

        public List<OrderBL> OrderAlreadyPaidToday
        {
            get
            {
                List<OrderBL> paidOrders = ProgramStart.EmployeeController.GetAllClosedOrder(DateTime.Now);
                return paidOrders;
            }
        }

        private void btnButtonRevenueReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dateRevenue.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select a date in order to run the report");
                    return;
                }
                txtRevenueReportHeader.Text = "All Orders on " + dateRevenue.SelectedDate.Value.ToShortDateString();
                gridRevenueReportOrders.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();

                CalculateRevenueReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// This method will calcualte all revenue per date: Total Tax, Total Tip, Total Revenue
        /// </summary>
        private void CalculateRevenueReport()
        {
            decimal totalTip = 0;
            decimal totalTax = 0;
            decimal totalRevenue = 0;
            foreach (OrderBL order in _lstAllClosedOrder)
            {
                totalTip += order.tip;
                totalTax += order.tax;
                totalRevenue += order.subTotal;
            }

            //update GUI
            txtTotalTax.Text = string.Format("Total Tax: {0:C}", totalTax);
            txtTip.Text = string.Format("Total Tip: {0:C}", totalTip);
            txtTotalRevenue.Text = string.Format("Total Revenue: {0:C}", totalRevenue);
        }

        //this event uses for viewing order detail of revenue report
        private void gridRevenueReportOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Ensure row was clicked and not empty space
                DataGrid item = (DataGrid)e.OriginalSource;
                OrderBL currentRow = (OrderBL)item.CurrentItem;

                if (currentRow != null)
                {
                    _currentCloseOrderId = currentRow.idOrder;
                    //update data source for ticket orders detail grid
                    gridRevenueOrdersDetail.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void btnEntreeReport_Click(object sender, RoutedEventArgs e)
        {
            _reportCategory = 1;
            RunFoodReport();
        }

        private void RunFoodReport()
        {
            try
            {
                if (!dateFoodReport.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select a date in order to run the report");
                    return;
                }

                txtFoodReportHeader.Text = "Top 3 Foods report on " + dateFoodReport.SelectedDate.Value.ToShortDateString();
                gridFoodReports.GetBindingExpression(DataGrid.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDessertReport_Click(object sender, RoutedEventArgs e)
        {
            _reportCategory = 4;
            RunFoodReport();
        }

        private void btnDrinkReport_Click(object sender, RoutedEventArgs e)
        {
            _reportCategory = 2;
            RunFoodReport();
        }

        private void btnAppertizerReport_Click(object sender, RoutedEventArgs e)
        {
            _reportCategory = 3;
            RunFoodReport();
        }

        int _reportCategory = -1;

        public List<FoodReportBL> Top3FoodReport
        {
            get
            {
                List<FoodReportBL> lstResult = ProgramStart.EmployeeController.GetAllFood(dateFoodReport.SelectedDate.Value, _reportCategory);
                //get top 3
                List<FoodReportBL> lstTop3 = new List<FoodReportBL>();
                int i = 0;
                foreach (FoodReportBL f in lstResult)
                {
                    if (i < 3)
                    {
                        lstTop3.Add(f);
                        i++;
                    }
                    else
                        break;
                }

                return lstTop3;

            }
        }
    }

    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class StatusToColorConverter : IValueConverter
    {
        #region IValueConverter Members: Use to convert a status of menu item to appropriate color
        public object Convert(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value == "1")
                return Brushes.YellowGreen;
            else
                return Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
