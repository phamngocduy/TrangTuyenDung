using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TrangTuyenDung.Models;
namespace TrangTuyenDung.Areas.Staff.Models {
    public class CreateCompanyViewModel {

        //Company Information
        [Display(Name = "Tên Công ty")]
        [Required(ErrorMessage = "Vui lòng cung cấp tên đầy đủ của Công ty!")]
        public string Com_Name { get; set; }

       
        [Display(Name = "Tên Người đại diện")]
        [Required(ErrorMessage = "Vui lòng cung cấp tên người đại diện!")]
        public string Representative { get; set; }

        
        [Display(Name = "Email liên hệ")]
        [Required(ErrorMessage = "Vui lòng cấp Email liên hệ!")]
        public string Email_contact { get; set; }

        [Display(Name = "Logo")]
        [Required(ErrorMessage = "Vui lòng tải lên Logo của công ty!")]
        //[FileExtensions(ErrorMessage ="Logo chỉ chấp nhận các định dạng sau: .png, .jpg, .jpeg" ,Extensions ="png,jpg,jpeg")]

        public HttpPostedFileBase Com_Logo { get; set; }

        [Display(Name = "Số điện thoại liên hệ")]
        [Required(ErrorMessage = "Vui lòng cấp số điên thoại liên hệ!")]
        public string phone { get; set; }
    }

    public class EditRecruitmentViewModel : RecruitmentCreate {
        public int Id { get; set; }
    }
    public class StaffStatisticViewModel {
        public string Time { get; set; }
        public DateTime Now { get; set; }
        public string staffEmail { get; set; }
        public int numRecs { get; set; }
        public int numApprovedRecs { get; set; }
        public int numFullTime { get; set; }
        public int numPartTime { get; set; }
        public int numIntern { get; set; }
        public int numApplied { get; set; }
        public int numApplying { get; set; }
        public int numApprovedCan { get; set; }
        public string staJP { get; set; }

    }

    public class StaffDashboardViewModel {
        //Recruitments
        public int numRecs { get; set; }
        public int numPendingRecs { get; set; }
        public int numActivedRecs { get; set; }
        public int numExpiredRecs { get; set; }
        //Recruiters
        public int numCompany { get; set; }
        public int numPendingComs { get; set; }
        //Candidate
        public int numCandidate { get; set; }
        public int numApplyingCans { get; set; }
        public int numCVs { get; set; }
        public int numBookmarks { get; set; }
        public int numSuccessCans { get; set; }
        //Other
        public int numFaculties { get; set; }
        public int numMajors { get; set; }
        public int numJobPositions { get; set; }
    }
}