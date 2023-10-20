using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class TonKho
    {
        public int Idtk { get; set; }
        public int? Idctpn { get; set; }
        public double? SoLuong { get; set; }
        public DateTime? NgayNhap { get; set; }

        public virtual ChiTietPhieuNhap IdctpnNavigation { get; set; }
    }
}
