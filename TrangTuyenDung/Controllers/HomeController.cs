using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Controllers {

    

    [AllowAnonymous]
    public class HomeController : Controller {
        private SelectList provinces;
        private SelectList faculties;

        public HomeController() {
            EJobEntities db = new EJobEntities();
            provinces = new SelectList(db.Provinces, "Id", "Name");
            faculties = new SelectList(db.Faculties, "Id", "Name");
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));

        [HandleError]
        public ActionResult Index() {
            log.Info(Request.UserHostAddress);
            EJobEntities db = new EJobEntities();
            var model = db.Company_Info.Where(x => x.Status == 1).OrderByDescending(x => x.Is_hot_Company).Take(12).ToList();
            var recruiment = db.Recruitments.Count();
            var stu = db.Student_Info.Count();
            var company = db.Company_Info.Count();
            ViewBag.com = recruiment;
            ViewBag.mem = stu;
            ViewBag.count = company;
            ViewBag.city = this.provinces;
            ViewBag.fac = this.faculties;
            return View(model);
        }
        //TEST - 404 Page
        public ActionResult Index1() {
            throw new Exception("Có gì đó sai sai!");
        }

        public ActionResult IndexCallBack() {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        public ActionResult Logos(int? id) {

            //var logo = Path.Combine("~/Content/Img/Company", id + "png");
            //return File(logo,"image/png");
            var dir = Server.MapPath("~/Areas/Company/Images/Logos");
            string path;
            if (id != null) {
                path = Path.Combine(dir, id + ".png"); //validate the path for security or use other means to generate the path.
            }
            else {
                path = Path.Combine(dir, "vlu-logo.png");
            }
            var theFile = new FileInfo(path);
            if (theFile.Exists) {
                return base.File(path, "image/png");
            }
            return this.HttpNotFound();
        }

        [ChildActionOnly]
        public ActionResult LeftMenu() {
            EJobEntities db = new EJobEntities();
            var User_Id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var User = db.Student_Info.FirstOrDefault(x => x.Account_Id == User_Id);
            var day = DateTime.Now;
            var model = db.Majors_In_Recruitments.Where(x => x.Recruitment.Status_id == 2 && x.Recruitment.Is_Show == true && x.Recruitment.Expire_date >= day).OrderByDescending(x => x.Recruitment.Id).ToList();
            if (User != null) {
                ViewBag.Name = User.Student_Name;
                ViewBag.Faculty = User.Faculty.Name;
                //check student have cv or not
                var CV = db.CV_Info.FirstOrDefault(x => x.Student_Id == User.Id);
                if (CV != null) {
                    ViewBag.CV_Id = CV.ID;
                }
                model = model.Where(x => x.Faculties_Majors.Faculty_Id == User.Faculties_Id).ToList();
                //For clear cache image logo
                Random rand = new Random();
                ViewBag.Random = rand.Next(1, 1000);
            }

            return PartialView("_LeftMenu", model);
        }

        [ChildActionOnly]
        public ActionResult RightMenu() {
            EJobEntities db = new EJobEntities();
            var day = DateTime.Now;
            var model = db.Recruitments.Where(x => x.Status_id == 2 && x.Is_Show == true && x.Expire_date >= day).OrderByDescending(x => x.Nums_view).ToList();
            return PartialView("_RightMenu", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 86400)]
        public ActionResult partialJobFilter() {
            EJobEntities db = new EJobEntities();
            //dropdown list
            ViewBag.city = new SelectList(db.Provinces, "Id", "Name");
            ViewBag.fac = new SelectList(db.Faculties, "Id", "Name");
            return PartialView("_partialJobFilter");
        }

        [ChildActionOnly]
        public ActionResult LayoutHeader() {
            EJobEntities db = new EJobEntities();
            var link = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            ViewBag.Link = "";
            if (link.EmailSubject != "") {
                ViewBag.Link = link.EmailSubject;
            }
            return PartialView("_LayoutHeader");
        }

        public ActionResult ListCompany() {
            return View();
        }

        //Thuan Nguyen - GET: Profile Company
        [HttpGet]
        public ActionResult Company(int id) {
            EJobEntities db = new EJobEntities();
            //Get Company Info
            var model = db.Company_Info.FirstOrDefault(x => x.Id == id);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ShowListAllCompany() {
            EJobEntities db = new EJobEntities();
            var model = db.Company_Info.ToList();
            return PartialView("_partialCompany", model);
        }

        [ChildActionOnly]
        public ActionResult ShowListMajorCompany() {
            EJobEntities db = new EJobEntities();
            var User_Id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var User = db.Student_Info.FirstOrDefault(x => x.Account_Id == User_Id);
            var model = db.Faculties_In_Companies.Where(x => x.Faculty_Id == User.Faculties_Id).Select(x => x.Company_Info);
            return PartialView("_partialCompany", model);
        }

        public bool sendEmail(string to, string from, string subject, string content) {
            /*Notes:
             - to: truyền vào địa chỉ email người nhận
             - from: truyền vào địa chỉ email người gửi
             - subject: truyền vào tiêu đề email
             - content: truyền vào nội dung email, em có thể dùng dạng html, ví dụ: <b>Hello</b> thì nội dung email gửi tới ngta sẽ là chữ Hello in đậm
             */

            try {
                //Email Config - Khu vực cấu hình nội dung email sẽ gửi
                MailMessage mail = new MailMessage();
                mail.To.Add(to); //
                mail.From = new MailAddress(from);
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true; //Cái này bật true để nội dung mail được hiển thị theo html

                //SMTP Config - Khu vực cấu hình server gửi mail, ở đây mình dùng SMTP của gmail
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("<taikhoan@gmail.com>", "<password>"); //Chỗ này là tài khoản và mật khẩu email gửi đi.
                client.EnableSsl = true;
                client.Send(mail);

                return true;

            }
            catch (Exception ex) {
                return false;

            }
        }

        public ActionResult doSomething() {

            var result = sendEmail("attendance@gmail.com", "from@gmail.com", "Đây là tiêu đề email", "Đây là nội dung email");
            //Kiểm tra kết quả gửi
            if (result == true) {
                //Thông báo email gửi thành công
            }
            else {
                //Email gửi thất bại
            }

            return View();
        }

        [ChildActionOnly]
        public ActionResult HotCompany() {
            EJobEntities db = new EJobEntities();
            var model = db.Company_Info.ToList();
            return PartialView("_partialHotCompany", model);
        }

    }
}