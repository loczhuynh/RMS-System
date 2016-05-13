using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMS.UI.ViewModel;


namespace RMS.UI.Test
{
    [TestClass]
    public class VMDynamicTest
    {
        [TestMethod]
        public void makeDynamic()
        {
            // Arrange
            VMDynamic testDynamic = getDynamic();

            // Act1
            testDynamic.Window = 2;
            // Assert
            Assert.AreEqual(2, testDynamic.Window);
        }

        [TestMethod]
        public void SwitchMessageExecute()
        {
            // Arrange
            VMDynamic testDynamic = getDynamic();

            // Act
            // Assert
            Assert.AreNotEqual(0, testDynamic.Window);

        }

        private VMDynamic getDynamic() { return new VMDynamic("Test"); }
    }
}
