using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TrangTuyenDung.Areas.Company.Controllers;
using TrangTuyenDung.Areas.Company.Models;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class CompanyAccountControllerTest {
        EJobEntities db = new EJobEntities();

        // --- Unit test for Register Company ---//
        [Priority(36)]
        [TestMethod]
        public async Task Register_WithFullInputAndLogoExist() {
            // Arr

            string filePath = Path.GetFullPath(@"/K21T2-CAP/Project/Source code/Source250219/TrangTuyenDung/App_Data/Company/Logos/tải xuống.png");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

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
            postedfile.Setup(f => f.ContentLength).Returns(26594).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();
            postedfile.Setup(f => f.ContentType).Returns("image/png").Verifiable();

            postedfile.Setup(f => f.InputStream).Returns(fileStream);

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            CompanyRegisterViewModel comModel = new CompanyRegisterViewModel() {
                Com_Name = "Cty software",
                Email_Contact = "sophievo1297@gmail.com",
                Phone_Contact = "1234567890",
                Com_Intro = "Công ty chuyên sản xuất các sản phẩm phần mềm",
                Com_Website = "cntttest",
                Com_Address = "45 Nguyễn Khắc Nhu",
                Com_Province = 22,
                Com_District = 204,
                FullName = "Võ Thị Kim Chi",
                Email = "sophievo1297@gmail.com",
                Password = "1234567890",
                ConfirmPassword = "1234567890",
                Com_Faculty = new int[] { 1 },
                Com_OtherFaculties = "Công nghệ thực phẩm",
                Com_Logo = postedfile.Object

            };

            // Act
            var redirect = controller.Register(comModel) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Register", redirect.RouteValues["action"]);
            Assert.AreEqual("Account", redirect.RouteValues["controller"]);
        }

        [Priority(35)]
        [TestMethod]
        public async Task RegisterCompany_WithLogoLengthIsNull() {
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
            postedfile.Setup(f => f.ContentType).Returns("image/png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            var controller = new AccountController();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            CompanyRegisterViewModel comModel = new CompanyRegisterViewModel() {
                Com_Name = "Cty software",
                Email_Contact = "sophievo1297@gmail.com",
                Phone_Contact = "1234567890",
                Com_Intro = "Công ty chuyên sản xuất các sản phẩm phần mềm",
                Com_Website = "cntttest",
                Com_Address = "45 Nguyễn Khắc Nhu",
                Com_Province = 22,
                Com_District = 204,
                FullName = "Võ Thị Kim Chi",
                Email = "sophievo1297@gmail.com",
                Password = "1234567890",
                ConfirmPassword = "1234567890",
                //Com_Faculty = new int[] {1},
                Com_Logo = postedfile.Object

            };

            // Act
            var redirect = controller.Register(comModel) as ViewResult;

            // Assert
            Assert.IsTrue(redirect.ViewData.ModelState[""].Errors.Count == 1);
        }

        // --- Unit test for Register Company ---//

        // --- Unit test for Update Company Infor ---//
        [Priority(38)]
        [TestMethod]
        public void UpdateCompanyInfor_WithLogoExist() {
            // Arr
            var controller = new AccountController();

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            // ---Create mock for save avatar in server---

            //create mock of HttpServerUtilityBase
            var server = new Mock<HttpServerUtilityBase>();

            //set up mock to return known value on call.
            server.Setup(x => x.MapPath("~/App_Data/Company/Logos")).Returns("~/App_Data/Company/Logos");

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
            postedfile.Setup(f => f.ContentLength).Returns(26594).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();
            postedfile.Setup(f => f.ContentType).Returns("image/png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var provinceid = db.Provinces.FirstOrDefault(x => x.Name == "Hồ Chí Minh").Id;
            var type = db.Districts.FirstOrDefault(x => x.Type == "Quận").Id;
            var districtname = db.Districts.FirstOrDefault(x => x.District_Name == "1").Id;
            var districtid = type + districtname;

            var companyid = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id;
            Company_Info comUpdate = new Company_Info {
                Id = companyid,
                Name_Company = "Cty software",
                Introduce_Company = "Công ty chuyên sản xuất các sản phẩm phần mềm, tư vấn doanh nghiệp",
                Address_Company = "45 Nguyễn Khắc Nhu",
                Province_ID = provinceid,
                District_ID = districtid,
                Contact_Phone = "1234567890",
                Contact_Email = "sophievo1297@gmail.com",
                website_Company = "cntttest.vanlanguni.edu.vn:18080/Ejob/",
                Faculties_Other = "Công nghệ thực phẩm",

            };

            string uID = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id.ToString();
            // Act
            var redirect = controller.Update(uID) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Update", redirect.RouteValues["action"]);
        }

        [Priority(37)]
        [TestMethod]
        public void UpdateCompanyInfor_WithLogoLengthNull() {
            // Arr
            var controller = new AccountController();

            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            // ---Create mock for save avatar in server---

            //create mock of HttpServerUtilityBase
            var server = new Mock<HttpServerUtilityBase>();

            //set up mock to return known value on call.
            server.Setup(x => x.MapPath("~/App_Data/Company/Logos")).Returns("~/App_Data/Company/Logos");

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
            postedfile.Setup(f => f.ContentLength).Returns(0).Verifiable();
            postedfile.Setup(f => f.FileName).Returns("tải về.png").Verifiable();
            postedfile.Setup(f => f.ContentType).Returns("image/png").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Setup(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var provinceid = db.Provinces.FirstOrDefault(x => x.Name == "Hồ Chí Minh").Id;
            var type = db.Districts.FirstOrDefault(x => x.Type == "Quận").Id;
            var districtname = db.Districts.FirstOrDefault(x => x.District_Name == "1").Id;
            var districtid = type + districtname;

            var companyid = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id;
            Company_Info comUpdate = new Company_Info {
                Id = companyid,
                Name_Company = "Cty software",
                Introduce_Company = "Công ty chuyên sản xuất các sản phẩm phần mềm, tư vấn doanh nghiệp",
                Address_Company = "45 Nguyễn Khắc Nhu",
                Province_ID = provinceid,
                District_ID = districtid,
                Contact_Phone = "1234567890",
                Contact_Email = "sophievo1297@gmail.com",
                website_Company = "cntttest.vanlanguni.edu.vn:18080/Ejob/",
                Faculties_Other = "Công nghệ thực phẩm",
                FacRec = new int[] { 1 }

            };

            string uID = db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software").Id.ToString();
            // Act
            var redirect = controller.Update(uID) as ViewResult;

            // Assert
            Assert.IsTrue(redirect.ViewData.ModelState[""].Errors.Count == 1);

        }

    }
}
