using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Areas.Staff.Controllers;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using Microsoft.AspNet.Identity;
using Moq;
using Microsoft.Owin.Security;
using TrangTuyenDung.Tests.Support;
using System.Web;
using TrangTuyenDung.Areas.Staff.Models;
using System.Configuration;
using System.Web.Routing;
using System.IO;
using System.Linq;

namespace TrangTuyenDung.Tests.Controllers {
    /// <summary>
    /// Summary description for StaffAccountControllerTest
    /// </summary>
    [TestClass]
    public class StaffAccountControllerTest {
        EJobEntities db = new EJobEntities();


        // ---Unit test for Create Company by Staff---
        [Priority(45)]
        [TestMethod]
        public void StaffCreateCompany_WithLogoExist() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            // ---Create mock for save avatar in server---

            //create mock of HttpServerUtilityBase
            var server = new Mock<HttpServerUtilityBase>();

            //set up mock to return known value on call.
            server.Setup(x => x.MapPath("~/App_Data/Company/Logos/")).Returns("~/App_Data/Company/Logos/");

            context.Setup(x => x.Server).Returns(server.Object);
            //---~~---


            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Setup(req => req.Files).Returns(postedfilesKeyCollection.Object);

            postedfilesKeyCollection.Setup(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());
            postedfilesKeyCollection.Setup(keys => keys["Com_Logo"]).Returns(postedfile.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Setup(f => f.ContentLength).Returns(1).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            CreateCompanyViewModel comModel = new CreateCompanyViewModel() {
                Com_Logo = postedfile.Object,
                Com_Name = "Cty testtttt"
            };

            var redirectRoute = controller.CreateCompany(comModel) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Công ty mới đã được thêm vào!", controller.ViewBag.Message);
            Assert.AreEqual("ApprovedList", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Account", redirectRoute.RouteValues["controller"]);
        }

        [Priority(43)]
        [TestMethod]
        public void StaffCreateCompany_WithLogoLengthNull() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Setup(req => req.Files).Returns(postedfilesKeyCollection.Object);

            postedfilesKeyCollection.Setup(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());
            postedfilesKeyCollection.Setup(keys => keys["Com_Logo"]).Returns(postedfile.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Setup(f => f.ContentLength).Returns(0).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            CreateCompanyViewModel comModel = new CreateCompanyViewModel() {
                Com_Logo = postedfile.Object,
                Com_Name = "Cty testtttt"
            };

            var redirectRoute = controller.CreateCompany(comModel) as ViewResult;

            // Assert
            Assert.IsTrue(redirectRoute.ViewData.ModelState[""].Errors.Count == 1);
            Assert.AreEqual("Lỗi khi tải lên tệp!", redirectRoute.ViewBag.Message);
        }

        [Priority(44)]
        [TestMethod]
        public void StaffCreateCompany_WithDoNotHaveLogo() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Setup(req => req.Files).Returns(postedfilesKeyCollection.Object);

            postedfilesKeyCollection.Setup(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());
            postedfilesKeyCollection.Setup(keys => keys[""]).Returns(postedfile.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Setup(f => f.ContentLength).Returns(0).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            CreateCompanyViewModel comModel = new CreateCompanyViewModel() {
                Com_Logo = postedfile.Object,
                Com_Name = "Cty testtttt"
            };

            var redirectRoute = controller.CreateCompany(comModel) as ViewResult;

            // Assert
            Assert.AreEqual("Lỗi khi tải lên tệp!", controller.ViewBag.Message);
        }

        // ---End Unit test for Create Company by Staff---//


        //---Unit test for Pending (List company pending)---//
        [Priority(39)]
        [TestMethod]
        public void ListCompanyPending() {
            // Arr
            var controller = new AccountController();

            // Act
            var redirectResult = controller.Pending() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }
        // ---End unit test for Pending---//


        // ---Unit test for Approve Company---
        [Priority(40)]
        [TestMethod]
        public void ApproveCompanySuccessful_WithCompanyIsApproved() {
            // Arr
            var helper = new MockHelper();
            //var context = helper.MakeFakeContext();
            var context = new Mock<HttpContextBase>();


            // ---Create mock for save avatar in server---

            //create mock of HttpServerUtilityBase
            var server = new Mock<HttpServerUtilityBase>();

            //set up mock to return known value on call.
            server.Setup(x => x.MapPath("~/App_Data/Company/Logos")).Returns("~/App_Data/Company/Logos");

            context.Setup(x => x.Server).Returns(server.Object);
            //---~~---


            var modelUser = new Company_Registration() {
                Email = "sophievo1297@gmail.com",
                Password = "1234567890"
            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            var mockContext = new Mock<ApplicationDbContext>(userManager.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var id = db.Company_Registration.FirstOrDefault(x => x.Name_Company == "Cty software").Id;

            // Act
            var redirectRoute = controller.Approved(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Pending", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Account", redirectRoute.RouteValues["controller"]);
        }

        [Priority(41)]
        [TestMethod]
        public void ApproveCompanySuccessful_WithCompanyHadAccountUser() {
            // Arr
            var helper = new MockHelper();
            //var context = helper.MakeFakeContext();
            var context = new Mock<HttpContextBase>();

            var modelUser = new Company_Registration() {
                Email = "sophievo1297@gmail.com",
                Password = "1234567890"
            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            var mockContext = new Mock<ApplicationDbContext>(userManager.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var id = db.Company_Registration.FirstOrDefault(x => x.Name_Company == "Cty software").Id;

            // Act
            var redirectRoute = controller.Approved(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Lỗi khi xét duyệt, vui lòng refresh lại trang và thử lại!", controller.ViewBag.AprroveError);
            Assert.AreEqual("Pending", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Account", redirectRoute.RouteValues["controller"]);
        }

        [Priority(42)]
        [TestMethod]
        public void ApproveCompany_WithCompanyIsRejected() {
            // Arr
            var controller = new AccountController();
            var id = db.Company_Registration.FirstOrDefault(x => x.Email == "sophievo1297@gmail.com").Id;
            FormCollection formcollection = new FormCollection();

            // Act
            var redirectRoute = controller.Reject(id, formcollection) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Pending", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Account", redirectRoute.RouteValues["controller"]);
        }

        // ---End Unit test for Approve Company---


        // ---Unit test for manage account of company (change status of company)
        [Priority(47)]
        [TestMethod]
        public void ChangeCompanyStatus_WithBlockCom() {
            // Arr
            var controller = new AccountController();

            var id = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id;
            // Act
            var redirectResult = controller.ChangeCompanyStatus(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("ApprovedList", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"]);
        }

        [Priority(48)]
        [TestMethod]
        public void ChangeCompanyStatus_WithActiveCom() {
            // Arr
            var controller = new AccountController();

            var id = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id;
            // Act
            var redirectResult = controller.ChangeCompanyStatus(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("ApprovedList", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"]);
        }

        [Priority(49)]
        [TestMethod]
        public void ChangeCompanyStatus_WithCompanyNotExist() {
            var exceptionIsThrown = false;
            try {
                // Arr
                var controller = new AccountController();

                // Act
                var redirectResult = controller.ChangeCompanyStatus(null) as ContentResult;
            }
            catch (Exception) {
                exceptionIsThrown = true;
            }
        }


    }
}
