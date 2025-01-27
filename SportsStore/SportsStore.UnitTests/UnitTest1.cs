﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static ProductController productController;
        private static NavController navController;
        private void InitTest()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] 
                {
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                });
            productController = new ProductController(mock.Object);
            navController = new NavController(mock.Object);
        }

        //[ClassInitialize]
        //public void InitAllTest()
        //{
        //    InitTest();
        //}

        [TestMethod]
        public void Can_Paginate()
        {
            InitTest();
            // Arrange
            productController.PageSize = 3;
            // Act
            ProductsListViewModel result = (ProductsListViewModel)productController.List(null, 2).Model;
            // Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }



        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;
            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;
            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            // Assert
            Assert.AreEqual
                (
                @"<a class=""btn btn-default"" href=""Page1"">1</a>" +
                @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" +
                @"<a class=""btn btn-default"" href=""Page3"">3</a>",
            result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            InitTest();
            // Arrange
            productController.PageSize = 3;
            // Act
            ProductsListViewModel result = (ProductsListViewModel)productController.List(null, 2).Model;
            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            InitTest();
            // Arrange - create a controller and make the page size 3 items
            productController.PageSize = 3;
            // Action
            Product[] result = ((ProductsListViewModel)productController.List("Cat2", 1).Model).Products.ToArray();
            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Act = get the set of categories
            string[] results = ((IEnumerable<string>)navController.Menu().Model).ToArray();
            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Cat1");
            Assert.AreEqual(results[1], "Cat2");
            Assert.AreEqual(results[2], "Cat3");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            InitTest();
            // Arrange - define the category to selected
            string categoryToSelect = "Cat1";
            // Action
            string result = navController.Menu(categoryToSelect).ViewBag.SelectedCategory;
            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            InitTest();
            productController.PageSize = 3;
            // Action - test the product counts for different categories
            int res1 = ((ProductsListViewModel)productController
                .List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)productController
                .List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)productController
                .List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)productController
                .List(null).Model).PagingInfo.TotalItems;
            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
