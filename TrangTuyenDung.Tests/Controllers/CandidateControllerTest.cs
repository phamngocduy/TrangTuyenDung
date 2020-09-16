using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Areas.Company.Controllers;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using Moq;
using System.Web;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Collections.Generic;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class CandidateControllerTest {
        EJobEntities db = new EJobEntities();

        [Priority(33)]
        [TestMethod]
        public void Pending() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            var email = "Company@gmail.com";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;

            var controller = new CandidateController();

            var dataid = new ApplicationUser {
                Id = id,
                Email = email,
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
            var redirect = controller.Pending() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        [Priority(34)]
        [TestMethod]
        public void Interviewing() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            var email = "Company@gmail.com";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;

            var controller = new CandidateController();

            var dataid = new ApplicationUser {
                Id = id,
                Email = email,
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
            var redirect = controller.Interviewing() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }
    }
}
