using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace QuanLyNhaHang.Models
{
    public partial class ChiTietPhieuNhap
    {
        public ChiTietPhieuNhap()
        {
            ChiTietPhieuXuat = new HashSet<ChiTietPhieuXuat>();
            TonKho = new HashSet<TonKho>();
        }

        public int Idctpn { get; set; }
        public int? Idpn { get; set; }
        public int? Idhh { get; set; }
        public double? SoLuong { get; set; }
        public double? Gia { get; set; }
        public DateTime? Nsx { get; set; }
        public DateTime? Hsd { get; set; }
        public bool? Active { get; set; }

        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual PhieuNhap IdpnNavigation { get; set; }
        public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
        public virtual ICollection<TonKho> TonKho { get; set; }
    }
}
