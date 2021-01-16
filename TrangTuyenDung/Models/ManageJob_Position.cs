using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangTuyenDung.Models {
    public class ManageJob_Position :Job_Positions{
        public string Name { get; set; }
        public int[] Major_Id { get; set; }
        public System.DateTime Date_Create { get; set; }

    }
}