using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class PhieuNhap
    {
        public PhieuNhap()
        {
            ChiTietPhieuNhap = new HashSet<ChiTietPhieuNhap>();
        }

        public int Idpn { get; set; }
        public int? Idncc { get; set; }
        public int? Idnv { get; set; }
        public string SoPn { get; set; }
        public string SoHd { get; set; }
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayHd { get; set; }
        public string GhiChu { get; set; }
        public bool? Active { get; set; }

        public virtual NhaCungCap IdnccNavigation { get; set; }
        public virtual NhanVien IdnvNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
    }
}
