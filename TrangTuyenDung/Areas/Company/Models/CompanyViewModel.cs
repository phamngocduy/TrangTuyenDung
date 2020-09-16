using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Company.Models {
    public class CompanyRegisterViewModel {
        //Login Information
        //[Remote("EmailAlreadyExistAsync","Account",HttpMethod ="POST", ErrorMessage ="Email đã tồn tại trong hệ thống, vui lòng chọn Email khác!")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng, vui lòng nhập lại!")]
        [Required(ErrorMessage = "Email không được bỏ trống!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Mật khẩu")]
        [StringLength(maximumLength: 16, ErrorMessage = "Mật khẩu phải tối đa {1} ký tự và tối thiểu {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }


        [Display(Name = "Xác thực Mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác thực không trùng khớp!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ họ và tên!")]
        public string FullName { get; set; }

        //Company Information
        [Display(Name = "Tên Công ty")]
        [Required(ErrorMessage = "Vui lòng cung cấp tên đầy đủ của Công ty!")]
        public string Com_Name { get; set; }

        [Display(Name = "Email Liên hệ")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        public string Email_Contact { get; set; }

        [Display(Name = "Người đại diện")]
        public string Representative { get; set; }


        [Display(Name = "Điện thoại liên hệ")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại Công ty!")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại!")]
        public string Phone_Contact { get; set; }

        [Display(Name = "Mã số doanh nghiệp")]
        [Required(ErrorMessage = "Vui lòng nhập Mã số doanh nghiệp!")]
        public string Com_Code { get; set; }

        [Display(Name = "Giới thiệu Công ty")]
        [Required(ErrorMessage = "Vui lòng giới thiệu về công ty!")]
        public string Com_Intro { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Công ty!")]
        public string Com_Address { get; set; }

        [Display(Name = "Tỉnh/TP")]
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/TP")]
        public int Com_Province { get; set; }

        [Display(Name = "Quận")]
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int Com_District { get; set; }

        [Display(Name = "Website")]
        [Required(ErrorMessage = "Vui lòng nhập Website của Công ty!!")]
        public string Com_Website { get; set; }

        [Display(Name = "Logo")]
        [Required(ErrorMessage = "Vui lòng tải lên Logo của công ty!")]
        //[FileExtensions(ErrorMessage = "Logo chỉ chấp nhận các định dạng sau: .png, .jpg, .jpeg", Extensions = "png,jpg,jpeg")]
        public HttpPostedFileBase Com_Logo { get; set; }

        [Display(Name = "Lĩnh vực chính")]
        [Required(ErrorMessage = "Vui lòng chọn lĩnh vực chính!")]
        public int[] Com_Faculty { get; set; }

        [Display(Name = "Lĩnh vực khác")]
        public string Com_OtherFaculties { get; set; }
        [Display(Name = "Ngành tuyển dụng tại Văn Lang")]
        public int company_facultites { get; set; }
    }
    public class CompanyRefRegisterViewModel {
        //Login Information
        //[Remote("EmailAlreadyExistAsync","Account",HttpMethod ="POST", ErrorMessage ="Email đã tồn tại trong hệ thống, vui lòng chọn Email khác!")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng, vui lòng nhập lại!")]
        [Required(ErrorMessage = "Email không được bỏ trống!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Mật khẩu")]
        [StringLength(maximumLength: 16, ErrorMessage = "Mật khẩu phải tối đa {1} ký tự và tối thiểu {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }


        [Display(Name = "Xác thực Mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác thực không trùng khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ họ và tên!")]
        public string FullName { get; set; }
        public string _ref { get; set; }
    }
    public class EmployeeRegisterViewModel {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng, vui lòng nhập lại!")]
        [Required(ErrorMessage = "Email không được bỏ trống!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Mật khẩu")]
        [StringLength(maximumLength: 16, ErrorMessage = "Mật khẩu phải tối đa {1} ký tự và tối thiểu {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Xác thực Mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác thực không trùng khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ họ và tên!")]
        public string FullName { get; set; }
    }
    public class HiredViewModel {
        public List<Apply_Recruitments> listHire { get; set; }
        public int status { get; set; }
    }


}