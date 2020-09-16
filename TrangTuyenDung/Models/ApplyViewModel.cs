using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Models {
    public class ApplyViewModel {
        public CV_Info CV_Info { get; set; }
        public List<Student_Skills> Skill { get; set; }
        public List<Student_Projects> Proj { get; set; }
        public List<Student_Certificates> Cer { get; set; }
        public List<Student_Work_Experiences> WE { get; set; }
    }
}