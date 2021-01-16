using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Models {
    public class InternshipCreate: Internship {
        public int Id { get; set; }
        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "Hãy nhập họ tên")]
        public string Student_Name { get; set; }
        [Display(Name = "Bậc đào tạo")]
        [Required(ErrorMessage = "Hãy nhập bậc đào tạo")]
        public int Level_Id { get; set; }
        [Display(Name = "Ngành học")]
        [Required(ErrorMessage = "Hãy nhập ngành học")]
        public int Major_Id { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Hãy nhập số điện thoại")]
        public string CellPhone { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Hãy nhập Email")]
        public string Email { get; set; }
        [Display(Name = "Công ty")]
        [Required(ErrorMessage = "Hãy nhập công ty")]
        public string Company_Name { get; set; }
        [Display(Name = "Hình thức thực tập")]
        [Required(ErrorMessage = "Hãy nhập hình thức thực tập")]
        public Nullable<int> Type_Inter { get; set; }
        [Display(Name = "Học kì thực tập")]
        [Required(ErrorMessage = "Hãy nhập học kì thực tập")]
        public int Semester_Inter { get; set; }
        [Display(Name = "Thực tập từ")]
        [Required(ErrorMessage = "Hãy nhập Thời gian thực tập")]
        public System.DateTime Inter_from { get; set; }
        [Display(Name = "Thực tập đến")]
        [Required(ErrorMessage = "Hãy Thời gian thực tập")]
        public System.DateTime Inter_to { get; set; }
        [Display(Name = "Năm học")]
        [Required(ErrorMessage = "Hãy nhập năm học")]
        public string School_year { get; set; }
        
        public string internTypeOther { get; set; }
      
        public string Class { get; set; }
        [Display(Name = "Mã số sinh viên")]
        [Required(ErrorMessage = "Hãy nhập mã số sinh viên")]
        public string MSSV { get; set; }

    }
}