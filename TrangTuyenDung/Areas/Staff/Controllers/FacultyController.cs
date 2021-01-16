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
    public class FacultyController : Controller {
        //Entites
        EJobEntities db = new EJobEntities();
        // GET: Staff/Faculty
        public ActionResult Index() {
            return View();
        }
        public ActionResult FacultiesList(string status) {
            //Generate List Faculties
            var model = db.Faculties.ToList();

            //list faculty majors 
            var listFaculty = db.Faculties.Select(x => x.Name);
            string formatToString = "";
            foreach (var item in listFaculty) {
                formatToString += item + "|";
            }

            //Check status to show message
            if (status == "AddSuccess") {
                ViewBag.Message = "AddSuccess";
            }
            else if (status == "AddFail") {
                ViewBag.Message = "AddFail";
            }
            else if (status == "DeleteSuccess") {
                ViewBag.Message = "DeleteSuccess";
            }
            else if (status == "AddMajorSuccess") {
                ViewBag.Message = "AddMajorSuccess";
            }
            else if (status == "AddMajorFail") {
                ViewBag.Message = "AddMajorFail";
            }
            else if (status == "DeleteMajorSuccess") {
                ViewBag.Message = "DeleteMajorSuccess";
            }

            ViewBag.listFaculty = formatToString;
            return View(model);
        }
        public ActionResult AddFaculty(Faculty data) {
            if (ModelState.IsValid) {
                //Check Input value
                if (data.Name == "" || data.Name == null) {
                    return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "AddFail" });
                }
                //Add new record
                var model = new Faculty {
                    Name = data.Name,
                    Date_Created = DateTime.Now
                };
                db.Faculties.Add(model);
                db.SaveChanges();
                //Return
                return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "AddSuccess" });
            }
            return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "AddFail" });
        }
        public ActionResult DeleteFaculty(int? id) {
            if (id != null) {
                //Find faculty in db
                var record = db.Faculties.Find(id);
                if (record != null) {
                    var majorList = db.Faculties_Majors.Where(x => x.Faculty_Id == id);
                    foreach (var item in majorList) {
                        db.Faculties_Majors.Remove(item);
                    }
                    //Delete that row
                    db.Faculties.Remove(record);
                    db.SaveChanges();
                    return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "DeleteSuccess" });
                }
                else {
                    throw new Exception("Not Found!");
                }
            }
            else {
                throw new Exception("Not Found!");
            }
        }

        [HttpGet]
        public ActionResult ListJob_Position(string status) {
            //Generate List Faculties
            var model = db.Job_Positions.ToList();
            var Faculty_Major = db.Faculties_Majors.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            liFaculty.Add(new SelectListItem { Text = "--Chọn Ngành--", Value = "0", Disabled = true, Selected = true });

            foreach (var fac in db.Faculties) {
                SelectListGroup group = new SelectListGroup() { Name = fac.Name };
                foreach (var item in Faculty_Major.Where(x => x.Faculty_Id == fac.Id)) {
                    //Add to ViewBag
                    liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Group = group });

                }
            }


            ViewBag.Major_Id = liFaculty;
            //Check status to show message
            if (status == "AddSuccess") {
                ViewBag.Message = "AddSuccess";
            }
            else if (status == "AddFail") {
                ViewBag.Message = "AddFail";
            }
            else if (status == "DeleteSuccess") {
                ViewBag.Message = "DeleteSuccess";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AddJob_Postion(ManageJob_Position data) {
            if (ModelState.IsValid) {
                //Check Input value
                //if (data.Name == "" || data.Name == null) {
                //    return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "AddFail" });
                //}
                //if (data.Major_Id != 0) {
                //    //Add new record
                //    var model = new Job_Positions {
                //        Name = data.Name,
                //        Major_Id = data.Major_Id,
                //        Date_Create = DateTime.Now,
                //    };
                //    db.Job_Positions.Add(model);
                //    db.SaveChanges();
                //    return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "AddSuccess" });
                //}

                if (data.Major_Id != null) {
                    foreach (var item in data.Major_Id) {
                        db.Job_Positions.Add(new Job_Positions {
                            Name = data.Name,
                            Major_Id = item,
                            Date_Create = DateTime.Now,
                        });
                    }
                    db.SaveChanges();
                    return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "AddSuccess" });
                }

                return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "AddFail" });
            }
            return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "AddFail" });
        }

        public ActionResult DeleteJob_Postion(int id) {
            //Find faculty in db
            var record = db.Job_Positions.Find(id);
            //Delete that row
            db.Job_Positions.Remove(record);
            db.SaveChanges();
            return RedirectToAction("ListJob_Position", "Faculty", new { area = "Staff", @status = "DeleteSuccess" });
        }

        public ActionResult AddNewMajor(Faculties_Majors data) {
            //add data
            if (string.IsNullOrEmpty(data.Name) || string.IsNullOrWhiteSpace(data.Name)) {
                return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "AddMajorFail" });
            }
            else {
                var NewMajor = new Faculties_Majors() {
                    Name = data.Name,
                    Faculty_Id = data.Faculty_Id,
                    Date_Create = DateTime.Now
                };
                db.Faculties_Majors.Add(NewMajor);
                db.SaveChanges();
                return RedirectToAction("FacultiesList", "Faculty", new { area = "Staff", @status = "AddMajorSuccess" });
            }
        }
        //Thuan Nguyen - POST: Delete Major
        public ActionResult DeleteMajor(int? id) {
            if (id != null) {
                //Get current major row in db
                var model = db.Faculties_Majors.FirstOrDefault(x => x.Id == id);
                //Check value
                if (model != null) {
                    //Delete
                    db.Faculties_Majors.Remove(model);
                    db.SaveChanges();
                    return RedirectToAction("FacultiesList", new { @status = "DeleteMajorSuccess" });
                }
                else {
                    throw new Exception("Not Found!");
                }

            }
            else {
                throw new Exception("Not Found!");
            }

        }
        public ActionResult Export_Faculties() {

            var model = db.Faculties.ToList();
            string major_list="";
            var list = "";
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Tên Khoa"),
                                            new DataColumn("Chuyên Ngành"),                                           
                                            new DataColumn("Ngày tạo")});
                foreach (var fal in model) {
                    var major = db.Faculties_Majors.Where(x => x.Faculty_Id == fal.Id);
                   foreach(var item in major) {
                        major_list += item.Name + ",";
                    }
                    if (major_list != "") {
                        list = major_list.Substring(0, major_list.Length - 1);
                    }
                    dt.Rows.Add(fal.Name, list, fal.Date_Created);
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Faculties.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }

        public ActionResult Export_JobPosition() {

            var model = db.Job_Positions.ToList();
            string major_list = "";
            var list = "";
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[2] { new DataColumn("Tên vị trí ứng tuyển"),                                        new DataColumn("Thuộc chuyên ngành")});
                foreach (var job in model) {
                   
                    dt.Rows.Add(job.Name, job.Faculties_Majors.Name );
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Job.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }

    }
}