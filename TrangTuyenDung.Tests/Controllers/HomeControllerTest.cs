using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Controllers;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class HomeControllerTest {
        [Priority(1)]
        [TestMethod]
        public void Index() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Priority(2)]
        [TestMethod]
        public void About() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [Priority(3)]
        [TestMethod]
        public void Contact() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        //[TestMethod]
        //public void ShowListAllCom()
        //{
        //    // Arr
        //    var controller = new HomeController();

        //    // Act
        //    var result = controller.ShowListAllCompany() as PartialViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("_partialCompany", result.ViewName);
        //}

        //[TestMethod]
        //public void ShowListMajorCompany()
        //{
        //    // Arr
        //    var controller = new HomeController();

        //    // Act
        //    var result = controller.ShowListMajorCompany() as PartialViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("_partialCompany", result.ViewName);
        //}
    }
}
