using RMS.Server.ServiceModel.Service.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Server.BL
{
    //Create service class that implements the service contract.
    enum OrderStatus { Open = 0, Send_To_Kitchen, Ready_Pickup, Closed};
    class RestaurantService : IRestaurantService
    {
        #region Translators
        private PaymentBL TranslatePaymentEntityToPaymentBL(Payment p)
        {
            PaymentBL P = new PaymentBL();
            P.amount = p.Amount;
            P.idCustomer = p.Customer_idCustomer;
            P.idOrder = p.Order_idOrder;
            P.idPayment = p.idPayment;
            P.method = p.Method;
            return P;
        }
        private CustomerBL TranslateCustomerEntityToCustomerBL(Customer e)
        {
            CustomerBL c = new CustomerBL();
            c.idCustomer = e.idCustomer;
            c.name = e.Name;
            c.birthday = e.Birthday.HasValue ? e.Birthday.Value : DateTime.MinValue;
            c.email = e.Email;
            c.phone = e.Phone_;
            c.points = e.Points;

            return c;
        }
        private MenuItemBL TranslateMenuItemEntityToMenuItemBL(Menu_Item m)
        {
            MenuItemBL item = new MenuItemBL();
            item.calories = m.Calories;
            item.description = m.Description;
            item.category = GetCategoryName((int)m.Category_idCategory);
            item.idMenuItem = m.idMenu_Item;
            item.name = m.Name;
            item.price = m.Price;
            item.status = m.Status; //get the status

            return item;
        }
        private OrderBL TranslateOrderEntityToOrderBL(Order o)
        {
            OrderBL oBL = new OrderBL();
            if (o.Date == null)
                oBL.date = DateTime.Today;
            else
                oBL.date = (DateTime)o.Date;
            oBL.idCustomer = (int)o.Customer_idCustomer;
            oBL.idOrder = o.idOrder;
            oBL.idServer = o.idServer.HasValue ? o.idServer.Value : -1;
            oBL.idTable = o.Table_idTable.HasValue ? o.Table_idTable.Value : -1;
            oBL.subTotal = o.SubTotal.HasValue ? o.SubTotal.Value : 0;
            oBL.tax = o.Tax.HasValue ? o.Tax.Value : 0;
            oBL.tip = o.Tip.HasValue ? o.Tip.Value : 0;
            if (o.Status.HasValue)
            {
                switch (o.Status.Value)
                {
                    case 0: //open
                        oBL.Status = "Open";
                        break;
                    case 1: //send_to_kitchen
                        oBL.Status = "Send To Kitchen";
                        break;
                    case 2:
                        oBL.Status = "Ready for Pick Up";
                        break;
                    case 3:
                        oBL.Status = "Closed";
                        break;
                    default:
                        break;
                }
            }
            return oBL;

        }
        private EmployeeBL TranslateEmployeeEntityToEmployeeBL(Employee e)
        {
            EmployeeBL c = new EmployeeBL();
            c.idEmployee = (int)e.idEmployee;
            c.employeeType = e.idEmployee_Type;
            c.name = e.Name;
            c.password = e.Password;

            return c;
        }
        private MenuOrderBL TranslateMenuOrderEntityToMenuOrderBL(Menu_Order item)
        {
            MenuOrderBL m = new MenuOrderBL();
            m.idMenuItem = item.idMenu_Item;
            m.idMenuOrder = item.idMenu_Order;
            if (item.idComp != null)
                m.idComp = (int)item.idComp;
            else
                m.idComp = -1;
            m.Request = item.Request;
            m.idOrder = item.idOrder;
            m.ItemName = GetMenuItemName(item.idMenu_Item);
            m.Status = item.Status;
            m.Price = GetMenuItemPrice(item.idMenu_Item);
            m.idCategory = GetMenuItemCategory(item.idMenu_Item);
            return m;
        }              
        private CompBL TranslateCompEntityToCompBL(Comp comp)
        {
            CompBL c = new CompBL();
            c.idComp = comp.idComp;
            c.idEmployee = comp.idEmployee;
            c.price = comp.Price;
            c.reason = comp.Text;
            return c;
        }
        private TableBL TranslateTableEntityToTableBL(Table table)
        {
            TableBL t = new TableBL();
            t.idTable = table.idTable;
            t.location = table.Location;
            t.idServer = table.idServer;
            return t;
        }
        private CategoryBL TranslateCategoryEntityToCategoryBL(Category e)
        {
            CategoryBL c = new CategoryBL();
            c.CategoryID = (int)e.idCategory;
            c.Name = e.Name;

            return c;
        }
        #endregion
        #region Category
        public IList<CategoryBL> GetAllCategory()
        {
            Console.WriteLine("Received Category list request");
            List<CategoryBL> lstCategory = new List<CategoryBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {

                    var myItem = (from c in db.Categories
                                  select c);

                    foreach (Category e in myItem)
                    {
                        lstCategory.Add(TranslateCategoryEntityToCategoryBL(e));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Category list request failed");
                    GetInternalException(ex);
                    return null;
                }
            }

            Console.WriteLine("Category list returned");
            return lstCategory;
        }
        public int InsertCategory(string name)
        {
            Console.WriteLine("Received InsertCategory");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check serviceName is existed or not.
                    var myItem = (from c in db.Categories where c.Name.ToLower() == name.ToLower() select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        //category is already existed.
                        return -1; //category already exsited, do not need to insert
                    }
                    else
                    {
                        //create product category.
                        Category e = new Category();

                        e.Name = name;
                        db.Categories.Add(e);
                        db.SaveChanges();
                        Console.WriteLine("Return InsertCategory");
                        return (int)e.idCategory;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Unable to insert a new Menu Category");
                    GetInternalException(ex);
                    return -1;
                }
            }

        }
        public string GetCategoryName(int CategoryID)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Cat = (from c in db.Categories
                               where c.idCategory == CategoryID
                               select c).FirstOrDefault();
                    if (Cat == null)
                        return null;
                    return Cat.Name;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not remove category");
                    GetInternalException(e);
                    return null;
                }

            }
        }
        public bool UpdateCategoryName(int idCategory, string name)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Cat = (from c in db.Categories
                               where c.idCategory == idCategory
                               select c).FirstOrDefault();
                    if (Cat == null)
                        return false;
                    Cat.Name = name;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not change category name");
                    GetInternalException(e);
                    return false;
                }

            }
        }
        public bool DeleteCategory(int idCategory)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Cat = (from c in db.Categories
                               where c.idCategory == idCategory
                               select c).FirstOrDefault();
                    if (Cat == null)
                        return false;
                    db.Categories.Remove(Cat);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not remove category");
                    GetInternalException(e);
                    return false;
                }

            }
        }
        #endregion
        #region Customer
        public int InsertCustomer(string name, DateTime bday, string phone, string email)
        {
            Console.WriteLine("Received InsertCustomer");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check Customer is existed or not.
                    var myItem = (from c in db.Customers
                                  where (c.Email.ToLower() == email.ToLower() || c.Phone_ == phone)
                                  select c).FirstOrDefault();
                    //Console.WriteLine("Search complete");
                    if (myItem != null)
                    {
                        Console.WriteLine("Customer Already Exists");
                        //customer is already existed.
                        return -1; 
                    }
                    else
                    {
                        //create product category.
                        Customer cus = new Customer();

                        cus.Phone_ = phone;
                        cus.Name = name;
                        cus.Points = 0;
                        cus.Email = email;
                        cus.Birthday = bday;
                        //Console.WriteLine("Customer profile built");
                        db.Customers.Add(cus);
                        //Console.WriteLine("profile added");
                        db.SaveChanges();
                        //Console.WriteLine("changes saved");
                        return (int)cus.idCustomer;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("InsertCustomer Failed");
                    GetInternalException(ex);
                    return -1;
                }
            }
        }
        public CustomerBL CustomerPhoneLookup(string phone)
        {
            Console.WriteLine("Received CustomerPhoneLookup");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {

                    var myItem = (from c in db.Customers
                                  where c.Phone_.ToLower() == phone.ToLower()
                                  select c).FirstOrDefault();

                    if (myItem != null)
                    {
                        Console.WriteLine("Return Received CustomerPhoneLookup ");
                        return TranslateCustomerEntityToCustomerBL(myItem);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: can not find CustomerPhoneLookup ");
                    GetInternalException(ex);
                    return null;
                }
            }
        }
        public CustomerBL CustomerEmailLookup(string email)
        {
            Console.WriteLine("Received CustomerEmailLookup");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {

                    var myItem = (from c in db.Customers
                                  where c.Email.ToLower() == email.ToLower()
                                  select c).FirstOrDefault();

                    if (myItem != null)
                    {
                        Console.WriteLine("Return Received CustomerEmailLookup ");
                        return TranslateCustomerEntityToCustomerBL(myItem);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: can not find CustomerEmailLookup ");
                    GetInternalException(ex);
                    return null;
                }
            }
        }
        public bool UpdateCustomer(CustomerBL customer)
        {
            Console.WriteLine("Received UpdateCustomer");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Customers
                                  where c.idCustomer == customer.idCustomer
                                  select c).FirstOrDefault();

                    if (myItem != null)
                    {
                        myItem.Birthday = customer.birthday;
                        myItem.Email = customer.email;
                        myItem.Name = customer.name;
                        myItem.Phone_ = customer.phone;
                        myItem.Points = customer.points;

                        db.SaveChanges();
                        Console.WriteLine("Return Received UpdateCustomer ");
                        return true;
                    }

                    //Console.WriteLine("Return Received UpdateCustomer ");
                    return false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: There is an error while UpdateCustomer ");
                    GetInternalException(ex);
                    return false;
                }
            }
        }
        public bool CustomerReset(string name, string new_phone)
        {
            Console.WriteLine("Received Customer password reset");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myCust = (from c in db.Customers
                                  where c.Email == name
                                  select c).FirstOrDefault();
                    if (myCust == null)
                        return false;
                    myCust.Phone_ = new_phone;
                    db.SaveChanges();
                    Console.WriteLine("Password reset");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not change password");
                    GetInternalException(e);
                    return false;
                }
            }
        }
        #endregion
        #region Employee
        public int InsertEmployee(int employeeCategoryId, string name, string password)
        {
            Console.WriteLine("Received InsertCustomer request");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check employee is existed or not.
                    var myItem = (from c in db.Employees
                                  where (c.Name.ToLower() == name.ToLower())
                                  select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        //employee is already existed.
                        return -1;
                    }
                    else
                    {
                        //create employee category.
                        Employee emp = new Employee();

                        emp.Name = name;
                        emp.idEmployee_Type = employeeCategoryId;
                        emp.Password = password;

                        db.Employees.Add(emp);
                        db.SaveChanges();
                        Console.WriteLine("Return InsertEmployee");
                        return (int)emp.idEmployee;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Unable to insert a new Employee");
                    GetInternalException(ex);
                    return -1;
                }
            }
        }
        public bool UpdateEmployee(EmployeeBL employee)
        {
            Console.WriteLine("Received UpdateEmployee");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Employees
                                  where c.idEmployee == employee.idEmployee
                                  select c).FirstOrDefault();

                    if (myItem != null)
                    {
                        myItem.Name = employee.name;
                        myItem.idEmployee_Type = employee.employeeType;
                        myItem.Password = employee.password;
                        db.SaveChanges();
                        Console.WriteLine("Return Received UpdateEmployee ");
                        return true;
                    }

                    //Console.WriteLine("Return Received UpdateEmployee ");
                    return false;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: There is an error while UpdateEmployee ");
                    GetInternalException(ex);
                    return false;
                }
            }
        }
        public EmployeeBL EmployeeLookup(string name)
        {
            Console.WriteLine("Received EmployeeLookup");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {

                    var myItem = (from c in db.Employees
                                  where c.Name.ToLower() == name.ToLower()
                                  select c).FirstOrDefault();

                    if (myItem != null)
                    {
                        Console.WriteLine("Return Received EmployeeLookup ");
                        return TranslateEmployeeEntityToEmployeeBL(myItem);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: can not find EmployeeLookup ");
                    GetInternalException(ex);
                    return null;
                }
            }
        }
        public int InsertEmployee(string name, string password, string type)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myType = (from t in db.Employee_Type
                                  where t.Name == type
                                  select t).FirstOrDefault();
                    if (myType == null)
                    {
                        Console.WriteLine("Incorrect Type presented to insert: " + type);
                        return -1;
                    }
                    Employee e = new Employee();
                    e.Name = name;
                    e.Password = password;
                    e.idEmployee_Type = myType.idEmployee_Type;

                    db.Employees.Add(e);
                    db.SaveChanges();
                    return e.idEmployee;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not add employee");
                    GetInternalException(e);
                    return -1;
                }
            }
        }
        public bool EmployeeReset(string name, string new_password)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Emp = (from e in db.Employees
                               where e.Name == name
                               select e).FirstOrDefault();
                    if (Emp == null)
                        return false;
                    Emp.Password = new_password;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not change password");
                    GetInternalException(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// This method will return a list of order on a specific date
        /// </summary>
        /// <returns></returns>
        public IList<OrderBL> GetAllClosedOrder(DateTime date)
        {
            Console.WriteLine("Received GetAllOrderTableStatus");
            List<OrderBL> lstOrder = new List<OrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Orders
                                  where (c.Date.Value.Day == date.Day 
                                            && c.Date.Value.Month == date.Month
                                            && c.Date.Value.Year == date.Year
                                            && c.Status == 3) // 3: closed 
                                  select c);

                    foreach (Order m in myItem)
                    {
                        lstOrder.Add(TranslateOrderEntityToOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllOrderTableStatus" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllOrderTableStatus ");
            return lstOrder;
        }

        public IList<OrderBL> GetAllOrderTableStatus()
        {
            Console.WriteLine("Received GetAllOrderTableStatus");
            List<OrderBL> lstOrder = new List<OrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Orders
                                  where c.Status != 3 // 3: closed 
                                  select c);

                    foreach (Order m in myItem)
                    {
                        lstOrder.Add(TranslateOrderEntityToOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllOrderTableStatus" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllOrderTableStatus ");
            return lstOrder;
        }
		
		public IList<OrderBL> GetAllAvailableOrdersForKitchen()
        {
            Console.WriteLine("Received GetAllAvailableOrdersForKitchen");
            List<OrderBL> lstOrder = new List<OrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Orders 
                                  where c.Status == 1 // 1: is send_to_kitchen status 
                                  select c);

                    foreach (Order m in myItem)
                    {
                        lstOrder.Add(TranslateOrderEntityToOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllAvailableOrdersForKitchen" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllAvailableOrdersForKitchen ");
            return lstOrder;
        }

        /// <summary>
        /// this function will return all menu order in submitted status 
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IList<MenuOrderBL> GetAllMenuOrderForKitchen(int orderId)
        {
            Console.WriteLine("Received GetAllMenuOrderForKitchen");
            List<MenuOrderBL> lstMenuOrder = new List<MenuOrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Order
                                  where (c.idOrder == orderId 
                                            && c.Status.ToLower() == "submitted")
                                  select c);

                    foreach (Menu_Order m in myItem)
                    {
                        lstMenuOrder.Add(TranslateMenuOrderEntityToMenuOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllMenuOrderForKitchen" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllMenuOrderForOrder ");
            return lstMenuOrder;
        }

        public IList<MenuOrderBL> GetAllMenuOrderForOrder(int orderId)
        {
            Console.WriteLine("Received GetAllMenuOrderForOrder");
            List<MenuOrderBL> lstMenuOrder = new List<MenuOrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Order
                                  where c.idOrder == orderId                                           
                                  select c);

                    foreach (Menu_Order m in myItem)
                    {
                        lstMenuOrder.Add(TranslateMenuOrderEntityToMenuOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllMenuOrderForOrder" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllMenuOrderForOrder ");
            return lstMenuOrder;
        }
        #endregion
        #region Menu Item
        private string GetMenuItemName(int menuItem_id)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Item
                                  where c.idMenu_Item == menuItem_id
                                  select c).FirstOrDefault();

                    if (myItem != null)
                        return myItem.Name;

                    return string.Empty;

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of Menu Item " + ex.Message);
                }
            }
        }

        private decimal GetMenuItemPrice(int menuItem_id)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Item
                                  where c.idMenu_Item == menuItem_id
                                  select c).FirstOrDefault();

                    if (myItem != null)
                        return myItem.Price;

                    return 0;

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get price of Menu Item " + ex.Message);
                }
            }
        }

        private int GetMenuItemCategory(int menuItem_id)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Item
                                  where c.idMenu_Item == menuItem_id
                                  select c).FirstOrDefault();

                    if (myItem != null)
                        return (int)myItem.Category_idCategory;

                    return 0;

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get price of Menu Item " + ex.Message);
                }
            }
        }

        public IList<MenuItemBL> GetAllMenuItem()
        {
            Console.WriteLine("Received GetAllMenuItem");
            List<MenuItemBL> lstMenuItem = new List<MenuItemBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Item
                                  select c);

                    foreach (Menu_Item m in myItem)
                    {
                        lstMenuItem.Add(TranslateMenuItemEntityToMenuItemBL(m));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of Menu Item " + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllMenuItems ");
            return lstMenuItem;
        }

        public IList<MenuItemBL> GetAllMenuItems(int idCategory)
        {
            Console.WriteLine("Received GetAllMenuItems");
            List<MenuItemBL> lstMenuItem = new List<MenuItemBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Menu_Item
                                  where c.Category_idCategory == idCategory
                                  select c);

                    foreach (Menu_Item m in myItem)
                    {
                        lstMenuItem.Add(TranslateMenuItemEntityToMenuItemBL(m));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: can not get list of Menu Item " );
                    GetInternalException(ex);
                    return null;
                }
            }

            Console.WriteLine("Return Received GetAllMenuItems ");
            return lstMenuItem;
        }
        public bool DeactivateItem(MenuItemBL dead)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var offItem = (from i in db.Menu_Item
                                   where i.idMenu_Item == dead.idMenuItem
                                   select i).FirstOrDefault();
                    if (offItem == null)
                        return false;//couldn't find to turn off
                    if (offItem.Status == 1)
                        offItem.Status = 0;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Item cannot be deactivated: ");
                    GetInternalException(e);
                    return false;
                }
            }
        }
        public bool ActivateItem(MenuItemBL dead)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var offItem = (from i in db.Menu_Item
                                   where i.idMenu_Item == dead.idMenuItem
                                   select i).FirstOrDefault();
                    if (offItem == null)
                        return false;//couldn't find to turn off
                    if (offItem.Status == 0)
                        offItem.Status = 1;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Item cannot be activated: ");
                    GetInternalException(e);
                    return false;
                }
            }
        }
        public IList<MenuItemBL> CategoryRetrieve(int idCategory)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    List<MenuItemBL> ret = new List<MenuItemBL>();
                    var myItems = (from item in db.Menu_Item
                                   where item.Category_idCategory == idCategory
                                   select item);
                    if (myItems == null)
                        return null;
                    foreach (Menu_Item item in myItems)
                        ret.Add(TranslateMenuItemEntityToMenuItemBL(item));
                    return ret;
                }
                catch (Exception e)
                {
                    Console.Write("Could not retrieve category items: " + e.Message);
                    return null;
                }
            }
        }
        public IList<MenuItemBL> GetAvailableMenuItems(int idCategory)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    List<MenuItemBL> ret = new List<MenuItemBL>();
                    var myItems = (from item in db.Menu_Item
                                   where (item.Category_idCategory == idCategory && item.Status == 1) //1: mean the menu item is active
                                   select item);
                    if (myItems == null)
                        return null;
                    foreach (Menu_Item item in myItems)
                        ret.Add(TranslateMenuItemEntityToMenuItemBL(item));
                    return ret;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not retrieve category items: ");
                    GetInternalException(e);
                    return null;
                }
            }
        }
        public MenuItemBL ItemLookup(int idItem)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Item = (from i in db.Menu_Item
                                where i.idMenu_Item == idItem
                                select i).FirstOrDefault();
                    if (Item == null)
                        return null;
                    return TranslateMenuItemEntityToMenuItemBL(Item);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not find menu item" );
                    GetInternalException(e);
                    return null;
                }

            }
        }
        public double PriceLookup(int idItem)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Item = (from i in db.Menu_Item
                                where i.idMenu_Item == idItem
                                select i).FirstOrDefault();
                    if (Item == null)
                        return -1.0;
                    return (double)Item.Price;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not remove category" );
                    GetInternalException(e);
                    return -1.0;
                }

            }
        }
        #endregion
        #region Order
        public bool ReadyItem(int Item)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from m in db.Menu_Order
                                  where m.idMenu_Order == Item
                                  select m).FirstOrDefault();
                    if (myItem == null)
                        return false;
                    myItem.Status = "Ready";
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not update item: ");
                    GetInternalException(e);
                    return false;
                }
            }

        }
        public int CompItem(CompBL Discount)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    Comp c = new Comp();
                    c.idEmployee = Discount.idEmployee;
                    c.Price = (decimal)Discount.price;
                    c.Text = Discount.reason;

                    db.Comps.Add(c);
                    db.SaveChanges();
                    return c.idComp;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not create new discount: " );
                    GetInternalException(e);
                    return -1;
                }
            }
        }

        public int SubmitOrder(OrderBL new_order)
        {
            Console.Write("Start SubmitOrder: ");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                Order o = new Order();
                var table = (from t in db.Tables
                             where t.idTable == new_order.idTable
                             select t).FirstOrDefault();
                var serv = (from s in db.Employees
                            where s.idEmployee == new_order.idServer
                            select s).FirstOrDefault();
                if (table == null)
                {
                    Console.WriteLine("Unknown table");
                    return -1;
                }
                if (serv == null)
                    o.idServer = table.idServer;
                else
                    o.idServer = serv.idEmployee;
                o.Date = new_order.date;
                o.Status = 1;
                o.SubTotal = (decimal)new_order.subTotal;
                o.Table_idTable = table.idTable;
                o.Tax = (decimal)new_order.tax;
                o.Tip = (decimal)new_order.tip;
                o.Customer_idCustomer = new_order.idCustomer;
                try
                {

                    if (new_order.idCustomer == -1)
                    {
                        //will do it in client side
                        Customer c = new Customer();
                        c.Name = "Guest";
                        c.Phone_ = "0000000000";
                        c.Email = "cs4444@unt.edu";

                        db.Customers.Add(c);
                        o.Customer_idCustomer = c.idCustomer;
                    }

                    db.Orders.Add(o);
                    Console.WriteLine("Order Successfully built, saving");
                    db.SaveChanges();

                    Console.Write("End SubmitOrder: ");

 
                    return o.idOrder;
                }
                catch (Exception e)
                {
                    Console.Write("SubmitOrder: Could not add Order" + e.Message);
GetInternalException(e);
                    return -1;
                }
            }
        }
        public int SubmitOrderItem(MenuOrderBL item)
        {
            Console.WriteLine("Setting up Ordered Item");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    Menu_Order new_item = new Menu_Order();
                    new_item.idOrder = item.idOrder;
                    new_item.idMenu_Item = item.idMenuItem;
                    new_item.Request = item.Request;
                    new_item.Status = "Submitted";                  

                    db.Menu_Order.Add(new_item);
                    Console.WriteLine("new MenuOrder created");
                    db.SaveChanges(); //save to DB

                    Console.WriteLine("new MenuOrder submitted to db. id:"+new_item.idMenu_Order);

                    return new_item.idMenu_Order;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not create order item" );
                    GetInternalException(e);
                    return -1;
                }

            }
        }
        public OrderBL RetrieveOrder(int idTable)
        {
            Console.WriteLine("Recieved order request for table " + idTable);
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //selection lambda
                    System.Linq.Expressions.Expression<Func<Order, bool>> thisday = t => t.Date > DateTime.Today;
                    var Ord = (from o in db.Orders
                               where o.Table_idTable == idTable && o.Status == 1
                               orderby o.Date
                               select o);
                    if (Ord == null)
                        return null;
                    //Console.WriteLine("Orderset returned");
                    Order Max = null;
                    //Console.WriteLine("Searching for the most recent");
                    foreach (Order o in Ord)
                    {
                        //Console.WriteLine("checking order:");
                        //Console.WriteLine("#" + o.idOrder);
                        if (Max == null)
                            Max = o;
                        if (o.Date > Max.Date)
                            Max = o;
                    }
                    Console.WriteLine("Order found: " + Max.idOrder);
                    return TranslateOrderEntityToOrderBL(Max);
                }
                catch (Exception e)
                {
                    Console.WriteLine("No current order found" );
                    GetInternalException(e);
                    return null;
                }

            }
        }
        public IList<MenuOrderBL> RetrieveOrderItems(int Order)
        {
            Console.WriteLine("Retrieving items associated with order: " + Order);
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                List<MenuOrderBL> ret = new List<MenuOrderBL>();
                try
                {
                    var Items = (from i in db.Menu_Order
                                 where i.idOrder == Order
                                 select i);
                    if (Items == null)
                    {
                        Console.WriteLine("No items found");
                        return null;
                    }
                    Console.WriteLine(Items.Count() + " items found");
                    foreach (Menu_Order Item in Items)
                        ret.Add(TranslateMenuOrderEntityToMenuOrderBL(Item));
                    Console.WriteLine(ret.Count + " items translated and returned");
                    return ret;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not retrieve order items");
                    GetInternalException(e);
                    return null;
                }

            }
        }
        public bool UpdateOrderStatus(int idOrder, int status)
        {
            Console.WriteLine("Received UpdateOrderStatus");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check employee is existed or not.
                    var myItem = (from c in db.Orders
                                  where (c.idOrder == idOrder)
                                  select c).FirstOrDefault();
                    if (myItem == null)
                    {
                        //order does not exist.
                        return false;
                    }
                    else
                    {
                        //create employee category.
                        myItem.Status = status;
                        db.SaveChanges();

                        Console.WriteLine("Return UpdateOrderStatus");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Unable to update a UpdateOrderStatus" );
                    GetInternalException(ex);
                    return false;
                }
            }
        }

        public bool UpdateOrderTip(int idOrder, decimal tip)
        {
            Console.WriteLine("Received UpdateOrderTip");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check employee is existed or not.
                    var myItem = (from c in db.Orders
                                  where (c.idOrder == idOrder)
                                  select c).FirstOrDefault();
                    if (myItem == null)
                    {
                        //order does not exist.
                        return false;
                    }
                    else
                    {
                        //create employee category.
                        myItem.Tip = tip;
                        db.SaveChanges();

                        Console.WriteLine("Return UpdateOrderTip");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: Unable to update a UpdateOrderMenuStatus" + ex.Message);
                }
            }
        }

        public bool UpdateOrderMenuStatus(int menuOrderId, string status)
        {
            Console.WriteLine("Received UpdateOrderMenuStatus");
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //check employee is existed or not.
                    var myItem = (from c in db.Menu_Order
                                  where (c.idMenu_Order == menuOrderId)
                                  select c).FirstOrDefault();
                    if (myItem == null)
                    {
                        //order does not exist.
                        return false;
                    }
                    else
                    {
                        //create employee category.
                        myItem.Status = status;
                        db.SaveChanges();

                        Console.WriteLine("Return UpdateOrderMenuStatus");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: Unable to update a UpdateOrderMenuStatus" + ex.Message);
                }
            }
        }

        public bool ClaimItem(int Item)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from i in db.Menu_Order
                                  where i.idMenu_Order == Item
                                  select i).FirstOrDefault();
                    if (myItem == null)
                        return false;
                    myItem.Status = "Claimed";
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not claim order item" );
                    GetInternalException(e);
                    return false;
                }

            }
        }
        public IList<OrderBL> GetOrderByTableId(int tableId)
        {
            Console.WriteLine("Received GetOrderByTableId");
            List<OrderBL> lstOrder = new List<OrderBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var myItem = (from c in db.Orders
                                  where c.Table_idTable == tableId
                                  select c);

                    foreach (Order m in myItem)
                    {
                        lstOrder.Add(TranslateOrderEntityToOrderBL(m));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: can not get list of Orders by TableId" );
                    GetInternalException(ex);
                    return null;
                }
            }

            Console.WriteLine("Return Received GetOrderByTableId ");
            return lstOrder;
        }
        public CompBL CompLookup(int idComp)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
/*                    var Item = (from i in db.Menu_Order
                                where i.idMenu_Order == idMenuOrder
                                select i).FirstOrDefault();
                    if (Item == null || Item.idComp == null)
                        return null;*/
                    var Comp = (from c in db.Comps
                                where c.idComp == idComp
                                select c).FirstOrDefault();
                    if (Comp == null)
                        return null;
                    return TranslateCompEntityToCompBL(Comp);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not find comp for provided order item");
                    GetInternalException(e);
                    return null;
                }

            }
        }
        internal void GetInternalException (Exception e)
        {
            while (e.InnerException != null)
                e = e.InnerException;
            Console.WriteLine(e.Message);
        }
        public bool updateComp(int idMenuOrder, int idComp)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Item = (from i in db.Menu_Order
                                where i.idMenu_Order == idMenuOrder
                                select i).FirstOrDefault();
                    if (Item == null)
                    {
                        Console.WriteLine("Could not find associated order item");
                        return false;
                    }
                    Item.idComp = idComp;
                    db.SaveChanges();
                    return true;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not associate comp with order item: ");
                    GetInternalException(e);
                    return false;
                }

            }
        }
        #endregion
        #region Reports
        public CompBL[] DiscountReport()
        {
            DateTime cutoff = DateTime.Today.Add(new TimeSpan (-24,0,0));
            List<CompBL> ret = new List<CompBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                var comps = (from o in db.Orders from i in db.Menu_Order from c in db.Comps
                              where o.Date >= cutoff && i.idOrder == o.idOrder && i.idComp == c.idComp
                              select c);
                if (comps == null)
                {
                    Console.WriteLine("No recent discounts to report");
                    return null;
                }
                foreach (Comp c in comps)
                    ret.Add(TranslateCompEntityToCompBL(c));
                return ret.ToArray();
            }
        }
        public MenuItemBL[] Top3Report()
        {
            DateTime cutoff = DateTime.Today.Add(new TimeSpan(-24, 0, 0));
            List<MenuItemBL> ret = new List<MenuItemBL>();
            Dictionary<MenuItemBL, int> unordered = new Dictionary<MenuItemBL, int>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                var Items = (from o in db.Orders
                             from i in db.Menu_Order
                             from m in db.Menu_Item
                             where o.Date >= cutoff && i.idOrder == o.idOrder && i.idMenu_Item == m.idMenu_Item
                             select m);
                if (Items == null) return null;
                foreach(Menu_Item m in Items)
                {
                    MenuItemBL M = TranslateMenuItemEntityToMenuItemBL(m);
                    if (unordered.ContainsKey(M))
                        unordered[M] += 1;
                    else
                        unordered.Add(M, 1);
                }
                var sorted = (from pair in unordered
                              orderby pair.Value descending
                              select pair);
                foreach (KeyValuePair<MenuItemBL, int> pair in sorted)
                    ret.Add(pair.Key);
                return ret.ToArray();
            }
        }
        public PaymentBL[] RevenueReport()
        {
            DateTime cutoff = DateTime.Today.Add(new TimeSpan(-24, 0, 0));
            List<PaymentBL> ret = new List<PaymentBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                var pays = (from p in db.Payments
                            from o in db.Orders
                            where o.Date > cutoff && o.idOrder == p.Order_idOrder
                            select p);
                if (pays == null) return null;
                foreach (Payment p in pays)
                    ret.Add(TranslatePaymentEntityToPaymentBL(p));
                return ret.ToArray();
            }
        }

        public IList<FoodReportBL> GetAllFoodReport(DateTime date, int idCategory)
        {
            Console.WriteLine("Received GetAllFoodReport");
           
            //get list of all order on a specific date
            List<OrderBL> lstOrder = new List<OrderBL>(GetAllClosedOrder(date));

            //find all menu order of each order
            List<MenuOrderBL> lstMenuOrder = new List<MenuOrderBL>();
            foreach (OrderBL o in lstOrder)
            {
                lstMenuOrder.AddRange(GetAllMenuOrderForOrder(o.idOrder));
            }

            List<MenuOrderBL> lstMenuOrderInCategory = new List<MenuOrderBL>();
            foreach (MenuOrderBL m in lstMenuOrder)
            {
                if (m.idCategory == idCategory)
                    lstMenuOrderInCategory.Add(m);
            }

            //count how many time an menu item was occurred during this time.
            //it means that we need to group by menu item
            List<FoodReportBL> result = lstMenuOrderInCategory
                                            .GroupBy(m => m.idMenuItem)
                                            .Select(cm => new FoodReportBL
                                            {
                                                name = cm.First().ItemName,
                                                count = cm.Count()                                               
                                            }).OrderByDescending(o => o.count).ToList();

            Console.WriteLine("Return Received GetAllFoodReport ");
            return result;
        }
        #endregion
        #region Payments
        public int Payment(int idOrder, string method, double amount, int custId)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    Payment p = new Payment();
                    p.Amount = (decimal)amount;
                    p.Method = method;
                    p.Order_idOrder = idOrder;
                    p.Customer_idCustomer = custId;

                    //need to has customer id in order to pay, 
                    //get customer_id from idOrder.
                    var order = (from c in db.Orders where c.idOrder == idOrder select c).FirstOrDefault();
                    if (order == null)
                        return -1;
                    p.Customer_idCustomer = order.Customer_idCustomer.Value;

                    db.Payments.Add(p);
                    db.SaveChanges();
                    return p.idPayment;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not process payment: ");
                    GetInternalException(e);
                    return -1;
                }
            }
        }
        #endregion

        #region Table
        /// <summary>
        /// Return a list of Available Tables for WaitStaff
        /// </summary>
        /// <returns></returns>
        public IList<TableBL> GetAllAvailableTables()
        {
            Console.WriteLine("Received GetAllAvailableTables");
            List<TableBL> lstTable = new List<TableBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //list of all table
                    var myItem = (from c in db.Tables                                  
                                  select c);

                    IList<OrderBL> lstOrders = GetAllOrderTableStatus();            
        
                    //get all list occupy table id.
                    List<int> lstOccupyTable = new List<int>();
                    foreach (OrderBL o in lstOrders)
                        lstOccupyTable.Add(o.idTable);

                    foreach (Table t in myItem)
                    {
                        if (!lstOccupyTable.Contains(t.idTable))
                            lstTable.Add(TranslateTableEntityToTableBL(t));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllAvailableTables" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllAvailableTables ");
            return lstTable;
        }
        public IList<TableBL> GetAllAssistanceRequests(int idEmp)
        {
            Console.WriteLine("Received GetAllAssistanceRequests");
            List<TableBL> lstTable = new List<TableBL>();
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    //list of all table
                    var myItem = (from c in db.Tables
                                  where c.idServer == idEmp && c.Request != null
                                  select c);

                    foreach (Table t in myItem)
                    {
                        lstTable.Add(TranslateTableEntityToTableBL(t));
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error: can not get list of GetAllAssistanceRequests" + ex.Message);
                }
            }

            Console.WriteLine("Return Received GetAllAvailableTables ");
            return lstTable;
        }
        public bool RequestAssistance(int table, string AssistType)
        {
            using (dbrestaurantEntities db = new dbrestaurantEntities())
            {
                try
                {
                    var Table = (from t in db.Tables
                                 where t.idTable == table
                                 select t).FirstOrDefault();
                    if (Table == null)
                    {
                        Console.WriteLine("Table request could not be updated due to unknown table");
                        return false;
                    }
                    Table.Request = AssistType;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Table request could not be processed:");
                    GetInternalException(e);
                    return false;
                }
            }
        }
        #endregion
    }
}
