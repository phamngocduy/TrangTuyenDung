using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace TrangTuyenDung.Areas.Staff.Controllers {
    [Authorize(Roles = "Staff")]
    public class InternshipController : Controller {
        EJobEntities db = new EJobEntities();
        public ActionResult List(string message = null, string id = null) {
            if (message == "Success") {
                ViewBag.Message = "Success";
            }
            if (id != null) {
                ViewBag.id = id;
            }
            var model = db.Internships.Where(x=>x.status ==0 ).OrderByDescending(x => x.Id).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id) {
            var model = db.Internships.FirstOrDefault(x => x.Id == id);
            if (model == null) {
                return HttpNotFound();
            }
            else {
                int year = DateTime.Now.Year;
                var current = year.ToString();
                var next = (year + 1).ToString();
                var nnext = (year + 2).ToString();

                var Level = new[] { new SelectListItem { Text = "Đại học", Value = "0" }, new SelectListItem { Text = "Cao đẳng", Value = "1" } };

                var Semester = new[] { new SelectListItem { Text = "Học kì 1", Value = "0" }, new SelectListItem { Text = " Học kì 2", Value = "1" }, new SelectListItem { Text = "Học kì hè", Value = "2" } };

                //0 = thực tập tốt nghiệp
                //1 = Thực tập nghề nghiệp
                //2 = Khác
                var type = new[] { new SelectListItem { Text = "Thực tập tốt nghiệp", Value = "0" }, new SelectListItem { Text = "Thực tập nghề nghiệp", Value = "1" }, new SelectListItem { Text = "Khác", Value = "2" } };
                var shoolyear = new[] { new SelectListItem { Text = current + "-" + next, Value = current + "-" + next }, new SelectListItem { Text = next + "-" + nnext, Value = next + "-" + nnext } };

                //list company name for autocomplete
                var listCompanyName = db.Company_Info.Select(x => x.Name_Company).ToList();
                string formatCompanyName = "";
                foreach (var item in listCompanyName) {
                    formatCompanyName += item + "|";
                }
                var data = new InternshipCreate {
                    Student_Name = model.Student_Name,
                    MSSV = model.MSSV,
                    Level_Id = model.Level_Id,
                    Major_Id = model.Major_Id,
                    Company_Name = model.Company_Name,
                    CellPhone = model.CellPhone,
                    Email = model.Email,
                    Type_Inter = model.Type_Inter,
                    Semester_Inter = model.Semester_Inter,
                    Inter_from = model.Inter_from,
                    Inter_to = model.Inter_to,
                    School_year = model.School_year,
                    internTypeOther = model.internTypeOther

                };

                ViewBag.Companies = formatCompanyName;
                ViewBag.School_year = new SelectList(shoolyear, "Value", "Text", data.School_year);
                ViewBag.Level_Id = new SelectList(Level, "Value", "Text", model.Level_Id);
                ViewBag.Semester_Inter = new SelectList(Semester, "Value", "Text", model.Semester_Inter);
                ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name", model.Major_Id);
                ViewBag.Type_Inter = new SelectList(type, "Value", "Text", model.Type_Inter);
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult Edit(InternshipCreate data) {
            if (!ModelState.IsValid) {
                int year = DateTime.Now.Year;
                var current = year.ToString();
                var next = (year + 1).ToString();
                var nnext = (year + 2).ToString();

                var Level = new[] { new SelectListItem { Text = "Đại học", Value = "0" }, new SelectListItem { Text = "Cao đẳng", Value = "1" } };

                var Semester = new[] { new SelectListItem { Text = "Học kì 1", Value = "0" }, new SelectListItem { Text = " Học kì 2", Value = "1" }, new SelectListItem { Text = "Học kì hè", Value = "2" } };

                //0 = thực tập tốt nghiệp
                //1 = Thực tập nghề nghiệp
                //2 = Khác
                var type = new[] { new SelectListItem { Text = "Thực tập tốt nghiệp", Value = "0" }, new SelectListItem { Text = "Thực tập nghề nghiệp", Value = "1" }, new SelectListItem { Text = "Khác", Value = "2" } };
                var shoolyear = new[] { new SelectListItem { Text = current + "-" + next, Value = current + "-" + next }, new SelectListItem { Text = next + "-" + nnext, Value = next + "-" + nnext } };

                //list company name for autocomplete
                var listCompanyName = db.Company_Info.Select(x => x.Name_Company).ToList();
                string formatCompanyName = "";
                foreach (var item in listCompanyName) {
                    formatCompanyName += item + "|";
                }
                ViewBag.School_year = new SelectList(shoolyear, "Value", "Text", data.School_year);
                ViewBag.Level_Id = new SelectList(Level, "Value", "Text", data.Level_Id);
                ViewBag.Semester_Inter = new SelectList(Semester, "Value", "Text", data.Semester_Inter);
                ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name", data.Major_Id);
                ViewBag.Type_Inter = new SelectList(type, "Value", "Text", data.Type_Inter);
                ViewBag.Company = new SelectList(db.Company_Info, "Id", "Company_Name");
                return View(data);
            }
            else {
                var interUpdate = db.Internships.FirstOrDefault(x => x.Id == data.Id);
                if (data.Type_Inter != 2) {
                    data.internTypeOther = null;
                }
                if (interUpdate != null) {
                    interUpdate.Student_Name = data.Student_Name;
                    interUpdate.MSSV = data.MSSV;
                    interUpdate.Level_Id = data.Level_Id;
                    interUpdate.Major_Id = data.Major_Id;
                    interUpdate.Email = data.Email;
                    interUpdate.CellPhone = data.CellPhone;
                    interUpdate.Company_Name = data.Company_Name;
                    interUpdate.Semester_Inter = data.Semester_Inter;
                    interUpdate.Type_Inter = data.Type_Inter;
                    interUpdate.internTypeOther = data.internTypeOther;
                    interUpdate.Inter_from = data.Inter_from;
                    interUpdate.Inter_to = data.Inter_to;
                    interUpdate.School_year = data.School_year;
                }
                db.SaveChanges();
                return RedirectToAction("List", "Internship", new { area = "Staff", @message = "Success", @id = data.Id });
            }
        }

        [HttpGet]
        public ActionResult Preview(int id) {
            var model = db.Internships.FirstOrDefault(x => x.Id == id);
            return View(model);
        }
        public ActionResult Confirm(int id) {
            var updateInter = db.Internships.FirstOrDefault(x => x.Id == id);
            if (updateInter.status == 0) {
                updateInter.status = 1;
            }
            else {
                updateInter.status = 0;
            }

            db.SaveChanges();
            return RedirectToAction("List", "Internship", new { area = "Staff" });
        }

        [HttpPost]
        public ActionResult Export(string startDay,string endDay) {
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.Internships.Where(x => x.create_day >= start && x.create_day <= end).OrderByDescending(x=>x.Id).ToList();
            if(model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[13] {new DataColumn("Ngày tạo"),
                                            new DataColumn("Tên sinh viên"),
                                            new DataColumn("MSSV"),
                                            new DataColumn("Bậc đào tạo"),
                                            new DataColumn("Ngành học"),
                                            new DataColumn("Số điện thoại"),
                                            new DataColumn("Email"),
                                            new DataColumn("Tên Công ty"),
                                            new DataColumn("Hình thức thực tập"),
                                            new DataColumn("Học kỳ thực tập"),
                                            new DataColumn("Thực tập từ"),
                                            new DataColumn("Thực tập đến"),
                                            new DataColumn("Năm học") });

                foreach (var intern in model) {
                    var level = "Cao đẳng";
                    var semester = "";
                    var interntype = intern.internTypeOther;
                    var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == intern.Major_Id);
                    // check level
                    if (intern.Level_Id == 0) {
                        level = "Đại học";
                    }
                    //check semester intern
                    if (intern.Semester_Inter == 0) {
                        semester = "HK 1";
                    }
                    else if (intern.Semester_Inter == 1) {
                        semester = "HK 2";
                    }
                    else {
                        semester = "HK hè";
                    }
                    //check intern type
                    if (intern.Type_Inter == 0) {
                        interntype = "Thực tập tốt nghiệp";
                    }
                    else if (intern.Type_Inter == 1) {
                        interntype = "Thực tập nghề nghiệp";
                    }
                    dt.Rows.Add(intern.create_day,intern.Student_Name, intern.MSSV, level, major.Name, intern.CellPhone, intern.Email, intern.Company_Name, interntype, semester, intern.Inter_from, intern.Inter_to, intern.School_year);
                }

                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GTGTT.xlsx");
                    }
                }
            }
            return HttpNotFound();
            //return View(model);
        }
    }
}