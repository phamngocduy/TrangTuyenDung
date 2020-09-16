using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Sercurity_SendMail {
    public class SendMail {
        EJobEntities db = new EJobEntities();
        UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        // POST: Send Approvement Email when click Approve Company
        // This function will send email to Contact_Email of Company
        public string SendEmail_ApproveAccount_toCompany(int? id) {
            //Get data from DB
            var ComRegister = db.Company_Registration.FirstOrDefault(x => x.Id == id);
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "APPROVE");
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Set Hello Meesage
            string email_Hello = "Xin chào " + ComRegister.Name_Company.ToString() + "! <br />";

            string subject= EmailTemplate.EmailSubject;
            string body = email_Hello + EmailTemplate.Content_Header + "<br />" + EmailTemplate.Content_Footer;
            string receiver = ComRegister.Contact_Email.ToString();

            var result = Send_mail(subject, body, receiver);
            if (result == true) {
                return ("Email đã được gửi thành công!");
            }
            return ("Email gửi không thành công! Vui lòng kiểm tra lại các trường, hoặc liên hệ bộ phận Hỗ trợ để được giải đáp!");

        }
        // POST: Send Rejected Email when click Reject Company
        // This function will send email to Contact_Email of Company attack list of reason
        public string SendEmail_RejectAccount_toCompany(int? id, FormCollection formCollection) {
            //Get Data from View Form
            string reasons = "";

            if (!string.IsNullOrEmpty(formCollection["option1"])) {
                reasons += "<strong> + Thông tin chưa đầy đủ! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option2"])) {
                reasons += "<strong>+ " + formCollection["other_reason"].ToString() + "</strong><br />";
            }
            //if (!string.IsNullOrEmpty(formCollection["option_other"])) {

            //}

            //Get data from DB
            var ComRegister = db.Company_Registration.FirstOrDefault(x => x.Id == id);
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "REJECT");
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Set Hello Meesage
            string email_Hello = "Xin chào " + ComRegister.Name_Company.ToString() + "! <br />";
            //--------------//
            string subject= EmailTemplate.EmailSubject;
            string body= email_Hello + EmailTemplate.Content_Header + reasons + EmailTemplate.Content_Footer + EmailTemplate.Signature;
            string receiver = ComRegister.Email.ToString();
            var result = Send_mail(subject, body, receiver);
            if (result == true) {
                return ("Email đã được gửi thành công!");
            }
            return ("Email gửi không thành công! Vui lòng kiểm tra lại các trường, hoặc liên hệ bộ phận Hỗ trợ để được giải đáp!");

        }
        // POST: Send Rejected Email when click Reject Company
        // This function will send email to Contact_Email of Company attack list of reason
        public string SendEmail_RejectRecruitment_toCompany(int? id, FormCollection formCollection) {
            //Get Data from View Form
            string reasons = "";

            if (!string.IsNullOrEmpty(formCollection["option1"])) {
                reasons += "<strong> + Thông tin chưa đầy đủ! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option2"])) {
                reasons += "<strong>+ " + formCollection["other_reason"].ToString() + "</strong><br />";
            }

            //Get data from DB
            var ComInfo = db.Company_Info.FirstOrDefault(x => x.Id == id);
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "REJECT_RECRUITMENT");
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Set Hello Meesage
            string email_Hello = "Xin chào " + ComInfo.Name_Company.ToString() + "! <br />";
            //--------------//
            string subject= EmailTemplate.EmailSubject;
            string body = email_Hello + EmailTemplate.Content_Header + reasons + EmailTemplate.Content_Footer + EmailTemplate.Signature;
            string receiver = ComInfo.Contact_Email.ToString();

            var result = Send_mail(subject, body, receiver);
            if(result == true) {
                return ("Email đã được gửi thành công!");
            }
            return ("Email gửi không thành công! Vui lòng kiểm tra lại các trường, hoặc liên hệ bộ phận Hỗ trợ để được giải đáp!");
           
        }
        // POST: Send Approvement Email when click Approve Company
        // This function will send email to Contact_Email of Company
        public bool SendEmail_Invite_toCompany(string comName, string contact_email,string content,string sign ,int num_Applying, string token) {
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "INVITE_COM");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //Get Rec Detail Url
            var registerLink = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("AccountRegister", "Account", new { area = "Company", @_ref = token })
                + "'>" + "Đăng ký ngay" + "</a>";

            //Set Hello Meesage
            string email_EJOB = "<strong> EJOB - Trang tin Việc Làm Văn Lang!" + "</strong>";
            string email_Hello = "Xin chào quý Công ty <strong><i>" + comName + "</i></strong>! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            //string email_Applying = " Hiện tại đang có <strong> <span style=\"color:rgb(255,0,0)\">" + num_Applying + " Ứng Viên </span></strong> đang đợi quý Công ty xét duyệt!!<br />";
            string email_RegisterLink = "<br /> Vui lòng truy cập vào link để đăng ký  " + registerLink + "<br />";

            string subject= EmailTemplate.EmailSubject;
            string body = content + email_Seperate + sign+email_RegisterLink;
            //string body= email_Seperate + email_EJOB + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />" + email_Applying + email_RegisterLink + EmailTemplate.Content_Footer + email_Seperate + EmailTemplate.Signature;
            string receiver = contact_email;
            var result = Send_mail(subject, body, receiver);
            if (result == true) {
                return true;
            }
            return false;
        }

        // auto send mail invite when create company account
        public bool Auto_SendEmail_Invite_toCompany(string comName, string contact_email, string token) {
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "INVITE_COM");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //Get Rec Detail Url
            var registerLink = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("AccountRegister", "Account", new { area = "Company", @_ref = token })
                + "'>" + "Đăng ký ngay" + "</a>";

            //Set Hello Meesage
            string email_EJOB = "<strong> EJOB - Trang tin Việc Làm Văn Lang!" + "</strong>";
            string email_Hello = "Xin chào quý Công ty <strong><i>" + comName + "</i></strong>! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            //string email_Applying = " Hiện tại đang có <strong> <span style=\"color:rgb(255,0,0)\">" + num_Applying + " Ứng Viên </span></strong> đang đợi quý Công ty xét duyệt!!<br />";
            string email_RegisterLink = "<br /> Vui lòng truy cập vào link để đăng ký  " + registerLink + "<br />";


            string body = email_Seperate + email_EJOB + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />"  + email_RegisterLink + EmailTemplate.Content_Footer + email_Seperate + EmailTemplate.Signature;
            string receiver = contact_email;
            var result = Send_mail(EmailTemplate.EmailSubject, body, receiver);
            if (result == true) {
                return true;
            }
            return false;
        }

        //Thuan Nguyen - POST: Auto send Email to Student for Hire
        // This function will send email to Student Email (@vanlanguni.vn)
        public bool SendEmail_Hire_toStudent(string stuID, string companyName, int? recID, string recTitle, FormCollection formCollection) {
            //Get Interview Date from View Form
            string workDate = "";
            workDate = formCollection["workDate"].ToString();
            //Get Notes from View Form
            string notes = "";
            string address = "";

            //Check if Company has noted
            if (!string.IsNullOrEmpty(formCollection["hireNoted"])) {
                notes += "- Ghi chú: <br /> <i>" + formCollection["hireNoted"].ToString() + "</i>";
            }
            else {
                notes = "";
            }
            if (!string.IsNullOrEmpty(formCollection["workAddress"])) {
                address = formCollection["workAddress"].ToString();
            }
            else {
                address = "Vui lòng xem tại Trang cá nhân của Công ty " + companyName + "!";
            }

            //Get Student Account
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "HIRE");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_recTitle = "- Vị trí ứng tuyển: " + checktitle + "<br />";
            string email_Schedule = "- Ngày bắt đầu công việc:  <strong> <span style=\"color:rgb(255,0,0)\">" + workDate + "</span></strong><br />";
            string email_Address = "- Địa điểm làm việc: <strong>" + address + "</strong><br />";
            
            string subject = EmailTemplate.EmailSubject;
            string body = email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />" + email_recTitle + email_Schedule + email_Address + notes + email_Seperate + EmailTemplate.Signature;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            var result= Send_mail(subject, body, receiver);
            if(result == true) {
                return true;
            }
            return false;
        }
        //Thuan Nguyen - POST: Auto send Email to Student when Not Hire
        // This function will send email to Student Email (@vanlanguni.vn)
        public bool SendEmail_NotHire_toStudent(string stuID, string companyName, int? recID, string recTitle, string interviewDate) {
            //Get Student Account
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "NOT_HIRE");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_recTitle = "- Vị trí ứng tuyển: " + checktitle + "<br />";
            string email_InterviewDate = "- Ngày phỏng vấn: " + interviewDate + "<br />";

            string subject= EmailTemplate.EmailSubject;
            string body = email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />-Thông tin chi tiết: <br />" + email_recTitle + email_InterviewDate + EmailTemplate.Content_Footer + email_Seperate + EmailTemplate.Signature;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            var result = Send_mail(subject, body, receiver);
            if (result == true) {
                return true;
            }
            return false;

        }
        //Thuan Nguyen - POST: Auto send Email to Student when Approve CV
        // This function will send email to Student Email (@vanlanguni.vn)
        public void SendEmail_ApproveCV_toStudent(string stuID, string companyName, int? recID, string recTitle, FormCollection formCollection) {
            //Get Interview Date from View Form
            string interviewDate = "";
            interviewDate = formCollection["interviewDate"].ToString();
            //Get Notes from View Form
            string notes = "";
            string address = "";

            //Check if Company has noted
            if (!string.IsNullOrEmpty(formCollection["approveNoted"])) {
                notes += "- Ghi chú: <br /> <i>" + formCollection["approveNoted"].ToString() + "</i>";
            }
            else {
                notes = "";
            }
            if (!string.IsNullOrEmpty(formCollection["interviewAddress"])) {
                address = formCollection["interviewAddress"].ToString();
            }
            else {
                address = "Vui lòng xem tại Trang cá nhân của Công ty " + companyName + "!";
            }

            //Get Student Account
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "APPROVE_STU");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_recTitle = "- Vị trí ứng tuyển: " + checktitle + "<br />";
            string email_Schedule = "- Lịch phỏng vấn:  <strong> <span style=\"color:rgb(255,0,0)\">" + interviewDate + "</span></strong><br />";
            string email_Address = "- Địa chỉ phỏng vấn: <strong>" + address + "</strong><br />";
            

            string subject= EmailTemplate.EmailSubject;
            string body= email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />" + email_recTitle + email_Schedule + email_Address + notes + email_Seperate + EmailTemplate.Signature;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            Send_mail(subject, body, receiver);
          
        }
        //Thuan Nguyen - POST: Auto send Email to Student when Reject CV
        //This function will send email to Student Email (@vanlanguni.vn)
        public void SendEmail_RejectCV_toStudent(string stuID, string companyName, int? recID, string recTitle, FormCollection formCollection) {
            //Get Data from View Form
            string reasons = "";
            if (!string.IsNullOrEmpty(formCollection["option1"])) {
                reasons += "<strong> + Chưa đạt yêu cầu cho công việc! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option2"])) {
                reasons += "<strong> + Đã tuyển đủ nhân sự! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option_other"])) {
                reasons += "<strong> + " + formCollection["other_reason"].ToString() + "</strong><br />";
            }
            //Get data from DB
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "REJECT_STU");
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_recTitle = "<br /><br />- Vị trí ứng tuyển: " + checktitle + "<br />";
            
            string subject= EmailTemplate.EmailSubject;
            string body = email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + email_recTitle + "- Lý do:<br />" + reasons + EmailTemplate.Content_Footer + email_Seperate + EmailTemplate.Signature;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            Send_mail(subject, body, receiver);
         
        }
        //Thuan Nguyen - POST: Auto send Email to Student when Approve CV
        // This function will send email to Student Email (@vanlanguni.vn)
        public void SendEmail_ChangeDateInterview_toStudent(string stuID, string companyName, int? recID, string recTitle, FormCollection formCollection) {
            //Get Interview Date from View Form
            string interviewDate = "";
            interviewDate = formCollection["interviewDate"].ToString();
            //Get Notes from View Form
            string notes = "";
            //Check if Company has noted
            if (!string.IsNullOrEmpty(formCollection["changeDateNoted"])) {
                notes += "- Ghi chú: <br /> <i>" + formCollection["changeDateNoted"].ToString() + "</i>";
            }
            else {
                notes = "";
            }

            //Get Student Account
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            //Get Email Approve_STU content
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "CHANGE_STU");
            //Get Sending Email info
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_recTitle = "- Vị trí ứng tuyển: " + checktitle + "<br />";
            string email_Schedule = "- <strong>Lịch phỏng vấn mới:</strong>  <strong> <span style=\"color:rgb(255,0,0)\">" + interviewDate + "</span></strong><br />";
            
            string subject= EmailTemplate.EmailSubject;
            string body= email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + "<br />" + email_recTitle + email_Schedule + notes + email_Seperate + EmailTemplate.Signature;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            Send_mail(subject, body, receiver);
            //--------------//
         
        }
        //Thuan Nguyen - POST: Auto send Email to Student when Cancel Interview Date
        //This function will send email to Student Email (@vanlanguni.vn)
        public void SendEmail_CancelInterview_toStudent(string stuID, string companyName, int? recID, string recTitle, FormCollection formCollection) {
            //Get Data from View Form
            string reasons = "";

            if (!string.IsNullOrEmpty(formCollection["option1"])) {
                reasons += "<strong> + Công việc đã tuyển đủ người! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option2"])) {
                reasons += "<strong> + Thay đổi yêu cầu tuyển dụng! </strong><br />";
            }
            if (!string.IsNullOrEmpty(formCollection["option_other"])) {
                reasons += "<strong> + " + formCollection["other_reason"].ToString() + "</strong><br />";
            }

            //Get data from DB
            var StuInfo = db.Student_Info.FirstOrDefault(x => x.Account_Id == stuID);
            var EmailTemplate = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "CANCEL_STU");
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            //Get Rec Detail Url
            var checktitle = "<a href='"
                + HttpContext.Current.Request.Url.Scheme + "://"
                + HttpContext.Current.Request.Url.Authority
                + url.Action("Detail", "Recruitment", new { area = "", id = recID })
                + "'>" + recTitle + "</a>";

            //Set Hello Meesage
            string email_Company = "<strong> Công ty " + companyName + "! </strong>";
            string email_Seperate = "<br />----------------------------------------------------------------<br />";
            string email_Hello = "Xin chào " + StuInfo.Student_Name.ToString() + "! <br />";
            string email_recTitle = "<br /><br />- Vị trí ứng tuyển: " + checktitle + "<br />";
            string body= email_Seperate + email_Company + email_Seperate + email_Hello + EmailTemplate.Content_Header + email_recTitle + "- Lý do:<br />" + reasons + EmailTemplate.Content_Footer + email_Seperate + EmailTemplate.Signature;
            string subject = EmailTemplate.EmailSubject;
            string receiver = StuInfo.AspNetUser.Email.ToString();
            //send mail
            Send_mail(subject, body, receiver);
            //--------------//
           
        }
        public bool SendMail_Nofication(string company) {
            
            var link = "<a href='"
               + HttpContext.Current.Request.Url.Scheme + "://"
               + HttpContext.Current.Request.Url.Authority
               + url.Action("Pending", "Account", new { area = "Staff" })
               + "'>" + "tại đây" + "</a>";
            string subject = "XÉT DUYỆT CÔNG TY MỚI";
            string body = "Công ty <strong>" + company + " ! </strong> đã đăng ký <br/ > Xét duyệt :"+link;
            var receiver = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            var result = Send_mail(subject,body,receiver.Signature);
            if(result == true) {
                return true;
            }
            return false;
        }
        public void SendMail_Reset(string receiver, string url) {
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            var subject= "[EJOB - VLU] Reset Password";
            var body= "Ấn vào đây để đặt lại mật khẩu mới:</br> <a href='" + url + "'>Đặt lại mật khẩu</a>";
            Send_mail(subject, body, receiver);
        }
        
        public bool  Send_mail(string Subject, string Body ,string receiver) {
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            try {
                //Email Config
                MailMessage mail = new MailMessage();
                mail.To.Add(receiver);
                mail.From = new MailAddress(EmailAccount.Email);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
               

                //SMTP Config
                SmtpClient client = new SmtpClient();
                client.Host = "smtp-mail.outlook.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(EmailAccount.Email, EmailAccount.Password);
                client.EnableSsl = true;
                client.Send(mail);
                //Send mail status
                return true;

            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}