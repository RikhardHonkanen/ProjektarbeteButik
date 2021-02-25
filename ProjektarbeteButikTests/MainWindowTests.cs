using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjektarbeteButik;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjektarbeteButik.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void CheckIfCodeIsValidTest()
        {
            var codes = new Dictionary<string, decimal>()
            {
                { "abc123", 10 },
                { "def456", 20 },
                { "test", 50 },

            };
            string input = "abc123";
            //bool result = MainWindow.CheckIfCodeIsValid(codes, input);
            Assert.Fail();
        }        

        [TestMethod()]
        public void LoadCartTest()
        {
            Assert.Fail();
        }
    }
}