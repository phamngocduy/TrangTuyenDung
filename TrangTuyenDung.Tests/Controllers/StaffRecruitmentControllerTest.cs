using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Areas.Staff.Controllers;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using Moq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Principal;
using System.Web;
using System.Security.Claims;
using System.Web.Routing;
using TrangTuyenDung.Tests.Support;
using System.Linq;

namespace TrangTuyenDung.Tests.Controllers {
    /// <summary>
    /// Summary description for StaffRecruitmentControllerTest
    /// </summary>
    [TestClass]
    public class StaffRecruitmentControllerTest {
        EJobEntities db = new EJobEntities();

        // ---Unit test for Approve Recruitment functional---
        [Priority(9)]
        [TestMethod]
        public void ApproveRecruitment_WithSuccessful() {
            // Arr
            var controller = new RecruitmentController();
            var id = db.Recruitments.FirstOrDefault(x => x.title == "Công việc thử nghiệm").Id;

            // Act
            var result = controller.Approved(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Approve", result.RouteValues["action"]);
            Assert.AreEqual("Recruitment", result.RouteValues["controller"]);
        }

        // ---End unit test for Approve Recruitment functional---

        // ---Unit test for Show-hide functional---
        [Priority(10)]
        [TestMethod]
        public void Showhide_WithSuccessful() {
            // Arr
            var controller = new RecruitmentController();
            int id = 85;

            // Act
            var result = controller.Show_Hide(id) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("List", result.RouteValues["action"]);
            Assert.AreEqual("Recruitment", result.RouteValues["controller"]);
        }

        // ---End unit test for Show-hide functional---

        // ---Unit test for Post Recruitment functional---
        [Priority(11)]
        [TestMethod]
        public void PostNewRecruitment_WithSuccessfull() {
            // Arr

            var model = new LoginViewModel {
                Email = "staff@gmail.com",
                Password = "Password1%",
                RememberMe = false

            };

            var dataid = new ApplicationUser {
                Id = "d2095630-175b-410c-be39-e955bbab393c",
                Email = "staff@gmail.com",
                EmailConfirmed = true
            };

            List<Claim> claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", dataid.Email),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", dataid.Id)
            };


            var controller = new RecruitmentController();

            var controllerContext = new Mock<ControllerContext>();
            var username = controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(dataid.Email);
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(p => p.HttpContext.Response.Cookies).Returns(new HttpCookieCollection());

            //controllerContext.Setup(p => p.HttpContext.User.Identity.AuthenticationType).Returns("ApplicationCookie");
            //controllerContext.Setup(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);


            var genericIdentity = new GenericIdentity(dataid.Email);
            genericIdentity.AddClaims(claims);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            controllerContext.SetupGet(p => p.HttpContext.User).Returns(genericPrincipal);


            controller.ControllerContext = controllerContext.Object;

            var expire_date = "2019/3/20";
            var recruiting_date = "2019/2/20";
            RecruitmentCreate data = new RecruitmentCreate {
                Company_id = 1,
                Is_Show = true,
                Nums_view = 0,
                Status_id = 2,
                Districts_id = 2,
                title = "Test Unit Data",
                Expire_date = DateTime.Parse(expire_date),
                Recruiting_dates = DateTime.Parse(recruiting_date),
                Salary_from = 5000000,
                Salary_to = 10000000,
                Is_Full_Time = true,
                Is_Part_Time = false,
                Is_Intership = false,
                Mo_ta_Chi_Tiet = "Công ty thử nghiệm game",
                Ky_Nang_Cong_Viec = "Tốt nghiệp đại học trở lên",
                Phuc_Loi = "Phúc lợi tốt",
                Tuy_Chon = "Liên hệ:....",
                //recttag = new int[] { 1, 2 }
            };

            // Act
            var result = controller.Post_NewRecruitment(data) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("List", result.RouteValues["action"]);
            Assert.AreEqual("Recruitment", result.RouteValues["controller"]);
        }
    }
}
