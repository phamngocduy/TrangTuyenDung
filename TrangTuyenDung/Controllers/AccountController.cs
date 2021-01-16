using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TrangTuyenDung.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.Net;
using TrangTuyenDung.Sercurity_SendMail;

namespace TrangTuyenDung.Controllers {
    [Authorize]
    public class AccountController : Controller {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        EJobEntities db = new EJobEntities();
        SendMail sm = new SendMail();
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string message = null) {
            if (Request.IsAuthenticated) {
                TempData["messLogOut"] = "Vui lòng đăng xuất trước khi đổi tài khoản!";
                return RedirectToAction("IndexCallBack", "Home", new { arae = "" });
            }
            EJobEntities db = new EJobEntities();
            //Get number of Student
            var num_member = db.Student_Info.Where(x => x.Status_Id == 1).Count();
            //Get number of Company
            var num_company = db.Company_Info.ToList().Count();
            //Add to ViewBag
            ViewBag.com = num_company;
            ViewBag.mem = num_member;
            //Get message
            if (message == "CantLogin") {
                ViewBag.Message = "CantLogin";
            }
            else if (message == "Blocked") {
                ViewBag.Message = "Blocked";
            }
            else if (message == "Success") {
                ViewBag.Message = "Success";
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            EJobEntities db = new EJobEntities();
            //Get number of Student
            var num_member = db.Student_Info.Where(x => x.Status_Id == 1).Count();
            //Get number of Company
            var num_company = db.Company_Info.ToList().Count();

            if (!ModelState.IsValid) {
                //Add to ViewBag
                ViewBag.com = num_company;
                ViewBag.mem = num_member;

                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            //get info user login
            var user = await UserManager.FindByNameAsync(model.Email);

            Session["user"] = User.Identity.GetUserName();
            //check user confirm mail
            if (user != null) {
                if (!user.EmailConfirmed && result == SignInStatus.Success) {
                    ViewBag.mess = "Vui lòng xác minh qua Email để kích hoạt tài khoản!!";
                    return View(model);
                }
            }

            switch (result) {
                case SignInStatus.Success:
                    //get user role
                    var roles = await UserManager.GetRolesAsync(user.Id);
                    //check user role admin
                    if (roles.Contains("Admin")) {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    //check user role company
                    else if (roles.Contains("Company")) {
                        //---Check Status of Company
                        //Check current com
                        var crrCom = db.User_In_Company.FirstOrDefault(x => x.Account_id == user.Id && x.Company_Info.Status == 1); //Status 1 is Active
                        if (crrCom != null) {
                            return RedirectToAction("Dashboard", "Home", new { area = "Company" });
                        }
                        else {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return RedirectToAction("Login", "Account", new { area = "", @message = "Blocked" });
                        }

                    }
                    //check user role staff
                    else if (roles.Contains("Staff")) {
                        return RedirectToAction("Dashboard", "Home", new { area = "Staff" });
                    }
                    //check user role staff
                    else if (roles.Contains("Manager")) {
                        return RedirectToAction("Dashboard", "Home", new { area = "Manager" });
                    }
                    //check user role student
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác! \n    Vui lòng kiểm tra lại!");
                    ViewBag.com = num_company;
                    ViewBag.mem = num_member;
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe) {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync()) {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code) {
            if (userId == null || code == null) {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword(string message = null) {
            if(message == "false") {
                ViewBag.Message = "false";
            }

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id))) {
                    // Don't reveal that the user does not exist or is not confirmed

                    return RedirectToAction("ForgotPassword", "Account", new { area = "", @message = "false" });
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                sm.SendMail_Reset(model.Email, callbackUrl);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code) {
          
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null) {
                // Don't reveal that the user does not exist
                TempData["Err"] = "Email không tồn tại";
                return View();
            }
            String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.Password);
            user.PasswordHash = hashedNewPassword;
            UserManager.Update(user);
            return RedirectToAction("Login","Account", new { area = "", @message = "Success" });
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl) {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe) {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null) {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model) {
            if (!ModelState.IsValid) {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider)) {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl) {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            loginInfo = loginInfo ?? Session["ExternalLoginInfo"] as ExternalLoginInfo;
            if (loginInfo == null) {
                return RedirectToAction("Login", "Account", new { area = "", @message = "CantLogin" });
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            //get info user login
            var user = await UserManager.FindByNameAsync(loginInfo.Email);
            switch (result) {
                case SignInStatus.Success:
                    //check user role stident
                    if (await UserManager.IsInRoleAsync(user.Id, "Student") && returnUrl == null) {
                        Student_Info StuInfo = new Student_Info();
                        return RedirectToAction("Dashboard", "Account", new { area = "Student" });
                    }
                    //return RedirectToLocal(returnUrl);
                    return Redirect(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    EJobEntities db = new EJobEntities();
                    ViewBag.Faculties_Id = new SelectList(db.Faculties, "Id", "Name");
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl) {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid) {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null) {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //create new variable student
                var student = new Create_Student { Id = user.Id, Email = model.Email, UserName = model.Email, Faculties_Id = model.Faculties_Id, Student_Gender = model.Student_Gender, Student_Birthday = model.Student_Birthday };
                //check using van lang mail
                var user1 = await UserManager.FindByNameAsync(info.Email);

                //
                if (user1 != null) {
                    var addLoginResult = await UserManager.AddLoginAsync(user1.Id, info.Login);
                    if (addLoginResult.Succeeded) {
                        await SignInManager.SignInAsync(user1, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else {
                    var result = await UserManager.CreateAsync(user);
                    var check = CreateStudent(student);
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    if (result.Succeeded && check == true) {
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded) {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
                //
                // if (info.Login.LoginProvider=="Văn Lang" && model.Email.Contains("@vanlanguni.vn"))
                //  {

                // }
                //else
                //{
                //    return View("ExternalLoginFailure");
                //}
            }
            EJobEntities db = new EJobEntities();
            ViewBag.Faculties_Id = new SelectList(db.Faculties, "Id", "Name");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure() {
            return View();
        }

        //create student_info
        public bool CreateStudent(Create_Student user) {
            using (EJobEntities db = new EJobEntities()) {

                bool gender = false;
                if (user.Student_Gender == "Male") {
                    gender = true;
                }

                Student_Info nStudent = new Student_Info {
                    Account_Id = user.Id,
                    Status_Id = 1,
                    Student_Name = user.UserName.Split('@')[0],
                    Student_Create_at = DateTime.Now,
                    Faculties_Id = user.Faculties_Id,
                    Student_Gender = gender,
                    Student_Birthday = user.Student_Birthday
                };
                db.Student_Info.Add(nStudent);
                try {
                    db.SaveChanges();
                    var check = UserManager.AddToRole(user.Id, "Student");
                    return true;
                }
                catch { }

            }
            return false;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_userManager != null) {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null) {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        public static string GenerateToken(int length, Random random) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) {
            }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context) {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null) {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}