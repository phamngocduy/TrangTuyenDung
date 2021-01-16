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
using System.Data.Entity.Validation;


namespace TrangTuyenDung.Areas.Student.Controllers {
    [Authorize(Roles = "Student")]
    public class IntershipController : Controller {
        EJobEntities db = new EJobEntities();

        public ActionResult Index() {
            return View();
        }
        [HttpGet]
        public ActionResult Create(string message = null, string id = null) {
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

            ViewBag.Companies = formatCompanyName;
            ViewBag.Level_Id = Level;
            ViewBag.Type_Inter = type;
            ViewBag.School_year = shoolyear;
            ViewBag.Semester_Inter = Semester;
            ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name");
            if (message == "Thành công") {
                ViewBag.Message = "Thành công";
                ViewBag.MaXacNhan = id;
            }
            if (TempData["data"] != null) {
                InternshipCreate model = (InternshipCreate)TempData["data"];

                ViewBag.School_year = new SelectList(shoolyear, "Value", "Text", model.School_year);
                ViewBag.Level_Id = new SelectList(Level, "Value", "Text", model.Level_Id);
                ViewBag.Semester_Inter = new SelectList(Semester, "Value", "Text", model.Semester_Inter);
                ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name", model.Major_Id);
                ViewBag.Type_Inter = new SelectList(type, "Value", "Text", model.Type_Inter);
                ViewBag.Company = new SelectList(db.Company_Info, "Id", "Company_Name");
                return View(model);
            }


            return View();
        }
        [HttpPost]
        public ActionResult Create(InternshipCreate data, string submit = null) {
            if (ModelState.IsValid) {
                if (submit == "Xem trước") {
                    TempData["data"] = data;
                    return RedirectToAction("Preview", "Intership", new { area = "Student" });
                }
                else {
                    if (data.Type_Inter != 2) {
                        data.internTypeOther = null;
                    }
                    Internship nInter = new Internship {
                        Student_Name = data.Student_Name,
                        MSSV = data.MSSV,
                        Level_Id = data.Level_Id,
                        Major_Id = data.Major_Id,
                        Company_Name = data.Company_Name,
                        CellPhone = data.CellPhone,
                        Email = data.Email,
                        create_day = DateTime.Now.Date,
                        status = 0,
                        Type_Inter = data.Type_Inter,
                        Semester_Inter = data.Semester_Inter,
                        Inter_from = data.Inter_from,
                        Inter_to = data.Inter_to,
                        School_year = data.School_year,
                        internTypeOther = data.internTypeOther
                    };

                    try {
                        db.Internships.Add(nInter);
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx) {
                        foreach (var validationErrors in dbEx.EntityValidationErrors) {
                            foreach (var validationError in validationErrors.ValidationErrors) {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }

                }//ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name");
                var id = db.Internships.Max(x => x.Id);
                TempData["data"] = null;
                return RedirectToAction("Create", "Intership", new { area = "Student", @message = "Thành công", @id = id });
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

                ViewBag.Companies = formatCompanyName;
                ViewBag.Level_Id = Level;
                ViewBag.Type_Inter = type;
                ViewBag.School_year = shoolyear;
                ViewBag.Semester_Inter = Semester;
                ViewBag.Major_Id = new SelectList(db.Faculties_Majors, "Id", "Name");
                return View(data);
            }


        }

        [HttpGet]
        public ActionResult Preview() {
            InternshipCreate value = (InternshipCreate)TempData["data"];
            InternshipCreate model = new InternshipCreate {
                Student_Name = value.Student_Name,
                Level_Id = value.Level_Id,
                Major_Id = value.Major_Id,
                MSSV = value.MSSV,
                Company_Name = value.Company_Name,
                CellPhone = value.CellPhone,
                Email = value.Email,
                Type_Inter = value.Type_Inter,
                internTypeOther = value.internTypeOther,
                Semester_Inter = value.Semester_Inter,
                Inter_from = value.Inter_from,
                Inter_to = value.Inter_to,
                School_year = value.School_year
            };
            var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == value.Major_Id);
            ViewBag.Major = major.Name;
            TempData["data"] = value;
            return View(model);
        }
        [HttpPost]
        public ActionResult PreviewCreate() {
            InternshipCreate value = (InternshipCreate)TempData["data"];
            if (value.Type_Inter != 2) {
                value.internTypeOther = null;
            }
            Internship model = new Internship {
                Student_Name = value.Student_Name,
                Level_Id = value.Level_Id,
                Major_Id = value.Major_Id,
                MSSV = value.MSSV,
                Company_Name = value.Company_Name,
                CellPhone = value.CellPhone,
                Email = value.Email,
                Type_Inter = value.Type_Inter,
                Semester_Inter = value.Semester_Inter,
                Inter_from = value.Inter_from,
                Inter_to = value.Inter_to,
                School_year = value.School_year,
                create_day = DateTime.Now.Date,
                status = 0,
                internTypeOther = value.internTypeOther
            };
            db.Internships.Add(model);
            db.SaveChanges();
            var id = db.Internships.Max(x => x.Id);
            TempData["data"] = null;
            return RedirectToAction("Create", "Intership", new { area = "Student", @message = "Thành công", @id = id });
        }
    }
}