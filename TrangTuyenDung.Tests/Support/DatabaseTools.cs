using TrangTuyenDung.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace TrangTuyenDung.Tests.Support {
    [TestClass]
    public static class DatabaseTools {
        [AssemblyCleanup]
        public static void CleanDatabase() {
            using (var db = new EJobEntities()) {

                // delete db from Register company
                //db.User_In_Company.Remove(db.User_In_Company.FirstOrDefault(x => x.FullName == "Võ Thị Kim Chi"));
                //db.Faculties_In_Companies.Remove(db.Faculties_In_Companies.FirstOrDefault(x => x.Faculty.Name == "Công Nghệ Thông Tin"));
                //db.AspNetUsers.Remove(db.AspNetUsers.FirstOrDefault(x => x.Email == "sophievo1297@gmail.com"));
                //db.Company_Info.Remove(db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty software"));

                db.Company_Info.Remove(db.Company_Info.FirstOrDefault(x => x.Name_Company == "Cty testtttt"));

                // delete from Apply CV
                db.Apply_Recruitments.Remove(db.Apply_Recruitments.FirstOrDefault(x => x.CV_Info.Student_Info.Student_Name == "chivo10"));

                // delete db from Create CV
                db.Student_Certificates.Remove(db.Student_Certificates.FirstOrDefault(x => x.Student_Info.Student_Name == "chivo10"));
                db.Student_Projects.Remove(db.Student_Projects.FirstOrDefault(x => x.Student_Info.Student_Name == "chivo10"));
                db.Student_Skills.Remove(db.Student_Skills.FirstOrDefault(x => x.Student_Info.Student_Name == "chivo10"));
                db.Student_Work_Experiences.Remove(db.Student_Work_Experiences.FirstOrDefault(x => x.Student_Info.Student_Name == "chivo10"));

                db.Student_Certificates.Remove(db.Student_Certificates.FirstOrDefault(x => x.Student_Info.Student_Name == "vinhnguyen57"));
                db.Student_Projects.Remove(db.Student_Projects.FirstOrDefault(x => x.Student_Info.Student_Name == "vinhnguyen57"));
                db.Student_Skills.Remove(db.Student_Skills.FirstOrDefault(x => x.Student_Info.Student_Name == "vinhnguyen57"));
                db.Student_Work_Experiences.Remove(db.Student_Work_Experiences.FirstOrDefault(x => x.Student_Info.Student_Name == "vinhnguyen57"));
                db.CV_Info.Remove(db.CV_Info.FirstOrDefault(x => x.Student_Info.Student_Name == "vinhnguyen57"));

                // delete db from Create recruitment
                db.Tags_Recruitments.Remove(db.Tags_Recruitments.FirstOrDefault(x => x.Recruitment.title == "Công Việc Thử Nghiệm"));
                db.Recruitments.Remove(db.Recruitments.FirstOrDefault(x => x.title == "Công Việc Thử Nghiệm"));

                db.Tags_Recruitments.Remove(db.Tags_Recruitments.FirstOrDefault(x => x.Recruitment.title == "Test Unit Data"));
                db.Recruitments.Remove(db.Recruitments.FirstOrDefault(x => x.title == "Test Unit Data"));

                db.SaveChanges();
            }
        }
    }
}
