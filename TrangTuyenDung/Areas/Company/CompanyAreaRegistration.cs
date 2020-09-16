using System.Web.Mvc;

namespace TrangTuyenDung.Areas.Company {
    public class CompanyAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Company";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Company_default",
                "Company/{controller}/{action}/{id}",
                new { action = "Dashboard", id = UrlParameter.Optional }, new string[] { "TrangTuyenDung.Areas.Company.Controllers" }
            );
        }
    }
}