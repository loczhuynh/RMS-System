using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RMS.Server.BL
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://RMS.Server.ServiceModel.Service.BL")]
    public interface IRestaurantService
    {
        #region CategoryOps
        [OperationContract]
        int InsertCategory(string name);
        [OperationContract]
        string GetCategoryName(int CategoryID);
        [OperationContract]
        bool UpdateCategoryName(int idCategory, string name);
        [OperationContract]
        bool DeleteCategory(int idCategory);
        #endregion
        #region UserOps
        [OperationContract]
        //creates an entry in the customer table
        int InsertCustomer(string name, DateTime bday, string phone, string email);
        [OperationContract]
        //creates an entry in the employee table
        int InsertEmployee(string name, string password, string type);
        [OperationContract]
        //resets an employee's password
        bool EmployeeReset(string name, string new_password);
        [OperationContract]
        //resets a customer's phone#
        bool CustomerReset(string name, string new_phone);
        [OperationContract]
        //retrieve a customer's profile information
        CustomerBL CustomerPhoneLookup(string phone);
        [OperationContract]
        //retrieve a customer's profile information
        CustomerBL CustomerEmailLookup(string email);
        [OperationContract]
        //retrieve an employee's profile information
        EmployeeBL EmployeeLookup(string name);
        #endregion
        #region OrderOps
        [OperationContract]
        //changes the status on an order item
        bool UpdateOrderStatus(int idOrder, int status);
        [OperationContract]
        //update tip for Order
        bool UpdateOrderTip(int idOrder, decimal tip);
        [OperationContract]
        //Initiates the order processing, inserts order information into correct tables
        int SubmitOrder(OrderBL new_order);
        [OperationContract]
        int SubmitOrderItem(MenuOrderBL Item);
        [OperationContract]
        OrderBL RetrieveOrder(int idTable);
        [OperationContract]
        IList<MenuOrderBL> RetrieveOrderItems(int Order);
        [OperationContract]
        //changes the status on an order item
        bool ClaimItem(int Item);
		[OperationContract]
        //return list of open order of a table
        IList<OrderBL> GetOrderByTableId(int tableId);
        [OperationContract]
        //return list of avaialbe order for kitchen
        IList<OrderBL> GetAllAvailableOrdersForKitchen();
        [OperationContract]
        IList<OrderBL> GetAllOrderTableStatus();
        [OperationContract]
        //return list of menu order for one specific order
        IList<MenuOrderBL> GetAllMenuOrderForOrder(int orderId);
        [OperationContract]
        IList<MenuOrderBL> GetAllMenuOrderForKitchen(int orderId);
        [OperationContract]
        //return list of menu order for one specific order
        bool UpdateOrderMenuStatus(int menuOrderId, string status);
        [OperationContract]
        //update order item status to Ready
        bool ReadyItem(int Item);
        [OperationContract]
        bool RequestAssistance(int table, string AssistType);
        #endregion
        #region MenuOps
        [OperationContract]
        IList<CategoryBL> GetAllCategory();
        [OperationContract]
        IList<MenuItemBL> GetAllMenuItem();
        [OperationContract]
        //Retrieve all menu items in a category
        IList<MenuItemBL> CategoryRetrieve(int idCategory);
        //Retrieve all available menu items in a category
        [OperationContract]
        IList<MenuItemBL> GetAvailableMenuItems(int idCategory);
        [OperationContract]
        double PriceLookup(int idItem);
        [OperationContract]
        MenuItemBL ItemLookup(int idItem);
        [OperationContract]
        bool ActivateItem(MenuItemBL alive);
        [OperationContract]
        bool DeactivateItem(MenuItemBL dead);
        #endregion
        #region PaymentOps
        [OperationContract]
        CompBL CompLookup(int idMenuOrder);
        [OperationContract]
        int CompItem(CompBL newComp);
        [OperationContract]
        bool updateComp(int idMenuOrder, int idComp);
        [OperationContract]
        //Stores payment information
        int Payment(int idOrder, string method, double amount, int userID);        
        #endregion
        #region ReportOps
        [OperationContract]
        CompBL[] DiscountReport();
        [OperationContract]
        MenuItemBL[] Top3Report();
        [OperationContract]
        PaymentBL[] RevenueReport();
        [OperationContract]
        //return all closed order on a specific date
        IList<OrderBL> GetAllClosedOrder(DateTime date);
        [OperationContract]
        IList<FoodReportBL> GetAllFoodReport(DateTime date, int idCategory);
        #endregion
        #region TableOps
        [OperationContract]
        IList<TableBL> GetAllAvailableTables();
        [OperationContract]
        IList<TableBL> GetAllAssistanceRequests(int idEmp);
        #endregion
    }

    [DataContract]
    public class CategoryBL
    {
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    [DataContract]
    public class CustomerBL
    {
        [DataMember]
        public int idCustomer { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public DateTime birthday { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public int points { get; set; }
    }
    [DataContract]
    public class EmployeeBL
    {
        [DataMember]
        public int idEmployee { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int employeeType { get; set; }
        [DataMember]
        public string password { get; set; }
    }
    [DataContract]
    public class OrderBL
    {
        [DataMember]
        public int idOrder { get; set; }
        [DataMember]
        public int idTable { get; set; }
        [DataMember]
        public int idServer { get; set; }
        [DataMember]
        public int idCustomer { get; set; }
        [DataMember]
        public decimal tax { get; set; }
        [DataMember]
        public decimal subTotal { get; set; }
        [DataMember]
        public decimal tip { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public DateTime date { get; set; }

    }
    [DataContract]
    public class MenuOrderBL
    {
        [DataMember]
        public int idOrder { get; set; }
        [DataMember]
        public int idMenuItem { get; set; }
        [DataMember]
        public int idComp { get; set; }       
        [DataMember]
        public string Request { get; set; }
        [DataMember]
        public int idMenuOrder { get; set; }
		[DataMember]
        public string ItemName { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int idCategory { get; set; }
    }
    [DataContract]
    public class MenuItemBL
    {
        [DataMember]
        public int idMenuItem { get; set; }
        [DataMember]
        public int calories { get; set; }
        [DataMember]
        public decimal price { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }//contains allergen information
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public int status { get; set; }
    }
    [DataContract]
    public class PaymentBL
    {
        [DataMember]
        public int idPayment { get; set; }
        [DataMember]
        public int idOrder { get; set; }
        [DataMember]
        public int idCustomer { get; set; }
        [DataMember]
        public string method { get; set; }
        [DataMember]
        public decimal amount { get; set; }
    }
    [DataContract]
    public class CompBL
    {
        [DataMember]
        public int idComp { get; set; }
        [DataMember]
        public int idEmployee { get; set; }
        [DataMember]
        public string reason { get; set; }
        [DataMember]
        public string couponCode { get; set; }
        [DataMember]
        public DateTime expirationDate { get; set; }
        [DataMember]
        public decimal price { get; set; }
    }
    [DataContract]
    public class TableBL
    {
        [DataMember]
        public int idTable { get; set; }
        [DataMember]
        public int idServer { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string Request { get; set; }
    }

    [DataContract]
    public class FoodReportBL
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int count { get; set; }        
    }
}

