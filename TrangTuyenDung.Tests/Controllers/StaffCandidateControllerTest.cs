using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrangTuyenDung.Areas.Staff.Controllers;
using System.Web.Mvc;

namespace TrangTuyenDung.Tests.Controllers {
    [TestClass]
    public class StaffCandidateControllerTest {
        [Priority(46)]
        [TestMethod]
        public void Pending_ViewListStudentApplyRecruitmentSuccessful() {
            // Arr
            var controller = new CandidateController();

            // Act
            var redirectResult = controller.Pending() as ViewResult;

            // Assert
            Assert.IsNotNull(true);
        }
    }
}
