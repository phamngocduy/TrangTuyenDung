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
    
    public partial class Bookmark
    {
        public int Id { get; set; }
        public int Student_Id { get; set; }
        public int Rec_Id { get; set; }
        public Nullable<System.DateTime> Date_Create { get; set; }
    
        public virtual Recruitment Recruitment { get; set; }
        public virtual Student_Info Student_Info { get; set; }
    }
}
