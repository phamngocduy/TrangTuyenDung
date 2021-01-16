using System.Web;
using System.Web.Mvc;

namespace TrangTuyenDung {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
