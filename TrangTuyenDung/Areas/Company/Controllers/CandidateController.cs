using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Areas.Company.Models;
using TrangTuyenDung.Areas.Student.Models;
using TrangTuyenDung.Models;
using TrangTuyenDung.Sercurity_SendMail;

namespace TrangTuyenDung.Areas.Company.Controllers {
    [Authorize(Roles = "Company")]
    public class CandidateController : Controller {
        //DB Entites
        EJobEntities db = new EJobEntities();
        SendMail sm = new SendMail();
        // GET: Company/Candidate
        public ActionResult Index() {
            return View();
        }

        // Thuan Nguyen - GET: Company/Candidate/Pending
        public ActionResult Pending(string message = null) {

            //Get current User
            var currentUser = User.Identity.GetUserId();
            //Get Company Info of current User
            var currentCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == currentUser);
            //Get list of Student Applying to company - Current Status - 3 Pending
            var model = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == currentCom.Company_id && x.CV_Status == 3);
            //Get message
            if (message == "ApproveSuccess") {
                ViewBag.Message = "ApproveSuccess";
            }
            else if (message == "RejectSuccess") {
                ViewBag.Message = "RejectSuccess";
            }

            return View(model);
        }
        //Thuan Nguyen - GET: Company View Interviewing Candidate
        public ActionResult Interviewing(string message = null) {
            //Get current User
            var currentUser = User.Identity.GetUserId();
            //Get Company Info of current User
            var currentCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == currentUser);
            //Get list of Student Applying to company - Current Status - 4 Interviewing
            var model = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == currentCom.Company_id && x.CV_Status == 4);
            //Get message
            if (message == "ChangeDateSuccess") {
                ViewBag.Message = "ChangeDateSuccess";
            }
            else if (message == "CancelDateSuccess") {
                ViewBag.Message = "CancelDateSuccess";
            }
            else if (message == "HireSuccess") {
                ViewBag.Message = "HireSuccess";
            }
            else if (message == "NotHireSuccess") {
                ViewBag.Message = "NotHireSuccess";
            }

            return View(model);
        }

        public ActionResult Fail() {
            //Get current User
            var currentUser = User.Identity.GetUserId();
            //Get Company Info of current User
            var currentCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == currentUser);
            //Get list of Student Applying to company - Current Status - 4 Interviewing
            var model = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == currentCom.Company_id && x.CV_Status == 8);
            //Get message
        

            return View(model);
        }
        //Thuan Nguyen - POST: Company Approve Student CV
        public ActionResult Approve(int? applyID, FormCollection formCollection) {
            //Get current User
            var currentUser = User.Identity.GetUserId();
            //Get current Recruiter info
            var recruiter = db.AspNetUsers.FirstOrDefault(m => m.Id == currentUser);
            //New record
            var newRecord = db.Apply_Recruitments.FirstOrDefault(m => m.Id == applyID);
            //Get StuID
            var stuID = newRecord.CV_Info.Student_Info.Account_Id;
            //Get Company Name
            var comName = newRecord.Recruitment.Company_Info.Name_Company.ToString();
            //Get Recruitment Title
            var recTitle = newRecord.Recruitment.title.ToString();
            //Get Recruitment Id
            var recID = newRecord.Recruitment_ID;
            //Check data
            if (applyID != null) {
                //Get Interview Datetime
                var strDatetime = formCollection["interviewDate"]; //String value
                var interviewDate = DateTime.Parse(strDatetime); //Parse to Datetime
                //Change CV Status to: 4 - Interviewing
                newRecord.CV_Status = 4;
                newRecord.Recruiter_Interview = recruiter.Email;
                newRecord.Date_Interview = interviewDate;
                newRecord.Notes = formCollection["approveNoted"];
                newRecord.Address_Interview = formCollection["interviewAddress"]; ;
                //Update DB
                db.Entry(newRecord).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() == 1) {
                    //Send email to Student
                    sm.SendEmail_ApproveCV_toStudent(stuID, comName, recID, recTitle, formCollection);
                    //Redirect View
                    return RedirectToAction("Pending", "Candidate", new { area = "Company", @message = "ApproveSuccess" });
                }
                else {
                    return HttpNotFound();
                }
            }
            else {
                return HttpNotFound();
            }
        }
        //Thuan Nguyen - POST: Company Reject Student CV
        public ActionResult Reject(int? id, FormCollection formCollection) {
            //Get current User
            var currentUser = User.Identity.GetUserId();
            //Get current Recruiter info
            var recruiter = db.AspNetUsers.FirstOrDefault(m => m.Id == currentUser);
            //Check data
            if (id != null) {
                //Get current value
                var rejectedValue = db.Apply_Recruitments.Find(id);
                rejectedValue.CV_Status = 6;
                //Get Company Name
                var comName = rejectedValue.Recruitment.Company_Info.Name_Company.ToString();
                //Get Stu Info
                var stu = rejectedValue.CV_Info.Student_Info.Account_Id;
                //Get Recruitment Title
                var recTitle = rejectedValue.Recruitment.title.ToString();
                //Get Recruitment Id
                var recID = rejectedValue.Recruitment_ID;
                //Delete Row
                //db.Apply_Recruitments.Remove(rejectedValue);
                db.Entry(rejectedValue).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() == 1) {
                    //Send email to Student
                    sm.SendEmail_RejectCV_toStudent(stu, comName, recID, recTitle, formCollection);
                    //Redirect View
                    return RedirectToAction("Pending", "Candidate", new { area = "Company", @message = "RejectSuccess" });
                }
                else {
                    return HttpNotFound();
                }
            }
            else {
                return HttpNotFound();
            }
        }

        //Thuan Nguyen - POST: Company View Candidate's CV 
        public ActionResult ViewCV(int? id) {
            //Check data
            if (id != null) {
                //Get current user
                var currentUser = User.Identity.GetUserId();
                //Get current Company
                var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
                //Get applying CV info
                var applyingCV = db.Apply_Recruitments.Find(id);

                //check ajax request
                //if (Request.IsAjaxRequest())
                //{

                if (applyingCV == null) {
                    return Content("Không tìm thấy CV để hiển thị hoặc bạn không có quyền để xem! Vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
                }
                //Just for recruitment owner!
                if (applyingCV.Recruitment.Company_id != currentCom.Company_id) {
                    return Content("Bạn không có quyền xem CV này! Vui lòng quay lại! Xin cảm ơn!");
                }
                else {
                    //Get Student_Info
                    var student = db.Student_Info.FirstOrDefault(x => x.Id == applyingCV.CV_Info.Student_Id);

                    //----------------------------------------------------
                    if (applyingCV.CV_Content == null) {
                        return Content("Không thể tìm thấy CV! Vui lòng liên hệ bộ phận hỗ trợ để được giúp đỡ! Xin cảm ơn quý doanh nghiệp!!");
                    }
                    var cvContent = JsonConvert.DeserializeObject<ApplyViewModel>(applyingCV.CV_Content);

                    //----------------------------------------------------
                    //Get data for "Company Approve Candidate
                    ViewBag.Apply_Id = id;
                    ViewBag.Address = currentCom.Company_Info.Name_Company;
                    //Check Pending or Interviewing
                    if (applyingCV.Status_CV.Id == 3) {
                        //Pending
                        ViewBag.CV_Status = 3;
                    }
                    else if (applyingCV.Status_CV.Id == 4) {
                        //Interviewing
                        ViewBag.CV_Status = 4;
                    }
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
                        email = student.AspNetUser.UserName, //FIXING
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
                        if (model.CV_Template_Id == 0) {
                            return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01", model);
                        }
                        else if (model.CV_Template_Id == 1) {
                            return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01.cshtml", model);
                        }
                        else if (model.CV_Template_Id == 2) {
                            return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate02.cshtml", model);
                        }
                    }
                    //}

                }
            }
            else {
                return Content("Không tìm thấy CV để hiển thị! Vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
            }
            return HttpNotFound();
        }
        //Thuan Nguyen - GET: View student's public CV
        [HttpGet]
        public ActionResult ViewPublicCV(int? id) {
            //Check data
            if (id != null) {
                //Get current user
                var currentUser = User.Identity.GetUserId();
                //Get current Company
                var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
                //Get public CV info: just CV is in public status
                var crrCV = db.CV_Publics.FirstOrDefault(x => x.Id == id && x.CV_Status == 2);
                //check cv
                if (crrCV == null) {
                    //Wrong Id or Status is not public
                    return Content("Không tìm thấy CV để hiển thị hoặc bạn không có quyền để xem! Vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
                }
                else {
                    //Get Student_Info
                    var student = db.Student_Info.FirstOrDefault(x => x.Id == crrCV.Student_Id);

                    //----------------------------------------------------
                    if (crrCV.CV_Content == null) {
                        //Not have CV content in db
                        return Content("Không thể tìm thấy CV! Vui lòng liên hệ bộ phận hỗ trợ để được giúp đỡ! Xin cảm ơn quý doanh nghiệp!!");
                    }
                    else {
                        //Get content of CV
                        var cvContent = JsonConvert.DeserializeObject<ApplyViewModel>(crrCV.CV_Content);
                        //Get current public id and status
                        ViewBag.PublicId = id;
                        ViewBag.CV_Status = 2;
                        //Binding data to viewModel to show to View
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
                            email = student.AspNetUser.UserName, //FIXME
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
                            //Template_Id = null -> defaul template is 01
                            return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01", model);
                        }
                        else {
                            //Fix Logo cache
                            Random rand = new Random();
                            ViewBag.Random = rand.Next(1, 1000);
                            //Check CV_Template
                            if (model.CV_Template_Id == 0) {
                                return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01", model);
                            }
                            else if (model.CV_Template_Id == 1) {
                                return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate01.cshtml", model);
                            }
                            else if (model.CV_Template_Id == 2) {
                                return PartialView(@"~/Views/Shared/CV_Templates/_CVTemplate02.cshtml", model);
                            }
                        }
                    }

                }
            }
            else {
                return Content("Không tìm thấy CV để hiển thị hoặc bạn không có quyền để xem! Vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
            }
            return HttpNotFound();
        }
        //Thuan Nguyen - GET: View List of Public CV
        [HttpGet]
        public ActionResult PublicCVs(int? faculty = null) {
            //Get list Public CVs
            var model = db.CV_Publics.Where(x => x.CV_Status == 2);
            //Fix Logo cache
            Random rand = new Random();
            ViewBag.Random = rand.Next(1, 1000);
            //Return View
            return View(model);
        }
        //Thuan Nguyen - POST: Change Interview Date
        //Parameter: Apply_Recruitment - id
        [HttpPost]
        public ActionResult ChangeInterviewDate(int? id, FormCollection formCollection) {

            if (id != null) {
                //get current row
                var currentApply = db.Apply_Recruitments.Find(id);
                //get OLD Date_Interview
                var oldInterviewDate = currentApply.Date_Interview;
                //Get Company Name
                var comName = currentApply.Recruitment.Company_Info.Name_Company.ToString();
                //Get Stu Info
                var stu = currentApply.CV_Info.Student_Info.Account_Id;
                //Get Recruitment Title
                var recTitle = currentApply.Recruitment.title.ToString();
                //Get Recruitment Id
                var recID = currentApply.Recruitment_ID;
                //Get Interview Datetime
                var strDatetime = formCollection["interviewDate"]; //String value
                var interviewDate = DateTime.Parse(strDatetime); //Parse to Datetime
                //Update row in db
                currentApply.Date_Interview = interviewDate;
                //Save
                db.Entry(currentApply).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() == 1) {
                    sm.SendEmail_ChangeDateInterview_toStudent(stu, comName, recID, recTitle, formCollection);
                    return RedirectToAction("Interviewing", "Candidate", new { area = "Company", @message = "ChangeDateSuccess" });
                }
                else {
                    return Content("Không thể lưu dữ liệu, vui lòng làm mới lại trang! Xin cảm ơn");
                }
            }
            else {
                return Content("Không tìm thấy Đơn ứng tuyển để thay đổi lịch! Vui lòng làm mới lại trang và thử lại! Xin cảm ơn!");

            }
        }
        //Thuan Nguyen - POST: Cancel Interview Date
        //Parameter: Apply_Recruitment - id
        [HttpPost]
        public ActionResult CancelInterviewDate(int? id, FormCollection formCollection) {

            if (id != null) {
                //Get current value
                var currentApply = db.Apply_Recruitments.Find(id);
                //Get Company Name
                var comName = currentApply.Recruitment.Company_Info.Name_Company.ToString();
                //Get Stu Info
                var stu = currentApply.CV_Info.Student_Info.Account_Id;
                //Get Recruitment Title
                var recTitle = currentApply.Recruitment.title.ToString();
                //Get Recruitment Id
                var recID = currentApply.Recruitment_ID;
                db.Apply_Recruitments.Remove(currentApply);
                if (db.SaveChanges() == 1) {
                    //Send Email
                    sm.SendEmail_CancelInterview_toStudent(stu, comName, recID, recTitle, formCollection);
                    return RedirectToAction("Interviewing", "Candidate", new { area = "Company", @message = "CancelDateSuccess" });
                }
                else {
                    return Content("Không thể lưu dữ liệu, vui lòng làm mới lại trang! Xin cảm ơn!");
                }
            }
            else {
                return Content("Không tìm thấy Đơn ứng tuyển để hủy! Vui lòng làm mới lại trang và thử lại! Xin cảm ơn!");
            }
        }

        //INTERVIEW PHASE
        //POST: Hire Candidate
        [HttpPost]
        public ActionResult HireCandidate(int id, FormCollection formCollection) {
            //Current Apply
            var crrApply = db.Apply_Recruitments.Find(id);
            if (crrApply != null) {
                //Get Company Name
                var comName = crrApply.Recruitment.Company_Info.Name_Company.ToString();
                //Get Stu Info
                var stu = crrApply.CV_Info.Student_Info.Account_Id;
                //Get Recruitment Title
                var recTitle = crrApply.Recruitment.title.ToString();
                //Get Recruitment Id
                var recID = crrApply.Recruitment_ID;
                //Convert data
                DateTime workDate = DateTime.Parse(formCollection["workDate"].ToString());

                //Update Status of current apply CV
                crrApply.CV_Status = 7; // Success Interview
                crrApply.Date_Complete = DateTime.Now;
                crrApply.Date_Work = workDate;
                crrApply.Address_Work = formCollection["workAddress"].ToString();
                crrApply.Student_Confirm = null;
                crrApply.Employer_Confirm = null;
                //
                db.Entry(crrApply).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //FIXME: Send email or notifications to Student!
                bool sendEmail = sm.SendEmail_Hire_toStudent(stu, comName, recID, recTitle, formCollection);
                //return
                return RedirectToAction("Interviewing", new { @message = "HireSuccess" });
            }
            else {
                return Content("Không tìm thấy dữ liệu! Vui lòng làm mới trang và thử lại! Xin cảm ơn!");
            }
        }
        //POST: Not Hire Candidate
        public ActionResult NotHireCandidate(int id) {
            //find candidate
            var crrApply = db.Apply_Recruitments.Find(id);
            if (crrApply != null) {
                //Get Company Name
                var comName = crrApply.Recruitment.Company_Info.Name_Company.ToString();
                //Get Stu Info
                var stu = crrApply.CV_Info.Student_Info.Account_Id;
                //Get Recruitment Title
                var recTitle = crrApply.Recruitment.title.ToString();
                //Get Recruitment Id
                var recID = crrApply.Recruitment_ID;
                //Send to Apply_Results
                //Update status of current apply CV
                crrApply.CV_Status = 8; // Fail
                crrApply.Date_Complete = DateTime.Now;

                //Send to DB
                db.Entry(crrApply).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //: Send email or notifications to Student!
                bool sendEmail = sm.SendEmail_NotHire_toStudent(stu, comName, recID, recTitle, crrApply.Date_Interview.ToString());
                //Return
                return RedirectToAction("Interviewing", new { @message = "NotHireSuccess" });
            }
            else {
                return Content("Không tìm thấy dữ liệu! Vui lòng làm mới trang và thử lại! Xin cảm ơn!");
            }
        }
        //GET: Company view List Hired Candidate
        [HttpGet]
        public ActionResult Hired(int status, string message = null) {
            //check status input
            if (status != 7 && status != 8) {
                return HttpNotFound();
            }
            else {
                //Crr user
                var crrUser = User.Identity.GetUserId();
                //Crr Company
                var crrCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == crrUser);
                //Fix Logo cache
                Random rand = new Random();
                ViewBag.Random = rand.Next(1, 1000);
                //model
                var model = new HiredViewModel();
                model.listHire = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == crrCom.Company_id && x.CV_Status == status).ToList();
                model.status = status;
                //Check message
                if (message == "CONFIRMED") {
                    ViewBag.Message = "CONFIRMED";
                }
                else if (message == "NOTCONFIRMED") {
                    ViewBag.Message = "NOTCONFIRMED";
                }
                return View(model);
            }
        }
        //POST: confirm worked
        public ActionResult confirmWork(int id, int decision) {
            var crrApply = db.Apply_Recruitments.Find(id);
            if (crrApply != null) {
                if (decision == 0) {
                    crrApply.Employer_Confirm = false;
                    db.Entry(crrApply).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Hired", new { @status = 7, @message = "NOTCONFIRMED" });
                }
                else if (decision == 1) {
                    crrApply.Employer_Confirm = true;
                    db.Entry(crrApply).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Hired", new { @status = 7, @message = "CONFIRMED" });
                }
                else {
                    return Content("Không thể tìm thấy dữ liệu! Vui lòng làm mới lại trang và thử lại!");
                }
            }
            else {
                return Content("Không thể tìm thấy dữ liệu! Vui lòng làm mới lại trang và thử lại!");
            }
        }
    }
}