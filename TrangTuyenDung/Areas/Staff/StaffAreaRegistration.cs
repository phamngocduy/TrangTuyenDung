using System.Web.Mvc;

namespace TrangTuyenDung.Areas.Staff {
    public class StaffAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Staff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Staff_default",
                "Staff/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }, new string[] { "TrangTuyenDung.Areas.Staff.Controllers" }
            );
        }
    }
}