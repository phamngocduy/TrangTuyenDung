using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrangTuyenDung.Models {

    public class RecruitmentCreate : Recruitment {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Hãy nhập tiêu đề")]
        [StringLength(60, ErrorMessage = "Độ dài tiêu đề không quá 60 ký tự")]
        public string title { get; set; }
        [Display(Name = "Mức Lương Thấp Nhất")]
        public Nullable<int> Salary_from { get; set; }
        [Display(Name = "Mức Lương Cao Nhất")]
        public Nullable<int> Salary_to { get; set; }
        [Display(Name = "Ngày Bắt Đầu Tuyển Dụng")]
        [Required(ErrorMessage = "Hãy nhập ngày bắt đầu tuyển dụng")]

        public System.DateTime Recruiting_dates { get; set; }
        [Display(Name = "Ngày Hết Hạn Tuyển Dụng")]
        [Required(ErrorMessage = "Hãy nhập ngày hết hạn tuyển dụng")]

        public System.DateTime Expire_date { get; set; }

        [Display(Name = "nơi làm việc")]
        [Required(ErrorMessage = "Hãy chọn nơi làm việc")]
        public int Districts_id { get; set; }

        [Display(Name = "Toàn thời gian")]
        public bool Is_Full_Time { get; set; }
        [Display(Name = "Bán thời gian")]
        public bool Is_Part_Time { get; set; }
        [Display(Name = "Thực tập sinh")]
        public bool Is_Intership { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Hãy nhập mô tả công việc")]
        [Display(Name = "Mô tả công việc")]
        public string Mo_ta_Chi_Tiet { get; set; }
        [AllowHtml]
        [Display(Name = "Kỹ Năng Yêu Cầu")]
        public string Ky_Nang_Cong_Viec { get; set; }
        [AllowHtml]
        [Display(Name = "Công Việc Yêu Cầu")]
        public string Yeu_Cau_Cong_Viec { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Hãy nhập Phúc Lợi")]
        [Display(Name = "Phúc Lợi")]
        public string Phuc_Loi { get; set; }
        [AllowHtml]
        [Display(Name = "Thông tin liên hệ")]
        public string Tuy_Chon { get; set; }
        public int Province_ID { get; set; }
        [Required(ErrorMessage = "Hãy chọn khoa/ngành")]
        [Display(Name = "Khoa/Ngành")]
        public int Faculties_ID { get; set; }
        //[MaxLength(3,ErrorMessage ="Không Chọn Quá 3 Ngành")]
        public int[] RecMajor { get; set; }

        public string[] RecMajorString { get; set; }
        
        public Nullable<int> Job_Position_Id { get; set; }

        public string other_job { get; set; }
    }

    public class RecruitmentShow : Recruitment {
        public string Yeu_Cau { get; set; }
        public int[] RecMajor { get; set; }

        public RecruitmentCreate ConvertToRecruitmentSupport(RecruitmentShow data) {
            RecruitmentCreate value = new RecruitmentCreate {
                title = data.title,
                Company_id = data.Company_id,
                Districts_id = data.Districts_id,
                Expire_date = data.Expire_date,
                Recruiting_dates = data.Recruiting_dates,
                Is_Full_Time = data.Is_Full_Time,
                Is_Intership = data.Is_Intership,
                Is_Part_Time = data.Is_Part_Time,
                Phuc_Loi = data.Job_Benefits,
                Job_Positions = data.Job_Positions,
                Job_Position_Id = data.Job_Position_Id,
                Nums_view = data.Nums_view,
                Num_FullTime = data.Num_FullTime,
                Num_Intern = data.Num_Intern,
                Num_PartTime = data.Num_PartTime,
                Salary_from = data.Salary_from,
                Salary_to = data.Salary_to,
                //Faculty = model.Faculty,
                //Faculty_Id = model.Faculty_Id,
                Mo_ta_Chi_Tiet = data.Job_Description,
                Tuy_Chon = data.Job_Optional,
                Soft_Skills = data.Soft_Skills,
                Required_Skills = data.Required_Skills,
                other_job=data.other_job
            };
            value.RecMajor = data.RecMajor;
            return value;
        }
    }

    public class RecruitmentSupport {
        //check out
        public void CheckOutDate(List<Recruitment> data) {
            var db = new EJobEntities();
            foreach (var item in data) {
                if (item.Status_id == 2 && item.Expire_date < DateTime.Now) {
                    var changedata = db.Recruitments.FirstOrDefault(x => x.Id == item.Id);
                    changedata.Status_id = 3;
                }
            }
            db.SaveChanges();
        }
    }

}