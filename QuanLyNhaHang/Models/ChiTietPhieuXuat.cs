using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class ChiTietPhieuXuat
    {
        public int Idctpx { get; set; }
        public int? Idpx { get; set; }
        public int? Idctpn { get; set; }
        public int? Idhh { get; set; }
        public double? SoLuong { get; set; }
        public double? Gia { get; set; }
        public bool? Active { get; set; }

        public virtual ChiTietPhieuNhap IdctpnNavigation { get; set; }
        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual PhieuXuat IdpxNavigation { get; set; }
    }
}
