//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrangTuyenDung.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Company_Registration
    {
        public int Id { get; set; }
        public string Name_Company { get; set; }
        public string Contact_Email { get; set; }
        public string Contact_Phone { get; set; }
        public string Introduce_Company { get; set; }
        public string website_Company { get; set; }
        public string Address_Company { get; set; }
        public int Province_ID { get; set; }
        public int District_ID { get; set; }
        public System.DateTime Created_at { get; set; }
        public Nullable<System.DateTime> Update_at { get; set; }
        public string Slug_Company { get; set; }
        public Nullable<int> Is_hot_Company { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string ImageBase64 { get; set; }
        public string Faculties { get; set; }
        public string Faculties_Other { get; set; }
        public Nullable<int> company_facultites { get; set; }
    
        public virtual District District { get; set; }
        public virtual Province Province { get; set; }
    }
}
