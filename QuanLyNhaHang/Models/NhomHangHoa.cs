using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class NhomHangHoa
    {
        public NhomHangHoa()
        {
            HangHoaNavigation = new HashSet<HangHoa>();
        }

        public int Idnhh { get; set; }
        public string MaNhh { get; set; }
        public string TenNhh { get; set; }
        public bool? NguyenLieu { get; set; }
        public bool? HangHoa { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<HangHoa> HangHoaNavigation { get; set; }
    }
}
