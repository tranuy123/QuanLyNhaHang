using System.Collections.Generic;
using System;

namespace QuanLyNhaHang.Models.Mapping
{
    public class PhieuXuatMap
    {
        public int Idpx { get; set; }
        public string Idkh { get; set; }
        public int Idnv { get; set; }
        public string SoPx { get; set; }
        public string SoHd { get; set; }
        public string NgayHd { get; set; }
        public string NgayTao { get; set; }

        public string GhiChu { get; set; }
        public bool? Active { get; set; }

        public virtual NhanVien IdnvNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
    }
}
