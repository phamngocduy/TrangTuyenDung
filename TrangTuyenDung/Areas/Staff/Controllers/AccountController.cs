using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Areas.Staff.Models;
using TrangTuyenDung.Models;
using TrangTuyenDung.Sercurity_SendMail;
using ClosedXML.Excel;
using System.Data;
using System.Text.RegularExpressions;

namespace TrangTuyenDung.Areas.Staff.Controllers {
    [Authorize(Roles = "Staff")]
    public class AccountController : Controller {
        EJobEntities db = new EJobEntities();
        //using AccountController of Company
        Company.Controllers.AccountController comAccController = new Company.Controllers.AccountController();
        SendMail sm = new SendMail();
        // Thuan Nguyen - GET: Staff Create New Company
        public ActionResult CreateCompany() {
            //list company name for autocomplete
            var listCompanyName = db.Company_Info.Select(x => x.Name_Company).ToList();
            string formatCompanyName = "";
            foreach (var item in listCompanyName) {
                formatCompanyName += item + "|";
            }
            ViewBag.Companies = formatCompanyName;

            return View();
        }
        // Thuan Nguyen - POST: Staff Create New Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCompany(CreateCompanyViewModel comModel) {
            //Get logo file
            HttpPostedFileBase file = Request.Files["Com_Logo"];

            //Check View Input
            if (ModelState.IsValid) {
                //Generate token
                Random rand = new Random();
                var token = GenerateToken(20, rand);

                //Add info to Company_Info db
                Company_Info comInfo = new Company_Info() {
                    Name_Company = comModel.Com_Name,
                    Representative= comModel.Representative,
                    Contact_Email=comModel.Email_contact,
                    Contact_Phone=comModel.phone,
                    Token=token,
                    Created_at = DateTime.Now,
                    Province_ID = 79, //Default is Ho Chi Minh City
                    District_ID = 760, // Default is District 01
                    Status = 3 // Chưa Đăng Ký
                };
                comInfo.Slug_Company = comAccController.GenerateSlug(comInfo.Name_Company);

                //Send to Database
                db.Company_Info.Add(comInfo);
                //Processing Logo upload
                if (file != null) {
                    // If content length is zero means no file attached
                    if (file.ContentLength == 0) {
                        ModelState.AddModelError("", "Không thể tải lên tệp, vui lòng kiểm tra lại!");
                        ViewBag.Message = "Lỗi khi tải lên tệp!";
                        return View(comModel);
                    }
                    else {

                        //Save Com info to DB
                        db.SaveChanges();
                        //Get info of image
                        //string imgName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                        //string imgExtension = Path.GetExtension(file.FileName);
                        //Change name of logo image to Company_ID.extension (Ex: 10.jpg)
                        string newName = comInfo.Id.ToString();
                        //string physicalPath = Server.MapPath("~/Areas/Company/Images/Avatars/" + imageName)
                        string path = Path.Combine(Server.MapPath("~/App_Data/Company/Logos/"), newName);
                        // If same name of file present then delete that file first
                        if (System.IO.File.Exists(path)) {
                            System.IO.File.Delete(path);
                        }
                        //Save image to physical folder
                        file.SaveAs(path);
                    }

                }
                else {
                    ViewBag.Message = "Lỗi khi tải lên tệp!";
                    return View(comModel);
                }
            
                ViewBag.Message = "Công ty mới đã được thêm vào!";
                sm.Auto_SendEmail_Invite_toCompany(comModel.Com_Name, comModel.Email_contact, token);
                return RedirectToAction("ApprovedList", "Account", new { area = "Staff" });
            }
            else {
                return View(comModel);
            }
        }
        // Thuan Nguyen - GET: Staff/Company_Registration/: "List Company Pending"
        [HttpGet]
        public ActionResult Pending(string message = null) {
            var model = db.Company_Registration.ToList();
            //check message
            if (message == "ApproveSuccess") {
                ViewBag.Message = "ApproveSuccess";
            }
            else if (message == "RejectSuccess") {
                ViewBag.Message = "RejectSuccess";
            }
            else if (message == "LogoFail") {
                ViewBag.Message = "LogoFail";
            }
            else if (message == "ApproveFail") {
                ViewBag.Message = "ApproveFail";
            }
            return View(model);
        }
        // Thuan Nguyen - POST: Staff/Approved: "Company from "Pending" to "Approved" "
        public ActionResult Approved(int id) {
            var comRegister = db.Company_Registration.FirstOrDefault(x => x.Id == id);
            //Register new Company Account---------
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //Create new account
            var user = new ApplicationUser();
            user.UserName = comRegister.Email;
            user.Email = comRegister.Email;
            user.EmailConfirmed = true; //Activated Account
            string userPassWord = comRegister.Password;
            var chkUser = userManager.Create(user, userPassWord);
            //If register success, add user role "Company"
            if (chkUser.Succeeded) {
                var resultUser = userManager.AddToRole(user.Id, "Company");

                //Create new Company_Info
                Company_Info comInfo = new Company_Info() {
                    Name_Company = comRegister.Name_Company,
                    Contact_Email = comRegister.Contact_Email,
                    Contact_Phone = comRegister.Contact_Phone,
                    Introduce_Company = comRegister.Introduce_Company,
                    website_Company = comRegister.website_Company,
                    Address_Company = comRegister.Address_Company,
                    Province_ID = comRegister.Province_ID,
                    District_ID = comRegister.District_ID,
                    Created_at = DateTime.Now,
                    Slug_Company = comRegister.Slug_Company,
                    Faculties_Other = comRegister.Faculties_Other,
                    //Active this company
                    Status = 1 //1 is Activated
                };
                db.Company_Info.Add(comInfo);

                //Convert Company_Logo from base64 to physical file
                if (comRegister.ImageBase64 != null) {
                    try {
                        Image img = base64ToImage(comRegister.ImageBase64.ToString());
                        string directory = Server.MapPath("~/App_Data/Company/Logos");
                        if (!Directory.Exists(directory)) {
                            Directory.CreateDirectory(directory);
                        }
                        //Save Info first to get Id
                        db.SaveChanges();
                        //Save image to physical
                        string filename = comInfo.Id.ToString();
                        string filenameWithExtension = filename;
                        string path = System.IO.Path.Combine(directory, filenameWithExtension);
                        // If same name of file present then delete that file first
                        if (System.IO.File.Exists(path)) {
                            System.IO.File.Delete(path);
                        }
                        img.Save(path);
                        img.Dispose();
                    }
                    catch (Exception ex) {
                        Response.Write("<script language='javascript'>alert(" + ex.Message + ")</script>");
                    }
                }
                else {
                    ViewBag.AprroveError = "Không tìm thấy Logo của công ty, vui lòng liên hệ bộ phận hỗ trợ!";
                    return RedirectToAction("Pending", "Account", new { area = "Staff", @message = "LogoFail" });
                }
                // Save Faculties Value
                if (comRegister.Faculties != null) {
                    //Convert from string to int[]
                    string[] sFac = comRegister.Faculties.Split(',');
                    int[] iFac = new int[sFac.Length];
                    for (int i = 0; i < sFac.Length; i++) {
                        iFac[i] = int.Parse(sFac[i]);
                    }
                    foreach (var item in iFac) {
                        db.Faculties_In_Companies.Add(new Faculties_In_Companies {
                            Company_Id = comInfo.Id,
                            Date_Created = DateTime.Now,
                            Faculty_Id = item
                        });
                        db.SaveChanges();
                    }
                }
                //Create User In Company
                User_In_Company userInCompany = new User_In_Company {
                    Account_id = user.Id,
                    Company_id = comInfo.Id,
                    Status_id = 1,
                    FullName = comRegister.FullName
                };
                //Default first account can create new other account
                db.User_In_Actions.Add(new User_In_Actions {
                    Account_Id = user.Id,
                    Action_Id = 1,
                    Date_Create = DateTime.Now
                });

                db.User_In_Company.Add(userInCompany);
                db.SaveChanges();
                //Send Successful Email to Company
                string result = sm.SendEmail_ApproveAccount_toCompany(id);
                ViewBag.EmailStatus = result;
                //Remove this record in Company_Registration
                db.Company_Registration.Remove(comRegister);
                db.SaveChanges();

                //Return Approve 
                return RedirectToAction("Pending", "Account", new { area = "Staff", @message = "ApproveSuccess" });

            }
            else {
                return RedirectToAction("Pending", "Account", new { area = "Staff", @message = "ApproveFail" });
            }

        }
        // Base64 to Image file
        public Image base64ToImage(string base64string) {
            //Convert Base64 to bytes[]
            byte[] imageBytes = Convert.FromBase64String(base64string);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            //Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image img = Image.FromStream(ms, true);
            return img;
        }
        // Thuan Nguyen - POST: Company_Residtration/Delete/id?: "Reject Company Pending"
        [HttpPost]
        public ActionResult Reject(int? id, FormCollection formCollection) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company_Registration values = db.Company_Registration.Find(id);
            //Send Reject email before remove this Company
            string result = sm.SendEmail_RejectAccount_toCompany(id, formCollection);
            ViewBag.EmailStatus = result;
            //Removing rejected company
            db.Company_Registration.Remove(values);
            db.SaveChanges();
            return RedirectToAction("Pending", "Account", new { area = "Staff", @message = "RejectSuccess" });

        }
        // Thuan Nguyen - POST: Company_Registration/Delete/id: "Reject Comfirm" 

        // Thuan Nguyen - GET: Company_Info/List: "List of Company Approved"
        [HttpGet]
        public ActionResult ApprovedList(string message = null) {
            var model = db.Company_Info.OrderByDescending(x => x.Id).ToList();
            //Check Message
            if (message == "BlockSuccess") {
                ViewBag.Message = "BlockSuccess";
            }
            else if (message == "ActiveSuccess") {
                ViewBag.Message = "ActiveSuccess";
            }
            else if (message == "InviteSuccess") {
                ViewBag.Message = "InviteSuccess";
            }
            else if (message == "NeedEmail") {
                ViewBag.Message = "NeedEmail";
            }
            else if (message == "SendError") {
                ViewBag.Message = "SendError";
            }
            return View(model);
        }
        // POST: Company_Info/Block
        // GET: Company_Info/BlockedList
        //---------------Email Sending------------------//
        // GET: Email View Configs
        public ActionResult EmailConfig(string emailType) {
            var model = db.Email_Configs.FirstOrDefault(x => x.Email_Type == emailType);
            return View(model);
        }
        // POST: Update Email Configs
        [HttpPost]
        public ActionResult EmailConfig(Email_Configs model) {
            var newRecord = db.Email_Configs.FirstOrDefault(x => x.Id == model.Id);
            if (ModelState.IsValid) {
                //Account Config
                if (newRecord.Email_Type == "ACCOUNT") {
                    newRecord.Email = model.Email;
                    newRecord.Password = model.Password;
                    newRecord.Signature = model.Signature;
                    newRecord.Content_Header = model.EmailSubject;
                }
                //Approve and Reject Email Config
  
                    newRecord.EmailSubject = model.EmailSubject;
                    newRecord.Content_Header = model.Content_Header;
                    newRecord.Content_Footer = model.Content_Footer;
                    newRecord.Signature = model.Signature;
                
                //Update State of row
                db.Entry(newRecord).State = System.Data.Entity.EntityState.Modified;
                int result = db.SaveChanges();
                if (result == 1) {
                    ViewBag.Message = "Cập nhật thành công!";
                }
                else {
                    ViewBag.Message = "Có lỗi khi cập nhật! Vui lòng thử lại";
                }
            }
            return View(newRecord);
        }

        // GET: View Company Details
        public ActionResult ViewPendingCompanyDetail(int id) {
            var model = db.Company_Registration.FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        // Thuan Nguyen - POST: Change Status of Company
        public ActionResult ChangeCompanyStatus(int? id) {
            if (id != null) {
                //Get company Info
                var model = db.Company_Info.FirstOrDefault(x => x.Id == id);
                if (model.Status == 1) {
                    //Block
                    model.Status = 2;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ApprovedList", "Account", new { area = "Staff", @message = "BlockSuccess" });
                }
                else if (model.Status == 2) {
                    //Active
                    model.Status = 1;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ApprovedList", "Account", new { area = "Staff", @message = "ActiveSuccess" });
                }

            }
            else {
                throw new Exception("Item Not Found!");
            }

            return View();
        }
        //Thuan Nguyen - POST: Invite Company
        [HttpPost]
        public ActionResult InviteCompany(int? id, FormCollection formCollection) {
            if (id != null) {
                //Get value
                var model = db.Company_Info.FirstOrDefault(x => x.Id == id);
                if (model != null) {
                    if (!string.IsNullOrEmpty(formCollection["Email"])) {
                        //Generate token
                        Random rand = new Random();
                        var token = GenerateToken(20, rand);
                        //Get number of applying candidate
                        var num_Applying = db.Apply_Recruitments.Where(x => x.Recruitment.Company_id == model.Id).Count();
                        //Update new record
                        model.Status = 4;
                        model.Contact_Email = formCollection["Email"].ToString();
                        model.Token = token;
                        //update to db
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //Send Email
                        var result = sm.SendEmail_Invite_toCompany(model.Name_Company, formCollection["Email"].ToString(), formCollection["mail-content"].ToString(), formCollection["mail-sign"].ToString(),num_Applying, token);
                        if (result == false) {
                            return RedirectToAction("ApprovedList", "Account", new { area = "Staff", @message = "SendError" });
                        }
                        return RedirectToAction("ApprovedList", "Account", new { area = "Staff", @message = "InviteSuccess" });
                    }
                    else {
                        return RedirectToAction("ApprovedList", "Account", new { area = "Staff", @message = "NeedEmail" });
                    }

                }
                else {
                    throw new Exception("Item Not Found!");
                }


            }
            else {
                throw new Exception("Item Not Found!");
            }

        }
        //Thuan Nguyen - Generate Random Token
        public static string GenerateToken(int length, Random random) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RemoveHtmlTags(string strHtml) {
            if(strHtml == null) {
                return strHtml;
            }
            string strText = Regex.Replace(strHtml, "<(.|\n)*?>", String.Empty);
            strText = HttpUtility.HtmlDecode(strText);
            strText = Regex.Replace(strText, @"\s+", " ");
            return strText;
        }
        public ActionResult Export_Company(string startDay, string endDay) {
          
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.Company_Info.Where(x => x.Created_at >= start && x.Created_at <= end ).OrderByDescending(x => x.Id).ToList();
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("Ngày tạo"),
                                            new DataColumn("Tên công ty"),
                                            new DataColumn("Họ tên"),
                                            new DataColumn("Mã số công ty"),
                                            new DataColumn("Địa chỉ email"),
                                            new DataColumn("Số điện thoại liên lạc"),
                                            new DataColumn("Giới thiệu"),
                                            new DataColumn("Website"),
                                            new DataColumn("Địa chỉ"),
                                            new DataColumn("Ngành")});
                foreach(var com in model) {
                    var fal = db.Faculties_In_Companies.Where(x => x.Company_Id == com.Id).ToList();
                    var faculty = "";
                    var user = db.User_In_Company.FirstOrDefault(x => x.Company_id == com.Id && com.Status == 1) ;
                    var fullname = "";
                    foreach(var item in fal) {
                        var fa = db.Faculties.FirstOrDefault(x => x.Id == item.Faculty_Id);
                        faculty += fa.Name+",";
                    }
                    if(user != null) {
                        fullname = user.FullName;
                    }
                    var district = db.Districts.FirstOrDefault(x => x.Id == com.District_ID);
                    var province = db.Provinces.FirstOrDefault(x => x.Id == com.Province_ID);
                    var add = com.Address_Company +","+ district.District_Name+"," + province.Name;
                    dt.Rows.Add(com.Created_at,com.Name_Company, fullname, com.Code,com.Contact_Email,com.Contact_Phone,com.Introduce_Company,com.website_Company,add,faculty);
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Company.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }
        public ActionResult Export_Recuitments(string startDay, string endDay) {
           
            var start = DateTime.Parse(startDay);
            var end = DateTime.Parse(endDay);
            var model = db.Recruitments.Where(x => x.Created_date >= start && x.Created_date <= end).OrderByDescending(x => x.Id).ToList();
            if (model != null) {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[17] { new DataColumn("Tiêu đề"),
                                            new DataColumn("Tên công ty"),
                                            new DataColumn("Mức lương"),
                                            new DataColumn("Lượt xem"),
                                            new DataColumn("Tỉnh/Tp"),
                                            new DataColumn("Quận/Huyện"),
                                            new DataColumn("Ngày tuyển dụng"),
                                            new DataColumn("Ngày hết hạn"),
                                            new DataColumn("Hình thức tuyển dụng"),
                                             new DataColumn("Số lượng"),
                                            new DataColumn("Vị trí công việc"),
                                            new DataColumn("Ngành"),
                                            new DataColumn("Địa chỉ"),
                                            new DataColumn("Mô tả công việc"),
                                            new DataColumn("Kỹ năng yêu cầu"),
                                            new DataColumn("Lợi ích"),
                                            new DataColumn("Ngày đăng")});
                foreach (var rec in model) {

                    var major = db.Majors_In_Recruitments.Where(x => x.Id == rec.Id).ToList();
                    var major_list = "";
                    foreach (var item in major) {
                        var ma_name = db.Faculties_Majors.FirstOrDefault(x => x.Id == item.Major_Id);
                        major_list += ma_name.Name+", ";
                    }
                    var district = db.Districts.FirstOrDefault(x => x.Id == rec.Districts_id);
                    var province = db.Provinces.FirstOrDefault(x => x.Id == district.ProvinceId);
                    var salary = rec.Salary_from + "-" + rec.Salary_to;
                    var company = db.Company_Info.FirstOrDefault(x => x.Id == rec.Company_id);
                    var company_name = "";
                    if(company  != null) {
                        company_name = company.Name_Company;
                    }
                    string skill = "";
                    string optional = "";
                    string descript = "";
                    string benefits = "";
                    var work = "";
                    var num = "";
                    var work_type = "";
                    var num_hire="";
                    var posion = db.Job_Positions.FirstOrDefault(x => x.Id == rec.Job_Position_Id);
                    var posion_name = "";
                    if (posion != null) {
                       posion_name = posion.Name;
                    }
                    if (rec.Is_Full_Time == true) {
                        work +="Full time"+",";
                        num +=rec.Num_FullTime + ",";
                    }
                    if (rec.Is_Part_Time == true) {
                        work +="Part time" + ",";
                        num +=rec.Num_PartTime + ",";
                    }
                    if (rec.Is_Intership == true) {
                        work +="Thực tập" + ",";
                        num +=rec.Num_Intern + ",";
                    }
                   if(work.Length >0) {
                       work_type= work.Substring(0, work.Length - 1);
                    }
                   
                    if (num.Length > 0) {
                       num_hire= num.Substring(0, num.Length - 1);
                    }
                   
                     skill = RemoveHtmlTags(rec.Required_Skills);
                    
                    
                     optional = RemoveHtmlTags(rec.Job_Optional);
                    
                    
                     descript = RemoveHtmlTags(rec.Job_Description);
                    
                    
                     benefits = RemoveHtmlTags(rec.Job_Benefits);
                  

                   
                    dt.Rows.Add(rec.title, company_name, salary, rec.Nums_view, province.Name, district.District_Name, rec.Recruiting_dates, rec.Expire_date, work_type, num_hire, posion_name, major_list, optional, descript, skill, benefits, rec.Created_date);
                }
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Recruitment.xlsx");
                    }
                }
            }
            return HttpNotFound();
        }
       
    }
}