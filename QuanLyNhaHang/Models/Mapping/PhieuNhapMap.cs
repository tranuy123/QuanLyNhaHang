using System.Collections.Generic;
using System;

namespace QuanLyNhaHang.Models.Mapping
{
    public class PhieuNhapMap
    {
        public int Idpn { get; set; }
        public string? Idncc { get; set; }
        public string? Idnv { get; set; }
        public string SoPn { get; set; }
        public string SoHd { get; set; }
        public string NgayNhap { get; set; }
        public string? NgayHd { get; set; }
        public string GhiChu { get; set; }
        public bool? Active { get; set; }

        public virtual NhaCungCap IdnccNavigation { get; set; }
        public virtual NhanVien IdnvNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
    }
}
