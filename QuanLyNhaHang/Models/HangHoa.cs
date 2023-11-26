using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class HangHoa
    {
        public HangHoa()
        {
            ChiTietPhieuNhap = new HashSet<ChiTietPhieuNhap>();
            ChiTietPhieuXuat = new HashSet<ChiTietPhieuXuat>();
            DinhMuc = new HashSet<DinhMuc>();
        }

        public int Idhh { get; set; }
        public int? Idnhh { get; set; }
        public int? Iddvt { get; set; }
        public string MaHh { get; set; }
        public double? GiaBan { get; set; }
        public bool HangDemDuoc { get; set; }
        public string TenHh { get; set; }
        public string Avatar { get; set; }
        public bool? Active { get; set; }

        public virtual DonViTinh IddvtNavigation { get; set; }
        public virtual NhomHangHoa IdnhhNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
        public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
        public virtual ICollection<DinhMuc> DinhMuc { get; set; }
    }
}
