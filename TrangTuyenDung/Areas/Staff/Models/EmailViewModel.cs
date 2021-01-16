using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Areas.Staff.Models {

    public class RejectEmailViewModel {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string ToEmail { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Reasons { get; set; }

    }
}