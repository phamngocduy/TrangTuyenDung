using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Models {
    public class StaffAccountSuppor {
        [Required(ErrorMessage = "Xin hãy nhập Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Xin hãy nhập Password")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Mật khẩu phải từ 5 dến 255 ký tự", MinimumLength = 5)]
        public string Password { get; set; }

    }

}