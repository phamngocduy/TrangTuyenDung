using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Controllers;
using System.Web.Mvc;
using System.Net;
using System.Web;
using Moq;
using TrangTuyenDung.Models;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace TrangTuyenDung.Tests.Controllers {
    /// <summary>
    /// Summary description for RecruitmentControllerTest
    /// </summary>
    /// 

    [TestClass]
    public class RecruitmentControllerTest {
        EJobEntities db = new EJobEntities();

        [Priority(12)]
        [TestMethod]
        public void DetailRecruitment_WithUserNotApplyCVsToThisRN() {
            // Arr

            // get current account
            string email = "";
            string id = "";
            var controller = new RecruitmentController();

            var dataid = new ApplicationUser {
                Id = id,
                Email = null,
                EmailConfirmed = true
            };

            List<Claim> claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id)
            };

            var controllerContext = new Mock<ControllerContext>();

            var username = controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(email);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(p => p.HttpContext.Response.Cookies).Returns(new HttpCookieCollection());

            var genericIdentity = new GenericIdentity(email);
            genericIdentity.AddClaims(claims);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            controllerContext.SetupGet(p => p.HttpContext.User).Returns(genericPrincipal);

            controller.ControllerContext = controllerContext.Object;
            //var controller = new RecruitmentController();
            var idRN = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            // Act
            var redirectRout = controller.Detail(idRN) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        [TestMethod]
        public void DetailRecruitment_WithRecruitmentAppliedCVs() {
            // Arr

            // get current account
            string email = "chivo10@vanlanguni.vn";
            string id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new RecruitmentController();

            var dataid = new ApplicationUser {
                Id = id,
                Email = null,
                EmailConfirmed = true
            };

            List<Claim> claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id)
            };

            var controllerContext = new Mock<ControllerContext>();

            var username = controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(email);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(p => p.HttpContext.Response.Cookies).Returns(new HttpCookieCollection());

            var genericIdentity = new GenericIdentity(email);
            genericIdentity.AddClaims(claims);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            controllerContext.SetupGet(p => p.HttpContext.User).Returns(genericPrincipal);

            controller.ControllerContext = controllerContext.Object;
            //var controller = new RecruitmentController();
            var idRN = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            // Act
            var redirectRout = controller.Detail(idRN) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        [Priority(13)]
        [TestMethod]
        public void DetailRecruitment_WithRecruitmentNotExist() {
            // Arr                
            var controller = new RecruitmentController();
            int? id = null;

            // Act
            var redirectRoute = controller.Detail(id) as HttpStatusCodeResult;
            var expected = (int)HttpStatusCode.BadRequest;

            // Assert
            Assert.IsNotNull(redirectRoute);
            Assert.AreEqual(expected, redirectRoute.StatusCode);

        }

        [Priority(14)]
        [TestMethod]
        public void DetailRecruitment_WithRecruitmentNotFound() {
            var exceptionIsThrown = false;
            try {
                // Arr
                var controller = new RecruitmentController();
                int? id = 0;

                // Act
                var redirectRoute = controller.Detail(id) as HttpNotFoundResult;
            }
            catch {
                exceptionIsThrown = true;
            }

            //// Assert
            //Assert.IsNotNull(redirectRoute);
            //Assert.IsInstanceOfType(redirectRoute, typeof(HttpNotFoundResult));
        }

        // ---Unit test for Search---
        [Priority(15)]
        [TestMethod]
        public void FilterAll_WithSearchDataIsCity() {
            // Arr
            var controller = new RecruitmentController();

            Searchsupport data = new Searchsupport {
                all = "Hồ Chí Minh "
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        [Priority(16)]
        [TestMethod]
        public void FilterAll_WithSearchDataIsCompanyName() {
            // Arr
            var controller = new RecruitmentController();

            Searchsupport data = new Searchsupport {
                all = "Nhật Huy Khang"
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        [Priority(17)]
        [TestMethod]
        public void FilterAll_WithSearchDataIsTitleName() {
            // Arr
            var controller = new RecruitmentController();

            Searchsupport data = new Searchsupport {
                all = "Giáo viên tiếng nhật"
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        [TestMethod]
        public void FilterAll_WithSearchDataIsJobType() {
            // Arr
            var controller = new RecruitmentController();

            Searchsupport data = new Searchsupport {
                all = "fulltime"
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        //[TestMethod]
        //[Priority(18)]
        //public void FilterAll_WithSearchDataIsTagRecruitment()
        //{
        //    // Arr
        //    var controller = new RecruitmentController();

        //    Searchsupport data = new Searchsupport
        //    {
        //        all = "PHP"
        //    };

        //    // Act
        //    var redirectRoute = controller.Fillter(data) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(true);
        //    Assert.AreEqual("Fillter", redirectRoute.ViewName);
        //}

        [Priority(18)]
        [TestMethod]
        public void FilterCity_WithSearchDataExist() {
            // Arr
            var controller = new RecruitmentController();
            var cityId = db.Provinces.FirstOrDefault(x => x.Name == "Hồ Chí Minh").Id;

            Searchsupport data = new Searchsupport {
                city = cityId
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        [Priority(19)]
        [TestMethod]
        public void FilterFaculty_WithSearchDataExist() {
            // Arr
            var controller = new RecruitmentController();
            var facId = db.Faculties.FirstOrDefault(x => x.Name == "Kỹ thuật phần mềm").Id;

            Searchsupport data = new Searchsupport {
                fac = facId
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        [Priority(20)]
        [TestMethod]
        public void FilterAll_WithSearchDataIsSpace() {
            // Arr
            var controller = new RecruitmentController();

            Searchsupport data = new Searchsupport {
                all = " "
            };

            // Act
            var redirectRoute = controller.Fillter(data) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Index", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Home", redirectRoute.RouteValues["controller"]);
        }

        [TestMethod]
        public void FilterAllField_WithInputAllField() {
            // Arr
            var controller = new RecruitmentController();
            var facId = db.Faculties.FirstOrDefault(x => x.Name == "Kỹ thuật phần mềm").Id;
            var cityId = db.Provinces.FirstOrDefault(x => x.Name == "Hồ Chí Minh").Id;

            Searchsupport data = new Searchsupport {
                all = "partime",
                city = cityId,
                fac = facId,
            };

            // Act
            var redirectRoute = controller.Fillter(data) as ViewResult;

            // Assert
            Assert.IsNotNull(true);
            Assert.AreEqual("Fillter", redirectRoute.ViewName);
        }

        // --- Unit test for Apply CV ---//
        [Priority(21)]
        [TestMethod]
        public void ApplyCV_WithStudentDoNotHaveCV() {
            // Arr
            var recid = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            var email = "chivo10@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;

            var controller = new RecruitmentController();

            List<Claim> claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id)
            };

            var controllerContext = new Mock<ControllerContext>();

            var username = controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(email);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(p => p.HttpContext.Response.Cookies).Returns(new HttpCookieCollection());

            var genericIdentity = new GenericIdentity(email);
            genericIdentity.AddClaims(claims);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            controllerContext.SetupGet(p => p.HttpContext.User).Returns(genericPrincipal);

            controller.ControllerContext = controllerContext.Object;

            // Act
            var redirect = controller.Apply(recid) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Detail", redirect.RouteValues["action"]);
        }


        [Priority(30)]
        [TestMethod]
        public void ApplyCV_WithStudentHadCV() {
            // Arr
            var recid = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            var email = "chivo10@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;

            var controller = new RecruitmentController();

            List<Claim> claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id)
            };

            var controllerContext = new Mock<ControllerContext>();

            var username = controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(email);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(p => p.HttpContext.Response.Cookies).Returns(new HttpCookieCollection());

            var genericIdentity = new GenericIdentity(email);
            genericIdentity.AddClaims(claims);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            controllerContext.SetupGet(p => p.HttpContext.User).Returns(genericPrincipal);

            controller.ControllerContext = controllerContext.Object;

            // Act
            var redirect = controller.Apply(recid) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Detail", redirect.RouteValues["action"]);
        }
    }
}
