using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using TrangTuyenDung.Areas.Company.Models;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Company.Controllers {
    [Authorize(Roles = "Company")]
    public class HomeController : Controller {
        //DB Entites
        EJobEntities db = new EJobEntities();

        // GET: Company/Home
        public ActionResult Index() {
            return View();
        }
        public ActionResult Dashboard() {
            var userID = User.Identity.GetUserId();
            //Find User in Company
            var UIC = db.User_In_Company.FirstOrDefault(x => x.Account_id == userID);
            // --Statistic Recruitments
            //total of Posted Recs
            ViewBag.num_PostedRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Status_id == 2).ToList().Count();
            //total of Pending Recs
            ViewBag.num_PendingRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Status_id == 1).ToList().Count();
            //total of Rejected Recs
            ViewBag.num_RejectedRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Status_id == 4).ToList().Count();
            //total of Expired Recs
            var now = DateTime.Now;
            ViewBag.num_ExpiredRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Status_id == 2 && x.Expire_date < now).ToList().Count();
            // --Statistic Candidate (Students)
            //total of Pending Students
            ViewBag.num_PendingStu = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == UIC.Company_id && x.CV_Status == 3).Count();
            //total of Interviewing Students
            ViewBag.num_InterviewingStu = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == UIC.Company_id && x.CV_Status == 4).Count();
            //total of Success Students
            ViewBag.num_SuccessStu = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == UIC.Company_id && x.CV_Status == 7).Count();
            ViewBag.num_FailStu = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == UIC.Company_id && x.CV_Status == 8).Count();
            //-------For Chart-------
            ViewBag.TotalView = JsonConvert.SerializeObject(lineChartView(UIC.Company_id, DateTime.Now).Select(x => x.Y).ToList());

            return View();
        }
        //Thuan Nguyen - GET: Total View Summary - Company Id
        public List<DataPoint> lineChartView(int id, DateTime endTime) {
            List<DataPoint> dataViewMonth = new List<DataPoint>();
            //Get data
            int[] month = new int[endTime.Month];
            for (int i = 1; i <= month.Length; i++) {
                //Check current
                if (i > endTime.Month) {
                    //-1 for check null
                    month[i - 1] = -1;
                }
                else {
                    var mx = db.Recruitments.Where(x => x.Company_id == id && x.Expire_date.Month <= i).ToList();
                    foreach (var item in mx) {
                        month[i - 1] += item.Nums_view;
                    }
                }
                //Add data
                dataViewMonth.Add(new DataPoint("Tháng " + i, month[i - 1]));
            }

            //Notes: other way for add data

            //dataViewMonth.Add(new DataPoint("Tháng 01", month[0]));
            //dataViewMonth.Add(new DataPoint("Tháng 02", month[1]));
            //dataViewMonth.Add(new DataPoint("Tháng 03", month[2]));
            //dataViewMonth.Add(new DataPoint("Tháng 04", month[3]));
            //dataViewMonth.Add(new DataPoint("Tháng 05", month[4]));
            //dataViewMonth.Add(new DataPoint("Tháng 06", month[5]));
            //dataViewMonth.Add(new DataPoint("Tháng 07", month[6]));

            return dataViewMonth;
        }

        public ActionResult LogoCompany() {
            var id = User.Identity.GetUserId();
            //Find User in Company
            var model = db.User_In_Company.FirstOrDefault(x => x.Account_id == id);
            //Fix Cache at Logo
            Random randomKey = new Random();
            ViewBag.Random = randomKey.Next(1, 1000);
            //Return
            return PartialView("_PartialLogoCompany", model);
        }

        //Thuan Nguyen - GET: Company View Statistic
        [HttpGet]
        public ActionResult Statistic() {
            return View();
        }

        // Thuan Nguyen - POST: Company view Statistic
        public ActionResult Statistic(string startDay, string endDay) {
            if (startDay != null && endDay != null) {
                //Parse data - "sta" key is "statistic"
                var staStart = DateTime.Parse(startDay);
                var staEnd = DateTime.Parse(endDay);
                ViewBag.Time = "Từ <i>" + startDay + "</i> đến <i>" + endDay + "<i>";
                //Get current User Id
                var userID = User.Identity.GetUserId();
                //Find User in Company
                var UIC = db.User_In_Company.FirstOrDefault(x => x.Account_id == userID);
                //
                // --COLLECT DATA HERE
                //total of Posted Recs
                var modelPostedRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Created_date >= staStart && x.Created_date <= staEnd).ToList();
                //total of Expired Recs
                var now = DateTime.Now;
                ViewBag.num_ExpiredRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Status_id != 1 && x.Status_id != 4 && x.Expire_date >= staStart && x.Expire_date <= staEnd).ToList().Count();
                //total of Activated Post
                var modelActivatedRecs = db.Recruitments.Where(x => x.Company_id == UIC.Company_id && x.Created_date >= staStart && x.Created_date <= staEnd && x.Status_id != 1 && x.Status_id != 4).ToList();


                // -- SUMMARY Statistic Count Number HERE
                ViewBag.name_Com = UIC.Company_Info.Name_Company;
                ViewBag.num_PostedRecs = modelPostedRecs.Count();
                ViewBag.num_ActivatedRecs = modelActivatedRecs.Count();
                //num job types
                int numFullTime = 0, numPartTime = 0, numIntern = 0;
                int numApplied = 0;
                foreach (var item in modelActivatedRecs) {
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
                }
                ViewBag.num_FullTime = numFullTime;
                ViewBag.num_PartTime = numPartTime;
                ViewBag.num_Intern = numIntern;
                ViewBag.num_Applied = numApplied;
                //num views
                int numView = 0;
                foreach (var item in modelPostedRecs) {
                    numView += item.Nums_view;
                }
                ViewBag.num_View = numView;

                //return table
                return PartialView(@"~/Areas/Company/Views/Shared/_StatisticResult.cshtml");
            }
            else {
                return Content("Vui lòng chọn ngày bắt đầu và kết thúc");
            }

        }


    }
}