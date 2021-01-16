using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangTuyenDung.Models;

namespace TrangTuyenDung.Areas.Student.Models {
    public class Student_InfoViewModel {
        public int Id { get; set; }
        [DisplayName("Tên")]
        [Required(ErrorMessage = "Xin hãy nhập Tên")]
        public string Student_Name { get; set; }
        [DisplayName("Chuyên Ngành")]
        [Required(ErrorMessage = "Xin hãy chọn chuyên ngành")]
        public Nullable<int> Faculties_Id { get; set; }
        [DisplayName("Ngày Sinh")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "Xin hãy nhập Ngày Sinh")]
        public Nullable<System.DateTime> Student_Birthday { get; set; }
        [DisplayName("Giới Tính")]
        [UIHint("NamNữ")]
        public Nullable<bool> Student_Gender { get; set; }
        [DisplayName("Địa Chỉ Nhà")]
        [Required(ErrorMessage = "Xin hãy nhập Địa Chỉ")]
        public string Student_Address { get; set; }
        [DisplayName("Số Điện Thoại")]
        [Required(ErrorMessage = "Xin hãy nhập Số Điện Thoại")]
        public string Student_PhoneNumber { get; set; }
        [DisplayName("Mã Số Sinh Viên")]
        [Required(ErrorMessage = "Xin hãy nhập Mã Số Sinh Viên")]
        public string MSSV { get; set; }
        [DisplayName("Lớp")]
        [Required(ErrorMessage = "Xin hãy nhập Lớp")]
        public string Student_Class { get; set; }
        public string Account_Id { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual Status_Account Status_Account { get; set; }
        public HttpPostedFileBase Student_Avatar { get; set; }
    }

    public class Personal_infoViewModel {
        //first form
        ///////
        public string Account_Id { get; set; }
        public Nullable<int> Faculties_Id { get; set; }
        [DisplayName("Ngày Sinh")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "Xin hãy nhập Ngày Sinh")]
        public Nullable<System.DateTime> Student_Birthday { get; set; }
        [DisplayName("Giới Tính")]
        [UIHint("NamNữ")]
        public Nullable<bool> Student_Gender { get; set; }
        [DisplayName("Địa Chỉ Nhà")]
        public string Student_Address { get; set; }
        [DisplayName("Số Điện Thoại")]
        public string Student_PhoneNumber { get; set; }
        [DisplayName("Ảnh Đại Diện")]
        public string Student_Avatar { get; set; }
        [DisplayName("Tên")]
        [Required(ErrorMessage = "Xin hãy nhập Tên")]
        public string Student_Name { get; set; }
        public int ID { get; set; }
        public int Student_Id { get; set; }
        public string email { get; set; }
        public string MSSV { get; set; }
        public string Student_Class { get; set; }
        //------------------------------------------
        // --- SECOND FORM ------------
        [AllowHtml]
        [DisplayName("Mục Tiêu Cá Nhân")]
        public string Personal_Goal { get; set; }
        [AllowHtml]
        [DisplayName("Giới Thiệu Bản Thân")]
        public string About_Student { get; set; }

        // --- For Personal Skills-- PS --
        // --PS--01--
        public string PS_01 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_01 { get; set; }
        // --PS--02--
        public string PS_02 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_02 { get; set; }
        // --PS--03--
        public string PS_03 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_03 { get; set; }

        //For Work Experiences -- WE --
        // --WE--01--
        //public string Work_Experience { get; set; }
        public string WE_Company_01 { get; set; }
        public string WE_Date_Start_01 { get; set; }
        public string WE_Date_End_01 { get; set; }
        [AllowHtml]
        public string WE_Description_01 { get; set; }
        // --WE--02--
        [AllowHtml]
        public string WE_Company_02 { get; set; }
        public string WE_Date_Start_02 { get; set; }
        public string WE_Date_End_02 { get; set; }
        [AllowHtml]
        public string WE_Description_02 { get; set; }
        // --WE--03--
        public string WE_Company_03 { get; set; }
        public string WE_Date_Start_03 { get; set; }
        public string WE_Date_End_03 { get; set; }
        [AllowHtml]
        public string WE_Description_03 { get; set; }
        // -----END WE-----

        // --- CERTIFICATES ---
        //get info certificate 1
        public string Cer_Name_01 { get; set; }
        [AllowHtml]
        public string Cer_Description_01 { get; set; }
        public string Cer_GetDate_01 { get; set; }

        //get info certificate 2
        public string Cer_Name_02 { get; set; }
        [AllowHtml]
        public string Cer_Description_02 { get; set; }
        public string Cer_GetDate_02 { get; set; }

        //get info certificate 3
        public string Cer_Name_03 { get; set; }
        [AllowHtml]
        public string Cer_Description_03 { get; set; }
        public string Cer_GetDate_03 { get; set; }
        // --- END CER ---

        // for Project Info --
        public string Project_Name_1 { get; set; }
        [AllowHtml]
        public string Project_Description_1 { get; set; }
        public string Project_Date_Start_1 { get; set; }
        public string Project_Date_End_1 { get; set; }
        //get info project 2
        public string Project_Name_2 { get; set; }
        [AllowHtml]
        public string Project_Description_2 { get; set; }
        public string Project_Date_Start_2 { get; set; }
        public string Project_Date_End_2 { get; set; }


        public Nullable<int> CV_Template_Id { get; set; }

        public virtual Faculty Faculty { get; set; }
        public virtual Student_Info Student_Info { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }

        public virtual ICollection<CV_Info> CV_Info { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Certificates> Student_Certificates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Projects> Student_Projects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Skills> Student_Skills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Work_Experiences> Student_Work_Experiences { get; set; }
    }

    public class CV_UpdateViewModel {
        //account student id
        public int ID { get; set; }

        // Student goal
        [AllowHtml]
        [DisplayName("Mục Tiêu Cá Nhân")]
        public string Personal_Goal { get; set; }
        //introduce student
        [AllowHtml]
        [DisplayName("Giới Thiệu Bản Thân")]
        public string About_Student { get; set; }

        // --- For Personal Skills-- PS --
        // --PS--01--
        public string PS_01 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_01 { get; set; }
        // --PS--02--
        public string PS_02 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_02 { get; set; }
        // --PS--03--
        public string PS_03 { get; set; }
        [Range(0, 100, ErrorMessage = "Số phải nằm trong khoảng 0% - 100%")]
        public int? PS_Rate_03 { get; set; }

        //For Work Experiences -- WE --
        // --WE--01--
        //public string Work_Experience { get; set; }
        public string WE_Job_Position_01 { get; set; }
        public string WE_Company_01 { get; set; }
        public string WE_Date_Start_01 { get; set; }
        public string WE_Date_End_01 { get; set; }
        [AllowHtml]
        public string WE_Description_01 { get; set; }
        // --WE--02--
        public string WE_Job_Position_02 { get; set; }
        public string WE_Company_02 { get; set; }
        public string WE_Date_Start_02 { get; set; }
        public string WE_Date_End_02 { get; set; }
        [AllowHtml]
        public string WE_Description_02 { get; set; }
        // --WE--03--
        public string WE_Job_Position_03 { get; set; }
        public string WE_Company_03 { get; set; }
        public string WE_Date_Start_03 { get; set; }
        public string WE_Date_End_03 { get; set; }
        [AllowHtml]
        public string WE_Description_03 { get; set; }
        // -----END WE-----

        // --- CERTIFICATES ---
        //get info certificate 1
        public string Cer_Name_01 { get; set; }
        [AllowHtml]
        public string Cer_Description_01 { get; set; }
        public string Cer_GetDate_01 { get; set; }

        //get info certificate 2
        public string Cer_Name_02 { get; set; }
        [AllowHtml]
        public string Cer_Description_02 { get; set; }
        public string Cer_GetDate_02 { get; set; }

        //get info certificate 3
        public string Cer_Name_03 { get; set; }
        [AllowHtml]
        public string Cer_Description_03 { get; set; }
        public string Cer_GetDate_03 { get; set; }
        // --- END CER ---

        // for Project Info --
        public string Project_Name_1 { get; set; }
        [AllowHtml]
        public string Project_Description_1 { get; set; }
        public string Project_Date_Start_1 { get; set; }
        public string Project_Date_End_1 { get; set; }
        //get info project 2
        public string Project_Name_2 { get; set; }
        [AllowHtml]
        public string Project_Description_2 { get; set; }
        public string Project_Date_Start_2 { get; set; }
        public string Project_Date_End_2 { get; set; }


        public Nullable<int> CV_Template_Id { get; set; }

    }
    public class StudentDashboardViewModel {
        //Applying
        public int numApplying { get; set; }
        public int numPendingApply { get; set; }
        public int numInterviewingApply { get; set; }
        public int numRejectedApply { get; set; }
        //Interview Result
        public int numSuccessInterview { get; set; }
        public int numFailInterview { get; set; }
        //Status of personal CV
        public int CVStatus { get; set; }
        //Recruitment
        public int numBookmarked { get; set; }
    }
    public class InterviewDetailViewModel {
        public string rec { get; set; }
        public string com { get; set; }
        public string date_apply { get; set; }
        public string date_inter { get; set; }
        public string date_complete { get; set; }
        public string date_work { get; set; }
        public string address_work { get; set; }
    }
}