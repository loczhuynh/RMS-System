using RMS.Server.BL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RMS.Client.BL
{
    enum userTypes { CustomerType, ManagerType, WaitStaffType, KitchenType };
    public class CustomerController
    {
        RestaurantServiceClient _client = null;
        CustomerBL _user = null;
        OrderBL _Order = null;
        IList<MenuOrderBL> _orderItems = null;
        int _table = 1;
        double _tax = 0.085;
        double _total = 0.0;
        double _tip;

        public CustomerController()
        {
            _client = new RestaurantServiceClient();
        }

        #region UserOps
        public bool UserLookup(string uName, string password)
        {
            bool login = false;
            if (string.IsNullOrEmpty(uName))
            {
                _user = CreateGuest();
                return true;
            }
            else if (_client != null)
            {
                if (password.Contains('-'))
                {
                    string[] groups = password.Split('-');
                    password = null;
                    foreach (string group in groups)
                        password += group;
                }

                _user = _client.CustomerEmailLookup(uName);
                if (_user != null)
                {
                    login = (password == _user.phone);
                }
                //need to notify that no user found. . .
                else
                    throw new UserException("Email not found");
                if (!login)
                    throw new UserException("Invalid password");
                return true;
            }
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public string username()
        {
            if (_user == null)
                throw new UserException("No user logged in");
            return _user.name;
        }
        public int InsertUser(string name, DateTime bday, string phone, string email)
        {
            if (_client != null)
            {
                if (phone.Contains('-'))
                {
                    string[] groups = phone.Split('-');
                    phone = null;
                    foreach (string group in groups)
                        phone += group;
                }
                int id = _client.InsertCustomer(name, bday, phone, email);
                if (id > 0)
                    return id;
                throw new UserException("User already exists");
            }
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public bool PasswordReset(string name, string new_phone)
        {
            if (_client != null)
            {
                if (new_phone.Contains('-'))
                {
                    string[] groups = new_phone.Split('-');
                    new_phone = null;
                    foreach (string group in groups)
                        new_phone += group;
                }

                return _client.CustomerReset(name, new_phone);
            }
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        internal CustomerBL CreateGuest()
        {
            CustomerBL Guest = new CustomerBL();
            Guest.idCustomer = 3;
            Guest.phone = "(555) 555-1234";
            Guest.birthday = DateTime.Today;
            Guest.email = "null_name@nullmail.com";
            Guest.name = "Guest";
            Guest.points = 0;
            return Guest;
        }
        #endregion
        #region OrderOps
        //submit order to server
        public int SubmitOrder(OrderBL order)
        {
            if (_client != null)
                return _client.SubmitOrder(order);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        public int SubmitMenuOrderItem(MenuOrderBL menuOrder)
        {
            if (_client != null)
                return _client.SubmitOrderItem(menuOrder);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        internal bool UpdateOrderTip(int idOrder, decimal totalTip)
        {
            if (_client != null)
                return _client.UpdateOrderTip(idOrder, totalTip);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        public IList<MenuOrderBL> OrderedItems()
        {
            if(_orderItems == null) { return null; }
            if (_orderItems.Count() == 0) { return null; }
            return _orderItems;
        }
        public void CreateOrder(MenuItemBL new_Item, string Request)
        {
            if (_Order != null)
                throw new OrderException("Cannot create new order, current order in progress");
            _orderItems = new List<MenuOrderBL>();
            _Order = new OrderBL();
            MenuOrderBL orderItem = new MenuOrderBL();

            //initialize order
            _Order.idTable = _table;//hard coded for testing, production hard coded for each table
            _Order.idCustomer = _user.idCustomer;
            _Order.subTotal = new_Item.price;
            _Order.date = DateTime.Today;
            _Order.idOrder = -1;
            //initialize orderItem
            orderItem.idMenuItem = new_Item.idMenuItem;
            orderItem.Request = Request;
            _orderItems.Add(orderItem);
        }
        public void AddOrderItem(MenuItemBL new_Item, string Request)
        {
            if (_Order == null)
                CreateOrder(new_Item, Request);
            else
            {
                MenuOrderBL orderItem = new MenuOrderBL();
                orderItem.idMenuItem = new_Item.idMenuItem;
                orderItem.Request = Request;
                _orderItems.Add(orderItem);
                _Order.subTotal += new_Item.price;
            }
        }
        public void RemoveOrderItem(MenuItemBL dead_Item, string Request)
        {
            if (_Order == null)
                return;//no order to remove from
            if (_Order.idOrder > 0)
                throw new OrderException("Cannot remove item, order already submitted");
            foreach (MenuOrderBL Item in _orderItems)
            {
                if (Item.idMenuItem == dead_Item.idMenuItem && Item.Request == Request)
                {
                    _orderItems.Remove(Item);
                    return;
                }
            }
            _Order.subTotal -= dead_Item.price;
        }
        public void SubmitOrder()
        {
            if (_client != null)
                if (_Order != null)
                {
                    IEnumerator<MenuOrderBL> ItemEnum = _orderItems.GetEnumerator();
                    _Order.idOrder = _client.SubmitOrder(_Order);
                    if (_Order.idOrder < 0)
                        throw new OrderException("Order could not be created");
                    for (int i = 0; i < _orderItems.Count; i++)
                    {
                        _orderItems[i].idOrder = _Order.idOrder;
                        _orderItems[i].idMenuOrder = _client.SubmitOrderItem(_orderItems[i]);
                    }
                }
                else
                    throw new OrderException("No order to submit, create an order first");
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public bool RequestRefill(MenuItemBL drink)
        {
            if(_client != null)
            {
                return _client.RequestAssistance(_table, "Drink Refill Request: " + drink.name);
            }
            throw new ConnectionException("Request could not be completed due to lack of connectivity");
        }
        public bool RequestAssistance()
        {
            if (_client != null)
                return _client.RequestAssistance(_table, "Assistance Requested");
            else
                throw new ConnectionException("Request could not be completed due to lack of connectivity");
        }
        #endregion
        #region MenuOps
        public CategoryBL[] GetAllCategory()
        {
            if (_client != null)
                return _client.GetAllCategory();
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public IList<MenuItemBL> RetrieveCategory(int idCategory)
        {
            if (_client != null)
                return _client.CategoryRetrieve(idCategory);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public List<MenuItemBL> RetrieveAvailableMenuItemsInCategory(int idCategory)
        {
            if (_client != null)
                return new List<MenuItemBL>(_client.GetAvailableMenuItems(idCategory));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
 
        public MenuItemBL RetrieveItem(int idMenuItem)
        {
            if (_client == null)
                throw new ConnectionException();
            return _client.ItemLookup(idMenuItem);
        }
        #endregion
        #region PaymentOps
        internal void calcTotal(OrderBL PayOrder, double tip)
        {
            double discount = 0.0;
            if (_user.points >= 5)
                discount = 10.0;
            _total = (double)(PayOrder.subTotal - (decimal)discount) * (1 + _tax);
            if (tip > 1)
            {
                _tip = tip;
                _total = _total + _tip;
            }
            else
            {
                _tip = _total * tip;
                _total = _total + _tip;
            }
        }
        public double total(double tip)
        {
            calcTotal(_Order, tip);
            return _total;
        }
        //allows flexability for item split payments
        public bool Payment(OrderBL PayOrder, double amount, string method)
        {
            if (_client == null)
                throw new ConnectionException("This RMS station is not connected to the server");

            if (amount > _total)
            {
                calcTotal(PayOrder, 0.0);
                _tip = amount - _total;
                calcTotal(PayOrder, _tip);
            }
            int result = _client.Payment(PayOrder.idOrder, method, amount, _user.idCustomer);
            if (result > 0)
                return true;
            return false;
        }
        //simple non-split payment
        public bool Payment(double amount, string method)
        {
            return Payment(_Order, amount, method);
        }
        public OrderBL Split(IList<MenuOrderBL> SplitItems)
        {
            OrderBL NewSplit = new OrderBL();

            NewSplit.idTable = _table;//hard coded for testing, production hard coded for each table
            NewSplit.idCustomer = _user.idCustomer;
            NewSplit.subTotal = (decimal)0.0;
            NewSplit.date = DateTime.Today;
            NewSplit.idOrder = _Order.idOrder;
            double price = 0.0;
            for (int i = 0; i < _orderItems.Count; i++)
            {
                price = _client.PriceLookup(_orderItems[i].idMenuItem);
                _orderItems.Remove(_orderItems[i]);
                _Order.subTotal -= (decimal)price;
                NewSplit.subTotal += (decimal)price;
            }
            return NewSplit;
        }

        public void ProcessComp()
        {
            if (_client == null)
                throw new ConnectionException("This RMS station is not connected to the system");
            CompBL itemComp = null;
            _orderItems = _client.RetrieveOrderItems(_Order.idOrder);
            foreach( MenuOrderBL Item in _orderItems)
            {
                itemComp = _client.CompLookup(Item.idComp);
                if (itemComp != null)
                {
                    _Order.subTotal -= Item.Price;
                    _Order.subTotal += itemComp.price;
                }
            }
        }
        #endregion        
    }
    public class EmployeeController
    {
        //regions: each Operations region contains member functions associated with
        //its title, as well as subregions associated with usertypes that use them
        RestaurantServiceClient _client = null;
        EmployeeBL _user = null;
        CustomerBL Guest = null;
        OrderBL _Order = null;
        IList<MenuOrderBL> _orderItems;
        int _table = 1;
        double _tax = 0.085;
        double _total = 0.0;
        double _tip;
        userTypes _userType = 0;

        public EmployeeController()
        {
            _client = new RestaurantServiceClient();
            Guest = CreateGuest();
        }
        #region UserOps
        public void UserLookup(string uName, string password)
        {
            bool login = false;

            if (_client != null)
            {
                _user = _client.EmployeeLookup(uName);
                if (_user != null)
                {
                    _userType = (userTypes)_user.employeeType;
                    login = (password == _user.password);
                }

                //need to notify that no user found. . .
                else
                    throw new UserException("User name not found");
                if (!login)
                    throw new UserException("Invalid password");

            }
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public bool IsEmployeeLogInSuccess(string uName, string password, out int employeeType)
        {
            bool login = false;
            employeeType = -1;
            if (_client != null)
            {
                EmployeeBL emp = _client.EmployeeLookup(uName);
                if (emp != null)
                {
                    employeeType = emp.employeeType;
                    login = (password == emp.password);
                    return login;
                }

                return false;

            }
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public int InsertEmployee(string name, string password, string type)
        {
            if (_client != null)
            {
                int id = _client.InsertEmployee(name, password, type);
                if (id == -1)
                    throw new UserException("User already exists");
                else return id;
            }
            else
                throw new UserException("This RMS station is not connected to the server");
        }
        public bool PasswordReset(string name, string new_password)
        {
            if (_client != null)
                return _client.EmployeeReset(name, new_password);
            else
                throw new UserException("This RMS station is not connected to the server");
        }
        //included for case where employee helps a customer to enroll
        public int InsertCustomer(string name, DateTime bday, string phone, string email)
        {
            if (_client != null)
                return _client.InsertCustomer(name, bday, phone, email);
            else
                throw new UserException("This RMS station is not connected to the server");
        }
        internal CustomerBL CreateGuest()
        {
            CustomerBL Guest = new CustomerBL();
            Guest.idCustomer = 3;
            Guest.phone = "(555) 555-1234";
            Guest.birthday = DateTime.Today;
            Guest.email = "null_name@nullmail.com";
            Guest.name = "Guest";
            Guest.points = 0;
            return Guest;
        }
        public string username()
        {
            if (_user == null)
                throw new UserException("Cannot return username, no user logged in");
            return _user.name;
        }
        #endregion
        #region MenuOps
        #region WaitStaffOps
        public CategoryBL[] GetAllCategory()
        {
            if (_client != null)
                return _client.GetAllCategory();
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public IList<MenuItemBL> RetrieveCategory(int Category)
        {
            if (_client != null)
                return _client.CategoryRetrieve(Category);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public MenuItemBL RetrieveItem(int idMenuItem)
        {
            if (_client == null)
                throw new ConnectionException();
            return _client.ItemLookup(idMenuItem);
        }
        #endregion
        #region KitchenOps
        public void DeactivateItem(MenuItemBL dead)
        {
            if (_client == null)
                throw new ConnectionException("Cannot deactivate menu item, no connection to the system");
            if (!_client.DeactivateItem(dead))
                throw new MenuException("Could not deactivate menu item due to server side issue");
        }
        public void ActivateItem(MenuItemBL live)
        {
            if (_client == null)
                throw new ConnectionException("Cannot activate menu item, no connection to the system");
            if (!_client.ActivateItem(live))
                throw new MenuException("Could not activate menu item due to server side issue");
        }
        #endregion
        #endregion
        #region OrderOps
        #region WaitStaffOps
        public void RetrieveOrder(int idTable)
        {
            if (_client == null)
                throw new ConnectionException("Client not connected, order unavailable");
            _Order = _client.RetrieveOrder(idTable);
            if (_Order == null)
                throw new OrderException("No current order available for this table");
            _orderItems = _client.RetrieveOrderItems(_Order.idOrder);
            _table = idTable;
        }
        public IList<MenuOrderBL> OrderedItems()
        {
            if (_orderItems.Count() == 0)
                return null;
            return _orderItems;
        }
        public void CreateOrder(MenuItemBL new_Item, string Request)
        {
            if (_Order != null)
                throw new OrderException("Cannot create new order, current order in progress");
            _Order = new OrderBL();
            _orderItems = new List<MenuOrderBL>();
            MenuOrderBL orderItem = new MenuOrderBL();
            //initialize order
            _Order.idTable = _table;//hard coded for testing, production hard coded for each table
            _Order.idCustomer = Guest.idCustomer;
            _Order.subTotal = new_Item.price;
            _Order.date = DateTime.Today;
            _Order.idOrder = -1;
            //initialize orderItem
            //orderItem.idCustomer = Guest.idCustomer;
            orderItem.idMenuItem = new_Item.idMenuItem;
            orderItem.Request = Request;
            _orderItems.Add(orderItem);
        }
        public void AddOrderItem(MenuItemBL new_Item, string Request)
        {
            if (_Order == null)
                CreateOrder(new_Item, Request);
            else
            {
                MenuOrderBL orderItem = new MenuOrderBL();
                //orderItem.idCustomer = Guest.idCustomer;
                orderItem.idMenuItem = new_Item.idMenuItem;
                orderItem.Request = Request;
                _orderItems.Add(orderItem);
                _Order.subTotal += new_Item.price;
            }
        }
        public void RemoveOrderItem(MenuItemBL dead_Item, string Request)
        {
            if (_Order == null)
                return;//no order to remove from
            if (_Order.idOrder > 0)
                throw new OrderException("Cannot remove item, order already submitted");

            CustomerBL Cust = CreateGuest();
            foreach (MenuOrderBL orderItem in _orderItems)
            {
                if (orderItem.idMenuItem == dead_Item.idMenuItem && orderItem.Request == Request)
                {
                    _orderItems.Remove(orderItem);
                    _Order.subTotal -= dead_Item.price;
                    return;
                }
            }
        }
        public void SubmitOrder()
        {
            if (_client != null)
                if (_Order != null)
                {
                    IEnumerator<MenuOrderBL> ItemEnum = _orderItems.GetEnumerator();
                    _Order.idOrder = _client.SubmitOrder(_Order);
                    for (int i = 0; i < _orderItems.Count; i++)
                    {
                        _orderItems[i].idOrder = _Order.idOrder;
                        _orderItems[i].idMenuOrder=_client.SubmitOrderItem(_orderItems[i]);//requires an additional contract
                    }
                }
                else
                    throw new OrderException("No order to submit, create an order first");
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        //get all available table for the waitstaff
        internal List<TableBL> GetAllAvailableTables()
        {
            if (_client != null)
                return new List<TableBL>(_client.GetAllAvailableTables());
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        public List<TableBL>  RetrieveAssistanceRequests()
        {
            if (_client != null)
                return new List<TableBL>(_client.GetAllAssistanceRequests(_user.idEmployee));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        #endregion
        #region KitchenOps
        public bool ClaimItem(MenuOrderBL claimed)
        {
            if (_client == null)
                throw new ConnectionException("Client not connected, cannot claim this item");
            return _client.ClaimItem(claimed.idMenuOrder);
        }
        public bool ReadyItem(MenuOrderBL ready)
        {
            if (_client == null)
                throw new ConnectionException("Client not connected, cannot update this item");
            return _client.ReadyItem(ready.idMenuOrder);
        }

        //get all send_to_kitchen orders
        internal List<OrderBL> GetAllOpenKitchenOrders()
        {
            if (_client != null)
                return new List<OrderBL>(_client.GetAllAvailableOrdersForKitchen());
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        //get all open orders, so waitstaff can know the status of each table 
        internal List<OrderBL> GetAllOrderTableStatus()
        {
            if (_client != null)
                return new List<OrderBL>(_client.GetAllOrderTableStatus());
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        //get all menu order of one specific order
        internal List<MenuOrderBL> GetAllMenuOrderForOrder(int orderId)
        {
            if (_client != null)
                return new List<MenuOrderBL>(_client.GetAllMenuOrderForOrder(orderId));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        //get all menu order of one specific order
        internal List<MenuOrderBL> GetAllMenuOrderForKitchen(int orderId)
        {
            if (_client != null)
                return new List<MenuOrderBL>(_client.GetAllMenuOrderForKitchen(orderId));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        internal void UpdateOrderStatus(int orderId, int status)
        {
            if (_client != null)
                _client.UpdateOrderStatus(orderId, status);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        internal void UpdateOrderMenuStatus(int menuOrderId, string status)
        {
            if (_client != null)
                _client.UpdateOrderMenuStatus(menuOrderId, status);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }

        internal int InsertPayment(int idOrder, string method, double amount)
        {
            if (_client != null)
                return _client.Payment(idOrder, method, amount, Guest.idCustomer);
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        #endregion
        #endregion
        #region PaymentOps
        internal void calcTotal(OrderBL PayOrder, double tip)
        {
            _total = (double)(PayOrder.subTotal) * (1 + _tax);
            if (tip > 1)
            {
                _tip = tip;
                _total = _total + _tip;
            }
            else
            {
                _tip = _total * tip;
                _total = _total + _tip;
            }
        }
        public double total(double tip)
        {
            calcTotal(_Order, tip);
            return _total;
        }
        //allows flexability for item split payments
        public bool Payment(OrderBL PayOrder, double amount, string method)
        {
            if (_client == null)
                throw new ConnectionException("This RMS station is not connected to the server");

            if (amount > _total)
            {
                calcTotal(PayOrder, 0.0);
                _tip = amount - _total;
                calcTotal(PayOrder, _tip);
            }
            int result = _client.Payment(PayOrder.idOrder, method, amount,Guest.idCustomer);
            if (result > 0)
                return true;
            return false;
        }
        //simple non-split payment
        public bool Payment(double amount, string method)
        {
            return Payment(_Order, amount, method);
        }
        public OrderBL Split(IList<MenuOrderBL> SplitItems)
        {
            IEnumerator<MenuOrderBL> SplitEnum = SplitItems.GetEnumerator();
            OrderBL NewSplit = new OrderBL();

            NewSplit.idTable = _table;//hard coded for testing, production hard coded for each table
            NewSplit.idCustomer = Guest.idCustomer;
            NewSplit.subTotal = (decimal)0.0;
            NewSplit.date = DateTime.Today;
            NewSplit.idOrder = _Order.idOrder;
            double price = 0.0;
            for (int i = 0; i < SplitItems.Count; i++)
            {
                price = _client.PriceLookup(SplitItems[i].idMenuItem);
                _orderItems.Remove(SplitItems[i]);
                _Order.subTotal -= (decimal)price;
                NewSplit.subTotal += (decimal)price;
            }
            return NewSplit;
        }
        public int CreateComp(int orderItem, int item, string reason, double price)
        {
            if (_client == null)
                throw new ConnectionException("This RMS station is not connected to the system");
            MenuItemBL Comped = _client.ItemLookup(item);
            if (Comped == null)
                throw new MenuException("Could not find item to comp");
            if (price > 0 && price < 1) // percentage discount
            {
                price = price * (double)Comped.price;
            }
            CompBL newComp = new CompBL();
            newComp.reason = reason;
            newComp.idEmployee = _user.idEmployee;
            newComp.price = (decimal)price;
            int ret = _client.CompItem(newComp);
            if (_client.updateComp(orderItem, ret))
                return ret;
            return -1;

        }
        public IList<int> CreateComp(IList<int> orderitems, IList<int> items, string[] reason, double[] price)
        {
            List<int> result = new List<int>();
            string message = null;
            for (int i = 0; i < items.Count(); i++)
            {
                try
                {
                    result.Add(CreateComp(orderitems[i], items[i], reason[i], price[i]));
                }
                catch (Exception e)
                {
                    message += e.Message;
                }
            }
            if (message == null)
                return result;
            throw new Exception(message);
        }
        public void ProcessComp(IList<MenuOrderBL> Items)
        {
            if (_client == null)
                throw new ConnectionException("This RMS station is not connected to the system");
            CompBL itemComp = null;
            MenuItemBL OrderedItem = null;
            int i;
            for (int m = 0; m < Items.Count(); m++)
            {

                i = _orderItems.IndexOf(Items[m]);
                OrderedItem = _client.ItemLookup(Items[m].idMenuItem);
                if (OrderedItem == null) throw new OrderException("Attempted complementary discount of unrecongnized menu item.");
                _Order.subTotal -= OrderedItem.price;
                itemComp = _client.CompLookup(Items[m].idMenuOrder);
                if (itemComp == null) throw new OrderException("Menu Item presented for complementary discount without matching database record: " + OrderedItem.name);
                _Order.subTotal += itemComp.price;
            }
        }
        #endregion
        #region ReportOps
        public CompBL[] DiscountReport()
        {
            if (_client == null)
                throw new ConnectionException("Client not connected to the system, discount report unavailable");
            return _client.DiscountReport();
        }
        public MenuItemBL[] Top3Report()
        {
            if (_client == null)
                throw new ConnectionException("Client not connected to the system, discount report unavailable");
            return _client.Top3Report();
        }
        public PaymentBL[] RevenueReport()
        {
            if (_client == null)
                throw new ConnectionException("Client not connected to the system, discount report unavailable");
            return _client.RevenueReport();
        }
        //get all closed orders on a specific date 
        internal List<OrderBL> GetAllClosedOrder(DateTime date)
        {
            if (_client != null)
                return new List<OrderBL>(_client.GetAllClosedOrder(date));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        } 

        internal List<FoodReportBL> GetAllFood(DateTime date, int idCategory)
        {
            if (_client != null)
                return new List<FoodReportBL>(_client.GetAllFoodReport(date, idCategory));
            else
                throw new ConnectionException("This RMS station is not connected to the server");
        }
        #endregion
    }
    #region Exceptions
    public class OrderException : Exception
    {
        public OrderException() { }
        public OrderException(string message) : base(message) { }
        public OrderException(string message, Exception inner) : base(message, inner) { }
    }
    public class UserException : Exception
    {
        public UserException() { }
        public UserException(string message) : base(message) { }
        public UserException(string message, Exception inner) : base(message, inner) { }

    }
    public class MenuException : Exception
    {
        public MenuException() { }
        public MenuException(string message) : base(message) { }
        public MenuException(string message, Exception inner) : base(message, inner) { }

    }
    public class ConnectionException : Exception
    {
        public ConnectionException() { }
        public ConnectionException(string message) : base(message) { }
        public ConnectionException(string message, Exception inner) : base(message, inner) { }

    }
    #endregion
}
