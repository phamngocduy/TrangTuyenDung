using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Areas.Staff.Models;
using TrangTuyenDung.Models;
using Microsoft.AspNet.Identity;

namespace TrangTuyenDung.Areas.Manager.Controllers {
    [Authorize(Roles = "Manager")]
    public class HomeController : Controller {

        EJobEntities db = new EJobEntities();
        // GET: Manager/Home
        public ActionResult Index() {
            return View();
        }

        //Thuan Nguyen - GET: Staff view Dashboard
        [HttpGet]
        public ActionResult Dashboard() {
            StaffDashboardViewModel model = new StaffDashboardViewModel();
            //Get number of Recruitments
            var recModel = db.Recruitments;
            model.numRecs = recModel.Count();
            model.numPendingRecs = recModel.Where(x => x.Status_id == 1).Count();
            model.numActivedRecs = recModel.Where(x => x.Status_id == 2).Count();
            model.numExpiredRecs = recModel.Where(x => x.Status_id == 3).Count();
            //Get number of Companies
            var comModel = db.Company_Info;
            model.numCompany = comModel.Count();
            model.numPendingComs = db.Company_Registration.Count();
            //Get number of Candidates
            var canModel = db.Student_Info;
            model.numCandidate = canModel.Count();
            model.numApplyingCans = db.Apply_Recruitments.Count();
            model.numCVs = db.CV_Info.Count();
            //Get number of Others
            model.numFaculties = db.Faculties.Count();
            model.numJobPositions = db.Job_Positions.Count();
            model.numMajors = db.Faculties_Majors.Count();

            return View(model);
        }

        //Thuan Nguyen - GET: Staff view Statistic
        [HttpGet]
        public ActionResult Statistic() {
            return View();
        }

        //Thuan Nguyen - POST: Staff view Statistic [ajax]
        public ActionResult Statistic(string startDay, string endDay) {
            if (startDay != null && endDay != null) {
                //Parse data - "sta" key is "statistic"
                var staStart = DateTime.Parse(startDay);
                var staEnd = DateTime.Parse(endDay);
                //Get view model
                var model = new StaffStatisticViewModel();
                //Time
                model.Time = "Từ " + startDay + " đến " + endDay + " <i> (mm/dd/yyyy) </i>";
                model.Now = DateTime.Now;

                //Get current User Id
                var userID = User.Identity.GetUserId();

                //Get current User Info
                var userInfo = db.AspNetUsers.FirstOrDefault(x => x.Id == userID);
                model.staffEmail = userInfo.Email;

                //Get Recruitment Statistic
                var modelRecs = db.Recruitments.Where(x => x.Created_date >= staStart && x.Created_date <= staEnd);
                model.numRecs = modelRecs.Count();
                var modelApprovedRecs = modelRecs.Where(x => x.Status_id != 1 && x.Status_id != 4); // != pending and rejected
                model.numApprovedRecs = modelApprovedRecs.Count();
                //Get Job Type Statistic
                int numFullTime = 0, numPartTime = 0, numIntern = 0, numApplied = 0, numApplying = 0, numApprovedCan = 0;
                foreach (var item in modelApprovedRecs) {
                    //check db value to count num job types
                    if (item.Num_FullTime != null) {
                        numFullTime += (int)item.Num_FullTime;
                    }
                    if (item.Num_PartTime != null) {
                        numPartTime += (int)item.Num_PartTime;
                    }
                    if (item.Num_Intern != null) {
                        numIntern += (int)item.Num_Intern;
                    }
                    //Count student apllied to current recruitment
                    var checkApplied = db.Apply_Recruitments.Where(x => x.Recruitment_ID == item.Id);
                    numApplied += checkApplied.Count();
                    numApplying += checkApplied.Where(x => x.CV_Status == 3 || x.CV_Status == 4).Count();
                    numApprovedCan += checkApplied.Where(x => x.CV_Status == 5).Count();
                }
                model.numFullTime = numFullTime;
                model.numPartTime = numPartTime;
                model.numIntern = numIntern;
                model.numApplied = numApplied;
                model.numApplying = numApplying;
                model.numApprovedCan = numApprovedCan;
                //Statistic Job_Positions
                var modelJobPosition = db.Job_Positions.ToList();
                foreach (var item in modelJobPosition) {
                    var checkJP = modelApprovedRecs.Where(x => x.Job_Position_Id == item.Id).Count();
                    if (checkJP > 0) {
                        model.staJP += "- " + item.Name + ": " + checkJP + "<br />";
                    }
                }

                return PartialView(@"~/Areas/Staff/Views/Shared/_StatisticResult.cshtml", model);
            }
            else {
                return Content("Vui lòng nhập ngày bắt đầu và kết thúc!");
            }
        }
    }
}