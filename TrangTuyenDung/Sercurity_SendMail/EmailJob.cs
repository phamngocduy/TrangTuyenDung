using Quartz;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Sercurity_SendMail {

    public class EmailJob : IJob {
        EJobEntities db = new EJobEntities(); 

        public void Execute(IJobExecutionContext context) {
            var EmailAccount = db.Email_Configs.FirstOrDefault(x => x.Email_Type == "ACCOUNT");
            var num = db.Internships.Where(x => x.status == 0).Count();
            if (num > 0) {
                using (var message = new MailMessage(EmailAccount.Email, EmailAccount.Signature)) {

                    message.Subject = "[EJOB - VLU] Xét duyệt Giấy Giới Thiệu";
                    message.Body = "Hiện đang có " + num + " GTT chưa xét duyệt";
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp-mail.outlook.com";
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(EmailAccount.Email, EmailAccount.Password);
                    client.EnableSsl = true;
                    try {
                        client.Send(message);
                    }
                    catch (Exception ex) {
                        throw ex;
                    }
                }
            }

        }

      
    }

}