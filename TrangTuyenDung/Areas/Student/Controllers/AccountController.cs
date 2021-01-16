using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrangTuyenDung.Models;
using TrangTuyenDung.Areas.Student.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Web.UI;
using Newtonsoft.Json;

namespace TrangTuyenDung.Areas.Student.Controllers {
    [Authorize(Roles = "Student")]
    public class AccountController : Controller {
        EJobEntities db = new EJobEntities();
        // GET: Student/Account
        public ActionResult Index() {
            return View();
        }

        //Thuan Nguyen - GET: Dashboard of Student
        [HttpGet]
        public ActionResult Dashboard() {
            //Hide suggest jobs form
            TempData["ShowSuggest"] = false;
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //Get view model
            var model = new StudentDashboardViewModel();
            //--Statistic--
            //number of Applying
            var modelRecs = db.Apply_Recruitments.Where(x => x.CV_Info.Student_Info.Account_Id == crrUser);
            model.numApplying = modelRecs.Count();
            model.numPendingApply = modelRecs.Where(x => x.CV_Status == 3).Count();
            model.numInterviewingApply = modelRecs.Where(x => x.CV_Status == 4).Count();
            model.numRejectedApply = modelRecs.Where(x => x.CV_Status == 6).Count();
            //number of Interview Result --FIXME
            model.numSuccessInterview = modelRecs.Where(x => x.CV_Status == 7).Count();
            model.numFailInterview = modelRecs.Where(x => x.CV_Status == 8).Count();
            //Status of personal CV 
            var chkPublic = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
            if (chkPublic == null) {
                model.CVStatus = 0; //Not created CV public
            }
            else {
                if (chkPublic.CV_Status == 1) {
                    model.CVStatus = 1; //Private
                }
                else if (chkPublic.CV_Status == 2) {
                    model.CVStatus = 2; //Publishing
                }
            }
            //number of Bookmarked CV
            model.numBookmarked = db.Bookmarks.Where(x => x.Student_Info.Account_Id == crrUser).Count();

            //return
            return View(model);
        }
        //GET: Student Tracking CV
        [HttpGet]
        public ActionResult TrackingCV() {
            //Hide suggest jobs form
            TempData["ShowSuggest"] = false;
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //Get applying info of crrUser
            var model = db.Apply_Recruitments.Where(x => x.CV_Info.Student_Info.Account_Id == crrUser);
            //return
            return View(model);
        }

        // GET: Student/Account/Detail
        // GET detail Student's infomation
        //[OutputCache(CacheProfile = "NoCache")]
        [HttpGet]
        public ActionResult Detail(string message = null) {
            //Get current User
            var User_Id = User.Identity.GetUserId();
            //Get Student_Info
            var student = db.Student_Info.FirstOrDefault(x => x.Account_Id == User_Id);
            //Get CV Info
            var CV = db.CV_Info.FirstOrDefault(x => x.Student_Id == student.Id);
            //Get Personal_Skills
            var PS = db.Student_Skills.Where(x => x.Student_Id == student.Id).ToArray();
            //Get Work Experiences
            var WE = db.Student_Work_Experiences.Where(x => x.Student_Id == student.Id).ToArray();
            //Get Certificates
            var Cer = db.Student_Certificates.Where(x => x.Student_Id == student.Id).ToArray();
            //Get Personal Project
            var Proj = db.Student_Projects.Where(x => x.Student_Id == student.Id).ToArray();
            //----------------------------------------------------
            //Binding data to viewModel
            //Student Info
            var model = new Personal_infoViewModel() {
                ID = student.Id,
                AspNetUser = student.AspNetUser,
                Faculties_Id = student.Faculties_Id,
                Faculty = student.Faculty,
                MSSV = student.Student_Id,
                Student_Address = student.Student_Address,
                Student_Birthday = student.Student_Birthday,
                Student_Gender = student.Student_Gender,
                Student_Name = student.Student_Name,
                Student_PhoneNumber = student.Student_PhoneNumber,
                email = User.Identity.GetUserName(),
                Account_Id = student.Account_Id,
                Student_Class = student.Student_Class

            };
            //Student CV
            if (CV != null) {

                model.CV_Template_Id = CV.CV_Template_Id;
                model.Personal_Goal = CV.Personal_Goal;
                model.About_Student = CV.About_Student;
            }
            else {
                model.Personal_Goal = "";
                model.About_Student = "";
            }
            //Personal Skills
            if (PS.Length > 0) {

                model.PS_01 = "" + PS[0].Name;
                model.PS_Rate_01 = PS[0].Rate;
                //
                model.PS_02 = "" + PS[1].Name;
                model.PS_Rate_02 = PS[1].Rate;
                //
                model.PS_03 = "" + PS[2].Name;
                model.PS_Rate_03 = PS[2].Rate;
            }
            else {
                model.PS_01 = "";
                model.PS_Rate_01 = null;
                model.PS_02 = "";
                model.PS_Rate_02 = null;
                model.PS_03 = "";
                model.PS_Rate_03 = null;
            }
            //WE
            if (WE.Length > 0) {

                model.WE_Company_01 = "" + WE[0].Company_Name;
                model.WE_Date_Start_01 = "" + WE[0].Date_Start;
                model.WE_Date_End_01 = "" + WE[0].Date_End;
                model.WE_Description_01 = "" + WE[0].Description;
                //
                model.WE_Company_02 = "" + WE[1].Company_Name;
                model.WE_Date_Start_02 = "" + WE[1].Date_Start;
                model.WE_Date_End_02 = "" + WE[1].Date_End;
                model.WE_Description_02 = "" + WE[1].Description;
                //
                model.WE_Company_03 = "" + WE[2].Company_Name;
                model.WE_Date_Start_03 = "" + WE[2].Date_Start;
                model.WE_Date_End_03 = "" + WE[2].Date_End;
                model.WE_Description_03 = "" + WE[2].Description;
            }
            else {
                model.WE_Company_01 = "";
                model.WE_Date_Start_01 = "";
                model.WE_Date_End_01 = "";
                model.WE_Description_01 = "";
                //
                model.WE_Company_02 = "";
                model.WE_Date_Start_02 = "";
                model.WE_Date_End_02 = "";
                model.WE_Description_02 = "";
                //
                model.WE_Company_03 = "";
                model.WE_Date_Start_03 = "";
                model.WE_Date_End_03 = "";
                model.WE_Description_03 = "";
            }
            //Cer
            if (Cer.Length > 0) {

                model.Cer_Name_01 = "" + Cer[0].Name;
                model.Cer_GetDate_01 = "" + Cer[0].Date_Get;
                model.Cer_Description_01 = "" + Cer[0].Description;
                //
                model.Cer_Name_02 = "" + Cer[1].Name;
                model.Cer_GetDate_02 = "" + Cer[1].Date_Get;
                model.Cer_Description_02 = "" + Cer[1].Description;
                //
                model.Cer_Name_03 = "" + Cer[2].Name;
                model.Cer_GetDate_03 = "" + Cer[2].Date_Get;
                model.Cer_Description_03 = "" + Cer[2].Description;
            }
            else {
                model.Cer_Name_01 = "";
                model.Cer_GetDate_01 = "";
                model.Cer_Description_01 = "";
                //
                model.Cer_Name_02 = "";
                model.Cer_GetDate_02 = "";
                model.Cer_Description_02 = "";
                //
                model.Cer_Name_03 = "";
                model.Cer_GetDate_03 = "";
                model.Cer_Description_03 = "";
            }
            //Project
            if (Proj.Length > 0) {

                //project 1
                model.Project_Name_1 = "" + Proj[0].Name;
                model.Project_Description_1 = "" + Proj[0].Description;
                model.Project_Date_Start_1 = "" + Proj[0].Date_Start;
                model.Project_Date_End_1 = "" + Proj[0].Date_End;
                //project 2
                model.Project_Name_2 = "" + Proj[1].Name;
                model.Project_Description_2 = "" + Proj[1].Description;
                model.Project_Date_Start_2 = "" + Proj[1].Date_Start;
                model.Project_Date_End_2 = "" + Proj[1].Date_End;
            }
            else {
                //project 1
                model.Project_Name_1 = "";
                model.Project_Description_1 = "";
                model.Project_Date_Start_1 = null;
                model.Project_Date_End_1 = null;
                //project 2
                model.Project_Name_2 = "";
                model.Project_Description_2 = "";
                model.Project_Date_Start_2 = null;
                model.Project_Date_End_2 = null;
            }
            //
            ViewBag.Faculties_Id = new SelectList(db.Faculties, "Id", "Name", db.Faculties.FirstOrDefault(x => x.Id == model.Faculties_Id).Id.ToString());
            var Gender = new[] { new SelectListItem { Text = "Nam", Value = true.ToString() }, new SelectListItem { Text = "Nữ", Value = false.ToString() } };
            foreach (var item in Gender) {
                if (item.Value == model.Student_Gender.ToString()) {
                    item.Selected = true;
                    break;
                }
            }

            //var CV_template = new[] { new SelectListItem { Text = "Mẫu CV Mặc Định", Value = "0" }, new SelectListItem { Text = "Mẫu CV 01", Value = "1" }, new SelectListItem { Text = "Mẫu CV 02", Value = "2" } };
            //foreach (var item in CV_template)
            //{
            //    if (item.Value == model.CV_Template_Id.ToString())
            //    {
            //        item.Selected = true;
            //        break;
            //    }
            //}
            //ViewBag.CV_Template_Id = CV_template;
            ViewBag.Student_Gender = Gender;
            //Fix Cache when change avatar
            Random randomKey = new Random();
            ViewBag.Random = randomKey.Next(1, 1000);

            //Check Message
            if (message == "NeedCreateCV") {
                ViewBag.Message = "NeedCreateCV";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditStudent_Info(Student_InfoViewModel model) {
            EJobEntities db = new EJobEntities();
            if (ModelState.IsValid) {
                var stuInfo = db.Student_Info.Find(model.Id);
                //Mapping ViewModel and dât in model
                stuInfo.Student_Update_at = DateTime.Now;
                stuInfo.Student_Name = model.Student_Name;
                stuInfo.Faculties_Id = model.Faculties_Id;
                stuInfo.Student_Birthday = model.Student_Birthday;
                stuInfo.Student_Gender = model.Student_Gender;
                stuInfo.Student_Address = model.Student_Address;
                stuInfo.Student_PhoneNumber = model.Student_PhoneNumber;
                stuInfo.Student_Id = model.MSSV;
                stuInfo.Student_Class = model.Student_Class;
                //Save Avatar
                //Change name of logo image to Student Account_Id.extension (Ex: 10.jpg)
                if (model.Student_Avatar != null) {
                    if (model.Student_Avatar.ContentLength != 0 && model.Student_Avatar.FileName != "") {
                        string newName = stuInfo.Account_Id.ToString();
                        //string physicalPath = Server.MapPath("~/Areas/Student/Logos/" + imageName)
                        string path = Path.Combine(Server.MapPath("~/App_Data/Student/Logos/"), newName);
                        // If same name of file present then delete that file first
                        if (System.IO.File.Exists(path)) {
                            System.IO.File.Delete(path);
                        }
                        //Save image to physical folder
                        model.Student_Avatar.SaveAs(path);
                    }
                }
                TempData["update"] = "UserUpdateSuccess";
                //Save new record
                db.SaveChanges();
                HttpResponse.RemoveOutputCacheItem("");
                return RedirectToAction("Detail", "Account", new { area = "Student" });
            }
            TempData["update"] = "UserUpdateFail";
            return RedirectToAction("Detail", "Account", new { area = "Student" });
        }


        [HttpPost]
        public ActionResult Update_CV(CV_UpdateViewModel data) {
            EJobEntities db = new EJobEntities();
            if (ModelState.IsValid) {
                //Post CV_Info
                var CV = db.CV_Info.FirstOrDefault(x => x.Student_Id == data.ID);
                if (CV != null) {
                    CV.Personal_Goal = data.Personal_Goal;
                    CV.About_Student = data.About_Student;
                    CV.CV_Template_Id = data.CV_Template_Id;
                    db.SaveChanges();
                }
                else {
                    var newCV = new CV_Info {
                        Personal_Goal = data.Personal_Goal,
                        About_Student = data.About_Student,
                        CV_Template_Id = data.CV_Template_Id,
                        Student_Id = data.ID
                    };
                    db.CV_Info.Add(newCV);
                    db.SaveChanges();
                }
                //Post Personal_Skills
                var PS = db.Student_Skills.Where(x => x.Student_Id == data.ID).ToArray();
                if (PS.Length > 0) {

                    PS[0].Name = data.PS_01;
                    PS[0].Rate = data.PS_Rate_01;
                    //
                    PS[1].Name = data.PS_02;
                    PS[1].Rate = data.PS_Rate_02;
                    //
                    PS[2].Name = data.PS_03;
                    PS[2].Rate = data.PS_Rate_03;
                    //Save
                    db.Entry(PS[0]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(PS[1]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(PS[2]).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    db.Student_Skills.Add(new Student_Skills {
                        Name = data.PS_01,
                        Rate = data.PS_Rate_01,
                        Student_Id = data.ID
                    });
                    //
                    db.Student_Skills.Add(new Student_Skills {
                        Name = data.PS_02,
                        Rate = data.PS_Rate_02,
                        Student_Id = data.ID
                    });
                    db.Student_Skills.Add(new Student_Skills {
                        Name = data.PS_03,
                        Rate = data.PS_Rate_03,
                        Student_Id = data.ID
                    });
                    db.SaveChanges();
                }
                //POST Work Experiences
                var WE = db.Student_Work_Experiences.Where(x => x.Student_Id == data.ID).ToArray();
                if (WE.Length > 0) {

                    //01
                    WE[0].Job_Position = data.WE_Job_Position_01;
                    WE[0].Company_Name = data.WE_Company_01;
                    WE[0].Date_Start = data.WE_Date_Start_01;
                    WE[0].Date_End = data.WE_Date_End_01;
                    WE[0].Description = data.WE_Description_01;
                    //02
                    WE[1].Job_Position = data.WE_Job_Position_02;
                    WE[1].Company_Name = data.WE_Company_02;
                    WE[1].Date_Start = data.WE_Date_Start_02;
                    WE[1].Date_End = data.WE_Date_End_02;
                    WE[1].Description = data.WE_Description_02;
                    //03
                    WE[2].Job_Position = data.WE_Job_Position_03;
                    WE[2].Company_Name = data.WE_Company_03;
                    WE[2].Date_Start = data.WE_Date_Start_03;
                    WE[2].Date_End = data.WE_Date_End_03;
                    WE[2].Description = data.WE_Description_03;
                    //save
                    db.Entry(WE[0]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(WE[1]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(WE[2]).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    //01
                    db.Student_Work_Experiences.Add(new Student_Work_Experiences {
                        Job_Position = data.WE_Job_Position_01,
                        Company_Name = data.WE_Company_01,
                        Date_Start = data.WE_Date_Start_01,
                        Date_End = data.WE_Date_End_01,
                        Description = data.WE_Description_01,
                        Student_Id = data.ID
                    });
                    //02
                    db.Student_Work_Experiences.Add(new Student_Work_Experiences {
                        Job_Position = data.WE_Job_Position_02,
                        Company_Name = data.WE_Company_02,
                        Date_Start = data.WE_Date_Start_02,
                        Date_End = data.WE_Date_End_02,
                        Description = data.WE_Description_02,
                        Student_Id = data.ID
                    });
                    //03
                    db.Student_Work_Experiences.Add(new Student_Work_Experiences {
                        Job_Position = data.WE_Job_Position_03,
                        Company_Name = data.WE_Company_03,
                        Date_Start = data.WE_Date_Start_03,
                        Date_End = data.WE_Date_End_03,
                        Description = data.WE_Description_03,
                        Student_Id = data.ID
                    });
                    //save
                    db.SaveChanges();
                }
                //POST Certificates
                var Cer = db.Student_Certificates.Where(x => x.Student_Id == data.ID).ToArray();
                if (Cer.Length > 0) {

                    Cer[0].Name = data.Cer_Name_01;
                    Cer[0].Date_Get = data.Cer_GetDate_01;
                    Cer[0].Description = data.Cer_Description_01;
                    //
                    Cer[1].Name = data.Cer_Name_02;
                    Cer[1].Date_Get = data.Cer_GetDate_02;
                    Cer[1].Description = data.Cer_Description_02;
                    //
                    Cer[2].Name = data.Cer_Name_03;
                    Cer[2].Date_Get = data.Cer_GetDate_03;
                    Cer[2].Description = data.Cer_Description_03;
                    //Save
                    db.Entry(Cer[0]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(Cer[1]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(Cer[2]).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    db.Student_Certificates.Add(new Student_Certificates {
                        Name = data.Cer_Name_01,
                        Date_Get = data.Cer_GetDate_01,
                        Description = data.Cer_Description_01,
                        Student_Id = data.ID
                    });
                    //
                    db.Student_Certificates.Add(new Student_Certificates {
                        Name = data.Cer_Name_02,
                        Date_Get = data.Cer_GetDate_02,
                        Description = data.Cer_Description_02,
                        Student_Id = data.ID
                    });
                    //
                    db.Student_Certificates.Add(new Student_Certificates {
                        Name = data.Cer_Name_03,
                        Date_Get = data.Cer_GetDate_03,
                        Description = data.Cer_Description_03,
                        Student_Id = data.ID
                    });
                    //Save
                    db.SaveChanges();
                }
                //POST Student_Project
                var Proj = db.Student_Projects.Where(x => x.Student_Id == data.ID).ToArray();
                if (Proj.Length > 0) {

                    Proj[0].Name = data.Project_Name_1;
                    Proj[0].Date_Start = data.Project_Date_Start_1;
                    Proj[0].Date_End = data.Project_Date_End_1;
                    Proj[0].Description = data.Project_Description_1;
                    //
                    Proj[1].Name = data.Project_Name_2;
                    Proj[1].Date_Start = data.Project_Date_Start_2;
                    Proj[1].Date_End = data.Project_Date_End_2;
                    Proj[1].Description = data.Project_Description_2;
                    //Save
                    db.Entry(Proj[0]).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(Proj[1]).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                    //
                    db.Student_Projects.Add(new Student_Projects {
                        Name = data.Project_Name_1,
                        Date_Start = data.Project_Date_Start_1,
                        Date_End = data.Project_Date_End_1,
                        Description = data.Project_Description_1,
                        Student_Id = data.ID
                    });
                    //
                    db.Student_Projects.Add(new Student_Projects {
                        Name = data.Project_Name_2,
                        Date_Start = data.Project_Date_Start_2,
                        Date_End = data.Project_Date_End_2,
                        Description = data.Project_Description_2,
                        Student_Id = data.ID
                    });
                    //Save
                    db.SaveChanges();
                }

                //
                TempData["update"] = "CvUpdateSuccess";
                return RedirectToAction("Detail", "Account", new { area = "Student" });
            }
            else {
                TempData["update"] = "CVUpdateFail";
                return RedirectToAction("Detail", "Account", new { area = "Student" });
            }
        }


        //Thuan Nguyen - GET: View CV by Student_Id
        [HttpGet]
        public ActionResult View_CV() {
            //Get current User
            var User_Id = User.Identity.GetUserId();
            //Get Student_Info
            var student = db.Student_Info.FirstOrDefault(x => x.Account_Id == User_Id);
            //Get CV Info
            var CV = db.CV_Info.FirstOrDefault(x => x.Student_Id == student.Id);

            //Check have created CV?
            if (CV == null) {
                return Content("Bạn cần tạo thông tin cho CV trước khi xem nhé! Xin cảm ơn! ^^");
            }
            else {
                //Get Personal_Skills
                var PS = db.Student_Skills.Where(x => x.Student_Id == student.Id).ToArray();
                //Get Work Experiences
                var WE = db.Student_Work_Experiences.Where(x => x.Student_Id == student.Id).ToArray();
                //Get Certificates
                var Cer = db.Student_Certificates.Where(x => x.Student_Id == student.Id).ToArray();
                //Get Personal Project
                var Proj = db.Student_Projects.Where(x => x.Student_Id == student.Id).ToArray();
                //----------------------------------------------------
                //Binding data to viewModel
                //Student Info
                var model = new Personal_infoViewModel() {
                    //Student_Info
                    ID = student.Id,
                    AspNetUser = student.AspNetUser,
                    Faculties_Id = student.Faculties_Id,
                    Faculty = student.Faculty,
                    MSSV = student.Student_Id,
                    Student_Address = student.Student_Address,
                    Student_Birthday = student.Student_Birthday,
                    Student_Gender = student.Student_Gender,
                    Student_Name = student.Student_Name,
                    Student_PhoneNumber = student.Student_PhoneNumber,
                    email = User.Identity.GetUserName(),
                    Account_Id = student.Account_Id,
                    Student_Class = student.Student_Class,
                    //CV_Info
                    CV_Template_Id = CV.CV_Template_Id,
                    About_Student = CV.About_Student,
                    Personal_Goal = CV.Personal_Goal,
                    //Virtual 
                    Student_Certificates = Cer,
                    Student_Projects = Proj,
                    Student_Work_Experiences = WE,
                    Student_Skills = PS
                };
                //Check template null
                if (model.CV_Template_Id == null) {
                    //Template_Id = null -> default
                    return RedirectToAction("Detail", "Account", new { area = "Student", @message = "NeedCreateCV" });
                }
                else {
                    //Check CV_Template
                    if (model.CV_Template_Id == 1) {
                        return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01.cshtml", model);
                    }
                    else if (model.CV_Template_Id == 2) {
                        return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate02.cshtml", model);
                    }
                    else {
                        return RedirectToAction("Detail", "Account", new { area = "Student", @message = "NeedCreateCV" });
                    }
                }
            }
        }

        //Thuan Nguyen - GET: Public CV for Student
        [HttpGet]
        public ActionResult Public_CV(string message = null) {
            //Get current user 
            var crrUser = User.Identity.GetUserId();
            //Hide suggest jobs form
            TempData["ShowSuggest"] = false;
            //Check user have create CV?
            var chkCV = db.CV_Info.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
            if (chkCV != null) {
                ViewBag.chkCV = true;
            }
            else {
                ViewBag.chkCV = false;
            }

            //Check status of public CV
            var chkCVPublic = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
            if (chkCVPublic == null) {
                ViewBag.CVStatus = 0;
            }
            else {
                if (chkCVPublic.CV_Status == 1) {
                    ViewBag.CVStatus = 1;
                }
                else if (chkCVPublic.CV_Status == 2) {
                    ViewBag.CVStatus = 2;
                }
            }
            //Check message
            if (message == "PublicSuccess") {
                ViewBag.Message = "PublicSuccess";
            }
            else if (message == "PublicFail") {
                ViewBag.Message = "PublicFail";
            }
            else if (message == "PrivateSuccess") {
                ViewBag.Message = "PrivateSuccess";
            }
            else if (message == "PrivateFail") {
                ViewBag.Message = "PrivateFail";
            }
            else if (message == "UpdateSuccess") {
                ViewBag.Message = "UpdateSuccess";
            }
            else if (message == "Wrong") {
                ViewBag.Message = "Wrong";
            }

            return View();
        }
        //Thuan Nguyen - POST: Public CV for Student
        public ActionResult Publicing_CV(int status) {
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //Check status
            if (status == 2) {
                //Check if this is the first time of public CV
                var chkCVPublic = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
                if (chkCVPublic == null) {
                    //--Create new Public CV record
                    //Find current Student
                    var currentStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == crrUser);
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
                    //Publishing this CV content
                    db.CV_Publics.Add(new CV_Publics {
                        Student_Id = currentStu.Id,
                        CV_Status = 2, //status: public
                        Date_Public = DateTime.Now,
                        CV_Content = strCV
                    });

                    int result = db.SaveChanges();
                    //Check result of adding to db
                    if (result == 1) {
                        return RedirectToAction("Public_CV", new { message = "PublicSuccess" });
                    }
                    else {
                        return RedirectToAction("Public_CV", new { message = "PublicFail" });
                    }
                }
                else {
                    //Change status of CV Public
                    chkCVPublic.CV_Status = 2;

                    db.SaveChanges();
                    return RedirectToAction("Public_CV", new { message = "PublicSuccess" });
                }

            }
            else if (status == 1) {
                //--Private
                //check current CV Public record
                var chkCVPublic = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
                if (chkCVPublic != null) {
                    //change status to 1 (private)
                    chkCVPublic.CV_Status = 1;

                    db.Entry(chkCVPublic).State = System.Data.Entity.EntityState.Modified;
                    int result = db.SaveChanges();
                    //Check result of adding to db
                    if (result == 1) {
                        return RedirectToAction("Public_CV", new { message = "PrivateSuccess" });
                    }
                    else {
                        return RedirectToAction("Public_CV", new { message = "PrivateFail" });
                    }
                }
                else {
                    //We dont need to create public CV because default we must to public CV first, if not have CV public, the view will show 
                    //to user the status is 0 (need create CV, and then if user public their CV, this function will auto create CV public)
                    return RedirectToAction("Public_CV", new { message = "PrivateSuccess" });
                }

            }
            else {
                //Wrong status id
                return RedirectToAction("Public_CV", new { @message = "Wrong" });
            }
        }

        //Thuan Nguyen - POST: Update CV Public
        public ActionResult UpdatePublicCV() {
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //Check publicing CV
            var chkPublicCV = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);
            if (chkPublicCV != null) {
                //--Update current Public_CV
                //Find current Student
                var currentStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == crrUser);
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
                //Update to DB
                chkPublicCV.CV_Content = strCV;
                chkPublicCV.Date_Public = DateTime.Now;
                db.Entry(chkPublicCV).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Return View
                return RedirectToAction("Public_CV", new { message = "UpdateSuccess" });
            }
            else {
                //Create Public CV with status 2 (public)
                Publicing_CV(2);
                return RedirectToAction("Public_CV", new { message = "PublicSuccess" });
            }

        }

        //Thuan Nguyen - GET: Student View their public CV
        public ActionResult ViewPublicCV() {
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //Find current public CV
            var crrCV = db.CV_Publics.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser);

            if (crrCV == null) {
                return Content("Không tìm thấy CV của bạn! Vui lòng đến mục \"CV => Công Khai CV\" để cập nhật! Xin cảm ơn!");
            }
            else {
                //Get Student_Info
                var student = db.Student_Info.FirstOrDefault(x => x.Id == crrCV.Student_Id);

                //----------------------------------------------------
                if (crrCV.CV_Content == null) {
                    return Content("Không thể tìm thấy CV! Vui lòng liên hệ bộ phận hỗ trợ để được giúp đỡ! Xin cảm ơn quý doanh nghiệp!!");
                }
                var cvContent = JsonConvert.DeserializeObject<ApplyViewModel>(crrCV.CV_Content);

                //Binding data to viewModel
                //Student Info
                var model = new Personal_infoViewModel() {
                    //Student Info
                    ID = student.Id,
                    AspNetUser = student.AspNetUser,
                    Faculties_Id = student.Faculties_Id,
                    Faculty = student.Faculty,
                    MSSV = student.Student_Id,
                    Student_Address = student.Student_Address,
                    Student_Birthday = student.Student_Birthday,
                    Student_Gender = student.Student_Gender,
                    Student_Name = student.Student_Name,
                    Student_PhoneNumber = student.Student_PhoneNumber,
                    email = User.Identity.GetUserName(),
                    Account_Id = student.Account_Id,
                    Student_Class = student.Student_Class,
                    //CV Info
                    About_Student = cvContent.CV_Info.About_Student,
                    Personal_Goal = cvContent.CV_Info.Personal_Goal,
                    CV_Template_Id = cvContent.CV_Info.CV_Template_Id,
                    //Virtual
                    Student_Certificates = cvContent.Cer,
                    Student_Projects = cvContent.Proj,
                    Student_Work_Experiences = cvContent.WE,
                    Student_Skills = cvContent.Skill
                };
                //Check template null
                if (model.CV_Template_Id == null) {
                    //Template_Id = null -> default is 01
                    return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01", model);
                }
                else {
                    //Fix Logo cache
                    Random rand = new Random();
                    ViewBag.Random = rand.Next(1, 1000);
                    //Check CV_Template
                    if (cvContent.CV_Info.CV_Template_Id == 0) {
                        return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01", model);
                    }
                    else if (cvContent.CV_Info.CV_Template_Id == 1) {
                        return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01.cshtml", model);
                    }
                    else if (cvContent.CV_Info.CV_Template_Id == 2) {
                        return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate02.cshtml", model);
                    }
                }
            }
            return View();
        }

        //Thuan Nguyen - GET: Student View Their Interview Result
        public ActionResult InterviewResult(string confirmed = null) {
            //Hide suggest jobs form
            TempData["ShowSuggest"] = false;
            //Get current user
            var crrUser = User.Identity.GetUserId();
            //Get list result
            var model = db.Apply_Recruitments.Where(x => x.CV_Info.Student_Info.Account_Id == crrUser && x.CV_Status == 7 || x.CV_Status == 8).ToList();
            //Message
            if (confirmed == "Yes") {
                ViewBag.Message = "Yes";
            }
            else if (confirmed == "No") {
                ViewBag.Message = "No";
            }
            return View(model);
        }
        //
        [HttpPost]
        public ActionResult getInterviewDetail(int id) {
            var model = db.Apply_Recruitments.FirstOrDefault(x => x.Id == id);
            return PartialView(@"~/Areas/Student/Views/Shared/_InterviewDetail.cshtml", model);
        }
        //POST: Student confirm to work
        public ActionResult ConfirmWork(int id, string confirmed) {
            //Get current user
            var crrUser = User.Identity.GetUserId();
            //Get data
            var model = db.Apply_Recruitments.FirstOrDefault(x => x.Id == id);
            //Update row
            if (confirmed != null) {
                if (confirmed == "Yes") {
                    model.Student_Confirm = true;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("InterviewResult", "Account", new { area = "Student", @confirmed = "Yes" });
                }
                else if (confirmed == "No") {
                    model.Student_Confirm = false;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("InterviewResult", "Account", new { area = "Student", @confirmed = "No" });
                }
                else {
                    return Content("Đã có lỗi xảy ra, vui lòng làm mới lại trang và thử lại!");
                }
            }
            else {
                return Content("Đã có lỗi xảy ra, vui lòng làm mới lại trang và thử lại!");
            }
        }
        public ActionResult Reject(int? id,FormCollection formCollection) {
            string reasons = "";
            var cv = db.Apply_Recruitments.FirstOrDefault((x => x.Recruitment_ID == id));
            if (!string.IsNullOrEmpty(formCollection["option1"])) {
                reasons += "Đã kiếm được việc! ";
            }
            if (!string.IsNullOrEmpty(formCollection["option_other"])) {
                reasons +=  formCollection["other_reason"].ToString();
            }
            cv.Student_Confirm = false;
            cv.Reason = reasons;
            db.SaveChanges();
            return RedirectToAction("TrackingCV","Account", new { area = "Student" });
        }
        public ActionResult Accept(int? id) {
          
            var cv = db.Apply_Recruitments.FirstOrDefault((x => x.Recruitment_ID == id));
            cv.Student_Confirm = true;
            db.SaveChanges();
            return RedirectToAction("TrackingCV", "Account", new { area = "Student" });
        }
    }
}