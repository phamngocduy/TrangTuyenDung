using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Student.Controllers {
    [Authorize(Roles = "Student")]
    public class RecruitmentController : Controller {
        EJobEntities db = new EJobEntities();
        //Thuan Nguyen - POST: Student bookmark recs with ajax
        public string Bookmark(int id) {
            //id: Recruitment - id
            //Get current user
            var crrUser = User.Identity.GetUserId();
            var crrStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == crrUser);
            //Check bookmark
            var chkBookmark = db.Bookmarks.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser && x.Rec_Id == id);
            if (chkBookmark != null) {
                //unBookmark
                db.Bookmarks.Remove(chkBookmark);
                //Save to DB
                int result = db.SaveChanges();
                if (result != 0) {
                    return "UBMsuccess";
                }
                else {
                    return "error";
                }
            }
            else {
                //Create bookmark
                db.Bookmarks.Add(new Bookmark {
                    Student_Id = crrStu.Id,
                    Rec_Id = id,
                    Date_Create = DateTime.Now
                });
                //Save to DB
                int result = db.SaveChanges();
                if (result != 0) {
                    return "BMsuccess";
                }
                else {
                    return "error";
                }
            }

        }

        //Thuan Nguyen - GET: Student view list of bookmarked rec
        [HttpGet]
        public ActionResult Bookmarked(string message = null) {
            //Hide suggest jobs form
            TempData["ShowSuggest"] = false;
            //Get current User
            var crrUser = User.Identity.GetUserId();
            //check message
            if (message == "DeleteSuccess") {
                ViewBag.Message = "DeleteSuccess";
            }

            var model = db.Bookmarks.Where(x => x.Student_Info.Account_Id == crrUser);
            return View(model);
        }

        //Thuan Nguyen - POST: Student delete bookmark of their
        public ActionResult DeleleBookmarked(int id) {
            //id: Recruitment - id
            //Get current user
            var crrUser = User.Identity.GetUserId();
            var crrStu = db.Student_Info.FirstOrDefault(x => x.Account_Id == crrUser);
            //Check bookmark
            var chkBookmark = db.Bookmarks.FirstOrDefault(x => x.Student_Info.Account_Id == crrUser && x.Rec_Id == id);
            if (chkBookmark != null) {
                //unBookmark
                db.Bookmarks.Remove(chkBookmark);
                //Save to DB
                int result = db.SaveChanges();
                if (result != 0) {
                    return RedirectToAction("Bookmarked", "Recruitment", new { area = "Student", @message = "DeleteSuccess" }); ;
                }
                else {
                    return Content("Có lỗi xảy ra, vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
                }
            }
            else {
                return Content("Có lỗi xảy ra, vui lòng làm mới lại danh sách và thử lại! Xin cảm ơn!");
            }
        }
    

    }
}