using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Models {
    public class IntershipCreate : Intership {
        [Display(Name = "Tên học sinh")]
        [Required(ErrorMessage = "Hãy tên học sinh")]
        public string Student_Name { get; set; }
        [Display(Name = "Nhập Bậc đào tạo")]
        public int Level_Id { get; set; }
        [Display(Name = "Nhập ngành đào tạo")]
        [Required(ErrorMessage = "Hãy nhập ngành đào tạo")]
        public int Major_ID { get; set; }
        [Display(Name = "Số Điện thoại")]
        [Required(ErrorMessage = "Hãy nhập số điện thoại")]
        public string CellPhone { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Hãy nhập Email")]
        public string Email { get; set; }
        [Display(Name = "Công ty")]
        [Required(ErrorMessage = "Hãy nhập tên công ty")]
        public string Company_Name { get; set; }
        [Display(Name = "Hình thức thực tập")]
        [Required(ErrorMessage = "Hãy nhập hình thức thực tập")]
        public string Type_Inter { get; set; }
        [Display(Name = "Học kì thực tập")]
        [Required(ErrorMessage = "Hãy nhập Học kì thực tập")]
        public int Semester_Inter { get; set; }
        [Display(Name = "Thực tập từ")]
        
        public System.DateTime Inter_from { get; set; }
        [Display(Name = "Thực tập đến")]
        public System.DateTime Inter_to { get; set; }
        [Display(Name = "Năm học thực tập")]
        public string School_year { get; set; }
    }
}