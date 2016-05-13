using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMS.UI.ViewModel;

namespace RMS.UI.Test
{
    [TestClass]
    public class VMUsersTest
    {
        [TestMethod]
        public void ConstructorInit()
        {
            // Arrange
            VMUsers testUser = GetVMUsers();

            // Act
            // Assert
            Assert.AreNotEqual("", testUser.UiPhoneNumber);
            Assert.AreNotEqual("", testUser.UiPassword);
            Assert.AreEqual("Login", testUser.UiButtonState);
            Assert.AreEqual((int)VMUsers.loginTypes.loggedOut, testUser.LoginStatus);
            Assert.AreEqual("", testUser.CurrentUser);
        }

        [TestMethod]
        public void GetUser()
        {
            // Arrange
            VMUsers testUser = GetVMUsers();

            // Act
            // Assert
            Assert.AreEqual(null, testUser.GetUser());

            // Act
            testUser.UiPhoneNumber = "1112223333";
            testUser.UiPassword = "password";

            // Assert
            Assert.AreEqual("Matt", testUser.GetUser());
        }

        [TestMethod]
        public void UpdateLoginStatus()
        {
            // Arrange
            VMUsers testUsers = GetVMUsers();

            // Act
            testUsers.UpdateLoginStatus(null);
            // Assert
            Assert.AreEqual((int)VMUsers.loginTypes.loggedOut, testUsers.LoginStatus);

            // Act
            testUsers.UpdateLoginStatus("!BadLogin");
            // Assert
            Assert.AreEqual((int)VMUsers.loginTypes.badLogin, testUsers.LoginStatus);

            // Act
            testUsers.UpdateLoginStatus("1");
            // Assert
            Assert.AreEqual((int)VMUsers.loginTypes.loggedIn, testUsers.LoginStatus);
        }

        [TestMethod]
        public void UpdateUser()
        {
            // Arrange
            VMUsers testUsers = GetVMUsers();

            // Act
            testUsers.LoginStatus = (int)VMUsers.loginTypes.loggedOut;
            testUsers.UpdateUser();
            // Assert
            Assert.AreEqual(string.Empty, testUsers.CurrentUser);

            // Act
            testUsers.LoginStatus = (int)VMUsers.loginTypes.loggedIn;
            testUsers.UpdateUser();
            // Assert
            Assert.AreEqual("Matt", testUsers.CurrentUser);

            // Act
            testUsers.LoginStatus = (int)VMUsers.loginTypes.badLogin;
            testUsers.UpdateUser();
            // Assert
            Assert.AreEqual("!BadLogin", testUsers.CurrentUser);
        }


        // Helper methods. Change signature in one place instead of many.
        private VMUsers GetVMUsers() { return new VMUsers("Test"); }
    }
}
