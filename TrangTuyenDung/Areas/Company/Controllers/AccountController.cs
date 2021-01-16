using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Areas.Company.Models;
using TrangTuyenDung.Models;
using TrangTuyenDung.Sercurity_SendMail;

namespace TrangTuyenDung.Areas.Company.Controllers {
    public class AccountController : Controller {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AccountController));
        // GET: Company/Account
        EJobEntities db = new EJobEntities();
        SendMail sm = new SendMail();
        public ActionResult Index() {

            return View();
        }
        //Thuan Nguyen - GET: Comapny References Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AccountRegister(string _ref, string message = null) {
            if (_ref != null) {
                CompanyRefRegisterViewModel model = new CompanyRefRegisterViewModel();
                model._ref = _ref;
                //Get info company
                var com = db.Company_Info.FirstOrDefault(x => x.Token == _ref);
                ViewBag.ComName = com.Name_Company;
                if (message == "CantRegisterAccount") {
                    ViewBag.Message = "CantRegisterAccount";
                }
                else if (message == "RegisterSuccess") {
                    ViewBag.Message = "RegisterSuccess";
                }
                return View(model);
            }
            else {
                return RedirectToAction("Register", "Account", new { area = "Company" });
            }
        }
        //Thuan Nguyen - POST: Company Register with the references key
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AccountRegister(CompanyRefRegisterViewModel model, string _ref) {
            if (ModelState.IsValid) {
                //Get Value
               
                var comInfo = db.Company_Info.FirstOrDefault(x => x.Token == _ref);
                //Register new Company Account---------
                ApplicationDbContext context = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //Create new account
                var user = new ApplicationUser();
                user.UserName = model.Email;
                user.Email = model.Email;
                user.EmailConfirmed = true; //Activated Account
                string userPassWord = model.Password;
                var chkUser = userManager.Create(user, userPassWord);
                //If register success, add user role "Company"
                if (chkUser.Succeeded) {
                    var resultUser = userManager.AddToRole(user.Id, "Company");
                    //Check if this is the first account of company => can create other account
                    var chkFirstUser = db.User_In_Company.FirstOrDefault(x => x.Company_id == comInfo.Id);
                    if (chkFirstUser == null) {
                        db.User_In_Actions.Add(new User_In_Actions {
                            Account_Id = user.Id,
                            Action_Id = 1,
                            Date_Create = DateTime.Now
                        });
                    }
                    //Create User In Company
                    User_In_Company userInCompany = new User_In_Company {
                        Account_id = user.Id,
                        Company_id = comInfo.Id,
                        Status_id = 1,
                        FullName = model.FullName
                    };
                    //Active Company
                    comInfo.Status = 1;
                    //Save
                    db.Entry(comInfo).State = System.Data.Entity.EntityState.Modified;
                    db.User_In_Company.Add(userInCompany);
                    db.SaveChanges();
                    //return
                  
                    return RedirectToAction("AccountRegister", "Account", new { area = "Company", @_ref = _ref, @message = "RegisterSuccess" });
                }
                else {
                    return RedirectToAction("AccountRegister", "Account", new { area = "Company", @_ref = _ref, @message = "CantRegisterAccount" });
                }

            }
            else {
                return View(model);
            }
        }

        //Thuan Nguyen - GET: Company/Account/Register
        [AllowAnonymous]
        public ActionResult Register(string message = null) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("PageNotFound", "Error", new { area = "" });
            }
            ListAll();
            if (message == "RegisterSuccess") {
                ViewBag.Message = "RegisterSuccess";
            }
            return View();
        }
        //POST: Company/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CompanyRegisterViewModel comModel) {
            //ApplicationDbContext context = new ApplicationDbContext();
            HttpPostedFileBase file = Request.Files["Com_Logo"];
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (ModelState.IsValid) {
                if (checkCompany(comModel.Com_Name) == "false") {
                    ListAll();
                    ModelState.AddModelError("Com_Name", "Bạn không thể sử dụng tên công ty này! Vui lòng liên hệ ejob.hachikoteam@gmail.com để được hỗ trợ!");
                    return View(comModel);
                }
                else if (checkEmailAlreadyExist(comModel.Email) == "false") {
                    ListAll();
                    ModelState.AddModelError("Email", "Email đã có người đăng ký! Vui lòng kiểm tra lại!");
                    return View(comModel);
                }
                else if (checkBusinessCode(comModel.Com_Code) == "false") {
                    ListAll();
                    ModelState.AddModelError("Com_Code", "Mã số doanh nghiệp không hợp lệ hoặc đã tồn tại! Vui lòng thử lại!");
                    return View(comModel);
                }
                else {
                    ViewBag.District_id = new SelectList(db.Districts, "ID", "Name");
                    //Thuan Nguyen - Create new Company Info and Account
                    //UPDATE 08-11-2018 : NO CREATE ACCOUNT WHEN REGISTER------//
                    //Create new Company_Registration
                    Company_Registration comRegister = new Company_Registration() {
                        Name_Company = comModel.Com_Name,
                        Contact_Email = comModel.Email_Contact,
                        Contact_Phone = comModel.Phone_Contact,
                        Introduce_Company = comModel.Com_Intro,
                        website_Company = comModel.Com_Website,
                        Address_Company = comModel.Com_Address,
                        Province_ID = comModel.Com_Province,
                        District_ID = comModel.Com_District,
                        company_facultites=comModel.company_facultites,
                        Created_at = DateTime.Now,
                        //Login Info
                        FullName = comModel.FullName,
                        Email = comModel.Email,
                        Password = comModel.ConfirmPassword
                    };
                    comRegister.Slug_Company = GenerateSlug(comRegister.Name_Company);

                    //Processing Logo upload
                    if (file != null) {
                        // If content length is zero means no file attached
                        if (file.ContentLength == 0) {
                            ModelState.AddModelError("", "Không thể tải lên tệp, vui lòng kiểm tra lại!");
                            ListAll();
                            return View(comModel);
                        }
                        else {
                            //Convert Image to Byte array
                            MemoryStream targetFile = new MemoryStream();
                            comModel.Com_Logo.InputStream.CopyTo(targetFile);
                            byte[] imgBytes = targetFile.ToArray();
                            //Convert byte[] to base64 String
                            string imgBase64 = Convert.ToBase64String(imgBytes);
                            //Save to db
                            comRegister.ImageBase64 = imgBase64;
                        }
                    }
                    else {
                        ModelState.AddModelError("", "Không thể tải lên tệp, vui lòng kiểm tra lại!");
                        ListAll();
                        return View(comModel);
                    }

                    //Check Faculties values
                    if (comModel.Com_Faculty != null) {
                        //Convert int[] to string
                        string sFaculties = "";
                        foreach (var item in comModel.Com_Faculty) {
                            sFaculties += item + ",";
                        }
                        //Add to comRegister
                        // I'm removing character ',' in last string for prevent error when split this string to int[]
                        comRegister.Faculties = sFaculties.Remove(sFaculties.Length - 1, 1);
                    }
                    //Check Faculties_Other value
                    if (comModel.Com_OtherFaculties != null) {
                        comRegister.Faculties_Other = comModel.Com_OtherFaculties;
                    }
                    //Send to Database
                    try
                    {
                        log.Info(comModel.Com_Name + " registered");
                        db.Company_Registration.Add(comRegister);
                        db.SaveChanges();
                        sm.SendMail_Nofication(comModel.Com_Name);
                    }
                    catch (Exception e)
                    {
                        log.Error("Company register error", e);
                    }
                    return RedirectToAction("Register", "Account", new { area = "Company", @message = "RegisterSuccess" });
                }
            }
            else {
                ListAll();
                return View(comModel);
            }

        }
        //Thuan Nguyen - Check already exist email
        public string checkEmailAlreadyExist(string email) {
            if (email == "") {
                return "empty";
            }
            //
            if (IsValidEmailAddress(email)) {
                var chkEmail = db.AspNetUsers.FirstOrDefault(x => x.Email == email);
                if (chkEmail != null) {
                    return "false";
                }
                else {
                    return "true";
                }
            }
            else {
                return "invalid";
            }

        }
        //Thuan Nguyen - Check is valid email
        public static bool IsValidEmailAddress(string emailaddress) {
            try {
                Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
                return rx.IsMatch(emailaddress);
            }
            catch (FormatException) {
                return false;
            }
        }
        //Thuan Nguyen - Check exist company
        public string checkCompany(string com_name) {
            //Check part 01
            var chkPart01 = db.Company_Info.Where(x => x.Name_Company.Contains(com_name)).ToList();
            if (chkPart01.Count() != 0) {
                return "false";
            }
            //------------
            //Check part 02
            //Solving data
            com_name = com_name.ToLower();
            var keyToExclude = new string[] { "công ty", "cong ty", "công ti", "tnhh", "cong ti", "cổ phần", "co phan", "ngân hàng", "ngan hang", "khách sạn", "khach san" };
            foreach (var item in keyToExclude) {
                com_name = com_name.Replace(item, string.Empty).Trim();
            }
            //checking
            var chkPart02 = db.Company_Info.Where(x => x.Name_Company.Contains(com_name)).ToList();
            if (chkPart02.Count() != 0) {
                return "false";
            }
            else {
                return "true";
            }
        }
        //Thuan Nguye - Check Business code
        public string checkBusinessCode(string code) {
            var chkCode = db.Company_Info.FirstOrDefault(x => x.Code == code);
            if (chkCode == null) {
                return "true";
            }
            else {
                return "false";
            }
        }
        //Thuan Nguyen - ListAll for DropdownList
        public void ListAll() {
            //Generate List of Provinces to ViewBag
            var province = db.Provinces.ToList();
            List<SelectListItem> liProvinces = new List<SelectListItem>();
            liProvinces.Add(new SelectListItem { Text = "--Chọn Tỉnh/TP--", Value = "" });

            foreach (var item in province) {
                //Add to ViewBag
                liProvinces.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                ViewBag.province = liProvinces;
            }

            //ViewBag for Faculties Dropdown
            ViewBag.faculty = new SelectList(db.Faculties, "Id", "Name");
        }
        //Thuan Nguyen - Json for select Province - District
        public JsonResult GetDistrictsByProvinceID(int id) {
            var districts = db.Districts.Where(x => x.ProvinceId == id).ToList();
            List<SelectListItem> liDistricts = new List<SelectListItem>();
            //Default value for District
            liDistricts.Add(new SelectListItem { Text = "--Chọn Quận/Huyện--", Value = "" });
            if (districts != null) {
                foreach (var item in districts) {
                    liDistricts.Add(new SelectListItem { Text = item.Type + " " + item.District_Name, Value = item.Id.ToString() });
                }
            }
            return Json(new SelectList(liDistricts, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        //Thuan Nguyen - Althogrims for Generate Slug => For friendly URLs
        public string GenerateSlug(string Name) {

            string str = RemoveAccent(Name).ToLower();
            //Replace invalid characters
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            //Convert multi space to one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            //Cut and trim
            str = str.Substring(0, str.Length <= 50 ? str.Length : 50).Trim();
            //hyphens: "Company Name" to "company-name"
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }
        public string RemoveAccent(string txt) {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        [Authorize(Roles = "Company")]
        [HttpGet]
        public ActionResult Update(string status = null, string errorList = null) {
            ListAll();
            //get current user in company
            var crrUser = User.Identity.GetUserId();
            var user = db.User_In_Company.FirstOrDefault(x => x.Account_id == crrUser);

            //get current user fullname
            if (user.FullName == null) {
                ViewBag.FullName = "";
            }
            else {
                ViewBag.FullName = "" + user.FullName.ToString();
            }

            //get company info of user
            var model = db.Company_Info.FirstOrDefault(x => x.Id == user.Company_id);

            //get current district of province
            var district = db.Districts.Where(x => x.ProvinceId == model.Province_ID).ToList();
            List<SelectListItem> liDistricts = new List<SelectListItem>();
            foreach (var item in district) {
                //Add to ViewBag
                liDistricts.Add(new SelectListItem { Text = item.Type + " " + item.District_Name, Value = item.Id.ToString() });
                ViewBag.district = liDistricts;
            }

            //Get current faculties
            List<SelectListItem> liFaculties = new List<SelectListItem>();
            if (model.Faculties_In_Companies != null) {
                ViewBag.comFaculties = new MultiSelectList(db.Faculties, "Id", "Name", model.Faculties_In_Companies.Where(x => x.Company_Id == user.Company_id).Select(x => x.Faculty_Id));

            }
            //For clear cache in logoUpdate
            Random randomKey = new Random();
            ViewBag.Random = randomKey.Next(1, 1000).ToString();
            //Check Scale Value
            if (model.Scale_from == null) {
                model.Scale_from = 0;
            }
            if (model.Scale_to == null) {
                model.Scale_to = 0;
            }
            //Check Update Status == Success
            if (status == "UpdateSuccess") {
                ViewBag.update = "UpdateSuccess";


            }
            else if (status == "ChangePasswordSuccess") {
                ViewBag.update = "ChangePasswordSuccess";
            }
            else if (status == "UserUpdateSuccess") {
                ViewBag.update = "UserUpdateSuccess";
            }
            //Check Update Status == Fail
            else if (status == "ChangePasswordFail") {
                ViewBag.update = "ChangePasswordFail";
                //Get error list to show message
                ViewBag.errorlist = errorList;
            }
            return View(model);
        }
        // Thuan Nguyen - POST: Update Company Info
        [HttpPost]
        public ActionResult Update(Company_Info comUpdate) {
            string status = null;
            //Get Logo
            HttpPostedFileBase file = Request.Files["Com_Logo"];
            //Get current Company_Info need to update
            var newRecord = db.Company_Info.FirstOrDefault(m => m.Id == comUpdate.Id);
            if (ModelState.IsValid) {
                //Get Scale Range
                var scale = comUpdate.Scale_Range.Split(',');
                //Update db
                newRecord.Name_Company = comUpdate.Name_Company;
                newRecord.Introduce_Company = comUpdate.Introduce_Company;
                newRecord.Address_Company = comUpdate.Address_Company;
                newRecord.Province_ID = comUpdate.Province_ID;
                newRecord.District_ID = comUpdate.District_ID;
                newRecord.Contact_Phone = comUpdate.Contact_Phone;
                newRecord.Contact_Email = comUpdate.Contact_Email;
                newRecord.website_Company = comUpdate.website_Company;
                newRecord.Faculties_Other = comUpdate.Faculties_Other;
                newRecord.Scale_from = int.Parse(scale[0].ToString());
                newRecord.Scale_to = int.Parse(scale[1].ToString());
                newRecord.Representative = comUpdate.Representative;


                //Update Faculties
                if (comUpdate.FacRec != null) {
                    //Delete old data first
                    int query = db.Database.ExecuteSqlCommand("Delete from Faculties_In_Companies where Company_Id = " + comUpdate.Id);
                    //Add new record
                    foreach (var item in comUpdate.FacRec) {
                        db.Faculties_In_Companies.Add(new Faculties_In_Companies {
                            Company_Id = newRecord.Id,
                            Date_Created = DateTime.Now,
                            Faculty_Id = item
                        });
                        db.SaveChanges();
                    }
                }
                //Processing Logo upload
                if (file.FileName != "") {
                    // If content length is zero means no file attached
                    if (file.ContentLength == 0) {
                        ModelState.AddModelError("", "Không thể tải lên tệp, vui lòng kiểm tra lại!");
                        ListAll();
                        return View(comUpdate);
                    }
                    else {
                        //Change name of logo image to Company_ID (Ex: 10)
                        string filename = comUpdate.Id.ToString();
                        //Map Path
                        string directory = Server.MapPath("~/App_Data/Company/Logos");
                        if (!Directory.Exists(directory)) {
                            Directory.CreateDirectory(directory);
                        }
                        //Generate path to save file
                        string path = System.IO.Path.Combine(directory, filename);
                        // If same name of file present then delete that file first
                        if (System.IO.File.Exists(path)) {
                            System.IO.File.Delete(path);
                        }
                        //Save image to physical folder
                        file.SaveAs(path);
                    }
                }
                //Update DB
                db.Entry(newRecord).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                status = "UpdateSuccess";
            }
            else {
                status = "UpdateFail";
            }
            return RedirectToAction("Update", new { @status = status });
        }
        [HttpPost]
        public ActionResult UpdateUser(FormCollection userUpdate) {
            string status = "UserUpdateFail";
            var crrUser = User.Identity.GetUserId();
            var newRecord = db.User_In_Company.FirstOrDefault(m => m.Account_id == crrUser);
            if (ModelState.IsValid) {
                //get new data
                newRecord.FullName = userUpdate["FullName"].ToString();
                //save new data
                db.Entry(newRecord).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                status = "UserUpdateSuccess";
            }

            return RedirectToAction("Update", new { @status = status });
        }

        //Thuan Nguyen - GET: Create employee in Company
        [HttpGet]
        public ActionResult CreateEmployee(string message = null) {
            //Check create new employee permission of current user
            var crrUser = User.Identity.GetUserId();
            var crrPermission = db.User_In_Actions.FirstOrDefault(x => x.Account_Id == crrUser && x.Action_Id == 1);
            if (crrPermission != null) {
                //get list actions
                ViewBag.Actions = db.Actions;
                ViewBag.Permission = true;
            }
            else {
                ViewBag.Permission = false;
            }
            //Check message
            if (message == "CreateSuccess") {
                ViewBag.Message = "CreateSuccess";
            }
            else if (message == "CreateFail") {
                ViewBag.Message = "CreateFail";
            }
            return View();
        }
        //Thuan Nguyen - POST: Create employee in Company
        [HttpPost]
        public ActionResult CreateEmployee(FormCollection formCollection, EmployeeRegisterViewModel model) {
            //Check data from view
            if (ModelState.IsValid) {
                if (checkEmailAlreadyExist(model.Email) == "false") {
                    ModelState.AddModelError("Email", "Email đã có người đăng ký! Vui lòng kiểm tra lại!");
                    return View(model);
                }
                else {
                    //Get current user info
                    var crrUser = User.Identity.GetUserId();
                    var crrCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == crrUser);
                    //Create User in company
                    //Register new Company Account---------
                    ApplicationDbContext context = new ApplicationDbContext();
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    //Create new account
                    var user = new ApplicationUser();
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.EmailConfirmed = true; //Activated Account
                    string userPassWord = model.ConfirmPassword;
                    var chkUser = userManager.Create(user, userPassWord);
                    //If register success, add user role "Company"
                    if (chkUser.Succeeded) {
                        var resultUser = userManager.AddToRole(user.Id, "Company");
                        //Create User In Company
                        User_In_Company userInCompany = new User_In_Company {
                            Account_id = user.Id,
                            Company_id = crrCom.Company_id,
                            Status_id = 1,
                            FullName = model.FullName
                        };
                        //Check actions was set for this user
                        foreach (var item in db.Actions) {
                            //Check if checkbox is checked
                            if (!string.IsNullOrEmpty(formCollection["act_" + item.Id])) {
                                db.User_In_Actions.Add(new User_In_Actions {
                                    Account_Id = user.Id,
                                    Action_Id = item.Id,
                                    Date_Create = DateTime.Now
                                });
                            }
                        }
                        //Save
                        db.User_In_Company.Add(userInCompany);
                        db.SaveChanges();
                        //return
                        return RedirectToAction("CreateEmployee", "Account", new { area = "Company", @message = "CreateSuccess" });
                    }
                    else {
                        foreach (var err in chkUser.Errors) {
                            ModelState.AddModelError("", err);
                        }
                        //Check create new employee permission of current user
                        var crrPermission = db.User_In_Actions.FirstOrDefault(x => x.Account_Id == crrUser && x.Action_Id == 1);
                        if (crrPermission != null) {
                            //get list actions
                            ViewBag.Actions = db.Actions;
                            ViewBag.Permission = true;
                        }
                        else {
                            ViewBag.Permission = false;
                        }
                        return View(model);
                        //return RedirectToAction("CreateEmployee", "Account", new { area = "Company", @message = "CreateFail" });
                    }
                }

            }
            else {
                //Check create new employee permission of current user
                var crrUser = User.Identity.GetUserId();
                var crrPermission = db.User_In_Actions.FirstOrDefault(x => x.Account_Id == crrUser && x.Action_Id == 1);
                if (crrPermission != null) {
                    //get list actions
                    ViewBag.Actions = db.Actions;
                    ViewBag.Permission = true;
                }
                else {
                    ViewBag.Permission = false;
                }
                return View(model);
            }
        }
        //Thuan Nguyen - GET: Company view list employee
        [HttpGet]
        public ActionResult ListEmployee() {
            //Current User & Com
            var crrUser = User.Identity.GetUserId();
            var crrCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == crrUser);

            //Get list of employee in company
            var model = db.User_In_Company.Where(x => x.Company_id == crrCom.Company_id).ToList();
            //Num employee
            ViewBag.numEmployee = model.Count();

            return View(model);
        }

    }
}