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
            //var products = new List<Product>() { new Product { Name = "Thing1", Description = "Testthing1", Price = 100, PicturePath = "" } };
            //string path = "TestCart.csv";
            //var result = MainWindow.LoadCart(products, path);
            //bool s = result.Name;
            //Assert.AreEqual(result.ContainsKey(Thing1), "Thing1");
        }
    }
}
