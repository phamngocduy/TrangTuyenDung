using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Admin.Controllers {
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController() {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        // GET: Admin/Account
        [HttpGet]
        public ActionResult Create_Staff() {
            //createcompanydemo();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create_Staff(StaffAccountSuppor data) {
            var db = new EJobEntities();
            if (ModelState.IsValid) {
                var q = await UserManager.FindByNameAsync(data.Email);
                var user = new ApplicationUser { UserName = data.Email, Email = data.Email };

                var result = await UserManager.CreateAsync(user, data.Password);
                //Add user to role Company
                if (result.Succeeded) {
                    var resultUser = UserManager.AddToRole(user.Id, "Staff");
                    db.SaveChanges();
                    var a = db.AspNetUsers.FirstOrDefault(x => x.Id == user.Id);

                    //
                    Staff_Info staff = new Staff_Info() {
                        Account_Id = user.Id,
                        Staff_Name = data.Email,
                        Staff_Create_at = DateTime.Now,
                        Status_Id = 1,
                    };
                    db.Staff_Info.Add(staff);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }


        //private void createcompanydemo()
        //{
        //    var db = new EJobEntities();
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationDbContext context = new ApplicationDbContext();
        //        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        //        var user = new ApplicationUser();
        //        user.UserName = "Vanlanguniversity@gmail.com";
        //        user.Email = "Vanlanguniversity@gmail.com";
        //        user.EmailConfirmed = true; //Default is not active account
        //        string userPWD = "Password99%"; //When ConfirmPass is compared with Password Textbox
        //        var chkUser = userManager.Create(user, userPWD);
        //        //Add user to role Company
        //        if (chkUser.Succeeded)
        //        {
        //            var resultUser = userManager.AddToRole(user.Id, "Company");
        //        }
        //        //
        //        Company_Info company = new Company_Info()
        //        {
        //            Name_Company="Văn Lang University",
        //            Created_at = DateTime.Now,
        //            Province_ID = 79, //Default is Ho Chi Minh City
        //            District_ID = 760, // Default is District 01
        //            Status = 1 ,// Chưa Đăng Ký
        //            Slug_Company= "Van-Lang-University"
        //        };
        //        db.Company_Info.Add(company);
        //        db.SaveChanges();
        //        User_In_Company usCom = new User_In_Company()
        //        {
        //            Account_id = user.Id,
        //            Company_id = company.Id,
        //            Status_id = 1,
        //            FullName = "Văn Lang"
        //        };
        //        db.User_In_Company.Add(usCom);
        //        db.SaveChanges();
        //    }

        //}

        [HttpGet]
        public ActionResult Show_ListAccount() {
            var db = new EJobEntities();
            var model = db.AspNetUsers.Where(x => x.AspNetRoles.Select(role => role.Name).Contains("Staff")).ToList();
            return View(model);
        }
    }
}