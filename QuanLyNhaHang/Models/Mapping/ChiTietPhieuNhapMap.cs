using System.Collections.Generic;

namespace QuanLyNhaHang.Models.Mapping
{
    public class ChiTietPhieuNhapMap
    {
        public int Idctpn { get; set; }
        public string? Idpn { get; set; }
        public string? Idhh { get; set; }
        public string? SoLuong { get; set; }
        public string? Gia { get; set; }
        public string? Nsx { get; set; }
        public string? Hsd { get; set; }
        public bool? Active { get; set; }

        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual PhieuNhap IdpnNavigation { get; set; }
        public virtual ICollection<TonKho> TonKho { get; set; }
    }
}
