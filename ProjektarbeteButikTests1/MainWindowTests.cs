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
        public void DiscountCodeValid()
        {
            var codes = new Dictionary<string, decimal>()
            {
                { "abc123", 10 },
                { "def456", 20 },
                { "test", 50 }
            };
            string input = "abc123";
            bool result = MainWindow.CheckIfCodeIsValid(codes, input);
            Assert.AreEqual(result, true);
        }

        [TestMethod()]
        public void DiscountCodeInvalid()
        {
            var codes = new Dictionary<string, decimal>()
            {
                { "abc123", 10 },
                { "def456", 20 },
                { "test", 50 }
            };
            string input = "";
            bool result = MainWindow.CheckIfCodeIsValid(codes, input);
            Assert.AreEqual(result, false);
        }

        [TestMethod()]
        public void LoadCart()
        {
            Dictionary<string, int> discountCodes = new Dictionary<string, int>();
           
            
            string path = "TestCart.csv";
            var result = MainWindow.LoadDiscountCodes(discountCodes, path);
            Assert.AreEqual(produkt.Name = "Thing1", result);
        }
    }
}
