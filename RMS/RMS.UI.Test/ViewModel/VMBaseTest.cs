using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RMS.UI.ViewModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RMS.UI.Test
{
    [TestClass]
    public class VMBaseTest
    {
        [TestMethod]
        public void makeBase()
        {
            // Arrange
            VMBase testBase = getBase();
            // Act
            // Assert
            Assert.AreNotEqual(null, testBase.DisplayName);
            // Arrange
            // Act
            testBase.DisplayName = "TestWindow";
            // Assert
            Assert.AreEqual("TestWindow", testBase.DisplayName);
        }
        [TestMethod]
        public void RaisePropertyChangedTest()
        {
            // Arrange
            VMBase testBase = getBase();
            VMBase secondBase = getBase();
            // Act
            testBase = null;
            // Assert
            Assert.AreEqual(null, testBase);
            Assert.AreEqual("Test", secondBase.DisplayName);

            // Arrange
        }
        private VMBase getBase() { return new VMBase("Test"); }
        /*[TestClass]
        public class RelayCommandTest
        {
            readonly Action executeNoVar;
            readonly Action<object> execute;
            readonly Predicate<object> canExecute;
            public delegate int testNums(int x, int y);
            

            [TestMethod]
            public void makeCommandTest()
            {
                // Arrange
                VMBase.RelayCommand testRelay1 = new VMBase.RelayCommand(executeNoVar);
                VMBase.RelayCommand testRelay2 = new VMBase.RelayCommand(executeNoVar, canExecute);
                VMBase.RelayCommand testRelay3 = new VMBase.RelayCommand(execute);
                VMBase.RelayCommand testRelay4 = new VMBase.RelayCommand(execute, canExecute);

                //Act
                //Assert
                //Assert.AreEqual(null, testRelay1);
                //Assert.AreEqual(null, testRelay2);
                //Assert.AreEqual(null, testRelay3);
                //Assert.AreEqual(null, testRelay4);
            }

            public int testNums(int x, int y)
            {
                int a = x;
                int b = y;
                return a + b;
            }
        }*/
    }
}
