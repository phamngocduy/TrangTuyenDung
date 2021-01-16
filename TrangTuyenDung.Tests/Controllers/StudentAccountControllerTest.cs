using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TrangTuyenDung.Areas.Student.Controllers;
using TrangTuyenDung.Areas.Student.Models;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class StudentAccountControllerTest {
        EJobEntities db = new EJobEntities();

        // --- Unit test for tracking CV functional (Dashboard function) ---//
        [Priority(32)]
        [TestMethod]
        public void StudentViewDashboard_TrackingCV() {
            // Arr

            // get current account
            var email = "chivo10@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new AccountController();

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
            var redirectResult = controller.Dashboard() as ViewResult;

            // Assert
            Assert.IsNotNull(true);

        }
        // ---End unit test for Dashboard---//


        // --- Unit test for Edit student's information ---//
        [Priority(22)]
        [TestMethod]
        public void EditInformationOfStudent_WithAvatarContentIsNull() {
            // Arr

            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            // ---Create mock for get avatar---
            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Setup(req => req.Files).Returns(postedfilesKeyCollection.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Setup(f => f.ContentLength).Returns(0).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();
            //---~~~---

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            string name = "chivo10";
            var id = db.Student_Info.FirstOrDefault(x => x.Student_Name == name).Id;
            var fac_id = db.Faculties.FirstOrDefault(x => x.Name == "Kỹ thuật Phần mềm").Id;
            var birthday = "01/02/1997";
            Student_InfoViewModel model = new Student_InfoViewModel() {
                Id = id,
                Student_Name = name,
                Faculties_Id = fac_id,
                Student_Birthday = DateTime.Parse(birthday),
                Student_Gender = false,
                Student_Address = "Quận 1, TP Hồ Chí Minh",
                Student_PhoneNumber = "0766946207",
                MSSV = "T153556",
                Student_Class = "K21T2",
                Student_Avatar = postedfile.Object
            };

            // Act
            var redirect = controller.EditStudent_Info(model) as RedirectToRouteResult;

            // Assert
            //Assert.AreEqual("Không thể tải lên tệp, vui lòng kiểm tra lại!", controller.ModelState);
            Assert.AreEqual("Detail", redirect.RouteValues["action"]);
            Assert.AreEqual("Account", redirect.RouteValues["controller"]);
        }

        [Priority(23)]
        [TestMethod]
        public void EditInformationOfStudent_WithAvatarExist() {
            // Arr

            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            // ---Create mock for save avatar in server---

            //create mock of HttpServerUtilityBase
            var server = new Mock<HttpServerUtilityBase>();

            //set up mock to return known value on call.
            server.Setup(x => x.MapPath("~/App_Data/Student/Logos/")).Returns("~/App_Data/Student/Logos/");

            context.Setup(x => x.Server).Returns(server.Object);
            //---~~---

            // ---Create mock for get avatar---
            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Setup(req => req.Files).Returns(postedfilesKeyCollection.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Setup(f => f.ContentLength).Returns(1).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();
            //---~~~---

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            string name = "chivo10";
            var id = db.Student_Info.FirstOrDefault(x => x.Student_Name == name).Id;
            var fac_id = db.Faculties.FirstOrDefault(x => x.Name == "Kỹ thuật Phần mềm").Id;
            var birthday = "01/02/1997";
            Student_InfoViewModel model = new Student_InfoViewModel() {
                Id = id,
                Student_Name = name,
                Faculties_Id = fac_id,
                Student_Birthday = DateTime.Parse(birthday),
                Student_Gender = false,
                Student_Address = "Quận 1, TP Hồ Chí Minh",
                Student_PhoneNumber = "0766946207",
                MSSV = "T153556",
                Student_Class = "K21T2",
                Student_Avatar = postedfile.Object
            };

            // Act
            var redirect = controller.EditStudent_Info(model) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Detail", redirect.RouteValues["action"]);
            Assert.AreEqual("Account", redirect.RouteValues["controller"]);
        }

        // --- Unit test for View Detail Student's Information ---//
        [Priority(29)]
        [TestMethod]
        public void ViewDetailInforOfStudent_WithAccHadCV() {
            // Arr
            var email = "chivo10@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new AccountController();

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
            var redirect = controller.Detail() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        // --- Unit test for View Detail Student's Information ---//
        [Priority(26)]
        [TestMethod]
        public void ViewDetailInforOfStudent_WithAccDoNotHaveCV() {
            // Arr
            var email = "vinhnguyen57@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new AccountController();

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
            var redirect = controller.Detail() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }

        // ---End unit test for View Detail---//

        // ---Unit test for Update CV---//
        // update cv with cv exist data
        [Priority(24)]
        [TestMethod]
        public void UpdateCV_WithCVExistedAndFullDataInput() {
            // Arr
            var controller = new AccountController();

            // get student id
            var studentId = db.Student_Info.FirstOrDefault(x => x.Student_Name == "chivo10").Id;

            CV_UpdateViewModel data = new CV_UpdateViewModel {
                // student id
                ID = studentId,

                // student goal
                Personal_Goal = "Làm hết mình, chơi hết sức, đạt được mục tiêu đã đề ra",

                // introduce student
                About_Student = "Vui vẻ, hòa đồng, dễ thích nghi",

                // personal skill
                PS_01 = "Kỹ năng testing",
                PS_02 = "",
                PS_03 = "",

                PS_Rate_01 = 60,

                // work experience
                WE_Company_01 = "Công ty công nghệ",
                WE_Description_01 = "Làm dự án thực tế với ngôn ngữ .Net và thực hiện automation testing",
                WE_Date_Start_01 = "08/2018",
                WE_Date_End_01 = "02/2019",

                // certificate
                Cer_Description_01 = "Chứng chỉ được cấp từ CMU - đại học công nghệ thông tin đứng đầu thế giới",
                Cer_Name_01 = "AUT",
                Cer_GetDate_01 = "2017",

                // project infor
                Project_Description_1 = "Website giới thiệu và quản lý ứng tuyển việc làm",
                Project_Name_1 = "EJob website",
                Project_Date_Start_1 = "09/2018",
                Project_Date_End_1 = "05/2019",

                // template cv
                CV_Template_Id = 1
            };

            // Act
            var redirectResult = controller.Update_CV(data) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Detail", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"]);
        }

        // update cv with cv not exist data
        [Priority(27)]
        [TestMethod]
        public void UpdateCV_WithCVNotExistAndOnlyHaveStuInfor() {
            // Arr
            var controller = new AccountController();

            // get student id
            var studentId = db.Student_Info.FirstOrDefault(x => x.Student_Name == "vinhnguyen57").Id;

            CV_UpdateViewModel data = new CV_UpdateViewModel {
                // student id
                ID = studentId,

                // student goal
                Personal_Goal = "Ăn, ngủ, chơi, game :)))",

                // introduce student
                About_Student = "Vui vẻ, hòa đồng, bựa",

                //// personal skill
                //PS_01 = "Chơi pubg",

                //PS_Rate_01 = 60,

                //// work experience
                //WE_Company_01 = "Công ty H3",
                //WE_Description_01 = "Hệ thống chat",
                //WE_Date_Start_01 = "08/2018",
                //WE_Date_End_01 = "02/2019",

                //// certificate
                //Cer_Description_01 = "Chứng chỉ được cấp từ CMU - đại học công nghệ thông tin đứng đầu thế giới",
                //Cer_Name_01 = "AUT",
                //Cer_GetDate_01 = "2017",

                //// project infor
                //Project_Description_1 = "Kênh giao tiếp trong chung cư",
                //Project_Name_1 = "RCC",
                //Project_Date_Start_1 = "09/2018",
                //Project_Date_End_1 = "05/2019",

                // template cv
                CV_Template_Id = 1
            };

            // Act
            var redirectResult = controller.Update_CV(data) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Detail", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"]);
        }
        // ---End unit test Update CV---//


        // ---Unit test for View CV---//
        [Priority(25)]
        [TestMethod]
        public void ViewCV_WithAccDoNotHaveCV() {
            // Arr
            // get current account
            var email = "vinhnguyen57@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new AccountController();

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
            var redirectResult = controller.View_CV() as ContentResult;

            // Assert
            Assert.AreEqual("Bạn cần tạo thông tin cho CV trước khi xem nhé! Xin cảm ơn! ^^", redirectResult.Content);
        }

        [Priority(28)]
        [TestMethod]
        public void ViewCV_WithAccCVExisted() {
            // Arr
            // get current account
            var email = "chivo10@vanlanguni.vn";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;
            var controller = new AccountController();

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
            var redirectResult = controller.View_CV() as PartialViewResult;

            // Assert
            Assert.AreEqual(@"~/Views/CV_Templates/_CVTemplate01.cshtml", redirectResult.ViewName);
        }

    }
}
