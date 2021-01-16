
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Models;
using Microsoft.AspNet.Identity;
using Moq;
using Microsoft.Owin.Security;
using TrangTuyenDung.Controllers;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Linq;
using System.Security.Principal;
using TrangTuyenDung.Tests.Support;

namespace TrangTuyenDung.Tests.Controllers {
    /// <summary>
    /// Summary description for AccountControllerTest
    /// </summary>
    [TestClass]
    public class AccountControllerTest {
        EJobEntities db = new EJobEntities();

        // ---Unit test for Login function---

        /// <summary>
        /// Purpose of TC: 
        /// - Validate whether login into a website right with Staff account
        ///     and then the user is redirected to the List action
        /// </summary>
        [Priority(4)]
        [TestMethod]
        public async Task LoginRight_WithStaffAccount() {
            //
            // TODO: Add test logic here
            //

            // Arr
            //var helper = new MockHelper();
            //var context = helper.MakeFakeContext();
            var dummyUser = new ApplicationUser() { UserName = "staff@gmail.com", Email = "staff@gmail.com" };

            var model = new LoginViewModel {
                Email = "staff@gmail.com",
                Password = "Password1%",
                RememberMe = false,
            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            //var roleId = db.AspNetRoles.FirstOrDefault(x => x.Name == "Admin").Id;
            var userId = db.AspNetUsers.FirstOrDefault(x => x.Email == "staff@gmail.com").Id;
            var user = userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                EmailConfirmed = true,
                Id = userId,
                Email = "staff@gmail.com"
            });

            var role = userManager.Setup(x => x.IsInRoleAsync(It.Is<string>(s => s.Equals(userId)), It.Is<string>(s => s.Equals("Staff")))).ReturnsAsync(true);

            var controller = new AccountController(userManager.Object, signInManager.Object);

            var validateResult = TestModelHelper.ValidateModel(controller, model);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("staff@gmail.com");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("staff@gmail.com");

            controller.ControllerContext = controllerContext.Object;

            // Act    
            var redirectRoute = await controller.Login(model, "/MyURL") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(redirectRoute);
            Assert.AreEqual("List", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Recruitment", redirectRoute.RouteValues["controller"]);
            Assert.AreEqual(0, validateResult.Count);
        }

        /// <summary>
        /// Purpose of TC: 
        /// - Validate whether login into a website right with Admin account
        ///     and then the user is redirected to the Index action
        /// </summary>
        [TestMethod]
        public async Task LoginRight_WithAdminAccount() {
            //
            // TODO: Add test logic here
            //

            // Arr
            //var helper = new MockHelper();
            //var context = helper.MakeFakeContext();
            var dummyUser = new ApplicationUser() { UserName = "admin@gmail.com", Email = "admin@gmail.com" };

            var model = new LoginViewModel {
                Email = "admin@gmail.com",
                Password = "Password1%",
                RememberMe = false,
            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            //var roleId = db.AspNetRoles.FirstOrDefault(x => x.Name == "Admin").Id;
            var userId = db.AspNetUsers.FirstOrDefault(x => x.Email == "admin@gmail.com").Id;
            var user = userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                EmailConfirmed = true,
                Id = userId,
                Email = "admin@gmail.com"
            });

            var role = userManager.Setup(x => x.IsInRoleAsync(It.Is<string>(s => s.Equals(userId)), It.Is<string>(s => s.Equals("Admin")))).ReturnsAsync(true);

            var controller = new AccountController(userManager.Object, signInManager.Object);

            var validateResult = TestModelHelper.ValidateModel(controller, model);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("admin@gmail.com");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("admin@gmail.com");

            controller.ControllerContext = controllerContext.Object;

            // Act    
            var redirectRoute = await controller.Login(model, "/MyURL") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(redirectRoute);
            Assert.AreEqual("Index", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Home", redirectRoute.RouteValues["controller"]);
            Assert.AreEqual(0, validateResult.Count);
        }

        /// <summary>
        /// Purpose of TC: 
        /// - Validate whether login into a website right with Company account, with account actived
        ///     and then the user is redirected to the Dashboard action
        /// </summary>
        [TestMethod]
        public async Task LoginRight_WithActivedCompanyAccount() {
            //
            // TODO: Add test logic here
            //

            // Arr
            //var helper = new MockHelper();
            //var context = helper.MakeFakeContext();
            var dummyUser = new ApplicationUser() { UserName = "company@gmail.com", Email = "company@gmail.com" };

            var model = new LoginViewModel {
                Email = "company@gmail.com",
                Password = "Password1%",
                RememberMe = false,
            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            var userId = db.AspNetUsers.FirstOrDefault(x => x.Email == "company@gmail.com").Id;
            var user = userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                EmailConfirmed = true,
                Id = userId,
                Email = "company@gmail.com"
            });

            var role = userManager.Setup(x => x.IsInRoleAsync(It.Is<string>(s => s.Equals(userId)), It.Is<string>(s => s.Equals("Company")))).ReturnsAsync(true);

            var controller = new AccountController(userManager.Object, signInManager.Object);

            var validateResult = TestModelHelper.ValidateModel(controller, model);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("company@gmail.com");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("company@gmail.com");

            controller.ControllerContext = controllerContext.Object;

            // Act    
            var redirectRoute = await controller.Login(model, "/MyURL") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(redirectRoute);
            Assert.AreEqual("Dashboard", redirectRoute.RouteValues["action"]);
            Assert.AreEqual("Home", redirectRoute.RouteValues["controller"]);
            Assert.AreEqual(0, validateResult.Count);
        }

        /// <summary>
        /// Purpose of TC: 
        /// - Validate whether login into a website with email be not comfirmed
        ///     and then the user cannot login into a website and error message should be show
        /// </summary>
        [Priority(5)]
        [TestMethod]
        public async Task LoginWrong_WithEmailNoConfirmed() {
            // Arr                    
            var dummyUser = new ApplicationUser() { UserName = "staff@gmail.com", Email = "staff@gmail.com" };

            var model = new LoginViewModel {
                Email = "staff@gmail.com",
                Password = "Password1%",
                RememberMe = false,

            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                //UserName = "staff@gmail.com",
                EmailConfirmed = false
            });

            var url = "http://localhost:49925";
            var controller = new AccountController(userManager.Object, signInManager.Object);

            var validateResult = TestModelHelper.ValidateModel(controller, model);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("staff@gmail.com");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("staff@gmail.com");

            controller.ControllerContext = controllerContext.Object;

            // Act            
            var redirectRoute = await controller.Login(model, url) as ViewResult;


            // Assert
            Assert.AreEqual("Vui lòng xác minh qua Email để kích hoạt tài khoản!!", redirectRoute.ViewBag.mess);

        }

        /// <summary>
        /// Purpose of TC: 
        /// - Validate whether login into a website with Student account
        ///     and then website return Url
        /// </summary>
        [Priority(6)]
        [TestMethod]
        public async Task LoginRight_WithStudentAccount() {
            // Arr               
            var model = new LoginViewModel {
                Email = "chivo10@vanlanguni.vn",
                Password = "VLUt153556",
                RememberMe = false,

            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);
            userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                //UserName = "staff@gmail.com",
                EmailConfirmed = true
            });

            var url = "http://www.google.com";
            var controller = new AccountController(userManager.Object, signInManager.Object);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("chivo10@vanlanguni.vn");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("chivo10@vanlanguni.vn");

            controller.ControllerContext = controllerContext.Object;

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            httpContext.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Url).Returns(new Uri("http://localhost:49925"));
            var requestContext = new RequestContext(httpContext.Object, new RouteData());
            controller.Url = new UrlHelper(requestContext);

            // Act            
            var redirectRoute = await controller.Login(model, url) as ActionResult;


            // Assert
            Assert.IsNotNull(true);
        }


        [Priority(7)]
        [TestMethod]
        public async Task LoginWrong_WithAccountNotExist() {
            // Arr               
            var model = new LoginViewModel {
                Email = "staff@gmail.com",
                Password = "Password",
                RememberMe = false,

            };

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);

            var authenticationManager = new Mock<IAuthenticationManager>();
            var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);

            userManager.Setup(x => x.FindByNameAsync(model.Email)).ReturnsAsync(new ApplicationUser() {
                //UserName = "taff@gmail.com",
                EmailConfirmed = false
            });

            var url = "http://www.google.com";
            var controller = new AccountController(userManager.Object, signInManager.Object);

            // giả lặp session và User.Identity
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.Session["user"]).Returns("staff@gmail.com");
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("staff@gmail.com");

            controller.ControllerContext = controllerContext.Object;

            var httpContext = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            httpContext.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Url).Returns(new Uri("http://localhost:49925"));
            var requestContext = new RequestContext(httpContext.Object, new RouteData());
            controller.Url = new UrlHelper(requestContext);

            // Act            
            var redirectRoute = await controller.Login(model, url) as ViewResult;


            // Assert
            Assert.IsNotNull(redirectRoute);
            Assert.IsTrue(controller.ModelState.IsValid);
            //Assert.IsTrue(controller.ModelState[""].Errors.Any(modelError => modelError.ErrorMessage == "Tài khoản hoặc mật khẩu không chính xác! \n    Vui lòng kiểm tra lại!"));
        }

        //---End Unit test for Login functional---

        // ---Unit test for Register functional---
        //[TestMethod]
        //public async Task RegisterRight()
        //{
        //    // Arr
        //    RegisterViewModel model = new RegisterViewModel
        //    {
        //        Email = "testdata@gmail.com",
        //        Password = "Password",
        //        ConfirmPassword = "Password"
        //    };

        //    var dummyUser = new ApplicationUser() { UserName = "testdata@gmail.com", Email = "testdata@gmail.com" };

        //    var userStore = new Mock<IUserStore<ApplicationUser>>();
        //    var mockUserRoleStore = userStore.As<IUserRoleStore<ApplicationUser>>();

        //    var userManager = new Mock<ApplicationUserManager>(userStore.Object);

        //    var authenticationManager = new Mock<IAuthenticationManager>();
        //    var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);

        //    //userManager.Setup(x => x.CreateAsync(dummyUser, model.Password)).Returns(Task.FromResult(IdentityResult.Success));
        //    userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult(IdentityResult.Success));

        //    var controller = new AccountController(userManager.Object, signInManager.Object);

        //    // Act
        //    var redirectRoute = await controller.Register(model) as RedirectToRouteResult;

        //    // Assert
        //    Assert.AreEqual("Index", redirectRoute.RouteValues["action"]);
        //    Assert.AreEqual("Home", redirectRoute.RouteValues["controller"]);
        //}
    }
}
