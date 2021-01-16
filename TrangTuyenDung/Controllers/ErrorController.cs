using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrangTuyenDung.Controllers {
    public class ErrorController : Controller {
        // GET: Error
        public ActionResult Index() {
            return View();
        }
        //Thuan Nguyen - GET: 404 Page Not Found
        public ActionResult PageNotFound() {
            return View();
        }
        //Thuan Nguyen - GET: 404.13 file large size (file > maxContentLength)
        public ActionResult FileSizeExceed() {
            return View();
        }
        //Thuan Nguyen - GET: 500 Server Error
        public ActionResult ServerError() {
            return View();
        }


    }
}