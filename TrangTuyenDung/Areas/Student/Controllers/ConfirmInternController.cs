using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using Microsoft.AspNet.Identity;

namespace TrangTuyenDung.Areas.Student.Controllers {
    [Authorize(Roles = "Student")]

    public class ConfirmInternController : Controller {
        EJobEntities db = new EJobEntities();

        // GET: Student/ConfirmIntern
        public ActionResult Create(string message = null) {

            //
            if (message == "success") {
                ViewBag.Message = "success";
            }
            if (message == "create") {
                ViewBag.Message = "create";
            };
            //Generate List of shoolyear to ViewBag
            int year = DateTime.Now.Year;
            var current = year.ToString();
            var next = (year + 1).ToString();
            var nnext = (year + 2).ToString();
            var school_year = new[] { new SelectListItem { Text = current + "-" + next, Value = current + "-" + next }, new SelectListItem { Text = next + "-" + nnext, Value = next + "-" + nnext } };

            //Generate List of Type Intern to ViewBag
            var type_intern = new[] { new SelectListItem { Text = "Thực tập tốt nghiệp", Value = "0" }, new SelectListItem { Text = "Thực tập nghề nghiệp", Value = "1" }, new SelectListItem { Text = "Khác", Value = "2" } };

            //Generate List of Districts to ViewBag
            var district = db.Districts.Select(x => new {
                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            });

            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            foreach (var item in province) {
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            //Generate List of faculty to ViewBag
            var Faculty = db.Faculties.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            foreach (var item in Faculty) {
                liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            //Generate List of Level to ViewBag
            var Level = new[] { new SelectListItem { Text = "Đại học", Value = "0" }, new SelectListItem { Text = "Cao đẳng", Value = "1" } };

            //Generate List of Semester to ViewBag
            var semester_intern = new[] { new SelectListItem { Text = "Học kì 1", Value = "0" }, new SelectListItem { Text = " Học kì 2", Value = "1" }, new SelectListItem { Text = "Học kì hè", Value = "2" } };
            // check tempdata 
            if (TempData["data"] != null) {
                confirmation_intern model = (confirmation_intern)TempData["data"];
                ViewBag.school_year = new SelectList(school_year, "Value", "Text", model.school_year);
                ViewBag.type_intern = new SelectList(type_intern, "Value", "Text", model.type_intern);
                ViewBag.semester = new SelectList(semester_intern, "Value", "Text", model.semester);
                ViewBag.level_id = new SelectList(Level, "Value", "Text", model.level_id);
                ViewBag.faculty_id = new SelectList(liFaculty, "Value", "Text", model.faculty_id);
                ViewBag.province = new SelectList(liProvinces, "Value", "Text", model.faculty_id);
                ViewBag.major_id = new SelectList(db.Faculties_Majors, "Id", "Name", model.major_id);

                ViewBag.student_district_id = new SelectList(district, "ID", "District_Name", model.student_district_id);
                ViewBag.student_province_id = new SelectList(liProvinces, "Value", "Text", model.student_province_id);
                ViewBag.org_district_id = new SelectList(district, "ID", "District_Name", model.org_district_id);
                ViewBag.org_province_id = new SelectList(liProvinces, "Value", "Text", model.org_province_id);
                return View(model);
            }

            ViewBag.school_year = school_year;
            ViewBag.type_intern = type_intern;
            ViewBag.semester = semester_intern;
            ViewBag.level_id = Level;
            ViewBag.faculty_id = liFaculty;
            ViewBag.province = liProvinces;
            ViewBag.major_id = new SelectList(db.Faculties_Majors, "Id", "Name");
            ViewBag.student_district_id = new SelectList(district, "ID", "District_Name");
            ViewBag.student_province_id = liProvinces;
            ViewBag.org_district_id = new SelectList(district, "ID", "District_Name");
            ViewBag.org_province_id = liProvinces;

            return View();
        }
        [HttpPost]

        public ActionResult Create(confirmation_intern data, string submit = null) {
            var account_id = User.Identity.GetUserId();
            if (ModelState.IsValid) {
                if (submit == "Xem trước") {
                    TempData["data"] = data;
                    return RedirectToAction("Preview", "ConfirmIntern", new { area = "Student" });
                }
                if (data.type_intern != 2) {
                    data.other_type_intern = null;
                }
                confirmation_intern intern = new confirmation_intern {
                    student_name = data.student_name,
                    student_class = data.student_class,
                    student_id = data.student_id,
                    major_id = data.major_id,
                    faculty_id = data.faculty_id,
                    level_id = data.level_id,
                    student_email = data.student_email,
                    cellphone = data.cellphone,
                    student_street = data.student_street,
                    student_ward = data.student_ward,
                    student_district_id = data.student_district_id,
                    student_province_id = data.student_province_id,
                    type_intern = data.type_intern,
                    other_type_intern = data.other_type_intern,
                    semester = data.semester,
                    school_year = data.school_year,
                    position = data.position,
                    department = data.department,
                    start_day = data.start_day,
                    end_day = data.end_day,
                    schedule_work = data.schedule_work,
                    organization_name = data.organization_name,
                    website = data.website,
                    tax = data.tax,
                    org_telephone = data.org_telephone,
                    org_street = data.org_street,
                    org_ward = data.org_ward,
                    org_district_id = data.org_district_id,
                    org_province_id = data.org_province_id,
                    human_resource_name = data.human_resource_name,
                    human_resource_position = data.human_resource_position,
                    human_resource_email = data.human_resource_email,
                    human_resource_cel = data.human_resource_cel,
                    human_resource_position_tel = data.human_resource_position_tel,
                    supervisor_name = data.supervisor_name,
                    supervisor_position = data.supervisor_position,
                    supervisor_email = data.supervisor_email,
                    supervisor_cellphone = data.supervisor_cellphone,
                    supervisor_tel = data.supervisor_tel,
                    create_day = DateTime.Now.Date,
                    account_id = account_id

                };
                try {
                    db.confirmation_intern.Add(intern);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx) {
                    foreach (var validationErrors in dbEx.EntityValidationErrors) {
                        foreach (var validationError in validationErrors.ValidationErrors) {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    throw;
                }
                return RedirectToAction("Create", "ConfirmIntern", new { area = "Student", @message = "success" });
            }
            int year = DateTime.Now.Year;
            var current = year.ToString();
            var next = (year + 1).ToString();
            var nnext = (year + 2).ToString();
            var school_year = new[] { new SelectListItem { Text = current + "-" + next, Value = current + "-" + next }, new SelectListItem { Text = next + "-" + nnext, Value = next + "-" + nnext } };

            //Generate List of Type Intern to ViewBag
            var type_intern = new[] { new SelectListItem { Text = "Thực tập tốt nghiệp", Value = "0" }, new SelectListItem { Text = "Thực tập nghề nghiệp", Value = "1" }, new SelectListItem { Text = "Khác", Value = "2" } };

            //Generate List of Districts to ViewBag
            var district = db.Districts.Select(x => new {
                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            });

            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            foreach (var item in province) {
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            //Generate List of faculty to ViewBag
            var Faculty = db.Faculties.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            foreach (var item in Faculty) {
                liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }

            //Generate List of Level to ViewBag
            var Level = new[] { new SelectListItem { Text = "Đại học", Value = "0" }, new SelectListItem { Text = "Cao đẳng", Value = "1" } };

            //Generate List of Semester to ViewBag
            var semester_intern = new[] { new SelectListItem { Text = "Học kì 1", Value = "0" }, new SelectListItem { Text = " Học kì 2", Value = "1" }, new SelectListItem { Text = "Học kì hè", Value = "2" } };

            ViewBag.school_year = school_year;
            ViewBag.type_intern = type_intern;
            ViewBag.semester = semester_intern;
            ViewBag.level_id = Level;
            ViewBag.faculty_id = liFaculty;
            ViewBag.province = liProvinces;
            ViewBag.major_id = new SelectList(db.Faculties_Majors, "Id", "Name");
            ViewBag.student_district_id = new SelectList(district, "ID", "District_Name");
            ViewBag.student_province_id = liProvinces;
            ViewBag.org_district_id = new SelectList(district, "ID", "District_Name");
            ViewBag.org_province_id = liProvinces;

            return View(data);
        }
        [HttpGet]
        public ActionResult Preview() {
            confirmation_intern data = (confirmation_intern)TempData["data"];
            var student_district = db.Districts.FirstOrDefault(x => x.Id == data.student_district_id);
            var student_province = db.Provinces.FirstOrDefault(x => x.Id == data.student_province_id);
            var org_district = db.Districts.FirstOrDefault(x => x.Id == data.student_district_id);
            var org_province = db.Provinces.FirstOrDefault(x => x.Id == data.student_province_id);
            var major = db.Faculties_Majors.FirstOrDefault(x => x.Id == data.major_id);
            var faculty = db.Faculties.FirstOrDefault(x => x.Id == data.faculty_id);
            confirmation_intern intern = new confirmation_intern {
                student_name = data.student_name,
                student_class = data.student_class,
                student_id = data.student_id,
                major_id = data.major_id,
                faculty_id = data.faculty_id,
                level_id = data.level_id,
                student_email = data.student_email,
                cellphone = data.cellphone,
                student_street = data.student_street,
                student_ward = data.student_ward,
                student_district_id = data.student_district_id,
                student_province_id = data.student_province_id,
                type_intern = data.type_intern,
                other_type_intern = data.other_type_intern,
                semester = data.semester,
                school_year = data.school_year,
                position = data.position,
                department = data.department,
                start_day = data.start_day,
                end_day = data.end_day,
                schedule_work = data.schedule_work,
                organization_name = data.organization_name,
                website = data.website,
                tax = data.tax,
                org_telephone = data.org_telephone,
                org_street = data.org_street,
                org_ward = data.org_ward,
                org_district_id = data.org_district_id,
                org_province_id = data.org_province_id,
                human_resource_name = data.human_resource_name,
                human_resource_position = data.human_resource_position,
                human_resource_email = data.human_resource_email,
                human_resource_cel = data.human_resource_cel,
                human_resource_position_tel = data.human_resource_position_tel,
                supervisor_name = data.supervisor_name,
                supervisor_position = data.supervisor_position,
                supervisor_email = data.supervisor_email,
                supervisor_cellphone = data.supervisor_cellphone,
                supervisor_tel = data.supervisor_tel,
                create_day = DateTime.Now.Date

            };
            TempData["data"] = data;
            ViewBag.student_dictrict = student_district.Type + " " + student_district.District_Name;
            ViewBag.org_dictrict = org_district.Type + " " + org_district.District_Name;
            ViewBag.student_provinces = student_province.Type + " " + student_province.Name;
            ViewBag.org_provinces = org_province.Type + " " + org_province.Name;
            ViewBag.major = major.Name;
            ViewBag.faculty = faculty.Name;
            return View(intern);
        }

        public ActionResult ConfirmInternPreview() {
            confirmation_intern data = (confirmation_intern)TempData["data"];
            var account_id = User.Identity.GetUserId();
            if (data.type_intern != 2) {
                data.other_type_intern = null;
            }
            confirmation_intern intern = new confirmation_intern {
                student_name = data.student_name,
                student_class = data.student_class,
                student_id = data.student_id,
                major_id = data.major_id,
                faculty_id = data.faculty_id,
                level_id = data.level_id,
                student_email = data.student_email,
                cellphone = data.cellphone,
                student_street = data.student_street,
                student_ward = data.student_ward,
                student_district_id = data.student_district_id,
                student_province_id = data.student_province_id,
                type_intern = data.type_intern,
                other_type_intern = data.other_type_intern,
                semester = data.semester,
                school_year = data.school_year,
                position = data.position,
                department = data.department,
                start_day = data.start_day,
                end_day = data.end_day,
                schedule_work = data.schedule_work,
                organization_name = data.organization_name,
                website = data.website,
                tax = data.tax,
                org_telephone = data.org_telephone,
                org_street = data.org_street,
                org_ward = data.org_ward,
                org_district_id = data.org_district_id,
                org_province_id = data.org_province_id,
                human_resource_name = data.human_resource_name,
                human_resource_position = data.human_resource_position,
                human_resource_email = data.human_resource_email,
                human_resource_cel = data.human_resource_cel,
                human_resource_position_tel = data.human_resource_position_tel,
                supervisor_name = data.supervisor_name,
                supervisor_position = data.supervisor_position,
                supervisor_email = data.supervisor_email,
                supervisor_cellphone = data.supervisor_cellphone,
                supervisor_tel = data.supervisor_tel,
                create_day = DateTime.Now.Date,
                account_id = account_id
            };
            db.confirmation_intern.Add(intern);
            db.SaveChanges();
            TempData["data"] = null;
            return RedirectToAction("Create", "ConfirmIntern", new { area = "Student", @message = "success" });
        }

        [HttpGet]
        public ActionResult Upload(string message = null) {
            if (message == "success") {
                ViewBag.Message = "success";
            }
            var id = User.Identity.GetUserId();
            var model = db.confirmation_intern.Where(x => x.account_id == id).ToList();
            if (model == null) {
                return RedirectToAction("Create", new { @message = "create" });
                // cho no redirect veef trang tao phieu + message 
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(int id,HttpPostedFileBase file) {
            if (file != null && file.ContentLength > 0) {
                //create service
                DriveService service = GetService();
                string path = Path.Combine(HostingEnvironment.MapPath("~/GoogleDriveFiles"),
                Path.GetFileName(file.FileName));
                file.SaveAs(path);

                var folder_id = "1NSLBfSXs1qy63SfdkdP0z4A8BQcNb0iQ";
                var fileMetadata = new Google.Apis.Drive.v3.Data.File() {
                    Name = file.FileName,
                    Parents = new List<string>
                   {
                        folder_id
                    }
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path,
                               System.IO.FileMode.Open)) {
                    request = service.Files.Create(
                        fileMetadata, stream, "application/pdf");
                    request.Fields = "id,webViewLink";
                    request.Upload();
                }
                System.IO.File.Delete(path);
                var link = request.ResponseBody;
                var check = id;
                var model = db.confirmation_intern.FirstOrDefault(x => x.id == id);
                model.link = link.WebViewLink;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Upload", new { @message = "success" });
        }

      
        public static DriveService GetService() {
            //get Credentials from client_secret.json file 
            String serviceAccountEmail = "luong-593@driveupload-274203.iam.gserviceaccount.com";
            string pathh = HostingEnvironment.MapPath("~/Content/GoogleKey/key.p12");
            var certificate = new X509Certificate2(pathh, "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail) {
                   Scopes = new[] { DriveService.Scope.DriveFile }
               }.FromCertificate(certificate));

            var service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "cc",
            });
            return service;
        }
    }
}