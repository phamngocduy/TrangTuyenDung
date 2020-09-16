using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrangTuyenDung.Models;
using System.Web.Mvc;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using System.Text;
using Newtonsoft.Json;
using PagedList;
using PagedList.Mvc;

namespace TrangTuyenDung.Controllers {
    public class RecruitmentController : Controller {

        EJobEntities db = new EJobEntities();
        List<Recruitment> model;
        // GET: Recruitment
        public ActionResult Index() {
            return View();
        }
        // GET: Student/Recruitment
        [AllowAnonymous]
        public ActionResult ListOfRecruitment(int? page) {
            if (page == null) page = 1;

            var model = db.Recruitments.Where(x => x.Status_id != 1 && x.Status_id!=4&& x.Is_Show==true).OrderByDescending(x => x.Id).ToList();

            var list = model.ToPagedList(page ?? 1, 10);



            // 5. Trả về các Link được phân trang theo kích thước và số trang.
            return View(list);
        }


        public static string StripTagRegex(string source) {
            return Regex.Replace(source, @"<(?!\/?p(?=>|\s.*>))\/?.*?>", string.Empty);
        }


        public ActionResult Detail(int? id, string applyStatus = null, bool flag = false) {
            var day = DateTime.Now;
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var model = db.Recruitments.FirstOrDefault(x => x.Id == id && x.Is_Show == true );

            if (model == null) {
                //return HttpNotFound();

                TempData["shortMessage"] = "Fail";
                return RedirectToAction("Pending", "Candidate", new { area = "Staff" });
            }
            //Check User have create CV?
            var currentUser = User.Identity.GetUserId();
            var checkCV = db.CV_Info.FirstOrDefault(m => m.Student_Info.Account_Id == currentUser);
            if (checkCV != null) {
                ViewBag.checkCV = true;
            }
            else {
                ViewBag.checkCV = false;
            }
            //Check User applied or not?
            var check = db.Apply_Recruitments.FirstOrDefault(x => x.CV_Info.Student_Info.Account_Id == currentUser && x.Recruitment_ID == id);
            if (check != null) {
                ViewBag.checkApplied = true;
            }

            //Check Status when applying
            if (applyStatus == "Success") {
                ViewBag.ApplyStatus = "Success";
            }
            model.Nums_view = model.Nums_view + 1;
            //Check user have bookmark?
            var chkBookmark = db.Bookmarks.FirstOrDefault(x => x.Student_Info.Account_Id == currentUser && x.Rec_Id == id);
            if (chkBookmark != null) {
                ViewBag.chkBookmark = true;
            }
            else {
                ViewBag.chkBookmark = false;
            }

            //Leader add new here
            var data = new RecruitmentShow {
                title = model.title,
                Company_id = model.Company_id,
                Company_Info = model.Company_Info,
                Districts_id = model.Districts_id,
                District = model.District,
                Expire_date = model.Expire_date,
                Is_Full_Time = model.Is_Full_Time,
                Is_Intership = model.Is_Intership,
                Is_Part_Time = model.Is_Part_Time,
                Job_Benefits = model.Job_Benefits,
                Job_Positions = model.Job_Positions,
                Job_Position_Id = model.Job_Position_Id,
                //Job_Description = StripTagRegex(model.Job_Description),
                Job_Description = model.Job_Description,
                Num_FullTime = model.Num_FullTime,
                Nums_view = model.Nums_view,
                Num_Intern = model.Num_Intern,
                Num_PartTime = model.Num_PartTime,
                Salary_from = model.Salary_from,
                Salary_to = model.Salary_to,
                //Faculty = model.Faculty,
                //Faculty_Id = model.Faculty_Id,
                Job_Optional = model.Job_Optional,
                Recruiting_dates = model.Recruiting_dates,
                Id = model.Id,
                Required_Skills = model.Required_Skills,
                Soft_Skills=model.Soft_Skills
            
            };
            db.SaveChanges();
            if (flag == true) {
                ViewBag.Title1 = "Phát triển năng lực nghề nghiệp cho sinh viên";
            }
            return View(data);
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("Search")]
        public ActionResult Fillter(Searchsupport data) {
            string sort = "";

            string type_Job = "";

            if (!string.IsNullOrEmpty(data.all)) {
                sort = Regex.Replace(data.all, @"[\`|\~|\!|\@|\#|\$|\%|\^|\&|\*|∣|∣|\+|\=|\[|\{|\]|\}|\||\\|\'|\<|\,|\.|\>|\?|\/|\""|\;|\:|]", "").Trim();
                StringBuilder sb = new StringBuilder();
                foreach (char c in sort) {
                    if (Char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == ' ' || c == '%') { sb.Append(c); }
                }
                type_Job = sb.ToString().ToLower();
            }


            if (db.Provinces.Any(x => x.Id == data.city) || db.Faculties.Any(x => x.Id == data.fac) || (!string.IsNullOrEmpty(sort) && !string.IsNullOrWhiteSpace(sort))) {
                var day = DateTime.Now;
                model = db.Recruitments.Where(x => x.Is_Show == true && x.Expire_date >= day && x.Status_id == 2).ToList();

                if (type_Job == "fulltime") {
                    model = model.Where(x => x.Is_Full_Time == true).ToList();
                }
                if (type_Job == "parttime") {
                    model = model.Where(x => x.Is_Part_Time == true).ToList();
                }
                if (type_Job == "intership") {
                    model = model.Where(x => x.Is_Intership == true).ToList();
                }
                //query select faculties
                if (db.Faculties.Any(x => x.Id == data.fac)) {
                    ////tim tin tuyen dung voi nganh tuong ung
                    var get_recruiment_by_faculties = db.Majors_In_Recruitments.Where(x => x.Faculties_Majors.Faculty_Id == data.fac).Select(x => x.Recruitment);
                    ////so sanh list tin tuyen dung duoc show voi list vua tim duoc o tren
                    model = model.Intersect(get_recruiment_by_faculties).ToList();
                }
                //query select city
                if (db.Provinces.Any(x => x.Id == data.city)) {
                    model = model.Where(x => x.District.ProvinceId == data.city).ToList();
                }
                if ((!string.IsNullOrEmpty(sort) && !string.IsNullOrWhiteSpace(sort))) {
                    var name_title = db.Recruitments.Where(x => x.title.ToLower().Contains(sort.ToLower())).ToList();
                    var name_company = db.Recruitments.Where(x => x.Company_Info.Name_Company.ToLower().Contains(sort.ToLower())).ToList();
                    var tag_recruitment = db.Tags_Recruitments.Where(x => x.Tag.Name_Tag.ToLower().Contains(sort.ToLower())).Select(y => y.Recruitment).ToList();

                    //compare model and name_company get data
                    if (name_company.Count > 0) {
                        model = model.Intersect(name_company).ToList();
                    }
                    //compare model and name_title get duplicate data
                    if (name_title.Count > 0) {
                        model = model.Intersect(name_title).ToList();
                    }
                    //compare model and tag_recruitment
                    if (tag_recruitment.Count > 0) {
                        model = model.Intersect(tag_recruitment).ToList();
                    }
                    if (!db.Provinces.Any(x => x.Id == data.city) && !db.Faculties.Any(x => x.Id == data.fac) && tag_recruitment.Count == 0 && name_title.Count == 0 && name_company.Count == 0) {
                        TempData["ShowSuggest"] = true;
                        return View("Fillter");
                    }
                }
                //sap xep tu moi den cu theo id
                var ListFillter = model.OrderByDescending(x => x.Id).ToList();
                TempData["ShowSuggest"] = true;
                return View("Fillter", ListFillter);
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("Tim-Kiem-Theo-Nganh")]
        public ActionResult Search_Faculties(int id) {
            EJobEntities db = new EJobEntities();
            var day = DateTime.Now;
            //tim tin tuyen dung voi nganh tuong ung
            model = db.Recruitments.Where(x => x.Is_Show == true && x.Expire_date >= day && x.Status_id == 2 /*&& x.Faculty_Id==id*/).ToList(); //FIXME
            TempData["ShowSuggest"] = true;
            return View("Fillter", model);
        }

        // Thuan Nguyen - POST: Student Apply to Recruitment
        //public ActionResult Apply(int recID)
        //{
        //    //Get current Account
        //    var userID = User.Identity.GetUserId();
        //    //Find current Student
        //    var currentStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == userID);
        //    //Find CV of current Student
        //    var currentCV = db.CV_Info.FirstOrDefault(x => x.Student_Id == currentStu.Id);
        //    //Mapping current Student to current Recruitments
        //    Apply_Recruitments apply = new Apply_Recruitments()
        //    {
        //        CV_ID = currentCV.ID,
        //        Recruitment_ID = recID,
        //        Date_Apply = DateTime.Now,
        //        CV_Status = 3 //Status: Pending
        //    };

        //    db.Apply_Recruitments.Add(apply);
        //    int result = db.SaveChanges();

        //    //Check result of adding to db
        //    if(result == 1)
        //    {
        //        return RedirectToAction("Detail", new { id = recID , applyStatus = "Success" });
        //    }

        //    return RedirectToAction("Detail", new { id = recID });
        //}
        //
        //Thuan Nguyen - POST: Student apply to Recruitment - TESTING
        public ActionResult Apply(int recID) {
            //Get current Account
            var userID = User.Identity.GetUserId();
            //Find current Student
            var currentStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == userID);
            //Find CV of current Student
            var currentCV = db.CV_Info.FirstOrDefault(x => x.Student_Id == currentStu.Id);

            var GetCV_info = db.CV_Info.FirstOrDefault(x => x.Student_Id == currentStu.Id);
            var Value_CVinfo = new {
                About_Student = GetCV_info.About_Student,
                Personal_Goal = GetCV_info.Personal_Goal,
                CV_Template_Id = GetCV_info.CV_Template_Id
            };
            //Get curent CV data of Student, then convert to JSON and parse it to string to save to DB
            var valueCV = new {
                CV_Info = Value_CVinfo,
                Skill = db.Student_Skills.Where(x => x.Student_Id == currentStu.Id).Select(m => new {
                    Name = m.Name,
                    Rate = m.Rate
                }).ToArray(),
                WE = db.Student_Work_Experiences.Where(x => x.Student_Id == currentStu.Id).Select(x => new {
                    Job_Position = x.Job_Position,
                    Company_Name = x.Company_Name,
                    Description = x.Description,
                    Date_Start = x.Date_Start,
                    Date_End = x.Date_End
                }).ToArray(),
                Proj = db.Student_Projects.Where(x => x.Student_Id == currentStu.Id).Select(x => new {
                    Name = x.Name,
                    Description = x.Description,
                    Date_Start = x.Date_Start,
                    Date_End = x.Date_End
                }).ToArray(),
                Cer = db.Student_Certificates.Where(x => x.Student_Id == currentStu.Id).Select(x => new {
                    Name = x.Name,
                    Description = x.Description,
                    Date_Get = x.Date_Get
                }).ToArray()
            };
            var strCV = JsonConvert.SerializeObject(valueCV).ToString();

            //Note: this magic code for get data from db and covert to JSON, then parse it to viewModel
            //var dataModel = JsonConvert.DeserializeObject<ApplyViewModel>(strCV);
            //
            //Mapping current Student to current Recruitments
            Apply_Recruitments apply = new Apply_Recruitments() {
                CV_ID = currentCV.ID,
                Recruitment_ID = recID,
                Date_Apply = DateTime.Now,
                CV_Status = 3, //Status: Pending
                CV_Content = strCV
            };
            db.Apply_Recruitments.Add(apply);
            int result = db.SaveChanges();

            //Check result of adding to db
            if (result == 1) {
                return RedirectToAction("Detail", new { id = recID, applyStatus = "Success" });
            }

            return RedirectToAction("Detail", new { id = recID });
        }

        public JsonResult GetJobPositionByMajor(int[] id) {
            List<SelectListItem> liPosition = new List<SelectListItem>();
            liPosition.Add(new SelectListItem { Text = "--Chọn Vị Trí Ứng Tuyển--", Value = "" });
            foreach (var data in id) {
                var JobPostion = db.Job_Positions.Where(x => x.Major_Id == data).ToList();

                if (JobPostion.Count() > 0) {
                    foreach (var item in JobPostion) {
                        liPosition.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    }
                }
            }
            //Default value for Job Postion
            return Json(new SelectList(liPosition, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public JsonResult GetMajorByPosition(int id) {
            var position = db.Job_Positions.FirstOrDefault(x => x.Id == id);
            var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == position.Major_Id);

            return Json(new { id = major.Id }, JsonRequestBehavior.AllowGet);
        }

    }
}
