using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Areas.Student.Controllers;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using System.Web;
using System.Security.Principal;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class StudentRecruitmentControllerTest {
        EJobEntities db = new EJobEntities();

        [Priority(31)]
        [TestMethod]
        public void ListOfRecruitment_WithViewSuccess() {
            // Arr
            var controller = new RecruitmentController();

            // Act
            var redirectResult = controller.ListOfRecruitment() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        [TestMethod]
        public void Bookmark_WithRecruitmentNewsNoBookmark() {
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

            int recId = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            // Act
            var result = controller.Bookmark(recId);

            // Assert
            Assert.AreEqual("BMsuccess", result);

        }

        [TestMethod]
        public void Bookmark_WithRecruitmentNewsBookmarked() {
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

            int recId = db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data").Id;

            // Act
            var result = controller.Bookmark(recId);

            // Assert
            Assert.AreEqual("UBMsuccess", result);

        }

        //---Error--
        [TestMethod]
        public void Bookmark_WithRecruitmentNewsNoExists() {
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

            //int recId = db.Recruitments.FirstOrDefault(x => x.title == "Test Error").Id;

            // Act
            var result = controller.Bookmark(-1);

            // Assert
            Assert.AreEqual("error", result);

        }


        [TestMethod]
        public void Bookmark_ViewListRecruitmentBookmarked() {
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

            // Act
            var result = controller.Bookmarked() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
