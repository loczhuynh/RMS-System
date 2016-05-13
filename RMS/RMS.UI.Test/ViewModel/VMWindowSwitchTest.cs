using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMS.UI.ViewModel;

namespace RMS.UI.Test
{
    [TestClass]
    public class VMWindowSwitchTest
    {
        [TestMethod]
        public void OpenUISelectTest()
        {
            // Arrange
            VMWindowSwitch testWindow = GetVMWindowSwitch();

            // Act
            // Assert
            Assert.AreEqual(true, testWindow.OpenUISelect());

        }

        [TestMethod]
        public void OpenCustomerExecuteTest()
        {
            // HOW DO YOU TEST THIS??
        }

        private VMWindowSwitch GetVMWindowSwitch() { return new VMWindowSwitch("Test"); }
    }
}
