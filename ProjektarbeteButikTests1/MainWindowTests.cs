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
        public void CartToDictionaryKey()
        {
            var products = new List<Product>() { new Product { Name = "Thing1", Description = "Testthing1", Price = 100, PicturePath = "" } };
            string path = "TestCart.csv";
            var result = MainWindow.LoadCart(products, path);
            foreach (var p in result)
            {
                Assert.AreEqual(p.Key.Name, "Thing1");
            }
        }

        [TestMethod()]
        public void CartToDictionaryValue()
        {
            var products = new List<Product>() { new Product { Name = "Thing1", Description = "Testthing1", Price = 100, PicturePath = "" } };
            string path = "TestCart.csv";
            var result = MainWindow.LoadCart(products, path);
            foreach (var p in result)
            {
                Assert.AreEqual(p.Value, 5);
            }
        }

        [TestMethod()]
        public void CalculateTotalZero()
        {
            var result = MainWindow.CalculateTotalCost((decimal)0.9, 0);
            Assert.AreEqual(result, 0);
        }

        [TestMethod()]
        public void CalculateTotalNonZero()
        {
            var result = MainWindow.CalculateTotalCost((decimal)0.9, 180);
            Assert.AreEqual(result, 162);
        }
    }
}
