using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using RMS.Server.BL;
using RMS.Client.BL;

namespace RMS.Test.Client
{
    [TestClass]
    public class CustomerClientTests
    {
        CustomerController testclient = null;

        #region UserOps
        [TestMethod]
        public void TestControllerCreation()
        {
            testclient = new CustomerController();
            Assert.IsNotNull(testclient);
        }
        [TestMethod]
        public void TestCustomerCreation()
        {
            TestControllerCreation();
            string Name = "Megan Fox";
            DateTime BirthDay = new DateTime(1986, 5, 16);
            string Phone = "501-555-8265";
            string Email = "transformergrrl7@gmail.com";
            int id = 0;
            try
            {
                id = testclient.InsertUser(Name, BirthDay, Phone, Email);
            }
            catch(UserException)
            { //user already exists
                Assert.AreEqual(id, 0);
                return;
            }
            catch(Exception e)
            {
                throw new Exception("Insert Failed" + e.Message);
            }

            Assert.IsTrue(id > 0);
        }
        [TestMethod]
        public void TestCustomerLogin()
        {
            TestControllerCreation();
            TestCustomerCreation();                 
            string Name = "transformergrrl7@gmail.com";
            string Phone = "501-555-8265";
            bool result=testclient.UserLookup(Name, Phone);
            Assert.IsTrue(result);
            try
            {
                result = testclient.UserLookup(Name, Phone.Substring(4));
            }
            catch (UserException)
            {
                Assert.IsTrue(result);
            }
        }
        [TestMethod]
        public void GuestLogin()
        {
            //assume client creation works, but that's it
           TestControllerCreation();
                 
            string Email = "";
            string Phone = "";
            string expected = "Guest";
            string actual = "";

            try
            {
                testclient.UserLookup(Email, Phone);
                actual = testclient.username();
            }
            catch (ApplicationException e)
            {
                actual = "";
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(expected, actual, " ", "Guest account created and logged in");
        }
        [TestMethod]
        public void TestLookup()
        {
            //assume client creation works
            TestControllerCreation();//but not that it's been run this time
            TestCustomerCreation();
            string Name = "transformergrrl7@gmail.com";
            string Phone = "501-555-8265";
            string actual;
            string expected = "Megan Fox";
            try
            {
                testclient.UserLookup(Name, Phone);
                actual = testclient.username();
            }
            catch(Exception e)
            {
                actual = "";
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(expected, actual," ", expected + " successfully returned");
        }
        [TestMethod]
        public void TestReset()
        {
            //assume client creation works and at least the 'Megan Fox'
            //profile is inserted
            TestControllerCreation();//but not that it's been run
            TestCustomerCreation();
            TestCustomerLogin();
            string Email = "transformergrrl7@gmail.com";
            string new_Phone = "501-555-8265";
            bool success = false;
            try
            {
                success = testclient.PasswordReset(Email, new_Phone);
            }
            catch (Exception e)
            {
                success = false;
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(success, "Password (phone) successfully reset");
        }
        #endregion
        #region MenuOps
        [TestMethod]
        public void TestCategoryGet()
        {
            //assume that testclient is, and categories are loaded
            TestControllerCreation();
            string[] expected = new string[] { "Appetizers", "Entree", "Drink", "Dessert" };
            CategoryBL[] returned;
            bool match = false; // results of current test loop
            bool totalmatch = true; //results of last loop
            try
            {
                returned = testclient.GetAllCategory();
                foreach (CategoryBL cat in returned)
                {
                    match = false;
                    foreach (string n in expected)
                    {
                        if (cat.Name == n)
                        {
                            match = true;
                            break;
                        }
                        else match = false;
                    }
                    totalmatch = (totalmatch && match);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(totalmatch, "Returned categories match expected");
        }
        [TestMethod]
        public void TestMenuItemGet()
        {
            //assume that testclient is, and categories are loaded
           TestControllerCreation();
                 
            //can't test for each item at the moment, so just test for something returned
            IList<MenuItemBL> retrieved;

            try
            {
                retrieved = testclient.RetrieveCategory(2);
            }
            catch (Exception e)
            {
                retrieved = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(retrieved, "List of menu items retrieved successfully");
        }
        #endregion
        #region OrderOps
        [TestMethod]
        public void TestOrderCreation()
        {
            TestControllerCreation();
            GuestLogin();
            string request = "Make it good!";
            MenuItemBL testItem = testclient.RetrieveItem(1);
            IList<MenuOrderBL> returned;
            int expectedLength = 1;

            try
            {
                testclient.CreateOrder(testItem, request);
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestAssistanceRequest()
        {
            TestCustomerLogin();
            bool result = testclient.RequestAssistance();
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestRefillRequest()
        {
            TestCustomerLogin();
            MenuItemBL drink = testclient.RetrieveItem(6);
            bool result = testclient.RequestRefill(drink);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestsOrderExpansion()
        {
            TestOrderCreation();
            MenuItemBL testItem = testclient.RetrieveItem(2);
            IList<MenuOrderBL> returned;
            int expectedLength = 2;

            try
            {
                testclient.AddOrderItem(testItem, "");
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestOrderReduction()
        {
            TestsOrderExpansion();
            string request = "Make it good!";
            MenuItemBL testItem = testclient.RetrieveItem(1);
            IList<MenuOrderBL> returned;
            int expectedLength = 1;

            try
            {
                testclient.RemoveOrderItem(testItem, request);
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestOrderSubmission()
        {
            //assume that testclient is, and categories are loaded
            TestControllerCreation();
            TestLookup();
            TestOrderCreation();
            TestsOrderExpansion();                 
            int Unsubid = -1;
            int retid = -1;
            IList<MenuOrderBL> returned;

            try
            {
                testclient.SubmitOrder();
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);//depends heavily on the server
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            retid = returned[0].idOrder;
            Assert.AreNotSame(Unsubid, retid, "Order ID initialized correctly");
        }
        #endregion
        #region PayOps
        [TestMethod]
        public void TestTotal()
        {
            TestControllerCreation();
            GuestLogin();
            double expected = 0;
            double actual = 0.00;
            try
            {
                MenuItemBL testItem = testclient.RetrieveItem(1);
                expected += (double)testItem.price;
                testclient.CreateOrder(testItem, "");
                testItem = testclient.RetrieveItem(2);
                expected += (double)testItem.price;
                testclient.AddOrderItem(testItem, "");
                testItem = testclient.RetrieveItem(3);
                expected += (double)testItem.price;
                testclient.AddOrderItem(testItem, "");
                expected *= 1.085;
                actual = testclient.total(0.00);
            }
            catch (OrderException e)
            {
                Assert.Fail("Cannot test due to unknown values in a current order: " + e.Message);
            }
            catch (Exception e)
            {
                actual = 0.00;
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(expected, actual, .001, "Total without tip incorrectly calculated");
            actual = testclient.total(0.15);
            double percentage = expected * 1.15;
            Assert.AreEqual(percentage, actual, .001, "Total with percentage tip failed");
            actual = testclient.total(1.15);
            double literal = expected + 1.15;
            Assert.AreEqual(literal, actual, .001, "Total with literal tip failed");

        }
        [TestMethod]
        public void TestPayment()
        {
            TestControllerCreation();
            TestOrderCreation();
            TestsOrderExpansion();
            TestOrderSubmission();
            double pay = testclient.total(5.00);                 
            bool result = false;
            try
            {
                result = testclient.Payment(pay, "cash");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestSplit()
        {
            double expected = 0.00;
            double actual = 0.00;
            //create an order, sum up
            TestTotal();
            //split one item
            List<MenuOrderBL> Items = new List<MenuOrderBL>();
            IList<MenuOrderBL> ordered = testclient.OrderedItems();
            Items.Add(ordered[0]);
            ordered.RemoveAt(0);
            foreach(MenuOrderBL item in ordered)
                expected += (double)testclient.RetrieveItem(item.idMenuItem).price;
            expected *= 1.085;
            OrderBL newOrder = testclient.Split(Items);
            //check total
            actual = testclient.total(0.0);
            Assert.AreEqual(expected, actual, .001, "Split total unsuccessful");
        }
        [TestMethod]
        public void TestCompProcess()
        {
            TestOrderSubmission();//builds an order
            IList<MenuOrderBL> CompedItems = new List<MenuOrderBL>();
            IList<MenuOrderBL> orderedItems= testclient.OrderedItems();
            double expected = testclient.total(0.00)/1.085;
            double actual;
            CompedItems.Add(orderedItems[0]);
            MenuItemBL item=testclient.RetrieveItem(orderedItems[0].idMenuItem);
            if (item == null)
                throw new MenuException("Attempt to comp an item that does not exist");
            expected -= (double)item.price;
            expected = expected * 1.085;
            //Temporarily create a "Manager" to make a comp
            EmployeeClientTests manager = new EmployeeClientTests();
            manager.TestLookup();
            manager.OrderLookup(1);//will need to be input externally through RetrieveOrder(int idTable), good thing it's hard coded now
            manager.comp(orderedItems[0].idMenuOrder, orderedItems[0].idMenuItem);
            try
            {
                testclient.ProcessComp();
            }
            catch (OrderException o)
            {
                Assert.IsNotNull(o.Message);
            }
            catch (ConnectionException c)
            {
                Assert.Fail(c.Message);
            }
            actual = testclient.total(0.00);
            Assert.AreEqual(expected, actual, .001, "Unsuccessfully Comped a Item");
        }
        #endregion
    }
    [TestClass]
    public class EmployeeClientTests
    {
        EmployeeController testclient = null;

        #region UserOps

        [TestMethod]
        public void TestControllerCreation()
        {
            testclient = new EmployeeController();
            Assert.IsNotNull(testclient);
        }
        [TestMethod]
        public void TestWaitstaffCreation()
        {
            //assume client creation works
           TestControllerCreation();//but not that it's been run
            string Name = "Merry Anders";
            string Password = "PrehistoricWomen";
            string Type = "Wait Staff";
            int id = 0;

            try
            {
                id = testclient.InsertEmployee(Name, Password, Type);
                Assert.IsTrue(id > 0);
            }
            catch (UserException)
            {
                Assert.IsTrue(id == 0);
            }
        }
        public void OrderLookup(int table)
        {
            testclient.RetrieveOrder(table);
        }
        public int comp(int orderItem, int Item)
        {
            return testclient.CreateComp(orderItem, Item, "Comp for Processing Test", 0);
        }
        [TestMethod]
        public void TestKitchenCreation()
        {
            //assume client creation works
           TestControllerCreation();//but not that it's been run
                testclient = new EmployeeController();
            string Name = "Carroll Baker";
            string Password = "Carpetbaggers";
            string Type = "Kitchen";
            int id = 0;

            id = testclient.InsertEmployee(Name, Password, Type);

            Assert.IsTrue(id > 0);
        }
        [TestMethod]
        public void TestManagerCreation()
        {
            //assume client creation works
           TestControllerCreation();//but not that it's been run
            string Name = "Joan Bennett";
            string Password = "ScarlettStreet";
            string Type = "Manager";
            int id = 0;

            id = testclient.InsertEmployee(Name, Password, Type);

            Assert.IsTrue(id > 0);
        }
        [TestMethod]
        public void TestLookup()
        {
            //assume client creation works
            if(testclient == null)
                TestControllerCreation();//but not that it's been run this time
            string Name = "Joan Bennett";
            string Password = "ScarlettStreet";
            string actual = "";
            string expected = "Joan Bennett";
            try
            {
                testclient.UserLookup(Name, Password);
                actual = testclient.username();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(expected, actual, " ", expected + " successfully returned");
        }
        [TestMethod]
        public void TestReset()
        {
            //assume client creation works and at least the 'Megan Fox'
            //profile is inserted
            TestControllerCreation();//but not that it's been run
            TestWaitstaffCreation();
            TestLookup();
            string Name=testclient.username();
            string new_password="ScarlettStreet";
            bool success = false;

            try
            {
                success = testclient.PasswordReset(Name, new_password);
            }
            catch (ApplicationException e)
            {
                success = false;
                Assert.Fail(e.Message);
            }

            Assert.IsTrue(success, "Password (phone) successfully reset");
        }
        #endregion
        #region MenuOps
        [TestMethod]
        public void TestCategoryGet()
        {
            //assume that testclient is, and categories are loaded
           TestControllerCreation();
                testclient = new EmployeeController();
            string[] expected = new string[] { "Appetizers", "Entree", "Drink", "Dessert" };
            CategoryBL[] returned;
            bool match = false; // results of current test loop
            bool totalmatch = true; //results of last loop
            try
            {
                returned = testclient.GetAllCategory();
                foreach (CategoryBL cat in returned)
                {
                    match = false;
                    foreach (string n in expected)
                    {
                        if (cat.Name == n)
                        {
                            match = true;
                            break;
                        }
                        else match = false;
                    }
                    totalmatch = (totalmatch && match);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(totalmatch, "Returned categories match expected");
        }
        [TestMethod]
        public void TestMenuItemGet()
        {
            //assume that testclient is, and categories are loaded
            TestControllerCreation();

            //can't test for each item at the moment, so just test for something returned
            IList<MenuItemBL> retrieved;

            try
            {
                retrieved = testclient.RetrieveCategory(2);
            }
            catch (Exception e)
            {
                retrieved = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(retrieved, "List of menu items retrieved successfully");
        }
        #endregion
        #region OrderOps
        [TestMethod]
        public void TestOrderCreation()
        {
            if(testclient == null)
                TestControllerCreation();

            string request = "Make it good!";
            MenuItemBL testItem = testclient.RetrieveItem(1);
            IList<MenuOrderBL> returned;
            int expectedLength = 1;

            try
            {
                testclient.CreateOrder(testItem, request);
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestsOrderExpansion()
        {
            TestOrderCreation();
            MenuItemBL testItem = testclient.RetrieveItem(4);
            IList<MenuOrderBL> returned;
            int expectedLength = 2;

            try
            {
                testclient.AddOrderItem(testItem, "");
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestOrderReduction()
        {
            //assume that testclient is, and categories are loaded
            TestsOrderExpansion();
            string request = "";
            MenuItemBL testItem = testclient.RetrieveItem(4);
            IList<MenuOrderBL> returned;
            int expectedLength = 1;

            try
            {
                testclient.RemoveOrderItem(testItem, request);
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            Assert.AreEqual(expectedLength, returned.Count, .1, "Successfully added correct menu item");
        }
        [TestMethod]
        public void TestOrderSubmission()
        {
            //assume that testclient is, and categories are loaded
            TestsOrderExpansion();//uses order creation
            int Unsubid = -1;
            int retid = -1;
            IList<MenuOrderBL> returned;

            try
            {
                testclient.SubmitOrder();
                returned = testclient.OrderedItems();
            }
            catch (Exception e)
            {
                returned = null;
                Assert.Fail(e.Message);//depends heavily on the server
            }
            Assert.IsNotNull(returned, "Successfully returned some sort of order");
            retid = returned[0].idOrder;
            Assert.AreNotSame(Unsubid, retid, "Order ID initialized correctly");
        }
        #endregion
        #region PayOps
        [TestMethod]
        public void TestTotal()
        {
            TestControllerCreation();
            double expected = 0;
            double actual = 0.00;
            MenuItemBL testItem = testclient.RetrieveItem(1);
            try
            {
                expected += (double)testItem.price;
                testclient.CreateOrder(testItem, "");
                testItem = testclient.RetrieveItem(2);
                expected += (double)testItem.price;
                testclient.AddOrderItem(testItem, "");
                testItem = testclient.RetrieveItem(2);
                expected += (double)testItem.price;
                testclient.AddOrderItem(testItem, "");
                expected *= 1.085;
                actual = testclient.total(0.00);
            }
            catch (OrderException e)
            {
                Assert.Fail("Cannot test due to unknown values in a current order: " + e.Message);
            }
            catch (Exception e)
            {
                actual = 0.00;
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(expected, actual, .001, "Total without tip successfully calculated");
            actual = testclient.total(0.15);
            double percentage = expected * 1.15;
            Assert.AreEqual(percentage, actual, .001, "Total with percentage tip successful");
            actual = testclient.total(1.15);
            double literal = expected +1.15;
            Assert.AreEqual(literal, actual, .001, "Total with literal tip successful");

        }        
        [TestMethod]
        public void TestPayment()
        {
            TestOrderSubmission();
            double value = testclient.total(0.00);
            bool result = false;
            try
            {
                result = testclient.Payment(value+5, "cash");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestSplit()
        {
            double expected = 0.00;
            double actual = 0.00;
            //create an order, sum up
            TestTotal();
            //split one item
            List<MenuOrderBL> Items = new List<MenuOrderBL>();
            IList<MenuOrderBL> ordered = testclient.OrderedItems();
            actual = testclient.total(0.0);
            Items.Add(ordered[0]);
            for (int i = 1; i < ordered.Count; i++)
                expected += (double)testclient.RetrieveItem(ordered[i].idMenuItem).price;
            expected *= 1.085;
            OrderBL newOrder = testclient.Split(Items);
            //check total
            actual = testclient.total(0.0);
            Assert.AreEqual(expected, actual, .001, "Split total unsuccessful");
        }
        [TestMethod]
        public void TestCompCreation()
        {
            TestLookup();
            TestOrderSubmission();
            IList<MenuOrderBL> ordered = testclient.OrderedItems();
            int result = testclient.CreateComp(ordered[0].idMenuOrder,ordered[0].idMenuItem, "testing full comp", 0) ;
            Assert.AreNotEqual(-1,result, "Full comp failed");
            result = testclient.CreateComp(ordered[1].idMenuOrder, ordered[1].idMenuItem, "testing percentage comp", .5);
            Assert.AreNotEqual(-1,result,"Percentage comp failed");
        }
        [TestMethod]
        public void TestCompProcess()
        {
            TestLookup();
            TestOrderSubmission();
            //assumes that a comp has been entered, otherwise should fail. But I can't do that with this client
            IList<MenuOrderBL> orderedItems = testclient.OrderedItems();
            IList<MenuOrderBL> CompedItems = new List<MenuOrderBL>();
            double expected = testclient.total(0.00)/1.085;
            double actual;
            //Only comping last item
            CompedItems.Add(orderedItems[0]);
            expected -= (double)testclient.RetrieveItem(CompedItems[0].idMenuItem).price;
            expected *= 1.085;
            comp(CompedItems[0].idMenuOrder,CompedItems[0].idMenuItem);

            try
            {
                testclient.ProcessComp(CompedItems);
            }
            catch (OrderException o)
            {
                Assert.IsTrue(o.Message.Contains(":"), o.Message);
            }
            catch (ConnectionException c)
            {
                Assert.Fail(c.Message);
            }
            actual = testclient.total(0.00);
            Assert.AreEqual(expected, actual, .001, "Successfully Comped a Item");
        }
        #endregion
        #region ReportOps
        [TestMethod]
        public void TestDiscount()
        {
            TestControllerCreation();
            CompBL[] Discounts = testclient.DiscountReport();
            Assert.IsNotNull(Discounts);
        }
        [TestMethod]
        public void TestTop3()
        {
            TestControllerCreation();
            MenuItemBL[] sortedItems = testclient.Top3Report();
            Assert.IsNotNull(sortedItems);
        }
        [TestMethod]
        public void TestRevenue()
        {
            TestControllerCreation();
            PaymentBL[] payments = testclient.RevenueReport();
            Assert.IsNotNull(payments);
        }
        #endregion
        [TestMethod]
        public void TestRequestRetrieval()
        {
            TestControllerCreation();
            testclient.UserLookup("Loc", "1234");
            List<TableBL> active = null;
            active = testclient.RetrieveAssistanceRequests();
            Assert.IsNotNull(active);
        }
    }
}

