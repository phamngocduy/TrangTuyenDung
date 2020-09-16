using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Company.Controllers {

    [Authorize(Roles = "Company")]
    public class RecruitmentController : Controller {
        EJobEntities db = new EJobEntities();
        // GET: Company/Recruitment
        public ActionResult Index() {
            return View();
        }

        // Create Recruitment
        [HttpGet]
        public ActionResult Create(string message = null) {
            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "0" });

            foreach (var item in province) {
                //Add to ViewBag
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                ViewBag.province = liProvinces;
            }

            //Generate list of District
            var district = db.Districts.Select(x => new {

                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            });
            //leader change here
            //add ngành
            var Faculty = db.Faculties.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            liFaculty.Add(new SelectListItem { Text = "--Chọn Ngành--", Value = "" });

            foreach (var item in Faculty) {
                //Add to ViewBag
                liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

            }
            ViewBag.Faculties_ID = liFaculty;
            ViewBag.Job_Position_Id = new SelectList(db.Job_Positions, "Id", "Name");

            ViewBag.Districts_id = new SelectList(district, "ID", "District_Name");
            //view bag show major
            var Major_Faculties = db.Faculties_Majors;
            var items = new List<SelectListItem>();
            foreach (var item in db.Faculties) {
                SelectListGroup group = new SelectListGroup() { Name = item.Name };
                foreach (var value in Major_Faculties.Where(x => x.Faculty_Id == item.Id)) {
                    items.Add(new SelectListItem {
                        Value = value.Id.ToString(),
                        Text = value.Name,
                        Group = group
                    });
                }
            }

            ViewBag.RecMajor = items;
            //Check message
            if (message == "PostSuccess") {
                ViewBag.Message = "PostSuccess";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(RecruitmentCreate data, string submit = null) {
            //flag check if from preview page to create httppost no check model state again
            bool Flagcheck = false;
            var userId = User.Identity.GetUserId();
            if (submit == "Save" || submit == "Cancel") {
                data = (RecruitmentCreate)TempData["PreViewRecruitment"];
                if (submit == "Save") {
                    Flagcheck = true;
                    submit = "ĐĂNG TIN";
                }
                else if (submit == "Cancel") {
                    Flagcheck = false;
                }
            }
            data.Company_id = db.User_In_Company.FirstOrDefault(x => x.Account_id == userId).Company_id;
            data.Created_by = db.AspNetUsers.FirstOrDefault(x => x.Id == userId).Email;
            var UserInCompany = db.User_In_Company.FirstOrDefault(x => x.Account_id == userId);
            if ((!ModelState.IsValid || UserInCompany.Status_id != 1) && Flagcheck == false) {
                ListViewBagCreateRecruitment();
                submit = null;
                Flagcheck = false;
                return View(data);
            }
            else {
                //create new recruitment
                //Leader change here
                Recruitment nRec = new Recruitment {
                    Company_id = data.Company_id,
                    Is_Show = true,
                    Nums_view = 0,
                    Status_id = 1,
                    Districts_id = data.Districts_id,
                    title = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(data.title.ToLower()),
                    Expire_date = data.Expire_date,
                    Recruiting_dates = data.Recruiting_dates,
                    Salary_from = data.Salary_from,
                    Salary_to = data.Salary_to,
                    Is_Full_Time = data.Is_Full_Time,
                    Is_Part_Time = data.Is_Part_Time,
                    Is_Intership = data.Is_Intership,
                    Job_Description = data.Mo_ta_Chi_Tiet,
                    Job_Benefits = data.Phuc_Loi,
                    Required_Skills = data.Ky_Nang_Cong_Viec,
                    Soft_Skills = data.Yeu_Cau_Cong_Viec,
                    Job_Optional = data.Tuy_Chon,
                    //Faculty_Id = data.Faculties_ID,
                    Created_date = DateTime.Now,
                    Created_by = data.Created_by,
                    Role_Create = "Company",
                    other_job=data.other_job

                };

                //Checking value in Job Positions
                if (data.Job_Position_Id != 0) {
                    nRec.Job_Position_Id = data.Job_Position_Id;
                }
                if (data.Is_Full_Time == true) {
                    nRec.Num_FullTime = data.Num_FullTime;
                }
                if (data.Is_Part_Time == true) {
                    nRec.Num_PartTime = data.Num_PartTime;
                }
                if (data.Is_Intership == true) {
                    nRec.Num_Intern = data.Num_Intern;
                }
                //Add new record
                if (submit == "ĐĂNG TIN") {
                    db.Recruitments.Add(nRec);
                    db.SaveChanges();
                    //check tag_recruitment and add to database
                    if (data.RecMajor != null) {
                        foreach (var item in data.RecMajor) {
                            db.Majors_In_Recruitments.Add(new Majors_In_Recruitments {
                                Major_Id = item,
                                Recruitment_Id = nRec.Id,
                                Date_Create = DateTime.Now
                            });

                        }
                        db.SaveChanges();
                    }
                    Flagcheck = false;
                    return RedirectToAction("Create", "Recruitment", new { area = "Company", @message = "PostSuccess" });
                }
                else {
                    TempData["PreViewRecruitment"] = data;
                    Flagcheck = false;
                    return RedirectToAction("PreView", "Recruitment", new { area = "Company" });
                }


            }
        }

        // Thuan Nguyen - GET: Company View List Recruitment
        public ActionResult List() {
            //Get current User ID
            var currentUser = User.Identity.GetUserId();
            //Get current Company
            var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
            //Get List Recruitments of current Company
            var model = db.Recruitments.Where(m => m.Company_id == currentCom.Company_id && m.Status_id != 1 && m.Status_id != 4).ToList();
            var check = new RecruitmentSupport();
            check.CheckOutDate(model);
            return View(model);
        }
        //Thuan Nguyen - GET: Company View List of Pending Recruitments
        public ActionResult Pending() {
            //Get current User ID
            var currentUser = User.Identity.GetUserId();
            //Get current Company
            var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
            //Get List Pending Recruitments of current Company
            var model = db.Recruitments.Where(m => m.Company_id == currentCom.Company_id && m.Status_id == 1).ToList();

            return View(model);
        }
        //Thuan Nguyen - GET: Company View List of Rejected Recruitments
        public ActionResult Rejected() {
            //Get current User ID
            var currentUser = User.Identity.GetUserId();
            //Get current Company
            var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
            //Get List Pending Recruitments of current Company
            var model = db.Recruitments.Where(m => m.Company_id == currentCom.Company_id && m.Status_id == 4).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult RePost(int? id) {
            //Get current User ID
            var currentUser = User.Identity.GetUserId();
            //Get current Company
            var currentCom = db.User_In_Company.FirstOrDefault(m => m.Account_id == currentUser);
            var model = new Recruitment();
            var mess = TempData["Message"];
            //check mess null or not to show dialog notification
            if (mess == null) {
                //Get List Pending Recruitments of current Company
                model = db.Recruitments.FirstOrDefault(m => m.Company_id == currentCom.Company_id && (m.Status_id == 3 || m.Status_id == 4) && m.Id == id);
            }
            else {
                model = db.Recruitments.FirstOrDefault(m => m.Company_id == currentCom.Company_id && m.Id == id);
                ViewBag.Message = TempData["Message"].ToString();
            }
            //check modal null or not to return view
            if (model == null) {
                return RedirectToAction("List", "Recruitment", new { area = "Company" });
            }
            else {
                ListViewBagEditRecruitment(model.Districts_id, model.Job_Position_Id);
                var data = new RecruitmentCreate {
                    Id = model.Id,
                    title = model.title,
                    Districts_id = model.Districts_id,
                    Tags_Recruitments = model.Tags_Recruitments,
                    Ky_Nang_Cong_Viec = model.Required_Skills,
                    Mo_ta_Chi_Tiet = model.Job_Description,
                    Phuc_Loi = model.Job_Benefits,
                    Tuy_Chon = model.Job_Benefits,
                    Expire_date = DateTime.Now,
                    Recruiting_dates = DateTime.Now,
                    Company_id = model.Company_id,
                    Company_Info = model.Company_Info,
                    Is_Full_Time = model.Is_Full_Time,
                    Is_Part_Time = model.Is_Part_Time,
                    Is_Intership = model.Is_Intership,
                    Salary_from = model.Salary_from,
                    Salary_to = model.Salary_to,
                    Province_ID = model.District.ProvinceId,
                    Num_FullTime = model.Num_FullTime,
                    Num_PartTime = model.Num_PartTime,
                    Num_Intern = model.Num_Intern,
                    //Leader change here
                    Yeu_Cau_Cong_Viec = model.Soft_Skills,
                    Job_Position_Id = model.Job_Position_Id,
                    other_job=model.other_job,

                    Majors_In_Recruitments = model.Majors_In_Recruitments
                };
                //Checking value in Job Positions
                if (model.Job_Position_Id != 0) {
                    data.Job_Position_Id = model.Job_Position_Id;
                }
                if (model.Is_Full_Time == true) {
                    data.Num_FullTime = model.Num_FullTime;
                }
                if (model.Is_Part_Time == true) {
                    data.Num_PartTime = model.Num_PartTime;
                }
                if (model.Is_Intership == true) {
                    data.Num_Intern = model.Num_Intern;
                }
                data.RecMajorString = db.Majors_In_Recruitments.Where(x => x.Recruitment_Id == model.Id).Select(x => x.Faculties_Majors.Id.ToString()).ToArray();
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult RePost(RecruitmentCreate data) {
            var userId = User.Identity.GetUserId();
            data.Company_id = db.User_In_Company.FirstOrDefault(x => x.Account_id == userId).Company_id;
            data.Created_by = db.AspNetUsers.FirstOrDefault(x => x.Id == userId).Email;
            var UserInCompany = db.User_In_Company.FirstOrDefault(x => x.Account_id == userId);
            var RecruitmentUpdate = db.Recruitments.FirstOrDefault(x => x.Id == data.Id);
            if (!ModelState.IsValid || UserInCompany.Status_id != 1) {
                ListViewBagEditRecruitment(data.Districts_id, data.Job_Position_Id);
                return View(data);
            }
            else {
                int[] MajorId = Array.ConvertAll(data.RecMajorString, s => int.Parse(s));


                RecruitmentUpdate.Company_id = data.Company_id;
                RecruitmentUpdate.Is_Show = true;
                RecruitmentUpdate.Nums_view = 0;
                RecruitmentUpdate.Status_id = 1;
                RecruitmentUpdate.Districts_id = data.Districts_id;
                RecruitmentUpdate.title = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(data.title.ToLower());
                RecruitmentUpdate.Expire_date = data.Expire_date;
                RecruitmentUpdate.Recruiting_dates = data.Recruiting_dates;
                RecruitmentUpdate.Salary_from = data.Salary_from;
                RecruitmentUpdate.Salary_to = data.Salary_to;
                RecruitmentUpdate.Is_Full_Time = data.Is_Full_Time;
                RecruitmentUpdate.Is_Part_Time = data.Is_Part_Time;
                RecruitmentUpdate.Is_Intership = data.Is_Intership;
                RecruitmentUpdate.Job_Description = data.Mo_ta_Chi_Tiet;
                RecruitmentUpdate.Job_Benefits = data.Phuc_Loi;
                RecruitmentUpdate.Required_Skills = data.Ky_Nang_Cong_Viec;
                RecruitmentUpdate.Soft_Skills = data.Yeu_Cau_Cong_Viec;
                RecruitmentUpdate.Job_Optional = data.Tuy_Chon;
                //Faculty_Id = data.Faculties_ID,
                RecruitmentUpdate.Created_date = DateTime.Now;
                RecruitmentUpdate.Created_by = data.Created_by;
                RecruitmentUpdate.Role_Create = "Company";
                RecruitmentUpdate.other_job = data.other_job;



                //Checking value in Job Positions
                if (data.Job_Position_Id != 0) {
                    RecruitmentUpdate.Job_Position_Id = data.Job_Position_Id;
                }
                if (data.Is_Full_Time == true) {
                    RecruitmentUpdate.Num_FullTime = data.Num_FullTime;
                }
                if (data.Is_Part_Time == true) {
                    RecruitmentUpdate.Num_PartTime = data.Num_PartTime;
                }
                if (data.Is_Intership == true) {
                    RecruitmentUpdate.Num_Intern = data.Num_Intern;
                }
                //Add new record

                db.SaveChanges();
                //check tag_recruitment and add to database
                if (MajorId != null) {
                    foreach (var item in MajorId) {
                        db.Majors_In_Recruitments.Add(new Majors_In_Recruitments {
                            Major_Id = item,
                            Recruitment_Id = RecruitmentUpdate.Id,
                            Date_Create = DateTime.Now
                        });

                    }
                    db.SaveChanges();
                }
                TempData["Message"] = "PostSuccess";
                return RedirectToAction("Rejected", "Recruitment", new { area = "Company" });
            }
        }

        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return RedirectToAction("Rejected", "Recruitment", new { area = "Company" });
            }
            var datadelete = db.Recruitments.FirstOrDefault(x => x.Id == id);
            if (datadelete != null) {
                var tag_rec = db.Tags_Recruitments.Where(x => x.Id_Recruitment == datadelete.Id);
                foreach (var item in tag_rec) {
                    db.Tags_Recruitments.Remove(item);
                }
                var Major_rec = db.Majors_In_Recruitments.Where(x => x.Recruitment_Id == datadelete.Id);
                foreach (var item in Major_rec) {
                    db.Majors_In_Recruitments.Remove(item);
                }
                db.Recruitments.Remove(datadelete);
                db.SaveChanges();
                return RedirectToAction("Rejected", "Recruitment", new { area = "Company", @message = "DeleteSuccess" });
            }
            else {
                return RedirectToAction("Rejected", "Recruitment", new { area = "Company", @message = "DeleteFail" });
            }

        }

        public ActionResult PreView() {
            RecruitmentCreate value = (RecruitmentCreate)TempData["PreViewRecruitment"];
            TempData["PreViewRecruitment"] = value;
            var companyinfo = db.Company_Info.FirstOrDefault(x => x.Id == value.Company_id);
            var Districtinfo = db.Districts.FirstOrDefault(x => x.Id == value.Districts_id);
            var model = new RecruitmentShow {
                title = value.title,
                Company_id = value.Company_id,
                Company_Info = companyinfo,
                Districts_id = value.Districts_id,
                District = Districtinfo,
                Expire_date = value.Expire_date,
                Recruiting_dates = value.Recruiting_dates,
                Is_Full_Time = value.Is_Full_Time,
                Is_Intership = value.Is_Intership,
                Is_Part_Time = value.Is_Part_Time,
                Job_Benefits = value.Phuc_Loi,
                Job_Positions = value.Job_Positions,
                Job_Position_Id = value.Job_Position_Id,
                Nums_view = value.Nums_view,
                Num_FullTime = value.Num_FullTime,
                Num_Intern = value.Num_Intern,
                Num_PartTime = value.Num_PartTime,
                Salary_from = value.Salary_from,
                Salary_to = value.Salary_to,
                //Faculty = model.Faculty,
                //Faculty_Id = model.Faculty_Id,
                Job_Description = value.Mo_ta_Chi_Tiet,
                Job_Optional = value.Tuy_Chon,
                Soft_Skills = value.Ky_Nang_Cong_Viec,
                Required_Skills=value.Yeu_Cau_Cong_Viec,

            };
            ViewBag.FlagPreView = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult SavePreview() {
            RecruitmentCreate value = (RecruitmentCreate)TempData["PreViewRecruitment"];
            return View();
        }

        public void ListViewBagCreateRecruitment() {
            var district = db.Districts.Select(x => new {

                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            });
            ViewBag.Districts_id = new SelectList(district, "ID", "District_Name");

            //leader change here
            //add ngành
            var Faculty = db.Faculties.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            liFaculty.Add(new SelectListItem { Text = "--Chọn Ngành--", Value = "" });

            foreach (var item in Faculty) {
                //Add to ViewBag
                liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

            }
            ViewBag.Faculties_ID = liFaculty;

            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "0" });

            foreach (var item in province) {
                //Add to ViewBag
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                ViewBag.province = liProvinces;
            }
            ViewBag.Job_Position_Id = new SelectList(db.Job_Positions, "Id", "Name");
            //view bag show major
            var Major_Faculties = db.Faculties_Majors;
            var items = new List<SelectListItem>();
            foreach (var item in db.Faculties) {
                SelectListGroup group = new SelectListGroup() { Name = item.Name };
                foreach (var value in Major_Faculties.Where(x => x.Faculty_Id == item.Id)) {
                    items.Add(new SelectListItem {
                        Value = value.Id.ToString(),
                        Text = value.Name,
                        Group = group
                    });
                }
            }

            ViewBag.RecMajor = items;
        }

        public void ListViewBagEditRecruitment(int DistrictId, int? JobPositionId) {
            //get providence to show all district in that providence
            var Providence = db.Districts.FirstOrDefault(x => x.Id == DistrictId).ProvinceId;
            //Viewbag district
            var district = db.Districts.Where(x => x.ProvinceId == Providence).Select(x => new {

                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            });
            ViewBag.Districts_id = new SelectList(district, "ID", "District_Name");
            //leader change here
            //add ngành
            var Faculty = db.Faculties.ToList();
            List<SelectListItem> liFaculty = new List<SelectListItem>();
            liFaculty.Add(new SelectListItem { Text = "--Chọn Ngành--", Value = "" });

            foreach (var item in Faculty) {
                //Add to ViewBag
                liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

            }
            ViewBag.Faculties_ID = liFaculty;

            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "0" });

            foreach (var item in province) {
                //Add to ViewBag
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                ViewBag.province = liProvinces;
            }
            //viewbag job position 
            //get faculty to show all job position in that faculty
            var Major = db.Job_Positions.FirstOrDefault(x => x.Id == JobPositionId);
            if (Major != null) {
                var Job_Position = db.Job_Positions.Where(x => x.Major_Id == Major.Major_Id).Select(x => new {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Job_Position_Id = new SelectList(Job_Position, "Id", "Name");
            }
            else {

                ViewBag.Job_Position_Id = new SelectList(string.Empty, "Value", "Text");
            }
            //view bag show major
            var Major_Faculties = db.Faculties_Majors;
            var items = new List<SelectListItem>();
            foreach (var item in db.Faculties) {
                SelectListGroup group = new SelectListGroup() { Name = item.Name };
                foreach (var value in Major_Faculties.Where(x => x.Faculty_Id == item.Id)) {
                    items.Add(new SelectListItem {
                        Value = value.Id.ToString(),
                        Text = value.Name,
                        Group = group,
                    });
                }
            }
            ViewBag.RecMajor = items;
        }

    }
}