using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Collections.Generic;
using TrangTuyenDung.Areas.Company.Controllers;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using System.Web.Routing;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class CompanyRecruitmentControllerTest {
        EJobEntities db = new EJobEntities();

        [Priority(8)]
        [TestMethod]
        public void CreateRecruitment_WithCreateSuccessful() {
            // Arr

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            var email = "Company@gmail.com";
            var id = db.AspNetUsers.FirstOrDefault(x => x.Email == email).Id;

            var controller = new RecruitmentController();

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

            var companyid = db.User_In_Company.FirstOrDefault(x => x.AspNetUser.Email == email).Company_id;
            var type = db.Districts.FirstOrDefault(x => x.Type == "Quận").Id;
            var typename = db.Districts.FirstOrDefault(x => x.District_Name == "1").Id;
            var districtid = type + typename;
            var expiredate = "07/06/2019";
            var recruitingdate = "08/03/2019";

            // Act
            RecruitmentCreate data = new RecruitmentCreate() {
                Company_id = companyid,
                Districts_id = districtid,
                title = "Công việc thử nghiệm",
                Expire_date = DateTime.Parse(expiredate),
                Recruiting_dates = DateTime.Parse(recruitingdate),
                Is_Full_Time = true,
                Mo_ta_Chi_Tiet = "Công ty tuyển các vị trí kiểm thử,...",
                Ky_Nang_Cong_Viec = "Tốt nghiệp đại học các ngành liên quan đến CNTT",
                Phuc_Loi = "Lương tháng 13",
                Created_by = email,
                Num_FullTime = 8,
                //recttag = new int[] {2}

            };
            //var redirect = controller.Create(data) as RedirectToRouteResult;

            //// Assert
            //Assert.AreEqual("Create", redirect.RouteValues["action"]);
            //Assert.AreEqual("Recruitment", redirect.RouteValues["controller"]);
        }
    }
}
