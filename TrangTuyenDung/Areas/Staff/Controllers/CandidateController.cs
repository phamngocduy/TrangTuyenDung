using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TrangTuyenDung.Areas.Student.Models;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Staff.Controllers {
    [Authorize(Roles = "Staff")]
    public class CandidateController : Controller {
        //DB Entities
        EJobEntities db = new EJobEntities();
        // GET: Staff/Candidate
        public ActionResult Index() {
            return View();
        }
        //Thuan Nguyen - GET: List Candidate approving to Staff' Recs
        public ActionResult Pending() {
            //Get list student applying the recruitment was posted by Staff
            ViewBag.Message = TempData["shortMessage"];
            var model = db.Apply_Recruitments.ToList();
            return View(model);
        }
        public ActionResult Studentlist() {
            //Get list student applying the recruitment was posted by Staff

            var model = db.Student_Info.ToList();
            
            return View(model);
        }
        public ActionResult CVlist() {
            //Get list student applying the recruitment was posted by Staff

            var model = db.CV_Publics.ToList();
            return View(model);
        }
        public ActionResult Export_ListApply(string startDay,string endDay) {
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.Apply_Recruitments.Where(x => x.Date_Apply >= start && x.Date_Apply <= end).ToList();
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("MSSV"),
                    new DataColumn("Họ tên"),
                    new DataColumn("Vị trí ứng tuyển"),
                    new DataColumn("Công ty"),
                    new DataColumn("Trạng thái"),
                    new DataColumn("Ngày ứng tuyển") });
                foreach (var item in model) {

                    dt.Rows.Add(item.CV_Info.Student_Info.Student_Id, item.CV_Info.Student_Info.Student_Name,item.Recruitment.title,item.Recruitment.Company_Info.Name_Company,item.Status_CV.Name,item.Date_Apply.ToShortDateString());
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListStudentApply.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }
        public ActionResult Export_ListStudent(string startDay, string endDay) {
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.Student_Info.Where(x => x.Student_Create_at >= start && x.Student_Create_at <= end).ToList();
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("MSSV"),
                    new DataColumn("Lớp"),
                    new DataColumn("Họ tên"),
                    new DataColumn("Số điện thoại"),
                    new DataColumn("Ngày sinh"),
                    new DataColumn("địa chỉ"),
                    new DataColumn("Chuyên Ngành"),
                    new DataColumn("Ngày tham gia") });

                foreach (var item in model) {

                    dt.Rows.Add(item.Student_Id,item.Student_Class,item.Student_Name,item.Student_PhoneNumber,item.Student_Birthday,item.Student_Address,item.Faculty.Name,item.Student_Create_at);
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListStudent.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }
        public ActionResult Export_CVList(string startDay, string endDay) {
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.CV_Publics.Where(x => x.Date_Public >= start && x.Date_Public <= end).ToList();
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[9] { new DataColumn("MSSV"),
                    new DataColumn("Lớp"),
                    new DataColumn("Họ tên"),
                    new DataColumn("Giới thiệu"),
                    new DataColumn("Mục tiêu"),
                    new DataColumn("Kỹ năng"),
                    new DataColumn("Kinh nghiệm"),
                    new DataColumn("Dự án"),
                    new DataColumn("Chứng chỉ") });

                foreach (var item in model) {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic content = serializer.Deserialize<object>(item.CV_Content);
                    var g = content["CV_Info"]["Personal_Goal"];
                    var a= content["CV_Info"]["About_Student"];
                    var skill = content["Skill"][0]["Name"]+","+ content["Skill"][1]["Name"]+","+ content["Skill"][2]["Name"];
                    var intern = content["WE"][0]["Company_Name"] + "," + content["WE"][1]["Company_Name"] + "," + content["WE"][2]["Company_Name"];
                    var project = content["Proj"][0]["Name"] + "," + content["Proj"][1]["Name"] ;
                    var cer = content["Cer"][0]["Name"] + "," + content["Cer"][1]["Name"] + "," + content["Cer"][2]["Name"];

                    var introduce = RemoveHtmlTags(a);
                    var goal = RemoveHtmlTags(g);
                    dt.Rows.Add(item.Student_Info.Student_Id,item.Student_Info.Student_Class,item.Student_Info.Student_Name,introduce,goal,skill,intern,project,cer);
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListCV.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }
        public static string RemoveHtmlTags(string strHtml) {
            if (strHtml == "") {
                return strHtml;
            }
            string strText = Regex.Replace(strHtml, "<(.|\n)*?>", String.Empty);
            strText = HttpUtility.HtmlDecode(strText);
            strText = Regex.Replace(strText, @"\s+", " ");
            return strText;
        }

    }
}