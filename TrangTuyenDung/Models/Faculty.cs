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
    
    public partial class Faculty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Faculty()
        {
            this.Faculties_In_Companies = new HashSet<Faculties_In_Companies>();
            this.Faculties_Majors = new HashSet<Faculties_Majors>();
            this.Student_Info = new HashSet<Student_Info>();
            this.Tag_In_Faculties = new HashSet<Tag_In_Faculties>();
            this.confirmation_intern = new HashSet<confirmation_intern>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime Date_Created { get; set; }
        public Nullable<System.DateTime> Date_Updated { get; set; }
        public Nullable<int> hidden { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Faculties_In_Companies> Faculties_In_Companies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Faculties_Majors> Faculties_Majors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Info> Student_Info { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag_In_Faculties> Tag_In_Faculties { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<confirmation_intern> confirmation_intern { get; set; }
    }
}
