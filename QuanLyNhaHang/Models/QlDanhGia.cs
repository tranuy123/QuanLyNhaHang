using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class QlDanhGia
    {
        public int IddanhGia { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int? Idnnv { get; set; }
        public int? ThoiGianTu { get; set; }
        public int? ThoiGianDen { get; set; }
        public int? Diem { get; set; }
        public bool? Active { get; set; }

        public virtual NhomNhanVien IdnnvNavigation { get; set; }
    }
}
