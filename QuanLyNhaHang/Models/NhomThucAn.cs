using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class NhomThucAn
    {
        public NhomThucAn()
        {
            ThucDon = new HashSet<ThucDon>();
        }

        public int Idnta { get; set; }
        public string MaNta { get; set; }
        public string TenNta { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<ThucDon> ThucDon { get; set; }
    }
}
