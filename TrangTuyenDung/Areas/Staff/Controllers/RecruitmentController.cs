using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;
using TrangTuyenDung.Areas.Staff.Models;
using System.Net;
using System.Net.Mail;
using TrangTuyenDung.Sercurity_SendMail;

namespace TrangTuyenDung.Areas.Staff.Controllers {

    [Authorize(Roles = "Staff")]
    public class RecruitmentController : Controller {
        EJobEntities db = new EJobEntities();
        SendMail sm = new SendMail();
        // GET: Staff/Recruitment
        public ActionResult Approve(string message = null) {
            var model = db.Recruitments.Where(x => x.Status_id == 1).ToList();
            if (message == "ApproveSuccess") {
                ViewBag.Message = "ApproveSuccess";
            }
            else if (message == "RejectSuccess") {
                ViewBag.Message = "RejectSuccess";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Rejected(int? id, FormCollection formCollection) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else {
                var Recruitment = db.Recruitments.Find(id);
                string item = formCollection["other_reason"];
        
                sm.SendEmail_RejectRecruitment_toCompany(Recruitment.Company_id, formCollection);
                Recruitment.Updated_date = DateTime.Now;
                Recruitment.Status_id = 4;
                db.SaveChanges();
                return RedirectToAction("Approve", "Recruitment", new { area = "Staff", @message = "RejectSuccess" });
            }
        }

        //action thuc hien viec xac nhan tin tuyen dung chuyen trang thai tu "chua duyet" sang "da duyet"
        //httppost approve
        public ActionResult Approved(int id) {
            var rec = db.Recruitments.FirstOrDefault(x => x.Id == id);
            rec.Updated_date = DateTime.Now;
            rec.Status_id = 2;
            db.SaveChanges();
            return RedirectToAction("Approve", "Recruitment", new { area = "Staff", @message = "ApproveSuccess" });
        }

        //action thuc hien viec show ra list danh sach tin tuyen dung o trang thai "da duyet" or het han"
        public ActionResult List(string message = null) {
            var model = db.Recruitments.Where(x => x.Status_id == 2 || x.Status_id == 3).OrderByDescending(x => x.Id).ToList();
            if (message == "DeleteSuccess") {
                ViewBag.Message = "DeleteSuccess";
            }
            else if (message == "EditSuccess") {
                ViewBag.Message = "EditSuccess";
            }
            else if (message == "DeleteFail") {
                ViewBag.Message = "DeleteFail";
            }
            else if (message == "PostSuccess") {
                ViewBag.Message = "PostSuccess";
            }
            var check = new RecruitmentSupport();
            check.CheckOutDate(model);
            return View(model);
        }
        //ham an-hien tin tuyen dung sau khi da duyet
        public ActionResult Show_Hide(int id) {
            var item = db.Recruitments.Find(id);
            if (item.Is_Show == true) {
                item.Is_Show = false;
            }
            else {
                item.Is_Show = true;
            }
            db.SaveChanges();
            return RedirectToAction("List", "Recruitment", new { area = "Staff" });
        }

        //Staff dang tin tuyen dung moi
        //Get: Staff/Recruitment/Post_NewRecruitment
        [HttpGet]
        public ActionResult Post_NewRecruitment() {
            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "0" });

            foreach (var item in province) {
                //Add to ViewBag
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                ViewBag.province = liProvinces;
            }
            //Generate List of District
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
            ViewBag.recttag = new SelectList(db.Tags, "ID", "Name_Tag");
            ViewBag.Company_Id = new SelectList(db.Company_Info, "ID", "Name_Company");
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

            return View();
        }

        [HttpPost]
        //Post: Staff/Recruitment/Post_NewRecruitment
        public ActionResult Post_NewRecruitment(RecruitmentCreate data, string submit = null) {
            if (!ModelState.IsValid) {
                var district = db.Districts.Where(x => x.ProvinceId == data.Province_ID).Select(x => new {
                    ID = x.Id,
                    District_Name = x.Type + " " + x.District_Name
                });
                //Generate List of Provinces to ViewBag
                var province = db.Provinces.ToList();
                List<SelectListItem> liProvinces = new List<SelectListItem>();
                liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "0" });

                foreach (var item in province) {
                    //Add to ViewBag
                    liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                    ViewBag.province = liProvinces;

                }
                var Faculty = db.Faculties.ToList();
                List<SelectListItem> liFaculty = new List<SelectListItem>();
                liFaculty.Add(new SelectListItem { Text = "--Chọn Ngành--", Value = "0" });

                foreach (var item in Faculty) {
                    //Add to ViewBag
                    liFaculty.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

                }
                ViewBag.Faculties_ID = liFaculty;
                ViewBag.Job_Position_Id = new SelectList(db.Job_Positions, "Id", "Name");
                ViewBag.Districts_id = new SelectList(district, "ID", "District_Name");
                ViewBag.recttag = new SelectList(db.Tags, "ID", "Name_Tag");
                ViewBag.Company_Id = new SelectList(db.Company_Info, "ID", "Name_Company");
                //View bag show major
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
                TempData["flag"] = true;
                return View(data);
            }
            else {
                if (submit == "Xem trước") {
                    TempData["data"] = data;
                    return RedirectToAction("PreViewCreateRecruitment", "Recruitment");
                }
                else {
                    string a = Server.HtmlDecode(data.Mo_ta_Chi_Tiet);
                    var sUserId = User.Identity.GetUserId();
                    //create new recruitment
                    //Leader change here
                    Recruitment nRec = new Recruitment {
                        Company_id = data.Company_id,
                        Is_Show = true,
                        Nums_view = 0,
                        Status_id = 2,
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
                        Required_Skills = data.Ky_Nang_Cong_Viec,
                        Soft_Skills = data.Yeu_Cau_Cong_Viec,
                        Job_Benefits = data.Phuc_Loi,
                        Job_Optional = data.Tuy_Chon,
                        Created_date = DateTime.Now,
                        Updated_date = DateTime.Now,
                        //Faculty_Id = data.Faculties_ID,
                        Created_by = db.AspNetUsers.FirstOrDefault(x => x.Id == sUserId).Email,
                        Role_Create = "Staff",
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
                    db.Recruitments.Add(nRec);
                    //check major and add to database
                    if (data.RecMajor != null) {
                        foreach (var item in data.RecMajor) {
                            db.Majors_In_Recruitments.Add(new Majors_In_Recruitments {
                                Major_Id = item,
                                Recruitment_Id = nRec.Id,
                                Date_Create = DateTime.Now
                            });
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("List", "Recruitment", new { area = "Staff" , @message = "PostSuccess" });
            }
        }

        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return RedirectToAction("List", "Recruitment", new { area = "Staff", @mesage = "NotFound" });
            }
            var datadelete = db.Recruitments.FirstOrDefault(x => x.Id == id && x.Apply_Recruitments.Count == 0);
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
                return RedirectToAction("List", "Recruitment", new { area = "Staff", @message = "DeleteSuccess" });
            }
            else {
                return RedirectToAction("List", "Recruitment", new { area = "Staff", @message = "DeleteFail" });
            }

        }

        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return HttpNotFound();
            }
            var model = db.Recruitments.FirstOrDefault(x => x.Id == id && x.Role_Create == "Staff");
            if (model == null) {
                return HttpNotFound();
            }
            var data = new RecruitmentCreate {
                Id = model.Id,
                title = model.title,
                Districts_id = model.Districts_id,
                Tags_Recruitments = model.Tags_Recruitments,
                Ky_Nang_Cong_Viec = model.Required_Skills,
                Mo_ta_Chi_Tiet = model.Job_Description,
                Phuc_Loi = model.Job_Benefits,
                Tuy_Chon = model.Job_Optional,
                Expire_date = model.Expire_date,
                Recruiting_dates = model.Recruiting_dates,
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
                Majors_In_Recruitments = model.Majors_In_Recruitments,
                other_job =model.other_job
                

            };
            ViewBag.District = db.Districts.Where(x => x.ProvinceId == model.District.ProvinceId).Select(x => new {
                ID = x.Id,
                District_Name = x.Type + " " + x.District_Name
            }).ToList();
            ViewBag.Faculties_ID = db.Faculties.Select(x => new { ID = x.Id, Name = x.Name }).ToList();
            //add vi tri ung tuyen
            //var Job_Position = db.Job_Positions.Where(x => x.Faculty_Id == model.Faculty_Id).ToList();
            //List<SelectListItem> liJob = new List<SelectListItem>();
            //liJob.Add(new SelectListItem { Text = "--Chọn Vị trí ứng tuyển--", Value = "0" });
            ////foreach (var item in Job_Position)
            //{

            //        liJob.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            //}
            if (model.Job_Position_Id != null) {
                ViewBag.Job_Position_Id = new SelectList(db.Job_Positions, "Id", "Name", model.Job_Position_Id);
            }
            else {
                ViewBag.Job_Position_Id = new SelectList(db.Job_Positions, "Id", "Name", 0);
            }
            //ViewBag.Job_Position = db.Job_Positions.ToList();
            ViewBag.Company = db.Company_Info.ToList();
            //ViewBag.Tag = new MultiSelectList(db.Tags, "ID", "Name_Tag", db.Tags_Recruitments.Where(x => x.Id_Recruitment == id).Select(x => x.Id_Tags).ToList());
            ViewBag.Province = db.Provinces.ToList();
            //View bag show major
            //var RecMajor = new MultiSelectList(db.Faculties_Majors, "Id", "Name", db.Majors_In_Recruitments.Where(x => x.Recruitment_Id == id).Select(x => x.Major_Id).ToList(),db.Faculties_Majors.Select(x => x.Faculty.Name));
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
            var listMajor = new List<int>();
            foreach (var item in items) {
                foreach (var item_compare in db.Majors_In_Recruitments.Where(x => x.Recruitment_Id == id).Select(x => x.Major_Id)) {
                    if (item.Value.ToString() == item_compare.ToString()) {
                        item.Selected = true;
                        listMajor.Add(int.Parse(item.Value));
                    }
                }
            }
            data.RecMajor = listMajor.ToArray();


            ViewBag.RecMajor = items;
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(RecruitmentCreate data) {
            if (!ModelState.IsValid) {
                ViewBag.District = db.Districts.Select(x => new {

                    ID = x.Id,
                    District_Name = x.Type + " " + x.District_Name
                }).ToList();
                ViewBag.Faculties_ID = db.Faculties.Select(x => new { ID = x.Id, Name = x.Name }).ToList();
                //add vi tri ung tuyen
                //var Job_Position = db.Job_Positions.Where(x => x.Faculty_Id == data.Faculty_Id).ToList();
                List<SelectListItem> liJob = new List<SelectListItem>();
                liJob.Add(new SelectListItem { Text = "--Chọn Vị trí ứng tuyển--", Value = "0" });
                //foreach (var item in Job_Position)
                //{

                //    liJob.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                //}
                if (data.Job_Position_Id != null) {
                    ViewBag.Job_Position_Id = data.Job_Position_Id;
                }
                else {
                    ViewBag.Job_Position_Id = 0;
                }
                ViewBag.Job_Position = db.Job_Positions.Where(x => x.Id == data.Job_Position_Id).ToList();
                ViewBag.Company = db.Company_Info.ToList();
                //ViewBag.Tag = new MultiSelectList(db.Tags, "ID", "Name_Tag", db.Tags_Recruitments.Where(x => x.Id_Recruitment == data.Id).Select(x => x.Id_Tags).ToList());
                ViewBag.Province = db.Provinces.ToList();
                //View bag show major
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
                return View(data);
            }
            //Find recruitment in database
            var RecruitmentUpdate = db.Recruitments.FirstOrDefault(x => x.Id == data.Id);
            if (RecruitmentUpdate != null) {
                //update Recruitment
                RecruitmentUpdate.title = data.title;
                RecruitmentUpdate.Districts_id = data.Districts_id;
                RecruitmentUpdate.Job_Description = data.Mo_ta_Chi_Tiet;
                RecruitmentUpdate.Required_Skills = data.Ky_Nang_Cong_Viec;
                RecruitmentUpdate.Job_Benefits = data.Phuc_Loi;
                RecruitmentUpdate.Job_Optional = data.Tuy_Chon;
                RecruitmentUpdate.Recruiting_dates = data.Recruiting_dates;
                RecruitmentUpdate.Expire_date = data.Expire_date;
                RecruitmentUpdate.Salary_from = data.Salary_from;
                RecruitmentUpdate.Salary_to = data.Salary_to;
                RecruitmentUpdate.Is_Full_Time = data.Is_Full_Time;
                RecruitmentUpdate.Is_Intership = data.Is_Intership;
                RecruitmentUpdate.Is_Part_Time = data.Is_Part_Time;
                RecruitmentUpdate.Updated_date = DateTime.Now;
                RecruitmentUpdate.other_job = data.other_job;
                //Leader change here
                //RecruitmentUpdate.Faculty_Id = data.Faculties_ID;
                RecruitmentUpdate.Soft_Skills = data.Yeu_Cau_Cong_Viec;
                if (data.Job_Position_Id > 0 /*&& data.Job_Position_Id <= db.Faculties.Count()*/) {
                    RecruitmentUpdate.Job_Position_Id = data.Job_Position_Id;
                }
                else {
                    RecruitmentUpdate.Job_Position_Id = null;
                }

                if (data.Is_Full_Time) { RecruitmentUpdate.Num_FullTime = data.Num_FullTime; }
                if (data.Is_Part_Time) { RecruitmentUpdate.Num_PartTime = data.Num_PartTime; }
                if (data.Is_Intership) { RecruitmentUpdate.Is_Intership = data.Is_Intership; }
                //remove RecMajor select in recruitment
                var remove_RecMajor = db.Majors_In_Recruitments.Where(x => x.Recruitment_Id == data.Id).ToList();
                db.Majors_In_Recruitments.RemoveRange(remove_RecMajor);
                //check RecMajor and add to database
                if (data.RecMajor != null) {
                    foreach (var item in data.RecMajor) {
                        db.Majors_In_Recruitments.Add(new Majors_In_Recruitments {
                            Major_Id = item,
                            Recruitment_Id = RecruitmentUpdate.Id,
                            Date_Create = DateTime.Now
                        });
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("List", "Recruitment", new { area = "Staff", @message = "EditSuccess" });
        }

        [HttpGet]
        public ActionResult PreView(int? id) {
            if (id == null) {
                return HttpNotFound();
            }
            else {
                var model = db.Recruitments.FirstOrDefault(x => x.Id == id);
                if (model == null) {
                    return HttpNotFound();
                }
                else {
                    var data = new RecruitmentShow {
                        Id = model.Id,
                        title = model.title,
                        Company_id = model.Company_id,
                        Company_Info = model.Company_Info,
                        District = model.District,
                        Districts_id = model.Districts_id,
                        Expire_date = model.Expire_date,
                        Recruiting_dates = model.Recruiting_dates,
                        Is_Full_Time = model.Is_Full_Time,
                        Is_Intership = model.Is_Intership,
                        Is_Part_Time = model.Is_Part_Time,
                        Job_Benefits = model.Job_Benefits,
                        Job_Positions = model.Job_Positions,
                        Job_Position_Id = model.Job_Position_Id,
                        Nums_view = model.Nums_view,
                        Num_FullTime = model.Num_FullTime,
                        Num_Intern = model.Num_Intern,
                        Num_PartTime = model.Num_PartTime,
                        Salary_from = model.Salary_from,
                        Salary_to = model.Salary_to,
                        Job_Description = model.Job_Description,
                        Job_Optional = model.Job_Optional,
                        Yeu_Cau = model.Required_Skills + "</br>" + model.Soft_Skills,
                    };
                    return View(data);
                }

            }

        }

        [HttpGet]
        public ActionResult PreViewCreateRecruitment() {
            RecruitmentCreate value = (RecruitmentCreate)TempData["data"];
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
                Job_Description = value.Mo_ta_Chi_Tiet,
                Job_Optional = value.Tuy_Chon,
                Required_Skills = value.Ky_Nang_Cong_Viec,
                Soft_Skills = value.Yeu_Cau_Cong_Viec,

            };
            TempData["data"] = value;
            return View(model);
        }

        [HttpPost]
        public ActionResult PreViewCreateRecruitments() {
            RecruitmentCreate data = (RecruitmentCreate)TempData["data"];
            var sUserId = User.Identity.GetUserId();
            //create new recruitment
            //Leader change here
            Recruitment nRec = new Recruitment {
                Company_id = data.Company_id,
                Is_Show = true,
                Nums_view = 0,
                Status_id = 2,
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
                Required_Skills = data.Ky_Nang_Cong_Viec,
                Soft_Skills = data.Yeu_Cau_Cong_Viec,
                Job_Benefits = data.Phuc_Loi,
                Job_Optional = data.Tuy_Chon,
                Created_date = DateTime.Now,
                Updated_date = DateTime.Now,
                //Faculty_Id = data.Faculties_ID,
                Created_by = db.AspNetUsers.FirstOrDefault(x => x.Id == sUserId).Email,
                Role_Create = "Staff"
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
            db.Recruitments.Add(nRec);
            //check major and add to database
            if (data.RecMajor != null) {
                foreach (var item in data.RecMajor) {
                    db.Majors_In_Recruitments.Add(new Majors_In_Recruitments {
                        Major_Id = item,
                        Recruitment_Id = nRec.Id,
                        Date_Create = DateTime.Now
                    });
                }
            }
            db.SaveChanges();
            TempData["data"] = null;
            return RedirectToAction("List", "Recruitment", new { area = "Staff" });
        }

    }
}