using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using ClosedXML.Excel;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;

namespace TrangTuyenDung.Areas.Staff.Controllers {
    [Authorize(Roles = "Staff")]


    public class ConfirmInternController : Controller {
        EJobEntities db = new EJobEntities();

        public ActionResult List(string message =null) {
            if(message == "Fail") {
                ViewBag.Message = "Fail";
            }
            var model = db.confirmation_intern.OrderByDescending(x => x.id).ToList();
            return View(model);
        }

        public ActionResult Preview(int id) {
            var data = db.confirmation_intern.FirstOrDefault(x => x.id == id);
            var student_district = db.Districts.FirstOrDefault(x => x.Id == data.student_district_id);
            var student_province = db.Provinces.FirstOrDefault(x => x.Id == data.student_province_id);
            var org_district = db.Districts.FirstOrDefault(x => x.Id == data.student_district_id);
            var org_province = db.Provinces.FirstOrDefault(x => x.Id == data.student_province_id);
            var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == data.major_id);
            var faculty = db.Faculties.FirstOrDefault(x => x.Id == data.faculty_id);
            ViewBag.student_dictrict = student_district.Type + " " + student_district.District_Name;
            ViewBag.org_dictrict = org_district.Type + " " + org_district.District_Name;
            ViewBag.student_provinces = student_province.Type + " " + student_province.Name;
            ViewBag.org_provinces = org_province.Type + " " + org_province.Name;
            ViewBag.major = major.Name;
            ViewBag.faculty = faculty.Name;
            return View(data);
        }

        [HttpPost]
        public ActionResult Export(string startDay, string endDay) {
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.confirmation_intern.Where(x => x.create_day >= start && x.create_day <= end).OrderByDescending(x => x.id).ToList();

          if(model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[33] {new DataColumn("Ngày tạo"),
                                            new DataColumn("Tên sinh viên"),
                                            new DataColumn("MSSV"),
                                            new DataColumn("Lớp"),
                                            new DataColumn("Bậc đào tạo"),
                                            new DataColumn("Khoa"),
                                            new DataColumn("Ngành học"),
                                            new DataColumn("Năm học"),
                                            new DataColumn("Số điện thoại"),
                                            new DataColumn("Email"),
                                            new DataColumn("Địa chỉ sinh viên"),
                                            new DataColumn("Hình thức thực tập"),
                                            new DataColumn("Học kỳ thực tập"),
                                            new DataColumn("Vị trí thực tập"),
                                            new DataColumn("Phòng ban"),
                                            new DataColumn("Thực tập từ"),
                                            new DataColumn("Thực tập đến"),
                                            new DataColumn("Lịch thực tập"),
                                            new DataColumn("Tên Công ty"),
                                            new DataColumn("Website"),
                                            new DataColumn("Tax"),
                                            new DataColumn("Số điện thoại doanh nghiệp"),
                                            new DataColumn("Địa chỉ doanh nghiệp"),
                                            new DataColumn("Tên nhân sự tiếp nhân thực tập"),
                                            new DataColumn("Chức vụ của nhân sự tiếp nhận thực tập"),
                                            new DataColumn("Số điện thoại bàn của nhân sự tiếp nhận thực tập"),
                                            new DataColumn("Email của nhân sự tiếp nhận thực tập"),
                                            new DataColumn("Số di động của nhân sự tiếp nhận thực tập"),
                                            new DataColumn("Tên nhân sự hướng dẫn thực tập"),
                                            new DataColumn("Chức vụ của nhân sự hướng dẫn thực tập"),
                                            new DataColumn("Số điện thoại bàn của nhân sự hướng dẫn thực tập"),
                                            new DataColumn("Email của nhân sự hướng dẫn thực tập"),
                                            new DataColumn("Số điện thoại di động của nhân sự hướng dẫn")});

                foreach (var intern in model) {
                    var level = "Cao đẳng";
                    var semester = "";
                    //student address
                    var stu_dict = db.Districts.FirstOrDefault(x => x.Id == intern.student_district_id);
                    var stu_pro = db.Provinces.FirstOrDefault(x => x.Id == intern.student_province_id);
                    var student_add = intern.student_street+"," + "Phường " + intern.student_ward+","+ stu_dict.Type+" "+ stu_dict.District_Name +","+ stu_pro.Type+" "+stu_pro.Name;
  
                
                    JArray arr = JArray.Parse(intern.schedule_work);

                    var result = "";
                    var jsonResponse = JToken.Parse("[]");
                    foreach (var schedule in arr) {
                        var day = schedule["day"];
                        if (day.HasValues == true) {
                            var workingTime = "Thời gian làm việc: ";
                            
                            var startTime = schedule["start"];
                            var endTime = schedule["end"];

                            foreach (var item in day) {
                                workingTime += "Thứ " + item + ",";
                            }

                            workingTime = workingTime.Substring(0, workingTime.Length - 1);
                            result += workingTime +" bắt đầu vào lúc : "+startTime + " và kết thúc vào lúc : "+endTime+ ",";
                        }
                    }
                    var schedule_word = "";
                    if (result != "") {
                        schedule_word = result.Substring(0, result.Length - 1);
                    }
                   

                   
                    // org address
                    var org_dict = db.Districts.FirstOrDefault(x => x.Id == intern.org_district_id);
                    var org_pro = db.Provinces.FirstOrDefault(x => x.Id == intern.org_province_id);
                    var org_add = intern.org_street +","+"Phường " +intern.org_ward +","+org_dict.Type+" "+org_dict.District_Name +","+org_pro.Type+" "+ org_pro.Name;
                    var faculty = db.Faculties.FirstOrDefault(x => x.Id == intern.faculty_id);
                    var interntype = intern.other_type_intern;
                    var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == intern.major_id);
                    // check level
                    if (intern.level_id == 0) {
                        level = "Đại học";
                    }
                    //check semester intern
                    if (intern.semester == 0) {
                        semester = "HK 1";
                    }
                    else if (intern.semester == 1) {
                        semester = "HK 2";
                    }
                    else {
                        semester = "HK hè";
                    }
                    //check intern type
                    if (intern.type_intern == 0) {
                        interntype = "Thực tập tốt nghiệp";
                    }
                    else if (intern.type_intern == 1) {
                        interntype = "Thực tập nghề nghiệp";
                    }

                    dt.Rows.Add(intern.create_day,intern.student_name, intern.student_id, intern.student_class, level, faculty.Name, major.Name, intern.school_year, intern.cellphone, intern.student_email, student_add, interntype, semester, intern.position, intern.department, intern.start_day, intern.end_day, schedule_word, intern.organization_name, intern.website, intern.tax, intern.org_telephone, org_add, intern.human_resource_name, intern.human_resource_position, intern.human_resource_position_tel, intern.human_resource_email, intern.human_resource_cel, intern.supervisor_name, intern.supervisor_position, intern.supervisor_tel, intern.supervisor_email, intern.supervisor_cellphone);

                }

                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PTNTT.xlsx");
                    }
                }
            }
            return HttpNotFound();
          
           
        }
    }
}