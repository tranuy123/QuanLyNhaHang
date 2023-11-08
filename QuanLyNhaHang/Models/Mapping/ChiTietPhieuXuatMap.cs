namespace QuanLyNhaHang.Models.Mapping
{
    public class ChiTietPhieuXuatMap
    {
        public int Idctpx { get; set; }
        public string Idpx { get; set; }
        public string Idctpn { get; set; }
        public string Idhh { get; set; }
        public string SoLuong { get; set; }
        public string Gia { get; set; }
        public bool? Active { get; set; }

        public virtual ChiTietPhieuNhap IdctpnNavigation { get; set; }
        public virtual HangHoa IdhhNavigation { get; set; }
        public virtual PhieuXuat IdpxNavigation { get; set; }
    }
}
