using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            ChiTietHoaDon = new HashSet<ChiTietHoaDon>();
        }

        public int Idhd { get; set; }
        public string MaHd { get; set; }
        public DateTime? Tgxuat { get; set; }
        public double? TongTien { get; set; }
        public bool? TinhTrang { get; set; }
        public bool? TinhTrangTt { get; set; }
        public bool? Active { get; set; }
        public int? Idban { get; set; }
        public int? Idnv { get; set; }

        public virtual Ban IdbanNavigation { get; set; }
        public virtual NhanVien IdnvNavigation { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDon { get; set; }
    }
}
