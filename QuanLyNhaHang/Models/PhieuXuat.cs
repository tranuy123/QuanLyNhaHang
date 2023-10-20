using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class PhieuXuat
    {
        public PhieuXuat()
        {
            ChiTietPhieuXuat = new HashSet<ChiTietPhieuXuat>();
        }

        public int Idpx { get; set; }
        public int? Idkh { get; set; }
        public int? Idnv { get; set; }
        public string SoPx { get; set; }
        public string SoHd { get; set; }
        public DateTime? NgayHd { get; set; }
        public string GhiChu { get; set; }
        public bool? Active { get; set; }

        public virtual NhanVien IdnvNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
    }
}
